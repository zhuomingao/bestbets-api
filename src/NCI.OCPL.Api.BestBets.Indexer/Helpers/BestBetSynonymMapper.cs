﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NCI.OCPL.Api.BestBets;

namespace NCI.OCPL.Api.BestBets.Indexer
{
    /// <summary>
    /// Provides mapping functions to map a CancerGovBestBet object
    /// to a collection of BestBetSynonym objects.
    /// </summary>
    public class BestBetSynonymMapper : IEnumerable<BestBetsMatch>
    {
        private CancerGovBestBet _bestBet;
        private readonly ITokenAnalyzerService _tokenAnalyzer = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The CancerGovBestBet which is being mapped to other objects.</param>
        public BestBetSynonymMapper(ITokenAnalyzerService tokenAnalyzer, CancerGovBestBet source)
        {
            _bestBet = source;
            _tokenAnalyzer = tokenAnalyzer;
        }

        public IEnumerator<BestBetsMatch> GetEnumerator()
        {
            // Create a list for the main category.
            var mainList =
                from item in new Array[1]
                select new { Synonym = _bestBet.Name, IsExact = _bestBet.IsExactMatch, IsNegated = false };

            // Create a list for each of the Include synonyms
            var includeList =
                from item in _bestBet.IncludeSynonyms
                select new { Synonym = item.Text, IsExact = item.IsExactMatch, IsNegated = false };

           // Create a list for each of the Exclude synonyms
           var excludeList=
                from item in _bestBet.ExcludeSynonyms
                select new { Synonym = item.Text, IsExact = item.IsExactMatch, IsNegated = true };

            // Combine the lists and return the actual BestBetMatch objects.
            // NOTE: The assumption is that there are no duplicates. Union would
            //       remove duplicates, but at the cost of additional processing.
            foreach(var item in mainList.Concat(includeList).Concat(excludeList))
            {
                int tokenCount = -1;

                try
                {
                    //TODO: Uh, do we need to clean the synonym?????????
                    tokenCount = _tokenAnalyzer.GetTokenCount(item.Synonym);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Error in getting token count for: " 
                        + _bestBet.Name + ", Synonym: ," + item.Synonym, ex);
                }

                // Return a new object each time rather than update an existing object
                // that might be referenced/used somewhere else.
                yield return new BestBetsMatch()
                {
                    Category = _bestBet.Name,
                    ContentID = _bestBet.ID,
                    Synonym = item.Synonym,
                    Language = _bestBet.Language,
                    IsNegated = item.IsNegated,
                    IsExact = item.IsExact,
                    TokenCount = tokenCount
                };
            }

        }

        /// <summary>
        /// Returns the IEnumerator<BestBetsMatch> GetEnumerator()"/>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

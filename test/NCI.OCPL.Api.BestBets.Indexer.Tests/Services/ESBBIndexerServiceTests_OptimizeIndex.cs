﻿
using Elasticsearch.Net;
using Nest;

using Xunit;
using Moq;

using System;
using System.Threading.Tasks;

using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Api.BestBets.Indexer.Services;
using Microsoft.Extensions.Options;

using NCI.OCPL.Api.BestBets.Indexer.Tests.Util;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests
{

    /// <summary>
    /// Class for testing ESBBIndexerService.OptimizeIndex
    /// </summary>
    /// <seealso cref="NCI.OCPL.Api.BestBets.Indexer.Tests.ESBBIndexerServiceTests__Base" />
    public class ESBBIndexerServiceTests_OptimizeIndex : ESBBIndexerServiceTests__Base
    {
        /// <summary>
        /// Tests for a successful optimization
        /// </summary>
        [Fact]
        public void OptimizedSucceeded()
        {
            string aliasName = "bestbets";
            string indexName = aliasName + "20161207125500";

            string actualPath = string.Empty;
            object actualRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
                aliasName,
                conn =>
                {

                    //This is the handler for the Optimize call
                    conn.RegisterRequestHandlerForType<Nest.ForceMergeResponse>((req, res) =>
                    {
                        //Store off Request
                        actualPath = req.Path;                        
                        actualRequestOptions = conn.GetRequestPost(req);

                        //Setup Response
                        res.StatusCode = 200;
                        res.Stream = TestingTools.GetTestFileAsStream("ForceMerge_Good_Response.json");
                    });
                }
            );

            bool succeeded = service.OptimizeIndex(indexName);

            //Make sure the Request is the expected request
            //Params for this are actual query params.
   
            // Hostname doesn't matter, just needed so we have an absolute Uri (relative Uri allows very few operations).

            Uri actualReqUri = new Uri(
                "http://localhost" + actualPath,
                UriKind.Absolute);


            Uri expectedUri = new Uri(
                "http://localhost" + indexName + "/_forcemerge?wait_for_merge=true&max_num_segments=1",
                UriKind.Absolute);

            //Check the path/parameters are as expected
            Assert.Equal(expectedUri, actualReqUri, new UriComparer());

            //Check that there was not request body
            Assert.Null(actualRequestOptions);

            //Technically, there is no condition where false would be returned as
            //an exception would be thrown.
            Assert.True(succeeded);
        }
    }
}

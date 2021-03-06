using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace NCI.OCPL.Api.BestBets.Tests
{

    /// <summary>
    /// A IEqualityComparer for IBestBetDisplay
    /// </summary>
    public class IBestBetDisplayComparer : IEqualityComparer<IBestBetDisplay>
    {
        public bool Equals(IBestBetDisplay x, IBestBetDisplay y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            bool isEqual =
                x.Name == y.Name
                && x.ID == y.ID
                && x.Weight == y.Weight
                && NormalizeString(x.HTML) == NormalizeString(y.HTML); //Special case where we will trim on comparison

            return isEqual;
        }

        /// <summary>
        /// Since we don't care about newlines for purposes of these tests, NormalizeString()
        /// removes leading and trailing whitspace along with carriage return (\r) and newline (\n)
        /// characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string NormalizeString(string text)
        {
            return text?.Trim().Replace("\r", null).Replace("\n", null);
        }

        public int GetHashCode(IBestBetDisplay obj)
        {
            int hash = 0;
            hash ^=
                obj.ID.GetHashCode()
                ^ obj.Name.GetHashCode()
                ^ obj.Weight.GetHashCode()
                ^ (obj.HTML?.Trim()).GetHashCode(); //Explicitly trim HTML as leading and trailing WS does not matter

            return hash;
        }

    }
}
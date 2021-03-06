using System.Collections.Generic;
using System.Linq;
using Xunit;
using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;
using System.Xml;
using System.IO;
using System;

namespace NCI.OCPL.Api.BestBets.Tests
{
    /// <summary>
    /// Tests for the CancerGovBestBet class, 
    /// and by extension the CancerGovBestBetSynonym class.
    /// These are really just deserialization tests,    
    /// </summary>
    public class CancerGovBestBetTest
    {
        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] { new PancoastTumorCategoryTestData() },
            new object[] { new BreastCancerCategoryTestData() }
        };

        [Theory, MemberData("XmlDeserializingData")]
        public void Can_Deserialize_XML(BaseCategoryTestData data) 
        {
            //Setup the expected object.
            CancerGovBestBet actCat = TestingTools.DeserializeXML<CancerGovBestBet>(data.TestFilePath);

            //Compare Object
            Assert.Equal(data.ExpectedData, actCat, new IBestBetCategoryComparer());
        }

        [Fact]
        public void GetSchema_ReturnsNull()
        {
            CancerGovBestBet bestBet = new CancerGovBestBet();

            Assert.Null(bestBet.GetSchema());
        }

        [Fact]
        public void WriteXML_ThrowsNotSupportedException()
        {
            CancerGovBestBet bestBet = new CancerGovBestBet();
            
            XmlWriter xmlWriter = XmlWriter.Create(new StringWriter());

            Assert.Throws<NotSupportedException>(() => bestBet.WriteXml(xmlWriter));
        }
    }
}
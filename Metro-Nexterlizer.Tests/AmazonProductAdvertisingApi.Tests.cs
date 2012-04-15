using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metro_Nexterlizer.Tests
{
    [TestClass]
    public class AmazonProductAdvertisingApiTests
    {
        [TestMethod]
        public void WhenConstructingShouldNotThrow()
        {
            new AmazonProductAdvertisingApi();
        }

        [TestMethod]
        public void WhenSigningWithHMACShouldReturnCorrectString()
        {
            var amz = new AmazonProductAdvertisingApi();
            var signedString = amz.CreateHMAC("TESTMESSAGE", "HMAC_SHA256", "SECRETKEY");
            Assert.AreEqual("wI+7tU9dsIrdBA6IwtXsvX5VJuBTHClD9THAea8M6UM=", signedString);
        }

        [TestMethod]
        public void WhenFlatteningQueryParamsShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            var amz = new AmazonProductAdvertisingApi();
            var flattenedQueryParams = amz.GetFlattenedParams(queryParams);
            Assert.AreEqual("A=B&C=D", flattenedQueryParams);
        }

        [TestMethod]
        public void WhenCallingGetStringToSignShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            var amz = new AmazonProductAdvertisingApi();
            Assert.AreEqual("GET\necs.amazonaws.co.uk\n/onca/xml\nA=B&C=D", amz.GetStringToSign(queryParams));
        }

        [TestMethod]
        public void WhenCallingGetCompleteUrlShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            var domain = "http://domain.com/x/y/";
            var hmac = "+HMAC=";

            var amz = new AmazonProductAdvertisingApi();
            Assert.AreEqual(domain + "A=B&C=D&Signature=%2bHMAC%3d", amz.GetCompleteUrl(domain, queryParams, hmac));
        }

        [TestMethod]
        public void WhenCallingGetQueryParamsShouldReturnDictionaryWithCorrectParams()
        {
            var amz = new AmazonProductAdvertisingApi();
            var expected = new SortedDictionary<string, string>();
            expected["Service"] = "AWSECommerceService";
            expected["AWSAccessKeyId"] = "ACCES KEY";
            expected["AssociateTag"] = "jamesthebloom-20";
            expected["Operation"] = "ItemSearch";
            expected["SearchIndex"] = "Books";
            expected["Keywords"] = "SEARCHTEXT";
            expected["ResponseGroup"] = "Similarities";
            expected["Timestamp"] = DateTime.Now.ToString(); // need  [YYYY-MM-DDThh:mm:ssZ]
            expected["Version"] = "2011-08-01";

            var queryParams = amz.GetQueryParams("SEARCHTEXT");
            Assert.IsTrue(queryParams.SequenceEqual(expected));
        }

        [TestMethod]
        public void UrlEncodeQueryParamsShouldEncodeValuesCorrectly()
        {
            var amz = new AmazonProductAdvertisingApi();
            var queryParams = new SortedDictionary<string, string>();
            queryParams["A"] = " ";
            queryParams["B"] = "+";
            queryParams["C"] = "=";
            var encodedQueryParams = amz.UrlEncodeQueryParams(queryParams);
            Assert.AreEqual("+", encodedQueryParams["A"]);
            Assert.AreEqual("%2b", encodedQueryParams["B"]);
            Assert.AreEqual("%3d", encodedQueryParams["C"]);
        }
    }
}

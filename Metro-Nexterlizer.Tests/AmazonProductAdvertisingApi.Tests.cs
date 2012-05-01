using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metro_Nexterlizer.Tests
{
    [TestClass]
    public class AmazonProductAdvertisingApiTests
    {
        public AmazonProductAdvertisingApi Amazon { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.Amazon = new AmazonProductAdvertisingApi("SECRETACCESSKEY", "ACCESSKEY");
        }

        [TestMethod]
        public void WhenConstructingShouldNotThrow()
        {
        }

        [TestMethod]
        public void WhenSigningWithHMACShouldReturnCorrectString()
        {
            var hmac = this.Amazon.CreateHMAC("TESTMESSAGE", "HMAC_SHA256", "SECRETKEY");
            var encoded = WebUtility.UrlEncode(hmac);
            Assert.AreEqual("wI%2B7tU9dsIrdBA6IwtXsvX5VJuBTHClD9THAea8M6UM%3D", encoded);
        }

        [TestMethod]
        public void WhenSigningWithHMACShouldReturnCorrectString2()
        {
            string source = "GET\nwebservices.amazon.com\n/onca/xml\nAWSAccessKeyId=AKIAIOSFODNN7EXAMPLE&ItemId=0679722769&Operation=ItemLookup&ResponseGroup=ItemAttributes%2COffers%2CImages%2CReviews&Service=AWSECommerceService&Timestamp=2009-01-01T12%3A00%3A00Z&Version=2009-01-06";
            var hmac = this.Amazon.CreateHMAC(source, "HMAC_SHA256", "1234567890");
            var encoded = WebUtility.UrlEncode(hmac);
            Assert.AreEqual("Nace%2BU3Az4OhN7tISqgs1vdLBHBEijWcBeCqL5xN9xg%3D", encoded);
            // M/y0+EAFFGaUAp4bWv/WEuXYah99pVsxvqtAuC8YN7I=
        }

        [TestMethod]
        public void WhenSigningWithHMACShouldReturnCorrectString3()
        {
            string source = "GET\necs.amazonaws.co.uk\n/onca/xml\nAWSAccessKeyId=AKIAIOSFODNN7EXAMPLE&Actor=Johnny%20Depp&AssociateTag=mytag-20&Operation=ItemSearch&ResponseGroup=ItemAttributes%2COffers%2CImages%2CReviews%2CVariations&SearchIndex=DVD&Service=AWSECommerceService&Sort=salesrank&Timestamp=2009-01-01T12%3A00%3A00Z&Version=2009-01-0";
            var hmac = this.Amazon.CreateHMAC(source, "HMAC_SHA256", "1234567890");
            var encoded = WebUtility.UrlEncode(hmac);
            Assert.AreEqual("TuM6E5L9u%2FuNqOX09ET03BXVmHLVFfJIna5cxXuHxiU%3D", encoded);
        }

        [TestMethod]
        public void WhenSigningWithHMAC2ShouldReturnCorrectString()
        {
            string source = "GET\necs.amazonaws.co.uk\n/onca/xml\nAWSAccessKeyId=AKIAIOSFODNN7EXAMPLE&Actor=Johnny%20Depp&AssociateTag=mytag-20&Operation=ItemSearch&ResponseGroup=ItemAttributes%2COffers%2CImages%2CReviews%2CVariations&SearchIndex=DVD&Service=AWSECommerceService&Sort=salesrank&Timestamp=2009-01-01T12%3A00%3A00Z&Version=2009-01-0";
            var hmac = this.Amazon.CreateHMAC("AKIAIOSFODNN7EXAMPLE", "1234567890", source);
            var encoded = WebUtility.UrlEncode(hmac);
            Assert.AreEqual("TuM6E5L9u%2FuNqOX09ET03BXVmHLVFfJIna5cxXuHxiU%3D", encoded);
        }

        [TestMethod]
        public void WhenFlatteningQueryParamsShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            var flattenedQueryParams = this.Amazon.GetFlattenedParams(queryParams);
            Assert.AreEqual("A=B&C=D", flattenedQueryParams);
        }

        [TestMethod]
        public void WhenCallingGetStringToSignShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            Assert.AreEqual("GET\necs.amazonaws.co.uk\n/onca/xml\nA=B&C=D", this.Amazon.GetStringToSign(queryParams));
        }

        [TestMethod]
        public void WhenCallingGetCompleteUrlShouldReturnCorrectString()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams["A"] = "B";
            queryParams["C"] = "D";

            var domain = "http://domain.com/x/y/";
            var hmac = "+HMAC=";

            Assert.AreEqual(domain + "A=B&C=D&Signature=%2bHMAC%3d", this.Amazon.GetCompleteUrl(domain, queryParams, hmac));
        }

        [TestMethod]
        public void WhenCallingGetQueryParamsShouldReturnDictionaryWithCorrectParams()
        {
            var expected = new SortedDictionary<string, string>();
            expected["Service"] = "AWSECommerceService";
            expected["AWSAccessKeyId"] = "ACCESSKEY";
            expected["AssociateTag"] = "jamesthebloom-20";
            expected["Operation"] = "ItemSearch";
            expected["SearchIndex"] = "Books";
            expected["Keywords"] = "SEARCHTEXT";
            expected["ResponseGroup"] = "Similarities";
            var now = DateTime.UtcNow;
            this.Amazon.Now = () => { return now; };
            expected["Timestamp"] = now.ToString("yyyy-MM-ddTHH:mm:ssZ"); 
            expected["Version"] = "2011-08-01";

            var queryParams = this.Amazon.GetQueryParams("SEARCHTEXT");
            Assert.IsTrue(queryParams.SequenceEqual(expected));
        }

        [TestMethod]
        public void UrlEncodeQueryParamsShouldEncodeValuesCorrectly()
        {
            var queryParams = new SortedDictionary<string, string>();
            queryParams["A"] = " ";
            queryParams["B"] = "+";
            queryParams["C"] = "=";
            var encodedQueryParams = this.Amazon.UrlEncodeQueryParams(queryParams);
            Assert.AreEqual("+", encodedQueryParams["A"]);
            Assert.AreEqual("%2b", encodedQueryParams["B"]);
            Assert.AreEqual("%3d", encodedQueryParams["C"]);
        }

        [TestMethod]
        public void GetQueryParamsShouleReturnCorrectParams()
        {
            var now = new DateTime(2012, 4, 18, 22, 36, 3);
            this.Amazon.Now = () => { return now; };

            var queryParams = this.Amazon.GetQueryParams("SEARCHTEXT");
            Assert.AreEqual("AWSECommerceService", queryParams["Service"]);
            Assert.AreEqual("ACCESSKEY", queryParams["AWSAccessKeyId"]);
            Assert.AreEqual("jamesthebloom-20", queryParams["AssociateTag"]);
            Assert.AreEqual("ItemSearch", queryParams["Operation"]);
            Assert.AreEqual("Books", queryParams["SearchIndex"]);
            Assert.AreEqual("SEARCHTEXT", queryParams["Keywords"]);
            Assert.AreEqual("Similarities", queryParams["ResponseGroup"]);
            Assert.AreEqual(now.ToString("2012-04-18T22:36:03Z"), queryParams["Timestamp"]); 
            Assert.AreEqual("2011-08-01", queryParams["Version"]);
        }

        [TestMethod]
        public void UppercaseUrlEncodeShouldCorrectlyUppercaseEncodedCharacters()
        {
            Assert.AreEqual("abc%2Cdef", this.Amazon.UppercaseUrlEncode("abc%2cdef"));
            Assert.AreEqual("%2Cdef", this.Amazon.UppercaseUrlEncode("%2cdef"));
            Assert.AreEqual("abc%2C", this.Amazon.UppercaseUrlEncode("abc%2c"));
            Assert.AreEqual("abc2cdef", this.Amazon.UppercaseUrlEncode("abc2cdef"));
        }
    }
}

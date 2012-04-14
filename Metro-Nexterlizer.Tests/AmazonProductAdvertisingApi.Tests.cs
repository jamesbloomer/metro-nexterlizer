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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;

namespace TopDown.Core._Testing
{
    [TestClass]
    public class JsonReaderTester
    {
        [TestMethod]
        public void TrivialObjectTest()
        {
            var json = "{}";
            using (var reader = new JsonReader(new JsonTextReader(new StringReader(json))))
            {
                reader.Read(delegate
                {
                });
            }
        }
        
        [TestMethod]
        public void ObjectWithPropertyObjectTest()
        {
            var json = "{a: {}}";
            using (var reader = new JsonReader(new JsonTextReader(new StringReader(json))))
            {
                reader.Read(delegate
                {
                    reader.Read("a", delegate
                    {
                    });
                });
            }
        }

        [TestMethod]
        public void DecimalPropertyTest()
        {
            var json = "{a: 0.1234}";
            using (var reader = new JsonReader(new JsonTextReader(new StringReader(json))))
            {
                reader.Read(delegate
                {
                    Assert.AreEqual(0.1234m, reader.ReadAsDecimal("a"));
                });
            }
        }

        [TestMethod]
        public void DecimalNullablePropertyTest()
        {
            var json = "{a: null}";
            using (var reader = new JsonReader(new JsonTextReader(new StringReader(json))))
            {
                reader.Read(delegate
                {
                    Assert.AreEqual(null, reader.ReadAsNullableDecimal("a"));
                });
            }
        }
    }
}

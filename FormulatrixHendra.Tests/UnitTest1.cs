using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulatrixHendra.Data.Repositories;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FormulatrixHendra.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public class SampleData
        {
            public string Data { get; set; }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var repo = new DataRepository();
            var data = new SampleData
            {
                Data = "data1"
            };

            var jsonContent = JsonConvert.SerializeObject(data);


            var xmlContent = string.Empty;
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(stringwriter, data);
                xmlContent = stringwriter.ToString();
            }

            repo.Register("item1", jsonContent, 1);

            //simulate overwritting
            data.Data = "data2";
            jsonContent = JsonConvert.SerializeObject(data);

            var task1 = Task.Factory.StartNew(() => repo.Register("item1", jsonContent, 1));
            var task2 = Task.Factory.StartNew(() => repo.Register("item2", xmlContent, 2));

            Task.WaitAll(task1, task2);

            var itemContent = repo.Retrieve("item1");
            Assert.IsNotNull(itemContent);
            //check whether got overwritten 
            Assert.IsTrue(itemContent.Contains("data1"));

            itemContent = repo.Retrieve("item2");
            Assert.IsNotNull(itemContent);

            repo.Deregister("item1");
            itemContent = repo.Retrieve("item1");
            Assert.IsNull(itemContent);
        }
    }
}

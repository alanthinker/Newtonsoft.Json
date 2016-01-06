using System;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Tests
{
    public class DollarQuoteStringTest
    {
        [Test]
        public void DoNotIndentPrimitiveArray()
        {
            dynamic d = new JObject();
            d.name = "T'om";
            d.items = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            d.items2 = new JArray("a", "bc",
                new JObject() { new JProperty("aa", "bb"), new JProperty("aa2", "bb"), new JProperty("aa3", "bb") },
                "def");

            JsonSerializer js = new JsonSerializer();
            StringBuilder sb = new StringBuilder();
            using (var stringW = new StringWriter(sb))
            using (var jw = new JsonTextWriter(stringW))
            {
                jw.DollarTag = "";
                jw.Formatting = Formatting.Indented;
                jw.IsIndentPrimitiveArray = false;
                js.Serialize(jw, d);
                var str = sb.ToString().Replace(Environment.NewLine, "\n");
                var expected = @"{
  ""name"": ``T'om``,
  ""items"": [
    1, 2, 3, 4, 5, 6, 7, 8, 9
  ],
  ""items2"": [
    ``a``, ``bc``,
    {
      ""aa"": ``bb``,
      ""aa2"": ``bb``,
      ""aa3"": ``bb``
    },
    ``def``
  ]
}".Replace(Environment.NewLine,"\n");
                Assert.AreEqual(expected, str);
            }
        }

        [Test]
        public void ReadDollarQuoteString()
        {
            string json = @"{""NameOfStore"":$$Forest's Bakery And Cafe$$}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }

        [Test]
        public void ReadDollarQuoteStringWithTag()
        {
            string json = @"{""NameOfStore"":$tag$Forest's Bakery And Cafe$tag$}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }

        [Test]
        public void ReadDollarQuotePropertyWithTag()
        {
            string json = @"{$pp$Name'Of""Store$pp$:$tag$Forest's Bakery And Cafe$tag$}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            Assert.AreEqual(@"Name'Of""Store", jsonTextReader.Value);
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }

        [Test]
        public void ReadDollarQuoteString2()
        {
            string json = @"{""NameOfStore"":``Forest's Bakery And Cafe``}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }

        [Test]
        public void ReadDollarQuoteStringWithTag2()
        {
            string json = @"{""NameOfStore"":`tag`Forest's Bakery And Cafe`tag`}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }

        [Test]
        public void ReadDollarQuotePropertyWithTag2()
        {
            string json = @"{`pp`Name'Of""Store`pp`:`tag`Forest's Bakery And Cafe`tag`}";

            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(json));
            jsonTextReader.Read();
            jsonTextReader.Read();
            Assert.AreEqual(@"Name'Of""Store", jsonTextReader.Value);
            jsonTextReader.Read();

            Assert.AreEqual(@"Forest's Bakery And Cafe", jsonTextReader.Value);
        }


        [Test]
        public void ReadSomeDollarQuoteString()
        {
            {
                string str = "{``name``:`'`Tom`'`}";
                dynamic d = JsonConvert.DeserializeObject(str);
                Assert.AreEqual("Tom", d.name.Value);
            }
            {
                string str = "{``name``:`'`T`om`'`}";
                dynamic d = JsonConvert.DeserializeObject(str);
                Assert.AreEqual("T`om", d.name.Value);
            }
            {
                string str = "{``name``:`'`T``om`'`}";
                dynamic d = JsonConvert.DeserializeObject(str);
                Assert.AreEqual("T``om", d.name.Value);
            }
            {
                string str = "{``name``:`''`T`'`om`''`}";
                dynamic d = JsonConvert.DeserializeObject(str);
                Assert.AreEqual("T`'`om", d.name.Value);
            }
        }

        [Test]
        public void WriteDollarQuoteJson()
        {
            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T'om";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$$T'om$$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T\"om";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$$T\"om$$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To$m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$$To$m$$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To$$m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$'$To$$m$'$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To$$``m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$'$To$$``m$'$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To$'$`'`m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$$To$'$`'`m$$}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T$$o$'$`'`m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarType = '$';
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":$''$T$$o$'$`'`m$''$}", str);
                }
            }

            {
                Assert.Catch(delegate ()
                {
                    dynamic d = new Json.Linq.JObject();
                    d.name = "To``$$m";
                    JsonSerializer js = new JsonSerializer();
                    StringBuilder sb = new StringBuilder();
                    using (var stringW = new StringWriter(sb))
                    using (var jw = new JsonTextWriter(stringW))
                    {
                        jw.DollarType = '$';
                        jw.DollarTag = "$";
                        js.Serialize(jw, d);
                        var str = sb.ToString();
                        Assert.AreEqual("{\"name\":`'`To``$$m`'`}", str);
                    }
                }
                , "Invalid DollarTag or DollarType. Can't be same.");
            }

        }

        [Test]
        public void WriteDollarQuoteJson2()
        {
            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T'om";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":``T'om``}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T\"om";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":``T\"om``}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To`m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual( "{\"name\":``To`m``}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To``$$m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":`'`To``$$m`'`}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "To`'`$'$m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":``To`'`$'$m``}", str);
                }
            }

            {
                dynamic d = new Json.Linq.JObject();
                d.name = "T``o`'`$'$m";
                JsonSerializer js = new JsonSerializer();
                StringBuilder sb = new StringBuilder();
                using (var stringW = new StringWriter(sb))
                using (var jw = new JsonTextWriter(stringW))
                {
                    jw.DollarTag = "";
                    js.Serialize(jw, d);
                    var str = sb.ToString();
                    Assert.AreEqual("{\"name\":`''`T``o`'`$'$m`''`}", str);
                }
            }

            {
                Assert.Catch(delegate ()
                {
                    dynamic d = new Json.Linq.JObject();
                    d.name = "To``$$m";
                    JsonSerializer js = new JsonSerializer();
                    StringBuilder sb = new StringBuilder();
                    using (var stringW = new StringWriter(sb))
                    using (var jw = new JsonTextWriter(stringW))
                    {
                        jw.DollarTag = "`";
                        js.Serialize(jw, d);
                        var str = sb.ToString();
                        Assert.AreEqual("{\"name\":`'`To``$$m`'`}", str);
                    }
                }
                , "Invalid DollarTag or DollarType. Can't be same.");
            }
        }

        [Test]
        public void ReadAndWriteDollarQuoteJson()
        {

            dynamic d;
            using (var jsonFile = System.IO.File.OpenText("myTest.pjson"))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
            {
#if DEBUG
                jsonTextReader.SetCharBuffer(new char[2]);
#endif

                JsonSerializer serializer = new JsonSerializer();
                d = serializer.Deserialize(jsonTextReader);
            }

            //按照特定格式输出json文件.
            JsonSerializer js = new JsonSerializer();
            using (var fileWriter = new StreamWriter("myTest2.pjson"))
            using (JsonTextWriter jw = new JsonTextWriter(fileWriter))
            {
                //jw.QuoteName = false;//输出时不用双引号.
                jw.Formatting = Formatting.Indented;//格式化, 换行并缩进.
                jw.DollarTag = "'";
                js.Serialize(jw, d);
            }

            //读取刚才的文件.
            using (var jsonFile = System.IO.File.OpenText("myTest2.pjson"))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                d = serializer.Deserialize(jsonTextReader);
                d.Tasks[2].LastActiveTime = DateTime.Parse("2015-09-23T15:41:03.8329401+08:00");
                d.Tasks[2].Description = "用户若干分钟没有动鼠标或者键盘, 则强行睡眠. (windows自带的自动睡眠功能有时候无效.)";
            }
        }

        [Test]
        public void ReadAndWriteLargeJson()
        {
            dynamic d;
            using (var jsonFile = System.IO.File.OpenText("large.json"))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
            {
#if DEBUG
                jsonTextReader.SetCharBuffer(new char[7]);
#endif
                JsonSerializer serializer = new JsonSerializer();
                d = serializer.Deserialize(jsonTextReader);
            }

            //按照特定格式输出json文件.
            JsonSerializer js = new JsonSerializer();
            using (var fileWriter = new StreamWriter("large.pjson"))
            using (JsonTextWriter jw = new JsonTextWriter(fileWriter))
            {
                //jw.QuoteName = false;//输出时不用双引号.
                jw.Formatting = Formatting.Indented;//格式化, 换行并缩进.
                jw.DollarTag = "";
                js.Serialize(jw, d);
            }

            //读取刚才的文件.
            using (var jsonFile = System.IO.File.OpenText("large.pjson"))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                d = serializer.Deserialize(jsonTextReader);
            }
        }
    }
}

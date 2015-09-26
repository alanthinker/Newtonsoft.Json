using System;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Text;
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
                    Assert.AreEqual(str, "{\"name\":$$T'om$$}");
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
                    Assert.AreEqual(str, "{\"name\":$$T\"om$$}");
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
                    Assert.AreEqual(str, "{\"name\":$$To$m$$}");
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
                    Assert.AreEqual(str, "{\"name\":$'$To$$m$'$}");
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
                    Assert.AreEqual(str, "{\"name\":$'$To$$``m$'$}");
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
                        Assert.AreEqual(str, "{\"name\":`'`To``$$m`'`}");
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
                    Assert.AreEqual(str, "{\"name\":``T'om``}");
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
                    Assert.AreEqual(str, "{\"name\":``T\"om``}");
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
                    Assert.AreEqual(str, "{\"name\":``To`m``}");
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
                    Assert.AreEqual(str, "{\"name\":`'`To``$$m`'`}");
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
                        Assert.AreEqual(str, "{\"name\":`'`To``$$m`'`}");
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

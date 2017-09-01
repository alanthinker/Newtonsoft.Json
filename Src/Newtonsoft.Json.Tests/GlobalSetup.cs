using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace Newtonsoft.Json.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    { 
        public GlobalSetup()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(this.GetType().Assembly.Location));
        }
    }
}

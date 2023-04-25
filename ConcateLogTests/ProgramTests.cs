using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcatFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcatFiles.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void ParseBySepTest()
        {
       
            var line = "text1 text2 text3 text4 text5 text6";

            line = Program.ParseBySep(line, sep: " ", getIndexs: new int[] { 0, 2 });

            Assert.AreEqual("text1\ttext3", line);
        }
    }
}
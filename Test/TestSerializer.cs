using System;
using System.Collections.Generic;
using System.Linq;
using JsonTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class TestSerializer
    {
        [TestMethod]
        public void ToJson()
        {
            // Arrange
            const string sIn = "{ a: 'a', b: 1, c: true, d: 1.11, e: [ 'e1', 'e2' ], f: { f1: 'f1', f2: 'f2' } }";
            var root = new JsonTree.Node(sIn);

            // Act
            string sOut = root.ToJson();

            // Assert
            Assert.IsFalse(String.IsNullOrEmpty(sOut));
        }

        [TestMethod]
        public void ToStringCreatesHumanReadableJsNotJsonScrewDoubleQuotes()
        {
            // Arrange
            const string sIn = "{ a: 'b\\'c b\"c', b: 1, c: true, d: false, e: 1.11, f: [ 'g', 'h' ], i: { j: 'k', l: 2, m: [ 1, 2 ], n: { o: 3, p: 4, q: { r: 's', t: [ 1, 2, 3 ], u: [ { v: 1, w: 2 }, { x: 3, y: 4 } ] } } } }";
            var root = new JsonTree.Node(sIn);

            // Act
            string sOut = root.ToString();

            // Assert
            Assert.AreEqual(sIn, sOut);
        }

        [TestMethod]
        public void ToJsonDefaultCreatesDoubleQuotesAndQuotedKeysAndNoFormatting()
        {
            // Arrange
            const string sIn = "{\"a\":\"a\",\"b\":1,\"c\":true,\"d\":1.11,\"e\":[\"e1\",\"e2\"],\"f\":{\"f1\":\"f1\",\"f2\":\"f2\"}}";
            var root = new JsonTree.Node(sIn);

            // Act
            string sOut = root.ToJson();

            // Assert
            Assert.AreEqual(sIn, sOut);
        }

        [TestMethod]
        public void SeralizeWithAddedNode()
        {
            // Arrange
            const string sIn = @"
[
  {
    aInt: 41,
    bBool: true,
    cLong: 42000000000,
    dString: '43',
    eFloat: 3.14159265358979323
  },
  {
    fInt: 44,
    gLong: 45000000000,
    hString: ""46""
  },
  {
    iList:
    [
      {
        jInt: 47,
        kString: 'true'
      },
      {
        lInt: 49,
        mString: '50'
      }
    ],
    nMap:
    {
      oInt: 51,
      pString: '52'
    }
  }
]
";
            var root = new JsonTree.Node(sIn);

            // Act
            root.List.ElementAt(2).Dictionary.Add("new child", new JsonTree.Node(JsonTree.Node.Type.String) { Value = "new string" });

            // Assert
            string sOut = root.ToJson();
            Assert.IsTrue(sOut.Contains("\"new child\":\"new string\""));
            var check = new JsonTree.Node(sOut);
            Assert.AreEqual(check.List.ElementAt(2).Dictionary["new child"].String, "new string");
        }

    }
}

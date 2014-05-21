using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonTree;

namespace Test
{
  [TestClass]
  public class TestInterop
  {
    [TestMethod]
    public void DeserializerReadsSerializerOutput()
    {
      // Arrange
      const string sIn = "{ a: 'b\\'c b\"c', b: 1, c: true, d: false, e: 1.11, f: ['g', 'h'], i:{ j:'k', l: 2, m: [1, 2], n:{o:3, p:4, q:{r:'s',t:[1,2,3], u:[{v:1,w:2},{x:3,y:4}]}} } }";
      var root = new JsonTree.Node(sIn);

      // Act
      string sOut = root.ToJson();

      // Assert
      var check = new JsonTree.Node(sOut);
      Assert.IsNotNull(check);
    }

    [TestMethod]
    public void ReadWriteStandardQuotation()
    {
      // Arrange
      const string sIn = "{\"a\":[[{\"b\":[{\"c\":[{\"d\":0},{\"e\":[{\"d\":42},{\"f\":[{\"g\":[{\"h\":[{\"g\":[\"i\"]}]}]}]}]}]}],\"j\":{\"k\":[\"l\"]}}]]}";
      const string sExpectedOut = sIn;

      // Act
      var root = new JsonTree.Node(sIn);
      string sOut = root.ToJson();

      Assert.AreEqual(sOut, sExpectedOut);
    }

  }
}

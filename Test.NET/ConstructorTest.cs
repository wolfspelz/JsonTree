using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonTree;

namespace Test
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void AddNodes()
        {
            // Arrange
            // Act
            var node = new Node(Node.Type.Dictionary);
            node.AsDictionary.Add("a", new Node(Node.Type.String, "fourtytwo"));
            node.AsDictionary.Add("b", new Node(Node.Type.Int, 42));
            node.AsDictionary.Add("c", new Node(Node.Type.List, new List { new Node(Node.Type.Bool, true), new Node(Node.Type.Float, 3.14) }));
            string sJson = node.ToJson();

            // Assert
            Assert.AreEqual(new Node(sJson).Dictionary["a"].String, "fourtytwo");
            Assert.AreEqual(new Node(sJson).Dictionary["b"].Int, 42);
            Assert.AreEqual(new Node(sJson).Dictionary["c"].AsList[0].Bool, true);
            Assert.AreEqual(new Node(sJson).Dictionary["c"].AsList[1].Float, 3.14);
        }

        [TestMethod]
        public void StringConstructor()
        {
            var root = new Node("42");
        }

    }
}

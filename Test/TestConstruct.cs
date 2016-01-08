using System;
using System.Collections.Generic;
using System.Linq;
using JsonTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class TestConstruct
    {
        [TestMethod]
        public void AddNodes()
        {
            // Arrange
            // Act
            var node = new JsonTree.Node(JsonTree.Node.Type.Dictionary);
            node.AsDictionary.Add("a", new JsonTree.Node(JsonTree.Node.Type.String, "fourtytwo"));
            node.AsDictionary.Add("b", new JsonTree.Node(JsonTree.Node.Type.Int, 42));
            node.AsDictionary.Add("c", new JsonTree.Node(JsonTree.Node.Type.List, new List<JsonTree.Node> { new JsonTree.Node(JsonTree.Node.Type.Bool, true), new JsonTree.Node(JsonTree.Node.Type.Float, 3.14) }));
            string sJson = node.ToJson();

            // Assert
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["a"].String, "fourtytwo");
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["b"].Int, 42);
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["c"].AsList[0].Bool, true);
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["c"].AsList[1].Float, 3.14);
        }

        [TestMethod]
        public void DictionaryStringStringContructor()
        {
            // Arrange
            // Act
            var node = new JsonTree.Node(new Dictionary<string, string> { {"a", "fourtytwo" }, { "b", "42" }});
            string sJson = node.ToJson();

            // Assert
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["a"].String, "fourtytwo");
            Assert.AreEqual(new JsonTree.Node(sJson).Dictionary["b"].String, "42");
        }

        [TestMethod]
        public void StringConstructor()
        {
            var root = new JsonTree.Node("42");
        }

        [TestMethod]
        public void DeserializeBasicTypes()
        {
            Assert.IsTrue(new JsonTree.Node("").IsEmpty);
            Assert.IsTrue(new JsonTree.Node("41").IsInt);
            Assert.IsTrue(new JsonTree.Node("true").IsBool);
            Assert.IsTrue(new JsonTree.Node("false").IsBool);
            Assert.IsTrue(new JsonTree.Node("'41'").IsString);
            Assert.IsTrue(new JsonTree.Node("41000000000").IsInt);
            Assert.IsTrue(new JsonTree.Node("3.14159265358979323").IsFloat);
            // Assert.IsTrue(new JsonTree.Node(".42").IsFloat); // not on mono
            Assert.IsTrue(new JsonTree.Node("{}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{a:41}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{a:'41'}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{a:41000000000}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{a:3.1415927}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{a:3.14159265358979323}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{'a':41}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{'a':'41'}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("{\"a\":\"41\"}").IsDictionary);
            Assert.IsTrue(new JsonTree.Node("[]").IsList);
            Assert.IsTrue(new JsonTree.Node("['a']").IsList);
            Assert.IsTrue(new JsonTree.Node("['a','b']").IsList);
            Assert.IsTrue(new JsonTree.Node("[41,42]").IsList);
            Assert.IsTrue(new JsonTree.Node("['a',42]").IsList);
            Assert.AreEqual(new JsonTree.Node("['a',42]").List.First().String, "a");
            Assert.AreEqual(new JsonTree.Node("['a',42]").List.ElementAt(1).Int, 42);
            Assert.IsTrue(new JsonTree.Node("{a:41}").Dictionary.First().Value.IsInt);
            Assert.IsTrue(new JsonTree.Node("{a:'41'}").Dictionary.First().Value.IsString);
            Assert.IsTrue(new JsonTree.Node("{a:41000000000}").Dictionary.First().Value.IsInt);
            Assert.IsTrue(new JsonTree.Node("{a:3.1415927}").Dictionary.First().Value.IsFloat);
            Assert.IsTrue(new JsonTree.Node("{a:3.14159265358979323}").Dictionary.First().Value.IsFloat);
            //Assert.IsTrue(new JsonTree.Node("{a:.42}").Map.First().Value.IsFloat); // not on mono
            Assert.AreEqual(new JsonTree.Node("{a:41}").Dictionary.First().Value.Int, 41);
            Assert.AreEqual(new JsonTree.Node("{a:41}").Dictionary.First().Value.Float, 41);
            Assert.AreEqual(new JsonTree.Node("{a:'41'}").Dictionary.First().Value.String, "41");
            Assert.AreEqual(new JsonTree.Node("{a:41000000000}").Dictionary.First().Value.Int, 41000000000);
            Assert.AreEqual(new JsonTree.Node("{a:3.1415927}").Dictionary.First().Value.Float, 3.1415927);
            Assert.AreEqual(new JsonTree.Node("{a:3.1415927}").Dictionary["a"].Float, 3.1415927);
            Assert.AreEqual(new JsonTree.Node("{a:3.14159265358979323}").Dictionary.First().Value.Float, 3.14159265358979323);
            Assert.AreEqual(new JsonTree.Node("41").String, "41");
            Assert.AreEqual(new JsonTree.Node("41000000000").String, "41000000000");
            Assert.AreEqual(new JsonTree.Node("3.14159265358979").String, "3.14159265358979");
            Assert.AreEqual(new JsonTree.Node("3.14159265358979").Float, 3.14159265358979);
        }
    }
}

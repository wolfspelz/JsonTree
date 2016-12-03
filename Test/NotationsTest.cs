using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonTree;

namespace Test
{
    [TestClass]
    public class NotationsTest
    {
        [TestMethod]
        public void Skalars()
        {
            // Arrange
            const string sIn = "{ a: '41', b: 42, c: true, d: 3.14 }";

            // Act
            var root = new Node(sIn);

            // Assert
            Assert.AreEqual(root.AsDictionary["a"].String, "41");
            Assert.AreEqual(root.AsDictionary["a"].AsString, "41");
            Assert.AreEqual(root.AsDictionary["b"].Int, 42);
            Assert.AreEqual(root.AsDictionary["b"].AsInt, 42);
            Assert.AreEqual(root.AsDictionary["c"].Bool, true);
            Assert.AreEqual(root.AsDictionary["c"].AsBool, true);
            Assert.AreEqual(root.AsDictionary["d"].Float, 3.14);
            Assert.AreEqual(root.AsDictionary["d"].AsFloat, 3.14);
        }

        [TestMethod]
        public void Structures()
        {
            // Arrange
            const string sIn = "{ a: 'a', b: [ 'b0', 'b1' ] }";

            // Act
            var root = new Node(sIn);

            // Assert
            Assert.AreEqual(root.Object["b"].Array[0].String, "b0");
            Assert.AreEqual(root.Dictionary["b"].List[0].String, "b0");
            Assert.AreEqual(root.AsObject["b"].AsArray[0].String, "b0");
            Assert.AreEqual(root.AsDictionary["b"].AsList[0].String, "b0");
        }

    }
}

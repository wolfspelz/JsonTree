using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonTree;

namespace Test
{
    [TestClass]
    public class TestDeserializer
    {
        [TestMethod]
        public void NodeConstructor()
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

        [TestMethod]
        public void FloatFromStringWithInvariantCulture()
        {
            Assert.AreEqual(new JsonTree.Node("{\"a\":\"3.14159265358979323\"}").Dictionary.First().Value.Float, 3.14159265358979323);
        }

        [TestMethod]
        public void DeserializeTypicalJson()
        {
            var sJson = @"
[
  {
    aInt: 41,
    bBool: true,
    bLong: 42000000000,
    cString: '43',
    dFloat: 3.14159265358979323
  },
  {
    aInt: 44,
    bLong: 45000000000,
    cString: ""46""
  },
  {
    aList:
    [
      {
        aInt: 47,
        bString: '48'
      },
      {
        aInt: 49,
        bString: '50'
      }
    ],
    bMap:
    {
      aInt: 51,
      bString: '52'
    }
  }
]
";
            // Act
            var root = new JsonTree.Node(sJson);

            // Assert
            Node lastListItem = null;
            string lastMapItemKey = null;
            Node lastMapItemValue = null;
            foreach (var item in root.List) {
                lastListItem = item;
            }
            foreach (var pair in root.List.First().Dictionary) {
                lastMapItemKey = pair.Key;
                lastMapItemValue = pair.Value;
            }

            Assert.AreEqual(lastListItem.Dictionary.Count, 2);
            Assert.AreEqual(lastMapItemKey, "dFloat");
            Assert.AreEqual(lastMapItemValue.Float, 3.14159265358979323);

            // Act/Assert
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(0).Value.Int, 41);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(1).Key, "bBool");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(1).Value.Bool, true);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(2).Key, "bLong");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(2).Value.Int, 42000000000);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(3).Key, "cString");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(3).Value.String, "43");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(4).Key, "dFloat");

            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(0).Dictionary.ElementAt(4).Value.Float, 3.14159265358979323);
            Assert.AreEqual(new JsonTree.Node(sJson).List[0].Dictionary.ElementAt(4).Value.Float, 3.14159265358979323);
            Assert.AreEqual(new JsonTree.Node(sJson).Array[0].Object.ElementAt(4).Value.Float, 3.14159265358979323);

            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(0).Value.Int, 44);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(1).Key, "bLong");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(1).Value.Int, 45000000000);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(2).Key, "cString");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(1).Dictionary.ElementAt(2).Value.String, "46");

            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Key, "aList");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.Count, 2);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Value.Int, 47);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Key, "bString");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Value.String, "48");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Value.Int, 49);
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Key, "bString");

            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value.String, "50");
            Assert.AreEqual(new JsonTree.Node(sJson).List[2].Dictionary["aList"].List[1].Dictionary["bString"].String, "50");
            Assert.AreEqual(new JsonTree.Node(sJson).Array[2].Object["aList"].Array[1].Object["bString"].String, "50");

            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(1).Key, "bMap");
            Assert.AreEqual(new JsonTree.Node(sJson).List.ElementAt(2).Dictionary.ElementAt(1).Value.Dictionary.Count, 2);
        }

        [TestMethod]
        public void JsonRecursionExceeds100()
        {
            // Arrange
            const string sIn = "{'a':[{'b':1},{'c':[{'b':1.83},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'j','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'f','g':'h','i':'_'},{'d':[{'e':'j','g':'t','i':'_'},{'k':[{'q':[{'p':[{'q':['uDE']},{'q':['s8','s10','s16']},{'q':['t0','t1','t3','t4','t5','t7','t10','t11','t1']},{'q':['va']},{'p':[{'r':'xw'}]},{'p':[{'r':'aa1'}]}]}],'o':{'n':['m']}}],'o':{'n':['m']}}],'o':{'n':['m','145']}}],'o':{'n':['m','151']}}],'o':{'n':['m','137']}}],'o':{'n':['m','155']}}],'o':{'n':['m','156']}}],'o':{'n':['m','143']}}],'o':{'n':['m','131']}}],'o':{'n':['m','165']}}],'o':{'n':['m','170']}}],'o':{'n':['m','148']}}],'o':{'n':['m','144']}}],'o':{'n':['m','138']}}],'o':{'n':['m','150']}}],'o':{'n':['m','158']}}],'o':{'n':['m','146']}}],'o':{'n':['m','159']}}],'o':{'n':['m','147']}}],'o':{'n':['m','161']}}],'o':{'n':['m','163']}}],'o':{'n':['m','13']}}],'o':{'n':['m','154']}}],'o':{'n':['m','130']}}],'o':{'n':['m','141']}}],'o':{'n':['m','136']}}],'o':{'n':['m','139']}}],'o':{'n':['m','167']}}],'o':{'n':['m','135']}}],'o':{'n':['m','168']}}],'o':{'n':['m','160']}}],'o':{'n':['m','171']}}],'o':{'n':['m','19']}}],'o':{'n':['m','14']}}],'o':{'n':['m','166']}}],'o':{'n':['m','157']}}],'o':{'n':['m','15']}}],'o':{'n':['m','134']}}],'o':{'n':['m','153']}}],'o':{'n':['m','164']}}],'o':{'n':['m','133']}}],'o':{'n':['m','169']}}],'o':{'n':['m','140']}}],'o':{'n':['m','149']}}],'o':{'n':['m','16']}}],'o':{'n':['m','17']}}],'o':{'n':['m']}}],'o':{'n':['m','l']}}";
            const string sExpectedOut = sIn;

            // Act
            var root = new JsonTree.Node(sIn, new JsonTree.Deserializer.Options { RecursionLimit = 110 });
            string sOut = root.ToJson(new JsonTree.Serializer.Options { EncapsulateKeys = "'", EncapsulateStrings = "'" });

            // Assert
            Assert.AreEqual(sOut, sExpectedOut);
        }

    }
}

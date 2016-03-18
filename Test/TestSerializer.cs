using System;
using System.Collections.Generic;
using System.Linq;
using JsonTree;
using System.Xml;
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
        public void ArrayOfDictionary_ToJson()
        {
            // Arrange
            const string sIn = "[ { 'param': { 'name': 'defaultsequence', 'value': 'idle' }}, { 'sequence': { 'group': 'idle', 'name': 'still', 'type': 'status', 'probability': '1000', 'in': 'standard', 'out': 'standard', 'src': 'idle.gif' }} ]";
            var root = new JsonTree.Node(sIn);

            // Act
            string sOut = root.ToJson();

            // Assert
            Assert.IsFalse(String.IsNullOrEmpty(sOut));
        }

        [TestMethod]
        public void AnimationsXml_ToJson()
        {
            // Arrange
            var xml = "<config xmlns='http://schema.bluehands.de/character-config' version='1.0'> <param name='defaultsequence' value='idle'/> <sequence group='idle' name='still' type='status' probability='1000' in='standard' out='standard'><animation src='idle.gif'/></sequence> <sequence group='idle' name='idle1' type='status' probability='2' in='standard' out='standard'><animation src='idle-1.gif'/></sequence> <sequence group='idle' name='idle2' type='status' probability='5' in='standard' out='standard'><animation src='idle-2.gif'/></sequence> <sequence group='idle' name='idle3' type='status' probability='5' in='standard' out='standard'><animation src='idle-3.gif'/></sequence> <sequence group='idle' name='idle4' type='status' probability='1' in='standard' out='standard'><animation src='idle-4.gif'/></sequence> <sequence group='moveleft' name='moveleft' type='basic' probability='1' in='moveleft' out='moveleft'><animation dx='-55' dy='0' src='walk-l.gif'/></sequence> <sequence group='moveright' name='moveright' type='basic' probability='1' in='moveright' out='moveright'><animation dx='55' dy='0' src='walk-r.gif'/></sequence> <sequence group='chat' name='chat1' type='basic' probability='1000' in='standard' out='standard'><animation src='chat.gif'/></sequence> <sequence group='chat' name='chat2' type='basic' probability='100' in='standard' out='standard'><animation src='chat-2.gif'/></sequence> <sequence group='wave' name='wave' type='emote' probability='1000' in='standard' out='standard'><animation src='wave.gif'/></sequence> <sequence group='kiss' name='kiss' type='emote' probability='1000' in='standard' out='standard'><animation src='kiss.gif'/></sequence> <sequence group='cheer' name='cheer' type='emote' probability='1000' in='standard' out='standard'><animation src='cheer.gif'/></sequence> <sequence group='agree' name='agree' type='emote' probability='1000' in='standard' out='standard'><animation src='agree.gif'/></sequence> <sequence group='deny' name='deny' type='emote' probability='1000' in='standard' out='standard'><animation src='disagree.gif'/></sequence> <sequence group='clap' name='clap' type='emote' probability='1000' in='standard' out='standard'><animation src='clap.gif'/></sequence> <sequence group='dance' name='dance' type='emote' probability='1000' in='standard' out='standard'><animation src='dance.gif'/></sequence> <sequence group='yawn' name='yawn' type='emote' probability='1000' in='standard' out='standard'><animation src='yawn.gif'/></sequence> <sequence group='angry' name='angry' type='emote' probability='1000' in='standard' out='standard'><animation src='angry.gif'/></sequence> <sequence group='laugh' name='laugh' type='emote' probability='1000' in='standard' out='standard'><animation src='laugh.gif'/></sequence> <sequence group='sleep' name='sleep' type='status' probability='1000' in='standard' out='standard'><animation src='idle.gif'/></sequence> </config>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nodes = doc.DocumentElement.ChildNodes;

            var json = new JsonTree.Node(JsonTree.Node.Type.List);

            var keys = new Dictionary<string, List<string>> {
                { "param", new List<string> { "name", "value" }},
                { "sequence", new List<string> { "group", "name", "type", "probability", "in", "out" }},
            };

            foreach (XmlNode node in nodes) {
                if (keys.ContainsKey(node.Name)) {
                    var dict = new JsonTree.Node(JsonTree.Node.Type.Dictionary);
                    foreach (var key in keys[node.Name]) {
                        var attr = node.Attributes.GetNamedItem(key);
                        if (attr != null) {
                            dict.AsDictionary.Add(attr.Name, new JsonTree.Node(JsonTree.Node.Type.String, attr.Value));
                        }
                    }
                    if (node.Name == "sequence") {
                        var src = node.FirstChild.Attributes["src"].Value;
                        dict.AsDictionary.Add("src", new JsonTree.Node(JsonTree.Node.Type.String, src));
                    }
                    var item = new JsonTree.Node(JsonTree.Node.Type.Dictionary);
                    item.AsDictionary.Add(node.Name, dict);
                    json.AsArray.Add(item);
                }
            }

            // Act
            var sOut = json.ToJson();

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

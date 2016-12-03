using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonTree
{
    public class Dictionary : Dictionary<string, Node>
    {
        public Node Get(string key)
        {
            if (ContainsKey(key)) {
                return this[key];
            }
            return new Node(Node.Type.Empty);
        }
    }

    public class List : List<Node>
    {
        public Node Get(int index)
        {
            if (index < Count) {
                return this[index];
            }
            return new Node(Node.Type.Empty);
        }
    }

    public class Node
    {
        public enum Type { Empty, List, Dictionary, Int, Bool, String, Float }

        private readonly Type _type = Type.Empty;
        public object Value;
        private bool _throwExceptionIfConversionFails = false;

        public bool IsEmpty { get { return _type == Type.Empty; } }
        public bool IsList { get { return _type == Type.List; } }
        public bool IsDictionary { get { return _type == Type.Dictionary; } }
        public bool IsInt { get { return _type == Type.Int; } }
        public bool IsBool { get { return _type == Type.Bool; } }
        public bool IsString { get { return _type == Type.String; } }
        public bool IsFloat { get { return _type == Type.Float; } }

        public List AsList { get { return IsList ? (List)Value : new List(); } }
        public Dictionary AsDictionary { get { return IsDictionary ? (Dictionary)Value : new Dictionary(); } }

        // Aliases
        public List List { get { return AsList; } }
        public Dictionary Dictionary { get { return AsDictionary; } }
        public List AsArray { get { return AsList; } }

        public List Array { get { return AsList; } }
        public Dictionary AsObject { get { return AsDictionary; } }
        public Dictionary Object { get { return AsDictionary; } }
        public long AsInt { get { return Int; } }
        public bool AsBool { get { return Bool; } }
        public string AsString { get { return String; } }
        public double AsFloat { get { return Float; } }

        public long Int
        {
            get {
                if (IsInt) {
                    return (long)Value;
                } else if (IsString) {
                    long result;
                    if (Int64.TryParse(String, out result)) {
                        return result;
                    }
                } else if (IsFloat) {
                    return Convert.ToInt64(Float);
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Int.ToString() + " from " + _type.ToString());
                } else {
                    return 0;
                }
            }
        }

        public bool Bool
        {
            get {
                if (IsBool) {
                    return (bool)Value;
                } else if (IsString) {
                    var s = String.ToLower();
                    return s == "true";
                } else if (IsInt) {
                    return Int != 0;
                } else if (IsFloat) {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    return Float != 0.0;
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Bool.ToString() + " from " + _type.ToString());
                } else {
                    return false;
                }
            }
        }

        public string String
        {
            get {
                if (IsString) {
                    return (string)Value;
                } else if (IsInt) {
                    return Int.ToString(CultureInfo.InvariantCulture);
                } else if (IsFloat) {
                    return String.Format(CultureInfo.InvariantCulture, "{0}", (double)Value);
                } else if (IsBool) {
                    return Bool ? "true" : "false";
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.String.ToString() + " from " + _type.ToString());
                } else {
                    return "";
                }
            }
        }

        public double Float
        {
            get {
                if (IsFloat) {
                    return (double)Value;
                } else if (IsInt) {
                    return (double)Int;
                } else if (IsString) {
                    double result = 0.0;
                    if (Double.TryParse(String, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) {
                        return Double.Parse(String, CultureInfo.InvariantCulture);
                    }
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Float.ToString() + " from " + _type.ToString());
                } else {
                    return 0.0;
                }
            }
        }

        public Node(Type type, object value = null)
        {
            _type = type;

            if (value != null) {
                switch (type) {
                    case Type.Int: Value = Convert.ToInt64(value); break;
                    case Type.Float: Value = Convert.ToDouble(value); break;
                    default: Value = value; break;
                }
            } else {
                switch (type) {
                    case Type.List: Value = new List(); break;
                    case Type.Dictionary: Value = new Dictionary(); break;
                    case Type.Int: Value = 0; break;
                    case Type.Bool: Value = false; break;
                    case Type.String: Value = ""; break;
                    case Type.Float: Value = 0.0; break;
                }
            }
        }

        public Node(string sJson, Deserializer.Options options = null)
        {
            var node = Deserializer.FromJson(sJson, options);

            _type = node._type;
            Value = node.Value;
        }

        public override string ToString()
        {
            var options = new Serializer.Options(bFormatted: true) { EncapsulateKeys = "", EncapsulateStrings = "'" };
            return Serializer.ToJson(this, options);
        }

        public string ToJson(Serializer.Options options)
        {
            return Serializer.ToJson(this, options);
        }

        public string ToJson(bool bFormatted = false, bool bWrapped = false)
        {
            return Serializer.ToJson(this, bFormatted, bWrapped);
        }
    }

    public class Deserializer
    {
        public class Options
        {
        }

        public static Node FromJson(string json, Deserializer.Options options = null)
        {
            if (string.IsNullOrEmpty(json)) {
                return new Node(Node.Type.Empty);
            }

            var jsonObject = JToken.Parse(json);
            return Deserializer.NodeFromJsonObject(jsonObject);
        }

        internal static Node NodeFromJsonObject(object obj)
        {
            if (obj == null) {
                return new Node(Node.Type.Empty);
            }

            var value = obj as JValue;
            if (value != null) {
                switch (value.Type) {
                    case JTokenType.Comment: return new Node(Node.Type.Empty);
                    case JTokenType.Integer: return new Node(Node.Type.Int) { Value = (long)value.Value };
                    case JTokenType.Float: return new Node(Node.Type.Float) { Value = (double)value.Value };
                    case JTokenType.String: return new Node(Node.Type.String) { Value = (string)value.Value };
                    case JTokenType.Boolean: return new Node(Node.Type.Bool) { Value = (bool)value.Value };
                    case JTokenType.Null: return new Node(Node.Type.Empty);
                    case JTokenType.Undefined: return new Node(Node.Type.Empty);
                    case JTokenType.Guid: return new Node(Node.Type.String) { Value = value.Value.ToString() };
                    case JTokenType.Uri: return new Node(Node.Type.String) { Value = value.Value.ToString() };
                    case JTokenType.None:
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case JTokenType.Constructor:
                    case JTokenType.Property:
                    case JTokenType.Date:
                    case JTokenType.Raw:
                    case JTokenType.Bytes:
                    case JTokenType.TimeSpan:
                    default:
                        throw new Exception("Json.NET JToken.Type=" + value.Type.ToString() + " not supported");
                }
            }

            var list = obj as JArray;
            if (list != null) {
                var node = new Node(Node.Type.List);
                foreach (var item in list) {
                    node.List.Add(NodeFromJsonObject(item));
                }
                return node;
            }

            var dict = obj as JObject;
            if (dict != null) {
                var node = new Node(Node.Type.Dictionary);
                foreach (var pair in dict) {
                    node.Dictionary.Add(pair.Key, NodeFromJsonObject(pair.Value));
                }
                return node;
            }

            return new Node(Node.Type.Empty);
        }
    }

    public class Serializer
    {
        public class Options
        {
            public bool BlankBeforeMapColon = false;
            public bool BlankAfterMapColon = false;
            public bool BlankBeforeMapBracket = false;
            public bool BlankAfterMapBracket = false;
            public bool BlankBeforeListComma = false;
            public bool BlankAfterListComma = false;
            public bool BlankBeforeListBracket = false;
            public bool BlankAfterListBracket = false;
            public bool BlankBeforeMapComma = false;
            public bool BlankAfterMapComma = false;
            public bool WrapAfterMapPair = false;
            public bool WrapAfterListElement = false;
            public bool IndentList = false;
            public bool IndentMap = false;
            public string IndentString = "";
            public string EncapsulateKeys = "\"";
            public string EncapsulateStrings = "\"";

            public Options(bool bFormatted = false, bool bWrapped = false)
            {
                if (bFormatted) {
                    if (bWrapped) {
                        IndentMap = true;
                        WrapAfterMapPair = true;
                        IndentList = true;
                        WrapAfterListElement = true;
                        IndentString = "  ";
                    }

                    BlankAfterMapColon = true;
                    BlankAfterListComma = true;
                    BlankAfterMapComma = true;

                    BlankBeforeListBracket = true;
                    BlankAfterListBracket = true;

                    BlankBeforeMapBracket = true;
                    BlankAfterMapBracket = true;
                }
            }

        }

        private Options _options = new Options();

        private int IndentDepth { get; set; }

        internal void Indent(StringBuilder sb)
        {
            for (int i = 0; i < IndentDepth; i++) {
                sb.Append(_options.IndentString);
            }
        }

        public static string ToJson(Node node, Options options)
        {
            var js = new Serializer();

            if (options != null) {
                js._options = options;
            }

            return js.Serialize(node);
        }

        public static string ToJson(Node node, bool bFormatted = false, bool bWrapped = false)
        {
            var ser = new Serializer();

            ser._options = new Serializer.Options(bFormatted, bWrapped);

            return ser.Serialize(node);
        }

        internal string Serialize(Node node)
        {
            var sb = new StringBuilder();

            if (node.IsInt) {
                sb.Append(node.Int);
            }

            if (node.IsString) {
                sb.Append(_options.EncapsulateStrings);
                sb.Append(node.String.Replace(_options.EncapsulateStrings, "\\" + _options.EncapsulateStrings));
                sb.Append(_options.EncapsulateStrings);
            }

            if (node.IsFloat) {
                sb.Append(node.Float.ToString(CultureInfo.InvariantCulture));
            }

            if (node.IsBool) {
                sb.Append(node.Bool ? "true" : "false");
            }

            if (node.IsList) {
                sb.Append("[");
                if (node.List.Count > 0) {
                    if (_options.BlankAfterListBracket) { sb.Append(" "); }
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.List) {
                        if (!bFirst) {
                            if (_options.BlankBeforeListComma) { sb.Append(" "); }
                            sb.Append(",");
                            if (_options.BlankAfterListComma) { sb.Append(" "); }
                            if (_options.WrapAfterListElement) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentList) { Indent(sb); }
                        sb.Append(Serialize(prop));
                    }
                    if (_options.BlankBeforeListBracket) { sb.Append(" "); }
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentList) { Indent(sb); }
                }
                sb.Append("]");
            }

            if (node.IsDictionary) {
                sb.Append("{");
                if (node.Dictionary.Count > 0) {
                    if (_options.BlankAfterMapBracket) { sb.Append(" "); }
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.Dictionary) {
                        if (!bFirst) {
                            if (_options.BlankBeforeMapComma) { sb.Append(" "); }
                            sb.Append(",");
                            if (_options.BlankAfterMapComma) { sb.Append(" "); }
                            if (_options.WrapAfterMapPair) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentMap) { Indent(sb); }
                        sb.Append(_options.EncapsulateKeys);
                        sb.Append(prop.Key);
                        sb.Append(_options.EncapsulateKeys);
                        if (_options.BlankBeforeMapColon) { sb.Append(" "); }
                        sb.Append(":");
                        if (_options.BlankAfterMapColon) { sb.Append(" "); }
                        sb.Append(Serialize(prop.Value));
                    }
                    if (_options.BlankBeforeMapBracket) { sb.Append(" "); }
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentMap) { Indent(sb); }
                }
                sb.Append("}");
            }

            return sb.ToString();
        }
    }
}

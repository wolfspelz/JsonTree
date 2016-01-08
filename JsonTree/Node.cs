using System;
using System.Collections.Generic;
using System.Globalization;

namespace JsonTree
{
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

        public List<Node> AsList { get { return IsList ? (List<Node>)Value : new List<Node>(); } }
        public Dictionary<string, Node> AsDictionary { get { return IsDictionary ? (Dictionary<string, Node>)Value : new Dictionary<string, Node>(); } }

        // Aliases
        public List<Node> List { get { return AsList; } }
        public Dictionary<string, Node> Dictionary { get { return AsDictionary; } }
        public List<Node> AsArray { get { return AsList; } }
        public List<Node> Array { get { return AsList; } }
        public Dictionary<string, Node> AsObject { get { return AsDictionary; } }
        public Dictionary<string, Node> Object { get { return AsDictionary; } }
        public long AsInt { get { return Int; } }
        public bool AsBool { get { return Bool; } }
        public string AsString { get { return String; } }
        public double AsFloat { get { return Float; } }

        public long Int
        {
            get
            {
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
            get
            {
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
            get
            {
                if (IsString) {
                    return (string)Value;
                } else if (IsInt) {
                    return Int.ToString(CultureInfo.InvariantCulture);
                } else if (IsFloat) {
                    return String.Format(CultureInfo.InvariantCulture, "{0}", (double)Value);
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
            get
            {
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
                    case Type.List: Value = new List<Node>(); break;
                    case Type.Dictionary: Value = new Dictionary<string, Node>(); break;
                    case Type.Int: Value = 0; break;
                    case Type.Bool: Value = false; break;
                    case Type.String: Value = ""; break;
                    case Type.Float: Value = 0.0; break;
                }
            }
        }
        public Node(Dictionary<string, string> dict)
        {
            _type = Type.Dictionary;
            Value = new Dictionary<string, Node>();

            foreach (var pair in dict) {
                AsDictionary.Add(pair.Key, new JsonTree.Node(JsonTree.Node.Type.String, pair.Value));
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

}

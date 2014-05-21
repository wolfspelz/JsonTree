using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace JsonTree
{
  public class Deserializer
  {
    public class Options
    {
      internal const int DefaultRecursionLimit = -1;
      internal const int DefaultMaxJsonLength = -1;

      public int RecursionLimit = DefaultRecursionLimit;
      public int MaxJsonLength = DefaultMaxJsonLength;
    }

    public static Node FromJson(string sJson, Deserializer.Options options = null)
    {
      var jss = new JavaScriptSerializer();

      if (options != null) {
        if (options.RecursionLimit != Options.DefaultRecursionLimit) {
          jss.RecursionLimit = options.RecursionLimit;
        }
        if (options.MaxJsonLength != Options.DefaultMaxJsonLength) {
          jss.MaxJsonLength = options.MaxJsonLength;
        }
      }

      var jsonObject = jss.DeserializeObject(sJson);
      return Deserializer.NodeFromJsonObject(jsonObject);
    }

    internal static Node NodeFromJsonObject(object obj)
    {
      if (obj == null) {
        return new Node(Node.Type.Empty);
      }

      if (obj.GetType().BaseType == typeof(Array)) {
        var node = new Node(Node.Type.List);
        foreach (var item in obj as object[]) {
          node.List.Add(NodeFromJsonObject(item));
        }
        return node;
      }

      if (obj.GetType().BaseType == typeof(ValueType)) {
        if (obj is int) {
          var node = new Node(Node.Type.Int) { Value = (long)(int)obj };
          return node;
        } else if (obj is long) {
          var node = new Node(Node.Type.Int) { Value = (long)obj };
          return node;
        } else if (obj is bool) {
          var node = new Node(Node.Type.Bool) { Value = (bool)obj };
          return node;
        } else if (obj is float) {
          var node = new Node(Node.Type.Float) { Value = (double)obj };
          return node;
        } else if (obj is double) {
          var node = new Node(Node.Type.Float) { Value = (double)obj };
          return node;
        } else if (obj.GetType().FullName == "System.Decimal") {
          var node = new Node(Node.Type.Float) { Value = Decimal.ToDouble((decimal)obj) };
          return node;
        }
      }

      if (obj.GetType().BaseType == typeof(Object)) {
        if (obj is string) {
          var node = new Node(Node.Type.String) { Value = (string)obj };
          return node;
        } else if (obj.GetType().FullName == "System.String") {
          var node = new Node(Node.Type.String) { Value = (string)obj };
          return node;
        } else {
          var node = new Node(Node.Type.Dictionary);
          foreach (var pair in obj as Dictionary<string, object>) {
            node.Dictionary.Add(pair.Key, NodeFromJsonObject(pair.Value));
          }
          return node;
        }
      }

      return new Node(Node.Type.Empty);
    }

  }
}

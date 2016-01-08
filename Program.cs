using System;

namespace JsonTreeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Goal: inspect values in JSON strings with single line expressions and simple CLR objects without using foreach/if to extract values from JSON");
            Console.WriteLine("");

            const string data = "[ \"first\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            Console.WriteLine("Given the JSON: " + data);

            var json = new JsonTree.Node(data);

            Console.WriteLine("The 42 is the value of the aNumber-key in the map which is the second element of a list. JSON arrays/objects become C# lists/dictionaries.");
            Console.WriteLine("");

            Console.WriteLine("Get to the value quickly:");
            var aNumber = json.List[1].Dictionary["aNumber"].Int;
            Console.WriteLine("  json.List[1].Dictionary[\"aNumber\"].Int = " + aNumber);
            Console.WriteLine("");

            Console.WriteLine("The same in JavaScript notation:");
            var aNumber2 = json.Array[1].Object["aNumber"].Int;
            Console.WriteLine("  json.Array[1].Object[\"aNumber\"].Int = " + aNumber2);
            Console.WriteLine("");

            var serialized = json.ToJson();
            Console.WriteLine("Serialized again: " + serialized);
            Console.WriteLine("");

            Console.WriteLine("Serialized with formatting:");
            var formatted = json.ToJson(new JsonTree.Serializer.Options(bFormatted: true, bWrapped: true));
            Console.WriteLine(formatted);

            var rl = Console.ReadLine();

            Example();
        }

        static void Example()
        {
            const string data = "[ \"first\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            var node = new JsonTree.Node(data);

            // Deserialize
            var aNumber = node.List[1].Dictionary["aNumber"].Int; // C# notation
            // or
            var aNumber2 = node.Array[1].Object["aNumber"].Int; // Javascript notation

            // Serialize
            var serialized = node.ToJson();

            // Serialize formatted
            var formatted = node.ToJson(new JsonTree.Serializer.Options(bFormatted: true, bWrapped: true));
        }
    }
}

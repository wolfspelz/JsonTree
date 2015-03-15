JSON  Tree
==========

Inspect values in JSON stings with single line expressions without using foreach/if to extract values from JSON. 

Extract the 42 from:

    '[ "first ", { "aString": "HelloWorld", "aNumber":42 } ]'

with C# expression: 

    var fourtytwo = json.Array[1].Object["aNumber"].Int;

Or you may use C# notation (List/Dictionary instead of Array/Object) for the same:

    var fourtytwo = json.List[1].Dictionary["aNumber"].Int;

Dive deep into this JSON with a single line of code:

    var data = "[ { 
            aInt: 41, 
            bBool: true, 
            bLong: 42000000000, 
            cString: '43', 
            dFloat: 3.14159265358979323 
        }, { 
            aInt: 44, 
            bLong: 45000000000, 
            cString: ""46"" 
        }, { 
            aList: [ 
                { aInt: 47, bString: '48' }, 
                { aInt: 49, bString: '50' } ], 
            bMap: { aInt: 51, bString: '52' } 
        }
    ]";

Using JavaScript notation (keywords Array and Object):

    Assert.AreEqual(new JsonTree.Node(data).Array[2].Object["aList"].Array[1].Object["bString"].String, "50");

Using CLR notation (List and Dictionary):

    Assert.AreEqual(new JsonTree.Node(data).List[2].Dictionary["aList"].List[1].Dictionary["bString"].String, "50");

Using standard enumerators on CLR objects:

    Assert.AreEqual(new JsonTree.Node(data).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value.String, "50");

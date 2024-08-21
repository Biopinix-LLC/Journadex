// TODO this requires IronPython
//using System;
//using System.Collections.Generic;
//using NLTK.Internals;
//using NLTK.Tree;

//namespace Journadex.Library
//{
//    public class NLTKStrategy : INounIdentificationStrategy
//    {
//        public Dictionary<string, int> IdentifyNouns(string input)
//        {
//            Dictionary<string, int> nouns = new Dictionary<string, int>();

//            // Parse the input string into a syntax tree
//            var parser = new NLTK.Parser.RecursiveDescentParser();
//            var tree = parser.Parse(input);

//            // Iterate over the subtrees in the syntax tree
//            foreach (var subtree in tree.SubTrees())
//            {
//                // Check if the subtree is a noun phrase
//                if (subtree.Label() == "NP")
//                {
//                    // Add the noun phrase to the dictionary
//                    nouns[subtree.ToString()] = subtree.GetSpan().Start;
//                }
//            }

//            return nouns;
//        }
//    }
//}

//using System;
//using System.Collections.Generic;
//using edu.stanford.nlp.ling;
//using edu.stanford.nlp.pipeline;
//using edu.stanford.nlp.util;
//using java.util;

//namespace Journadex.Library
//{
//    public class StanfordCoreNLPStrategy : INounIdentificationStrategy
//    {
//        public Dictionary<string, int> IdentifyNouns(string input)
//        {
//            Dictionary<string, int> nouns = new Dictionary<string, int>();

//            // Create an English StanfordCoreNLP object
//            var props = new Properties();
//            props.setProperty("annotators", "tokenize, ssplit, pos");

//            var pipeline = new StanfordCoreNLP(props);

//            // Annotate the input string
//            var annotation = new Annotation(input);
//            pipeline.annotate(annotation);

//            // Iterate over the sentences in the input
//            var sentences = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;
//            foreach (CoreMap sentence in sentences)
//            {
//                // Iterate over the tokens in the sentence
//                var tokens = sentence.get(new CoreAnnotations.TokensAnnotation().getClass()) as ArrayList;
//                foreach (CoreLabel token in tokens)
//                {
//                    // Check if the token is a noun
//                    if (token.get(new CoreAnnotations.PartOfSpeechAnnotation().getClass()) is string str && str.StartsWith("N") && token.get(new CoreAnnotations.TextAnnotation().getClass()) is string str2)
//                    {
//                        // Add the noun to the dictionary
//                        nouns[str2] = token.beginPosition();
//                    }
//                }
//            }

//            return nouns;
//        }
//    }
//}

using System;
using System.Collections.Generic;

namespace Journadex.Library
{
    public class SplitProperCaseStrategy : INounIdentificationStrategy
    {
        private static readonly string[] nonAlphaNumericChars = { " ", "`", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+", "[", "{", "]", "}", "\\", "|", ";", ":", "'", "\"", ",", "<", ".", ">", "/", "?" };

        public Dictionary<string, int> IdentifyNouns(string input)
        {

            Dictionary<string, int> properCasedWords = new Dictionary<string, int>();

            // Split the input into an array of words
            string[] words = input.Split(nonAlphaNumericChars, StringSplitOptions.RemoveEmptyEntries);

            // Loop through the words array
            for (int i = 0; i < words.Length; i++)
            {
                // Get the current word
                string word = words[i];

                // Check if the word is proper-cased (first letter is uppercase, the rest is lowercase)
                // and if it's not "I", "A" or "An"
                if (char.IsUpper(word[0]) && word.Substring(1).ToLower() == word.Substring(1) &&
                    word != "I" && word != "A" && word != "An")
                {
                    // Calculate the character position of the word in the input string
                    int charPosition = input.IndexOf(word, StringComparison.Ordinal);

                    // Add the word and its position to the dictionary
                    properCasedWords.Add(word, charPosition);
                }
            }

            // Return the dictionary
            return properCasedWords;
        }

    }
}

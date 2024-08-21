using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Journadex.Library
{
    public class RegexStrategy : INounIdentificationStrategy
    {
        public Dictionary<string, int> IdentifyNouns(string input)
        {
            Dictionary<string, int> nouns = new Dictionary<string, int>();

            // Create a regular expression to match words that are capitalized or that follow a determiner
            // TODO not working 
            //Regex regex = new Regex(@"\b(the|a|an)\s+[A-Z]\w*", RegexOptions.IgnoreCase);

            // TODO not working either
            Regex regex = new Regex("^(?!I$|The$|A$|An$)([A-Z][a-z]*)+$");

            // Match the regular expression in the input string
            MatchCollection matches = regex.Matches(input);

            // Add each matched noun to the dictionary
            foreach (Match match in matches)
            {
                // Exclude the determiner if it exists
                if (match.Groups.Count == 2)
                {
                    int determinerLength = match.Groups[1].Length + 1;
                    nouns[match.Value.Substring(determinerLength)] = match.Index + determinerLength;
                }
                else
                {
                    nouns[match.Value] = match.Index;
                }
            }

            return nouns;
        }
    }
}

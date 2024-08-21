using System.Collections.Generic;

namespace Journadex.Library
{

    public interface INounIdentificationStrategy
    {
        Dictionary<string, int> IdentifyNouns(string input);
    }

    public class NounFinder
    {
        private INounIdentificationStrategy _strategy;

        public NounFinder(INounIdentificationStrategy strategy)
        {
            _strategy = strategy;
        }

        public Dictionary<string, int> FindNouns(string input)
        {
            return _strategy.IdentifyNouns(input);
        }
    }
}
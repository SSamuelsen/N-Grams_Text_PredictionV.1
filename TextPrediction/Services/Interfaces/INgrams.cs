using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextPrediction.Models;

namespace TextPrediction.Services.Interfaces
{
    public interface INgrams
    {

        IEnumerable<string> CreateNgrams(string text, byte nGramSize);

        string RetrieveSearches(List<Search> searches);

    }
}

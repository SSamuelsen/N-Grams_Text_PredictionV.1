using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextPrediction.Models;
using TextPrediction.Services.Interfaces;

namespace TextPrediction.Services
{
    public class Ngrams:INgrams
    {

        public static string[] uselessPunctuation = new string[] { ";", "," };        //modify this to suit your needs again 
        public static string[] uselessWords = new string[] { "a", "it", "I" };        //modify this so suit your needs


        public string RetrieveSearches(List<Search> searches)
        {

            var mostSearchedItem = from x in searches
                                   group x by x.Query into grp
                                   orderby grp.Count() descending
                                   select grp.Key;

            

            //make copy of the search results
            var mostSearchedCopy = mostSearchedItem.ToList();

            //remove any useless words so that they are not searched for
            mostSearchedCopy.RemoveAll(x => uselessWords.Contains(x));

            //remove punctuation
            mostSearchedCopy.RemoveAll(x => uselessPunctuation.Contains(x));
            

            

            //remove the actual queried item from teh cleaned list to avoid returning back a duplicate value in the prediction
            //mostSearchedCopy.RemoveAll(x => x.ToString() == Query.ToString());

            //save the search string to be used to create the N-Gram model
            string SearchString = string.Join(" ", mostSearchedCopy);
            

            return SearchString;

        }

        public IEnumerable<string> CreateNgrams(string text, byte nGramSize)
        {

            StringBuilder nGram = new StringBuilder();
            Queue<int> wordLengths = new Queue<int>();

            int wordCount = 0;
            int lastWordLen = 0;

            if(text != "" && char.IsLetterOrDigit(text[0]))
            {
                nGram.Append(text[0]);
                lastWordLen++;
            }
            for (int i = 1; i < text.Length - 1; i++)
            {
                char before = text[i - 1];
                char after = text[i + 1];

                if (char.IsLetterOrDigit(text[i]) || 
                    (text[i] != ' ' 
                    && (char.IsSeparator(text[i]) || char.IsPunctuation(text[i])) 
                    && (char.IsLetterOrDigit(before) && char.IsLetterOrDigit(after))
                    )
                    )
                {
                    nGram.Append(text[i]);
                    lastWordLen++;
                }//end if 
                else
                {
                    if (lastWordLen > 0)
                    {
                        wordLengths.Enqueue(lastWordLen);
                        lastWordLen = 0;
                        wordCount++;
                        if (wordCount >= nGramSize)
                        {
                            
                            yield return nGram.ToString();
                            nGram.Remove(0, wordLengths.Dequeue() + 1);
                            wordCount -= 1;
                        }//end if

                        nGram.Append(" ");

                    }//end if
                }//end else 
                 //end for

                
            }

            nGram.Append(text.Last());
            yield return nGram.ToString();

        }//end function

    }
}

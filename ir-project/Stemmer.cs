using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iveonik.Stemmers; 

namespace ir_project
{
    /// <summary>
    /// A stemmer class that wraps around the StemmersNET package.
    /// </summary>
    public class Stemmer
    {
        EnglishStemmer stemmer = new EnglishStemmer();

        public String stem(String term)
        {
            return stemmer.Stem(term);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    /// <summary>
    /// Implements the BM25 weighting scheme.
    /// </summary>
    public class BM25Scheme : WeightingScheme
    {
        // Algorithmic constants.
        public float k1 { get; private set; }
        public float b { get; private set; }

        public BM25Scheme(float k1, float b)
        {
            this.k1 = k1;
            this.b = b;
        }

        public float computeIDF(float numDocs, float docFreq)
        {
            return 1.0f + (float)Math.Log10((numDocs) / (docFreq + 1.0f));
        }

        public float computeTermSimilarity(float tf, float idf, float qtf, float dl, float dlAvg)
        {
            return (idf * tf * qtf) / (tf + k1 * ((1.0f - b) + b * (dl/dlAvg)));
        }
    }
}

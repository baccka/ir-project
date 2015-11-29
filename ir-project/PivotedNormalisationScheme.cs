using System;
using System.Collections.Generic;

namespace ir_project
{
    /// <summary>
    /// Implement the Pivoted normalisation weighting scheme.
    /// </summary>
    public class PivotedNormalisationScheme
    {
        public float s { get; private set; }

        public PivotedNormalisationScheme(float s)
        {
            this.s = s;
        }

        public float computeIDF(float numDocs, float docFreq)
        {
            return Math.Log10((numDocs + 1.0f) / docFreq);
        }

        public float pivNor(float tf, float idf, float dl, float dlAvg, float qtf)
        {
            return ((1.0f + Math.Log10(1.0f + Math.Log10 (tf))) / ((1.0f - s) + s * (dl / dlAvg))) * idf * qtf;
        }
    }
}
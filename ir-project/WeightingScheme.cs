using System;
using System.Collections.Generic;
using System.Linq;

namespace ir_project
{  
	public interface WeightingScheme
	{ 
        float computeTermSimilarity(float tf, float idf, float qtf, float dl, float dlAvg);
        float computeIDF(float numDocs, float docFreq);
	}
}

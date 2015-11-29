using System;
using System.Collections.Generic;
using System.Linq;

namespace ir_project
{  
	interface WeightingScheme
	{ 
		float computeTermSimilarity();
		float computeIDF();
	}
}

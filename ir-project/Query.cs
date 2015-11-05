using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    /// <summary>
    /// This class represents a query and contains all occurences of the terms in the query
    /// and their frequencies.
    /// </summary>
    public class Query
    {
        public struct QueryTerm
        {
            public Term term;
            public int frequency;

            public QueryTerm(Term term, int frequency)
            {
                this.term = term;
                this.frequency = frequency;
            }
        }

        /// <summary>
        /// The list of terms and their frequencies in this query.
        /// </summary>
        public List<QueryTerm> terms { get; private set; }

        public Query(List<QueryTerm> terms)
        {
            this.terms = terms;
        }
    }
}

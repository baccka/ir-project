using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    /// <summary>
    /// The relevance judgement for a query.
    /// </summary>
    public struct RelevanceJudgement
    {
        public int queryId { get; private set; }
        public int documentId { get; private set; }

        public RelevanceJudgement(int queryId, int documentId) : this()
        {
            this.queryId = queryId;
            this.documentId = documentId;
        }
    }
}

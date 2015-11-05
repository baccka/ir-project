using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    public class InformationRetriever
    {
        public TermDocumentMatrix terms = new TermDocumentMatrix();

        /// <summary>
        /// Update the IR system by building a search index using the
        /// given document collection.
        /// </summary>
        public void update(DocumentCollection documents)
        {
            var documentId = 0;
            foreach (var doc in documents.getDocuments())
            {
                // Create a local (document specific) term collection.
                Dictionary<String, int> documentTerms = new Dictionary<String, int>();
                foreach (var word in doc.getTerms())
                {
                    int frequency;
                    if (documentTerms.TryGetValue(word, out frequency))
                    {
                        documentTerms[word] = frequency + 1;
                    }
                    else
                    {
                        documentTerms[word] = 1;
                    }
                }
                // Merge the local term collection with the global term document matrix.
                foreach (var termOccurence in documentTerms) {
                    var term = terms.getTerm(termOccurence.Key);
                    term.addOccurenceForDocument(termOccurence.Value, documentId);
                }
                documentId++;
            }
        }

        /// <summary>
        /// Create a query that can be used for document retrival from the given document.
        /// </summary>
        public Query createQuery(Document query) {
            // Extract the terms that are present in a query.
            Dictionary<Term, int> queryTerms = new Dictionary<Term, int>();
            foreach (var word in query.getTerms())
            {
                int frequency;
                var term = terms.getTerm(word);
                if (queryTerms.TryGetValue(term, out frequency))
                {
                    queryTerms[term] = frequency + 1;
                }
                else
                {
                    queryTerms[term] = 1;
                }
            }
            // Convert the dictionary into a list of terms.
            List<Query.QueryTerm> termList = new List<Query.QueryTerm>();
            foreach (var i in queryTerms)
            {
                termList.Add(new Query.QueryTerm(i.Key, i.Value));
            }
            return new Query(termList);
        }
    }
}

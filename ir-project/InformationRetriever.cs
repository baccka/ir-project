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
                    if (documentTerms.TryGetValue(word, out frequency)) {
                        documentTerms[word] = frequency + 1;
                    } else {
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
    }
}

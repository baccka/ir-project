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
        public DocumentCollection documents = null;
        public StopWordRemover stopWords = new StopWordRemover();
        public Stemmer stemmer = new Stemmer();

        public struct SearchResultItem
        {
            public int documentId;
            public float similarity;

            public SearchResultItem(int documentId, float similarity)
            {
                this.documentId = documentId;
                this.similarity = similarity;
            }
        };

        /// <summary>
        /// Update the IR system by building a search index using the
        /// given document collection.
        /// </summary>
        public void update(DocumentCollection documents)
        {
            this.documents = documents;
            var documentId = 0;
            foreach (var doc in documents.getDocuments())
            {
                // Create a local (document specific) term collection.
                Dictionary<String, int> documentTerms = new Dictionary<String, int>();
                foreach (var rawWord in doc.getTerms())
                {
                    var word = stemmer.stem(rawWord);
                    if (stopWords.isStopWord(word)) { continue; }
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
            foreach (var rawWord in query.getTerms())
            {
                String word = stemmer.stem(rawWord);
                if (stopWords.isStopWord(word)) { continue; }
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

        /// <summary>
        /// Perform a search using the given query.
        /// </summary>
        public List<SearchResultItem> executeQuery(Query query, BM25Scheme scheme)
        {
            // Find the documents that contain terms that exists in the query.
            var results = new Dictionary<int,float>();
            foreach (var term in query.terms)
            {
                float idf = scheme.computeIDF((float)documents.getDocuments().Count, (float)term.term.getOccurences().Count);
                foreach (var occurence in term.term.getOccurences())
                {
                    float dl = (float)documents[occurence.documentId].length;
                    float termSimilarity = scheme.computeTermSimilarity((float)occurence.frequency, idf, (float)term.frequency, dl, documents.averageDocumentLength);
                    float similarity;
                    if (results.TryGetValue(occurence.documentId, out similarity))
                    {
                        results[occurence.documentId] = similarity + termSimilarity;
                    }
                    else
                    {
                        results[occurence.documentId] = termSimilarity;
                    }
                }
            }
            // Create an ordered list of resulting documents and their similarity scores.
            var resultList = new List<SearchResultItem>();
            foreach (var i in results)
            {
                resultList.Add(new SearchResultItem(i.Key, i.Value));
            }
            resultList.Sort((x, y) => -x.similarity.CompareTo(y.similarity));
            return resultList;
        }
    }
}

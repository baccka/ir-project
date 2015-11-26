using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    public class DocumentCollection
    {
        private List<Document> documents = new List<Document>();
        private bool isAverageDocumentLengthComputed = false;
        private float averageLength = 0;

        public DocumentCollection() { }
        public DocumentCollection(List<Document> documents)
        {
            this.documents = documents;
        }


        /// <summary>
        /// Return the average length of a document in this document collection.
        /// </summary of blalah>
        public float averageDocumentLength
        {
            get
            {
                if (!isAverageDocumentLengthComputed)
                {
                    averageLength = 0;
                    foreach (var doc in documents)
                    {
                        averageLength += doc.length;
                    }
                    averageLength /= documents.Count;
                }
                return averageLength;
            }
        }

        /// <summary>
        /// Add a document to the document collection.
        /// </summary>
        public void add(Document doc)
        {
            isAverageDocumentLengthComputed = false;
            documents.Add(doc);
        }

        /// <summary>
        /// Return the document that corresponds to the given document ID.
        /// </summary>
        public Document this[int key]
        {
            get
            {
                return documents[key];
            }
        }

        public IList<Document> getDocuments()
        {
            return documents;
        }
    }
}

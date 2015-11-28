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
        private Dictionary<int, Document> idDocumentMapping = new Dictionary<int, Document>();
        private bool isIdDocumentMappingDone = false;

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
            isIdDocumentMappingDone = false;
            documents.Add(doc);
        }

        /// <summary>
        /// Return a document that has the given document ID.
        /// Returns null if such document doesn't exist.
        /// </summary>
        public Document documentById(int id)
        {
            if (!isIdDocumentMappingDone)
            {
                foreach (var d in documents)
                {
                    idDocumentMapping[d.id] = d;
                }
                isIdDocumentMappingDone = true;
            }
            Document doc;
            if (idDocumentMapping.TryGetValue(id, out doc)) {
                return doc;
            }
            return null;
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

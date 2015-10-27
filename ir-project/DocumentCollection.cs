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

        /// <summary>
        /// Add a document to the document collection.
        /// </summary>
        public void add(Document doc)
        {
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

        public IEnumerable<Document> getDocuments()
        {
            return documents;
        }
    }
}

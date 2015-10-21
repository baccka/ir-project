using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    /// <summary>
    /// This class represents a term and contains all occurences of that term in the document collection. 
    /// </summary>
    class Term
    {
        public struct DocumentOccurence
        {
            int frequency;
            int documentId;

            public DocumentOccurence(int frequency, int documentId)
            {
                this.frequency = frequency;
                this.documentId = documentId;
            }
        }

        private String value;
        private int globalFrequency = 0;
        private List<DocumentOccurence> occurences = new List<DocumentOccurence>();

        public Term(String value)
        {
            this.value = value;
        }

        /// <summary>
        /// Record the occurence of this term in a specified document.
        /// </summary>
        public void addOccurenceForDocument(int frequency, int documentId) 
        {
            globalFrequency += frequency;
            occurences.Add(new DocumentOccurence(frequency, documentId));
        }

        public int getGlobalFrequency()
        {
            return globalFrequency;
        }

        public IList<DocumentOccurence> getOccurences()
        {
            return occurences;
        }
    }
}

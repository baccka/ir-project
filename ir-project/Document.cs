using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    public class Document
    {
        public String value { get; private set; }
        public int length { get; private set; }

        public Document(String value)
        {
            this.value = value;
            length = 0;

            foreach (var term in getTerms())
            {
                length += 1;
            }
        }

        /// <summary>
        /// Enumerate over all of the terms in a document.
        /// </summary>
        public IEnumerable<string> getTerms()
        {
            if (value.Length == 0) {
                yield break;
            }
            int cIndex = 0;
            int nIndex;
            while ((nIndex = value.IndexOf(' ', cIndex + 1)) != -1)
            {
                int sIndex = (cIndex == 0 ? 0 : cIndex + 1);
                yield return value.Substring(sIndex, nIndex - sIndex);
                cIndex = nIndex;
            }
            yield return value.Substring(cIndex == 0 ? 0 : cIndex + 1);
        }
    }
}

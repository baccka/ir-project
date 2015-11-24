using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ir_project
{
    public class Document
    {
        public String value { get; private set; }
        public int length { get; private set; }
        public int id { get; private set; }

        public Document(String value, int id)
        {
            this.value = value;
            this.id = id;
            length = 0;

            foreach (var term in getTerms())
            {
                length += 1;
            }
        }

        public static bool IsCharAlpha(string text, int pos)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(text, pos);
            return uc == UnicodeCategory.UppercaseLetter ||
                    uc == UnicodeCategory.LowercaseLetter ||
                    uc == UnicodeCategory.TitlecaseLetter ||
                    uc == UnicodeCategory.DecimalDigitNumber ||
                    uc == UnicodeCategory.LetterNumber ||
                    uc == UnicodeCategory.MathSymbol ||
                    uc == UnicodeCategory.OtherLetter;
        }

        /// <summary>
        /// Enumerate over all of the terms in a document.
        /// </summary>
        public IEnumerable<string> getTerms()
        {
            if (value.Length == 0) {
                yield break;
            }
            int[] TextElements = StringInfo.ParseCombiningCharacters(value);
        
            int n = TextElements.Length;
            int i = 0;
            
            while (i < n) {
                // Skip non words, such as spaces and punctuation.
                while (i < n && !IsCharAlpha(value, TextElements[i])) i++;
                // Get the word
                int start = i;
                while (i < n && IsCharAlpha(value, TextElements[i])) i++;
                if (i > start)
                    yield return value.Substring(start, i - start);
            }
            yield break;
        }
    }
}

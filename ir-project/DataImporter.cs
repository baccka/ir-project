using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ir_project
{
    /// <summary>
    /// Reads files that contain the documents or the queries.
    /// </summary>
    public class DocumentImporter
    {
        private static String matchValue(Regex regex, String value)
        {
            Match match = regex.Match(value);
            if (match.Success)
                return match.Groups[1].Value;
            return null;
        }

        /// <summary>
        /// Load the documents contained in .ALL or .QRY files, like MED.ALL or MED.QRY.
        /// </summary>
        /// <param name="input">contents of the file</param>
        public static List<Document> parseDocuments(String input)
        {
            List<Document> documents = new List<Document>();
            var dotIRegex = new Regex(@"^\.I\s+(\d+)$");
            var dotWRegex = new Regex(@"^\.W$");
            int documentId = -1;
            bool isPreviousLineDotI = false;
            String currentDocument = null;
            using (StringReader reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    String idString = matchValue(dotIRegex, line);
                    if (idString != null)
                    {
                        // Line in the form of '.I NNN'
                        documentId = int.Parse(idString);
                        isPreviousLineDotI = true;
                        if (currentDocument != null)
                        {
                            documents.Add(new Document(currentDocument));
                        }
                        currentDocument = "";
                        continue;
                    }
                    if (isPreviousLineDotI && dotWRegex.IsMatch(line))
                    {
                        // Line in the form of '.W'
                        isPreviousLineDotI = false;
                        continue;
                    }
                    isPreviousLineDotI = false;
                    // Add the current line to the current document.
                    if (currentDocument != null)
                    {
                        if (currentDocument.Length != 0)
                            currentDocument += "\n";
                        currentDocument += line;
                    }
                }
            }
            // Add the final document
            if (currentDocument != null)
            {
                documents.Add(new Document(currentDocument));
            }
            return documents;
        }
    }
}

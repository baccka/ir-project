using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ir_project;

namespace ir_project_unittests
{
    [TestClass]
    public class UnitTest1
    {
        List<String> getDocumentTerms(Document document)
        {
            var terms = new List<String>();
            foreach (var term in document.getTerms())
            {
                terms.Add(term);
            }
            return terms;
        }

        // This method ensures that the document can enumerate over all
        // the terms correctly.
        [TestMethod]
        public void TestDocumentTermEnumeration()
        {
            {
                var document = new Document("document");
                var terms = getDocumentTerms(document);
                Assert.AreEqual(1, terms.Count);
                Assert.AreEqual("document", terms[0]);
            }
            {
                var document = new Document("hello world");
                var terms = getDocumentTerms(document);
                Assert.AreEqual(2, terms.Count);
                Assert.AreEqual("hello", terms[0]);
                Assert.AreEqual("world", terms[1]);
            }
            {
                var document = new Document("a b c d xx yy 13 .af");
                var terms = getDocumentTerms(document);
                Assert.AreEqual(8, terms.Count);
                Assert.AreEqual("a", terms[0]);
                Assert.AreEqual("b", terms[1]);
                Assert.AreEqual("c", terms[2]);
                Assert.AreEqual("d", terms[3]);
                Assert.AreEqual("xx", terms[4]);
                Assert.AreEqual("yy", terms[5]);
                Assert.AreEqual("13", terms[6]);
                Assert.AreEqual(".af", terms[7]);
            }
        }
    }
}

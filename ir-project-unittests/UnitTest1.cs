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
        
        // This method ensures that the documents and the document collection compute
        // correct document and average document lengths.
        [TestMethod]
        public void TestDocumentLengthComputation()
        {
            {
                var d0 = new Document("hello world");
                var d1 = new Document("hello world hello man");
                var d2 = new Document("a b");
                var d3 = new Document("c d");
                Assert.AreEqual(2, d0.length);
                Assert.AreEqual(4, d1.length);
                Assert.AreEqual(2, d2.length);
                Assert.AreEqual(2, d3.length);
                var documents = new DocumentCollection();
                documents.add(d0);
                documents.add(d1);
                Assert.AreEqual(3.0, documents.averageDocumentLength);
                documents.add(d2);
                documents.add(d3);
                Assert.AreEqual(2.5, documents.averageDocumentLength);
            }
        }

        [TestMethod]
        public void TestSearchIndexConstruction()
        {
            {
                var d0 = new Document("hello world");
                var d1 = new Document("hello world hello man");
                var documents = new DocumentCollection();
                documents.add(d0);
                documents.add(d1);
                Assert.AreSame(d0, documents[0]);
                Assert.AreSame(d1, documents[1]);
                var ir = new InformationRetriever();
                ir.update(documents);
                var terms = ir.terms;
                
                var hello = terms.getTerm("hello");
                Assert.AreEqual(3, hello.getGlobalFrequency());
                Assert.AreEqual(1, hello.getOccurences()[0].frequency);
                Assert.AreEqual(0, hello.getOccurences()[0].documentId);
                Assert.AreEqual(2, hello.getOccurences()[1].frequency);
                Assert.AreEqual(1, hello.getOccurences()[1].documentId);

                var world = terms.getTerm("world");
                Assert.AreEqual(2, world.getGlobalFrequency());
                Assert.AreEqual(1, world.getOccurences()[0].frequency);
                Assert.AreEqual(0, world.getOccurences()[0].documentId);
                Assert.AreEqual(1, world.getOccurences()[1].frequency);
                Assert.AreEqual(1, world.getOccurences()[1].documentId);

                var man = terms.getTerm("man");
                Assert.AreEqual(1, man .getGlobalFrequency());
                Assert.AreEqual(1, man.getOccurences()[0].frequency);
                Assert.AreEqual(1, man.getOccurences()[0].documentId);
            }
        }
    }
}

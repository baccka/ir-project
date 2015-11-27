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
                var document = new Document("", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(0, terms.Count);
                Assert.AreEqual(0, document.id);
            }
            {
                var document = new Document("a", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(1, terms.Count);
                Assert.AreEqual("a", terms[0]);
            }
            {
                var document = new Document("document", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(1, terms.Count);
                Assert.AreEqual("document", terms[0]);
            }
            {
                var document = new Document("hello world", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(2, terms.Count);
                Assert.AreEqual("hello", terms[0]);
                Assert.AreEqual("world", terms[1]);
            }
            {
                var document = new Document("a b c d xx yy 13 .af", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(8, terms.Count);
                Assert.AreEqual("a", terms[0]);
                Assert.AreEqual("b", terms[1]);
                Assert.AreEqual("c", terms[2]);
                Assert.AreEqual("d", terms[3]);
                Assert.AreEqual("xx", terms[4]);
                Assert.AreEqual("yy", terms[5]);
                Assert.AreEqual("13", terms[6]);
                Assert.AreEqual("af", terms[7]);
            }
            {
                var document = new Document("Sentence, yes.\nHello world!", 0);
                var terms = getDocumentTerms(document);
                Assert.AreEqual(4, terms.Count);
                Assert.AreEqual("Sentence", terms[0]);
                Assert.AreEqual("yes", terms[1]);
                Assert.AreEqual("Hello", terms[2]);
                Assert.AreEqual("world", terms[3]);
            }
        }
        
        // This method ensures that the documents and the document collection compute
        // correct document and average document lengths.
        [TestMethod]
        public void TestDocumentLengthComputation()
        {
            {
                var d0 = new Document("hello world", 0);
                var d1 = new Document("hello world hello man", 1);
                var d2 = new Document("a b", 2);
                var d3 = new Document("c d", 3);
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

        // This method ensures that the document collection can return documents
        // based on their id.
        [TestMethod]
        public void TestDocumentIDMapping()
        {
            var d0 = new Document("hello world", 11);
            var d1 = new Document("hello world hello man", 42);
            var documents = new DocumentCollection();
            documents.add(d0);
            documents.add(d1);

            Assert.AreSame(d0, documents.documentById(11));
            Assert.AreSame(d1, documents.documentById(42));
            var d2 = new Document("foo", 0);
            documents.add(d2);
            Assert.AreSame(d2, documents.documentById(0));
        }


        [TestMethod]
        public void TestSearchIndexConstruction()
        {
            {
                var d0 = new Document("hello world", 0);
                var d1 = new Document("hello world hello man", 1);
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

                var query = ir.createQuery(new Document("world hello hello", 0));
                Assert.AreEqual(2, query.terms.Count);
                Assert.AreEqual(1, query.terms[0].frequency);
                Assert.AreEqual(2, query.terms[1].frequency);
            }
        }
    
        [TestMethod]
        public void TestBM25Searching()
        {
            var d0 = new Document("hello world", 0);
            var d1 = new Document("hello world hello man", 1);
            var documents = new DocumentCollection();
            documents.add(d0);
            documents.add(d1);
            var ir = new InformationRetriever();
            ir.update(documents);
            var scheme = new BM25Scheme(/*k1=*/2.0f, /*b=*/0.75f);

            {
                var results = ir.executeQuery(ir.createQuery(new Document("hello", 0)), scheme);
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(1, results[0].documentId);
                Assert.AreEqual(0, results[1].documentId);
                Assert.IsTrue(results[0].similarity > results[1].similarity);
            }

            {
                var results = ir.executeQuery(ir.createQuery(new Document("man", 0)), scheme);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[0].documentId);
            }
        }

        [TestMethod]
        public void TestDataImporter()
        {
            // Test .ALL and .QRY file import.
            var contents = @"
.I 1
.W
 the crystalline lens in vertebrates, including humans.
.I 2
.W
 the relationship of blood and cerebrospinal fluid oxygen concentrations
or partial pressures.  a method of interest is polarography.
.I 3
.W
 electron microscopy of lung or bronchi.
.I 4
.W
 tissue culture of lung or bronchial neoplasms.
.I  50
.W
 the crossing of fatty acids through the placental barrier.  normal
fatty acid levels in placenta and fetus.
";
            var docs = DataImporter.parseDocuments(contents);
            Assert.AreEqual(5, docs.Count);
            Assert.AreEqual(" the crystalline lens in vertebrates, including humans.", docs[0].value);
            Assert.AreEqual(1, docs[0].id);
            Assert.AreEqual(" the relationship of blood and cerebrospinal fluid oxygen concentrations\nor partial pressures.  a method of interest is polarography.", docs[1].value);
            Assert.AreEqual(2, docs[1].id);
            Assert.AreEqual(" electron microscopy of lung or bronchi.", docs[2].value);
            Assert.AreEqual(3, docs[2].id);
            Assert.AreEqual(" tissue culture of lung or bronchial neoplasms.", docs[3].value);
            Assert.AreEqual(4, docs[3].id);
            Assert.AreEqual(" the crossing of fatty acids through the placental barrier.  normal\nfatty acid levels in placenta and fetus.", docs[4].value);
            Assert.AreEqual(50, docs[4].id);


            // Test .REL file import.
            var rels = @"
1 0 13 1
1 0 14 1
2 0 301 1 
2 0 303 1
5 0 59 1
";
            var judgements = DataImporter.parseRelevance(rels);
            Assert.AreEqual(5, judgements.Count);
            Assert.AreEqual(1, judgements[0].queryId);
            Assert.AreEqual(13, judgements[0].documentId);
            Assert.AreEqual(1, judgements[1].queryId);
            Assert.AreEqual(14, judgements[1].documentId);
            Assert.AreEqual(2, judgements[2].queryId);
            Assert.AreEqual(301, judgements[2].documentId);
            Assert.AreEqual(2, judgements[3].queryId);
            Assert.AreEqual(303, judgements[3].documentId);
            Assert.AreEqual(5, judgements[4].queryId);
            Assert.AreEqual(59, judgements[4].documentId);
        }

        [TestMethod]
        public void TestStemmer()
        {
            var stemmer = new Stemmer();
            Assert.AreEqual("hello", stemmer.stem("Hello"));
            Assert.AreEqual("brake", stemmer.stem("braking"));
            Assert.AreEqual("park", stemmer.stem("parking"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using ir_project;

namespace ir_project_terminal
{
    class Program
    {

        static String readFile(String path)
        {
            String data;
            try
            {
                StreamReader streamReader = new StreamReader(path);
                data = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error: file '{0}' not found! Try again.", path);
                return null;
            }
            return data;
        }

        /// <summary>
        /// A command that can be ran on the command line.
        /// </summary>
        class Command
        {
            public String command { get; private set; }
            private Regex regex;

            public Command(String cmd)
            {
                command = cmd;
                regex = new Regex(@"^\s*" + cmd + @":(.+)$");
            }

            /// <summary>
            /// Return true if the current command matches a given string.
            /// If the command matches the string, it also returns the
            /// additional parameter which is specified after the command.
            /// </summary>
            /// <returns></returns>
            public Tuple<bool, String> matches(String str) 
            {
                Match match = regex.Match(str);
                if (!match.Success)
                    return new Tuple<bool, String>(false, null);
                return new Tuple<bool, String>(true, match.Groups[1].Value);
            }
        }

        static void executeQuery(InformationRetriever engine, WeightingScheme scheme, Query query)
        {
            var results = engine.executeQuery(query, scheme);
            Console.WriteLine("Found {0} results:", results.Count);
            foreach (var result in results)
            {
                Console.WriteLine("  document id: {0}, similarity score: {1}", result.documentId, result.similarity);
            }
        }

        /// <summary>
        /// Return a document for a query with the given id.
        /// </summary>
        static Document getQueryById(String sid, DocumentCollection queryCollection)
        {
            int queryId = int.Parse(sid);
            var doc = queryCollection.documentById(queryId);
            if (doc == null) 
            {
                Console.WriteLine("Error: Query with ID {0} doesn't exist!", queryId);
                return null;
            }
            return doc;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ir-project command line terminal interface.");
            Console.WriteLine("----");
            String input;

            var loadDocuments = new Command("load documents");
            var loadQueries = new Command("load queries");
            var loadRelevance = new Command("load relevance");
            var runNewQuery = new Command("run text query");
            var runQuery = new Command("run query");
            var showTerm = new Command("show term");
            var showDocument = new Command("show document");
            var showQuery = new Command("show query");
            var setScheme = new Command("set scheme");
            Command[] commands = { loadDocuments, loadQueries, loadRelevance, runNewQuery, runQuery, showTerm, showDocument, showQuery, setScheme };

            var documentCollection = new DocumentCollection();
            var queryCollection = new DocumentCollection();
            List<RelevanceJudgement> relevance = null;
            var engine = new InformationRetriever();
            // Weighting schemes.
            var bm25Scheme = new BM25Scheme(/*k1=*/2.0f, /*b=*/0.75f);
            var pivotedScheme = new PivotedNormalisationScheme(/*s=*/0.5f);
            WeightingScheme scheme = bm25Scheme;
            do 
            {
                Console.Write("> ");
                input = Console.ReadLine();
                if (input == null) { input = ""; }
                if (input == "exit" || input == "quit") { break; }
                if (input == "help") {
                    Console.WriteLine("Terminal UI for the ir-project IR system.");
                    Console.WriteLine("Available commands:");
                    Console.WriteLine("  exit");
                    Console.WriteLine("  quit");
                    Console.WriteLine("  help");
                    foreach (var command in commands)
                    {
                        Console.WriteLine("  {0}: <argument>", command.command);
                    }
                    continue;
                }
                String matchedCommand = null;
                String matchedArgument = null;
                foreach (var command in commands) {
                    var match = command.matches(input);
                    if (match.Item1) {
                        matchedCommand = command.command;
                        matchedArgument = match.Item2.Trim();
                        break;
                    }
                }

                switch (matchedCommand) {
                    case "load documents":
                        {
                            var data = readFile(matchedArgument);
                            if (data != null)
                            {
                                Console.WriteLine("Loading documents... ");
                                var documents = DataImporter.parseDocuments(data);
                                Console.WriteLine("Loaded {0} documents!", documents.Count);
                                Console.Write("Constructing the term-document indexing datastructures... ");
                                Stopwatch sw = new Stopwatch();                          
                                sw.Start();
                                documentCollection = new DocumentCollection(documents);
                                engine.update(documentCollection);
                                sw.Stop();
                                Console.WriteLine("Done ({0} ms)!", sw.ElapsedMilliseconds);
                            }
                            break;
                        }

                    case "load queries":
                        {
                            var data = readFile(matchedArgument);
                            if (data != null)
                            {
                                Console.WriteLine("Loading queries");
                                var queries = DataImporter.parseDocuments(data);
                                queryCollection = new DocumentCollection(queries);
                                Console.WriteLine("Loaded {0} queries!", queries.Count);
                            }
                            break;
                        }

                    case "load relevance":
                        {
                            var data = readFile(matchedArgument);
                            if (data != null)
                            {
                                Console.Write("Loading relevance judgements... ");
                                relevance = DataImporter.parseRelevance(data);
                                Console.WriteLine("Loaded {0} relevance judgements!", relevance.Count);
                            }
                            break;
                        }

                    case "run text query":
                        {
                            var query = engine.createQuery(new Document(matchedArgument, 0));
                            executeQuery(engine, scheme, query);
                            break;
                        }

                    case "run query":
                        {
                            var doc = getQueryById(matchedArgument, queryCollection);
                            if (doc == null) 
                            {
                                break;
                            }
                            Console.WriteLine("Query with id {0}, text: '{1}'", doc.id, doc.value);
                            Stopwatch sw = new Stopwatch();                          
                            sw.Start();
                            var query = engine.createQuery(doc);
                            var results = engine.executeQuery(query, scheme);
                            sw.Stop();
                            Console.WriteLine("Found {0} results in {1} ms:", results.Count, sw.ElapsedMilliseconds);
                            foreach (var result in results)
                            {
                                Console.WriteLine("  document id: {0}, similarity score: {1}", result.documentId, result.similarity);
                            }
                            // Compute precision/recall if possible.
                            if (relevance != null) {
                                var relevantDocuments = relevance.Where(x => x.queryId == doc.id).Select(x => x.documentId).ToArray();
                                var relevantSet = new HashSet<int>(relevantDocuments);

                                List<double> precision = new List<double>();
                                List<double> recall = new List<double>();

                                //add initial precision and recall values
                                precision.Add(100);
                                recall.Add(0);
                                double noOfRelevantDocs = relevantDocuments.Count();

                                double relevantDocCount = 1;
                                double resultCount = 1;
                               
                                foreach (var result in results)
                                {
                                    if (relevantSet.Contains(result.documentId))
                                    {
                                        recall.Add((relevantDocCount / noOfRelevantDocs)*100);
                                        precision.Add((relevantDocCount / resultCount)*100);
                                        relevantDocCount++;
                                    }
                                    resultCount++;
                                }
                                Console.WriteLine("Precision | Recall (graph is shown in the browser):");
                                for(int i = 0; i < precision.Count; i++)
                                {
                                    Console.WriteLine("  {0} | {1}", precision[i], recall[i]);
                                }
                                PrecisionRecallGraph.show(doc.id, precision, recall);
                            }
                            break;
                        }

                    case "show term":
                        {
                            var term = engine.terms.findTerm(matchedArgument);
                            if (term == null)
                            {
                                Console.WriteLine("Error: Invalid term '{0}'", matchedArgument);
                                break;
                            }
                            Console.WriteLine("Term '{0}', global frequency: {1}, occurences:", matchedArgument, term.getGlobalFrequency());
                            foreach (var occurence in term.getOccurences())
                            {
                                Console.WriteLine("  document id: {0}, frequency: {1}", occurence.documentId, occurence.frequency);
                            }
                            break;
                        }

                    case "show document":
                        {
                            int docId = int.Parse(matchedArgument);
                            var doc = documentCollection.documentById(docId);
                            if (doc == null) 
                            {
                                Console.WriteLine("Error: Document with ID {0} doesn't exist!", docId);
                                break;
                            }
                            Console.WriteLine("Document with id '{0}', terms:", doc.id);
                            foreach (var term in engine.terms.termList) 
                            {
                                var occurences = term.getOccurences().Where(x => x.documentId == doc.id);
                                // Usually this just one occurence.
                                foreach (var occurence in occurences)
                                {
                                    Console.WriteLine("  term '{0}', frequency: {1}", term.value, occurence.frequency);
                                }
                            }
                            break;
                        }

                    case "show query":
                        {
                            var doc = getQueryById(matchedArgument, queryCollection);
                            if (doc == null) 
                            {
                                break;
                            }
                            Console.WriteLine("Query with id {0}, text: '{1}', terms:", doc.id, doc.value);
                            var query = engine.createQuery(doc);
                            foreach (var term in query.terms)
                            {
                                Console.WriteLine("  term '{0}', frequency: {1}", term.term.value, term.frequency);
                            }
                            break;
                        }

                    case "set scheme":
                        {
                            if (matchedArgument == "bm25")
                            {
                                scheme = bm25Scheme;
                                Console.WriteLine("Using BM25.");
                            }
                            else if (matchedArgument == "pivoted")
                            {
                                scheme = pivotedScheme;
                                Console.WriteLine("Using Pivoted Normalisation.");
                            }
                            else
                            {
                                Console.WriteLine("  unknown weighting scheme '{0}'", matchedArgument);
                            }
                            break;
                        }

                        default:
                            Console.WriteLine("Error: Unknown command '{0}'!", input);
                            break;
                }
            } while (input != "exit" && input != "quit");
            Console.WriteLine("---");
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
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

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ir-project command line terminal interface.");

            Console.WriteLine("About to call Console.ReadLine in a loop.");
            Console.WriteLine("----");
            String input;

            var loadDocuments = new Command("load documents");
            var runNewQuery = new Command("run text query");
            Command[] commands = { loadDocuments, runNewQuery };

            var engine = new InformationRetriever();
            // Weighting schemes.
            var scheme = new BM25Scheme(/*k1=*/2.0f, /*b=*/0.75f);
            do 
            {
                Console.Write("> ");
                input = Console.ReadLine();
                String matchedCommand = null;
                String matchedArgument = null;
                foreach (var command in commands) {
                    var match = command.matches(input);
                    if (match.Item1) {
                        matchedCommand = command.command;
                        matchedArgument = match.Item2;
                        break;
                    }
                }

                switch (matchedCommand) {
                    case "load documents":
                        var data = readFile(matchedArgument);
                        if (data != null) 
                        {
                            Console.WriteLine("Loading documents... ");
                            var documents = DataImporter.parseDocuments(data);
                            Console.WriteLine("Loaded {0} documents!", documents.Count);
                            Console.Write("Constructing the term-document indexing datastructures... ");
                            engine.update(new DocumentCollection(documents));
                            Console.WriteLine("Done!");
                        }
                        break;

                    case "run text query":
                        var query = engine.createQuery(new Document(matchedArgument, 0));
                        var results = engine.executeQuery(query, scheme);
                        Console.WriteLine("Found {0} results:", results.Count);
                        foreach (var result in results)
                        {
                            Console.WriteLine("  document id: {0}, similarity score: {1}", result.documentId, result.similarity);
                        }
                        break;

                    default:
                        Console.WriteLine("Error: Unknown command '{0}'!", input);
                        break;
                }
            } while (input != "exit" && input != "quit");
            Console.WriteLine("---");
            
        }
    }
}

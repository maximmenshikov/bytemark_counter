using System;
using System.Collections.Generic;
using System.Linq;

namespace bytemark_counter
{
    class MainClass
    {

        /// <summary>
        /// Reads the nbench result file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="fileName">File name.</param>
        private static Dictionary<string, double> ReadFile(string fileName)
        {
            var dict = new Dictionary<string, double>();
            var sr = System.IO.File.OpenText(fileName);
            var line = sr.ReadLine();
            bool parse = false;

            while (line != null)
            {
                if (line.StartsWith("--------------------"))
                {
                    parse = true;
                }
                else if (line.StartsWith("=========================="))
                {
                    parse = false;
                }
                else if (parse)
                {
                    var parts = line.Split(':');
                    if (parts.Length < 2)
                    {
                        throw new Exception(
                            "Wrong number of measures in the line: " + line);
                    }
                    dict.Add(parts[0].Trim(), double.Parse(parts[1].Trim()));
                }
                line = sr.ReadLine();
            }
            sr.Close();

            return dict;
        }

        public static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                /* Print usage */
                Console.WriteLine("Usage:");
                Console.WriteLine(System.Reflection.Assembly
                                  .GetExecutingAssembly().GetName().Name +
                                  " filelocation");
                return;
            }

            var path = args[0];
            if (!System.IO.Directory.Exists(path))
            {
                Console.WriteLine("Invalid path: " + path);
                return;
            }

            /* Read file one by one and store in the dictionary */
            var dicts = new Dictionary<string, List<double>>();
            foreach (var file in System.IO.Directory.GetFiles(path))
            {
                var dict = ReadFile(file);
                foreach (var k in dict.Keys)
                {
                    if (!dicts.ContainsKey(k))
                        dicts.Add(k, new List<double>());
                    dicts[k].Add(dict[k]);
                }
            }

            /* Get average score for each measure */
            var scores = dicts.ToDictionary((a) => a.Key,
                                            (b) => b.Value.Average());
            foreach (var score in scores)
            {
                Console.WriteLine(score.Key + " | " + score.Value);
            }
        }
    }
}

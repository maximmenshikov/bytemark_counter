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
        private static Dictionary<string, List<double>> ReadFile(string fileName)
        {
            var dict = new Dictionary<string, List<double>>();
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
                    dict.Add(parts[0].Trim(),
                             new List<double>() { double.Parse(parts[1].Trim()) });
                }
                line = sr.ReadLine();
            }
            sr.Close();

            return dict;
        }

        /// <summary>
        /// Reads the mixed nbench result file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="fileName">File name.</param>
        private static Dictionary<string, List<double>> ReadMixedFile(string fileName)
        {
            var dict = new Dictionary<string, List<double>>();
            var sr = System.IO.File.OpenText(fileName);
            var line = sr.ReadLine();
            var measures = new List<string>() {
                "NUMERIC SORT",
                "STRING SORT",
                "BITFIELD",
                "FP EMULATION",
                "FOURIER",
                "ASSIGNMENT",
                "IDEA",
                "HUFFMAN",
                "NEURAL NET",
                "LU DECOMPOSITION"
            };
            while (line != null)
            {
                var parts = line.Split(':');
                if (parts.Length >= 2 && measures.Contains(parts[0].Trim()))
                {
                    var measure = parts[0].Trim();
                    if (!dict.ContainsKey(measure))
                    {
                        dict.Add(measure, new List<double>() {
                            double.Parse(parts[1].Trim())
                        });
                    }
                    else
                    {
                        dict[measure].Add(double.Parse(parts[1].Trim()));
                    }
                }
                line = sr.ReadLine();
            }
            sr.Close();

            return dict;
        }

        public static void Main(string[] args)
        {
            bool mixed = false;

            if (args.Count() == 0)
            {
                /* Print usage */
                Console.WriteLine("Usage:");
                Console.WriteLine(System.Reflection.Assembly
                                  .GetExecutingAssembly().GetName().Name +
                                  " filelocation" +
                                  " [mixed]");
                return;
            }

            var path = args[0];
            if (!System.IO.Directory.Exists(path))
            {
                Console.WriteLine("Invalid path: " + path);
                return;
            }

            if (args.Count() >= 2 && args[1] == "mixed")
                mixed = true;

            /* Read file one by one and store in the dictionary */
            var dicts = new Dictionary<string, List<double>>();
            foreach (var file in System.IO.Directory.GetFiles(path))
            {
                var dict = mixed ? ReadMixedFile(file) : ReadFile(file);
                foreach (var k in dict.Keys)
                {
                    if (!dicts.ContainsKey(k))
                        dicts.Add(k, new List<double>());
                    foreach (var d in dict[k])
                    {
                        dicts[k].Add(d);
                    }
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

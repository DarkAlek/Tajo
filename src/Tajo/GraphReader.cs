using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;


namespace Tajo
{
    public class GraphReader
    {
        public static Graph ReadCSV(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                var graph = new AdjacencyMatrixGraph(false, values.Length);
                int i = 0;
                int j = 0;
                foreach (var x in values)
                {
                    if (int.Parse(x) == 1)
                        graph.AddEdge(i, j);
                    i++;
                }
                while (!reader.EndOfStream)
                {
                    i = 0;
                    j++;
                    line = reader.ReadLine();
                    values = line.Split(',');
                    foreach (var x in values)
                    {
                        if (int.Parse(x) == 1)
                            graph.AddEdge(i, j);
                        i++;
                    }
                }
                return graph;
            }
        }
		public static void WriteCSV<T>(string path, int selectedOption, Dictionary<T, T> d)
		{
			var g1 = new StringBuilder();
			var g2 = new StringBuilder();
            var l = d.OrderBy(key => key.Key);
            d = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            bool first = true;
			foreach (var x in d)
			{
				if (!first)
				{
					g1.Append(',');
					g2.Append(',');
					
				}
				g1.Append(x.Key);
				g2.Append(x.Value);
				first = false;
			}

			g1.Append("\n");
			g1.Append(g2);
            Console.WriteLine(g1);
			path += "[" + selectedOption.ToString() +"].csv";
			File.WriteAllText(path, g1.ToString());
            Console.WriteLine("Saving solution to " + path);
        }
    }
}

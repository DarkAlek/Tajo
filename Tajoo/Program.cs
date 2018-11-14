using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tajoo
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("TAJO maximal common subgraph project.");
			Console.WriteLine("Press 1 to run exact algorithm.");
			Console.WriteLine("Press 2 to run approximate algorithm no.1");
			Console.WriteLine("Press 3 to run approximate algorithm no.2");

			var x = Console.Read();
			var modularGraph = create_modular_graph();
			
			switch (x)
			{
				case '1':
					Console.WriteLine("Exact algorithm");
					//
					break;
				case '2':
					Console.WriteLine("Approximate no.1");
					//
					break;
				case '3':
					Console.WriteLine("Approximate no.2");
					//
					break;
				default:
					break;
			}
			 
		}

		public static ASD.Graphs.Graph create_modular_graph()
		{
			var matrix1 = read_CSV("..\\..\\graph1.csv");
			var matrix2 = read_CSV("..\\..\\graph2.csv");
			var len1 = matrix1.GetLength(0);
			var len2 = matrix2.GetLength(0);
			var graph = new ASD.Graphs.AdjacencyMatrixGraph(false, len1 * len2);

			int[,] nxt = new int[len1 * len2, len1 * len2];
			for (int i = 0; i < len1; i++)
			{
				for (int j = 0; j < len2; j++)
				{
					for (int ipr = 0; ipr < len1; ipr++)
					{
						for (int jpr = 0; jpr < len2; jpr++)
						{
							if (matrix1[i, ipr] == matrix2[j, jpr] && (i != ipr && j != jpr))
							{
								graph.AddEdge(i + j * len1, ipr + jpr * len1);
								nxt[i + j * len1, ipr + jpr * len1] = 1;
							}
						}
					}
				}
			}

			for (int q = 0; q < len1 * len2; q++)
			{
				for (int w = 0; w < len1 * len2; w++)
				{
					Console.Write(nxt[q, w]);
					if (w != len1 * len2 - 1)
						Console.Write(",");
				}
				Console.WriteLine("");
			}
			return graph;
		}
		public static int[,] read_CSV(string path)
		{
			using (var reader = new StreamReader(path))
			{
				var line = reader.ReadLine();
				var values = line.Split(',');
				int[,] tab = new int[values.Length, values.Length];
				int i = 0;
				int j = 0;
				foreach (var x in values)
				{
					tab[i, j] = int.Parse(x);
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
						tab[i, j] = int.Parse(x);
						i++;
					}
				}
				return tab;
			}
		}
	}

}

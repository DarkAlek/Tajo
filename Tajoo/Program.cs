using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;

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
			var path1 = "..\\..\\graph1.csv";
			var path2 = "..\\..\\graph2.csv";

			var lg1 = create_line_graph(read_CSV(path1));
			var lg2 = create_line_graph(read_CSV(path2));
			var modularGraph = create_modular_graph(lg1,lg2);
			
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
		public static ASD.Graphs.Graph create_line_graph(Graph graph)
		{
			Graph gNew = graph.IsolatedVerticesGraph(false, graph.EdgesCount);
			Graph gPom = graph.IsolatedVerticesGraph();

			for (int v = 0; v < graph.VerticesCount; ++v)
			{
				foreach (Edge e in graph.OutEdges(v))
				{
					if (e.From < e.To)
					{
						gPom.AddEdge(e.From, e.To);
						
					}
				}
			}

			for (int v = 0; v < gPom.VerticesCount; ++v)
			{
				foreach (Edge e1 in gPom.OutEdges(v))
				{
					foreach (Edge e2 in gPom.OutEdges(e1.To))
					{
						if (e1.From != e2.To)
						{
							gNew.AddEdge((int)e1.Weight, (int)e2.Weight);
						}
					}
				}
			}
			return gNew;
		}
		public static Graph create_modular_graph(Graph graph1, Graph graph2)
		{			
			var len1 = graph1.VerticesCount;
			var len2 = graph2.VerticesCount;
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
							if (double.IsNaN(graph1.GetEdgeWeight(i, ipr))== double.IsNaN(graph2.GetEdgeWeight(j, jpr)) && (i != ipr && j != jpr))
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
		public static Graph read_CSV(string path)
		{
			using (var reader = new StreamReader(path))
			{
				var line = reader.ReadLine();
				var values = line.Split(',');
				var graph = new ASD.Graphs.AdjacencyMatrixGraph(false, values.Length);
				int i = 0;
				int j = 0;
				foreach (var x in values)
				{
					if(int.Parse(x)==1)
					graph.AddEdge(i,j);
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
						if(int.Parse(x)==1)
						graph.AddEdge(i, j);
						i++;
					}
				}
				return graph;
			}
		}
		public static Graph Greedy(Graph g)
		{
			if (g.VerticesCount == 0)
				return null;
			// return g[max_degree(g)] u Greedy( N(g[max_degree(g)]) todoogarniecia
			return null;

		}
		public int max_degree(Graph g)
		{
			int max = int.MinValue;
			int id=-1;
			for(int i=0;i<g.VerticesCount;i++)
			{
				if(g.InDegree(i)>max)
				{
					max = g.InDegree(i);
					id = i;
				}
			}
			return id;
		}
	}

}

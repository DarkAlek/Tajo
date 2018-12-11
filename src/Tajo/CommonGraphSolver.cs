using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;

namespace Tajo
{
	public class CommonGraphSolver
	{
		private Graph graph1;
		private Graph graph2;
		private Graph modularProductGraph;
		private (int x, int y)[] namesToModularProductGraph;
        private bool flag = true;

		public Graph Graph1 { get => graph1; set => graph1 = value; }
		public Graph Graph2 { get => graph2; set => graph2 = value; }
		public Graph ModularProductGraphVertices { get => modularProductGraph; set => modularProductGraph = value; }
		public (int x, int y)[] NamesToModularProductGraphVertices { get => namesToModularProductGraph; set => namesToModularProductGraph = value; }
		public CommonGraphSolver(Graph graph1, Graph graph2)
		{
			this.Graph1 = graph1;
			this.Graph2 = graph2;
			modularProductGraph = CreateModularGraph(graph1, graph2, out namesToModularProductGraph);

		}

		private Graph CreateLineGraph(Graph graph, out (int x, int y)[] names)
		{
			Graph gNew = graph.IsolatedVerticesGraph(false, graph.EdgesCount);
			Graph gPom = graph.IsolatedVerticesGraph();
			(int x, int y)[] namesPom = new(int x, int y)[graph.EdgesCount];
			int itV = 0;

			for (int v = 0; v < graph.VerticesCount; ++v)
			{
				foreach (Edge e in graph.OutEdges(v))
				{
					if (e.From < e.To)
					{
						gPom.AddEdge(e.From, e.To, itV);
						namesPom[itV] = (e.From, e.To);
						itV++;

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
			names = namesPom;
			return gNew;
		}

		private Graph CreateModularGraph(Graph graph1, Graph graph2, out (int x, int y)[] names)
		{
			var len1 = graph1.VerticesCount;
			var len2 = graph2.VerticesCount;
			var graph = new AdjacencyMatrixGraph(false, len1 * len2);
			(int x, int y)[] namesPom = new(int x, int y)[graph.VerticesCount];

			for (int i = 0; i < len1; i++)
			{
				for (int j = 0; j < len2; j++)
				{
					for (int ipr = 0; ipr < len1; ipr++)
					{
						for (int jpr = 0; jpr < len2; jpr++)
						{
							if (!graph1.GetEdgeWeight(i, ipr).IsNaN() && !graph2.GetEdgeWeight(j, jpr).IsNaN() && (i != ipr && j != jpr))
							{
								graph.AddEdge(i + j * len1, ipr + jpr * len1);
								namesPom[i + j * len1] = (i, j);
								namesPom[ipr + jpr * len1] = (ipr, jpr);
							}
							else if (graph1.GetEdgeWeight(i, ipr).IsNaN() && graph2.GetEdgeWeight(j, jpr).IsNaN() && (i != ipr && j != jpr))
							{
								graph.AddEdge(i + j * len1, ipr + jpr * len1, -1);
								namesPom[i + j * len1] = (i, j);
								namesPom[ipr + jpr * len1] = (ipr, jpr);
							}
						}
					}
				}
			}

			names = namesPom;
			return graph;
		}

		private Dictionary<int, int> TranslateResultCliqueToSolutionVertices(HashSet<int> clique)
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			int i = 0;

			foreach (var v in clique)
			{
				result.Add(namesToModularProductGraph[v].x, namesToModularProductGraph[v].y);

				i++;
			}

			return result;
		}

		private Dictionary<int, int> FindMaxVerticesClique(List<HashSet<int>> C)
		{
			int maxSum = int.MinValue;
			HashSet<int> maxClique = null;

			foreach (var c in C)
			{
				int sum = c.Count;
				if (maxSum < sum)
				{
					maxClique = c;
					maxSum = sum;
				}
			}
			return TranslateResultCliqueToSolutionVertices(maxClique);
		}


		private Dictionary<int, int> FindMaxVerticesEdgesClique(List<HashSet<int>> C)
		{
			int maxSum = int.MinValue;
			HashSet<int> maxClique = null;

			foreach (var c in C)
			{
				int sum = 0;
				Dictionary<int, int> mapping = TranslateResultCliqueToSolutionVertices(c);
				sum += mapping.Count;
				foreach (var v in mapping.Keys)
				{
					foreach (var e in graph1.OutEdges(v))
					{
						if (e.From < e.To)
						{
							if (mapping.Keys.Contains(e.To))
							{
								sum += 1;
							}
						}
					}
				}
				if (maxSum < sum)
				{
					maxClique = c;
					maxSum = sum;
				}
			}
			return TranslateResultCliqueToSolutionVertices(maxClique);
		}


		private void InitBronKerboschKoch(Graph g, HashSet<int> R, HashSet<int> P, HashSet<int> D, HashSet<int> X, ref List<HashSet<int>> C)
		{
			HashSet<int> T = new HashSet<int>();

			for (int u = 0; u < g.VerticesCount; ++u)
			{
                if (flag == false) break;
				P = new HashSet<int>();
				D = new HashSet<int>();
				X = new HashSet<int>();
				HashSet<int> Nv = new HashSet<int>();
				foreach (var e in g.OutEdges(u))
				{
					Nv.Add(e.To);
				}
				foreach (var v in Nv)
				{
					if (g.GetEdgeWeight(u, v) == 1)
					{
						if (T.Contains(v))
						{
							X.Add(v);
						}
						else
						{
							P.Add(v);
						}
					}
					else if (g.GetEdgeWeight(u, v) == -1)
					{
						D.Add(v);
					}
				}

				HashSet<int> uSet = new HashSet<int>();
				uSet.Add(u);
				BronKerboschKoch(g, uSet, P, D, X, ref T, ref C);

				T.UnionWith(uSet);
			}
		}

		private void BronKerboschKoch(Graph g, HashSet<int> R, HashSet<int> P, HashSet<int> D, HashSet<int> X, ref HashSet<int> T, ref List<HashSet<int>> C)
		{
            if(flag == false)
            {
                return;
            }

            if(R.Count == graph1.VerticesCount || R.Count == graph2.VerticesCount)
            {
                flag = false;
            }

			if (P.Count == 0 && X.Count == 0)
			{
				C.Add(R);
			}
			else
			{
				HashSet<int> Ppom = new HashSet<int>(P);
				foreach (var u in Ppom)
				{
                    if (flag == false)
                    {
                        return;
                    }
                    P.Remove(u);
					HashSet<int> Ptmp = new HashSet<int>(P);
					HashSet<int> Dtmp = new HashSet<int>(D);
					HashSet<int> Xtmp = new HashSet<int>(X);
					HashSet<int> Nu = new HashSet<int>();

					foreach (var e in g.OutEdges(u))
					{
						Nu.Add(e.To);
					}

					foreach (var v in D)
					{
						if (g.GetEdgeWeight(v, u) == 1)
						{
							if (T.Contains(v))
							{
								Xtmp.Add(v);
							}
							else
							{
								Ptmp.Add(v);
							}

							Dtmp.Remove(v);
						}
					}

					HashSet<int> uSet = new HashSet<int>();
					uSet.Add(u);

					BronKerboschKoch(g, new HashSet<int>(R.Union(uSet)), new HashSet<int>(Ptmp.Intersect(Nu)),
										new HashSet<int>(Dtmp.Intersect(Nu)), new HashSet<int>(Xtmp.Intersect(Nu)), ref T, ref C);

					X.UnionWith(uSet);
				}
			}
		}

		private HashSet<int> Greedy(Graph g, HashSet<int> vertices)
		{

			if (vertices.Count == 0)
			{
				return new HashSet<int>();
			}

			else
			{
				int i = 0;
				int pivot = 0;
				int maxDegree = int.MinValue;

				foreach (var v in vertices)
				{
					foreach (var e in g.OutEdges(v))
					{
						if (vertices.Contains(e.To)) i++;
					}

					if (i > maxDegree)
					{
						maxDegree = i;
						pivot = v;
					}

					i = 0;
				}
				var neighbours = new HashSet<int>();

				foreach (var e in g.OutEdges(pivot))
				{
					if (vertices.Contains(e.To))
					{
						neighbours.Add(e.To);
					}

				}

				var C = Greedy(g, neighbours);
				C.Add(pivot);
				return C;
			}

		}

		private (HashSet<int>, HashSet<int>) Ramsey(Graph g, HashSet<int> vertices)
		{
			if (vertices.Count == 0)
			{
				return (new HashSet<int>(), new HashSet<int>());
			}

			else
			{
				int i = 0;
				int pivot = 0;
				int maxDegree = int.MinValue;

				foreach (var v in vertices)
				{
					foreach (var e in g.OutEdges(v))
					{
						if (vertices.Contains(e.To)) i++;
					}

					if (i > maxDegree)
					{
						maxDegree = i;
						pivot = v;
					}

					i = 0;
				}
				var neighbours = new HashSet<int>();
				var notNeighbours = new HashSet<int>();


				foreach (var e in g.OutEdges(pivot))
				{
					if (vertices.Contains(e.To))
					{
						neighbours.Add(e.To);
					}

				}

				notNeighbours = new HashSet<int>(vertices.Except(neighbours));
				notNeighbours.Remove(pivot);

				(HashSet<int> C1, HashSet<int> I1) = Ramsey(g, neighbours);
				(HashSet<int> C2, HashSet<int> I2) = Ramsey(g, notNeighbours);

				C1.Add(pivot);
				I2.Add(pivot);

				HashSet<int> CR = C1.Count >= C2.Count ? C1 : C2;
				HashSet<int> IR = I1.Count >= I2.Count ? I1 : I2;

				return (CR, IR);
			}

		}

		private HashSet<int> ISRemoval(Graph g)
		{
			List<(HashSet<int>, HashSet<int>)> results = new List<(HashSet<int>, HashSet<int>)>();
			HashSet<int> vertices = new HashSet<int>();
			for (int v = 0; v < g.VerticesCount; ++v)
			{
				vertices.Add(v);
			}

			results.Add(Ramsey(g, vertices));

			while (vertices.Count > 0)
			{
				vertices = new HashSet<int>(vertices.Except(results.Last().Item2));
				results.Add(Ramsey(g, vertices));
			}

			HashSet<int> maxC = new HashSet<int>();
			int maxCount = int.MinValue;

			foreach (var el in results)
			{
				if (el.Item1.Count > maxCount)
				{
					maxC = el.Item1;
					maxCount = el.Item1.Count;
				}
			}

			return maxC;

		}


		public Dictionary<int, int> ExactAlghoritmVertices()
		{
			Graph graph = modularProductGraph;
			HashSet<int> R = new HashSet<int>();
			HashSet<int> P = new HashSet<int>();
			HashSet<int> D = new HashSet<int>();
			HashSet<int> X = new HashSet<int>();
			List<HashSet<int>> C = new List<HashSet<int>>();


			InitBronKerboschKoch(graph, R, P, D, X, ref C);
            flag = true;

			//translate C
			Dictionary<int, int> result = FindMaxVerticesClique(C);

			return result;
		}

		public Dictionary<int, int> ExactAlghoritmVerticesEdges()
		{
			Graph graph = modularProductGraph;
			HashSet<int> R = new HashSet<int>();
			HashSet<int> P = new HashSet<int>();
			HashSet<int> D = new HashSet<int>();
			HashSet<int> X = new HashSet<int>();
			List<HashSet<int>> C = new List<HashSet<int>>();


			InitBronKerboschKoch(graph, R, P, D, X, ref C);
            flag = true;

			Dictionary<int, int> result = FindMaxVerticesEdgesClique(C);

			return result;
		}

		public Dictionary<int, int> ApproximateAlgorithm1()
		{
			Graph graph = modularProductGraph;
			HashSet<int> vertices = new HashSet<int>();
			for (int v = 0; v < graph.VerticesCount; ++v)
			{
				vertices.Add(v);
			}

			HashSet<int> C = Greedy(graph, vertices);

			//translate C
			Dictionary<int, int> result = TranslateResultCliqueToSolutionVertices(C);
			result = validateSolution(result);

			return result;
		}
		
		public Dictionary<int, int> ApproximateAlgorithm2()
		{
			Graph graph = modularProductGraph;

			HashSet<int> C = ISRemoval(graph);

			//translate C
			Dictionary<int, int> result = TranslateResultCliqueToSolutionVertices(C);

			result = validateSolution(result);
			return result;
		}

		private Dictionary<int, int> validateSolution(Dictionary<int, int> result)
		{
			var x = result.Count;
			var hs = new HashSet<int>();
			var dict_id_val = new Dictionary<int, int>();
			var dict_val_id = new Dictionary<int, int>();
			var g = new AdjacencyMatrixGraph(false, x);
			int j = 0;

			//indeksy stare-nowe
			foreach (var v in result.Keys)
			{
				hs.Add(v);
				dict_id_val.Add(j, v);
				dict_val_id.Add(v, j);
				j++;
			}

			//graf
			foreach (var v in hs)
			{
				foreach (var e in graph1.OutEdges(v))
				{
					if (hs.Contains(e.To) && e.To != v)
					{
						int v1, v2;
						dict_val_id.TryGetValue(e.To, out v1);
						dict_val_id.TryGetValue(v, out v2);

						g.AddEdge(v1, v2);
					}

				}
			}

			int counter = 0;
			int cc = 0;
			var l =new List<HashSet<int>>();

			Predicate<int> vv = delegate (int n)
			{
				if (counter == cc - 1)
				{
					l.Add(new HashSet<int>());
				}
				l[l.Count - 1].Add(n);
				counter = cc;
				return true;
			};

			g.DFSearchAll(vv, null, out cc);

            if (cc != 1)
            {
                //max spójny
                int maxCount = int.MinValue;
                HashSet<int> maxComponent = new HashSet<int>();

                foreach (var hss in l)
                {
                    if (hss.Count > maxCount)
                    {
                        maxCount = hss.Count;
                        maxComponent = hss;
                    }
                }
                var res = new Dictionary<int, int>();
                int ve1 = int.MinValue;
                int ve2 = int.MinValue;

                foreach (var ve in maxComponent)
                {
                    dict_id_val.TryGetValue(ve, out ve1);
                    result.TryGetValue(ve1, out ve2);
                    res.Add(ve1, ve2);
                }
                return res;
            }

            return result;

		}
	}
}

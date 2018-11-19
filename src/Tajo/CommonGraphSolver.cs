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
        private Graph lineGraph1;
        private Graph lineGraph2;
        private Graph modularProductGraphVertices;
        private Graph modularProductGraphEdges;
        private (int x, int y)[] namesToLineGraph1;
        private (int x, int y)[] namesToLineGraph2;
        private (int x, int y)[] namesToModularProductGraphVertices;
        private (int x, int y)[] namesToModularProductGraphEdges;

        public Graph Graph1 { get => graph1; set => graph1 = value; }
        public Graph Graph2 { get => graph2; set => graph2 = value; }
        public Graph LineGraph1 { get => lineGraph1; set => lineGraph1 = value; }
        public Graph LineGraph2 { get => lineGraph2; set => lineGraph2 = value; }
        public (int x, int y)[] NamesToLineGraph1 { get => namesToLineGraph1; set => namesToLineGraph1 = value; }
        public (int x, int y)[] NamesToLineGraph2 { get => namesToLineGraph2; set => namesToLineGraph2 = value; }
        public Graph ModularProductGraphVertices { get => modularProductGraphVertices; set => modularProductGraphVertices = value; }
        public Graph ModularProductGraphEdge { get => modularProductGraphEdges; set => modularProductGraphEdges = value; }
        public (int x, int y)[] NamesToModularProductGraphVertices { get => namesToModularProductGraphVertices; set => namesToModularProductGraphVertices = value; }
        public (int x, int y)[] NamesToModularProductGraphEdge { get => namesToModularProductGraphEdges; set => namesToModularProductGraphEdges = value; }

        public CommonGraphSolver(Graph graph1, Graph graph2)
        {
            this.Graph1 = graph1;
            this.Graph2 = graph2;
            lineGraph1 = CreateLineGraph(graph1, out namesToLineGraph1);
            lineGraph2 = CreateLineGraph(graph2, out namesToLineGraph2);
            modularProductGraphVertices = CreateModularGraph(graph1, graph2, out namesToModularProductGraphVertices);
            modularProductGraphEdges = CreateModularGraph(lineGraph1, lineGraph2, out namesToModularProductGraphEdges);
        }

        private Graph CreateLineGraph(Graph graph, out (int x, int y)[] names)
        {
            Graph gNew = graph.IsolatedVerticesGraph(false, graph.EdgesCount);
            Graph gPom = graph.IsolatedVerticesGraph();
            (int x, int y)[] namesPom = new (int x, int y)[graph.EdgesCount];
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
            (int x, int y)[] namesPom = new (int x, int y)[graph.VerticesCount];

            //int[,] nxt = new int[len1 * len2, len1 * len2];
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    for (int ipr = 0; ipr < len1; ipr++)
                    {
                        for (int jpr = 0; jpr < len2; jpr++)
                        {
                            if (double.IsNaN(graph1.GetEdgeWeight(i, ipr)) == double.IsNaN(graph2.GetEdgeWeight(j, jpr)) && (i != ipr && j != jpr))
                            {
                                graph.AddEdge(i + j * len1, ipr + jpr * len1);
                                //nxt[i + j * len1, ipr + jpr * len1] = 1;
                                namesPom[i + j * len1] = (i, j);
                                namesPom[ipr + jpr * len1] = (ipr, jpr);
                            }
                        }
                    }
                }
            }

            //for (int q = 0; q < len1 * len2; q++)
            //{
            //    for (int w = 0; w < len1 * len2; w++)
            //    {
            //        Console.Write(nxt[q, w]);
            //        if (w != len1 * len2 - 1)
            //            Console.Write(",");
            //    }
            //    Console.WriteLine("");
            //}
            names = namesPom;
            return graph;
        }

        private Dictionary<int, int> TranslateResultCliqueToSolutionVertices(HashSet<int> clique)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            int i = 0;

            foreach(var v in clique)
            {
                result.Add(namesToModularProductGraphVertices[v].x, namesToModularProductGraphVertices[v].y);
            
                i++;
            }

            return result;
        }

        private  Dictionary<int, int> TranslateResultCliqueToSolutionEdges(HashSet<int> clique)
        {
            Dictionary<(int x, int y), (int x, int y)> resultPom = new Dictionary<(int x, int y), (int x, int y)>();
            Dictionary<int, int> result = new Dictionary<int, int>();

            foreach (var v in clique)
            {
                resultPom.Add(namesToLineGraph1[namesToModularProductGraphEdges[v].x], namesToLineGraph2[namesToModularProductGraphEdges[v].y]);
            }

            Graph gPom1 = new AdjacencyMatrixGraph(false, resultPom.Count + 1);
            Graph gPom2 = new AdjacencyMatrixGraph(false, resultPom.Count + 1);


            foreach (var el in resultPom.Keys)
            {
                gPom1.AddEdge(el.x, el.y);

            }

            foreach (var el in resultPom.Values)
            {
                gPom2.AddEdge(el.x, el.y);

            }

            // visualize common graphs
            //GraphExport ge = new GraphExport();
            //ge.Export(gPom1);
            //ge.Export(gPom2);

            for (int v = 0; v < gPom1.VerticesCount; ++v)
            {
                foreach(var e in gPom1.OutEdges(v))
                {
                    if(resultPom.Keys.Contains((e.From, e.To)))
                    {
                        if (!result.Keys.Contains(e.From))
                        {
                            result.Add(e.From, resultPom[(e.From, e.To)].x);
                        }
                        if(!result.Keys.Contains(e.To))
                        {
                            result.Add(e.To, resultPom[(e.From, e.To)].y);
                        }

                    }
                }
            }

            // delta-Y exchange check 
            IEnumerable<int> keys = result.Keys;
            foreach(var key in keys)
            {
                if(gPom1.OutDegree(key) != gPom2.OutDegree(result[key]))
                {
                    result.Remove(key);
                }
            }


            return result;
        }

        private int MaxDegree(Graph g)
        {
            int max = int.MinValue;
            int id = -1;
            for (int i = 0; i < g.VerticesCount; i++)
            {
                if (g.InDegree(i) > max)
                {
                    max = g.InDegree(i);
                    id = i;
                }
            }
            return id;
        }

        private void BronKerbosch(Graph g, HashSet<int> R, HashSet<int> P, HashSet<int> X, ref HashSet<int> C)
        {
            if(P.Count == 0 && X.Count == 0)
            {
                if(R.Count > C.Count)
                {
                    C = R;
                }
            }
            else
            {
                HashSet<int> PuX = new HashSet<int>(P.Union(X));
                int i = 0;
                int pivot = -1;
                int maxDegreeInP = int.MinValue;

                foreach(var v in PuX)
                {
                    foreach(var e in g.OutEdges(v))
                    {
                        if (P.Contains(e.To)) i++;
                    }
                    
                    if(i > maxDegreeInP)
                    {
                        maxDegreeInP = i;
                        pivot = v;
                    }

                    i = 0;
                }

                HashSet<int> Nu = new HashSet<int>();

                foreach(var e in g.OutEdges(pivot))
                {
                    Nu.Add(e.To);
                }

                HashSet<int> PeNu = new HashSet<int>(P.Except(Nu));
                foreach(var v in PeNu)
                {
                    HashSet<int> Nv = new HashSet<int>();

                    foreach (var e in g.OutEdges(v))
                    {
                        Nv.Add(e.To);
                    }
                    HashSet<int> vSet = new HashSet<int>();
                    vSet.Add(v);

                    BronKerbosch(g, new HashSet<int>(R.Union(vSet)), new HashSet<int>(P.Intersect(Nv)), new HashSet<int>(X.Intersect(Nv)), ref C);
                    P.Remove(v);
                    X.Add(v);

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
				int pivot=0;
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

                //for (int j = 0; j < g.VerticesCount; j++)
                //{
                //    if (!double.IsNaN(g.GetEdgeWeight(pivot, j)) && vertices.Contains(j))
                //    {
                //        hs.Add(j);
                //    }
                //}

                foreach (var e in g.OutEdges(pivot))
                {
                    if (vertices.Contains(e.To))
                    {
                        neighbours.Add(e.To);
                    }

                }

                var C  = Greedy(g, neighbours);
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

            while(vertices.Count > 0)
            {
                vertices = new HashSet<int>(vertices.Except(results.Last().Item2));
                results.Add(Ramsey(g, vertices));
            }

            HashSet<int> maxC = new HashSet<int>();
            int maxCount = int.MinValue;

            foreach(var el in results)
            {
                if(el.Item1.Count > maxCount)
                {
                    maxC = el.Item1;
                    maxCount = el.Item1.Count;
                }
            }

            return maxC;

        }


        public Dictionary<int, int> ExactAlghoritmVertices()
        {
            Graph graph = modularProductGraphVertices;
            HashSet<int> R = new HashSet<int>();
            HashSet<int> P = new HashSet<int>();
            HashSet<int> X = new HashSet<int>();
            HashSet<int> C = new HashSet<int>();

            for (int v=0; v <  graph.VerticesCount; ++v)
            {
                P.Add(v);
            }

            BronKerbosch(graph, R, P, X, ref C);

            //translate C
            Dictionary<int, int> result = TranslateResultCliqueToSolutionVertices(C);

            return result;
        }

        public Dictionary<int, int> ExactAlghoritmEdges()
        {
            Graph graph = modularProductGraphEdges;
            HashSet<int> R = new HashSet<int>();
            HashSet<int> P = new HashSet<int>();
            HashSet<int> X = new HashSet<int>();
            HashSet<int> C = new HashSet<int>();

            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                P.Add(v);
            }

            BronKerbosch(graph, R, P, X, ref C);

            Dictionary<int, int> result = TranslateResultCliqueToSolutionEdges(C);

            return result;
        }

        public Dictionary<int, int> ApproximateAlgorithm1Vertices()
        {
            Graph graph = modularProductGraphVertices;
            HashSet<int> vertices = new HashSet<int>();
            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                vertices.Add(v);
            }

            HashSet<int> C =  Greedy(graph, vertices);
            //translate C
            Dictionary<int, int> result = TranslateResultCliqueToSolutionVertices(C);

            return result;
        }

        public Dictionary<int, int> ApproximateAlgorithm1Edges()
        {
            Graph graph = modularProductGraphEdges;
            HashSet<int> vertices = new HashSet<int>();
            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                vertices.Add(v);
            }

            HashSet<int> C = Greedy(graph, vertices);
            //translate C
            Dictionary<int, int> result = TranslateResultCliqueToSolutionEdges(C);

            return result;
        }

        public Dictionary<int, int> ApproximateAlgorithm2Vertices()
        {
            //TO DO
            Graph graph = modularProductGraphVertices;

            HashSet<int> C = ISRemoval(graph);
            //translate C
            Dictionary<int, int> result = TranslateResultCliqueToSolutionVertices(C);

            return result;
        }

        public Dictionary<int, int> ApproximateAlgorithm2Edges()
        {
            //TO DO
            Graph graph = modularProductGraphEdges;
            HashSet<int> C = ISRemoval(graph);
            //translate C
            Dictionary<int, int> result = TranslateResultCliqueToSolutionEdges(C);

            return result;
        }

    }
}

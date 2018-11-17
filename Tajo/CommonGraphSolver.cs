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
        private Graph modularProductGraph;
        private (int x, int y)[] namesToLineGraph1;
        private (int x, int y)[] namesToLineGraph2;
        private (int x, int y)[] namesToModularProductGraph;

        public Graph Graph1 { get => graph1; set => graph1 = value; }
        public Graph Graph2 { get => graph2; set => graph2 = value; }
        public Graph LineGraph1 { get => lineGraph1; set => lineGraph1 = value; }
        public Graph LineGraph2 { get => lineGraph2; set => lineGraph2 = value; }
        public Graph ModularProductGraph { get => modularProductGraph; set => modularProductGraph = value; }
        public (int x, int y)[] NamesToLineGraph1 { get => namesToLineGraph1; set => namesToLineGraph1 = value; }
        public (int x, int y)[] NamesToLineGraph2 { get => namesToLineGraph2; set => namesToLineGraph2 = value; }
        public (int x, int y)[] NamesToModularProductGraph { get => namesToModularProductGraph; set => namesToModularProductGraph = value; }

        public CommonGraphSolver(Graph graph1, Graph graph2)
        {
            this.Graph1 = graph1;
            this.Graph2 = graph2;
            lineGraph1 = CreateLineGraph(graph1, out namesToLineGraph1);
            lineGraph2 = CreateLineGraph(graph2, out namesToLineGraph2);
            modularProductGraph = CreateModularGraph(lineGraph1, lineGraph2, out namesToModularProductGraph);
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

        private Graph BronKerbosch(Graph g)
        {
            // TO DO
            return null;

        }


        private Graph Greedy(Graph g)
        {
            // TO DO
            return null;

        }

        private Graph Ramsey(Graph g)
        {
            // TO DO
            return null;

        }

        private Graph ISRemoval(Graph g)
        {
            // TO DO
            return null;

        }

        public int[,] ExactAlghoritm()
        {
            //TO DO
            Graph graph = modularProductGraph;

            return null;
        }

        public int[,] ApproximateAlgorithm1()
        {
            //TO DO
            Graph graph = modularProductGraph;
            return null;
        }

        public int[,] ApproximateAlgorithm2()
        {
            //TO DO
            Graph graph = modularProductGraph;
            return null;
        }

    }
}

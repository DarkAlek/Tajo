using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Tajo
{
	public class Program
	{
        [STAThread]
        static void Main(string[] args)
		{
            DateTime startTime, endTime;
            char x = '0'; 
            char y = '0';
            Dictionary<int, int> output1;
            Dictionary<int, int> output2;
            GraphExport ge = new GraphExport();

            // Load *.csv with Open Dialog 
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.ShowDialog();
            var path_input1 = ofd1.FileName;

            OpenFileDialog ofd2 = new OpenFileDialog();
            ofd2.ShowDialog();
            var path_input2 = ofd2.FileName;

            if (path_input1 == "" || path_input2 == "")
                return;

            Console.WriteLine("Reading graphs from .csv...");
            var graph1 = GraphReader.ReadCSV(path_input1);
            var graph2 = GraphReader.ReadCSV(path_input2);
            CommonGraphSolver gs = new CommonGraphSolver(graph1, graph2);
            var modularGraph = gs.ModularProductGraphVertices;

            bool flag = true;

            while (flag)
            {
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine("TAJO maximal common subgraph project.");
                Console.WriteLine("Press 1 to run exact algorithm.");
                Console.WriteLine("Press 2 to run approximate algorithm no.1");
                Console.WriteLine("Press 3 to run approximate algorithm no.2");
                Console.WriteLine("---------------------------------------------------------");

                var inputString = Console.ReadLine();
                try
                {
                    x = inputString[0];
                }
                catch
                {
                    x = '0';
                }

                if(x == '1')
                {
                    Console.WriteLine("Press relevant key to choose exact alghoritm version:");
                    Console.WriteLine(" 1) Vertices");
                    Console.WriteLine(" 2) Vertices + Edges");
                    inputString = Console.ReadLine();
                    try
                    {
                        y = inputString[0];
                    }
                    catch
                    {
                        y = '0';
                    }

                }

                // TODO: Visualize result common graphs
                // visualize graphs
                //ge.Export(gs.Graph1);
                //ge.Export(gs.Graph2);
                //ge.Export(gs.ModularProductGraphVertices);

                var path_output1 = path_input1.Remove(path_input1.Length - 12) + "result1";
                var path_output2 = path_input1.Remove(path_input2.Length - 12) + "result2";

                switch (x)
                {
                    case '1':
                        if (y == '1')
                        {
                            Console.WriteLine("Exact algorithm - computing vertices...");
                            startTime = DateTime.Now;
                            output1 = gs.ExactAlghoritmVertices();
                            endTime = DateTime.Now;
                            if (output1 != null)
                            {
                                GraphReader.WriteCSV(path_output1, 1, output1);
                            }
                            Console.WriteLine(endTime - startTime);
                            if (graph1.VerticesCount <= 30 && graph2.VerticesCount <= 30)
                            {
                                VisualizeResultGraphs(ge, graph1, graph2, output1);
                            }
                        }
                        else if (y == '2')
                        {
                            Console.WriteLine("Exact algorithm - computing vertices + edges...");
                            startTime= DateTime.Now;
                            output2 = gs.ExactAlghoritmVerticesEdges();
                            endTime = DateTime.Now;
                            if (output2 != null)
                            {
                                GraphReader.WriteCSV(path_output2, 1, output2);
                            }
                            Console.WriteLine(endTime - startTime);
                            if (graph1.VerticesCount <= 30 && graph2.VerticesCount <= 30)
                            {
                                VisualizeResultGraphs(ge, graph1, graph2, output2);
                            }
                        }
                        break;

                    case '2':
                        Console.WriteLine("ApproximateAlgorithm1 - computing...");
                        startTime = DateTime.Now;
                        output1 = gs.ApproximateAlgorithm1();
                        endTime = DateTime.Now;
                        if (output1 != null)
                        {
                            GraphReader.WriteCSV(path_output1, 2, output1);
                        }
                        Console.WriteLine(endTime - startTime + " ms");
                        if (graph1.VerticesCount <= 30 && graph2.VerticesCount <= 30)
                        {
                            VisualizeResultGraphs(ge, graph1, graph2, output1);
                        }
                        break;
                    case '3':
                        Console.WriteLine("ApproximateAlgorithm2 - computing...");
                        startTime = DateTime.Now;
                        output1 = gs.ApproximateAlgorithm2();
                        endTime = DateTime.Now;
                        if (output1 != null)
                        {
                            GraphReader.WriteCSV(path_output1, 3, output1);
                        }
                        Console.WriteLine(endTime - startTime + " ms");
                        if (graph1.VerticesCount <= 30 && graph2.VerticesCount <= 30)
                        {
                            VisualizeResultGraphs(ge, graph1, graph2, output1);
                        }
                        break;
                    default:
                        break;
                }
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine("Press [ESC] to exit...");
                Console.WriteLine("Press [BACKSPACE] to load new graphs...");
                Console.WriteLine("Press any other button to contiune calculations on given graphs...");
                var keyPressed = Console.ReadKey();
                if(keyPressed.Key == ConsoleKey.Backspace)
                {
                    ofd1 = new OpenFileDialog();
                    ofd1.ShowDialog();
                    path_input1 = ofd1.FileName;

                    ofd2 = new OpenFileDialog();
                    ofd2.ShowDialog();
                    path_input2 = ofd2.FileName;

                    if (path_input1 == "" || path_input2 == "")
                        return;

                    Console.WriteLine("Reading graphs from .csv...");
                    graph1 = GraphReader.ReadCSV(path_input1);
                    graph2 = GraphReader.ReadCSV(path_input2);
                    gs = new CommonGraphSolver(graph1, graph2);
                    modularGraph = gs.ModularProductGraphVertices;
                }
                if (keyPressed.Key == ConsoleKey.Escape) flag = false;
            }
        }

        private static void VisualizeResultGraphs(GraphExport ge, Graph g1, Graph g2, Dictionary<int, int> mapping)
        {
            String[] verticesDescriptions1 = new String[g1.VerticesCount];
            String[] verticesDescriptions2 = new String[g2.VerticesCount];

            for(int it = 0; it < verticesDescriptions1.Length; ++it)
            {
                verticesDescriptions1[it] = it.ToString();
            }

            for (int it = 0; it < verticesDescriptions2.Length; ++it)
            {
                verticesDescriptions2[it] = it.ToString();
            }

            int i = 0;
            foreach(var el in mapping)
            {
                verticesDescriptions1[el.Key] = el.Key.ToString() + "(" + i.ToString() + ")";
                verticesDescriptions2[el.Value] = el.Value.ToString() + "(" + i.ToString() + ")";
                i++;
            }

            try
            {
                ge.Export(g1, null, verticesDescriptions1);
                ge.Export(g2, null, verticesDescriptions2);
            }
            catch
            {
                Console.WriteLine("GraphViz isn't installed. Graphs will be not visualized.");
            }
        }

	}

}

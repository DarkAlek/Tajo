using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;
using System.Diagnostics;

namespace Tajo
{
	public class Program
	{
		static void Main(string[] args)
		{
            Stopwatch stopwatch;
            GraphExport ge = new GraphExport();
            // TODO: Load *.csv with Open Dialog
            var path_input1 = "..\\data\\7_7_A_Ciesłowski.csv";
            var path_input2 = "..\\data\\7_7_B_Ciesłowski.csv";

            Console.WriteLine("Reading graphs from .csv...");
            var graph1 = GraphReader.ReadCSV(path_input1);
            var graph2 = GraphReader.ReadCSV(path_input2);
            CommonGraphSolver gs = new CommonGraphSolver(graph1, graph2);
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
                var x = inputString[0];

                // TODO: Visualize result common graphs
                // visualize graphs
                //ge.Export(gs.Graph1);
                //ge.Export(gs.Graph2);
                //ge.Export(gs.LineGraph1);
                //ge.Export(gs.LineGraph2);
                //ge.Export(gs.ModularProductGraphVertices);
                //ge.Export(gs.ModularProductGraphEdges);
                var path_output1 = path_input1.Remove(path_input1.Length - 12) + "result1";
                var path_output2 = path_input1.Remove(path_input2.Length - 12) + "result2";

                switch (x)
                {
                    case '1':
                        Console.WriteLine("Exact algorithm - computing vertices...");
                        stopwatch = Stopwatch.StartNew();
                        var output1 = gs.ExactAlghoritmVertices();
                        if (output1 != null)
                        {
                            GraphReader.WriteCSV(path_output1, 1, output1);
                        }
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
                        stopwatch.Reset();
                        stopwatch.Start();
                        Console.WriteLine("Exact algorithm - computing edges...");
                        var output2 = gs.ExactAlghoritmEdges();
                        if (output2 != null)
                        {
                            GraphReader.WriteCSV(path_output2, 1, output2);
                        }
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");


                        break;
                    case '2':
                        Console.WriteLine("ApproximateAlgorithm1 - computing vertices...");
                        stopwatch = Stopwatch.StartNew();
                        output1 = gs.ApproximateAlgorithm1Vertices();
                        if (output1 != null)
                        {
                            GraphReader.WriteCSV(path_output1, 2, output1);
                        }
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
                        stopwatch.Reset();
                        stopwatch.Start();
                        Console.WriteLine("ApproximateAlgorithm1- computing edges...");
                        output2 = gs.ApproximateAlgorithm1Edges();
                        if (output2 != null)
                        {
                            GraphReader.WriteCSV(path_output2, 2, output2);
                        }

                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
                        break;
                    case '3':
                        Console.WriteLine("ApproximateAlgorithm2 - computing vertices...");
                        stopwatch = Stopwatch.StartNew();
                        output1 = gs.ApproximateAlgorithm2Vertices();
                        if (output1 != null)
                        {
                            GraphReader.WriteCSV(path_output1, 3, output1);
                        }
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
                        stopwatch.Reset();
                        stopwatch.Start();
                        Console.WriteLine("ApproximateAlgorithm2 - computing edges...");
                        output2 = gs.ApproximateAlgorithm2Edges();
                        if (output2 != null)
                        {
                            GraphReader.WriteCSV(path_output2, 3, output2);
                        }
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
                        break;
                    default:
                        break;
                }

                Console.WriteLine("Press [ESC] to exit...");
                Console.WriteLine("Press any other button to contiune calculations on given graphs...");
                var keyPressed = Console.ReadKey();

                if (keyPressed.Key == ConsoleKey.Escape) flag = false;
            }
        }

	}

}

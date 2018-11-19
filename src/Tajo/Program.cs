using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ASD.Graphs;

namespace Tajo
{
	public class Program
	{
		static void Main(string[] args)
		{

            GraphExport ge = new GraphExport();

            Console.WriteLine("TAJO maximal common subgraph project.");
			Console.WriteLine("Press 1 to run exact algorithm.");
			Console.WriteLine("Press 2 to run approximate algorithm no.1");
			Console.WriteLine("Press 3 to run approximate algorithm no.2");

			var x = Console.Read();
            Console.WriteLine("Reading graphs from .csv...");

            // change *.csv name in path to read diffrent graphs
            var path_input1 = "..\\data\\5_5_A_mistal.csv";
			var path_input2 = "..\\data\\5_5_B_mistal.csv";
            var graph1 = GraphReader.ReadCSV(path_input1);
            var graph2 = GraphReader.ReadCSV(path_input2);

            CommonGraphSolver gs = new CommonGraphSolver(graph1, graph2);


            // visualize graphs
            //ge.Export(gs.Graph1);
            //ge.Export(gs.Graph2);
            //ge.Export(gs.LineGraph1);
            //ge.Export(gs.LineGraph2);
            //ge.Export(gs.ModularProductGraphVertices);
            //ge.Export(gs.ModularProductGraphEdges);
            var path_output1 = path_input1.Remove(path_input1.Length - 12) + "result1";
            var path_output2 = path_input2.Remove(path_input2.Length - 12) + "result2";

            switch (x)
			{
				case '1':
					Console.WriteLine("Exact algorithm - computing vertices...");
                    // TO DO
                    var output1 = gs.ExactAlghoritmVertices();
                    if (output1 != null)
                    {
                        GraphReader.WriteCSV(path_output1, 1, output1);
                    }
                    Console.WriteLine("Exact algorithm - computing edges...");
                    var output2 =  gs.ExactAlghoritmEdges();
                    if (output2 != null)
                    {
                        GraphReader.WriteCSV(path_output2, 1, output2);
                    }

					break;
				case '2':
                    Console.WriteLine("Exact algorithm - computing vertices...");
                    output1 = gs.ApproximateAlgorithm1Vertices();
                    if (output1 != null)
                    {
                        GraphReader.WriteCSV(path_output1, 2, output1);
                    }
                    Console.WriteLine("Exact algorithm - computing edges...");
                    output2 = gs.ApproximateAlgorithm1Edges();
                    if (output2 != null)
                    {
                        GraphReader.WriteCSV(path_output2, 2, output2);
                    }
                    break;
				case '3':
                    Console.WriteLine("Exact algorithm - computing vertices...");
                    output1 = gs.ApproximateAlgorithm2Vertices();
                    if (output1 != null)
                    {
                        GraphReader.WriteCSV(path_output1, 3, output1);
                    }
                    Console.WriteLine("Exact algorithm - computing edges...");
                    output2 = gs.ApproximateAlgorithm2Edges();
                    if (output2 != null)
                    {
                        GraphReader.WriteCSV(path_output2, 3, output2);
                    }
                    break;
				default:
					break;
			}
			 
		}

	}

}

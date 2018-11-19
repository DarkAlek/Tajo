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


            var path_input1 = "..\\data\\7_8_A_mistal.csv";
			var path_input2 = "..\\data\\7_8_B_mistal.csv";
            var graph1 = GraphReader.ReadCSV(path_input1);
            var graph2 = GraphReader.ReadCSV(path_input2);

            CommonGraphSolver gs = new CommonGraphSolver(graph1, graph2);

            //ge.Export(gs.Graph1);
            //ge.Export(gs.Graph2);
            //ge.Export(gs.LineGraph1);
            //ge.Export(gs.LineGraph2);
            //ge.Export(gs.ModularProductGraphVertices);
            //ge.Export(gs.ModularProductGraphEdges);
            var path_output1 = path_input1.Remove(path_input1.Length - 4) + "_result1";
            var path_output2 = path_input2.Remove(path_input2.Length - 4) + "_result2";

            switch (x)
			{
				case '1':
					Console.WriteLine("Exact algorithm");
                    // TO DO
                    var output1 = gs.ExactAlghoritmVertices();
                    var output2 =  gs.ExactAlghoritmEdges();
					GraphReader.WriteCSV(path_output1, 1, output1);
					GraphReader.WriteCSV(path_output2, 1, output2);

					break;
				case '2':
					Console.WriteLine("Approximate no.1");
                    output1 = gs.ApproximateAlgorithm1Vertices();
                    output2 = gs.ApproximateAlgorithm1Edges();
                    GraphReader.WriteCSV(path_output1, 2, output1);
                    GraphReader.WriteCSV(path_output2, 2, output2);
                    break;
				case '3':
					Console.WriteLine("Approximate no.2");
                    output1 = gs.ApproximateAlgorithm2Vertices();
                    output2 = gs.ApproximateAlgorithm2Edges();
                    GraphReader.WriteCSV(path_output1, 3, output1);
                    GraphReader.WriteCSV(path_output2, 3, output2);
                    break;
				default:
					break;
			}
			 
		}

	}

}

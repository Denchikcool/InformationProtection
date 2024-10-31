using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RGR
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Edge> edges = new List<Edge>();
            int numVertices = 0;

            try
            {
                string filePath = "graph.txt";
                string[] lines = File.ReadAllLines(filePath);

                string[] firstLine = lines[0].Split(' ');
                numVertices = int.Parse(firstLine[0]);
                int numEdges = int.Parse(firstLine[1]);

                for (int i = 1; i <= numEdges; i++)
                {
                    string[] edgeData = lines[i].Split(' ');
                    int source = int.Parse(edgeData[0]);
                    int destination = int.Parse(edgeData[1]);
                    edges.Add(new Edge(source, destination));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return;
            }

            try
            {
                Dictionary<int, string> vertexColors = Graph.ColorGraph(edges, numVertices);

                Console.WriteLine("Раскраска графа:");
                for (int i = 0; i < numVertices; i++)
                {
                    Console.WriteLine($"Вершина {i + 1}: {vertexColors[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
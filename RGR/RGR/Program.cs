using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace RGR
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Edge> edges = new List<Edge>();
            int numVertices = 0;
            List<Dictionary<string, BigInteger>> informationVertexes = new List<Dictionary<string, BigInteger>>();
            List<BigInteger> zVertex = new List<BigInteger>();

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

                List<ulong> vertexesWithR = new List<ulong>(numVertices);
                Random random = new Random();
                for (int i = 0; i < numVertices; i++)
                {
                    ulong r = MainOperations.GenerateExponent(1000000, 1000000000, random);
                    vertexesWithR.Add(r);
                }

                Dictionary<string, int> colorToBinary = new Dictionary<string, int>();
                for (int i = 0; i < Graph.colors.Length; i++)
                {
                    string binaryCode = Convert.ToString(i, 2).PadLeft(2, '0');
                    colorToBinary.Add(Graph.colors[i], int.Parse(binaryCode));
                }

                for(int i = 0; i < numVertices; i++)
                {
                    int binaryColor = colorToBinary[vertexColors[i]];
                    string tempNumber = vertexesWithR[i].ToString();
                    tempNumber = tempNumber.Substring(0, tempNumber.Length - 2) + binaryColor.ToString().PadLeft(2, '0');

                    vertexesWithR[i] = ulong.Parse(tempNumber);
                }

                for(int i = 0; i < numVertices; i++)
                {
                    RSAMethods rsa = new RSAMethods();

                    Dictionary<string, BigInteger> vertexInfo = new Dictionary<string, BigInteger>()
                    {
                        {"N", rsa.N },
                        {"D", rsa.D },
                        {"C", rsa.C }
                    };

                    informationVertexes.Add(vertexInfo);
                    Console.WriteLine($"R = {vertexesWithR[i]}, D = {informationVertexes[i]["D"]}, N = {informationVertexes[i]["N"]}");
                    zVertex.Add(MainOperations.FastPow1(vertexesWithR[i], informationVertexes[i]["D"], informationVertexes[i]["N"]));
                }

                Console.WriteLine("\nВершины с R и их двоичные коды:");
                for (int i = 0; i < numVertices; i++)
                {
                    Console.WriteLine($"Вершина {i + 1}: R = {vertexesWithR[i]}, Цвет = {vertexColors[i]}, Двоичный код = {colorToBinary[vertexColors[i]]}");
                    Console.WriteLine($"N = {informationVertexes[i]["N"]}, C = {informationVertexes[i]["C"]}, D = {informationVertexes[i]["D"]}");
                    Console.WriteLine($"Z = {zVertex[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
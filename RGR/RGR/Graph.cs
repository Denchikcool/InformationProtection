using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGR
{
    //класс ребра
    public class Edge
    {
        public int Source { get; set; }
        public int Destination { get; set; }

        public Edge(int source, int destination)
        {
            Source = source;
            Destination = destination;
        }
    }

    public class Graph
    {
        public static string[] colors = { "YELLOW", "RED", "BLUE", "BLACK" };
        public static Dictionary<int, string> ColorGraph(List<Edge> edges, int numVertices)
        {
            // Перемешиваем цвета случайным образом
            Random rnd = new Random();
            colors = colors.OrderBy(x => rnd.Next()).ToArray();

            // Словарь для хранения цвета каждой вершины
            Dictionary<int, string> vertexColors = new Dictionary<int, string>();

            // Инициализация всех вершин как не раскрашенных
            for (int i = 0; i < numVertices; i++)
            {
                vertexColors[i] = null;
            }

            // Сортируем вершины по степени
            List<int> sortedVertices = GetSortedVerticesByDegree(edges, numVertices);

            Console.WriteLine("Отсортированный список вершин:");
            foreach (var vertex in sortedVertices)
            {
                Console.Write($"{vertex + 1} ");
            }
            Console.WriteLine();

            // Раскрашиваем вершины по порядку, начиная с вершины с максимальной степенью
            foreach (int vertex in sortedVertices)
            {
                // Список цветов, которые уже использованы смежными вершинами
                List<string> usedColors = new List<string>();

                // Проверяем цвета соседей
                foreach (var edge in edges.Where(e => e.Source == vertex || e.Destination == vertex))
                {
                    int neighbor = (edge.Source == vertex) ? edge.Destination : edge.Source;
                    if (vertexColors.ContainsKey(neighbor) && vertexColors[neighbor] != null)
                    {
                        usedColors.Add(vertexColors[neighbor]);
                    }
                }

                // Проверяем, есть ли доступный цвет
                if (!colors.Except(usedColors).Any())
                {
                    // Если нет доступного цвета, то не удается раскрасить граф
                    throw new Exception("Невозможно раскрасить граф с использованием данного количества цветов.");
                }

                // Выбираем первый доступный цвет
                foreach (var color in colors)
                {
                    if (!usedColors.Contains(color))
                    {
                        vertexColors[vertex] = color;
                        break;
                    }
                }
            }

            return vertexColors;
        }

        // Метод для получения отсортированного списка вершин по степени
        private static List<int> GetSortedVerticesByDegree(List<Edge> edges, int numVertices)
        {
            // Создаем словарь для хранения степеней вершин
            Dictionary<int, int> degrees = new Dictionary<int, int>();
            for (int i = 0; i < numVertices; i++)
            {
                degrees[i] = 0;
            }

            // Подсчитываем степени вершин
            foreach (var edge in edges)
            {
                degrees[edge.Source]++;
                degrees[edge.Destination]++;
            }

            // Сортируем вершины по степени
            return degrees.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }
    }
}

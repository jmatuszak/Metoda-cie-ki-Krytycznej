using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


public class MainClass
{


    public class Dpv
    {
        public int distance;
        public int previous;
        public bool visited;
    };
    public class Node
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class Edge
    {
        public string V { get; set; }
        public string U { get; set; }
    }

    public class GraphData
    {
        public Dictionary<string, int> Nodes { get; set; }
        public List<string[]> Edges { get; set; }
    }

    public static void Main()
    {

        string text = File.ReadAllText(@"C:\Users\Jan\source\repos\Optymalizacja Kombinatoryczna\Szeregowanie zadań\Metoda Ścieżki Krytycznej\Metoda Ścieżki Krytycznej\input1.json");
        var graph = JsonConvert.DeserializeObject<GraphData>(text);

        var n = graph.Nodes.Count;
        var matrix = new int[n,n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = 0;
            }
        }

        for (int i = 0; i < n; i++)
        {
            string name = graph.Nodes.ElementAt(i).Key;
            int value = graph.Nodes.ElementAt(i).Value;
            var neighbours = graph.Edges.Where(x => String.Equals(x[0],name)).ToList();
            //Console.WriteLine(graph.Edges[0][0]);
            foreach (var neighbour in neighbours)
            {
                int j = graph.Nodes.Keys.ToList().IndexOf(neighbour[1]);
                matrix[i, j] = value;
            }
        }
        PrintMatrix(matrix);
        void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {

                    Console.Write(matrix[i, j]);
                    if (j < matrix.GetLength(1) - 1) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        /*        for (int i = 0; i < n; i++)
                {
                    string name = graph.Nodes.ElementAt(i).Name;
                    int value = graph.Nodes.ElementAt(i).Value;
                    var neighbours = graph.Edges.Select(x => x.U.Equals(name)).ToList();
                    foreach (var neighbour in neighbours)
                    {
                        Console.WriteLine(neighbour);
                    }
                    //matrix[i]
                }*/

        Dpv[] Dijkstra(int[,] matrix, int v)
        {
            var n = matrix.GetLength(0);
            Dpv[] dpvArray = new Dpv[n];
            for (int i = 0; i < n; i++)
            {
                dpvArray[i].distance = (i == v) ? 0 : int.MaxValue;
                dpvArray[i].previous = -1;
                dpvArray[i].visited = false;
            }
            int u = v;
            while (u != -1)
            {
                dpvArray[u].visited = true;
                for (int i = 0; i < n; i++)
                {
                    if (matrix[u, i] > 0 && dpvArray[u].distance + matrix[u, i] < dpvArray[i].distance)
                    {
                        dpvArray[i].distance = dpvArray[u].distance + matrix[u, i];
                        dpvArray[i].previous = u;
                    }
                }
                u = FindMin(ref dpvArray);
            }
            return dpvArray;
        }


        int FindMin(ref Dpv[] dpvArray)
        {
            int min = -1;
            int minDistance = int.MaxValue;
            var n = matrix.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (!dpvArray[i].visited && dpvArray[i].distance < minDistance)
                {
                    min = i;
                    minDistance = dpvArray[i].distance;
                }
            }
            return min;
        }




        /*
                static int[,] ReadMatrix()
                {
                    var line = Console.ReadLine();

                    var trimmed = line.Split(' ');
                    int n = trimmed.Length;
                    int[,] matrix = new int[n, n];
                    for (int j = 0; j < n; j++)
                    {
                        matrix[0, j] = int.Parse(trimmed[j]);
                    }
                    for (int i = 1; i < n; i++)
                    {
                        line = Console.ReadLine();
                        trimmed = line.Split(' ');
                        for (int j = 0; j < n; j++)
                        {
                            matrix[i, j] = int.Parse(trimmed[j]);
                        }

                    }
                    return matrix;
                }

                int[,] GenerateMatrix(int n)
                {
                    var numbers = new int[] { 0, 0, 0, 0, 1, 2, 3, 1, 1, 0, 0, 0, 0 };
                    var random = new Random();
                    int[,] matrix = new int[n, n];
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i == j) matrix[i, j] = 0;
                            else
                            {
                                matrix[i, j] = numbers[random.Next(0, numbers.Length)];
                            }
                        }
                    }
                    //PrintMatrix(matrix);
                    return matrix;
                }*/
    }
}
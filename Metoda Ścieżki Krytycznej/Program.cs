using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


public class MainClass
{


	public struct Dpv
	{
		public int distance;
		public int previous;
		public bool visited;
	};
	public struct DpvPlus
	{
		public int distance;
		public int previous;
		public int vertex;
		public string label;
		public int end;
	};
	public class Node
    {
        public string Label { get; set; }
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

		//string text = Console.ReadLine();
		string text = File.ReadAllText(@"C:\Users\Jan\source\repos\Optymalizacja Kombinatoryczna\Szeregowanie zadań\Metoda Ścieżki Krytycznej\Metoda Ścieżki Krytycznej\input1.json");
		var graph = JsonConvert.DeserializeObject<GraphData>(text);
		var graph1 = System.Text.Json.JsonSerializer.Deserialize<GraphData>(text);



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
            string label = graph.Nodes.ElementAt(i).Key;
            int value = graph.Nodes.ElementAt(i).Value;
            var neighbours = graph.Edges.Where(x => String.Equals(x[0], label)).ToList();
            foreach (var neighbour in neighbours)
            {
                int j = graph.Nodes.Keys.ToList().IndexOf(neighbour[1]);
                matrix[i, j] = value;
            }
        }
		int[,] copiedMatrix = (int[,])matrix.Clone();
		
		var count = TopologicalSort(copiedMatrix).Count;
		if (count!=n-1) Console.WriteLine("Błąd: Graf zawiera cykl, nie można wyznaczyć ścieżki krytycznej.");
		else
		{
			var dpvArray = Dijkstra(matrix, 0);
			var dpvList = dpvArray.ToList();
			int v = 0;
			int u = 0;

			var dpvPlusArray = new DpvPlus[n];
			for (int i = 0; i < n; i++)
			{
				dpvPlusArray[i].distance = dpvArray[i].distance;
				dpvPlusArray[i].previous = dpvArray[i].previous;
				dpvPlusArray[i].vertex = i;
				dpvPlusArray[i].label = graph.Nodes.ElementAt(i).Key;
				dpvPlusArray[i].end = dpvPlusArray[i].distance + graph.Nodes.ElementAt(i).Value;
			}
			var dpvPlusList = dpvPlusArray.ToList();
			dpvPlusList.OrderBy(x => x.distance);



			//PRINT OUTPUT
			var tempDistance = int.MinValue;
			Console.Write("Ścieżka krytyczna: ");
			foreach (var item in dpvPlusList)
			{
				if (item.distance == tempDistance) continue;
				tempDistance = item.distance;
				var tempList = dpvPlusList.Where(a => a.distance == item.distance).OrderByDescending(a => a.distance);
				Console.Write(tempList.ElementAt(0).label);
				if (!item.Equals(dpvPlusList.Last())) Console.Write(" -> ");
			}
			Console.WriteLine();
			Console.Write("Uszeregowanie zadań: ");
			foreach (var item in dpvPlusList)
			{
				Console.Write(item.label + " (" + item.distance + "-" + item.end + ")");
				if (!item.Equals(dpvPlusList.Last())) Console.Write(", ");
			}
			Console.WriteLine();
			Console.WriteLine("Łączny czas wykonania: " + dpvPlusList.Last().end + " dni");
		}
        

 
	}
	static List<int> TopologicalSort(int[,] matrix)
	{
		var n = matrix.GetLength(0);
		var L = new List<int>();
		var S = FindVertexesNoInEdge(matrix);
		while (S.Any())
		{
			for(int i=0; i < n; i++)
			{
				matrix[S[0], i] = 0;
			}
			L.Add(S[0]);
			S = FindVertexesNoInEdge(matrix);

		}
		return L;
	}
	static List<int> FindVertexesNoInEdge(int[,] matrix)
	{
		var n = matrix.GetLength(0);
		var S = new List<int>();
		for (int i = 0; i < n; i++)
		{
			int count = 0;
			bool stillExists = false;
			for (int j = 0; j < n; j++)
			{
				if (matrix[i, j] != 0) stillExists = true;
				if (matrix[j, i] != 0) count++;
			}
			if (stillExists == true && count == 0) S.Add(i);
		}
		return S;
	}

	static Dpv[] Dijkstra(int[,] matrix, int v)
	{
		v = 0;
		var n = matrix.GetLength(0);
		Dpv[] dpvArray = new Dpv[n];
		for (int i = 0; i < n; i++)
		{
			dpvArray[i].distance = (i == v) ? 0 : int.MinValue;
			dpvArray[i].previous = -1;
			dpvArray[i].visited = false;
		}
		int u = v;
		while (u != -1)
		{
			dpvArray[u].visited = true;
			for (int i = 0; i < n; i++)
			{
				if (matrix[u, i] > 0 && dpvArray[u].distance + matrix[u, i] > dpvArray[i].distance)
				{
					dpvArray[i].distance = dpvArray[u].distance + matrix[u, i];
					dpvArray[i].previous = u;
				}
			}
			u = FindMax(ref dpvArray, matrix);
		}
		return dpvArray;
	}

	static int FindMax(ref Dpv[] dpvArray, int[,]matrix)
	{
		int maxV = -1;
		int maxDistance = int.MinValue;
		var n = matrix.GetLength(0);
		for (int i = 0; i < n; i++)
		{
			if (!dpvArray[i].visited && dpvArray[i].distance > maxDistance)
			{
				maxV = i;
				maxDistance = dpvArray[i].distance;
			}
		}
		return maxV;
	}
}
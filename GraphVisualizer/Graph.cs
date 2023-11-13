using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace graph_test
{
    class Graph
    {
        Dictionary<string, List<Edge>> _arr;
        public struct Edge
        {
            public string From { get; }
            public string To { get; }
            public double Distance { get; set; }
            public Edge(string from, string to, double distance)
            {
                From = from;
                To = to;
                Distance = distance;
            }
        }

        public Graph()
        {
            _arr = new Dictionary<string, List<Edge>>();
        }

        public bool HasVertex(in string vertex)
        {
            return _arr.ContainsKey(vertex);
        }

        public void AddVertex(in string vertex)
        {
            try
            {
                _arr.Add(vertex, new List<Edge>());
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Вершина с таким значением уже есть");
            }
        }

        public bool RemoveVertex(in string vertex)
        {
            if (!_arr.Remove(vertex))
                return false;
            foreach (var i in _arr.Values)
            {
                foreach (var edge in i)
                {
                    if (edge.To == vertex)
                        i.Remove(edge);
                }
            }
            return true;
        }

        public bool AddEdge(in string from, in string to, in double distance)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            _arr[from].Add(new Edge(from, to, distance));
            return true;
        }

        public bool AddEdge(in Edge edge)
        {
            if (!_arr.ContainsKey(edge.From) || !_arr.ContainsKey(edge.To))
                return false;
            _arr[edge.From].Add(edge);
            return true;
        }

        public bool RemoveEdge(in string from, in string to)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            string tmp_from = from;
            string tmp_to = to;
            return _arr[from].Remove(_arr[from].Find(x => x.From == tmp_from && x.To == tmp_to));
        }

        public bool RemoveEdge(in Edge edge)
        {
            if (HasEdge(edge))
                return _arr[edge.From].Remove(edge);
            return false;
        }

        public bool HasEdge(in string from, in string to)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            return true;
        }

        public bool HasEdge(in Edge edge)
        {
            if (!_arr.ContainsKey(edge.From) || !_arr.ContainsKey(edge.To) || !_arr[edge.From].Contains(edge))
                return false;
            return true;
        }

        public int Order()
        {
            return _arr.Count();
        }

        public int Degree()
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Граф пуст");
            int max = 0;
            foreach (var list in _arr.Values)
            {
                int tmp = list.Count();
                if (tmp > max)
                    max = tmp;
            }
            return max;
        }

        public void PrintGraph()
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("НЕвозможно вывети граф, потому что он пуст");
            foreach (var list in _arr.Values)
            {
                foreach (var edge in list)
                {
                    Console.WriteLine($"{edge.From} -> {edge.To}, Расстояние: {edge.Distance}");
                }
            }
        }

        public void BreadthFirstSearch(in string start_vertex)
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно обойти граф, потому что он пуст");
            Queue<string> q = new Queue<string>();
            HashSet<string> visited = new HashSet<string>();
            q.Enqueue(start_vertex);
            visited.Add(start_vertex);
            while (q.Count > 0)
            {
                string v = q.Dequeue();
                Console.WriteLine(v);
                foreach (var edge in _arr[v])
                {
                    string neighbor = edge.To;
                    if (!visited.Contains(neighbor))
                    {
                        q.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        public void DepthFirstSearch(in string start_vertex)
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно обойти граф, потому что он пуст");
            Stack<string> q = new Stack<string>();
            HashSet<string> visited = new HashSet<string>();
            q.Push(start_vertex);
            visited.Add(start_vertex);
            while (q.Count > 0)
            {
                string tmp = q.Pop();
                Console.WriteLine(tmp);
                foreach (var edge in _arr[tmp])
                {
                    string neighbor = edge.To;
                    if (!visited.Contains(neighbor))
                    {
                        q.Push(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        public List<Edge> FindShortestPath(in string from, in string to)
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно найти кратчайший путь, потому что граф пуст");
            if (!_arr.ContainsKey(from))
                throw new InvalidOperationException("Начальной вершины с таким значением нет в графе");
            if (!_arr.ContainsKey(to))
                throw new InvalidOperationException("Конечной вершины с таким значением нет в графе");
            PriorityQueue<string, double> pq = new PriorityQueue<string, double>();
            Dictionary<string, double> distance = new Dictionary<string, double>();
            Dictionary<string, string> prev = new Dictionary<string, string>();
            foreach (var vertex in _arr.Keys)
            {
                distance[vertex] = double.PositiveInfinity;
            }
            distance[from] = 0;
            pq.Enqueue(from, 0);
            string cur;
            while (pq.Count > 0)
            {
                cur = pq.Dequeue();
                if (cur == to)
                    break;
                foreach (var edge in _arr[cur])
                {
                    string neighbor = edge.To;
                    double new_distance = distance[cur] + edge.Distance;
                    if (new_distance < distance[neighbor])
                    {
                        distance[neighbor] = new_distance;
                        prev[neighbor] = cur;
                        pq.Enqueue(neighbor, new_distance);
                    }
                }
            }
            List<Edge> path = new List<Edge>();
            cur = to;
            while (prev.ContainsKey(cur))
            {
                string prev_vertex = prev[cur];
                double weight = 0;
                foreach (var edge in _arr[prev_vertex])
                {
                    if (edge.To == cur)
                    {
                        weight = edge.Distance;
                        break;
                    }
                }
                path.Add(new Edge(prev_vertex, cur, weight));
                cur = prev_vertex;
            }
            path.Reverse();
            if (path.Count == 0)
                throw new ArgumentException("Между вершинами нет пути");
            return path;
        }
    }
}

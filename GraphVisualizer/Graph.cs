using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GraphVisualizer
{
    class Graph: INotifyPropertyChanged
    {
        Dictionary<string, List<Edge>> _arr;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            _arr.Add(vertex, new List<Edge>());
            OnPropertyChanged(nameof(Order));
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
                        RemoveEdge(edge);
                }
            }
            OnPropertyChanged(nameof(Order));
            return true;
        }

        public bool EditVertex(in string name, in string newName)
        {
            if (_arr.ContainsKey(newName) || !_arr.ContainsKey(name))
                return false;
            foreach (var i in _arr.Values)
            {
                foreach (var edge in i)
                {
                    if (edge.To == name)
                        edge.To = newName;
                }
            }
            AddVertex(newName);
            foreach (var edge in _arr[name])
            {
                AddEdge(newName, edge.To, edge.Distance);
            }
            RemoveVertex(name);
            return true;
        }

        public bool AddEdge(in string from, in string to, in double distance)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            _arr[from].Add(new Edge(from, to, distance));
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(Degree));
            return true;
        }

        public bool AddEdge(in Edge edge)
        {
            if (!_arr.ContainsKey(edge.From) || !_arr.ContainsKey(edge.To))
                return false;
            _arr[edge.From].Add(edge);
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(Degree));
            return true;
        }

        public bool RemoveEdge(in string from, in string to)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            string tmp_from = from;
            string tmp_to = to;
            bool tmp = _arr[from].Remove(_arr[from].Find(x => x.From == tmp_from && x.To == tmp_to));
            if (tmp)
            {
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(Degree));
            }
            return tmp;
        }

        public bool RemoveEdge(in Edge edge)
        {
            bool flag = false;
            if (HasEdge(edge))
            {
                flag = _arr[edge.From].Remove(edge);
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(Degree));
            }
            return flag;
        }

        public bool HasEdge(in string from, in string to)
        {
            if (!_arr.ContainsKey(from) || !_arr.ContainsKey(to))
                return false;
            foreach (Edge edge in _arr[from])
            {
                if (edge.To == to)
                    return true;
            }
            return false;
        }

        public bool HasEdge(in Edge edge)
        {
            if (!_arr.ContainsKey(edge.From) || !_arr.ContainsKey(edge.To) || !_arr[edge.From].Contains(edge))
                return false;
            return true;
        }

        public int Order
        {
            get
            {
                return _arr.Count();
            }
        }

        public int Degree
        {
            get
            {
                int max = 0;
                foreach (var list in _arr.Values)
                {
                    int tmp = list.Count();
                    if (tmp > max)
                        max = tmp;
                }
                return max;
            }
            
        }

        public int Size
        {
            get
            {
                int size = 0;
                foreach (var list in _arr.Values)
                {
                    size += list.Count();
                }
                return size;
            }
        }

        public void PrintGraph()
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно вывети граф, потому что он пуст");
            foreach (var list in _arr.Values)
            {
                foreach (var edge in list)
                {
                    Console.WriteLine($"{edge.From} -> {edge.To}, Расстояние: {edge.Distance}");
                }
            }
        }

        public List<string> BreadthFirstSearch(in string start_vertex)
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно обойти граф, потому что он пуст");
            Queue<string> q = new Queue<string>();
            HashSet<string> visited = new HashSet<string>();
            List<string> result = new();
            q.Enqueue(start_vertex);
            visited.Add(start_vertex);
            while (q.Count > 0)
            {
                string v = q.Dequeue();
                result.Add(v);
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
            return result;
        }

        public List<string> DepthFirstSearch(in string start_vertex)
        {
            if (_arr.Count == 0)
                throw new InvalidOperationException("Невозможно обойти граф, потому что он пуст");
            Stack<string> q = new Stack<string>();
            HashSet<string> visited = new HashSet<string>();
            List<string> result = new();
            q.Push(start_vertex);
            visited.Add(start_vertex);
            while (q.Count > 0)
            {
                string tmp = q.Pop();
                result.Add(tmp);
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
            return result;
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
                foreach (var edge in _arr[prev_vertex])
                {
                    if (edge.To == cur)
                    {
                        path.Add(edge);
                        break;
                    }
                }
                cur = prev_vertex;
            }
            path.Reverse();
            if (path.Count == 0)
                throw new ArgumentException("Между вершинами нет пути");
            return path;
        }
    }
}

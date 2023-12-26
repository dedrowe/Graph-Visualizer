using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.Json;
using Microsoft.Win32;
using System.IO;

namespace GraphVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _file;
        Graph _graph = new();
        Vertex? _selectedVertex;
        int _count = 0;
        static public byte Mode { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _graph;
        }

        private void CanvasLMBClick(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case 1:
                    try
                    {
                        Point position = e.GetPosition(GraphCanvas);
                        if (e.Source is Canvas)
                        {
                            double radius = 25;
                            AddVertex(position.X - radius, position.Y - radius, $"{++_count}");
                            ShowPath();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case 2:
                    if (e.Source is Vertex)
                    {
                        AddEdge((Vertex)e.Source);
                        ShowPath();
                    }
                    break;
                case 4:
                    DeleteElement(e.Source);
                    ShowPath();
                    break;
                case 5:
                    if (e.Source is Vertex)
                    {
                        Vertex vertex = (Vertex)e.Source;
                        List<string> vertices = _graph.BreadthFirstSearch(vertex.VertexName);
                        SetBypassOrder(vertices);
                    }
                    break;
                case 6:
                    if (e.Source is Vertex)
                    {
                        Vertex vertex = (Vertex)e.Source;
                        List<string> vertices = _graph.DepthFirstSearch(vertex.VertexName);
                        SetBypassOrder(vertices);
                    }
                    break;
            }
        }

        private void AddVertex(double x, double y, string name)
        {
            _graph.AddVertex(name);
            Vertex b = new Vertex();
            double radius = b.vertexRadius;
            double vertexPositionX = x;
            double vertexPositionY = y;
            if (x < 25)
                vertexPositionX = 1;
            else if (x > GraphCanvas.ActualWidth - radius * 2)
                vertexPositionX = GraphCanvas.ActualWidth - 2 * radius - 1;
            if (y < 25)
                vertexPositionY = 1;
            else if (y > GraphCanvas.ActualHeight - radius * 2)
                vertexPositionY = GraphCanvas.ActualHeight - 2 * radius - 1;
            b.PositionY = vertexPositionY;
            b.PositionX = vertexPositionX;
            b.SetValue(Canvas.LeftProperty, vertexPositionX);
            b.SetValue(Canvas.TopProperty, vertexPositionY);
            b.VertexName = name;
            GraphCanvas.Children.Add(b);
        }

        private void AddEdge(Vertex vertex, double distance = 1)
        {
            if (vertex.IsSelected)
            {
                vertex.DeSelect();
                _selectedVertex = null;
            }
            else
            {
                
                if (_selectedVertex is null)
                {
                    vertex.Select();
                    _selectedVertex = vertex;
                    return;
                }
                _selectedVertex.DeSelect();
                if (_graph.HasEdge(_selectedVertex.VertexName, vertex.VertexName))
                {
                    MessageBox.Show("Такое ребро уже существует");
                    return;
                }
                double x1 = _selectedVertex.PositionX + vertex.vertexRadius;
                double y1 = _selectedVertex.PositionY + vertex.vertexRadius;
                double x2 = vertex.PositionX + vertex.vertexRadius;
                double y2 = vertex.PositionY + vertex.vertexRadius;

                Edge edge = new(_selectedVertex.VertexName, vertex.VertexName, distance);
                edge.CalculateArrowParameters(x1, y1, x2, y2);
                int minZIndex = Panel.GetZIndex(_selectedVertex) < Panel.GetZIndex(this) ? Panel.GetZIndex(_selectedVertex) : Panel.GetZIndex(this);
                Panel.SetZIndex(edge, minZIndex - 1);
                GraphCanvas.Children.Add(edge);
                _graph.AddEdge(edge);
                _selectedVertex._edgesOut.Add(edge);
                vertex._edgesIn.Add(edge);
                _selectedVertex = null;
            }
        }

        private void DeleteElement(object e)
        {
            if (e is Vertex)
            {
                Vertex vertex = (Vertex)e;
                foreach (Edge edge in vertex._edgesIn)
                {
                    _graph.RemoveEdge(edge);
                }
                foreach (Edge edge in vertex._edgesOut)
                {
                    _graph.RemoveEdge(edge);
                }
                vertex.Remove();
                _graph.RemoveVertex(vertex.VertexNameBlock.Text);
                vertex.GetValue(Canvas.LeftProperty);
            }
            else if (e is Edge)
            {
                Edge edge = (Edge)e;
                _graph.RemoveEdge(edge);
                GraphCanvas.Children.Remove(edge);
            }
        }

        private void ClearGraph()
        {
            List<Vertex> vertices = new();
            List<Edge> edges = new();
            foreach (var i in GraphCanvas.Children)
            {
                if (i is Edge)
                    edges.Add((Edge)i);
                else if (i is Vertex) 
                    vertices.Add((Vertex)i);
            }
            foreach(Edge i in edges)
                DeleteElement(i);
            foreach(Vertex i in vertices)
                DeleteElement(i);
        }

        private void CanvasRMBClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Vertex)
            {
                
                Vertex vertex = (Vertex)e.Source;
                PropertiesWindow propertiesWindow = new();
                TextBox textBox = new TextBox() { FontSize = 14, Text = vertex.VertexName, Width=100 };
                Binding binding = new Binding();
                binding.Source = vertex;
                binding.Path = new PropertyPath("VertexName");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

                StackPanel tmp = new() { Orientation = Orientation.Horizontal };
                tmp.Children.Add(new TextBlock() { Text = "Название вершины ", FontSize = 14 });
                tmp.Children.Add(textBox);

                string tempName = vertex.VertexName;
                propertiesWindow.Properties.Children.Add(tmp);
                propertiesWindow.ShowDialog();
                if (tempName != vertex.VertexName && !_graph.EditVertex(tempName, vertex.VertexName))
                {
                    vertex.VertexName = tempName;
                    MessageBox.Show("Вершина с таким именем уже есть");
                }
                ShowPath();
            }
            else if (e.Source is Edge)
            {
                Edge edge = (Edge)e.Source;
                PropertiesWindow propertiesWindow = new();

                TextBox textBox = new TextBox() { FontSize = 14, Text = $"{edge.Distance}", Width = 100 };
                StackPanel tmp = new() { Orientation = Orientation.Horizontal };
                tmp.Children.Add(new TextBlock() { Text = "Длина ребра: ", FontSize = 14 });
                tmp.Children.Add(textBox);

                double newDistance;
                propertiesWindow.Properties.Children.Add(tmp);
                propertiesWindow.ShowDialog();
                if (!double.TryParse(textBox.Text, out newDistance) || newDistance <= 0)
                {
                    MessageBox.Show("Введите положительное число");
                }
                else
                {
                    edge.Distance = newDistance;
                }
                ShowPath();
            }
        }

        private void SetBypassOrder(List<string> vertices)
        {
            foreach (var tmp in GraphCanvas.Children)
            {
                if (tmp is Vertex tmpVertex)
                {
                    int index = vertices.IndexOf(tmpVertex.VertexName);
                    if (index >= 0)
                        tmpVertex.BypassOrder.Text = (index + 1).ToString();
                    else
                        tmpVertex.BypassOrder.Text = "";
                }
            }
        }

        private void ShowPath()
        {
            PathWarning.Text = "";
            PathLength.Text = "";
            foreach (var tmp in GraphCanvas.Children)
            {
                if (tmp is Edge tmpEdge)
                {
                    tmpEdge.Repaint(Brushes.Black);
                }
            }
            if (StartVertex.Text != "" && EndVertex.Text != "")
            {
                try
                {
                    List<Edge> path = _graph.FindShortestPath(StartVertex.Text, EndVertex.Text);
                    double pathLength = 0;
                    foreach (Edge edge in path)
                    {

                        edge.Repaint(Brushes.LightGreen);
                        pathLength += edge.Distance;
                    }
                    PathLength.Text = pathLength.ToString();
                }
                catch (InvalidOperationException ex)
                {
                    PathWarning.Text = ex.Message;
                }
                catch (ArgumentException ex)
                {
                    PathWarning.Text = ex.Message;
                }
            }
        }

        private string SerializeGraph()
        {
            List<Dictionary<string, string>> vertices = new();
            List<Dictionary<string, string>> edges = new();
            foreach (var tmp in GraphCanvas.Children)
            {
                if (tmp is Vertex tmpVertex)
                {
                    vertices.Add(new() {
                        { "name", tmpVertex.VertexName },
                        { "X", tmpVertex.PositionX.ToString() },
                        { "Y", tmpVertex.PositionY.ToString() }
                    });
                }
                else if (tmp is Edge tmpEdge)
                {
                    edges.Add(new()
                    {
                        { "from", tmpEdge.From },
                        { "to", tmpEdge.To },
                        { "distance", tmpEdge.Distance.ToString() }
                    });
                }
            }
            Dictionary<string, List<Dictionary<string, string>>> jsonString = new()
            {
                { "vertices", vertices },
                { "edges", edges}
            };
            return JsonSerializer.Serialize(jsonString);
        }

        private void FRAlgorithm()
        {   
            if (_graph.Order > 0)
            {
                List<Vertex> vertices = new List<Vertex>();
                List<List<double>> disp = new List<List<double>>();
                foreach (var tmp in GraphCanvas.Children)
                {
                    if (tmp is Vertex tmpVertex)
                    {
                        vertices.Add(tmpVertex);
                        disp.Add(new() { 0, 0 });
                    }
                }
                double length = 0.4 * Math.Sqrt(GraphCanvas.ActualWidth * GraphCanvas.ActualHeight / _graph.Order);
                for (double t = 10; t > 0; t += -0.1)
                {
                    double deltaX, deltaY;
                    double distance;
                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        disp[i][0] = 0;
                        disp[i][1] = 0;
                        for (int j = 0; j < vertices.Count; ++j)
                        {
                            if (i == j)
                                continue;
                            deltaX = vertices[i].PositionX - vertices[j].PositionX;
                            deltaY = vertices[i].PositionY - vertices[j].PositionY;
                            distance = deltaX * deltaX + deltaY * deltaY;
                            if (distance > 0)
                            {
                                disp[i][0] += deltaX / distance * length * length;
                                disp[i][1] += deltaY / distance * length * length;
                            }
                        }
                    }

                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        foreach (Edge e in vertices[i]._edgesOut)
                        {
                            int to = 0;
                            for (int j = 0; j < vertices.Count; ++j)
                            {
                                if (vertices[j].VertexName == e.To)
                                {
                                    to = j;
                                    break;
                                }
                            }
                            deltaX = vertices[i].PositionX - vertices[to].PositionX;
                            deltaY = vertices[i].PositionY - vertices[to].PositionY;
                            distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                            
                            disp[i][0] -= deltaX * distance / length;
                            disp[i][1] -= deltaY * distance / length;
                            disp[to][0] += deltaX * distance / length;
                            disp[to][1] += deltaY * distance / length;
                        }
                    }

                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        deltaX = disp[i][0];
                        deltaY = disp[i][1];
                        distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                        vertices[i].PositionX += disp[i][0] / distance * t;
                        vertices[i].PositionY += disp[i][1] / distance * t;
                        vertices[i].PositionX = Math.Min(GraphCanvas.ActualWidth - vertices[i].vertexRadius * 2 - 1, Math.Max(1, vertices[i].PositionX));
                        vertices[i].PositionY = Math.Min(GraphCanvas.ActualHeight - vertices[i].vertexRadius * 2 - 1, Math.Max(1, vertices[i].PositionY));
                        vertices[i].SetValue(Canvas.LeftProperty, vertices[i].PositionX);
                        vertices[i].SetValue(Canvas.TopProperty, vertices[i].PositionY);
                        vertices[i].CalculateEdgeCoordinates(vertices[i].PositionX, vertices[i].PositionY);
                    }
                }
            }
        }

        private void AddEdge_Checked(object sender, RoutedEventArgs e)
        {
            Mode = 2;
        }

        private void AddVertex_Checked(object sender, RoutedEventArgs e)
        {
            Mode = 1;
        }

        private void DragElement_Checked(object sender, RoutedEventArgs e)
        {
            Mode = 3;
        }

        private void DeleteElement_Checked(object sender, RoutedEventArgs e)
        {
            Mode = 4;
        }

        private void BreadthFirstSearch_Checked(object sender, RoutedEventArgs e)
        {
            Mode = 5;
        }

        private void DepthFirstSearch_Checked(object sender, RoutedEventArgs e)
        {
            Mode= 6;
        }

        private void PathChanged(object sender, TextChangedEventArgs e)
        {
            ShowPath();
        }

        private void ClearBypassOrder(object sender, RoutedEventArgs e)
        {
            foreach (var tmp in GraphCanvas.Children)
            {
                if (tmp is Vertex tmpVertex)
                {
                    tmpVertex.BypassOrder.Text = "";
                }
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            string graph = SerializeGraph();
            SaveFileDialog saveWindow = new SaveFileDialog();
            saveWindow.DefaultExt = ".graph";
            saveWindow.Filter = "Граф (.graph)|*.graph";
            if (saveWindow.ShowDialog() == true)
            {
                _file = saveWindow.FileName;
                using (StreamWriter sw = new StreamWriter(_file, false))
                {
                    sw.Write(graph);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string graph = SerializeGraph();
            if (_file is null)
            {
                SaveFileDialog saveWindow = new SaveFileDialog();
                saveWindow.DefaultExt = ".graph";
                saveWindow.Filter = "Граф (.graph)|*.graph";
                if (saveWindow.ShowDialog() == true)
                {
                    _file = saveWindow.FileName;
                    using (StreamWriter sw = new StreamWriter(_file, false))
                    {
                        sw.Write(graph);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(_file, false))
                {
                    sw.Write(graph);
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openWindow = new OpenFileDialog();
                openWindow.DefaultExt = ".graph";
                openWindow.Filter = "Граф (.graph)|*.graph";
                openWindow.RestoreDirectory = true;
                if (openWindow.ShowDialog() == true)
                {
                    _file = openWindow.FileName;
                    using (StreamReader sr = new StreamReader(_file, false))
                    {
                        ClearGraph();
                        var jsonGraph = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(sr.ReadToEnd());
                        if (jsonGraph is null || !jsonGraph.ContainsKey("vertices") || !jsonGraph.ContainsKey("edges"))
                        {
                            MessageBox.Show("Неверный формат файла");
                        }
                        else
                        {
                            foreach (var tmp in jsonGraph["vertices"])
                            {
                                AddVertex(double.Parse(tmp["X"]), double.Parse(tmp["Y"]), tmp["name"]);
                            }
                            foreach (var tmp in jsonGraph["edges"])
                            {
                                Vertex? from = null;
                                Vertex? to = null;
                                foreach (var i in GraphCanvas.Children)
                                {
                                    if (i is Vertex tmpVertex)
                                    {
                                        if (tmpVertex.VertexName == tmp["from"])
                                            from = tmpVertex;
                                        else if (tmpVertex.VertexName == tmp["to"])
                                            to = tmpVertex;
                                    }
                                }
                                _selectedVertex = from;
                                AddEdge(to, double.Parse(tmp["distance"]));
                            }
                            _selectedVertex = null;
                        }
                    }
                    _count = _graph.Order + 1;
                }
            }
            catch
            {
                ClearGraph();
                MessageBox.Show("Файл поврежден");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearGraph();
            ShowPath();
        }

        private void ForceAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            FRAlgorithm();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Graph _graph = new();
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
                            _graph.AddVertex($"{++_count}");
                            Vertex b = new Vertex();
                            double radius = b.vertexRadius;
                            double vertexPositionX = position.X - radius;
                            double vertexPositionY = position.Y - radius;
                            if (position.X < 25)
                                vertexPositionX = 0;
                            else if (position.X > GraphCanvas.ActualWidth - radius)
                                vertexPositionX = GraphCanvas.ActualWidth - 2 * radius;
                            if (position.Y < 25)
                                vertexPositionY = 0;
                            else if (position.Y > GraphCanvas.ActualHeight - radius)
                                vertexPositionY = GraphCanvas.ActualHeight - 2 * radius;
                            b.PositionY = vertexPositionY;
                            b.PositionX = vertexPositionX;
                            b.SetValue(Canvas.LeftProperty, vertexPositionX);
                            b.SetValue(Canvas.TopProperty, vertexPositionY);
                            b.VertexName = $"{_count}";
                            GraphCanvas.Children.Add(b);
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
                        Vertex vertex = (Vertex)e.Source;
                        if (vertex.IsSelected)
                        {
                            vertex.DeSelect();
                        }
                        else
                        {
                            Vertex? firstVertex = null;
                            foreach (var i in GraphCanvas.Children)
                            {
                                if (i is Vertex)
                                {
                                    if (((Vertex)i).IsSelected)
                                        firstVertex = (Vertex)i;
                                }
                            }
                            if (firstVertex is null)
                            {
                                vertex.Select();
                                return;
                            }
                            firstVertex.DeSelect();
                            if (_graph.HasEdge(firstVertex.VertexName, vertex.VertexName))
                            {
                                MessageBox.Show("Такое ребро уже существует");
                                break;
                            }
                            double x1 = firstVertex.PositionX + vertex.vertexRadius;
                            double y1 = firstVertex.PositionY + vertex.vertexRadius;
                            double x2 = vertex.PositionX + vertex.vertexRadius;
                            double y2 = vertex.PositionY + vertex.vertexRadius;

                            Edge edge = new(firstVertex.VertexName, vertex.VertexName, 1);
                            edge.CalculateArrowParameters(x1, y1, x2, y2);
                            int minZIndex = Panel.GetZIndex(firstVertex) < Panel.GetZIndex(this) ? Panel.GetZIndex(firstVertex) : Panel.GetZIndex(this);
                            Panel.SetZIndex(edge, minZIndex - 1);
                            GraphCanvas.Children.Add(edge);
                            _graph.AddEdge(edge);
                            firstVertex._edgesOut.Add(edge);
                            vertex._edgesIn.Add(edge);
                            ShowPath();
                        }
                    }
                    break;
                case 4:
                    if (e.Source is Vertex) 
                    {
                        Vertex vertex = (Vertex)e.Source;
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
                    else if (e.Source is Edge)
                    {
                        Edge edge = (Edge)e.Source;
                        _graph.RemoveEdge(edge);
                        GraphCanvas.Children.Remove(edge);
                    }
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
                Binding binding = new Binding();
                binding.Source = edge;
                binding.Path = new PropertyPath("Distance");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

                StackPanel tmp = new() { Orientation = Orientation.Horizontal };
                tmp.Children.Add(new TextBlock() { Text = "Длина ребра: ", FontSize = 14 });
                tmp.Children.Add(textBox);

                double tempDistance = edge.Distance;
                propertiesWindow.Properties.Children.Add(tmp);
                propertiesWindow.ShowDialog();
                if (edge.Distance <= 0)
                {
                    edge.Distance = tempDistance;
                    MessageBox.Show("Расстояние должно быть положительным");
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
                    tmpEdge.Color = Brushes.Black;
                    tmpEdge.Repaint();
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

                        edge.Color = Brushes.LightGreen;
                        edge.Repaint();
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
    }
}

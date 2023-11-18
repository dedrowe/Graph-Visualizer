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
        Graph _graph = new Graph();
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
                            double radius = b._vertexRadius;
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
                            b._positionY = vertexPositionY;
                            b._positionX = vertexPositionX;
                            b.SetValue(Canvas.LeftProperty, vertexPositionX);
                            b.SetValue(Canvas.TopProperty, vertexPositionY);
                            object tmp = b.VertexCanvas.FindName("VertexName");
                            if (tmp is TextBlock)
                            {
                                TextBlock child = (TextBlock)tmp;
                                child.Text = $"{_count}";
                            }
                            GraphCanvas.Children.Add(b);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case 4:
                    if (e.Source is Vertex) 
                    {
                        Vertex vertex = (Vertex)e.Source;
                        GraphCanvas.Children.Remove(vertex);
                        _graph.RemoveVertex(vertex.VertexName.Text);
                        vertex.GetValue(Canvas.LeftProperty);
                    }
                    else if (e.Source is Edge)
                    {
                        Edge edge = (Edge)e.Source;
                        GraphCanvas.Children.Remove(edge);
                    }
                    break;
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
    }
}

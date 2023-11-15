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
        Graph graph = new Graph();
        int count = 0;
        public MainWindow()
        {
            InitializeComponent();
            /*graph.AddVertex("1");
            graph.AddVertex("2");
            graph.AddVertex("3");
            graph.AddVertex("4");
            graph.AddVertex("5");
            graph.AddEdge("1", "2", 1);
            graph.AddEdge("1", "3", 10);
            graph.AddEdge("2", "4", 20);
            graph.AddEdge("3", "4", 11.1);
            */
            this.DataContext = graph;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            graph.AddVertex($"{++count}");
            var tmp = new TextBlock() { Text="12", Tag = "1" };
            Vertex b = new Vertex() { VertexName=tmp };
            b.VertexName = new TextBlock() { Text = "2", Tag = "1" };
            Canvas.SetTop(b, GraphCanvas.ActualHeight / 2);
            Canvas.SetLeft(b, GraphCanvas.ActualWidth / 2);
            GraphCanvas.Children.Add(b);
            
        }
        private void RemoveVertex(object sender, MouseButtonEventArgs e)
        {
        /*
            if (e.Source is Vertex)
            {
                Vertex vertex = (Vertex)e.Source;
                GraphCanvas.Children.Remove(vertex);
            }
        */
        }
    }
}

using graph_test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        public MainWindow()
        {
            Graph graph = new Graph();
            graph.AddVertex("1");
            graph.AddVertex("2");
            graph.AddVertex("3");
            graph.AddVertex("4");
            graph.AddVertex("5");
            graph.AddEdge("1", "2", 1);
            graph.AddEdge("1", "3", 10);
            graph.AddEdge("2", "4", 20);
            graph.AddEdge("3", "4", 11.1);
            List<Graph.Edge> tmp = graph.FindShortestPath("1", "4");
            foreach (Graph.Edge e in tmp)
            {
                Console.WriteLine($"{e.From} -> {e.To}: {e.Distance}");
            }
            graph.DepthFirstSearch("1");
            Console.WriteLine();
            graph.BreadthFirstSearch("1");
            InitializeComponent();
        }
    }
}

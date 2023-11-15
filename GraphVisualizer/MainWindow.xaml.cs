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
        byte mode = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = graph;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                graph.AddVertex($"{++count}");
                Vertex b = new Vertex();
                Canvas.SetTop(b, GraphCanvas.ActualHeight / 2);
                Canvas.SetLeft(b, GraphCanvas.ActualWidth / 2);
                object tmp = b.VertexCanvas.FindName("VertexName");
                if (tmp is TextBlock)
                {
                    TextBlock child = (TextBlock)tmp;
                    child.Text = $"{count}";
                }
                GraphCanvas.Children.Add(b);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CanvasLMBClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Canvas)
            {
                Point position = e.GetPosition(GraphCanvas);
                switch (mode)
                {
                    case 1:
                        try
                        {
                            graph.AddVertex($"{++count}");
                            Vertex b = new Vertex();
                            b.SetValue(Canvas.LeftProperty, position.X - 25);
                            b.SetValue(Canvas.TopProperty, position.Y - 25);
                            //Canvas.SetTop(b, position.X);
                            //Canvas.SetLeft(b, position.Y);
                            object tmp = b.VertexCanvas.FindName("VertexName");
                            if (tmp is TextBlock)
                            {
                                TextBlock child = (TextBlock)tmp;
                                child.Text = $"{count}";
                            }
                            GraphCanvas.Children.Add(b);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                }
            }
        }

        private void AddEdge_Checked(object sender, RoutedEventArgs e)
        {
            mode = 2;
        }

        private void AddVertex_Checked(object sender, RoutedEventArgs e)
        {
            mode = 1;
        }

        private void DragElement_Checked(object sender, RoutedEventArgs e)
        {
            mode = 3;
        }

        private void DeleteElement_Checked(object sender, RoutedEventArgs e)
        {
            mode = 4;
        }
    }
}

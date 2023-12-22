using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphVisualizer
{
    /// <summary>
    /// Логика взаимодействия для Vertex.xaml
    /// </summary>
    public partial class Vertex : UserControl, INotifyPropertyChanged
    {
        private object? movingObject;
        private double firstXPos, firstYPos;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double vertexRadius = 25;
        public List<Edge> _edgesOut = new();
        public List<Edge> _edgesIn = new();


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _vertexName = "";
        public string VertexName
        { 
            get
            {
                return _vertexName;
            }
            set
            {
                if (_vertexName != value)
                {
                    _vertexName = value;
                    OnPropertyChanged(nameof(VertexName));
                }
            }
        }

        public bool IsSelected { get; set; } = false;

        public Vertex()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Vertex_LMBDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Mode == 3)
            {
                Vertex vertex = (Vertex)sender;
                Canvas canvas = (Canvas)vertex.Parent;
                firstXPos = e.GetPosition(vertex).X;
                firstYPos = e.GetPosition(vertex).Y;
                movingObject = sender;
                int top = Panel.GetZIndex(vertex);
                foreach (var child in canvas.Children)
                    if (child is Vertex)
                    {
                        if (top < Panel.GetZIndex((Vertex)child))
                            top = Panel.GetZIndex((Vertex)child);
                    }
                Panel.SetZIndex(vertex, top + 1);
                vertex.CaptureMouse();
            }
        }

        private void Vertex_LMBUp(object sender, MouseButtonEventArgs e)
        {
            Vertex vertex = (Vertex)sender;
            movingObject = null;
            vertex.ReleaseMouseCapture();
        }

        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
            {
                Vertex vertex = (Vertex)sender;
                Canvas canvas = (Canvas)vertex.Parent;

                Point position = e.GetPosition(canvas);
                double newLeft = position.X - firstXPos;
                if (newLeft > canvas.ActualWidth - vertex.ActualWidth)
                    newLeft = canvas.ActualWidth - vertex.ActualWidth;
                else if (newLeft < 0)
                    newLeft = 0;

                double newTop = position.Y - firstYPos;
                if (newTop > canvas.ActualHeight - vertex.ActualHeight)
                    newTop = canvas.ActualHeight - vertex.ActualHeight;
                else if (newTop < 0)
                    newTop = 0;
                PositionX = newLeft;
                PositionY = newTop;

                vertex.SetValue(Canvas.LeftProperty, newLeft);
                vertex.SetValue(Canvas.TopProperty, newTop);
                CalculateEdgeCoordinates(newLeft, newTop);
            }
        }

        public void CalculateEdgeCoordinates(double newLeft, double newTop)
        {
            foreach (Edge edge in _edgesIn)
            {
                edge.CalculateArrowParameters(edge.EdgeLine.X1, edge.EdgeLine.Y1, newLeft + vertexRadius, newTop + vertexRadius);
            }
            foreach (Edge edge in _edgesOut)
            {
                edge.CalculateArrowParameters(newLeft + vertexRadius, newTop + vertexRadius, edge.EdgeLine.X2, edge.EdgeLine.Y2);
            }
        }

        public void Select()
        {
            IsSelected = true;
            Ellipse ellipse = (Ellipse)VertexCanvas.FindName("VertexObject");
            ellipse.Fill = Brushes.LightBlue;
        }

        public void DeSelect()
        {
            IsSelected = false;
            Ellipse ellipse = (Ellipse)VertexCanvas.FindName("VertexObject");
            ellipse.Fill = Brushes.White;
        }

        public void Remove()
        {
            Canvas canvas = (Canvas)this.Parent;
            foreach (Edge edge in _edgesIn)
            {
                canvas.Children.Remove(edge);
            }
            foreach(Edge edge in _edgesOut)
            {
                canvas.Children.Remove(edge);
            }
            canvas.Children.Remove(this);
        }
    }
}

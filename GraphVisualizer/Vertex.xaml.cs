using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public double _positionX { get; set; }
        public double _positionY { get; set; }
        public double _vertexRadius = 25;
        private List<Edge> _edgesOut = new List<Edge>();
        private List<Edge> _edgesIn = new List<Edge>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool _isSelected { get; set; } = false;

        public Vertex()
        {
            InitializeComponent();
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
            else if (MainWindow.Mode == 2)
            {
                if (_isSelected)
                {
                    DeSelect();
                }
                else
                {
                    Canvas canvas = (Canvas)this.Parent;
                    Vertex? firstVertex = null;
                    foreach (var i in canvas.Children)
                    {
                        if (i is Vertex)
                        {
                            if (((Vertex)i)._isSelected)
                                firstVertex = (Vertex)i;
                        }
                    }
                    if (firstVertex is null)
                    {
                        Select();
                        return;
                    }
                    firstVertex.DeSelect();
                    double x1 = firstVertex._positionX + _vertexRadius;
                    double y1 = firstVertex._positionY + _vertexRadius;
                    double x2 = _positionX + _vertexRadius;
                    double y2 = _positionY + _vertexRadius;
                    Edge edge = new Edge ();
                    Label label = (Label)edge.EdgeCanvas.FindName("EdgeWeight");
                    label.Content = edge.Weight;
                    Edge.CalculateArrowParameters(edge, x1, y1, x2, y2, _vertexRadius);
                    int minZIndex = Panel.GetZIndex(firstVertex) < Panel.GetZIndex(this) ? Panel.GetZIndex(firstVertex) : Panel.GetZIndex(this);
                    Panel.SetZIndex(edge, minZIndex - 1);
                    canvas.Children.Add(edge);
                    firstVertex._edgesOut.Add(edge);
                    _edgesIn.Add(edge);
                }
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
                _positionX = newLeft;
                _positionY = newTop;
                OnPropertyChanged(nameof(_positionX));
                OnPropertyChanged(nameof(_positionY));
                vertex.SetValue(Canvas.LeftProperty, newLeft);
                vertex.SetValue(Canvas.TopProperty, newTop);
                foreach (Edge edge in _edgesIn)
                {
                    Edge.CalculateArrowParameters(edge, edge.EdgeLine.X1, edge.EdgeLine.Y1, newLeft + _vertexRadius, newTop + _vertexRadius, _vertexRadius);
                }
                foreach (Edge edge in _edgesOut)
                {
                    Edge.CalculateArrowParameters(edge, newLeft + _vertexRadius, newTop + _vertexRadius, edge.EdgeLine.X2, edge.EdgeLine.Y2, _vertexRadius);
                }
            }
        }

        public void Select()
        {
            _isSelected = true;
            Ellipse ellipse = (Ellipse)this.VertexCanvas.FindName("VertexObject");
            ellipse.Fill = Brushes.LightBlue;
        }

        public void DeSelect()
        {
            _isSelected = false;
            Ellipse ellipse = (Ellipse)this.VertexCanvas.FindName("VertexObject");
            ellipse.Fill = Brushes.White;
        }
    }
}

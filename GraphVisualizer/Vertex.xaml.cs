using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Логика взаимодействия для Vertex.xaml
    /// </summary>
    public partial class Vertex : UserControl
    {
        private object movingObject;
        private double firstXPos, firstYPos;
        public Vertex()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Vertex_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(Vertex_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(Vertex_MouseMove);
        }
        private void Vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Vertex vertex = sender as Vertex;
            Canvas canvas = vertex.Parent as Canvas;
            firstXPos = e.GetPosition(vertex).X;
            firstYPos = e.GetPosition(vertex).Y;
            movingObject = sender;
            int top = Panel.GetZIndex(vertex);
            foreach (Vertex child in canvas.Children)
                if (top < Panel.GetZIndex(child))
                    top = Panel.GetZIndex(child);
            Panel.SetZIndex(vertex, top + 1);
            vertex.CaptureMouse();
        }

        private void Vertex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Vertex vertex = sender as Vertex;
            movingObject = null;
            vertex.ReleaseMouseCapture();
        }

        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
            {
                Vertex vertex = sender as Vertex;
                Canvas canvas = vertex.Parent as Canvas;

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

                vertex.SetValue(Canvas.LeftProperty, newLeft);
                vertex.SetValue(Canvas.TopProperty, newTop);
            }
        }
    }
}

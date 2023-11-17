using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace GraphVisualizer
{
    class Edge: Shape
    {
        public double X1
        {
            get { return (double)this.GetValue(X1Property); }
            set { this.SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X1Property = DependencyProperty.Register(
        "X1",
            typeof(double),
            typeof(Edge),
            new PropertyMetadata(0.0, OnX1PropertyChanged));

        private static void OnX1PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Edge control = sender as Edge;

            if (control != null)
            {

            }
        }

        public double Y1
        {
            get { return (double)this.GetValue(Y1Property); }
            set { this.SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register(
            "Y1",
            typeof(double),
            typeof(Edge),
            new PropertyMetadata(0.0, OnY1PropertyChanged));

        private static void OnY1PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Edge control = sender as Edge;

            if (control != null)
            {

            }
        }

        public double X2
        {
            get { return (double)this.GetValue(X2Property); }
            set { this.SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X2Property = DependencyProperty.Register(
            "X2",
            typeof(double),
            typeof(Edge),
            new PropertyMetadata(0.0, OnX2PropertyChanged));

        private static void OnX2PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Edge control = sender as Edge;

            if (control != null)
            {

            }
        }

        public double Y2
        {
            get { return (double)this.GetValue(Y2Property); }
            set { this.SetValue(Y2Property, value); }
        }

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register(
            "Y2",
            typeof(double),
            typeof(Edge),
            new PropertyMetadata(0.0, OnY2PropertyChanged));

        private static void OnY2PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Edge control = sender as Edge;

            if (control != null)
            {

            }
        }


        public Edge(double x1, double y1, double x2, double y2)
        {
            
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                LineGeometry line = new LineGeometry(new Point(X1, Y1), new Point(X2, Y2));
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 1;
                path.Data = line;
                return line;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Логика взаимодействия для Edge.xaml
    /// </summary>
    public partial class Edge : UserControl
    {
        public double Weight { get; set; } = 1;
        public Edge()
        {
            InitializeComponent();
        }

        public static void CalculateArrowParameters(Edge edge, double x1, double y1, double x2, double y2, double radius)
        {
            Line line = (Line)edge.EdgeCanvas.FindName("EdgeLine");
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            Label label = (Label)edge.EdgeCanvas.FindName("EdgeWeight");
            Line line2 = (Line)edge.EdgeCanvas.FindName("ArrowHead1");
            Line line3 = (Line)edge.EdgeCanvas.FindName("ArrowHead2");
            double length = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            double X = x2 - x1;
            double Y = y2 - y1;
            double Xp = y1 - y2;
            double Yp = x2 - x1;
            double x4 = x2 - X / length * 40;
            double y4 = y2 - Y / length * 40;
            double x5 = x4 + Xp / length * 5;
            double y5 = y4 + Yp / length * 5;
            double x6 = x4 - Xp / length * 5;
            double y6 = y4 - Yp / length * 5;
            line2.X1 = x2;
            line2.Y1 = y2;
            line2.X2 = x5;
            line2.Y2 = y5;
            line3.X1 = x2;
            line3.Y1 = y2;
            line3.X2 = x6;
            line3.Y2 = y6;
            label.SetValue(Canvas.LeftProperty, (line.X1 + line.X2) / 2 - label.ActualWidth / 2);
            label.SetValue(Canvas.TopProperty, (line.Y1 + line.Y2) / 2 - label.ActualHeight / 2);
        }
    }
}

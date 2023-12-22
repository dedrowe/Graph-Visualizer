using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace GraphVisualizer
{
    /// <summary>
    /// Логика взаимодействия для Edge.xaml
    /// </summary>
    public partial class Edge : UserControl, INotifyPropertyChanged
    {
        private double _distance;
        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }
        public Brush Color { get; set;}
        public string From { get; set; }
        public string To { get; set; }

        public Edge(string from, string to, double distance)
        {
            From = from;
            To = to;
            _distance = distance;
            Color = Brushes.Black;
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CalculateArrowParameters(double x1, double y1, double x2, double y2, int arrowHeadLength = 40, int arrowHeadPadding = 5)
        {
            Line arrow = (Line)EdgeCanvas.FindName("EdgeLine");
            arrow.Stroke = Color;
            arrow.X1 = x1;
            arrow.Y1 = y1;
            arrow.X2 = x2;
            arrow.Y2 = y2;

            Label label = (Label)EdgeCanvas.FindName("EdgeWeight");
            Line arrowHead1 = (Line)EdgeCanvas.FindName("ArrowHead1");
            Line arrowHead2 = (Line)EdgeCanvas.FindName("ArrowHead2");
            arrowHead1.Stroke = Color;
            arrowHead2.Stroke = Color;

            double length = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            if (length != 0)
            {
                double X = x2 - x1;
                double Y = y2 - y1;
                double Xp = y1 - y2;
                double Yp = x2 - x1;
                double arrowHeadEndX = x2 - X / length * arrowHeadLength;
                double arrowHeadEndY = y2 - Y / length * arrowHeadLength;
                double arrowHead1X = arrowHeadEndX + Xp / length * arrowHeadPadding;
                double arrowHead1Y = arrowHeadEndY + Yp / length * arrowHeadPadding;
                double arrowHead2X = arrowHeadEndX - Xp / length * arrowHeadPadding;
                double arrowHead2Y = arrowHeadEndY - Yp / length * arrowHeadPadding;
                arrowHead1.X1 = x2;
                arrowHead1.Y1 = y2;
                arrowHead1.X2 = arrowHead1X;
                arrowHead1.Y2 = arrowHead1Y;
                arrowHead2.X1 = x2;
                arrowHead2.Y1 = y2;
                arrowHead2.X2 = arrowHead2X;
                arrowHead2.Y2 = arrowHead2Y;
                label.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X2) / 2 - 5 + Xp / length * 30);
                label.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - 5 + Yp / length * 30);
            }
        }

        public void Repaint(Brush Color)
        {
            this.Color = Color;
            Line line = (Line)EdgeCanvas.FindName("EdgeLine");
            Line line2 = (Line)EdgeCanvas.FindName("ArrowHead1");
            Line line3 = (Line)EdgeCanvas.FindName("ArrowHead2");
            line.Stroke = Color;
            line2.Stroke = Color;
            line3.Stroke = Color;
        }
    }
}

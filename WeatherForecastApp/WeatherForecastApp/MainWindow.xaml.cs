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

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            //adding focus to search text box
            this.searchTextBox.GotFocus += searchTextBox_OnFocus;
            this.searchTextBox.LostFocus += searchTextBox_OnDefocus;
        }

        /* Search text box focus methods */
        private void searchTextBox_OnFocus(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Equals("Search..."))
                searchTextBox.Text = "";
        }

        private void searchTextBox_OnDefocus(object sender, EventArgs e)
        {
            if(searchTextBox.Text.Equals(""))
                searchTextBox.Text = "Search...";
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        

        private void loadWeatherByHours(object sender, EventArgs e)
        {

            int[] degrees = { 6, 11, 13, 15, 22, 20, 16, 11, 14, 12, 10, 7, 4, 6, 8};
            int maxTemp = degrees.Max();
            int minTemp = degrees.Min();
            int threshold = 5;
            
            double maxHeight = WeatherByHoursCanvas.ActualHeight;
            double maxWidth = WeatherByHoursCanvas.ActualWidth;
            double step = maxWidth / (degrees.Length - 1);

            Brush brush = Brushes.White;

            List<UIElement> elementsToRemove = new List<UIElement>();

            foreach (UIElement el in WeatherByHoursCanvas.Children)
            {
                if (el.GetType() == typeof(Polyline)) elementsToRemove.Add(el);

            }

            foreach (UIElement ui in elementsToRemove)
            {
                WeatherByHoursCanvas.Children.Remove(ui);
            }



            WeatherHourMinTemp.Text = minTemp.ToString() + "°C";
            WeatherHourMinTemp.SetValue(Canvas.TopProperty, maxHeight - WeatherHourMinTemp.ActualHeight/2 - minTemp * maxHeight / (maxTemp + threshold));
            WeatherHourMinTemp.SetValue(Canvas.LeftProperty, -WeatherHourMinTemp.ActualWidth);

            PointCollection ps1 = new PointCollection();
            ps1.Add(new Point(0, maxHeight - minTemp * maxHeight / (maxTemp + threshold)));
            ps1.Add(new Point(maxWidth, maxHeight - minTemp * maxHeight / (maxTemp + threshold)));
            Polyline line1 = new Polyline();
            line1.StrokeThickness = 1;
            line1.Stroke = Brushes.White;
            line1.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line1.Points = ps1;
            WeatherByHoursCanvas.Children.Add(line1);


            WeatherHourMaxTemp.Text = maxTemp.ToString() + "°C";
            WeatherHourMaxTemp.SetValue(Canvas.TopProperty, maxHeight - WeatherHourMaxTemp.ActualHeight / 2 - maxTemp * maxHeight / (maxTemp + threshold));
            WeatherHourMaxTemp.SetValue(Canvas.LeftProperty, -WeatherHourMaxTemp.ActualWidth);

            PointCollection ps2 = new PointCollection();
            ps2.Add(new Point(0, maxHeight - maxTemp * maxHeight / (maxTemp + threshold)));
            ps2.Add(new Point(maxWidth, maxHeight - maxTemp * maxHeight / (maxTemp + threshold)));
            Polyline line2 = new Polyline();
            line2.StrokeThickness = 1;
            line2.Stroke = Brushes.White;
            line2.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line2.Points = ps2;
            WeatherByHoursCanvas.Children.Add(line2);




            


            PointCollection points = new PointCollection();
            int i;
            for (i = 0; i < degrees.Length; i++)
            {
                double y = maxHeight - degrees[i]*maxHeight/ (maxTemp + threshold);
                points.Add(new Point(i * step, y));
            }
            

            step = maxWidth / 4;
            for (i = 0; i < 5; i++)
            {
                PointCollection ps = new PointCollection();
                ps.Add(new Point(i*step, 0));
                ps.Add(new Point(i*step, maxHeight));
                Polyline line = new Polyline();
                line.StrokeThickness = 1;
                line.Stroke = Brushes.White;
                line.StrokeDashArray = new DoubleCollection() { 2, 4 };
                line.Points = ps;
                WeatherByHoursCanvas.Children.Add(line);
            }

             WeatherHour1.SetValue(Canvas.LeftProperty, 0 - WeatherHour1.ActualWidth / 2);
             WeatherHour2.SetValue(Canvas.LeftProperty, maxWidth * 1 / 4 - WeatherHour2.ActualWidth / 2);
             WeatherHour3.SetValue(Canvas.LeftProperty, maxWidth * 2 / 4 - WeatherHour3.ActualWidth / 2);
             WeatherHour4.SetValue(Canvas.LeftProperty, maxWidth * 3 / 4 - WeatherHour4.ActualWidth / 2);
             WeatherHour5.SetValue(Canvas.LeftProperty, maxWidth * 4 / 4 - WeatherHour5.ActualWidth / 2);

            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 3;
            polyline.Stroke = brush;
            polyline.Points = points;

            WeatherByHoursCanvas.Children.Add(polyline);
            

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}

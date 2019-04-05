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
using System.Net.Http;
using System.Net.Http.Headers;

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static HttpClient client = new HttpClient(); //used for multiple requests to server
        static RestRequest rest_request = new RestRequest(); //object used for sending request to server

        public MainWindow()
        {
            InitializeComponent();

            //adding focus to search text box
            this.searchTextBox.GotFocus += searchTextBox_OnFocus;
            this.searchTextBox.LostFocus += searchTextBox_OnDefocus;

            //adding an on close event - disposing of the client
            this.Closed += new EventHandler(MainWindow_Closed);

            sendRequest("Novi Sad"); //testni primer
        }


        /* Search text box focus methods */
        private void searchTextBox_OnFocus(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Equals("Search..."))
                searchTextBox.Text = "";
        }

        private void searchTextBox_OnDefocus(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Equals(""))
                searchTextBox.Text = "Search...";
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void ReloadWeatherByHours(int[] degrees)
        {
            int averageTemp = (int)degrees.Average();
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
            WeatherHourMinTemp.SetValue(Canvas.TopProperty, maxHeight - WeatherHourMinTemp.ActualHeight / 2 - (Math.Abs(minTemp) + minTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp)));
            WeatherHourMinTemp.SetValue(Canvas.LeftProperty, -WeatherHourMinTemp.ActualWidth - 10);

            PointCollection ps1 = new PointCollection();
            ps1.Add(new Point(0, maxHeight - (Math.Abs(minTemp) + minTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
            ps1.Add(new Point(maxWidth, maxHeight - (Math.Abs(minTemp) + minTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
            Polyline line1 = new Polyline();
            line1.StrokeThickness = 1;
            line1.Stroke = Brushes.White;
            line1.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line1.Points = ps1;
            WeatherByHoursCanvas.Children.Add(line1);


            WeatherHourAverageTemp.Text = averageTemp.ToString() + "°C";
            WeatherHourAverageTemp.SetValue(Canvas.TopProperty, maxHeight - WeatherHourAverageTemp.ActualHeight / 2 - (Math.Abs(minTemp) + averageTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp)));
            WeatherHourAverageTemp.SetValue(Canvas.LeftProperty, -WeatherHourAverageTemp.ActualWidth - 10);

            PointCollection ps3 = new PointCollection();
            ps3.Add(new Point(0, maxHeight - (Math.Abs(minTemp) + averageTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
            ps3.Add(new Point(maxWidth, maxHeight - (Math.Abs(minTemp) + averageTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
            Polyline line3 = new Polyline();
            line3.StrokeThickness = 1;
            line3.Stroke = Brushes.White;
            line3.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line3.Points = ps3;
            WeatherByHoursCanvas.Children.Add(line3);



            WeatherHourMaxTemp.Text = maxTemp.ToString() + "°C";
            WeatherHourMaxTemp.SetValue(Canvas.TopProperty, maxHeight - WeatherHourMaxTemp.ActualHeight / 2 - (Math.Abs(minTemp) + maxTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp)));
            WeatherHourMaxTemp.SetValue(Canvas.LeftProperty, -WeatherHourMaxTemp.ActualWidth - 10);

            PointCollection ps2 = new PointCollection();
            ps2.Add(new Point(0, maxHeight - (Math.Abs(minTemp) + maxTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
            ps2.Add(new Point(maxWidth, maxHeight - (Math.Abs(minTemp) + maxTemp + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp))));
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
                double y = maxHeight - (Math.Abs(minTemp) + degrees[i] + threshold / 2) * maxHeight / (maxTemp + threshold + Math.Abs(minTemp));
                points.Add(new Point(i * step, y));
            }


            step = maxWidth / 4;
            for (i = 0; i < 5; i++)
            {
                PointCollection ps = new PointCollection();
                ps.Add(new Point(i * step, 0));
                ps.Add(new Point(i * step, maxHeight));
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

        private void loadWeatherByHours(object sender, EventArgs e)
        {

           ReloadWeatherByHours(new int[] { 5, 7, 10, 15, 16, 20, 18, 14, 11, 8 ,2});


        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
            ReloadWeatherByHours(new int[] { 5, 7, 10, 15, 16, 20, 18, 14, 11, 8, 2 });
        }




        /* Main window on close method */
        public void MainWindow_Closed(object sender, EventArgs e)
        {
            //Dispose once all HttpClient calls are complete
            client.Dispose();
        }


        /* Method used for sending a request to server */

        public void sendRequest(string cityName)
        {

            rest_request.CityName = cityName;

            rest_request.sendRequestToOpenWeather(client);
        }
    }
}

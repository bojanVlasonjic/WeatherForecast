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
using System.Windows.Media.Animation;

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string sunnyBackgroundPath = "data/images/sunny.jpg";
        const string cloudyBackgroundPath = "data/images/cloudy.jpg";
        const string snowyBackgroundPath = "data/images/snowy.jpg";
        const string rainyBackgroundPath = "data/images/rainy.jpg";
        const string foggyBackgroundPath = "data/images/foggy.jpg";

        const string sunnyIconPath = "data/icons/sunnyIcon.png";
        const string cloudyIconPath = "data/icons/cloudyIcon.png";
        const string snowyIconPath = "data/icons/snowyIcon.png";
        const string rainyIconPath = "data/icons/rainyIcon.png";
        const string cloudySunnyIconPath = "data/icons/cloudySunnyIcon.png";
        const string thunderstormIconPath = "data/icons/thunderstormIcon.png";


        static HttpClient client = new HttpClient(); //used for multiple requests to server
        static RestRequest rest_request = new RestRequest(); //object used for sending request to server
        static OpenWeatherCities openWeatherCities;
        static RootObject root;

        public MainWindow()
        {
            InitializeComponent();

            //adding focus to search text box
            this.searchTextBox.GotFocus += searchTextBox_OnFocus;
            this.searchTextBox.LostFocus += searchTextBox_OnDefocus;

            //adding an on close event - disposing of the client
            this.Closed += new EventHandler(MainWindow_Closed);

            //TODO: inicijalni update za trenutnu lokaciju

        }

        private void UpdateSearchTextBox(string searchValue)
        {

            
            if (SearchSelectionWindow.Visibility == Visibility.Hidden)
            {
                SearchSelectionWindow.Visibility = Visibility.Visible;
            }

            if (openWeatherCities == null)
            {
                return;
            }

            else if (openWeatherCities.Cities == null)
            {
                return;
            }

            if (searchValue.Equals(""))
            {

                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        SearchOption1_Text1.Text = openWeatherCities.Cities[i].name;
                        SearchOption1_Text2.Text = openWeatherCities.Cities[i].country;
                    }

                    else if (i == 1)
                    {
                        SearchOption2_Text1.Text = openWeatherCities.Cities[i].name;
                        SearchOption2_Text2.Text = openWeatherCities.Cities[i].country;
                    }

                    else if (i == 2)
                    {
                        SearchOption3_Text1.Text = openWeatherCities.Cities[i].name;
                        SearchOption3_Text2.Text = openWeatherCities.Cities[i].country;
                    }

                }
            }

            else
            {


                SearchOption1.Visibility = Visibility.Hidden;
                SearchOption2.Visibility = Visibility.Hidden;
                SearchOption3.Visibility = Visibility.Hidden;

                List<string> cityNames = new List<string>();
                for (int i = 0; i < 3; i++)
                {

                    string name = "";
                    string country = "";
                    bool continueSearch = false;
                    foreach (City c in openWeatherCities.Cities)
                    {
                        if (c.name.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase)>=0 && !cityNames.Contains(c.name + " " + c.country))
                        {
                            name = c.name;
                            country = c.country;
                            cityNames.Add(name + " " + country);
                            continueSearch = true;

                            if (i == 0)
                            {
                                SearchOption1.Visibility = Visibility.Visible;
                                SearchOption1_Text1.Text = name;
                                SearchOption1_Text2.Text = country;
                            }

                            else if (i == 1)
                            {
                                SearchOption2.Visibility = Visibility.Visible;
                                SearchOption2_Text1.Text = name;
                                SearchOption2_Text2.Text = country;
                            }

                            else if (i == 2)
                            {
                                SearchOption3.Visibility = Visibility.Visible;
                                SearchOption3_Text1.Text = name;
                                SearchOption3_Text2.Text = country;
                            }


                            break;
                        }
                    }


                    if (!continueSearch)        //increase performance of search (prevent additional search if not necessary)
                    {
                        break;
                    }


                }
            }
        }

        /* Search text box focus methods */
        private void searchTextBox_OnFocus(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Equals("Search..."))
            {
                searchTextBox.Text = "";
                SearchSelectionWindow.Visibility = Visibility.Visible;
                this.UpdateSearchTextBox(searchTextBox.Text);
            }
                
            
        }

        private void searchTextBox_OnDefocus(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Equals(""))
            {
                searchTextBox.Text = "Search...";
                SearchSelectionWindow.Visibility = Visibility.Hidden;
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textValue = searchTextBox.Text;
            if (textValue.Equals("") || textValue.Equals("Search..."))
            {
                return;
            }

            UpdateSearchTextBox(textValue);
        }

        /* Graph method */
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

        private void loadWeatherByHours(object sender, EventArgs e)                                     //Method called on window load
        {
            openWeatherCities = new OpenWeatherCities(); //WARNING! Loading huge data (cities)
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


        /* Button events */
        private void addToFavouritesBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            //za sad po default-u dobavi podatke za novi sad
            root = sendRequestForecast("Novi Sad");

            //refresh the displayed data
            if(root != null)
            {
                ChangeDisplayData();    //replaced method xD
            }

        }


        /* Method used for sending a forecast request to server.
         * Returns the forecast for the next week for every 3 hours */
        public RootObject sendRequestForecast(string cityName)
        {

            rest_request.CityName = cityName;

            RestResponse response = rest_request.sendRequestToOpenWeather(client, rest_request.URL_Forecast); //get response from server

            if (response.Root == null)
            {
                MessageBox.Show(response.Message); //something wen't wrong in the response
                return null;
            }

            //if the json parsed, but some of the importan't data is null
            if(response.Root.city == null || response.Root.list == null)
            {
                MessageBox.Show("Something went wrong. Please try again in a couple of minutes");
                return null;
            }

            return response.Root;
        }


        /* AUXILIARY METHODS */
        public double convertKelvinToCelsius(double kelvinVal)
        {
            double celsius = kelvinVal - 273.15;
            return Math.Round(celsius);
        }

        public double convertMetersToKilometers(double meters)
        {
            double kms = meters / 1000;
            return Math.Round(kms);
        }

        public void updateBasicTemperatureData(RootObject root)
        {
            /* Update current data */
            cityNameTextBlock.Text = $"{root.city.name}, {root.city.country}";
            temperatureTextBlock.Text = $"{convertKelvinToCelsius(root.list[0].main.temp)}°C";
            
            humidityTextBlock.Text = $"Humidity: {root.list[0].main.humidity}%";
            windSpeedTextBlock.Text = $"Wind speed: {root.list[0].wind.speed} m/s";

            pressureTextBlock.Text = $"Pressure: {root.list[0].main.pressure} mbar";

            updateMinAndMaxTemp(); 

            updateTimeTextBlock.Text = $"Last update at: {DateTime.Now.ToShortTimeString()}";

            if (root.list[0].weather.Count > 0)
            {
                smallDescrTextBlock.Text = root.list[0].weather[0].description;
                changeIconAndBackground(true, weatherIcon, root.list[0].weather[0].main);
                    
            } 

        }

        public void updateMinAndMaxTemp()
        {
            DateTime latest_temp = Convert.ToDateTime(root.list[0].dt_txt);
            double minTemp = 100000;
            double maxTemp = -100000;

            const int endIndex = 8;
            //iterate over the next 24 hours (3 hours * 8 = 24 hours)
            for(int i = 0; i < endIndex; i++)
            {
                //update max temp
                double currTemp = convertKelvinToCelsius(root.list[i].main.temp);

                if(currTemp > maxTemp)
                {
                    maxTemp = convertKelvinToCelsius(root.list[i].main.temp);
                }

                //update min temp
                if (currTemp < minTemp)
                {
                    minTemp = convertKelvinToCelsius(root.list[i].main.temp);
                }

            }

            //change gui components
            minTempTextBlock.Text = $"Minimum: {minTemp}°C";
            maxTempTextBlock.Text = $"Maximum: {maxTemp}°C";

        }


        public void updateDailyTemperatureData()
        {

            DateTime today = DateTime.Now;
            int counter = 0; //day counter

            DateTime followingDay = today.AddDays(counter);

            string weatherType = ""; //rainy, cloudy, ... used for icons
            string smallDescr = ""; //light rain - used for description

            double minTemp = 100000;
            double maxTemp = -100000;

            const int endIndex = 40;

            //update the temperature for the following 5 days, measuring min and max temp over 24 hours from now
            // 3 hours * 8 intervals = 24 hours; 8 intervals * 5 days = 40
            for (int i = 0; i < endIndex; i++)
            {
                //24 hours passed, reseting temperatures
                if(i%8 == 0 && i != 0)
                {
                    updateDailyValues(counter, minTemp, maxTemp, followingDay, weatherType, smallDescr);

                    minTemp = 100000;
                    maxTemp = -100000;

                    counter++;
                    followingDay = today.AddDays(counter);
                }

                //collect weather description after 12 hours 
                if(i%4 == 0)
                {
                    weatherType = root.list[i].weather[0].main;
                    smallDescr = root.list[i].weather[0].description;
                }

                double currTemp = convertKelvinToCelsius(root.list[i].main.temp);

                //update max temp
                if (currTemp > maxTemp)
                {
                    maxTemp = convertKelvinToCelsius(root.list[i].main.temp);
                }

                //update min temp
                if (currTemp < minTemp)
                {
                    minTemp = convertKelvinToCelsius(root.list[i].main.temp);
                }

                //update one last time at the end
                if(i == endIndex - 1) {
                    updateDailyValues(counter, minTemp, maxTemp, followingDay, weatherType, smallDescr);
                }
            }
        }

        private void updateDailyValues(int counter, double minTemp, double maxTemp, DateTime day, string weatherType, string descr)
        {
            switch(counter)
            {
                case 0:
                    day1TextBlock.Text = day.DayOfWeek.ToString();
                    day1TempTextBlock.Text = $"{maxTemp} / {minTemp}°C";
                    changeIconAndBackground(false, day1WeatherIcon, weatherType);
                    smallDescr1TextBlock.Text = descr; //update small description
                    break;

                case 1:
                    day2TextBlock.Text = day.DayOfWeek.ToString();
                    day2TempTextBlock.Text = $"{maxTemp} / {minTemp}°C";
                    changeIconAndBackground(false, day2WeatherIcon, weatherType);
                    smallDescr2TextBlock.Text = descr; //update small description
                    break;

                case 2:
                    day3TextBlock.Text = day.DayOfWeek.ToString();
                    day3TempTextBlock.Text = $"{maxTemp} / {minTemp}°C";
                    changeIconAndBackground(false, day3WeatherIcon, weatherType);
                    smallDescr3TextBlock.Text = descr; //update small description
                    break;

                case 3:
                    day4TextBlock.Text = day.DayOfWeek.ToString();
                    day4TempTextBlock.Text = $"{maxTemp} / {minTemp}°C";
                    changeIconAndBackground(false, day4WeatherIcon, weatherType);
                    smallDescr4TextBlock.Text = descr; //update small description
                    break;

                case 4:
                    day5TextBlock.Text = day.DayOfWeek.ToString();
                    day5TempTextBlock.Text = $"{maxTemp} / {minTemp}°C";
                    changeIconAndBackground(false, day5WeatherIcon, weatherType);
                    smallDescr5TextBlock.Text = descr; //update small description
                    break;

            }
        }

        private void changeIconAndBackground(bool updateBackground, Image iconImage, string weatherDescr)
        {
            
             switch (weatherDescr.ToLower())
             {
                case "clear":
                    updateIconsAndBackground(updateBackground, iconImage, sunnyIconPath, sunnyBackgroundPath);
                    return;

                case "clouds":
                    updateIconsAndBackground(updateBackground, iconImage, cloudySunnyIconPath, cloudyBackgroundPath);
                    return;

                case "rain":
                    updateIconsAndBackground(updateBackground, iconImage, rainyIconPath, rainyBackgroundPath);
                    return;

                case "snow":
                    updateIconsAndBackground(updateBackground, iconImage, snowyIconPath, snowyBackgroundPath);
                    return;

                case "haze":
                    updateIconsAndBackground(updateBackground, iconImage, cloudyIconPath, foggyBackgroundPath);
                    return;

                default:
                    return;
            }
        }


        private void updateIconsAndBackground(bool updateBackground, Image iconImage, string iconPath, string backgroundPath)
        {
            ChangeIcon(iconImage, iconPath);

            if(updateBackground)
            {
                ChangeBackgroundImage(backgroundPath);
            }
        }

        private void SearchOption1_MouseEnter(object sender, MouseEventArgs e)
        {

            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#276582")); 
            SearchOption1_Rectangle.Fill = brush;
        }

        private void SearchOption1_MouseLeave(object sender, MouseEventArgs e)
        {
            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#205770"));
            SearchOption1_Rectangle.Fill = brush;
        }

        private void SearchOption2_MouseEnter(object sender, MouseEventArgs e)
        {
            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#276582"));
            SearchOption2_Rectangle.Fill = brush;
        }

        private void SearchOption2_MouseLeave(object sender, MouseEventArgs e)
        {
            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#205770"));
            SearchOption2_Rectangle.Fill = brush;
        }

        private void SearchOption3_MouseEnter(object sender, MouseEventArgs e)
        {
            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#276582"));
            SearchOption3_Rectangle.Fill = brush;
        }

        private void SearchOption3_MouseLeave(object sender, MouseEventArgs e)
        {
            Brush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#205770"));
            SearchOption3_Rectangle.Fill = brush;
        }

        private void SearchOption1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Text = SearchOption1_Text1.Text;
            
            root = sendRequestForecast(SearchOption1_Text1.Text);
            if (root != null)
            {
                ChangeDisplayData();
            }


            SearchSelectionWindow.Visibility = Visibility.Hidden;

        }

        private void SearchOption2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Text = SearchOption2_Text1.Text;

            root = sendRequestForecast(SearchOption2_Text1.Text);
            if (root != null)
            {
                ChangeDisplayData();
            }


            SearchSelectionWindow.Visibility = Visibility.Hidden;
        }

        private void SearchOption3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Text = SearchOption3_Text1.Text;

            root = sendRequestForecast(SearchOption3_Text1.Text);
            if (root != null)
            {
                ChangeDisplayData();
            }


            SearchSelectionWindow.Visibility = Visibility.Hidden;
        }


        private void ChangeDisplayData()
        {

            StartDataChangeAnimation();
            
            
        }


        //Might cause troubles with invalid path
        private void ChangeIcon(Image image, string imageSourcePath)
        {
            image.Source = (ImageSource) new ImageSourceConverter().ConvertFrom(imageSourcePath);
        }

        //Change background image
        private void ChangeBackgroundImage(string imageSourcePath)
        {
            ApplicationBackgroundImage.ImageSource = new BitmapImage(new Uri(imageSourcePath, UriKind.Relative));   
        }



        //Changing DATA animation
        private void StartDataChangeAnimation()
        {
            
            
            DoubleAnimation da = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                AutoReverse = false
            };
            da.Completed += new EventHandler(backgroundFadeOutCompleted);

            ApplicationBackground.BeginAnimation(OpacityProperty, da);
        }

        
        private void backgroundFadeOutCompleted(object sender, EventArgs e)
        {
            updateBasicTemperatureData(root);   //Update data here, when fade out is finished
            updateDailyTemperatureData();

            
            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                AutoReverse = false
            };
            ApplicationBackground.BeginAnimation(OpacityProperty, da);
        }

        private void showFavouritesBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WeatherDayMouseEnter(object sender, MouseEventArgs e)
        {

            SolidColorBrush background =  (SolidColorBrush) this.FindResource("TransparentBackground");

            Grid grid = (Grid)sender;
            grid.Background = background;
        }

        private void WeatherDayMouseLeave(object sender, MouseEventArgs e)
        {
            

            Grid grid = (Grid)sender;
            grid.Background = Brushes.Transparent;
        }
    }
}

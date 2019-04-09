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
using System.Windows.Threading;
using System.Threading;

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string sunnyBackgroundPath = "SunnyBackground";
        const string cloudyBackgroundPath = "CloudyBackground";
        const string snowyBackgroundPath = "SnowyBackground";
        const string rainyBackgroundPath = "RainyBackground";
        const string foggyBackgroundPath = "FoggyBackground";

        const string deleteFavoritesIconPath = "DeleteCross";
        const string sunnyIconPath = "SunnyIcon";
        const string cloudyIconPath = "CloudyIcon";
        const string snowyIconPath = "SnowyIcon";
        const string rainyIconPath = "RainyIcon";
        const string cloudySunnyIconPath = "SunnyCloudyIcon";
        const string thunderstormIconPath = "ThunderStormIcon";

        static HttpClient client = new HttpClient(); //used for multiple requests to server
        static HttpClient locationClient = new HttpClient();
        static RestRequest rest_request = new RestRequest(); //object used for sending request to server
        static OpenWeatherCities openWeatherCities;
        static RootObject root;
        static FavoriteCities favoriteCities;

        static int[] graphTemperatures = new int[8];
        static string[] graphTimeIntervals = new string[5];

        static DispatcherTimer userNotificationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };

        static SerializableRootObject serial_root_object = new SerializableRootObject();

        static bool isFavoritesShown = false;

        static string currentCity;


        public MainWindow()
        {
            //loadiing ci
            Thread load_cities_thread = new Thread(load_cities);
            load_cities_thread.Start();

            InitializeComponent();

            //Initial update of the weather
            loadLastSavedData();

            //LocationData d = sendRequestLocation();

            //adding focus to search text box
            this.searchTextBox.GotFocus += searchTextBox_OnFocus;
            this.searchTextBox.LostFocus += searchTextBox_OnDefocus;

            //adding an on close event - disposing of the client
            this.Closed += new EventHandler(MainWindow_Closed);

        }

        private void load_cities()
        {
            openWeatherCities = new OpenWeatherCities();
        }

        private void loadDataForCurrentLocation()
        {
            LocationData locationData = sendRequestLocation();

            if (locationData != null)
            {
                RootObject rootRequest = sendRequestForecast(locationData.city);
                
                if (rootRequest != null)
                {
                    root = rootRequest;
                    currentCity = root.city.name;
                    ChangeDisplayData();
                }

            }
        }


        private void loadLastSavedData()
        {
            //TODO: Postavi city name, kako odgovor ne bi bio null
            RestResponse resp = rest_request.sendRequestToOpenWeather(client, rest_request.URL_Forecast);
            //something wen't wrong, or there is no internet connection
            if (resp.Root == null)
            {
                root = RootObjectIO.ReadRootFromFile();

                if (root != null)
                {
                    updateBasicTemperatureData(root);   //Update data here, when fade out is finished
                    updateDailyTemperatureData();
                    currentCity = root.city.name;
                    dayDisplayedTextBlock.Text = $"Currenly showing data for {DateTime.Now.DayOfWeek}";
                }

                else
                {
                    //TODO: update the weather based on current location - IP LOCATOR
                    loadDataForCurrentLocation();
                }

            }
           
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
        public void ReloadWeatherByHours(int[] degrees, string[] hours)
        {
            if (hours.Length != 5) throw new ArgumentException("Hours should be string[5]");



            int averageTemp = (int)degrees.Average();
            int maxTemp = degrees.Max();
            int minTemp = degrees.Min();
            int threshold = 0;

            int graphSuppress = maxTemp - minTemp + threshold;

            double maxHeight = WeatherByHoursCanvas.ActualHeight;
            double maxWidth = WeatherByHoursCanvas.ActualWidth;
            double step = maxWidth / (degrees.Length - 1);

            WeatherHour1.Text = hours[0];
            WeatherHour2.Text = hours[1];
            WeatherHour3.Text = hours[2];
            WeatherHour4.Text = hours[3];
            WeatherHour5.Text = hours[4];

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
            WeatherHourMinTemp.SetValue(Canvas.TopProperty, -WeatherHourMinTemp.ActualHeight/2 + (maxTemp - minTemp + threshold / 2) * maxHeight / graphSuppress);
            WeatherHourMinTemp.SetValue(Canvas.LeftProperty, -WeatherHourMinTemp.ActualWidth - 15);

            PointCollection ps1 = new PointCollection();
            ps1.Add(new Point(0, (maxTemp - minTemp + threshold / 2) * maxHeight / graphSuppress));
            ps1.Add(new Point(maxWidth, (maxTemp - minTemp + threshold / 2) * maxHeight / graphSuppress));
            Polyline line1 = new Polyline();
            line1.StrokeThickness = 1;
            line1.Stroke = Brushes.White;
            line1.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line1.Points = ps1;
            WeatherByHoursCanvas.Children.Add(line1);


            WeatherHourAverageTemp.Text = averageTemp.ToString() + "°C";
            WeatherHourAverageTemp.SetValue(Canvas.TopProperty, -WeatherHourAverageTemp.ActualHeight / 2 + (maxTemp - averageTemp + threshold / 2) * maxHeight / graphSuppress);
            WeatherHourAverageTemp.SetValue(Canvas.LeftProperty, -WeatherHourAverageTemp.ActualWidth - 15);

            PointCollection ps3 = new PointCollection();
            ps3.Add(new Point(0, (maxTemp - averageTemp + threshold / 2) * maxHeight / graphSuppress));
            ps3.Add(new Point(maxWidth, (maxTemp - averageTemp + threshold / 2) * maxHeight / graphSuppress));
            Polyline line3 = new Polyline();
            line3.StrokeThickness = 1;
            line3.Stroke = Brushes.White;
            line3.StrokeDashArray = new DoubleCollection() { 10, 20 };
            line3.Points = ps3;
            WeatherByHoursCanvas.Children.Add(line3);



            WeatherHourMaxTemp.Text = maxTemp.ToString() + "°C";
            WeatherHourMaxTemp.SetValue(Canvas.TopProperty, -WeatherHourMaxTemp.ActualHeight / 2 + (maxTemp - maxTemp + threshold / 2) * maxHeight / graphSuppress);
            WeatherHourMaxTemp.SetValue(Canvas.LeftProperty, -WeatherHourMaxTemp.ActualWidth - 15);

            PointCollection ps2 = new PointCollection();
            ps2.Add(new Point(0, (maxTemp - maxTemp + threshold / 2) * maxHeight / graphSuppress));
            ps2.Add(new Point(maxWidth, (maxTemp - maxTemp + threshold / 2) * maxHeight / graphSuppress));
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
                double y = (maxTemp - degrees[i] + threshold / 2) * maxHeight / graphSuppress;
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
            polyline.StrokeThickness = 2;
            polyline.Stroke = brush;
            polyline.Points = points;

            WeatherByHoursCanvas.Children.Add(polyline);
        }

        private void loadWeatherByHours(object sender, EventArgs e)  //Method called on window load
        {
            //WARNING! Loading huge data (cities) -> no worries, I made a thread for it :P
            favoriteCities = new FavoriteCities();
            ReloadWeatherByHours(graphTemperatures, graphTimeIntervals);


            AddAllCitiesToFavoriteHolder();     //on startup put all user favorites to favorite holder
        }


        //Add favorite cities to favorite holder
        private void AddAllCitiesToFavoriteHolder()
        {
            foreach(string city in favoriteCities.Cities)
            {
                AddItemToFavorites(city);
            }
        }


        private void WindowSizeChanged(object sender, EventArgs e)
        {
            ReloadWeatherByHours(graphTemperatures, graphTimeIntervals);
        }


        /* Main window on close method */
        public void MainWindow_Closed(object sender, EventArgs e)
        {
            //Dispose once all HttpClient calls are complete
            client.Dispose();
            locationClient.Dispose();
        }


        /* Button events */
        private void addToFavouritesBtn_Click(object sender, RoutedEventArgs e) //Add current city to favorites
        {

            if (root != null)
            {
                if (!favoriteCities.CityExists(root.city.name))
                {
                    favoriteCities.AddCity(root.city.name);
                    AddItemToFavorites(root.city.name);
                    NotifyUser("Current location has been added to favorites");
                }
                else
                {
                    NotifyUser("Current location is already in favorites");
                }
            }
            else
            {
                NotifyUser("Can't add current place to favorites");
            }





        }

        private void AddItemToFavorites(string cityName)
        {

            Grid mainGrid = new Grid();
            mainGrid.Height = 50;
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            var colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(80);
            mainGrid.ColumnDefinitions.Add(colDefinition);

            Grid textGrid = new Grid();
            textGrid.SetValue(Grid.ColumnProperty, 0);
            textGrid.MouseEnter += FavoritesItem_MouseEnter;
            textGrid.MouseLeave += FavoritesItem_MouseLeave;
            textGrid.MouseDown += FavoritesItem_MouseDown;

            TextBlock data = new TextBlock();
            data.Text = cityName;
            data.Foreground = Brushes.White;
            data.HorizontalAlignment = HorizontalAlignment.Left;
            data.VerticalAlignment = VerticalAlignment.Center;
            data.FontSize = 16;
            data.FontWeight = FontWeights.SemiBold;
            data.Margin = new Thickness(15, 0, 0, 0);

            textGrid.Children.Add(data);

            Button deleteBtn = new Button();
            deleteBtn.Width = 60;
            deleteBtn.Height = 40;
            deleteBtn.SetValue(Grid.ColumnProperty, 1);
            deleteBtn.ToolTip = "Delete";

            Image img = new Image();
            img.Source = (BitmapImage)Resources[deleteFavoritesIconPath];
            img.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);

            deleteBtn.Content = img;
            deleteBtn.Click += DeleteFavoriteItem;


            mainGrid.Children.Add(textGrid);
            mainGrid.Children.Add(deleteBtn);


            FavoritesData.Children.Add(mainGrid);


        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {

            RootObject root_from_server = sendRequestForecast(currentCity); //updating data from server

            //refresh the displayed data
            if(root_from_server != null)
            {
                root = root_from_server;
                ChangeDisplayData();    //replaced method xD
            }

        }

        //Retreiving location data
        private LocationData sendRequestLocation()
        {

            LocationRestResponse response = rest_request.sendRequestToIPStack(client, rest_request.LOCATION_URL); //get response from server

            if (response.Root == null)
            {
                NotifyUser(response.Message); //something wen't wrong in the response
                return null;
            }

            //if the json parsed, but some of the importan't data is null
            if (response.Root.city == null)
            {
                NotifyUser("Something went wrong. Please try again in a couple of minutes");
                return null;
            }

            return response.Root;
        }

        /* Method used for sending a forecast request to server.
         * Returns the forecast for the next week for every 3 hours */
        public RootObject sendRequestForecast(string cityName)
        {

            rest_request.CityName = cityName;

            RestResponse response = rest_request.sendRequestToOpenWeather(client, rest_request.URL_Forecast); //get response from server

            if (response.Root == null)
            {
                NotifyUser(response.Message); //something wen't wrong in the response
                return null;
            }

            //if the json parsed, but some of the importan't data is null
            if(response.Root.city == null || response.Root.list == null)
            {
                NotifyUser("Something went wrong. Please try again in a couple of minutes");
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
            visibilityTextBlock.Text = $"Wind direction: {Math.Round(root.list[0].wind.deg, 0)}°";

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

            int endIndex = root.list.Count;

            //dictionaries for measuring most common occurences of weather
            Dictionary<string, int> weatherTypes = new Dictionary<string, int>();
            Dictionary<string, int> smallDescriptions = new Dictionary<string, int>();

            int temperatureIndex = 0;
            int timeIntervalsIndex = 0;

            bool graph_update_flag = true;

            //update the temperature for the following 5 days, measuring min and max temp over 24 hours from now
            // 3 hours * 8 intervals = 24 hours; 8 intervals * 5 days = 40
            for (int i = 0; i < endIndex; i++)
            {

                if (i % 2 == 0 && graph_update_flag)
                {
                    string[] tokens = root.list[i].dt_txt.Split(' ');
                    graphTimeIntervals[timeIntervalsIndex] = tokens[1].Substring(0, tokens[1].Length - 3); //no need for seconds in format
                    timeIntervalsIndex++;
                }

                if (temperatureIndex > 7 || timeIntervalsIndex > 4)
                {
                    graph_update_flag = false;
                }

                //24 hours passed, reseting temperatures
                if(i%8 == 0 && i != 0)
                {
                    //find the most common weather description and occurence over the previous 24 hours
                    weatherType = weatherTypes.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                    smallDescr = smallDescriptions.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

                    updateDailyValues(counter, minTemp, maxTemp, followingDay, weatherType, smallDescr);

                    //reset temperature data
                    minTemp = 100000;
                    maxTemp = -100000;

                    //reset dictionaries for counting weather occurences
                    weatherTypes.Clear();
                    smallDescriptions.Clear();

                    //24 hours passed, move on to the next day
                    counter++;
                    followingDay = today.AddDays(counter);
                }

                /*
                //collect weather description after 12 hours
                if(i%4 == 0)
                {
                    weatherType = root.list[i].weather[0].main;
                    smallDescr = root.list[i].weather[0].description;
                } */

                //update small descriptions occurences
                if(smallDescriptions.ContainsKey(root.list[i].weather[0].description))
                {
                    smallDescriptions[root.list[i].weather[0].description] += 1;
                } else
                {
                    smallDescriptions[root.list[i].weather[0].description] = 1;
                }

                //update weather type occurences
                if(weatherTypes.ContainsKey(root.list[i].weather[0].main)) {
                    weatherTypes[root.list[i].weather[0].main] += 1;
                } else
                {
                    weatherTypes[root.list[i].weather[0].main] = 1;
                }

                double currTemp = convertKelvinToCelsius(root.list[i].main.temp);


                if(graph_update_flag)
                {
                    graphTemperatures[temperatureIndex] = (int)currTemp;
                }

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
                    //find the most common weather description and occurence over the previous 24 hours
                    weatherType = weatherTypes.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                    smallDescr = smallDescriptions.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

                    updateDailyValues(counter, minTemp, maxTemp, followingDay, weatherType, smallDescr);
                }

                temperatureIndex++;
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

            RootObject root_from_server = sendRequestForecast(SearchOption1_Text1.Text);
            if (root_from_server != null)
            {
                root = root_from_server;
                ChangeDisplayData();
            }


            SearchSelectionWindow.Visibility = Visibility.Hidden;

        }

        private void SearchOption2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Text = SearchOption2_Text1.Text;

            RootObject root_from_server = sendRequestForecast(SearchOption2_Text1.Text);
            if (root_from_server != null)
            {
                root = root_from_server;
                ChangeDisplayData();
            }


            SearchSelectionWindow.Visibility = Visibility.Hidden;
        }

        private void SearchOption3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Text = SearchOption3_Text1.Text;

            RootObject root_from_server = sendRequestForecast(SearchOption3_Text1.Text);
            if (root_from_server != null)
            {
                root = root_from_server;
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
            //image.Source = (ImageSource) new ImageSourceConverter().ConvertFrom(imageSourcePath);
            image.Source = (BitmapImage)Resources[imageSourcePath];
        }

        //Change background image
        private void ChangeBackgroundImage(string imageSourcePath)
        {
            //ApplicationBackgroundImage.ImageSource = new BitmapImage(new Uri(imageSourcePath, UriKind.Relative));
            ApplicationBackgroundImage.ImageSource = (BitmapImage)Resources[imageSourcePath];
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


            //memorising the last updated location, in order for it to load at the beginning
            if(root != null)
            {
                updateBasicTemperatureData(root);   //Update data here, when fade out is finished
                updateDailyTemperatureData();

                //updating graph
                ReloadWeatherByHours(graphTemperatures, graphTimeIntervals);

                //updating text block identifying the graph
                dayDisplayedTextBlock.Text = $"Currenly showing data for {DateTime.Now.DayOfWeek}";

                currentCity = root.city.name;
                RootObjectIO.WriteToFile(root);
            }


            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                AutoReverse = false
            };
            ApplicationBackground.BeginAnimation(OpacityProperty, da);
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

        private void showFavouritesBtn_Click(object sender, RoutedEventArgs e)
        {

            if (!isFavoritesShown)
            {

                DoubleAnimation da = new DoubleAnimation
                {
                    From = -FavoritesHolder.Width,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                    AutoReverse = false
                };

                FavoritesHolder.BeginAnimation(Canvas.LeftProperty, da);
                isFavoritesShown = true;
            }
        }

        private void CloseFavoritesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isFavoritesShown)
            {

                DoubleAnimation da = new DoubleAnimation
                {
                    From = 0,
                    To = -FavoritesHolder.Width,
                    Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                    AutoReverse = false
                };

                FavoritesHolder.BeginAnimation(Canvas.LeftProperty, da);
                isFavoritesShown = false;
            }
        }

        private void FavoritesItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#276582"));
        }

        private void FavoritesItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            grid.Background = Brushes.Transparent;
        }

        private void FavoritesItem_MouseDown(object sender, MouseEventArgs e)
        {

            Grid grid = (Grid)sender;
            TextBlock textBlock = grid.Children[0] as TextBlock;

            RootObject root_from_server = sendRequestForecast(textBlock.Text);

            if(root_from_server != null)
            {
                root = root_from_server;
                ChangeDisplayData();
            }
            
        }

        private void DeleteFavoriteItem(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Grid grid = (Grid)btn.Parent;

            Grid child1 = grid.Children[0] as Grid;
            TextBlock textChild = child1.Children[0] as TextBlock;
            favoriteCities.RemoveCity(textChild.Text);

            FavoritesData.Children.Remove(grid);

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!searchTextBox.Text.Equals("Search..."))
            {
                RootObject root_from_server = sendRequestForecast(searchTextBox.Text);

                if (root_from_server != null)
                {
                    root = root_from_server;
                    ChangeDisplayData();
                }
                SearchSelectionWindow.Visibility = Visibility.Hidden;
            }
        }


        private void NotifyUser(string message)
        {
            if (!userNotificationTimer.IsEnabled)
            {
                DoubleAnimation da = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(0.55)),
                    AutoReverse = false
                };

                UserNotificationMessage.Text = message;
                UserNotificationMessage.BeginAnimation(OpacityProperty, da);
                userNotificationTimer.Tick += NotificationMessageTimeout;
                userNotificationTimer.Start();
            }
            else
            {
                userNotificationTimer.Stop();
                UserNotificationMessage.Text = message;
                userNotificationTimer.Tick += NotificationMessageTimeout;
                userNotificationTimer.Start();
            }
        }

        private void NotificationMessageTimeout(object sender, EventArgs e)
        {

            DoubleAnimation da = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.55)),
                AutoReverse = false
            };
            UserNotificationMessage.BeginAnimation(OpacityProperty, da);

            (sender as DispatcherTimer).Stop();
        }


        private void changeDayHourWeather(object sender, EventArgs e, TextBlock textBlock, bool flag)
        {
            updateGraphForSelectedDay(textBlock.Text, flag);
            ReloadWeatherByHours(graphTemperatures, graphTimeIntervals);
            dayDisplayedTextBlock.Text = $"Currenly showing data for {textBlock.Text}";
        }

        private void AnimateWeatherChangeByHour(TextBlock textBlock, bool flag)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                AutoReverse = false
            };
            da.Completed += (sender, e) => WeatherByHoursAnimationComplete(sender, e, textBlock, flag);

            WeatherGraphHolder.BeginAnimation(OpacityProperty, da);
            
        }

        private void WeatherByHoursAnimationComplete(object sender, EventArgs e, TextBlock textBlock, bool flag)
        {
            changeDayHourWeather(sender, e, textBlock, flag);

            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                AutoReverse = false
            };
            WeatherGraphHolder.BeginAnimation(OpacityProperty, da);
        }

        /* Event handlers for clicking on daily temperatures */

        private void Day1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateWeatherChangeByHour(day1TextBlock, false);
        }

        private void Day2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateWeatherChangeByHour(day2TextBlock, false);
        }

        private void Day3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateWeatherChangeByHour(day3TextBlock, false);
        }

        private void Day4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateWeatherChangeByHour(day4TextBlock, false);
        }

        private void Day5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateWeatherChangeByHour(day5TextBlock, true);
        }


        private void updateGraphForSelectedDay(string selectedDay, bool lastDay)
        {

            if(root == null)
            {
                return;
            }

            int counter = 0;

            int temperatureIndex = 0;
            int timeIntervalsIndex = 0;

            string[] timeTokens = root.list[0].dt_txt.Split(' ');
            string startTime = timeTokens[1];

            bool time_reached = false;

            for (int i = 0; i < root.list.Count; i++)
            {
                DateTime dayInIteration = DateTime.Parse(root.list[i].dt_txt);

                if(dayInIteration.DayOfWeek.ToString().Equals(selectedDay) && root.list[i].dt_txt.Contains(startTime))
                {
                    time_reached = true;
                }

                //midnight for the selected day reached
                if (time_reached)
                {

                    if(counter%2 == 0)
                    {
                        string[] tokens = root.list[i].dt_txt.Split(' ');
                        graphTimeIntervals[timeIntervalsIndex] = tokens[1].Substring(0, tokens[1].Length-3); //no need for seconds in format
                        timeIntervalsIndex++;
                    }

                    double currTemp = convertKelvinToCelsius(root.list[i].main.temp);
                    graphTemperatures[temperatureIndex] = (int)currTemp;

                    temperatureIndex++;
                    counter++;

                }

                if(counter > 7)
                {

                    //last day encounters index out of bounds exception, since it displays temps for 21 hours, not 24
                    if(!lastDay)
                    {
                        string[] tokens = root.list[++i].dt_txt.Split(' ');
                        graphTimeIntervals[timeIntervalsIndex] = tokens[1].Substring(0, tokens[1].Length - 3); //no need for seconds in format
                    }

                    break;
                }
            }

        }

        private void locationBtn_Click(object sender, RoutedEventArgs e)
        {
            loadDataForCurrentLocation();
        }
    }
}

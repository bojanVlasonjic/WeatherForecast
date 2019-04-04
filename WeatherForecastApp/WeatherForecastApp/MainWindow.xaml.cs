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

            sendRequest();
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


        /* Main window on close method */
        public void MainWindow_Closed(object sender, EventArgs e)
        {

            //Dispose once all HttpClient calls are complete
            client.Dispose();

        }


        /* Method used for sending a request to server */

        public void sendRequest()
        {
            //postavi naziv grada u rest_request objektu, razmake odvoji sa '+'
            rest_request.CityName = "Novi+Sad";

            //posalji zahtev, kao parametar prosledi klijenta
            rest_request.sendRequestToOpenWeather(client);
        }
    }
}

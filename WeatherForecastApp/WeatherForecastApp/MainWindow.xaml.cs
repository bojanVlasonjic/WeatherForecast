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
    }
}

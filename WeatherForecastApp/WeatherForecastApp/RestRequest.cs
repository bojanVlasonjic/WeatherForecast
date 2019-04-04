using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WeatherForecastApp
{
    public class RestRequest
    {
        // when sending a request -> url + ?q= + cityName + api_key

        private const string URL = "https://api.openweathermap.org/data/2.5/weather"; //base address
        private const string API_KEY = "&appid=7e2ee9421fe5a28a316416a3b37483ef";

        public string CityName{ get; set; }

        public RestRequest()
        {
      
        }


        //send request method
        public bool sendRequestToOpenWeather(HttpClient client)
        {

            if(CityName == null)
            {
                return false;
            }

            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync("?q=" + CityName + API_KEY).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                //var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                var data = response.Content.ReadAsStringAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}

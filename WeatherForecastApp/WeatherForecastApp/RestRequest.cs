using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

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
        public RestResponse sendRequestToOpenWeather(HttpClient client)
        {

            RestResponse restResponse = new RestResponse();

            if(CityName == null)
            {
                restResponse.Message = "City name is undefined";
                return restResponse;
            }

            //if it's the first time submitting a request
            if(client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(URL);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            }
            
            HttpResponseMessage response;
            //send request and get the response
            try
            {
                response = client.GetAsync("?q=" + CityName + API_KEY).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

            }
            //if there is no internet connection the exception below e occurs
            catch (AggregateException)
            {
                restResponse.Message = "No internet connection";
                return restResponse;
            }


            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                RootObject parsedData = response.Content.ReadAsAsync<RootObject>().Result;
                //var data = response.Content.ReadAsStringAsync();
                restResponse.Message = "Success";
                restResponse.Root = parsedData;
            }
            else
            {
                restResponse.Message = $"Can't find city with name {CityName}";
            }

            return restResponse;

        }

    }
}

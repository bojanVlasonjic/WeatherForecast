﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace WeatherForecastApp
{
    public class RestRequest
    {
        // when sending a request -> url + ?q= + cityName + api_key

        private const string URL_forecast = "https://api.openweathermap.org/data/2.5/forecast"; //base address
        private const string URL_hourly = "https://api.openweathermap.org/data/2.5/forecast/hourly";
        private const string API_KEY = "&appid=7e2ee9421fe5a28a316416a3b37483ef";

        private const string _LOCATION_URL = "http://api.ipstack.com/check";
        private const string _LOCATION_API_KEY = "?access_key=7446218c54fedb473fd5370638b70535";

        public string CityName { get; set; }

        public string URL_Forecast
        {
            get { return URL_forecast; }
        }

        public string URL_Hourly
        {
            get { return URL_hourly; }
        }

        public string LOCATION_URL
        {
            get { return _LOCATION_URL; }
        }

        public RestRequest()
        {
      
        }


        //send request method
        public RestResponse sendRequestToOpenWeather(HttpClient clientt, string URL)
        {
            RestResponse restResponse = new RestResponse();

            using (HttpClient client = new HttpClient())
            {
                if (CityName == null)
                {
                    restResponse.Message = "City name is undefined";
                    return restResponse;
                }

                //if it's the first time submitting a request
                if (client.BaseAddress == null)
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
                    RootObject parsedData = null;
                    try
                    {
                        parsedData = response.Content.ReadAsAsync<RootObject>().Result; //parsing json into RootObject

                    }
                    catch (Exception)
                    {
                        restResponse.Message = "Something went wrong. Please try again";
                        return restResponse;
                    }

                    restResponse.Message = "Success";
                    restResponse.Root = parsedData;
                }
                else
                {
                    restResponse.Message = $"Can't find place with name {CityName}";
                }
            }
            

            return restResponse;

        }

        public LocationRestResponse sendRequestToIPStack(HttpClient clientt, string URL)
        {

            LocationRestResponse restResponse = new LocationRestResponse();

            using (HttpClient client = new HttpClient())
            {

                //if it's the first time submitting a request
                if (client.BaseAddress == null)
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
                    response = client.GetAsync(_LOCATION_API_KEY).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

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
                    LocationData parsedData = null;
                    try
                    {
                        parsedData = response.Content.ReadAsAsync<LocationData>().Result; //parsing json into RootObject

                    }
                    catch (Exception)
                    {
                        restResponse.Message = "Something went wrong. Please try again";
                        return restResponse;
                    }

                    restResponse.Message = "Success";
                    restResponse.Root = parsedData;
                }
                else
                {
                    restResponse.Message = $"Can't find place with name {CityName}";
                }
            }

            return restResponse;

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    public class LocationRestResponse
    {
        public LocationRestResponse()
        {

        }

        public string Message { get; set; } //message output depending on the response

        public LocationData Root { get; set; } //parsed json response
    }
}

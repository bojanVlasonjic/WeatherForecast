using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    public class RestResponse
    {
        
        public RestResponse()
        {
         
        }

        public string Message { get; set; } //message output depending on the response

        public RootObject Root { get; set; } //parsed json response

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherForecastApp
{
    public class Rain
    {
        [JsonProperty("3h")]
        public double rain_3h { get; set; }
    }
}

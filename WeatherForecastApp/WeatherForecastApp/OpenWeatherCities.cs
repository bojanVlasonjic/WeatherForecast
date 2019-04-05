using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherForecastApp
{
    public class OpenWeatherCities
    {

        private List<City> cities;
        private String path;

        public OpenWeatherCities()
        {

            this.path = @"data/city.list.json";
            

            this.cities = JsonConvert.DeserializeObject<List<City>>(File.ReadAllText(this.path)) ;

        }

        public List<City> Cities
        {
            get
            {
                return cities;
            }
            set
            {
                cities = value;
            }
        }


    }
}

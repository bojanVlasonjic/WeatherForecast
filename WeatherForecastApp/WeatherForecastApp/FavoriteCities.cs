using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    class FavoriteCities
    {

        private List<string> _cities;
        private BinaryFormatter formatter = new BinaryFormatter();
        private const string path = "data/files/favorites.fav";


        public List<string> Cities
        {
            get
            {
                return _cities;
            }
            set
            {
                _cities = value;
            }
        }


        public FavoriteCities() {

            if (!Directory.Exists("data/files"))
            {
                Directory.CreateDirectory("data/files");
            }

            if (File.Exists(path))
            {
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                _cities = (List<string>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                _cities = new List<string>();
            }
            
            
        }


        public bool CityExists(string cityName)
        {
            foreach (string city in _cities)
            {
                if (city == cityName)
                {
                    return true;
                }
            }


            return false;
        }


        public void AddCity(string cityName)
        {
            _cities.Add(cityName);
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, _cities);
            stream.Close();
        }


        public void RemoveCity(string cityName)
        {
            _cities.Remove(cityName);
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, _cities);
            stream.Close();
        }


    }
}

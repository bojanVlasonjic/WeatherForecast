using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    [Serializable]
    public class SerializableRootObject
    {

        public SerializableRootObject()
        {

        }

        public RootObject Root_Object { get; set; }
    }
}

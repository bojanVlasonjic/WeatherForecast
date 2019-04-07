using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WeatherForecastApp
{
    public class BinaryIO
    {
        public static bool WriteToBinaryFile(string filePath, SerializableRootObject objectToWrite)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, objectToWrite);
                    return true;
                }
            } catch(Exception)
            {
                return false;
            }
            
        }

        
        public static SerializableRootObject ReadFromBinaryFile(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (SerializableRootObject)binaryFormatter.Deserialize(stream);
                }
            } catch(Exception)
            {
                return null;
            }
            
        }
    }
}

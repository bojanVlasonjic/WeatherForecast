using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WeatherForecastApp
{
    public class RootObjectIO
    {
        const string directory_path = "data/files";
        const string file_path = "/last_root_object.txt";

        public static bool WriteToFile(RootObject root)
        {

            if (!Directory.Exists(directory_path))
            {
                Directory.CreateDirectory(directory_path);
            }

            try
            {
                using (StreamWriter file = File.CreateText(directory_path + file_path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, root);
                    return true;
                }
            } catch(Exception)
            {
                return false;
            }
            
        }

        
        public static RootObject ReadRootFromFile()
        {
            try
            {
                using (StreamReader r = new StreamReader(directory_path + file_path))
                {
                    string json = r.ReadToEnd();
                    RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                    return root;
                }

            } catch(Exception)
            {
                return null;
            }
            
        }
    }
}

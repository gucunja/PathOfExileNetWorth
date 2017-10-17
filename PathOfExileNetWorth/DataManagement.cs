using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace PathOfExileNetWorth
{
    class DataManagement
    {
        private static string getJSON(string apiAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiAddress);

            CookieContainer cookies = new CookieContainer();
            Cookie cookie = new Cookie("POESESSID", Properties.Settings.Default.POESESSID) { Domain = ".pathofexile.com" };
            cookies.Add(cookie);

            request.CookieContainer = cookies;
            WebResponse response = request.GetResponse(); 
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        public static T processJsonFromApi<T>(string apiAddress)
        {
            string json = getJSON(apiAddress);
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public static bool LeagueStartsWithSSF(League l)
        {
            return l.id.StartsWith("SSF");
        }

        public static T LoadJson<T>(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            }
        }

        public static void SaveJson<T>(T serializableObject, string filePath)
        {
            string json = JsonConvert.SerializeObject(serializableObject);
            File.WriteAllText(filePath, json);
        }

    }
}

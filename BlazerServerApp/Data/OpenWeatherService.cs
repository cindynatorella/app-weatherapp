using Newtonsoft.Json.Linq;

namespace BlazerServerApp.Data
{
    public class OpenWeatherService
    {
        HttpClientHandler handler = new HttpClientHandler();
        private const string URL = "http://api.openweathermap.org/data/2.5/";
        private bool hasRanOnce = false;
        private Dictionary<string, string> people = new Dictionary<string, string>();
        private static Dictionary<string, string> peeps = new Dictionary<string, string>(){
            {"Eva", "Paris,fr"},
            {"Cindy", "Colorado Springs,us"},
            {"Philippe", "Codognan,fr"},
            {"Jeff", "Vancouver,ca"},
            {"Gilles et Marie-Jose", "Montelimar,fr"}
        };

        private static Dictionary<string,JObject> resultsWeather = new Dictionary<string, JObject>();

        public async Task<JObject> GetWeatherInfo(string location)
        {
            string urlParameters = "weather?q="+ location + "&units=metric&APPID=bcbbea02d17407d9ccf1a4e494743777";
            JObject result = null;
            using (var client = new HttpClient(handler, false))
            {
                client.BaseAddress = new Uri(URL);
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;

                Console.WriteLine("address used: "+response.ToString());
                Console.WriteLine("address used: " + response.RequestMessage);
                if (response.IsSuccessStatusCode)
                {

                    //Console.WriteLine(response.ToString());
                    string product = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(product);
                    result = json;
                    //Console.WriteLine(result.ToString());
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                return result;
            }
            return null;
        }


        public async void InitTheDataForThePeople()
        {
            foreach(var person in peeps)
            {
               
                JObject temp = await GetWeatherInfo(person.Value);
                resultsWeather[person.Key] = temp;
            }
        }

        //http://api.openweathermap.org/data/2.5/weather?q=London,uk&APPID=bcbbea02d17407d9ccf1a4e494743777

        public String GetWeather(JObject json)
        {
            string result = null;

            result = (string)json["weather"][0]["main"];

            return result;
        }

        public String GetTemp(JObject json)
        {
            string result = null;

            result = (string)json["main"]["temp"];

            return result;
        }

        private string GetTemperature(JObject json)
        {
            return (string)json["main"]["temp"];
        }

        private string GetCountry(JObject json)
        {
            return (string)json["sys"]["country"];
        }

        private string GetCity(JObject json)
        {
            return (string)json["name"];
        }

        private string GetWeatherDescription(JObject json)
        {
            return (string)json["weather"][0]["main"];
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };     

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray());
        }

        private string GetTime(JObject json)
        {
            long timezone = (long)json["timezone"];
            return DateTime.UtcNow.AddSeconds(timezone).ToShortTimeString();
        }

        public Task<DataEntry[]> GetData()
        {
            if (!hasRanOnce)
            {
                hasRanOnce = true;
                foreach(var item in peeps)
                {
                    people.Add(item.Key,item.Value);
                }
            }
            //console.log(new Date(obj.dt*1000+(obj.timezone*1000)))
            return Task.FromResult(resultsWeather.Select(item => new DataEntry {
                Date = GetTime(item.Value),
                TemperatureC = GetTemperature(item.Value),
                Summary = GetWeatherDescription(item.Value),
                Name = item.Key,
                Location = GetCity(item.Value)
            }).ToArray());


        }

    }
}
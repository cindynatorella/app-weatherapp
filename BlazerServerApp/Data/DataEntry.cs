namespace BlazerServerApp.Data
{
    public class DataEntry
    {
        public string Date { get; set; }

        public string TemperatureC { get; set; }

        //public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }
    }
}
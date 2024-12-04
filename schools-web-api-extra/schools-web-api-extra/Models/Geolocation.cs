namespace schools_web_api_extra.Models
{
    public class Geolocation
    {
        public double Latitude { get; set; }

        // Используемое свойство для десериализации/сериализации
        public double Longitude { get; set; }
    }
}

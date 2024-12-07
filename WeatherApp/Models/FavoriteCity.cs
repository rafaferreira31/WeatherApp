using SQLite;

namespace WeatherApp.Models
{
    public class FavoriteCity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CityId { get; set; }

        public string? Name { get; set; }

        public bool IsFavourite { get; set; }

        [Ignore]
        public string Temperature { get; set; }

        [Ignore]
        public string WeatherIcon { get; set; }
    }
}

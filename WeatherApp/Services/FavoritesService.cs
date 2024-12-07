using SQLite;
using WeatherApp.Models;

namespace WeatherApp.Services
{

    public class FavoritesService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritesService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favorites.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<FavoriteCity>().Wait();
        }

        public async Task<FavoriteCity> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<FavoriteCity>().Where(p => p.CityId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FavoriteCity>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<FavoriteCity>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateAsync(FavoriteCity favouriteCity)
        {
            try
            {
                await _database.InsertAsync(favouriteCity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(FavoriteCity favouriteCity)
        {
            try
            {
                await _database.DeleteAsync(favouriteCity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
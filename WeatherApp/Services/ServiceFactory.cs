using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Services
{
    public static class ServiceFactory
    {
        public static FavoritesService CreateFavouritesService()
        {
            return new FavoritesService();
        }
    }
}

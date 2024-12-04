using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class ProfileImage
    {
        public string? UrlImagem { get; set; }

        public string? CaminhoImagem => "https://nmbd2wm8-7066.uks1.devtunnels.ms/" + UrlImagem;
    }
}

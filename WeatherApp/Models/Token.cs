using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class Token
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int? UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }

        public string? UsuarioEmail { get; set; }

        public string? UsuarioFone { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Emuns;

namespace minimal_api.Dominio.ModelView
{
    public class AdmLogado
    {
       
       public string Email { get; set; } = default!;
       public string Perfil { get; set; } = default!;
           public string Token { get; set; } = default!;
    }
}
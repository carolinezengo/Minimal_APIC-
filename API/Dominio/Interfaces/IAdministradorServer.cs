using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;



namespace minimal_api.Servicos
{
    public interface IAdministradorServer
    {
          Administrador? BuscarPorId (int id);
        List<Administrador> Todos(int? pagina = 1, string? email=null, string? senha=null, string? perfil=null);
        Administrador? Login(LoginDTO loginDto);
       Administrador Incluir(Administrador administrador);
    }
}
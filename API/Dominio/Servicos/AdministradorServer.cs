using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.DB;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;



namespace minimal_api.Servicos
{
  public class AdministradorServer : IAdministradorServer
    {
        private readonly DbContexto _contexto;
        public AdministradorServer(DbContexto contexto)
        {
           _contexto = contexto;
        }

        public Administrador? BuscarPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
        }

        public Administrador Incluir(Administrador administrador)
        {
             _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }

        public Administrador? Login(LoginDTO loginDto)
        {
             var adm = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha).FirstOrDefault();
          
            return adm;
          
        }

        public List<Administrador> Todos(int? pagina = 1, string? email = null, string? senha = null, string? perfil = null)
        {
         var query = _contexto.Administradores.AsQueryable();
            if(!string.IsNullOrEmpty(email))
            {
                query = query.Where(v => EF.Functions.Like(v.Email.ToLower(), $"%{email}".ToLower())); 
            }
            int itensPorPaginas =10;


            if(pagina != null)

            {
              query = query.Skip(((int)pagina -1) * itensPorPaginas).Take(itensPorPaginas);
                     }
            return query.ToList();
        }

             
    }
}
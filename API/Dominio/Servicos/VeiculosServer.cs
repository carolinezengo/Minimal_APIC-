using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.DB;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.DTOs;


namespace minimal_api.Servicos
{
    public class VeiculosServer : IVeiculosServer
    {
        private readonly DbContexto _contexto;
        public VeiculosServer(DbContexto contexto)
        {
           _contexto = contexto;
        }

        public void Apagar(Veiculo veiculos)
        {
            _contexto.Veiculos.Remove(veiculos);
              _contexto.SaveChanges();

        }

        public void Atualizar(Veiculo veiculos)
        {
         _contexto.Veiculos.Update(veiculos);
            _contexto.SaveChanges();  
        }

        public Veiculo? BuscarPorId(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public Veiculo? BuscarPorMarca(string marca)
        {
            return _contexto.Veiculos.Where(v => v.Marca == marca).FirstOrDefault();
        }

        public Veiculo? BuscarPorNome(string nome)
        {
            return _contexto.Veiculos.Where(v => v.Nome == nome).FirstOrDefault();
        }

        public void Incluir(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();

        }

        public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _contexto.Veiculos.AsQueryable();
            if(!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}".ToLower())); 
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
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minimal_api.DB;

using minimal_api.Dominio.Emuns;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.ModelView;
using minimal_api.Dominio.ModelVIew;
using minimal_api.Dominio.DTOs;
using minimal_api.Servicos;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;


#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key= "123456";


builder.Services.AddAuthentication(o => {
  o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => {
  o.TokenValidationParameters = new TokenValidationParameters{
    ValidateLifetime = true,
    IssuerSigningKey = new  SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});
builder.Services.AddAuthorization();



builder.Services.AddScoped<IAdministradorServer, AdministradorServer>();
builder.Services.AddScoped<IVeiculosServer, VeiculosServer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
   Name ="Authorization",
   Type = SecuritySchemeType.Http,
   Scheme = "bearer",
   BearerFormat= "JWT",
   In = ParameterLocation.Header,
   Description=" Insira o token JWT aqui:"


  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
    new OpenApiSecurityScheme{
      Reference = new OpenApiReference{
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer"
      }
    
    },
      new string [] {}
    }
  });
});



 builder.Services.AddDbContext<DbContexto>(options =>
 { options.UseMySql(
   builder.Configuration.GetConnectionString("mysql"), 
   ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
   );
 
 });
var app = builder.Build();
   
#endregion
#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");   
#endregion

#region Veiculos
 ErroDeValidacao validacaoDTO( VeiculoDTO veiculosDto)
  {
     var validacao = new ErroDeValidacao();
     validacao.Mensagens = new List<string>();
     
          if(string.IsNullOrEmpty(veiculosDto.Nome))
       validacao.Mensagens.Add("O nome não pode ser vazio");

        if(string.IsNullOrEmpty(veiculosDto.Marca))
       validacao.Mensagens.Add("O marca não pode ser vazio");

        if((veiculosDto.Ano < 1950))
       validacao.Mensagens.Add("Carro muito antigo. Digitar so acima do ano  1950");
    return validacao;

  }
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculosDto, IVeiculosServer veiculosServer) => 
{
  var validacao =  validacaoDTO(veiculosDto) ;
  if(validacao.Mensagens.Count > 0)
    return Results.BadRequest(validacao);
  
   Veiculo veiculo = new Veiculo();
 veiculo.Nome = veiculosDto.Nome;
 veiculo.Marca =veiculosDto.Marca;
 veiculo.Ano = veiculosDto.Ano; 
   
   veiculosServer.Incluir(veiculo);
   return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).RequireAuthorization()
 .RequireAuthorization( new AuthorizeAttribute { Roles= "Adm"})
 .WithTags("Veiculos");
app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculosServer veiculosServer) => 
{
    var veiculos = veiculosServer.Todos(pagina);
    return Results.Ok(veiculos);
       
} ).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"})
.WithTags("Veiculos"); 

app.MapGet("/veiculos/{Id}", ([FromQuery]int id, IVeiculosServer veiculosServer) => 
{
    var veiculos =veiculosServer.BuscarPorId(id);
    return Results.Ok(veiculos);
       
}).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"})
.WithTags("Veiculos"); 
app.MapGet("/veiculos/{nome}", ([FromQuery]string nome, IVeiculosServer veiculosServer) => 
{
    var veiculos =veiculosServer.BuscarPorNome(nome);
    return Results.Ok(veiculos);
       
} ).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos/{marca}", ([FromQuery]string marca, IVeiculosServer veiculosServer) => 
{
    var veiculos =veiculosServer.BuscarPorMarca(marca);
    return Results.Ok(veiculos);
       
} ).RequireAuthorization()

.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculosDto, IVeiculosServer veiculosServer) => 
{
  var veiculo = veiculosServer.BuscarPorId(id);
  if(veiculo == null) return Results.NotFound();


  var validacao =  validacaoDTO(veiculosDto) ;
  if(validacao.Mensagens.Count > 0)
    return Results.BadRequest(validacao);

  veiculo.Nome = veiculosDto.Nome;
  veiculo.Marca =veiculosDto.Marca;
  veiculo.Ano = veiculosDto.Ano; 
  veiculosServer.Atualizar(veiculo);
   return Results.Ok(veiculo);


}).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm"})
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute]int id, IVeiculosServer veiculosServer) => 
{
  var veiculo = veiculosServer.BuscarPorId(id);
   if(veiculo == null) return Results.NotFound();

    veiculosServer.Apagar(veiculo);
    return Results.NoContent();
       
}).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm"})
.WithTags("Veiculos");

#endregion

#region Administradores

string GerarTokenJwt(Administrador administrador)
{
   if(string.IsNullOrEmpty(key)) return string.Empty;

  var securityKey = new  SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
  var creddentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var claims = new List<Claim>()
{
  new Claim("Email", administrador.Email),
  new Claim("Perfil", administrador.Perfil.ToString()),
  new Claim(ClaimTypes.Role, administrador.Perfil.ToString()),

};


  var token = new JwtSecurityToken(
    claims:claims,
    expires: DateTime.Now.AddDays(1),
    signingCredentials: creddentials

  );
  
  return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrador/login", ([FromBody] LoginDTO loginDto, IAdministradorServer administradorServer) => 
{ 
  var adm = administradorServer.Login(loginDto);

    if(adm!= null)
    {    
    string token = GerarTokenJwt(adm);
    
    return Results.Ok(new AdmLogado
    {
      Email = adm.Email,
      Perfil = adm.Perfil.ToString(),
      Token = token
    });
    
    
    }else
    return Results.Unauthorized();
       
}).AllowAnonymous().WithTags("Administrador"); 

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServer administradorServer) => {
   
    var validacao = new ErroDeValidacao();
     validacao.Mensagens = new List<string>();
     
          if(string.IsNullOrEmpty(administradorDTO.Email))
       validacao.Mensagens.Add("O Email não pode ser vazio");


        if(string.IsNullOrEmpty(administradorDTO.Senha))
       validacao.Mensagens.Add("A senha não pode ser vazia");

       if(administradorDTO.Perfil == null)
       validacao.Mensagens.Add("O perfil não pode ser vazio");


  if(validacao.Mensagens.Count > 0)
    return Results.BadRequest(validacao);

  var administrador = new Administrador();
 administrador.Email = administradorDTO.Email;
 administrador.Senha =administradorDTO.Senha;
 administrador.Perfil = administradorDTO.Perfil.ToString() ?? "Editor" ;

   
   administradorServer.Incluir(administrador);

    AdministradorModelView admin = new AdministradorModelView();
   return Results.Created($"/administrador/{administrador.Id}",(

       admin.Id  = administrador.Id,
       admin.Email = administrador.Email,
      admin.Perfil = administrador.Perfil
      ));


}).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm"}).WithTags("Administrador");

app.MapGet("/administradores", ([FromQuery]int? pagina, IAdministradorServer administradorServer) => 
{   var adms = new List<AdministradorModelView>();
   var administradores = administradorServer.Todos(pagina);
   foreach(var adm in administradores)
   {
    adms.Add(new AdministradorModelView{
      Id = adm.Id,
      Email = adm.Email,
      Perfil = adm.Perfil.ToString()
    });
   }
   return Results.Ok(administradores);
       
} ).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"})
.WithTags("Administrador"); 

app.MapGet("/administradores/{Id}", ([FromQuery]int id, IAdministradorServer administradorServer) => 
{
    var administrador =administradorServer.BuscarPorId(id);
    return Results.Ok(administrador);
       
} ).RequireAuthorization()
.RequireAuthorization( new AuthorizeAttribute { Roles= "Adm, Editor"}).WithTags("Administrador"); 

#endregion
   
   #region App
   app.UseSwagger();
   app.UseSwaggerUI();

   app.UseAuthentication();
   app.UseAuthorization();

   app.Run(); 
   #endregion
  

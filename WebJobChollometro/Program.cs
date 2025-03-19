using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebJobChollometro.Data;
using WebJobChollometro.Repositories;

//Console.WriteLine("Bienvenido a nuestros Chollos!!!");
string connectionString = "Data Source=techriders.database.windows.net;Initial Catalog=PRODUCCION;Persist Security Info=True;User ID=adminsql;Password=Admin123;Encrypt=True;Trust Server Certificate=True";

//NECESITAMOS UTILIZAR INYECCION DE DEPENDENCIAS PARA PODER 
//CREAR LOS OBJETOS.
var provider = new ServiceCollection()
    .AddTransient<RepositoryChollometro>()
    .AddDbContext<ChollometroContext>
    (options => options.UseSqlServer(connectionString))
    .BuildServiceProvider();
//DESDE ESTE CODIGO NECESITAMOS RECUPERAR EL REPO INYECTADO
RepositoryChollometro repo =
    provider.GetService<RepositoryChollometro>();
//Console.WriteLine("Pulse Enter para Iniciar");
//Console.ReadLine();
await repo.PopulateChollosAzureAsync();
//Console.WriteLine("Proceso completado correctamente");
//Console.WriteLine("Enhorabuena!!!");

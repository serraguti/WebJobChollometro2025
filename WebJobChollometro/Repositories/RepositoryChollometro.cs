using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebJobChollometro.Data;
using WebJobChollometro.Models;

namespace WebJobChollometro.Repositories
{
    public class RepositoryChollometro
    {
        private ChollometroContext context;

        public RepositoryChollometro(ChollometroContext context)
        {
            this.context = context;
        }

        //LOS CHOLLOS LOS VAMOS A EXTRAER DE LA WEB
        //RECUPERAREMOS TODOS LO QUE TENGA DENTRO DEL RSS
        //IREMOS INCREMENTANDO CADA CHOLLO MEDIANTE UN MAX
        public async Task<int> GetMaxIdCholloAsync()
        {
            if (this.context.Chollos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await
                    this.context.Chollos.MaxAsync(x => x.IdChollo) + 1;
            }
        }

        //METODO PARA LEER LOS CHOLLOS DE LA WEB
        public async Task<List<Chollo>> GetChollosWebAsync()
        {
            //@"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)"
            string url = "https://www.chollometro.com/rss";
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);
            request.Accept = @"text/html application/xhtml+xml, *.*";
            request.Host = "www.chollometro.com";
            request.Headers.Add("Accept-language", "es-ES");
            request.Referer = "https://www.chollometro.com";
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            HttpWebResponse response = (HttpWebResponse)
                await request.GetResponseAsync();
            //ESTE TIPO DE PETICION SE PUEDE UTILIZAR PARA TODO, 
            //PODEMOS LEER UN VIDEO, UNA IMAGEN O SIMPLE TEXTO
            //LA INFORMACION NOS LA DEVUELVE EN STREAM
            //DEBEMOS CONVERTIR EL STREAM EN TEXTO (xml)
            string xmlData = "";
            using (StreamReader reader = 
                new StreamReader(response.GetResponseStream()))
            {
                xmlData = await reader.ReadToEndAsync();
            }
            //TENEMOS UN XML QUE PODEMOS LEER CON LINQ
            XDocument document = XDocument.Parse(xmlData);
            var consulta = from datos in document.Descendants("item")
                           select datos;
            List<Chollo> chollosWeb = new List<Chollo>();
            int idChollo = await this.GetMaxIdCholloAsync();
            foreach (var tag in consulta)
            {
                Chollo chollo = new Chollo();
                chollo.IdChollo = idChollo;
                chollo.Titulo = tag.Element("title").Value;
                chollo.Descripcion = tag.Element("description").Value;
                chollo.Link = tag.Element("link").Value;
                chollo.Fecha = DateTime.Now;
                idChollo += 1;
                chollosWeb.Add(chollo);
            }
            return chollosWeb;
        }

        //CREAMOS UN METODO PARA AGREGAR LOS CHOLLOS DE LA NUBE 
        //DENTRO DE LA BASE DE DATOS.
        public async Task PopulateChollosAzureAsync()
        {
            List<Chollo> chollos = await this.GetChollosWebAsync();
            foreach (Chollo c in chollos)
            {
                //LOS AGREGAMOS AL CONTEXT
                this.context.Chollos.Add(c);
            }
            await this.context.SaveChangesAsync();
        }
    }
}

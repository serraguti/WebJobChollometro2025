﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcNetCoreChollometro.Models
{
    [Table("CHOLLOS")]
    public class Chollo
    {
        [Key]
        [Column("IDCHOLLO")]
        public int IdChollo { get; set; }
        [Column("TITULO")]
        public string Titulo { get; set; }
        [Column("LINK")]
        public string Link { get; set; }
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("FECHA")]
        public DateTime Fecha { get; set; }
    }
}

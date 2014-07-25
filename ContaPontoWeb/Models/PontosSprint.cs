using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace ContaPontoWeb.Models
{
    public class PontosSprint
    {
        public PontosSprint(string json)
        {
            var jsonObject = Json.Decode(json);
            Desenvolvedor = (string)jsonObject.desenvolvedor;
            Os = (string)jsonObject.os;
            Sprint = (string)jsonObject.sprint;
            Semana = (string)jsonObject.semana;
            Pontos = (string)jsonObject.pontos;
            Testes = (string)jsonObject.testes;
        }

        public String Desenvolvedor { get; set; }
        public String Os { get; set; }
        public String Sprint { get; set; }
        public String Semana { get; set; }
        public String Pontos { get; set; }
        public String Testes { get; set; }
    }
}
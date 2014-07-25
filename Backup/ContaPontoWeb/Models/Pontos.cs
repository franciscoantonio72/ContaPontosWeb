using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContaPontoWeb.Models
{
    public class Pontos
    {
        public int TotalPontos { get; set; }
        public int TotalTestes { get; set; }
        public int PontosDesenvolvido { get; set; }
        public String Programador { get; set; }
        public double Complexidade { get; set; }
        public int NaoTestado { get; set; }
        public int Testado { get; set; }
        public int TotalQuebra { get; set; }

        public int PontosNaoTestados { get; set; }
        public int PontosQuebra { get; set; }
        public int TestesUnitarios { get; set; }
        public int MetaIndividual { get; set; }
        public int PontosDesenvolvidos { get; set; }

        public string Os { get; set; }
        public double QtdePontos { get; set; }
        public double QtdeTestes { get; set; }
        public string Situacao { get; set; }
        public string Imagem { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContaPontoWeb.Models
{
    public enum TipoStatusResposta
    {
        SUCESSO,
        ERRO
    }

    public class RespostaJsonWebApi
    {
        public string StatusResposta { get; private set; }

        public void SetStatusResposta(TipoStatusResposta tipoStatusResposta)
        {
            if (tipoStatusResposta == TipoStatusResposta.SUCESSO)
                this.StatusResposta = "SUCESSO";
            else
                this.StatusResposta = "ERRO";
        }

        public string Mensagem { get; set; }
        public Object Dados { get; set; }
    }
}
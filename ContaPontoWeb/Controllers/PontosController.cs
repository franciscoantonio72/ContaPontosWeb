using ContaPontoWeb.Dao;
using ContaPontoWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ContaPontoWeb.Controllers
{
    public class PontosController : Controller
    {
        DaoPontos dao = new DaoPontos();

        public ActionResult Index()
        {
            var totalTestado = 0;
            var totalDesenvolvido = 0;
            var numeroTestes = 0;

            var sprint = RetornarSprintDaSemana();
            var pontosETestes = RetornarPontosETestes2(sprint[0], sprint[1]);

            foreach (DataRow item in pontosETestes.Rows)
            {
                totalDesenvolvido = totalDesenvolvido + Convert.ToInt32(item["qtde_pontos"]);
                if (VerificaSeFoiTestado(item["os"].ToString()))
                    totalTestado = totalTestado + Convert.ToInt32(item["qtde_pontos"]);
                numeroTestes = numeroTestes + Convert.ToInt32(item["qtde_teste"]);
            }

            ViewBag.TotalTestado = totalTestado;
            ViewBag.TotalDesenvolvido = totalDesenvolvido;
            ViewBag.NumeroCartoes = pontosETestes.Rows.Count;
            ViewBag.NumeroTestes = numeroTestes;

            var listaSprint = dao.RetornarListaSprint();
            List<string> listaSprintSemana = new List<string>();
            foreach (DataRow item in listaSprint.Rows)
            {
                listaSprintSemana.Add(item["Sprint"].ToString());
            }

            var listaSemana = dao.RetornarListaSemana(sprint[0]);
            List<string> listarSprint = new List<string>();
            listarSprint.Add("Todas");
            foreach (DataRow item in listaSemana.Rows)
            {
                listarSprint.Add(item["SemanaSprint"].ToString());
            }

            ViewBag.ListaSprint = new SelectList(listaSprintSemana, "Sprint");
            ViewBag.ListaSemana = new SelectList(listarSprint, sprint[1]);

            // refatorar para retirar daqui de dentro
            var pontos = RetornarPontos2(sprint[0], sprint[1], "Produção");

            List<Pontos> listaPontos = new List<Pontos>();

            foreach (DataRow item in pontos.Rows)
            {
                Pontos teste = new Pontos();
                teste.Complexidade = Convert.ToDouble(item["Complexidade"]);
                teste.MetaIndividual = Convert.ToInt32(item["Testado"]);
                teste.PontosDesenvolvidos = Convert.ToInt32(item["Total"]);
                teste.PontosNaoTestados = Convert.ToInt32(item["NaoTestado"]);
                teste.PontosQuebra = Convert.ToInt32(item["TotalQuebra"]);
                teste.Programador = item["Programador"].ToString();
                teste.TestesUnitarios = Convert.ToInt32(item["TotalTestes"]);
                if (teste.MetaIndividual <= 32)
                    teste.Imagem = "../../Content/Imagens/NaoFolga.png";
                if (((teste.MetaIndividual >= 33 && teste.MetaIndividual <= 45) && (teste.TestesUnitarios < 25)) ||
                   (teste.MetaIndividual >= 46 && teste.TestesUnitarios < 33))
                    teste.Imagem = "../../Content/Imagens/QuaseFolga.png";
                if (teste.MetaIndividual >= 46 && teste.TestesUnitarios >= 25)
                    teste.Imagem = "../../Content/Imagens/Folga.png";

                listaPontos.Add(teste);
            }

            return View(listaPontos);
        }

        [HttpPost]
        public ActionResult Index(string texto)
        {
            string sprintSelecionado;
            string semanaSelecionado;
            if (texto.Length == 4)
            {
                sprintSelecionado = texto.Substring(0, 2);
                semanaSelecionado = texto.Substring(3, 1);
            }
            else
            {
                sprintSelecionado = texto.Substring(0, 3);
                semanaSelecionado = texto.Substring(4, 1);
            }
            if (texto.Substring(4, 1) == "T")
                semanaSelecionado = "0";
            var totalTestado = 0;
            var totalDesenvolvido = 0;
            var numeroTestes = 0;

            var pontosETestes = RetornarPontosETestes2(sprintSelecionado, semanaSelecionado);

            foreach (DataRow item in pontosETestes.Rows)
            {
                totalDesenvolvido = totalDesenvolvido + Convert.ToInt32(item["qtde_pontos"]);
                if (VerificaSeFoiTestado(item["os"].ToString()))
                    totalTestado = totalTestado + Convert.ToInt32(item["qtde_pontos"]);
                numeroTestes = numeroTestes + Convert.ToInt32(item["qtde_teste"]);
            }

            ViewBag.TotalTestado = totalTestado;
            ViewBag.TotalDesenvolvido = totalDesenvolvido;
            ViewBag.NumeroCartoes = pontosETestes.Rows.Count;
            ViewBag.NumeroTestes = numeroTestes;

            var listaSprint = dao.RetornarListaSprint();
            List<string> listaSprintSemana = new List<string>();
            foreach (DataRow item in listaSprint.Rows)
            {
                listaSprintSemana.Add(item["Sprint"].ToString());
            }

            var listaSemana = dao.RetornarListaSemana(sprintSelecionado);
            List<string> listarSprint = new List<string>();
            listarSprint.Add("Todas");
            foreach (DataRow item in listaSemana.Rows)
            {
                listarSprint.Add(item["SemanaSprint"].ToString());
            }

            ViewBag.ListaSprint = new SelectList(listaSprintSemana, "Sprint");
            ViewBag.ListaSemana = new SelectList(listarSprint, semanaSelecionado);

            // refatorar para retirar daqui de dentro
            var pontos = RetornarPontos2(sprintSelecionado, semanaSelecionado, "Produção");

            List<Pontos> listaPontos = new List<Pontos>();

            foreach (DataRow item in pontos.Rows)
            {
                Pontos teste = new Pontos();
                teste.Complexidade = Convert.ToDouble(item["Complexidade"]);
                teste.MetaIndividual = Convert.ToInt32(item["Testado"]);
                teste.PontosDesenvolvidos = Convert.ToInt32(item["Total"]);
                teste.PontosNaoTestados = Convert.ToInt32(item["NaoTestado"]);
                teste.PontosQuebra = Convert.ToInt32(item["TotalQuebra"]);
                teste.Programador = item["Programador"].ToString();
                teste.TestesUnitarios = Convert.ToInt32(item["TotalTestes"]);
                if (teste.MetaIndividual <= 32)
                    teste.Imagem = "../../Content/Imagens/NaoFolga.png";
                if (((teste.MetaIndividual >= 33 && teste.MetaIndividual <= 45) && (teste.TestesUnitarios < 25)) ||
                   (teste.MetaIndividual >= 46 && teste.TestesUnitarios < 33))
                    teste.Imagem = "../../Content/Imagens/QuaseFolga.png";
                if (teste.MetaIndividual >= 46 && teste.TestesUnitarios >= 25)
                    teste.Imagem = "../../Content/Imagens/Folga.png";

                listaPontos.Add(teste);
            }

            return View(listaPontos);
        }

        [HttpGet]
        public JsonResult RetornarListaSprints()
        {
            var retorno = dao.RetornarListaSprint();

            var listaSprint = new List<object>();
            foreach (DataRow item in retorno.Rows)
            {
                var sprint = new
                {
                    numeroSprint = item[0].ToString()
                };
                listaSprint.Add(sprint);
            }

            return Json(listaSprint, "json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetornarListaDesenvolvedores(String sprint, string semana)
        {
            var retorno = dao.RetonarPontos(sprint, semana, "Produção");

            List<String> listaDevelopers = new List<string>();
            listaDevelopers.Add("-- Selecione Developer --");
            foreach (DataRow row in retorno.Rows)
            {
                listaDevelopers.Add(row[0].ToString().ToUpper());
            }

            return Json(listaDevelopers);
        }

        [HttpGet]
        public JsonResult getRetornarListaDesenvolvedores(String sprint, string semana)
        {
            var retorno = dao.RetonarPontos(sprint, semana, "Produção");

            //List<String> listaDevelopers = new List<string>();
            var listaDevelopers = new List<object>();
            //listaDevelopers.Add("-- Selecione Developer --");
            foreach (DataRow row in retorno.Rows)
            {
                var developers = new
                {
                    Nome = row[0].ToString().ToUpper(),
                    Ponto = row[4].ToString().ToUpper(),
                    Teste = row[1].ToString().ToUpper()
                };

                listaDevelopers.Add(developers);
            }

            return Json(listaDevelopers, "json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetornarTodosDesenvolvedores()
        {
            var retorno = dao.RetornarTodosDesenvolvedores();

            //List<String> listaDevelopers = new List<string>();
            List<String> listaDevelopers = retorno;
            //foreach (DataRow row in retorno.Rows)
            //{
            //    listaDevelopers.Add(row[0].ToString().ToUpper());
            //}

            return Json(listaDevelopers);

        }

        [HttpPost]
        public JsonResult Teste()
        {
            var retorno = dao.RetornarSprintDaSemana();
            var pontos = RetornarPontosETestes(retorno[0].ToString(), retorno[1].ToString());

            String[] lista = new String[] { pontos[0].ToString(), pontos[1].ToString() };
            return Json(lista);
        }

        [HttpPost]
        public JsonResult RetornarPontosDeveloper(String sprint, string semana, string developer)
        {
            var retorno = RetornarPontos(sprint, semana, developer);
            return Json(retorno);
        }

        [HttpPost]
        public JsonResult Conectar(string usuario, string senha)
        {
            Boolean conectado = false;
            conectado = dao.Conectar(usuario, senha);

            var respostaJson = new RespostaJsonWebApi() { Mensagem = "OK" };
            if (conectado)
            {
                respostaJson.SetStatusResposta(TipoStatusResposta.SUCESSO);
                return Json(respostaJson);
            }
            respostaJson = new RespostaJsonWebApi() { Mensagem = "ERRO" };
            respostaJson.SetStatusResposta(TipoStatusResposta.ERRO);
            return Json(respostaJson);
        }

        private Pontos RetornarPontos(String sprint, string semana, string developer)
        {
            var retorno = dao.RetonarPontos(sprint, semana, "Produção");
            string condicao = "Programador ='" + developer + "'";
            var linhaRetorno = retorno.Select(condicao);

            Pontos pontos = new Pontos();
            if (linhaRetorno.Count() > 0)
            {
                pontos.Programador = linhaRetorno[0].ItemArray[0].ToString() == null ? developer : linhaRetorno[0].ItemArray[0].ToString();
                pontos.TotalTestes = Convert.ToInt16((linhaRetorno[0].ItemArray[1].ToString() == null ? "0" : linhaRetorno[0].ItemArray[1].ToString()));
                pontos.NaoTestado = Convert.ToInt16((linhaRetorno[0].ItemArray[3].ToString() == null ? "0" : linhaRetorno[0].ItemArray[3].ToString()));
                pontos.Testado = Convert.ToInt16((linhaRetorno[0].ItemArray[4].ToString() == null ? "0" : linhaRetorno[0].ItemArray[4].ToString()));
                pontos.TotalPontos = Convert.ToInt16((linhaRetorno[0].ItemArray[5].ToString() == null ? "0" : linhaRetorno[0].ItemArray[5].ToString()));
                pontos.TotalQuebra = Convert.ToInt16((linhaRetorno[0].ItemArray[6].ToString() == null ? "0" : linhaRetorno[0].ItemArray[6].ToString()));
                return pontos;
            }
            else
            {
                return null;
            }
        }

        private DataTable RetornarPontos2(String sprint, string semana, string tipo)
        {
            return dao.RetonarPontos(sprint, semana, tipo);
        }

        private List<String> RetornarPontosETestes(String sprint, String semana)
        {
            var totalTestado = 0;
            var totalDesenvolvido = 0;
            var retorno = dao.RetornarPontosETestes(sprint, semana);
            List<String> pontos = new List<string>();
            foreach (DataRow item in retorno.Rows)
            {
                totalDesenvolvido = totalDesenvolvido + Convert.ToInt32(item["qtde_pontos"]);
                if (VerificaSeFoiTestado(item["os"].ToString()))
                    totalTestado = totalTestado + Convert.ToInt32(item["qtde_pontos"]);
            }
            pontos.Add(Convert.ToString(totalDesenvolvido));
            pontos.Add(Convert.ToString(totalTestado));

            return pontos;
        }

        private DataTable RetornarPontosETestes2(String sprint, String semana)
        {
            return dao.RetornarPontosETestes(sprint, semana);
        }

        private Boolean VerificaSeFoiTestado(string os)
        {
            return dao.VerificarSeFoiTestado(os);
        }

        [HttpPost]
        public JsonResult EnviarDados(string nome, string sprint, string semana)
        {
            var lsprint = sprint;
            var lsemana = semana;
            if (lsemana == "Todas")
                lsemana = "0";
            var lista = dao.RetornarDetalhes(nome, lsprint, lsemana);

            var list = new List<object>();
            foreach (DataRow item in lista.Rows)
            {
                var pontos = new
                {
                    Os = item["Os"].ToString(),
                    QtdePontos = Convert.ToDouble(item["Qtde_Pontos"].ToString()),
                    QtdeTestes = Convert.ToDouble(item["Qtde_Teste"].ToString()),
                    Situacao = item["Situacao"].ToString(),
                    Data = item["Data"].ToString()
                };
                list.Add(pontos);
            }
            return this.Json(list);
        }

        [HttpGet]
        public JsonResult getEnviarDados(string nome, string sprint, string semana)
        {
            var lsprint = sprint;
            var lsemana = semana;
            if (lsemana == "Todas")
                lsemana = "0";
            var lista = dao.RetornarDetalhes(nome, lsprint, lsemana);

            var list = new List<object>();
            foreach (DataRow item in lista.Rows)
            {
                var Os = new
                {
                    NumeroOs = item["Os"].ToString(),
                    QtdePontos = Convert.ToDouble(item["Qtde_Pontos"].ToString()),
                    QtdeTestes = Convert.ToDouble(item["Qtde_Teste"].ToString()),
                    Situacao = item["Situacao"].ToString(),
                    Data = Convert.ToDateTime(item["Data"].ToString())
                };
                list.Add(Os);
            }
            return Json(list, "json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        private string[] RetornarSprintDaSemana()
        {
            return dao.RetornarSprintDaSemana();
        }

        [HttpGet]
        public JsonResult getRetornarSprintDaSemana()
        {
            var lista = RetornarSprintDaSemana();

            for (int i = 0; i < lista.Count(); i++)
            {
                var Sprint = new
                {
                    NumeroSprint = lista[0],
                    NumeroSemana = lista[1]
                };
                return Json(Sprint, "json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public JsonResult RetornarTestes(string sprint, string semana)
        {
            var lsprint = sprint;
            var lsemana = semana;
            if (lsemana == "Todas")
                lsemana = "0";
            var pontos = RetornarPontos2(lsprint, lsemana, "Testes");
            List<Pontos> listaPontos = new List<Pontos>();

            foreach (DataRow item in pontos.Rows)
            {
                Pontos teste = new Pontos();
                teste.Complexidade = Convert.ToDouble(item["Complexidade"]);
                teste.PontosDesenvolvidos = Convert.ToInt32(item["Total"]);
                teste.Programador = item["Programador"].ToString();
                listaPontos.Add(teste);
            }
            return this.Json(listaPontos);
        }

        [HttpPost]
        public JsonResult GravarDados(string dados)
        {
            Boolean gravou = false;

            var json = dados;
            PontosSprint pontos = new PontosSprint(json);

            gravou = dao.InserirDados(pontos);

            var respostaJson = new RespostaJsonWebApi() { Mensagem = "OK" };
            if (gravou)
            {
                respostaJson.SetStatusResposta(TipoStatusResposta.SUCESSO);
                return Json(respostaJson);
            }
            respostaJson = new RespostaJsonWebApi() { Mensagem = "ERRO" };
            respostaJson.SetStatusResposta(TipoStatusResposta.ERRO);
            return Json(respostaJson);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using ContaPontoWeb.Models;

namespace ContaPontoWeb.Dao
{
    public class DaoPontos
    {
        //SqlConnection conn = new SqlConnection(@"Data Source=NoteWIN-Fco;Initial Catalog=Producao;Integrated Security=True");
        //SqlConnection conn = new SqlConnection(@"Data Source=NOTEWIN-FCO\SQLEXPRESS;Initial Catalog=Producao;Integrated Security=True");
        SqlConnection conn = new SqlConnection(@"Data Source=BDSERVER;Initial Catalog=Producao;User ID=tec-soft;Password=master");
        //SqlConnection conn = new SqlConnection(@"Server=dbabe69b-b3e6-4274-8805-a0a700f28793.sqlserver.sequelizer.com;Database=dbdbabe69bb3e642748805a0a700f28793;User ID=iywhlcjjsykjhjyv;Password=SzYgDVj5uufHzpwtxVxdPDyPJRcbJj3iNGNYK3qGW65haX4BJf8D75inbjDUShuj");

        private String RetornarFiltroSemanaSprint(String semana)
        {
            var filtro = "";
            if (semana == "Todas")
                semana = "0";

            if (Convert.ToInt32(semana) > 0)
                filtro = " And SemanaSprint = '" + semana + "' ";

            return filtro;
        }

        public DataTable RetornarPontosETestes(String sprint, String semana)
        {
            try
            {
                var selecao = "select os,max(qtde_pontos) as qtde_pontos, max(qtde_teste) as qtde_teste " +
                              "from pontos " +
                              "where sprint=@parsprint " + RetornarFiltroSemanaSprint(semana) + " and " +
                              "      tipo='produção' " +
                              "group by dupla,idnota,os ";
                SqlCommand comand = new SqlCommand(selecao, conn);
                comand.Parameters.AddWithValue("@parsprint", sprint);
                SqlDataAdapter daq = new SqlDataAdapter(comand);
                DataTable tabela = new DataTable();

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                daq.Fill(tabela);
                return tabela;
            }
            finally
            {
                conn.Close();
            }
        }

        public SqlDataReader sqlQuery()
        {
            try
            {
                conn.Open();
                SqlCommand cmdSprint = new SqlCommand("SELECT DISTINCT(Sprint) FROM Pontos ORDER BY SPRINT DESC", conn);
                SqlDataReader dr = cmdSprint.ExecuteReader();
                return dr;
            }
            finally
            {
                // conn.Close();
            }
        }

        public Boolean VerificarSeFoiTestado(String numeroOS)
        {
            try
            {
                Boolean testado = false;

                var selecao = "SELECT Testado FROM Pontos WHERE OS=@parOS AND Testado=@parTestado";
                SqlCommand comTestado = new SqlCommand(selecao, conn);
                comTestado.Parameters.AddWithValue("@parOs", numeroOS);
                comTestado.Parameters.AddWithValue("@parTestado", 1);
                SqlDataAdapter adapter = new SqlDataAdapter(comTestado);
                DataTable tabela = new DataTable();
                conn.Open();
                adapter.Fill(tabela);

                if (tabela.Rows.Count > 0)
                {
                    testado = true;
                }
                return testado;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable RetornarOS(String indicePontos, String programador)
        {
            var selecao = "SELECT * FROM Pontos " +
                          "WHERE Sprint=@parSprint AND " +
                          "      Programador=@parProgramador " +
                          "ORDER BY Programador";
            SqlCommand cmd = new SqlCommand(selecao, conn);
            cmd.Parameters.AddWithValue("@parSprint", indicePontos);
            cmd.Parameters.AddWithValue("@parProgramador", programador);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            conn.Open();
            DataTable tabela = new DataTable();
            da.Fill(tabela);

            return tabela;
        }

        public DataTable RetornarDetalhes(string programador, string sprint, string semana)
        {
            try
            {
                var selecao = "SELECT OS,Qtde_Pontos,Qtde_Teste, Case when Testado = 1 then 'Testado' else 'Não Testado' end as Situacao, Data from Pontos " +
                         "WHERE Sprint=@parSprint and Programador=@parProgramador " + RetornarFiltroSemanaSprint(semana);

                SqlCommand cmd = new SqlCommand(selecao, conn);
                cmd.Parameters.AddWithValue("@parSprint", sprint);
                cmd.Parameters.AddWithValue("@parProgramador", programador);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                DataTable tabela = new DataTable();
                da.Fill(tabela);

                return tabela;
            }
            finally
            {
                conn.Close();
            }

        }

        public List<String> RetornarTodosDesenvolvedores()
        {
            try
            {
                //var selecao = "SELECT nome FROM Usuarios";

                //SqlCommand cmd = new SqlCommand(selecao, conn);
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //conn.Open();
                //DataTable tabela = new DataTable();
                //da.Fill(tabela);

                var sprint = RetornarSprintDaSemana();

                DataTable tabela = RetonarPontos(sprint[0].ToString(), sprint[1].ToString(), "Produção");

                List<String> listaDesenvolvedores = new List<string>();
                foreach (DataRow item in tabela.Rows)
                {
                    listaDesenvolvedores.Add(item[0].ToString());
                }
                return listaDesenvolvedores;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable RetonarPontos(String indicePontos, String semana, String tipo)
        {
            try
            {
                var selecao = "SELECT Pto.Programador, Sum(Pto.Qtde_Teste) as TotalTestes, " +
                  "       Round( sum(Pto.Qtde_Pontos) / Cast(rTrim(Cast(Count(Distinct(Pto.OS)) as char))+'.0' as float),4 )as Complexidade, " +
                  "       Sum( case when Pto.Testado=0 then Pto.Qtde_Pontos else 0 end ) as NaoTestado, " +
                  "       Sum( case when Pto.Testado=1 then Pto.Qtde_Pontos else 0 end ) - " +
                  "       Coalesce( ( Select Sum(Qtde_Pontos) from Pontos where Programador=pto.Programador and Sprint=@parSprint and Tipo='Quebra'" + RetornarFiltroSemanaSprint(semana) + "),0) " +
                    "           as Testado, " +
                  "       ( Sum( case when Pto.Testado=1 then Pto.Qtde_Pontos else 0 end ) + Sum( case when Pto.Testado=0 then Qtde_Pontos else 0 end )) as Total, " +
                  "Coalesce( ( Select Sum(Qtde_Pontos) from Pontos where Programador=pto.Programador and Sprint=@parSprint and Tipo='Quebra'" + RetornarFiltroSemanaSprint(semana) + "),0) as TotalQuebra " +
                  "FROM Pontos Pto " +
                  "WHERE Pto.Sprint=@parSprint and Pto.Tipo=@parTipo " +
                  RetornarFiltroSemanaSprint(semana) + " " +
                  "Group by programador " +
                  "Order by 2 desc, 3 desc";

                SqlCommand cmd = new SqlCommand(selecao, conn);
                cmd.Parameters.AddWithValue("@parSprint", indicePontos);
                cmd.Parameters.AddWithValue("@parTipo", tipo);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                DataTable tabela = new DataTable();
                da.Fill(tabela);

                return tabela;
            }
            finally
            {
                conn.Close();
            }
        }

        public string[] RetornarSprintDaSemana()
        {
            try
            {
                SqlCommand cdmSprint = new SqlCommand("select Sprint, Max(SemanaSprint ) " +
                                                      "from pontos " +
                                                      "group by Sprint " +
                                                      "order by Sprint Desc ", conn);
                SqlDataAdapter da = new SqlDataAdapter(cdmSprint);
                conn.Open();
                DataTable tabela = new DataTable();
                da.Fill(tabela);

                DataRow[] dtRow = tabela.Select();
                string[] dados = new string[] { dtRow[0].ItemArray[0].ToString(), dtRow[0].ItemArray[1].ToString() };

                return dados;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable RetornarListaSprint()
        {
            try
            {
                var selecao = "select Distinct(Sprint) as Sprint from Pontos order by 1 desc";
                SqlCommand cmd = new SqlCommand(selecao, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                DataTable tabela = new DataTable();
                da.Fill(tabela);

                return tabela;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable RetornarListaSemana(string semana)
        {
            try
            {
                var selecao = "select Distinct(SemanaSprint) as SemanaSprint " +
                              "FROM Pontos " +
                              "Where Sprint=@parSprint And " +
                              "      SemanaSprint is not null And SemanaSprint <> '' " +
                              "Order by 1 ";
                SqlCommand cmd = new SqlCommand(selecao, conn);
                cmd.Parameters.AddWithValue("@parSprint", semana);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                DataTable tabela = new DataTable();
                da.Fill(tabela);

                return tabela;
            }
            finally
            {
                conn.Close();
            }
        }

        public Boolean Conectar(string usuario, string senha)
        {
            try
            {
                Boolean conectado = false;

                var selecao = "SELECT * FROM Usuarios WHERE Login=@parLogin AND Senha=@parSenha";
                SqlCommand comTestado = new SqlCommand(selecao, conn);
                comTestado.Parameters.AddWithValue("@parLogin", usuario);
                comTestado.Parameters.AddWithValue("@parSenha", senha);
                SqlDataAdapter adapter = new SqlDataAdapter(comTestado);
                DataTable tabela = new DataTable();
                conn.Open();
                adapter.Fill(tabela);

                if (tabela.Rows.Count > 0)
                {
                    conectado = true;
                }
                return conectado;
            }
            finally
            {
                conn.Close();
            }
        }

        public Boolean InserirDados(PontosSprint pontos)
        {
            try
            {
                conn.Open();
                var selecao = "Insert into Pontos (OS,Qtde_Pontos,Qtde_teste,Programador,Sprint, SemanaSprint,IdNota,Tipo,Dupla,Data,Testado) " +
                              " VALUES (@parOS, @parQtde_Pontos, @parQtde_teste, @parProgramador, @parSprint, @parSemanaSprint, @parIdNota, @parTipo, @parDupla, @parData, @parTestado)";
                SqlCommand comand = new SqlCommand(selecao, conn);
                comand.Parameters.AddWithValue("@parOS", pontos.Os);
                comand.Parameters.AddWithValue("@parQtde_Pontos", pontos.Pontos);
                comand.Parameters.AddWithValue("@parQtde_teste", pontos.Testes);
                comand.Parameters.AddWithValue("@parProgramador", pontos.Desenvolvedor);
                comand.Parameters.AddWithValue("@parSprint", pontos.Sprint);
                comand.Parameters.AddWithValue("@parSemanaSprint", pontos.Semana);
                comand.Parameters.AddWithValue("@parIdNota", pontos.Os);
                comand.Parameters.AddWithValue("@parTipo", "Produção");
                comand.Parameters.AddWithValue("@parDupla", "Cowboy");
                comand.Parameters.AddWithValue("@parData", DateTime.Now);
                comand.Parameters.AddWithValue("@parTestado", 0);
                comand.ExecuteNonQuery();

                return true;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
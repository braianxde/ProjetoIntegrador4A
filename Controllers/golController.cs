﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ProjetoIntegrador4A.Models;
using ProjetoIntegrador4A.Model;

namespace ProjetoIntegrador4A.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class golController : ControllerBase
    {
        Conexao conexao = new Conexao();
        MySqlCommand cmd = new MySqlCommand();

        [AcceptVerbs("POST")]

        [Route("InserirGol")]
        public string InserirGol(gol gol)
        {
            try
            {

                UsuarioController user = new UsuarioController();
                bool sessao = user.validaSessao(this.Request.Headers["Authorization"]);

                if (!sessao)
                {
                    return "Usuario nao esta logado";
                }

                cmd.CommandText = "insert into gol (id, tempo, minuto, gol_contra, id_jogador, id_partida) values (@id, @tempo, @minuto, @gol_contra, @id_jogador, @id_partida)";

                cmd.Parameters.AddWithValue("@id", gol.Id);
                cmd.Parameters.AddWithValue("@tempo", gol.Tempo);
                cmd.Parameters.AddWithValue("@minuto", gol.Minuto);
                cmd.Parameters.AddWithValue("@gol_contra", gol.Gol_contra);
                cmd.Parameters.AddWithValue("@id_jogador", gol.Id_jogador);
                cmd.Parameters.AddWithValue("@id_partida", gol.Id_partida);

                cmd.Connection = conexao.conectar();

                cmd.ExecuteNonQuery();

                conexao.desconectar();

                return "Gol inserido na partida!";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [AcceptVerbs("GET")]
        [Route("ConsultaGolsPorPartida/{id_partida}")]
        public string ConsultaGolsPorPartida(int id_partida)
        {
            try
            {

                UsuarioController user = new UsuarioController();
                bool sessao = user.validaSessao(this.Request.Headers["Authorization"]);

                if (!sessao)
                {
                    return "Usuario nao esta logado";
                }

                List<gol> gols = new List<gol>();
                int varint = 0;
                cmd.CommandText = "SELECT * FROM gol where id_partida = @id_partida";
                cmd.Parameters.AddWithValue("@id_partida", id_partida);
                cmd.Connection = conexao.conectar();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        varint++;
                        gols.Add(new gol(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5)));
                    }
                    reader.NextResult();
                }
                reader.Close();
                return JsonConvert.SerializeObject(gols, Formatting.Indented);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexao.desconectar();
            }
        }
    }
}

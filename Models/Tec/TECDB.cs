﻿#define mantenimiento

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using TecAPI.Models.Tutorias;

namespace TecAPI.Models.Tec
{
    public static class TECDB
    {
        public static string host = "localhost";
        public static string dataBase = "TEC";
        public static string userName = "sa";
        public static string password = "7271";

        public static bool ExisteNumeroDeControl(string _numeroDeControl)
        {
            bool resp = false;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT aluctr FROM dcalum WHERE aluctr = '{_numeroDeControl}' ", connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    resp = true;
                }
                connection.Close();
            }
            catch (Exception e)
            {



            }

            return resp;

        }

        public static bool ExisteClave(string _clave)
        {
            bool resp = false;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT percve FROM dperso WHERE percve = '{_clave}' ", connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    resp = true;
                }
                connection.Close();
            }
            catch (Exception e)
            {
#if mantenimiento
                System.Console.Write($"--------------------------------\n\n ERROR -> \n {e.ToString()}\n\n--------------------------------");
#endif

            }

            return resp;

        }

        public static string TraerCarrera(string _numeroDeControl)
        {
            string carrera = "";
            SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
            connection.Open();
            SqlCommand command = new SqlCommand($"SELECT carcve FROM dcalum WHERE aluctr = '{_numeroDeControl}' ", connection);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                carrera = dr["carcve"].ToString();
            }
            connection.Close();
            return carrera;
        }
        public static Usuarios TraerDatosAlumno(string _numeroDeControl)
        {
            Usuarios resp = null;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT aluapp, aluapm, alunom, alusex FROM dalumn WHERE aluctr = '{_numeroDeControl}' ", connection);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {

                    resp = new Usuarios
                    {
                        Nombre = dr["alunom"].ToString().TrimEnd().ToLower(),
                        ApellidoPaterno = dr["aluapp"].ToString().TrimEnd().ToLower(),
                        ApellidoMaterno = dr["aluapm"].ToString().TrimEnd().ToLower(),
                        Genero = ((dr["alusex"].ToString() == "2") ? "H" : "M"),
                    };

                }



                connection.Close();
            }
            catch (Exception e)
            {


#if mantenimiento
                System.Console.Write($"--------------------------------\n\n ERROR -> \n {e.ToString()}\n\n--------------------------------");

#endif

            }
            return resp;

        }

        public static Usuarios TraerDatosPersonal(string _clave)
        {
            Usuarios resp = null;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT substring(PERAPE,1,CHARINDEX(' ',PERAPE)) AS APP,   substring(PERAPE,CHARINDEX(' ',PERAPE), LEN(PERAPE)) AS APM, pernom, persig, persex FROM dperso WHERE percve = {_clave} ", connection);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    resp = new Usuarios
                    {
                        Nombre = dr["pernom"].ToString().TrimEnd().ToLower(),
                        ApellidoPaterno = dr["APP"].ToString().TrimEnd().ToLower(),
                        ApellidoMaterno = dr["APM"].ToString().TrimEnd().ToLower(),
                        Genero = ((dr["persex"].ToString() == "2") ? "M" : "H")
                    };
                }



                connection.Close();
            }
            catch (Exception e)
            {


                System.Console.Write("ERROR -> ");
                System.Console.Write(e.ToString());

            }

            return resp;

        }
        public static byte MostrarCarreraId(string _numeroDeControl)
        {
            byte resp = 0;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT carcve FROM dcalum WHERE aluctr = '{_numeroDeControl}' ", connection);
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    resp = byte.Parse(dr["carcve"].ToString());
                }



                connection.Close();
            }
            catch (Exception e)
            {


                System.Console.Write("ERROR -> ");
                System.Console.Write(e.ToString());

            }
            return resp;

        }

        public static bool ExisteEstudiante(string _numeroDeControl, string _curp)
        {
            bool resp = false;
            try
            {
                SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT aluctr FROM dalumn WHERE aluctr = '{_numeroDeControl}' AND alucur = '{_curp}'", connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    resp = true;
                }
                connection.Close();
            }
            catch (Exception e)
            {




            }

            return resp;
        }

        public static List<Personales> MostrarPersonales() {

            List<Personales> PersonalLista = new List<Personales>();
            SqlConnection connection = new SqlConnection($"Data Source={host};Initial Catalog={dataBase};User ID={userName};Password={password}");
            connection.Open();
            SqlCommand command = new SqlCommand($" SELECT PERNOM, persex, PERCVE, substring(PERAPE,1,CHARINDEX(' ',PERAPE)) AS APP,   substring(PERAPE,CHARINDEX(' ',PERAPE), LEN(PERAPE)) AS APM  FROM dperso ORDER BY PERNOM", connection);
            SqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                bool existe = false;
                using(TUTORIASContext db = new TUTORIASContext()){
                    if(db.Personales.Where(w=> w.Cve == dr["PERCVE"].ToString()).Count() > 0)
                    {
                        existe = true;
                    }
                }
                if (!existe)
                {
                    PersonalLista.Add(
                        new Personales()
                        {
                            Usuario = new Usuarios()
                            {
                                Nombre = dr["PERNOM"].ToString().TrimEnd(),
                                ApellidoPaterno = dr["APP"].ToString().TrimEnd().TrimStart(),
                                ApellidoMaterno = dr["APM"].ToString().TrimEnd().TrimStart(),
                                Genero = ((dr["persex"].ToString() == "2") ? "h" : "m")
                            },
                            Cve = dr["PERCVE"].ToString()
                        }
                  );
                }
            }

            connection.Close();

             return PersonalLista;
        }
    }
}

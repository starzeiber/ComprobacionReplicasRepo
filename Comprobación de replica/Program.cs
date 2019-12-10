using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaNegocio;
using System.Configuration;
using CorreoElectronico;
using System.Diagnostics;

namespace Comprobación_de_replica
{
    class Program
    {
        /// <summary>
        /// Variable que contiene la configuración del correo electrónico
        /// </summary>
        public static ConfiguracionCorreo configuracionCorreo;
        /// <summary>
        /// Instancia para poder enviar un correo electrónico
        /// </summary>
        public static Correo correo;
        /// <summary>
        /// Instancia que contiene la respuesta sobre el envío de correo
        /// </summary>
        public static CorreoRespuesta respuestaCorreo;
        public static String cuerpoCorreo = "";

        static void Main(string[] args)
        {
            correo = new Correo();
            respuestaCorreo = new CorreoRespuesta();

            //@161018 Se comienza cargando la configuración para poder enviar un correo electrónico
            Boolean esCorrectaLaCargaInicial = true;
            esCorrectaLaCargaInicial = CargarConfiguracionCorreo();

            //@161018 Se comprueba que el log exista o se crea
            if (CrearLog() != true)
            {
                //@161018 Si hubo un problema con la creación del log, se enviará correo sobre el mismo comprobando si se cargó la configuración del correo
                if (esCorrectaLaCargaInicial == true)
                {
                    correo = new Correo();
                    respuestaCorreo = correo.EnviarCorreoError(configuracionCorreo, "Validación réplica", ObtenerHtml("Error al crear el log"));
                    if (respuestaCorreo.envioCorrecto != true)
                    {
                        cLogErrores.Escribir_Log_Error("Error: " + respuestaCorreo.descripcionRespuesta);
                    }
                    else
                    {
                        cLogErrores.Escribir_Log_Evento("Se envió el correo correctamente");
                    }
                }
            }
            else //@161018 Se inicia todo
            {
                try
                {
                    Debug.WriteLine("Ejecutando la comprobacion de informacion");

                    //@060919 se verifica que las ventas hayan sido recolectadas de manera correcta
                    if (Operaciones.ValidarPasoVentas(ref cuerpoCorreo) != true)
                    {
                        Debug.WriteLine("Error en validar el paso de ventas al BO");
                        cuerpoCorreo = "Error en validar el paso de ventas al BO";
                        EnviarCorreo();
                        Environment.Exit(0);
                    }

                    List<TablasValidacion> listaTablasValidacion = new List<TablasValidacion>();
                    //@060919 Se ha comprobado el paso de trx, ahora se necesita comprobar BO y para ello se cargan las tablas que se verificarán
                    if (CargarTablasValidacionBo(ref listaTablasValidacion) != true)
                    {
                        Debug.WriteLine("Error al cargar las tablas a validar sobre los BO");
                        cuerpoCorreo = "Error al cargar las tablas a validar sobre los BO";
                        EnviarCorreo();
                        Environment.Exit(0);
                    }

                    //@161018 Se consultan las dos bases de BO sobre las tablas a comprobar
                    if (Operaciones.ValidarReplicaEntreBOs(listaTablasValidacion, ref cuerpoCorreo) != true)
                    {
                        Debug.WriteLine("Error en el proceso de validación de réplica entre los BOs");
                        cuerpoCorreo = "Error en el proceso de validación de réplica entre los BOs";
                        EnviarCorreo();
                        Environment.Exit(0);
                    }

                    //@161018 una vez se tiene la información de cada tabla en los dos BOs, se valida el conteo
                    //cLogErrores.Escribir_Log_Evento("Se inicia la validación de los datos");
                    
                    //String mensaje = String.Empty;
                    //foreach (Conteo cadaTablaConteo in listaConteo)
                    //{
                    //    if (cadaTablaConteo.conteoPrimeraBase > cadaTablaConteo.conteoSegundaBase)
                    //    {
                    //        cLogErrores.Escribir_Log_Evento("La tabla en BO " + cadaTablaConteo.nombreTabla + " tiene mas registros que historico");
                    //        cadaTablaConteo.conteoDiferencia = cadaTablaConteo.conteoPrimeraBase - cadaTablaConteo.conteoSegundaBase;
                    //        mensaje = mensaje + "La tabla productiva " + cadaTablaConteo.nombreTabla + " presenta " + cadaTablaConteo.conteoDiferencia.ToString() + " mas registros que en la histórica <br/>";
                    //        cadaTablaConteo.conteoDiferencia = 0;
                    //    }
                    //    else if (cadaTablaConteo.conteoPrimeraBase < cadaTablaConteo.conteoSegundaBase)
                    //    {
                    //        cLogErrores.Escribir_Log_Evento("La tabla en BO " + cadaTablaConteo.nombreTabla + " tiene menos registros que historico");
                    //        cadaTablaConteo.conteoDiferencia = cadaTablaConteo.conteoSegundaBase - cadaTablaConteo.conteoPrimeraBase;
                    //        mensaje = mensaje + "La tabla histórica " + cadaTablaConteo.nombreTabla + " presenta " + cadaTablaConteo.conteoDiferencia.ToString() + " mas registros que en la productiva <br/>";
                    //        cadaTablaConteo.conteoDiferencia = 0;
                    //    }
                    //    else
                    //    {
                    //        cadaTablaConteo.conteoDiferencia = 0;
                    //        mensaje = mensaje + "La tabla " + cadaTablaConteo.nombreTabla + " no presenta diferencias entre las bases<br/>";
                    //    }
                    //}                    

                    
                    //@161018 se envía el correo con las validaciones
                    EnviarCorreo();
                }
                catch (Exception ex)
                {
                    cuerpoCorreo = ex.Message;
                    EnviarCorreo();
                    //respuestaCorreo = correo.EnviarCorreoError(configuracionCorreo, "Validación réplica", ObtenerHtml(ex.Message));
                    //if (respuestaCorreo.envioCorrecto != true)
                    //{
                    //    cLogErrores.Escribir_Log_Error("Error: " + respuestaCorreo.descripcionRespuesta);
                    //}
                    //else
                    //{
                    //    cLogErrores.Escribir_Log_Evento("Se envió el correo correctamente");
                    //}
                }
            }
        }

        /// <summary>
        /// Función que entrega el html para ser enviado por correo electrónico
        /// </summary>
        /// <returns>HTML formado con todo y cabecera</returns>
        public static String ObtenerHtml(String cuerpoCorreo)
        {
            try
            {
                StringBuilder sHtml = new StringBuilder();
                sHtml.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
                sHtml.Append("<html xmlns='http://www.w3.org/1999/xhtml' >");
                sHtml.Append("<head><title>Untitled Page</title></head>");
                sHtml.Append("<body><img src=\"cid:Encabezado\">");
                sHtml.Append(cuerpoCorreo);
                sHtml.Append("</body></html>");
                return sHtml.ToString();
            }
            catch (Exception ex)
            {
                return "Error creando el mensaje de correo: " + ex.Message;
            }

        }

        /// <summary>
        /// Función que comprueba y/o crea el log
        /// </summary>
        /// <returns></returns>
        public static Boolean CrearLog()
        {
            try
            {
                cLogErrores.sNombreLog = "ValRepli";
                cLogErrores.sNombreOrigen = "ValRepli";
                cLogErrores.Crear_Log();
                cLogErrores.Escribir_Log_Evento("Se inicia la aplicación");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Función que carga la configuración para el envío de correo
        /// </summary>
        /// <returns></returns>
        public static Boolean CargarConfiguracionCorreo()
        {
            try
            {
                configuracionCorreo = new ConfiguracionCorreo()
                {
                    conCertificado = true,
                    conImagenDeEncabezado = true,
                    usuario = ConfigurationManager.AppSettings["usuario"],
                    pass = ConfigurationManager.AppSettings["pass"],
                    puerto = int.Parse(ConfigurationManager.AppSettings["puerto"]),
                    remitente = ConfigurationManager.AppSettings["usuario"],
                    smtp = ConfigurationManager.AppSettings["smtp"],
                    pathImagenEncabezado = ConfigurationManager.AppSettings["pathImagenEncabezado"],
                    esHtmlElCuerpoCorreo = true
                };
                int contadorAux = int.Parse(ConfigurationManager.AppSettings["numeroDestinatarios"]);
                for (int i = 1; i <= contadorAux; i++)
                {
                    configuracionCorreo.listaDestinatarios.Add(ConfigurationManager.AppSettings["destinatario" + i.ToString()]);
                }
                contadorAux = int.Parse(ConfigurationManager.AppSettings["numeroDestinatariosError"]);
                for (int i = 1; i <= contadorAux; i++)
                {
                    configuracionCorreo.listaDestinatariosError.Add(ConfigurationManager.AppSettings["destinatarioError" + i.ToString()]);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Carga desde el web config, las tablas a validar entre  los BO
        /// </summary>
        /// <param name="listaTablasValidacion">Lista de objetos TablasValidacion</param>
        /// <returns></returns>
        public static Boolean CargarTablasValidacionBo(ref List<TablasValidacion> listaTablasValidacion)
        {
            //@161018 se obtiene las tablas que se comprobarán
            int contadorTablas = 1;
            try
            {
                int numeroTablas = int.Parse(ConfigurationManager.AppSettings["numTablas"].ToString());
                while (contadorTablas <= numeroTablas)
                {
                    TablasValidacion cadaTablaConSuCampoDeValidacion = new TablasValidacion()
                    {
                        nombreTabla = ConfigurationManager.AppSettings["tab" + contadorTablas.ToString()].ToString(),
                        campoValidacion = ConfigurationManager.AppSettings["campoFechatab" + contadorTablas.ToString()].ToString()
                    };
                    cLogErrores.Escribir_Log_Evento("Se revisará la tabla " + cadaTablaConSuCampoDeValidacion.nombreTabla + " sobre el campo " + cadaTablaConSuCampoDeValidacion.campoValidacion);
                    listaTablasValidacion.Add(cadaTablaConSuCampoDeValidacion);
                    contadorTablas++;
                }
                return true;
            }
            catch (Exception ex)
            {
                cLogErrores.Escribir_Log_Error("CargarTablasValidacion. Error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Función que envía el correo electrónico con los resultados
        /// </summary>
        public static void EnviarCorreo()
        {
            respuestaCorreo = correo.EnviarCorreo(configuracionCorreo, "Validación réplica", ObtenerHtml(cuerpoCorreo));
            if (respuestaCorreo.envioCorrecto != true)
            {
                cLogErrores.Escribir_Log_Error("error: " + respuestaCorreo.descripcionRespuesta);
            }
            else
            {
                cLogErrores.Escribir_Log_Evento("Se envió el correo correctamente");
            }
        }
    }
}

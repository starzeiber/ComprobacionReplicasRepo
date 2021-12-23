using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseDatos;
using System.Data.SqlClient;
using System.Data;

namespace CapaNegocio
{
    /// <summary>
    /// Operaciones publicas
    /// </summary>
    public static class Operaciones
    {
        /// <summary>
        /// Consulta todas las bases transaccionales y hace un conteo de registros
        /// </summary>
        /// <returns></returns>
        public static int ConteoTransaccionales()
        {
            //190717 objeto de resultados de la consulta a base de datos
            ResultadoDB resultados = new ResultadoDB();
            int conteo = 0;
            try
            {
                String Consulta = "SELECT SUM(SUMA) AS TOTAL " +
                                "FROM " +
                                "(SELECT COUNT(RESPONSE_CODE) AS SUMA from dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1) " +
                                "UNION ALL " +
                                "SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO2.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1) " +
                                "UNION ALL " +
                                "SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO4.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1) " +
                                ")AS RESULTADO";

                cLogErrores.Escribir_Log_Evento("Consulta a trx: " + Consulta);

                    //190717 ejecución de la consulta a la base de datos
                    resultados = BaseDatos.OperacionesBaseDatos.EjecutaQry(Consulta, false,true);

                    //190717 se pregunta si existió un error en base de datos
                    if (!resultados.Error)
                    {
                        //190717 Se revisa si existen resultados
                        if (resultados.Datos.Tables.Count > 0)
                        {
                            //190717 de existir resultados, se revisa cuántos para recorrerlos
                            if (resultados.Datos.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in resultados.Datos.Tables[0].Rows)
                                {
                                    conteo = dr.Field<int>("TOTAL");
                                    cLogErrores.Escribir_Log_Evento("Se obtuvieron " + conteo.ToString() + " registros de las trx");
                                
                                }
                                //listaTablaConteo.Add(cadaTablaConteo);
                            }
                            else
                            {
                                cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ConteoTransaccionales");                                
                            }
                        }
                        else
                        {
                            cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ConteoTransaccionales");
                        }
                    }
                    else
                    {
                        cLogErrores.Escribir_Log_Advertencia("No hay resultados, error: " + resultados.Excepcion + ", CapaNegocio,OperacionesBaseDatos,ConteoTransaccionales");
                    }
                return conteo;
            }
            catch (Exception ex)
            {
                cLogErrores.Escribir_Log_Error("CapaNegocio,OperacionesBaseDatos,ConteoTransaccionales: " + ex.Message);
                return conteo;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Consulta BO y realiza un conteo de registro en la tabla de ventas unicamente
        /// </summary>
        /// <returns></returns>
        public static int ConteoBO()
        {
            //190717 objeto de resultados de la consulta a base de datos
            ResultadoDB resultados = new ResultadoDB();
            int conteo = 0;
            try
            {
                String Consulta = "SELECT COUNT(ID_RELOAD_SALE) as TOTAL FROM NIP_RELOADS_SALE " +
                                "WHERE CONVERT(DATE,DATE_TIME)=CONVERT(DATE,GETDATE()-1) ";

                cLogErrores.Escribir_Log_Evento("Consulta a BO: " + Consulta);

                //190717 ejecución de la consulta a la base de datos
                resultados = BaseDatos.OperacionesBaseDatos.EjecutaQry(Consulta, false);

                //190717 se pregunta si existió un error en base de datos
                if (!resultados.Error)
                {
                    //190717 Se revisa si existen resultados
                    if (resultados.Datos.Tables.Count > 0)
                    {
                        //190717 de existir resultados, se revisa cuántos para recorrerlos
                        if (resultados.Datos.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in resultados.Datos.Tables[0].Rows)
                            {
                                conteo = dr.Field<int>("TOTAL");
                                cLogErrores.Escribir_Log_Evento("Se obtuvieron " + conteo.ToString() + " registros del BO");

                            }
                            //listaTablaConteo.Add(cadaTablaConteo);
                        }
                        else
                        {
                            cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ConteoBO");
                        }
                    }
                    else
                    {
                        cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ConteoBO");
                    }
                }
                else
                {
                    cLogErrores.Escribir_Log_Advertencia("No hay resultados, error: " + resultados.Excepcion + ", CapaNegocio,OperacionesBaseDatos,ConteoBO");
                }
                return conteo;
            }
            catch (Exception ex)
            {
                cLogErrores.Escribir_Log_Error("CapaNegocio,OperacionesBaseDatos,ConteoBO: " + ex.Message);
                return conteo;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Realiza un conteo de transacciones entre los dos BO en las distintas tablas espejo
        /// </summary>
        /// <param name="listaTablasValidacion"></param>
        /// <returns></returns>
        public static List<Conteo> ConteoEntreBOs(List<TablasValidacion> listaTablasValidacion)
        {
            //190717 objeto de resultados de la consulta a base de datos
            ResultadoDB resultados = new ResultadoDB();
            
            List<Conteo> listaTablaConteo = new List<Conteo>();

            try
            {
                foreach (TablasValidacion cadaTabla in listaTablasValidacion)
                {                    
                    Conteo cadaTablaConteo = new Conteo();
                    cadaTablaConteo.nombreTabla = cadaTabla.nombreTabla;
                    String Consulta = "SELECT COUNT(*) AS CONTEO FROM " + cadaTabla.nombreTabla + " where convert(date," +
                        cadaTabla.campoValidacion + ")=convert(date,GETDATE()-1)";

                    cLogErrores.Escribir_Log_Evento("Consulta a BO: " + Consulta);

                    //190717 ejecución de la consulta a la base de datos
                    resultados = BaseDatos.OperacionesBaseDatos.EjecutaQry(Consulta, false);

                    //190717 se pregunta si existió un error en base de datos
                    if (!resultados.Error)
                    {
                        //190717 Se revisa si existen resultados
                        if (resultados.Datos.Tables.Count > 0)
                        {
                            //190717 de existir resultados, se revisa cuántos para recorrerlos
                            if (resultados.Datos.Tables[0].Rows.Count > 0)
                            {                                
                                foreach (DataRow dr in resultados.Datos.Tables[0].Rows)
                                {
                                    cadaTablaConteo.conteoPrimeraBase = dr.Field<int>("CONTEO");
                                    cLogErrores.Escribir_Log_Evento("Se obtuvieron " + cadaTablaConteo.conteoPrimeraBase.ToString() + " registros de BO");
                                }
                                //listaTablaConteo.Add(cadaTablaConteo);
                            }
                            else
                            {
                                cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                                cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                                //listaTablaConteo.Add(cadaTablaConteo);
                            }
                        }
                        else
                        {
                            cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                            cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                            //listaTablaConteo.Add(cadaTablaConteo);
                        }
                    }
                    else
                    {
                        cLogErrores.Escribir_Log_Advertencia("No hay resultados, error: " + resultados.Excepcion + ", CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                        cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                        //listaTablaConteo.Add(cadaTablaConteo);
                    }

                    cLogErrores.Escribir_Log_Evento("Consulta a Historico: " + Consulta);

                    //190717 ejecución de la consulta a la base de datos
                    resultados = BaseDatos.OperacionesBaseDatos.EjecutaQry(Consulta, true);

                    //190717 se pregunta si existió un error en base de datos
                    if (!resultados.Error)
                    {
                        //190717 Se revisa si existen resultados
                        if (resultados.Datos.Tables.Count > 0)
                        {
                            //190717 de existir resultados, se revisa cuántos para recorrerlos
                            if (resultados.Datos.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in resultados.Datos.Tables[0].Rows)
                                {
                                    cadaTablaConteo.conteoSegundaBase = dr.Field<int>("CONTEO");
                                    cLogErrores.Escribir_Log_Evento("Se obtuvieron " + cadaTablaConteo.conteoSegundaBase.ToString() + " registros de Historico");
                                }
                                //listaTablaConteo.Add(cadaTablaConteo);
                            }
                            else
                            {
                                cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                                cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                                //listaTablaConteo.Add(cadaTablaConteo);
                            }
                        }
                        else
                        {
                            cLogErrores.Escribir_Log_Advertencia("No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                            cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                            //listaTablaConteo.Add(cadaTablaConteo);
                        }
                    }
                    else
                    {
                        cLogErrores.Escribir_Log_Advertencia("No hay resultados, error: " + resultados.Excepcion + ", CapaNegocio,OperacionesBaseDatos,ComprobarReplica");
                        cadaTablaConteo.error = "No hay resultados, CapaNegocio,OperacionesBaseDatos,ComprobarReplica";
                        //listaTablaConteo.Add(cadaTablaConteo);
                    }
                    listaTablaConteo.Add(cadaTablaConteo);
                }
                return listaTablaConteo;
            }
            catch (Exception ex)
            {
                cLogErrores.Escribir_Log_Error("CapaNegocio,OperacionesBaseDatos,ComprobarReplica: " + ex.Message);                
                return listaTablaConteo;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Se revisa si las transaccionales fueron recolectadas exitosamente
        /// </summary>
        /// <param name="listaConteo"></param>
        /// <returns></returns>
        public static Boolean ValidarPasoVentas(ref String cuerpoCorreo)
        {
            Conteo conteo = new Conteo();

            if (ObtenerVentasTrxBo(ref conteo) != true)
            {
                cLogErrores.Escribir_Log_Error("Error en la obtención de información entre TRX y BO");
                return false;
            }
            else
            {
                if (ComprobacionConteos(conteo, ref cuerpoCorreo) != true)
                {
                    cLogErrores.Escribir_Log_Error("Error en la validación de cantidades entre TRX y BO");
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Se obtiene un conteo de lo que hay en las transaccionales y de lo que hay en BO
        /// </summary>
        /// <param name="listaConteo"></param>
        /// <returns></returns>
        public static Boolean ObtenerVentasTrxBo(ref Conteo conteo)
        {
            try
            {
                conteo.nombreTabla = "TransactionSales-NIP_RELOADS_SALE";
                conteo.conteoPrimeraBase = Operaciones.ConteoTransaccionales();
                conteo.conteoSegundaBase = Operaciones.ConteoBO();
                //listaConteo.Add(new Conteo() { nombreTabla = "TransactionSales", conteoPrimeraBase = Operaciones.ConteoTransaccionales(), conteoSegundaBase = Operaciones.ConteoBO() });
                return true;
            }
            catch (Exception ex)
            {
                cLogErrores.Escribir_Log_Error("ObtenerVentasTrxBo. " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Se obtendran resultados de registros y se comprobarán que sean iguales
        /// </summary>
        /// <param name="listaTablasValidacion">Listado de las tablas a comprobar en espejo</param>
        /// <returns></returns>
        public static Boolean ValidarReplicaEntreBOs(List<TablasValidacion> listaTablasValidacion, ref String cuerpoCorreo)
        {
            List<Conteo> listaConteoTablasBOs = Operaciones.ConteoEntreBOs(listaTablasValidacion);
            if (listaConteoTablasBOs.Count>0)
            {
                //Se verifican igualdades
                return ComprobacionConteos(listaConteoTablasBOs, ref cuerpoCorreo);
            }
            else
            {
                cLogErrores.Escribir_Log_Advertencia("No se tuvieron resultados, Operacines.ValidarReplicaEntreBOs");
                return false;
            }            
        }

        /// <summary>
        /// Se comparan cantidades entre los resultados obtenidos de BOs
        /// </summary>
        /// <param name="listaConteoTablasBOs">Listado de tabla y su conteo en casa BO</param>
        /// <returns></returns>
        public static Boolean ComprobacionConteos(List<Conteo> listaConteoTablasBOs, ref String cuerpoCorreo)
        {
            try
            {
                foreach (Conteo cadaTablaConteo in listaConteoTablasBOs)
                {
                    if (cadaTablaConteo.conteoPrimeraBase > cadaTablaConteo.conteoSegundaBase)
                    {
                        cLogErrores.Escribir_Log_Evento("La tabla en BO " + cadaTablaConteo.nombreTabla + " tiene mas registros que historico");
                        cadaTablaConteo.conteoDiferencia = cadaTablaConteo.conteoPrimeraBase - cadaTablaConteo.conteoSegundaBase;
                        cuerpoCorreo = cuerpoCorreo + "La tabla productiva " + cadaTablaConteo.nombreTabla + " presenta " + cadaTablaConteo.conteoDiferencia.ToString() + " mas registros que en la histórica <br/>";
                        cadaTablaConteo.conteoDiferencia = 0;
                    }
                    else if (cadaTablaConteo.conteoPrimeraBase < cadaTablaConteo.conteoSegundaBase)
                    {
                        cLogErrores.Escribir_Log_Evento("La tabla en BO " + cadaTablaConteo.nombreTabla + " tiene menos registros que historico");
                        cadaTablaConteo.conteoDiferencia = cadaTablaConteo.conteoSegundaBase - cadaTablaConteo.conteoPrimeraBase;
                        cuerpoCorreo = cuerpoCorreo + "La tabla histórica " + cadaTablaConteo.nombreTabla + " presenta " + cadaTablaConteo.conteoDiferencia.ToString() + " mas registros que en la productiva <br/>";
                        cadaTablaConteo.conteoDiferencia = 0;
                    }
                    else
                    {
                        cadaTablaConteo.conteoDiferencia = 0;
                        cuerpoCorreo = cuerpoCorreo + "La tabla " + cadaTablaConteo.nombreTabla + " no presenta diferencias entre las bases<br/>";
                        cLogErrores.Escribir_Log_Evento("La tabla " + cadaTablaConteo.nombreTabla + " no presenta diferencias entre las bases");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                cuerpoCorreo = "Error en la comprobación de cantidades. ComprobacionConteos. " + ex.Message;
                cLogErrores.Escribir_Log_Evento("Error en la comprobación de cantidades. ComprobacionConteos. " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Verificación de cantidades entre solo 2 tablas
        /// </summary>
        /// <param name="conteo">Objeto con las cantidades de las dos tablas</param>
        /// <returns></returns>
        public static Boolean ComprobacionConteos(Conteo conteo, ref String cuerpoCorreo)
        {
            try
            {
                if (conteo.conteoPrimeraBase > conteo.conteoSegundaBase)
                {
                    conteo.conteoDiferencia = conteo.conteoPrimeraBase - conteo.conteoSegundaBase;
                    cuerpoCorreo = cuerpoCorreo + "La cuenta en trx presenta una diferencia de " + conteo.conteoDiferencia.ToString() + " con respecto a BO <br/>";
                    cLogErrores.Escribir_Log_Evento(cuerpoCorreo);
                }
                else if (conteo.conteoPrimeraBase < conteo.conteoSegundaBase)
                {
                    conteo.conteoDiferencia = conteo.conteoSegundaBase - conteo.conteoPrimeraBase;
                    cuerpoCorreo = cuerpoCorreo + "La tabla NIP_RELOADS_SALE presenta una diferencia de " + conteo.conteoDiferencia.ToString() + " con respecto a TRX <br/>";
                    cLogErrores.Escribir_Log_Evento(cuerpoCorreo);
                }
                else
                {
                    cuerpoCorreo = cuerpoCorreo + "No se presenta diferencia entre trx y BO <br/>";
                    cLogErrores.Escribir_Log_Evento(cuerpoCorreo);
                }
                return true;
            }
            catch (Exception ex)
            {
                cuerpoCorreo = cuerpoCorreo +  "Error en la comprobación de cantidades. ComprobacionConteos. " + ex.Message;
                cLogErrores.Escribir_Log_Evento("Error en la comprobación de cantidades. ComprobacionConteos. " + ex.Message);
                return false;
            }
        }        
        
    }
}

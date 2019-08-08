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
    public static class Operaciones
    {
        public static List<TablaConteo> ComprobarReplica(List<TablasValidacion> listaTablasValidacion)
        {
            //190717 objeto de resultados de la consulta a base de datos
            ResultadoDB resultados = new ResultadoDB();
            
            List<TablaConteo> listaTablaConteo = new List<TablaConteo>();

            try
            {
                foreach (TablasValidacion cadaTabla in listaTablasValidacion)
                {                    
                    TablaConteo cadaTablaConteo = new TablaConteo();
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
    }
}

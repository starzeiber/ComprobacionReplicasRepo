using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// La información general de un ensamblado se controla mediante el siguiente 
// conjunto de atributos. Cambie estos valores de atributo para modificar la información
// asociada con un ensamblado.
[assembly: AssemblyTitle("Comprobación de replica")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Comprobación de replica")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Si establece ComVisible en false, los tipos de este ensamblado no estarán visibles 
// para los componentes COM.  Si es necesario obtener acceso a un tipo en este ensamblado desde 
// COM, establezca el atributo ComVisible en true en este tipo.
[assembly: ComVisible(false)]

// El siguiente GUID sirve como id. de typelib si este proyecto se expone a COM.
[assembly: Guid("8097500e-a40d-469b-b4d6-0ac60fd1ab9f")]

// La información de versión de un ensamblado consta de los cuatro valores siguientes:
//
//      Versión principal
//      Versión secundaria
//      Número de compilación
//      Revisión
//
// Puede especificar todos los valores o utilizar los números de compilación y de revisión predeterminados
// mediante el carácter '*', como se muestra a continuación:
// [assembly: AssemblyVersion("1.0.*")]

//Siglas    Autor       Fecha       Versión     Descripción
//@         UMB         17/10/2017  1.0.0.0     Se libera consultando a dos bases
//@         UMB         16/10/2018  2.0.0.0     Se reestructura para un mejor desempeño añadiendo más logs también
//@         UMB         16/10/2018  2.0.0.0     Corrección en el envío de correo a multicuenta
//@         UMB         11/01/2019  2.0.2.0     Se coloca una validación de que si es cero el conteo mande correo de error

[assembly: AssemblyVersion("2.0.2.0")]
[assembly: AssemblyFileVersion("2.0.2.0")]

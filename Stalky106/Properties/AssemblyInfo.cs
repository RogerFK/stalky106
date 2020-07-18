using System.Reflection;
using System.Runtime.InteropServices;

using Plugin = Stalky106.StalkyPlugin;

// RogerFK's note: I'm not translating the comments.

// La información general de un ensamblado se controla mediante el siguiente 
// conjunto de atributos. Cambie estos valores de atributo para modificar la información
// asociada con un ensamblado.
[assembly: AssemblyTitle("Stalky106V" + Plugin.VersionStr)]
[assembly: AssemblyDescription("Probably one of your favourite SCPSL plugins.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("RogerFK")]
[assembly: AssemblyProduct("Stalky106V" + Plugin.VersionStr)]
[assembly: AssemblyCopyright("CC-BY 3.0")]
[assembly: AssemblyTrademark("S106")]
[assembly: AssemblyCulture("en-US")]

// Si establece ComVisible en false, los tipos de este ensamblado no estarán visibles 
// para los componentes COM.  Si es necesario obtener acceso a un tipo en este ensamblado desde 
// COM, establezca el atributo ComVisible en true en este tipo.
[assembly: ComVisible(false)]

// El siguiente GUID sirve como id. de typelib si este proyecto se expone a COM.
[assembly: Guid("b9fbf509-2fa5-4a68-907c-a1a016225ec9")]

// La información de versión de un ensamblado consta de los cuatro valores siguientes:
//
//      Versión principal
//      Versión secundaria
//      Número de compilación
//      Revisión
//
// Puede especificar todos los valores o usar los números de compilación y de revisión predeterminados
// mediante el carácter "*", como se muestra a continuación:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Plugin.VersionStr)]
[assembly: AssemblyFileVersion(Plugin.VersionStr)]

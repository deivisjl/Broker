using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.XPath;

namespace Broker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            message.Add("Nombre", "Intermediario de firma MEDIIGSS-CCG");
            message.Add("Version", "1.0");

            return Ok(message);
        }

        [HttpPost("/api/Archivo")]
        public IActionResult GuardarArchivo([FromForm] string archivo)
        {
            string directorio = "INFORME-SALARIAL";

            List<string> rutasDocumento = this.AlmacenarArchivo(directorio);

            string nombreArchivo = string.Format(@"ejemplo.pdf");

            string pathDocumento = Path.Combine(rutasDocumento.ElementAt(1), nombreArchivo);

            string pathWindows = Path.Combine(rutasDocumento.ElementAt(0), nombreArchivo);

            byte[] bytesArchivo = Convert.FromBase64String(archivo);

            FileStream filestream = new FileStream(pathDocumento, FileMode.Create);

            filestream.Write(bytesArchivo, 0, bytesArchivo.Length);

            filestream.Close();

            return Ok();
        }

        private List<string> AlmacenarArchivo(string nombreDirectorio)
        {
            string year;

            string nombreArchivo;

            List<string> rutaAlmacenamiento;

            year = DateTime.Now.Year.ToString();

            nombreArchivo = Path.Combine("IVS-NOMINAS", nombreDirectorio);

            nombreArchivo = Path.Combine(nombreArchivo,year);

            rutaAlmacenamiento = this.RutasAlmacenado(nombreArchivo);

            return rutaAlmacenamiento;
        }

        public List<string> RutasAlmacenado(string destino)
        {
            string _pathIP = string.Format(@"192.168.1.1");
            string ruta;
            string rutaWindows;
            List<string> rutas;

            rutaWindows = Path.Combine(_pathIP, destino);

            ruta = Path.Combine(AddSubTree(GetInteropSetting("Directories:DocumentosIVS"), destino));

            rutas = new List<string>
            {
                rutaWindows,
                ruta
            };

            return rutas;
        }
        public string AddSubTree(string path, string destino)
        {
            string newPath = path;

            try
            {
                newPath = Path.Combine(newPath, destino);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                return newPath;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GetInteropSetting(string settingKey)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return GetLinuxSetting(settingKey);
            else
                return GetWindowSetting(settingKey);
        }

        public string GetWindowSetting(string settingKey)
        {
            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs")) + "/sirrhh";
        }

        public string GetLinuxSetting(string settingKey)
        {
            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs")) + "/sirrhh";
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Broker.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Broker.Controllers
{
    public class DocumentosController : Controller
    {
        [HttpPost("/api/Documents/SignBox/{name}")]
        public async Task<IActionResult> Prescription(string name)
        {
            string filePath;
            HttpRequest req;
            StreamReader reader;
            byte[] bytes;
            FileStream pdfFile;

            try
            {
                filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                filePath += "/" + name;

                req = Request;

                if (!req.Body.CanSeek)
                {
                    req.EnableBuffering();
                }


                req.Body.Position = 0;

                reader = new StreamReader(req.Body, Encoding.UTF8);

                req.Body.Position = 0;

                bytes = default(byte[]);

                using (var memstream = new MemoryStream())
                {
                    var buffer = new byte[Convert.ToInt32(req.ContentLength)];
                    var bytesRead = default(int);
                    while ((bytesRead = await reader.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        memstream.Write(buffer, 0, bytesRead);
                    bytes = memstream.ToArray();
                }

                pdfFile = new FileStream(filePath, FileMode.Create);
                pdfFile.Write(bytes, 0, bytes.Length);
                pdfFile.Close();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/values
        [HttpPost("/api/Documents/Prescriptions/{name}")]
        public IActionResult Prescriptions(string name, [FromForm] byte[] data)
        {
            string filePath;

            try
            {
                filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                filePath += "/" + name;

                FileStream pdfFile = new FileStream(filePath, FileMode.Create);
                pdfFile.Write(data, 0, data.Length);
                pdfFile.Close();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/Documents/Logs/{name}")]
        public IActionResult Logs(string name, [FromForm] object data)
        {
            StreamWriter logFile;
            string description;
            string currentDate;
            string currentDateTime;
            string currentFile;

            try
            {
                currentDate = DateTime.Now.ToString("dd-MM-yyyy");

                currentDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                currentFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                currentFile += "/Log-" + currentDate + ".txt";

                description = string.Format(@"Fecha de registro: {0}, Nombre de documento: {1}", currentDateTime, name);

                if (System.IO.File.Exists(currentFile))
                {
                    logFile = System.IO.File.AppendText(currentFile);

                    logFile.WriteLine(description);

                    logFile.Dispose();

                    return Ok();
                }

                logFile = new StreamWriter(currentFile, true, Encoding.ASCII);

                logFile.WriteLine(description);

                logFile.Dispose();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/api/Documents/DetailLogs/{date}")]
        public IActionResult ShowLogs(string date)
        {
            string currentFile;
            List<Logs> list;
            string[] lines;
            int i;

            try
            {
                currentFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                currentFile += "/Log-" + date + ".txt";

                if (!System.IO.File.Exists(currentFile))
                {
                    throw new Exception("File with date " + date + " not found!");
                }

                lines = System.IO.File.ReadAllLines(currentFile);

                list = new List<Logs>();

                i = 0;

                foreach (string line in lines)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        i++;

                        var dto = new Logs
                        {
                            Detalle = i + ". " + line
                        };

                        list.Add(dto);
                    }
                }

                return Ok(JsonSerializer.Serialize(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/Documents/LogProvider/{name}")]
        public async Task<IActionResult> LogProvider(string name, [FromBody] string log)
        {
            StreamWriter logFile;
            string description;
            string currentDate;
            string currentDateTime;
            string currentFile;
            HttpRequest req;

            try
            {
                currentDate = DateTime.Now.ToString("dd-MM-yyyy");

                currentDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                currentFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                currentFile += "/Log-" + currentDate + ".txt";

                req = Request;

                description = log;

                if (System.IO.File.Exists(currentFile))
                {
                    logFile = System.IO.File.AppendText(currentFile);

                    logFile.WriteLine(description);

                    logFile.Dispose();

                    return Ok();
                }

                logFile = new StreamWriter(currentFile, true, Encoding.ASCII);

                logFile.WriteLine(description);

                logFile.Dispose();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

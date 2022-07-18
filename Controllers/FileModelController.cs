using Hocr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Hocr.Converter;
using System.IO;
using System.Text;
using Hocr.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Hocr.Controllers
{
    [ApiController]
    [Route("api/")]
    public class FileModelController : ControllerBase
    {
        private IHocrConverter _converter;
        private readonly ILogger<FileModel> _logger;
        private IFileDataBase<FileModel> _db;

        [ActivatorUtilitiesConstructor]
        public FileModelController(ILogger<FileModel> logger, IFileDataBase<FileModel> db)
        {
            _db = db;
            _converter = new HocrConverter();
            _logger = logger;
        }

        public FileModelController(IHocrConverter converter, IFileDataBase<FileModel> db)
        {
            _converter = converter;
            _db = db;
        }

        /// <summary>
        /// Скачать файл текущей сессии
        /// </summary>
        /// <returns>Файл текущей сессии</returns>
        [HttpGet()]
        public async Task<ActionResult> GetCurrent()
        {
            if (HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("id").Length == 0)
            {
                return BadRequest("No such file or id isn't set");
            }
            FileModel file = await _db.GetFileAsync(int.Parse(HttpContext.Session.GetString("id")));
            FileContentResult result = new FileContentResult(file.data, file.fileType);
            result.FileDownloadName = file.fileName;
            return result;
        }

        /// <summary>
        /// Переключить текущий файл сессии на файл с id = @id
        /// </summary>
        /// <param name="id">id желаемого файла</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var file = await _db.GetFileAsync(id);
            string result = await _converter.HocrToJson(file);
            HttpContext.Session.SetString("id", id.ToString());
            return Ok($"Current file switched to file with id: {id}");
        }


        /// <summary>
        /// Получить все id файлов
        /// </summary>
        /// <returns></returns>
        [HttpGet("ids")]
        public async Task<ActionResult<IEnumerable>> GetIds()
        {
            var result = await _db.GetIds();
            return Ok(result);
        }

        /// <summary>
        /// Пост файла и переключение на него, как текущего файла сессии
        /// </summary>
        /// <param name="file">Файл, который необходимо загрузить</param>
        /// <returns>id загруженного файла</returns>
        [HttpPost]
        public async Task<ActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            FileModel fm = new FileModel();
            using (MemoryStream ms = new MemoryStream())
            {
                fm.fileName = file.FileName;
                fm.fileSize = file.Length;
                fm.fileType = file.ContentType;
                await file.CopyToAsync(ms);
                fm.data = ms.ToArray();
            }
            int id = await _db.InsertAsync(fm);
            var result = await _converter.HocrToJson(fm);
            HttpContext.Session.SetString("json", result);
            HttpContext.Session.SetString("id", id.ToString());
            return Ok(HttpContext.Session.GetString("id"));
        }

        /// <summary>
        /// Удалить файл текущей сессии
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            if(HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("id").Length == 0)
            {
                return BadRequest("No such selected");
            }
            int id = int.Parse(HttpContext.Session.GetString("id"));
            await _db.DeleteAsync(id);
            HttpContext.Session.SetString("id", "");
            return Ok("Current file deleted");
        }

        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="id">id файла, который нужно удалить</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _db.DeleteAsync(id);
            return Ok("File deleted");
        }

        /// <summary>
        /// Удалить все файлы
        /// </summary>
        /// <returns></returns>
        [HttpDelete("all")]
        public async Task<ActionResult> DeleteAll()
        {
            await _db.DeleteAllAsync();
            HttpContext.Session.SetString("id", "");
            return Ok();
        }
    }
}

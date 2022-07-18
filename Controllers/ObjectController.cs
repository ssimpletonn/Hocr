using Hocr.Contracts;
using Hocr.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hocr.Converter;
using System.Text;


namespace Hocr.Controllers
{
    [ApiController]
    [Route("api/json/")]
    public class ObjectController : ControllerBase
    {
        private IFileDataBase<FileModel> _db;
        private IHocrConverter _converter;


        public ObjectController(IFileDataBase<FileModel> db)
        {
            _db = db;
            _converter = new HocrConverter();
        }

        /// <summary>
        /// Получить json текущего файла сессии
        /// </summary>
        /// <returns>json</returns>
        [HttpGet()]
        public async Task<ActionResult<string>> GetCurrentJson()
        {
            if (HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("id").Length == 0)
            {
                return "No file selected";
            }
            var file = await _db.GetFileAsync(int.Parse(HttpContext.Session.GetString("id")));
            var json = await _converter.HocrToJson(file);
            return json;
        }

        /// <summary>
        /// Получить значение по ключу из файла сессии
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>

        [HttpGet("object/{key}")]
        public async Task<ActionResult<string>> GetObjectFromCurrent(string key)
        {
            if(HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("id").Length == 0)
            {
                return "No file selected";
            }
            var file = await _db.GetFileAsync(int.Parse(HttpContext.Session.GetString("id")));
            string result = await _converter.GetObjectByKey(file, key);
            return result;
        }

        /// <summary>
        /// Получить значение по ключу из файла по id
        /// </summary>
        /// <param name="id">id файла</param>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        [HttpGet("object/{id}/{key}")]
        public async Task<ActionResult<string>> GetObjectByKeyAndId(int id, string key)
        {
            FileModel file = await _db.GetFileAsync(id);
            var obj = await _converter.GetObjectByKey(file, key);
            return obj;
        }
    }
}
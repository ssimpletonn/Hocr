using Hocr.Models;
using System.Threading.Tasks;

namespace Hocr.Contracts
{
    public interface IHocrConverter
    {
        /// <summary>
        /// Преобразует FileModel hocr  в строку JSON
        /// </summary>
        /// <param name="file"></param>
        /// <returns>JSON</returns>
        public Task<string> HocrToJson(FileModel file);

        /// <summary>
        /// Преобразовать FileModel hocr в XML строку
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<string> HocrToXML(FileModel file);

        /// <summary>
        /// Объект по ключу из FileModel hocr
        /// </summary>
        /// <param name="file">hocr-файл</param>
        /// <param name="key">ключ</param>
        /// <returns>Значение</returns>
        public Task<string> GetObjectByKey(FileModel file, string key);
    }
}

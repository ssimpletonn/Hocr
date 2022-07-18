using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hocr.Contracts
{
    public interface IFileDataBase<T>
    {
        /// <summary>
        /// Настроить базу данных
        /// </summary>
        /// <returns></returns>
        public Task<bool> SetUp();

        /// <summary>
        /// Вставка в базу данных
        /// </summary>
        /// <param name="file">Модель файла</param>
        /// <returns></returns>
        public Task<int> InsertAsync(T file);

        /// <summary>
        /// Удаление файла из бд по id
        /// </summary>
        /// <param name="id">id файла в базе данных</param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Получение файла из бд
        /// </summary>
        /// <param name="id">id желаемого файла</param>
        /// <returns></returns>
        public Task<T> GetFileAsync(int id);

        public Task<bool> IsExists();


        /// <summary>
        /// Удалить все документы
        /// </summary>
        /// <returns></returns>
        public Task<bool> DeleteAllAsync();

        /// <summary>
        /// Все id документов
        /// </summary>
        /// <returns></returns>
        public Task<List<int>> GetIds();
    }
}

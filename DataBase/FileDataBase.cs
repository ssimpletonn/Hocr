using Hocr.Models;
using System.Threading.Tasks;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Xml.Linq;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Hocr.Contracts;
using System.Collections.Generic;

namespace Hocr.DataBase
{
    public class FileDataBase : IFileDataBase<FileModel>
    {
        private readonly DBConfig _dbConfig;
        public FileDataBase(DBConfig databaseConfig)
        {
            _dbConfig = databaseConfig;
        }
        public async Task<bool> SetUp()
        {
            if(await IsExists())
            {
                return true;
            }
            return await createDataBaseAsync();
        }
        
        private async Task<bool> createDataBaseAsync()
        {
            using SqliteConnection connection = new SqliteConnection(_dbConfig.dbname);
            await connection.ExecuteAsync("create table File (" +
                "ID integer primary key autoincrement," +
                "DATA blob NOT NULL," +
                "FILENAME text(250)," +
                "FILESIZE INTEGER," +
                "FILETYPE text(100));");
            return true;
        }

        /// <summary>
        /// Вставка файла в бд
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(FileModel file)
        {
            using var connection = new SqliteConnection(_dbConfig.dbname);
            await connection.OpenAsync();
            int id = await connection.QuerySingleAsync<int>("insert into File (DATA, FILENAME, FILESIZE, FILETYPE)" + 
                "values (@data, @fileName, @fileSize, @fileType); " +
                "select last_insert_rowid();", 
                file);
            return id;
        }

        /// <summary>
        /// Удаление файла из бд
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_dbConfig.dbname);
            await connection.ExecuteAsync($"delete from File where ID = {id}");
            return true;
        }

        /// <summary>
        /// Получение файла по id
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns></returns>
        public async Task<FileModel> GetFileAsync(int id)
        {
            using var connection = new SqliteConnection(_dbConfig.dbname);
            await connection.OpenAsync();
            var file = await connection.QuerySingleAsync<FileModel>($"select * from File where ID = {id}");
            return file;
        }

        public async Task<bool> IsExists()
        {
            using SqliteConnection connection = new SqliteConnection(_dbConfig.dbname);
            var table = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'File';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "File")
                return true;
            return false;
        }

        public async Task<bool> DeleteAllAsync()
        {
            using SqliteConnection connection = new SqliteConnection(_dbConfig.dbname);
            await connection.ExecuteAsync("delete from File");
            return true;
        }

        public async Task<List<int>> GetIds()
        {
            using SqliteConnection connection = new SqliteConnection(_dbConfig.dbname);
            var ids = await connection.QueryAsync<int>("select ID from File");
            return ids.ToList();
        }

    }
}

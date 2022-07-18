using Microsoft.AspNetCore.Http;
using System;

namespace Hocr.Models
{
    public class FileModel
    {
        public int ID { get; set; }
        public byte[] data { get; set; }
        public string fileName { get; set; }
        public long fileSize { get; set; }
        public string fileType { get; set; }
    }
}

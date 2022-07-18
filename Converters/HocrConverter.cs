using Hocr.Contracts;
using Hocr.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;

namespace Hocr.Converter
{
    public class HocrConverter : IHocrConverter
    {
        public async Task<string> HocrToJson(FileModel file)
        {            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(await HocrToXML(file));
            string result = JsonConvert.SerializeXmlNode(doc);
            return result;
        }

        public async Task<string> HocrToXML(FileModel file)
        {
            string result = await Task<string>.Run(async () =>
            {
                StringBuilder res = new StringBuilder();
                using (var reader = new MemoryStream(file.data))
                {
                    res.AppendLine(Encoding.UTF8.GetString(reader.ToArray()));
                }
                return res.ToString();
            });
            return result;
        }

        public async Task<string> GetObjectByKey(FileModel file, string key)
        {
            XmlPreloadedResolver xmlPreloadedResolver = new XmlPreloadedResolver(XmlKnownDtds.Xhtml10);
            xmlPreloadedResolver.Add(new Uri("http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"), File.ReadAllBytes("./dtd/xhtml.dtd"));
            XmlResolver resolver =  xmlPreloadedResolver;
            resolver.Credentials = CredentialCache.DefaultNetworkCredentials;
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = resolver;
            doc.LoadXml(await HocrToXML(file));
            XmlElement elem = doc.GetElementById(key);
            return elem.InnerText;
        }
    }
}

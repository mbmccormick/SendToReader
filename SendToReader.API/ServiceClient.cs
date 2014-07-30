using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SendToReader.API.Models;

namespace SendToReader.API
{
    public class ServiceClient
    {
        public static async Task InsertWebsite(Action<string> callback, ReaderDocument data)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://sendtoreader.cloudapp.net") as HttpWebRequest;
            request.Method = "POST";

            var upload = await request.GetRequestStreamAsync().ConfigureAwait(false);

            UTF8Encoding encoding1 = new UTF8Encoding();
            StreamWriter sw = new StreamWriter(upload, encoding1);

            sw.Write(String.Format("URL={0}&EmailAddress={1}", data.URL, data.EmailAddress));
            sw.Flush();

            sw.Dispose();

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding2 = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding2);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(sr.ReadToEnd());

            var nodes = doc.DocumentNode.Descendants("p").ToList();

            callback(nodes[1].InnerText);
        }

        public static async Task GetQueueStatus(Action<string> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://sendtoreader.cloudapp.net") as HttpWebRequest;
            request.Method = "GET";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(sr.ReadToEnd());

            var nodes = doc.DocumentNode.Descendants("p").ToList();

            callback(nodes[1].InnerText);
        }
    }
}

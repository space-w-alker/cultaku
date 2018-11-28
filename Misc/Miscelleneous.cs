using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace culTAKU.Misc
{
    public static class Miscelleneous
    {
        public static string GetHtml(string url)
        {
            string html = string.Empty;
            

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
            HttpWebResponse response = null;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Status);
            }
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            html = reader.ReadToEnd();
            return html;
        }
    }
}

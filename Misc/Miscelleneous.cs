﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace culTAKU.Misc
{
    public static class Miscelleneous
    {
        private const string AnimeListUrl = "https://myanimelist.net/";

        public static string GetHtml(string url)
        {
            string html = string.Empty;
            

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
            HttpWebResponse response = null;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            StreamReader reader;
            int retryCount = 0;
            while (true)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException e)
                {
                    if(retryCount == 15)
                    {
                        return null;
                    }
                    Console.WriteLine(e.Status);
                    Thread.Sleep(1000 * retryCount + 1);
                    retryCount += 1;
                    continue;

                }
                Stream stream = response.GetResponseStream();
                reader = new StreamReader(stream);
                break;
            }

            html = reader.ReadToEnd();
            return html;
        }

        public static bool IsConnectionActive()
        {
            string returnHtml = GetHtml(AnimeListUrl);
            if (returnHtml == null) return false;
            else return true;
        }

        public static string GetImage(string url, int id)
        {
            using (WebClient client = new WebClient())
            {
                int RetryCount = 0;
                while (true)
                {
                    try
                    {
                        string image_location = string.Format("/Image/AnimeImage-{0}.jpg", id);
                        client.DownloadFile(new Uri(url), Path.GetFullPath(image_location));
                        return image_location;
                    }
                    catch (WebException e)
                    {
                        if (RetryCount == 10)
                        {
                            return null;
                        }

                        Thread.Sleep(1000 * RetryCount + 1);
                        RetryCount += 1;
                        continue;
                    }
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Text;

namespace TrOCR.Helper
{
    public class WebHelper
    {
        public static string GetHtmlContent(string url)
        {
            try
            {
                var uri = new Uri(url);
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.Timeout = 5000;
                myHttpWebRequest.Accept = @"text/html,application/xhtml+xml,application/xml;*/*";
                myHttpWebRequest.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
                var response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var responseReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    var result = responseReader.ReadToEnd();
                    return result;
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return "";
            }
        }
    }
}

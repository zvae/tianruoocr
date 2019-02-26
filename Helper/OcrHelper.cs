using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace TrOCR.Helper
{
    public class OcrHelper
    {
        public static string TxOcr(Image img)
        {
            const string boundary = "----WebKitFormBoundaryRDEqU0w702X9cWPJ";
            var url = "https://ai.qq.com/cgi-bin/appdemo_generalocr";
            var refer = "http://ai.qq.com/product/ocr.shtml";
            return CommPost(img, boundary, url, refer);
        }

        public static string SgOcr(Image img)
        {
            const string boundary = "----WebKitFormBoundary8orYTmcj8BHvQpVU";
            var url = "http://ocr.shouji.sogou.com/v2/ocr/json";
            return CommPost(img, boundary, url, "");
        }

        public static string CommPost(Image img, string boundary, string url, string refer)
        {
            var b = ImgToBytes(img);
            var disposition = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"image_file\"; filename=\"pic.jpg\"\r\nContent-Type: image/jpeg\r\n\r\n";
            var header = Encoding.ASCII.GetBytes(disposition);
            return CommonHelper.PostMultiData(url, FmMain.MergeByte(header,b, new byte[0]), boundary, disposition, "", refer);
        }
        
        public static byte[] ImgToBytes(Image img)
        {
            byte[] result;
            try
            {
                var memoryStream = new MemoryStream();
                img.Save(memoryStream, ImageFormat.Jpeg);
                var array = new byte[memoryStream.Length];
                memoryStream.Position = 0L;
                memoryStream.Read(array, 0, (int)memoryStream.Length);
                memoryStream.Close();
                result = array;
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
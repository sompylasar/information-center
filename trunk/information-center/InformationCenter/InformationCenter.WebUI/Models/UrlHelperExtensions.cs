using System.Text;
using System.Web.Mvc;

namespace InformationCenter.WebUI.Models
{
    public static class UrlHelperExtensions
    {
        public static string RawEncode(this UrlHelper urlHelper, string filename, Encoding encoding)
        {
            string encodedFilename = "";
            byte[] bytes = encoding.GetBytes(filename);

            for (int i = 0; i < bytes.Length; i++)
            {
                encodedFilename += "%" + string.Format("{0:X}", (int)bytes[i]);
            }
            return encodedFilename;
        }
    }
}
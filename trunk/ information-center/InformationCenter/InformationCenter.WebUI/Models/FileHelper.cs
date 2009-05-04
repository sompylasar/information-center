using System.Text;

namespace InformationCenter.WebUI.Models
{
    public static class FileHelper
    {
        private const string SEPARATOR = "///";
        public const string DEFAULT_CONTENTTYPE = "application/octet-stream";

        public static string CombineFilename(string filename, string contentType)
        {
            contentType = (contentType ?? "");
            return (filename ?? "") + SEPARATOR + (string.IsNullOrEmpty(contentType) ? DEFAULT_CONTENTTYPE : contentType);
        }
        public static void SplitFilename(string combinedFilename, out string filename, out string contentType)
        {
            int pos = combinedFilename.LastIndexOf(SEPARATOR);
            if (pos < 0) pos = combinedFilename.Length;
            filename = combinedFilename.Substring(0, pos);
            contentType = (pos >= combinedFilename.Length ? "" : combinedFilename.Substring(pos+SEPARATOR.Length));
        }
    }
}
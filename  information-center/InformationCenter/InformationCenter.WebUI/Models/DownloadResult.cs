using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace InformationCenter.WebUI.Models
{
    public class DownloadResult : ActionResult
    {
        public DownloadResult()
        {
        }

        public DownloadResult(byte[] binaryData, string filename, string contentType)
        {
            BinaryData = binaryData;
            FileName = filename;
            ContentType = contentType;
        }

        public byte[] BinaryData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            bool isOpera = (context.RequestContext.HttpContext.Request.UserAgent.ToLowerInvariant().IndexOf("opera") >= 0);

            context.HttpContext.Response.ContentType = (String.IsNullOrEmpty(ContentType) || isOpera
                ? "application/download" //FileHelper.DEFAULT_CONTENTTYPE 
                : ContentType);
            Encoding encoding = Encoding.UTF8;

            if (!String.IsNullOrEmpty(FileName))
            {
                string encodedFilename = this.FileName;
                
                encodedFilename = (new UrlHelper(context.RequestContext)).RawEncode(this.FileName, encoding);

                context.HttpContext.Response.AddHeader("Content-Disposition",
                  "attachment; filename*="+encoding.WebName+"''" + encodedFilename  + "");
            }

            context.HttpContext.Response.Charset = encoding.WebName;
            context.HttpContext.Response.BinaryWrite(BinaryData);
        }
    }
}
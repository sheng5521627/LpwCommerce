using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Web.Framework.Mvc
{
    public class XmlDownloadResult : ActionResult
    {
        public string Xml { get; set; }

        public string FileDownloadName { get; set; }

        public XmlDownloadResult(string xml,string fileDownloadName)
        {
            Xml = xml;
            FileDownloadName = fileDownloadName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var document = new XmlDocument();

            document.LoadXml(Xml);
            var decl = document.FirstChild as XmlDeclaration;
            if(decl != null)
            {
                decl.Encoding = "utf-8";
            }
        }
    }
}

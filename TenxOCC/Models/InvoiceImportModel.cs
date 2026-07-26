using System.Web;

namespace TenxOCC.Web.Models
{
    public class InvoiceImportModel
    {
        public HttpPostedFileBase ExcelFile { get; set; }

        public string Message { get; set; }
    }
}
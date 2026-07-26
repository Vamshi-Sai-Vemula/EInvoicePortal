using System;
using System.Collections.Generic;
using TenxOCC.Data.Entities;

namespace TenxOCC.Web.Models
{
    public class InvoiceViewModel
    {
        public InvoiceHeader Header { get; set; }
        public List<InvoiceLine> Lines { get; set; }

        public InvoiceViewModel()
        {
            Header = new InvoiceHeader();
            Lines = new List<InvoiceLine>();
        }
    }

    public class InvoiceListViewModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal VatSum { get; set; }
        public string DocCur { get; set; }
        public int LineCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

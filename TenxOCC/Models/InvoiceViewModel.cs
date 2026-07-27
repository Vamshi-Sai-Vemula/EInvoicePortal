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

    public class DashboardViewModel
    {
        public int TotalInvoices { get; set; }
        public int PendingApprovalCount { get; set; }
        public int ApprovedCount { get; set; }
        public int PostedSuccessCount { get; set; }
        public int PostedFailedCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalVatSum { get; set; }

        public int StatusSuccessCount { get; set; }
        public int StatusFailedCount { get; set; }

        public List<InvoiceHeader> RecentInvoices { get; set; } = new List<InvoiceHeader>();
    }
}

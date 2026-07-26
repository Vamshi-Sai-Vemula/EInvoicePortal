using System;

namespace TenxOCC.Data.Entities
{
    public class InvoiceLine
    {

        public int DocEntry { get; set; }

        public int LineNum { get; set; }


        public string ItemCode { get; set; }

        public string Description { get; set; }


        public decimal Quantity { get; set; }

        public decimal LineTotal { get; set; }

        public decimal VatAmount { get; set; }


        public string VatGroup { get; set; }

        public decimal VatPercent { get; set; }


        public string UnitOfMeasure { get; set; }


        public decimal UnitPrice { get; set; }


        public decimal DiscountPercent { get; set; }


        public string NatureCode { get; set; }

        public string CommodityCode { get; set; }

        public string ClassificationListId { get; set; }

        public string HSCode { get; set; }


        public DateTime CreatedDate { get; set; }



        public virtual InvoiceHeader InvoiceHeader { get; set; }

    }
}
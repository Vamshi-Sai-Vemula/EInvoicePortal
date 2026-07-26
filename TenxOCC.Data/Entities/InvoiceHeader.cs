using System;
using System.Collections.Generic;

namespace TenxOCC.Data.Entities
{
    public class InvoiceHeader
    {
     


        public int DocEntry { get; set; }

        public int DocNum { get; set; }

        public string DocCur { get; set; }

        public DateTime DocDate { get; set; }

        public DateTime? DocDueDate { get; set; }


        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string BillingAddressCode { get; set; }

        public int? ContactCode { get; set; }

        public int? PaymentGroupNum { get; set; }

        public string BankCode { get; set; }

        public string BuyerReference { get; set; }


        public decimal? ExchangeRate { get; set; }

        public decimal VatSum { get; set; }

        public decimal DocTotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal RoundDifference { get; set; }


        public string SelfBilledFlag { get; set; }

        public string InvoiceTransactionType { get; set; }

        public string BeneficiaryID { get; set; }

        public string PrincipalID { get; set; }


        // Supplier

        public string SupplierVAT { get; set; }

        public string SupplierName { get; set; }

        public string SupplierPhone { get; set; }

        public string SupplierEmail { get; set; }

        public string SupplierCompanyID { get; set; }


        public string SupplierStreet { get; set; }

        public string SupplierStreet2 { get; set; }

        public string SupplierCity { get; set; }

        public string SupplierZip { get; set; }

        public string SupplierState { get; set; }

        public string SupplierCountry { get; set; }


        // Customer

        public string CustomerVAT { get; set; }

        public string CustomerCompanyID { get; set; }

        public string CustomerRegNum { get; set; }

        public string CustomerCountryDefault { get; set; }


        public string CustomerStreet { get; set; }

        public string CustomerStreet2 { get; set; }

        public string CustomerCity { get; set; }

        public string CustomerZip { get; set; }

        public string CustomerState { get; set; }

        public string CustomerCountry { get; set; }


        // Contact

        public string CustomerContactName { get; set; }

        public string CustomerContactPhone1 { get; set; }

        public string CustomerContactPhone2 { get; set; }

        public string CustomerContactMobile { get; set; }

        public string CustomerContactEmail { get; set; }


        public string PaymentTerms { get; set; }

        public string IBAN { get; set; }


        public decimal MaxVatRate { get; set; }


        public int? AttachmentEntry { get; set; }

        public string AttachmentJson { get; set; }


        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }



        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }

    }
}
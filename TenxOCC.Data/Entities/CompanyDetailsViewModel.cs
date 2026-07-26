using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Entities
{
    public class CompanyDetailsViewModel
    {
        public int Id { get; set; }

        // LOCAL ADDRESS DATA
        [Display(Name = "Company Name")]
        public string companyNameLocal { get; set; }

        [Display(Name = "Address")]
        public string addressLocal { get; set; }

        [Display(Name = "Street / PO Box")]
        public string streetLocal { get; set; }

        [Display(Name = "Street No.")]
        public string streetNoLocal { get; set; }

        [Display(Name = "Block")]
        public string blockLocal { get; set; }

        [Display(Name = "Building/Floor/Room")]
        public string buildingLocal { get; set; }

        [Display(Name = "City")]
        public string cityLocal { get; set; }

        [Display(Name = "Zip Code")]
        public string zipLocal { get; set; }

        [Display(Name = "County")]
        public string countyLocal { get; set; }

        [Display(Name = "State / Region")]
        public string stateLocal { get; set; }

        [Display(Name = "Country/Region")]
        public string countryLocal { get; set; }

        // FOREIGN ADDRESS DATA
        [Display(Name = "Company Name (Foreign)")]
        public string companyNameForeign { get; set; }

        [Display(Name = "Address (Foreign)")]
        public string addressForeign { get; set; }

        [Display(Name = "Street / PO Box")]
        public string streetForeign { get; set; }

        [Display(Name = "Street No.")]
        public string streetNoForeign { get; set; }

        [Display(Name = "State / Region")]
        public string stateForeign { get; set; }

        [Display(Name = "Country/Region")]
        public string countryForeign { get; set; }

        // TAX IDENTIFIERS & ACCOUNTING DETAILS
        [Display(Name = "Tax Office")]
        public string taxOffice { get; set; }

        [Display(Name = "Federal Tax ID 1")]
        public string federalTaxId1 { get; set; }

        [Display(Name = "Federal Tax ID 2")]
        public string federalTaxId2 { get; set; }

        [Display(Name = "Accounts Office Ref. (AO Ref.)")]
        public string aoRef { get; set; }

        [Display(Name = "Additional ID")]
        public string additionalId { get; set; }

        [Display(Name = "Unique Taxpayer Ref. (UTR)")]
        public string utr { get; set; }

        [Display(Name = "Employer's Reference")]
        public string employerRef { get; set; }

        [Display(Name = "Company Tax Rate (%)")]
        public decimal? companyTaxRate { get; set; }

        [Display(Name = "Exemption Number")]
        public string exemptionNo { get; set; }

        [Display(Name = "Tax Deduction Number")]
        public string taxDeductionNo { get; set; }

        [Display(Name = "Tax Official")]
        public string taxOfficial { get; set; }

        // CURRENCY CONFIGURATION
        [Display(Name = "Local Currency")]
        public string localCurrency { get; set; }

        [Display(Name = "System Currency")]
        public string systemCurrency { get; set; }

        [Display(Name = "Default Account Currency")]
        public string defaultAccountCurrency { get; set; }
    
}
}

using System;

namespace TenxOCC.Data.Entities
{
    public class CompanyDetailsEntity
    {
        public int Id { get; set; }

        // LOCAL ADDRESS DATA
        public string companyNameLocal { get; set; }
        public string addressLocal { get; set; }
        public string streetLocal { get; set; }
        public string streetNoLocal { get; set; }
        public string blockLocal { get; set; }
        public string buildingLocal { get; set; }
        public string cityLocal { get; set; }
        public string zipLocal { get; set; }
        public string countyLocal { get; set; }
        public string stateLocal { get; set; }
        public string countryLocal { get; set; }

        // FOREIGN ADDRESS DATA
        public string companyNameForeign { get; set; }
        public string addressForeign { get; set; }
        public string streetForeign { get; set; }
        public string streetNoForeign { get; set; }
        public string stateForeign { get; set; }
        public string countryForeign { get; set; }

        // TAX IDENTIFIERS & ACCOUNTING DETAILS
        public string taxOffice { get; set; }
        public string federalTaxId1 { get; set; }
        public string federalTaxId2 { get; set; }
        public string aoRef { get; set; }
        public string additionalId { get; set; }
        public string utr { get; set; }
        public string employerRef { get; set; }
        public decimal companyTaxRate { get; set; }
        public string exemptionNo { get; set; }
        public string taxDeductionNo { get; set; }
        public string taxOfficial { get; set; }

        // CURRENCY CONFIGURATION
        public string localCurrency { get; set; }
        public string systemCurrency { get; set; }
        public string defaultAccountCurrency { get; set; }

        // METADATA
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string CompanyCode { get; set; }

        public string LogoPath { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }

    

        public string UpdatedBy { get; set; }

        
    }
}
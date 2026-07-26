using System;
using System.Linq;
using System.Web.Mvc;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;
//using TenxOCC.Models;

namespace TenxOCC.Controllers
{
    public class CompanyDetailsController : Controller
    {
        private readonly ICompanyDetails _companyDetailsRepository;

        public CompanyDetailsController()
        {
            _companyDetailsRepository =
       new CompanyDetailsRepository();
        }

        public CompanyDetailsController(ICompanyDetails companyDetailsRepository)
        {
            _companyDetailsRepository = companyDetailsRepository;
        }

        // GET: CompanyDetails/CompanyDetails
        [HttpGet]
        public ActionResult CompanyDetails()
        {

            var entity =
            _companyDetailsRepository
            .GetAll()
            .FirstOrDefault();


            if (entity == null)
            {
                entity = new CompanyDetailsEntity();
            }


            return View(
            MapToViewModel(entity));

        }

        // POST: CompanyDetails/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CompanyDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CompanyDetails", model);
            }

            try
            {
                if (model.Id > 0)
                {
                    // UPDATE Existing Record
                    var existingEntity = _companyDetailsRepository.GetAll().FirstOrDefault(x => x.Id == model.Id);

                    if (existingEntity != null)
                    {
                        MapToEntity(model, existingEntity);
                        existingEntity.updatedAt = DateTime.Now;

                        _companyDetailsRepository.Update(existingEntity);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Record not found to update.");
                        return View("CompanyDetails", model);
                    }
                }
                else
                {
                    // INSERT New Record
                    var newEntity = new CompanyDetailsEntity();
                    MapToEntity(model, newEntity);
                    newEntity.createdAt = DateTime.Now;
                    newEntity.updatedAt = DateTime.Now;

                    _companyDetailsRepository.Insert(newEntity);
                }

                TempData["SuccessMessage"] = "Company details saved successfully!";
                return RedirectToAction("CompanyDetails");
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "CompanyDetailsController", "Save");
                ModelState.AddModelError("", "An error occurred while saving company details: " + ex.Message);
                return View("CompanyDetails", model);
            }
        }

        #region Private Mapping Helpers

        private CompanyDetailsViewModel MapToViewModel(CompanyDetailsEntity entity)
        {
            return new CompanyDetailsViewModel
            {
                Id = entity.Id,
                companyNameLocal = entity.companyNameLocal,
                addressLocal = entity.addressLocal,
                streetLocal = entity.streetLocal,
                streetNoLocal = entity.streetNoLocal,
                blockLocal = entity.blockLocal,
                buildingLocal = entity.buildingLocal,
                cityLocal = entity.cityLocal,
                zipLocal = entity.zipLocal,
                countyLocal = entity.countyLocal,
                stateLocal = entity.stateLocal,
                countryLocal = entity.countryLocal,

                companyNameForeign = entity.companyNameForeign,
                addressForeign = entity.addressForeign,
                streetForeign = entity.streetForeign,
                streetNoForeign = entity.streetNoForeign,
                stateForeign = entity.stateForeign,
                countryForeign = entity.countryForeign,

                taxOffice = entity.taxOffice,
                federalTaxId1 = entity.federalTaxId1,
                federalTaxId2 = entity.federalTaxId2,
                aoRef = entity.aoRef,
                additionalId = entity.additionalId,
                utr = entity.utr,
                employerRef = entity.employerRef,
                companyTaxRate = entity.companyTaxRate,
                exemptionNo = entity.exemptionNo,
                taxDeductionNo = entity.taxDeductionNo,
                taxOfficial = entity.taxOfficial,

                localCurrency = entity.localCurrency,
                systemCurrency = entity.systemCurrency,
                defaultAccountCurrency = entity.defaultAccountCurrency
            };
        }

        private void MapToEntity(CompanyDetailsViewModel vm, CompanyDetailsEntity entity)
        {
            entity.companyNameLocal = vm.companyNameLocal;
            entity.addressLocal = vm.addressLocal;
            entity.streetLocal = vm.streetLocal;
            entity.streetNoLocal = vm.streetNoLocal;
            entity.blockLocal = vm.blockLocal;
            entity.buildingLocal = vm.buildingLocal;
            entity.cityLocal = vm.cityLocal;
            entity.zipLocal = vm.zipLocal;
            entity.countyLocal = vm.countyLocal;
            entity.stateLocal = vm.stateLocal;
            entity.countryLocal = vm.countryLocal;

            entity.companyNameForeign = vm.companyNameForeign;
            entity.addressForeign = vm.addressForeign;
            entity.streetForeign = vm.streetForeign;
            entity.streetNoForeign = vm.streetNoForeign;
            entity.stateForeign = vm.stateForeign;
            entity.countryForeign = vm.countryForeign;

            entity.taxOffice = vm.taxOffice;
            entity.federalTaxId1 = vm.federalTaxId1;
            entity.federalTaxId2 = vm.federalTaxId2;
            entity.aoRef = vm.aoRef;
            entity.additionalId = vm.additionalId;
            entity.utr = vm.utr;
            entity.employerRef = vm.employerRef;
            entity.companyTaxRate = vm.companyTaxRate ?? 0.00m;
            entity.exemptionNo = vm.exemptionNo;
            entity.taxDeductionNo = vm.taxDeductionNo;
            entity.taxOfficial = vm.taxOfficial;

            entity.localCurrency = vm.localCurrency;
            entity.systemCurrency = vm.systemCurrency;
            entity.defaultAccountCurrency = vm.defaultAccountCurrency;
        }

        #endregion
    }
}
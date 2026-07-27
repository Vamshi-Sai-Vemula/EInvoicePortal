using System;
using System.Linq;
using System.Web.Mvc;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;
using TenxOCC.Data.Repositories;

namespace TenxOCC.Controllers
{
    public class CompanyDetailsController : Controller
    {
        private readonly ICompanyDetails _companyDetailsRepository;

        public CompanyDetailsController()
        {
            _companyDetailsRepository = new CompanyDetailsRepository();
        }

        public CompanyDetailsController(ICompanyDetails companyDetailsRepository)
        {
            _companyDetailsRepository = companyDetailsRepository;
        }

        // GET: CompanyDetails/Index
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var list = _companyDetailsRepository.GetAll().ToList();
                return View(list);
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "CompanyDetailsController", "Index");
                TempData["ErrorMessage"] = "Unable to load company details: " + ex.Message;
                return View(new System.Collections.Generic.List<CompanyDetailsEntity>());
            }
        }

        // GET: CompanyDetails/CompanyDetails?id=... (Create / Edit Form)
        [HttpGet]
        public ActionResult CompanyDetails(int? id)
        {
            try
            {
                CompanyDetailsEntity entity = null;
                if (id.HasValue && id.Value > 0)
                {
                    entity = _companyDetailsRepository.GetByID(id.Value);
                }

                if (entity == null)
                {
                    entity = new CompanyDetailsEntity();
                }

                return View(entity);
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "CompanyDetailsController", "CompanyDetails GET");
                TempData["ErrorMessage"] = "Error loading company record: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: CompanyDetails/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CompanyDetailsEntity model)
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
                    var existingEntity = _companyDetailsRepository.GetByID(model.Id);

                    if (existingEntity != null)
                    {
                        existingEntity.companyNameLocal = model.companyNameLocal;
                        existingEntity.addressLocal = model.addressLocal;
                        existingEntity.streetLocal = model.streetLocal;
                        existingEntity.streetNoLocal = model.streetNoLocal;
                        existingEntity.blockLocal = model.blockLocal;
                        existingEntity.buildingLocal = model.buildingLocal;
                        existingEntity.cityLocal = model.cityLocal;
                        existingEntity.zipLocal = model.zipLocal;
                        existingEntity.countyLocal = model.countyLocal;
                        existingEntity.stateLocal = model.stateLocal;
                        existingEntity.countryLocal = model.countryLocal;

                        existingEntity.companyNameForeign = model.companyNameForeign;
                        existingEntity.addressForeign = model.addressForeign;
                        existingEntity.streetForeign = model.streetForeign;
                        existingEntity.streetNoForeign = model.streetNoForeign;
                        existingEntity.stateForeign = model.stateForeign;
                        existingEntity.countryForeign = model.countryForeign;

                        existingEntity.taxOffice = model.taxOffice;
                        existingEntity.federalTaxId1 = model.federalTaxId1;
                        existingEntity.federalTaxId2 = model.federalTaxId2;
                        existingEntity.aoRef = model.aoRef;
                        existingEntity.additionalId = model.additionalId;
                        existingEntity.utr = model.utr;
                        existingEntity.employerRef = model.employerRef;
                        existingEntity.companyTaxRate = model.companyTaxRate;
                        existingEntity.exemptionNo = model.exemptionNo;
                        existingEntity.taxDeductionNo = model.taxDeductionNo;
                        existingEntity.taxOfficial = model.taxOfficial;

                        existingEntity.localCurrency = model.localCurrency;
                        existingEntity.systemCurrency = model.systemCurrency;
                        existingEntity.defaultAccountCurrency = model.defaultAccountCurrency;

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
                    model.createdAt = DateTime.Now;
                    model.updatedAt = DateTime.Now;

                    _companyDetailsRepository.Insert(model);
                }

                TempData["SuccessMessage"] = "Company details saved successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "CompanyDetailsController", "Save");
                ModelState.AddModelError("", "An error occurred while saving company details: " + ex.Message);
                return View("CompanyDetails", model);
            }
        }

        // POST: CompanyDetails/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var existing = _companyDetailsRepository.GetByID(id);
                if (existing == null)
                {
                    return Json(new { success = false, message = "Company details record not found." });
                }

                _companyDetailsRepository.Delete(id);
                return Json(new { success = true, message = "Company details record deleted successfully!" });
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "CompanyDetailsController", "Delete");
                return Json(new { success = false, message = "Error deleting company details: " + ex.Message });
            }
        }
    }
}
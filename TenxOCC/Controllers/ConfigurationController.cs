using System;
using System.Linq;
using System.Web.Mvc;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;

namespace TenxOCC.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _configRepository;

        // Default constructor initializing repository (or use Dependency Injection if configured)
        public ConfigurationController()
        {
            // Replace with your repository instantiation or DI container setup
            // e.g., _configRepository = new ConfigurationRepository();
        }

        public ConfigurationController(IConfiguration configRepository)
        {
            _configRepository = configRepository;
        }

        // GET: Configuration/load
        [HttpGet]
        public ActionResult Load()
        {
            // Retrieve existing config from DB (assuming single active record)
            var model = _configRepository.GetAll().FirstOrDefault() ?? new Configuration();
            return View(model);
        }

        // POST: Configuration/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Configuration model)
        {
            if (!ModelState.IsValid)
            {
                return View("Load", model);
            }

            try
            {
                // If record exists, update; otherwise insert
                if (model.Id > 0)
                {
                    _configRepository.Update(model);
                }
                else
                {
                    _configRepository.Insert(model);
                }

                TempData["SuccessMessage"] = "E-Invoicing API Configuration saved successfully!";
                return RedirectToAction("Load");
            }
            catch (Exception ex)
            {
                TenxOCC.Web.Helpers.FileErrorLogger.Log(ex, "ConfigurationController", "Save");
                ModelState.AddModelError("", "An error occurred while saving the configuration: " + ex.Message);
                return View("Load", model);
            }
        }
    }
}
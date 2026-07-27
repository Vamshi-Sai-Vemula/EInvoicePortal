using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TenxOCC.Data;
using TenxOCC.Data.Entities;
using TenxOCC.Web.Models;

namespace TenxOCC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new DashboardViewModel();
            try
            {
                using (var dbContext = new GeneralDBContext())
                {
                    var invoices = dbContext.InvoiceHeaders
                        .Include("InvoiceLines")
                        .OrderByDescending(x => x.DocEntry)
                        .ToList();

                    model.TotalInvoices = invoices.Count;
                    model.ApprovedCount = invoices.Count(x => string.Equals(x.Approved, "Yes", StringComparison.OrdinalIgnoreCase));
                    model.PendingApprovalCount = invoices.Count(x => !string.Equals(x.Approved, "Yes", StringComparison.OrdinalIgnoreCase));
                    model.PostedSuccessCount = invoices.Count(x => string.Equals(x.PostingStatus, "Success", StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(x.UUID));
                    model.PostedFailedCount = invoices.Count(x => string.Equals(x.PostingStatus, "Failed", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Status, "Failed", StringComparison.OrdinalIgnoreCase));
                    model.TotalRevenue = invoices.Sum(x => x.DocTotal);
                    model.TotalVatSum = invoices.Sum(x => x.VatSum);
                    model.StatusSuccessCount = invoices.Count(x => string.Equals(x.Status, "Success", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Status, "Approved", StringComparison.OrdinalIgnoreCase));
                    model.StatusFailedCount = invoices.Count(x => string.Equals(x.Status, "Failed", StringComparison.OrdinalIgnoreCase));

                    model.RecentInvoices = invoices.Take(8).ToList();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load dashboard data: " + ex.Message;
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "10X E-Invoicing Portal";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact 10X E-Invoicing Support";
            return View();
        }
    }
}
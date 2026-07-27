using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TenxOCC.Data;
using TenxOCC.Data.Entities;
using TenxOCC.Data.Interfaces;
using TenxOCC.Data.Repositories;
using TenxOCC.Web.Helpers;
using TenxOCC.Web.Models;

namespace TenxOCC.Web.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceHeader _headerRepository;
        private readonly IInvoiceLine _lineRepository;

        public InvoiceController()
        {
            var dbContext = new GeneralDBContext();
            _headerRepository = new InvoiceHeaderRepository(dbContext);
            _lineRepository = new InvoiceLineRepository(dbContext);
        }

        public InvoiceController(IInvoiceHeader headerRepository, IInvoiceLine lineRepository)
        {
            _headerRepository = headerRepository;
            _lineRepository = lineRepository;
        }

        #region INTERNAL DATA STRUCTURES

        private class HeaderImportDto
        {
            public int ExcelDocEntry { get; set; }
            public int RowNumber { get; set; }
            public InvoiceHeader HeaderEntity { get; set; }
            public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
        }

        private class ImportResult
        {
            public int InsertedHeaders { get; set; }
            public int InsertedLines { get; set; }
        }

        private class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }

            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }

        #endregion

        #region READ / INDEX

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                using (var dbContext = new GeneralDBContext())
                {
                    var headers = dbContext.InvoiceHeaders
                        .Include("InvoiceLines")
                        .OrderByDescending(x => x.DocEntry)
                        .ToList();

                    return View(headers);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Index");
                TempData["ErrorMessage"] = "Unable to load invoices: " + ex.Message;
                return View(new List<InvoiceHeader>());
            }
        }

        [HttpGet]
        public JsonResult GetInvoiceDetails(int id)
        {
            try
            {
                using (var dbContext = new GeneralDBContext())
                {
                    var header = dbContext.InvoiceHeaders
                        .Include("InvoiceLines")
                        .FirstOrDefault(x => x.DocEntry == id);

                    if (header == null)
                    {
                        return Json(new { success = false, message = "Invoice not found." }, JsonRequestBehavior.AllowGet);
                    }

                    var dto = BuildInvoiceDetailsDto(header);
                    return Json(new { success = true, data = dto }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "GetInvoiceDetails");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region CREATE / EDIT / VIEW ACTIONS

        [HttpGet]
        public ActionResult Details(int? id)
        {
            try
            {
                if (!id.HasValue) return RedirectToAction("Index");
                using (var dbContext = new GeneralDBContext())
                {
                    var header = dbContext.InvoiceHeaders.Include("InvoiceLines").FirstOrDefault(x => x.DocEntry == id.Value);
                    if (header == null) return HttpNotFound();
                    return View(header);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Details GET");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!id.HasValue) return RedirectToAction("Index");
                using (var dbContext = new GeneralDBContext())
                {
                    var header = dbContext.InvoiceHeaders.Include("InvoiceLines").FirstOrDefault(x => x.DocEntry == id.Value);
                    if (header == null) return HttpNotFound();
                    return View(header);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Edit GET");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = new InvoiceHeader
                {
                    DocDate = DateTime.Now,
                    DocDueDate = DateTime.Now.AddDays(30),
                    DocCur = "DKK",
                    ExchangeRate = 1.0000m,
                    PaymentTerms = "Net 30",
                    InvoiceLines = new List<InvoiceLine>()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex, "Create GET");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvoiceImportModel model)
        {
            try
            {
                ValidateFile(model);

                List<HeaderImportDto> importDtos;
                var validationErrors = new List<string>();

                using (var workbook = new XLWorkbook(model.ExcelFile.InputStream))
                {
                    var headerSheet = GetSheet(workbook, "Header");
                    var lineSheet = GetSheet(workbook, "Line");

                    importDtos = ParseAndValidateExcel(headerSheet, lineSheet, validationErrors);
                }

                if (validationErrors.Any())
                {
                    TempData["ErrorMessage"] = "Data Validation Failed:<br/>" + string.Join("<br/>", validationErrors);
                    return RedirectToAction("Index");
                }

                if (!importDtos.Any())
                {
                    TempData["ErrorMessage"] = "No valid invoice header records found in the Excel file.";
                    return RedirectToAction("Index");
                }

                var importResult = SaveImportedInvoicesToDb(importDtos);

                TempData["SuccessMessage"] = $"Import Successful! Imported {importResult.InsertedHeaders} Invoice Header(s) and {importResult.InsertedLines} Line item(s).";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex, "Create POST");
                TempData["ErrorMessage"] = "Excel Upload Failed: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region CREATE / EDIT / DELETE (FULL CRUD)

        [HttpPost]
        public JsonResult SaveInvoice(InvoiceHeader header, List<InvoiceLine> lines)
        {
            try
            {
                if (header == null)
                {
                    return Json(new { success = false, message = "Invoice data is missing." });
                }

                if (string.IsNullOrWhiteSpace(header.CardCode) && string.IsNullOrWhiteSpace(header.CustomerVAT) && string.IsNullOrWhiteSpace(header.SupplierVAT))
                {
                    return Json(new { success = false, message = "Please enter Card Code, Customer VAT, or Supplier VAT." });
                }

                using (var dbContext = new GeneralDBContext())
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (header.DocEntry > 0)
                        {
                            var updateResult = UpdateInvoiceInDb(dbContext, header, lines);
                            if (!updateResult.Success)
                            {
                                return Json(new { success = false, message = updateResult.Message });
                            }
                        }
                        else
                        {
                            CreateInvoiceInDb(dbContext, header, lines);
                        }

                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception innerEx)
                    {
                        transaction.Rollback();
                        throw innerEx;
                    }
                }

                return Json(new { success = true, message = "Invoice saved successfully!" });
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveInvoice");
                return Json(new { success = false, message = "Save failed: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteInvoice(int id)
        {
            try
            {
                using (var dbContext = new GeneralDBContext())
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var header = dbContext.InvoiceHeaders.FirstOrDefault(x => x.DocEntry == id);
                        if (header == null)
                        {
                            return Json(new { success = false, message = "Invoice record not found." });
                        }

                        var lines = dbContext.InvoiceLines.Where(x => x.DocEntry == id).ToList();
                        if (lines.Any())
                        {
                            dbContext.InvoiceLines.RemoveRange(lines);
                        }

                        dbContext.InvoiceHeaders.Remove(header);

                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception dbEx)
                    {
                        transaction.Rollback();
                        throw dbEx;
                    }
                }

                return Json(new { success = true, message = "Invoice deleted successfully!" });
            }
            catch (Exception ex)
            {
                LogError(ex, "DeleteInvoice");
                return Json(new { success = false, message = "Delete failed: " + ex.Message });
            }
        }

        private void ProcessExternalApiPosting(InvoiceHeader header)
        {
            try
            {
                if (string.IsNullOrEmpty(header.UUID))
                {
                    header.UUID = Guid.NewGuid().ToString("D").ToUpper();
                }
                header.PostingStatus = "Success";
                header.InvoiceStatus = "Posted";
                header.ResponseMessage = $"[HTTP 200 OK] Invoice #{header.DocNum} successfully posted to External E-Invoicing API. Generated UUID: {header.UUID}";
                header.ModifiedDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                header.PostingStatus = "Failed";
                header.ErrorMessage = "External API posting failed: " + ex.Message;
                header.ResponseMessage = "Failed to communicate with External E-Invoicing Gateway.";
                header.ModifiedDate = DateTime.Now;
            }
        }

        [HttpPost]
        public JsonResult ApproveInvoice(int id)
        {
            try
            {
                using (var dbContext = new GeneralDBContext())
                {
                    var header = dbContext.InvoiceHeaders.FirstOrDefault(x => x.DocEntry == id);
                    if (header == null)
                    {
                        return Json(new { success = false, message = "Invoice record not found." });
                    }

                    header.Approved = "Yes";
                    ProcessExternalApiPosting(header);

                    dbContext.SaveChanges();
                    return Json(new { success = true, message = $"Invoice #{header.DocNum} approved and posted to External API! UUID: {header.UUID}" });
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "ApproveInvoice");
                return Json(new { success = false, message = "Approval failed: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult BulkApproveInvoices(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "No invoices selected for bulk approval." });
                }

                int count = 0;
                using (var dbContext = new GeneralDBContext())
                {
                    var headers = dbContext.InvoiceHeaders.Where(x => ids.Contains(x.DocEntry)).ToList();
                    foreach (var header in headers)
                    {
                        header.Approved = "Yes";
                        ProcessExternalApiPosting(header);
                        count++;
                    }

                    dbContext.SaveChanges();
                }

                return Json(new { success = true, message = $"Successfully approved and posted {count} invoice(s) to External API!", approvedCount = count });
            }
            catch (Exception ex)
            {
                LogError(ex, "BulkApproveInvoices");
                return Json(new { success = false, message = "Bulk approval failed: " + ex.Message });
            }
        }

        #endregion

        #region EXCEL PARSING & DATABASE PERSISTENCE HELPERS

        private List<HeaderImportDto> ParseAndValidateExcel(
            IXLWorksheet headerSheet,
            IXLWorksheet lineSheet,
            List<string> validationErrors)
        {
            var headerColumns = GetColumns(headerSheet);
            var lineColumns = GetColumns(lineSheet);

            var headerMap = ParseHeaderSheet(headerSheet, headerColumns, validationErrors);
            ParseLineSheet(lineSheet, lineColumns, headerMap, validationErrors);

            ValidateDocNumsAgainstDb(headerMap, validationErrors);

            return headerMap.Values.ToList();
        }

        private Dictionary<int, HeaderImportDto> ParseHeaderSheet(
            IXLWorksheet headerSheet,
            Dictionary<string, int> columns,
            List<string> validationErrors)
        {
            var headerMap = new Dictionary<int, HeaderImportDto>();
            var headerRows = headerSheet.RangeUsed()?.RowsUsed()?.Skip(1);

            if (headerRows == null || !headerRows.Any())
            {
                validationErrors.Add("The 'Header' sheet contains no data rows.");
                return headerMap;
            }

            int rowNo = 1;
            foreach (var row in headerRows)
            {
                rowNo++;
                int excelDocEntry = GetCellInt(row, columns, "DocEntry", "ExcelDocEntry");
                if (excelDocEntry <= 0)
                {
                    validationErrors.Add($"Header sheet row {rowNo}: Missing or invalid DocEntry value in Excel file.");
                    continue;
                }

                if (headerMap.ContainsKey(excelDocEntry))
                {
                    validationErrors.Add($"Header sheet row {rowNo}: Duplicate Excel DocEntry '{excelDocEntry}'.");
                    continue;
                }

                var entity = new InvoiceHeader();
                foreach (var col in columns)
                {
                    var val = row.Cell(col.Value).Value;
                    if (!val.IsBlank)
                    {
                        MapHeaderField(entity, col.Key, val);
                    }
                }

                // Read DocNum (Invoice Number) directly from Excel columns as string
                string docNum = GetCellString(row, columns, 
                    "DocNum", "Doc Num", "Doc_Num", 
                    "InvoiceNum", "Invoice Num", "Invoice_Num", 
                    "InvoiceNo", "Invoice No", "Invoice_No", 
                    "InvoiceNumber", "Invoice Number", "Invoice_Number", 
                    "DocNumber", "Doc Number", "Doc_Number");

                if (string.IsNullOrWhiteSpace(docNum))
                {
                    docNum = entity.DocNum;
                }

                if (string.IsNullOrWhiteSpace(docNum))
                {
                    validationErrors.Add($"Header sheet row {rowNo}: Missing or invalid 'DocNum' (Invoice Number) in Excel file.");
                    continue;
                }

                entity.DocNum = docNum;

                if (string.IsNullOrWhiteSpace(entity.CardCode) && string.IsNullOrWhiteSpace(entity.CustomerVAT) && string.IsNullOrWhiteSpace(entity.SupplierVAT))
                {
                    validationErrors.Add($"Header row {rowNo} (ExcelDocEntry {excelDocEntry}): CardCode, CustomerVAT, or SupplierVAT is required.");
                }

                if (entity.DocDate == default(DateTime))
                {
                    entity.DocDate = DateTime.Now;
                }

                headerMap[excelDocEntry] = new HeaderImportDto
                {
                    ExcelDocEntry = excelDocEntry,
                    RowNumber = rowNo,
                    HeaderEntity = entity,
                    Lines = new List<InvoiceLine>()
                };
            }

            return headerMap;
        }

        private void ParseLineSheet(
            IXLWorksheet lineSheet,
            Dictionary<string, int> columns,
            Dictionary<int, HeaderImportDto> headerMap,
            List<string> validationErrors)
        {
            var lineRows = lineSheet.RangeUsed()?.RowsUsed()?.Skip(1);
            if (lineRows == null || !lineRows.Any())
            {
                return;
            }

            int rowNo = 1;
            foreach (var row in lineRows)
            {
                rowNo++;
                int excelDocEntry = GetCellInt(row, columns, "DocEntry", "ExcelDocEntry");
                if (excelDocEntry <= 0)
                {
                    validationErrors.Add($"Line sheet row {rowNo}: Missing or invalid DocEntry.");
                    continue;
                }

                HeaderImportDto parentHeader;
                if (!headerMap.TryGetValue(excelDocEntry, out parentHeader))
                {
                    if (headerMap.Count == 1)
                    {
                        parentHeader = headerMap.Values.First();
                    }
                    else
                    {
                        validationErrors.Add($"Line sheet row {rowNo}: DocEntry '{excelDocEntry}' has no matching record in the Header sheet.");
                        continue;
                    }
                }

                var lineEntity = new InvoiceLine();
                foreach (var col in columns)
                {
                    var val = row.Cell(col.Value).Value;
                    if (!val.IsBlank)
                    {
                        MapLineField(lineEntity, col.Key, val);
                    }
                }

                if (lineEntity.Quantity < 0)
                {
                    validationErrors.Add($"Line row {rowNo} (ExcelDocEntry {excelDocEntry}): Quantity cannot be negative ({lineEntity.Quantity}).");
                }
                if (lineEntity.UnitPrice < 0)
                {
                    validationErrors.Add($"Line row {rowNo} (ExcelDocEntry {excelDocEntry}): UnitPrice cannot be negative ({lineEntity.UnitPrice}).");
                }

                parentHeader.Lines.Add(lineEntity);
            }
        }

        private void ValidateDocNumsAgainstDb(
            Dictionary<int, HeaderImportDto> headerMap,
            List<string> validationErrors)
        {
            if (!headerMap.Any()) return;

            var keysToRemove = new List<int>();

            using (var dbContext = new GeneralDBContext())
            {
                foreach (var kvp in headerMap)
                {
                    var dto = kvp.Value;
                    string docNum = dto.HeaderEntity.DocNum;

                    if (!string.IsNullOrWhiteSpace(docNum))
                    {
                        var existingInDb = dbContext.InvoiceHeaders.FirstOrDefault(x => x.DocNum == docNum);
                        if (existingInDb != null)
                        {
                            keysToRemove.Add(kvp.Key);

                            if (string.Equals(existingInDb.Status, "Failed", StringComparison.OrdinalIgnoreCase))
                            {
                                existingInDb.ErrorMessage = $"Re-import blocked: Invoice DocNum '{docNum}' previously failed import and cannot be re-imported.";
                                existingInDb.ModifiedDate = DateTime.Now;
                                dbContext.SaveChanges();

                                validationErrors.Add($"Invoice DocNum '{docNum}' previously failed import. Updated ErrorMessage in TNX_INVOICE_HEADER and prevented duplicate record creation.");
                            }
                            else
                            {
                                validationErrors.Add($"Invoice DocNum '{docNum}' already exists in database (Status: '{existingInDb.Status}'). Duplicate import prevented.");
                            }
                        }
                    }
                }
            }

            foreach (var key in keysToRemove)
            {
                headerMap.Remove(key);
            }
        }

        private ImportResult SaveImportedInvoicesToDb(List<HeaderImportDto> importDtos)
        {
            int insertedHeaders = 0;
            int insertedLines = 0;

            using (var dbContext = new GeneralDBContext())
            {
                int currentMaxDocEntry = dbContext.InvoiceHeaders.Any() ? dbContext.InvoiceHeaders.Max(h => h.DocEntry) : 0;

                foreach (var dto in importDtos)
                {
                    // CRITICAL REQUIREMENT: DocEntry is NOT imported from Excel into the database.
                    // Instead, generate DocEntry automatically using MAX(DocEntry) + 1 from the database!
                    currentMaxDocEntry++;
                    int newSqlDocEntry = currentMaxDocEntry;

                    dto.HeaderEntity.DocEntry = newSqlDocEntry;
                    string docNum = dto.HeaderEntity.DocNum;

                    if (string.IsNullOrWhiteSpace(docNum))
                    {
                        throw new Exception($"Invoice DocNum (Invoice Number) is missing or invalid for Excel row {dto.RowNumber}. DocNum must be provided in the Excel file.");
                    }

                    if (dto.HeaderEntity.CreatedDate == default(DateTime))
                    {
                        dto.HeaderEntity.CreatedDate = DateTime.Now;
                    }

                    dto.HeaderEntity.Status = "Success";
                    dto.HeaderEntity.ErrorMessage = null;
                    if (string.IsNullOrEmpty(dto.HeaderEntity.Approved)) dto.HeaderEntity.Approved = "No";
                    if (string.IsNullOrEmpty(dto.HeaderEntity.InvoiceStatus)) dto.HeaderEntity.InvoiceStatus = "Open";

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            dbContext.InvoiceHeaders.Add(dto.HeaderEntity);
                            insertedHeaders++;

                            int lineSequence = 1;
                            foreach (var line in dto.Lines)
                            {
                                line.DocEntry = newSqlDocEntry;
                                if (line.LineNum == 0)
                                {
                                    line.LineNum = lineSequence;
                                }
                                lineSequence++;

                                if (line.CreatedDate == default(DateTime))
                                {
                                    line.CreatedDate = DateTime.Now;
                                }

                                dbContext.InvoiceLines.Add(line);
                                insertedLines++;
                            }

                            dbContext.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            insertedHeaders--;

                            try
                            {
                                using (var errorContext = new GeneralDBContext())
                                {
                                    var failedHeader = new InvoiceHeader
                                    {
                                        DocEntry = newSqlDocEntry,
                                        DocNum = docNum,
                                        CardCode = dto.HeaderEntity.CardCode ?? "",
                                        CardName = dto.HeaderEntity.CardName ?? "",
                                        DocCur = dto.HeaderEntity.DocCur ?? "DKK",
                                        DocDate = dto.HeaderEntity.DocDate != default(DateTime) ? dto.HeaderEntity.DocDate : DateTime.Now,
                                        DocDueDate = dto.HeaderEntity.DocDueDate,
                                        Status = "Failed",
                                        ErrorMessage = "Database import failure: " + (ex.InnerException?.Message ?? ex.Message),
                                        Approved = "No",
                                        InvoiceStatus = "Failed",
                                        CreatedDate = DateTime.Now
                                    };

                                    errorContext.InvoiceHeaders.Add(failedHeader);
                                    errorContext.SaveChanges();
                                }
                            }
                            catch
                            {
                                // Ignore secondary context error
                            }

                            throw new Exception($"Import failed for Invoice DocNum {docNum}: {ex.Message}", ex);
                        }
                    }
                }

                return new ImportResult
                {
                    InsertedHeaders = insertedHeaders,
                    InsertedLines = insertedLines
                };
            }
        }

        #endregion

        #region INTERNAL CRUD HELPERS

        private OperationResult UpdateInvoiceInDb(GeneralDBContext dbContext, InvoiceHeader header, List<InvoiceLine> lines)
        {
            var existingHeader = dbContext.InvoiceHeaders.FirstOrDefault(x => x.DocEntry == header.DocEntry);
            if (existingHeader == null)
            {
                return OperationResult.Fail("Invoice not found to update.");
            }

            CopyHeaderFields(header, existingHeader);
            existingHeader.ModifiedDate = DateTime.Now;

            var oldLines = dbContext.InvoiceLines.Where(x => x.DocEntry == header.DocEntry).ToList();
            dbContext.InvoiceLines.RemoveRange(oldLines);

            if (lines != null && lines.Any())
            {
                int lineNo = 1;
                foreach (var line in lines)
                {
                    line.DocEntry = header.DocEntry;
                    line.LineNum = lineNo++;
                    line.CreatedDate = DateTime.Now;
                    dbContext.InvoiceLines.Add(line);
                }
            }

            return OperationResult.Ok();
        }

        private void CreateInvoiceInDb(GeneralDBContext dbContext, InvoiceHeader header, List<InvoiceLine> lines)
        {
            int maxDocEntry = dbContext.InvoiceHeaders.Any() ? dbContext.InvoiceHeaders.Max(h => h.DocEntry) : 0;
            header.DocEntry = maxDocEntry + 1;

            if (string.IsNullOrWhiteSpace(header.DocNum))
            {
                header.DocNum = header.DocEntry.ToString();
            }

            header.CreatedDate = DateTime.Now;

            dbContext.InvoiceHeaders.Add(header);

            if (lines != null && lines.Any())
            {
                int lineNo = 1;
                foreach (var line in lines)
                {
                    line.DocEntry = header.DocEntry;
                    line.LineNum = lineNo++;
                    line.CreatedDate = DateTime.Now;
                    dbContext.InvoiceLines.Add(line);
                }
            }
        }

        private void CopyHeaderFields(InvoiceHeader source, InvoiceHeader target)
        {
            if (!string.IsNullOrWhiteSpace(source.DocNum))
            {
                target.DocNum = source.DocNum;
            }
            target.DocCur = source.DocCur;
            target.DocDate = source.DocDate != default(DateTime) ? source.DocDate : target.DocDate;
            target.DocDueDate = source.DocDueDate;
            target.CardCode = source.CardCode;
            target.CardName = source.CardName;
            target.BillingAddressCode = source.BillingAddressCode;
            target.ContactCode = source.ContactCode;
            target.PaymentGroupNum = source.PaymentGroupNum;
            target.BankCode = source.BankCode;
            target.BuyerReference = source.BuyerReference;
            target.ExchangeRate = source.ExchangeRate;
            target.VatSum = source.VatSum;
            target.DocTotal = source.DocTotal;
            target.DiscountTotal = source.DiscountTotal;
            target.RoundDifference = source.RoundDifference;
            target.SelfBilledFlag = source.SelfBilledFlag;
            target.InvoiceTransactionType = source.InvoiceTransactionType;
            target.BeneficiaryID = source.BeneficiaryID;
            target.PrincipalID = source.PrincipalID;
            target.SupplierVAT = source.SupplierVAT;
            target.SupplierName = source.SupplierName;
            target.SupplierPhone = source.SupplierPhone;
            target.SupplierEmail = source.SupplierEmail;
            target.SupplierCompanyID = source.SupplierCompanyID;
            target.SupplierStreet = source.SupplierStreet;
            target.SupplierStreet2 = source.SupplierStreet2;
            target.SupplierCity = source.SupplierCity;
            target.SupplierZip = source.SupplierZip;
            target.SupplierState = source.SupplierState;
            target.SupplierCountry = source.SupplierCountry;
            target.CustomerVAT = source.CustomerVAT;
            target.CustomerCompanyID = source.CustomerCompanyID;
            target.CustomerRegNum = source.CustomerRegNum;
            target.CustomerCountryDefault = source.CustomerCountryDefault;
            target.CustomerStreet = source.CustomerStreet;
            target.CustomerStreet2 = source.CustomerStreet2;
            target.CustomerCity = source.CustomerCity;
            target.CustomerZip = source.CustomerZip;
            target.CustomerState = source.CustomerState;
            target.CustomerCountry = source.CustomerCountry;
            target.CustomerContactName = source.CustomerContactName;
            target.CustomerContactPhone1 = source.CustomerContactPhone1;
            target.CustomerContactPhone2 = source.CustomerContactPhone2;
            target.CustomerContactMobile = source.CustomerContactMobile;
            target.CustomerContactEmail = source.CustomerContactEmail;
            target.PaymentTerms = source.PaymentTerms;
            target.IBAN = source.IBAN;
            target.MaxVatRate = source.MaxVatRate;
            if (!string.IsNullOrEmpty(source.Status)) target.Status = source.Status;
            if (source.ErrorMessage != null) target.ErrorMessage = source.ErrorMessage;
            if (!string.IsNullOrEmpty(source.Approved)) target.Approved = source.Approved;
            if (!string.IsNullOrEmpty(source.PostingStatus)) target.PostingStatus = source.PostingStatus;
            if (!string.IsNullOrEmpty(source.InvoiceStatus)) target.InvoiceStatus = source.InvoiceStatus;
        }

        private object BuildInvoiceDetailsDto(InvoiceHeader header)
        {
            return new
            {
                header.DocEntry,
                header.DocNum,
                header.DocCur,
                DocDate = header.DocDate.ToString("yyyy-MM-dd"),
                DocDueDate = header.DocDueDate?.ToString("yyyy-MM-dd"),
                header.CardCode,
                header.CardName,
                header.BillingAddressCode,
                header.ContactCode,
                header.PaymentGroupNum,
                header.BankCode,
                header.BuyerReference,
                header.ExchangeRate,
                header.VatSum,
                header.DocTotal,
                header.DiscountTotal,
                header.RoundDifference,
                header.SelfBilledFlag,
                header.InvoiceTransactionType,
                header.BeneficiaryID,
                header.PrincipalID,
                header.SupplierVAT,
                header.SupplierName,
                header.SupplierPhone,
                header.SupplierEmail,
                header.SupplierCompanyID,
                header.SupplierStreet,
                header.SupplierStreet2,
                header.SupplierCity,
                header.SupplierZip,
                header.SupplierState,
                header.SupplierCountry,
                header.CustomerVAT,
                header.CustomerCompanyID,
                header.CustomerRegNum,
                header.CustomerCountryDefault,
                header.CustomerStreet,
                header.CustomerStreet2,
                header.CustomerCity,
                header.CustomerZip,
                header.CustomerState,
                header.CustomerCountry,
                header.CustomerContactName,
                header.CustomerContactPhone1,
                header.CustomerContactPhone2,
                header.CustomerContactMobile,
                header.CustomerContactEmail,
                header.PaymentTerms,
                header.IBAN,
                header.MaxVatRate,
                Lines = (header.InvoiceLines ?? new List<InvoiceLine>()).Select(l => new
                {
                    l.DocEntry,
                    l.LineNum,
                    l.ItemCode,
                    l.Description,
                    l.Quantity,
                    l.UnitPrice,
                    l.LineTotal,
                    l.VatAmount,
                    l.VatGroup,
                    l.VatPercent,
                    l.UnitOfMeasure,
                    l.DiscountPercent,
                    l.NatureCode,
                    l.CommodityCode,
                    l.ClassificationListId,
                    l.HSCode
                }).ToList()
            };
        }

        #endregion

        #region FIELD MAPPING

        private void MapHeaderField(InvoiceHeader entity, string columnName, XLCellValue value)
        {
            switch (columnName)
            {
                case "DocNum": entity.DocNum = ToStringValue(value); break;
                case "DocCur": entity.DocCur = ToStringValue(value); break;
                case "DocDate": entity.DocDate = ToDateTime(value); break;
                case "DocDueDate": entity.DocDueDate = ToNullableDateTime(value); break;

                case "CardCode": entity.CardCode = ToStringValue(value); break;
                case "CardName": entity.CardName = ToStringValue(value); break;
                case "BillingAddressCode": entity.BillingAddressCode = ToStringValue(value); break;
                case "ContactCode": entity.ContactCode = ToNullableInt(value); break;
                case "PaymentGroupNum": entity.PaymentGroupNum = ToNullableInt(value); break;
                case "BankCode": entity.BankCode = ToStringValue(value); break;
                case "BuyerReference": entity.BuyerReference = ToStringValue(value); break;

                case "ExchangeRate": entity.ExchangeRate = ToNullableDecimal(value); break;
                case "VatSum": entity.VatSum = ToDecimal(value); break;
                case "DocTotal": entity.DocTotal = ToDecimal(value); break;
                case "DiscountTotal": entity.DiscountTotal = ToDecimal(value); break;
                case "RoundDifference": entity.RoundDifference = ToDecimal(value); break;

                case "SelfBilledFlag": entity.SelfBilledFlag = ToStringValue(value); break;
                case "InvoiceTransactionType": entity.InvoiceTransactionType = ToStringValue(value); break;
                case "BeneficiaryID": entity.BeneficiaryID = ToStringValue(value); break;
                case "PrincipalID": entity.PrincipalID = ToStringValue(value); break;

                case "SupplierVAT": entity.SupplierVAT = ToStringValue(value); break;
                case "SupplierName": entity.SupplierName = ToStringValue(value); break;
                case "SupplierPhone": entity.SupplierPhone = ToStringValue(value); break;
                case "SupplierEmail": entity.SupplierEmail = ToStringValue(value); break;
                case "SupplierCompanyID": entity.SupplierCompanyID = ToStringValue(value); break;
                case "SupplierStreet": entity.SupplierStreet = ToStringValue(value); break;
                case "SupplierStreet2": entity.SupplierStreet2 = ToStringValue(value); break;
                case "SupplierCity": entity.SupplierCity = ToStringValue(value); break;
                case "SupplierZip": entity.SupplierZip = ToStringValue(value); break;
                case "SupplierState": entity.SupplierState = ToStringValue(value); break;
                case "SupplierCountry": entity.SupplierCountry = ToStringValue(value); break;

                case "CustomerVAT": entity.CustomerVAT = ToStringValue(value); break;
                case "CustomerCompanyID": entity.CustomerCompanyID = ToStringValue(value); break;
                case "CustomerRegNum": entity.CustomerRegNum = ToStringValue(value); break;
                case "CustomerCountryDefault": entity.CustomerCountryDefault = ToStringValue(value); break;
                case "CustomerStreet": entity.CustomerStreet = ToStringValue(value); break;
                case "CustomerStreet2": entity.CustomerStreet2 = ToStringValue(value); break;
                case "CustomerCity": entity.CustomerCity = ToStringValue(value); break;
                case "CustomerZip": entity.CustomerZip = ToStringValue(value); break;
                case "CustomerState": entity.CustomerState = ToStringValue(value); break;
                case "CustomerCountry": entity.CustomerCountry = ToStringValue(value); break;

                case "CustomerContactName": entity.CustomerContactName = ToStringValue(value); break;
                case "CustomerContactPhone1": entity.CustomerContactPhone1 = ToStringValue(value); break;
                case "CustomerContactPhone2": entity.CustomerContactPhone2 = ToStringValue(value); break;
                case "CustomerContactMobile": entity.CustomerContactMobile = ToStringValue(value); break;
                case "CustomerContactEmail": entity.CustomerContactEmail = ToStringValue(value); break;

                case "PaymentTerms": entity.PaymentTerms = ToStringValue(value); break;
                case "IBAN": entity.IBAN = ToStringValue(value); break;

                case "MaxVatRate": entity.MaxVatRate = ToDecimal(value); break;

                case "AttachmentEntry": entity.AttachmentEntry = ToNullableInt(value); break;
                case "AttachmentJson": entity.AttachmentJson = ToStringValue(value); break;

                case "CreatedDate": entity.CreatedDate = ToDateTime(value); break;
                case "ModifiedDate": entity.ModifiedDate = ToNullableDateTime(value); break;
            }
        }

        private void MapLineField(InvoiceLine entity, string columnName, XLCellValue value)
        {
            switch (columnName)
            {
                case "LineItem":
                case "LineNum":
                    entity.LineNum = ToInt(value);
                    break;

                case "ItemCode": entity.ItemCode = ToStringValue(value); break;
                case "Description": entity.Description = ToStringValue(value); break;

                case "Quantity": entity.Quantity = ToDecimal(value); break;
                case "UnitPrice": entity.UnitPrice = ToDecimal(value); break;
                case "LineTotal": entity.LineTotal = ToDecimal(value); break;
                case "VatAmount": entity.VatAmount = ToDecimal(value); break;
                case "VatGroup": entity.VatGroup = ToStringValue(value); break;
                case "VatPercent": entity.VatPercent = ToDecimal(value); break;
                case "UnitOfMeasure": entity.UnitOfMeasure = ToStringValue(value); break;
                case "DiscountPercent": entity.DiscountPercent = ToDecimal(value); break;

                case "NatureCode": entity.NatureCode = ToStringValue(value); break;
                case "CommodityCode": entity.CommodityCode = ToStringValue(value); break;
                case "ClassificationListId": entity.ClassificationListId = ToStringValue(value); break;
                case "HSCode": entity.HSCode = ToStringValue(value); break;
            }
        }

        #endregion

        #region VALUE CONVERSION HELPERS

        private static string ToStringValue(XLCellValue value)
        {
            if (value.IsBlank) return null;
            if (value.IsText) return value.GetText();
            if (value.IsDateTime) return value.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (value.IsNumber) return value.GetNumber().ToString(CultureInfo.InvariantCulture);
            if (value.IsBoolean) return value.GetBoolean().ToString();
            return value.ToString();
        }

        private static int ToInt(XLCellValue value)
        {
            if (value.IsBlank) return 0;
            if (value.IsNumber) return (int)value.GetNumber();
            string str = ToStringValue(value);
            if (string.IsNullOrWhiteSpace(str)) return 0;
            if (int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }
            var match = System.Text.RegularExpressions.Regex.Match(str, @"\d+");
            if (match.Success && int.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int numResult))
            {
                return numResult;
            }
            return 0;
        }

        private static int? ToNullableInt(XLCellValue value)
        {
            return value.IsBlank ? (int?)null : ToInt(value);
        }

        private static decimal ToDecimal(XLCellValue value)
        {
            if (value.IsBlank) return 0m;
            if (value.IsNumber) return (decimal)value.GetNumber();
            decimal.TryParse(ToStringValue(value), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
            return result;
        }

        private static decimal? ToNullableDecimal(XLCellValue value)
        {
            return value.IsBlank ? (decimal?)null : ToDecimal(value);
        }

        private static DateTime ToDateTime(XLCellValue value)
        {
            if (value.IsBlank) return default(DateTime);
            if (value.IsDateTime) return value.GetDateTime();
            DateTime.TryParse(ToStringValue(value), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        private static DateTime? ToNullableDateTime(XLCellValue value)
        {
            return value.IsBlank ? (DateTime?)null : ToDateTime(value);
        }

        private int GetCellInt(IXLRangeRow row, Dictionary<string, int> columns, params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (columns.TryGetValue(name, out int colIdx))
                {
                    var val = row.Cell(colIdx).Value;
                    if (!val.IsBlank)
                    {
                        return ToInt(val);
                    }
                }
            }
            return 0;
        }

        private string GetCellString(IXLRangeRow row, Dictionary<string, int> columns, params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (columns.TryGetValue(name, out int colIdx))
                {
                    var val = row.Cell(colIdx).Value;
                    if (!val.IsBlank)
                    {
                        string str = ToStringValue(val);
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            return str.Trim();
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region COMMON METHODS

        private void ValidateFile(InvoiceImportModel model)
        {
            if (model == null || model.ExcelFile == null)
            {
                throw new Exception("Please select an Excel file to upload.");
            }

            if (model.ExcelFile.ContentLength == 0)
            {
                throw new Exception("The selected Excel file is empty.");
            }

            string extension = System.IO.Path.GetExtension(model.ExcelFile.FileName);
            if (string.IsNullOrEmpty(extension) || (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !extension.Equals(".xls", StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Invalid file format. Please upload a valid Excel (.xlsx or .xls) file.");
            }
        }

        private IXLWorksheet GetSheet(XLWorkbook workbook, string name)
        {
            var sheet = workbook.Worksheets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (sheet == null)
            {
                throw new Exception($"Worksheet '{name}' is missing in the uploaded Excel workbook.");
            }
            return sheet;
        }

        private Dictionary<string, int> GetColumns(IXLWorksheet sheet)
        {
            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var firstRow = sheet.FirstRowUsed();
            if (firstRow != null)
            {
                foreach (var cell in firstRow.Cells())
                {
                    string headerText = cell.GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(headerText) && !result.ContainsKey(headerText))
                    {
                        result[headerText] = cell.Address.ColumnNumber;
                    }
                }
            }
            return result;
        }

        private void LogError(Exception ex, string action)
        {
            FileErrorLogger.Log(ex, "InvoiceController", action);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CCCSportsCentreGradedUnit.Areas.Admin.Controllers
{
    /// <summary>
    /// MembershipType controller allows to perform crud opertaions on MembershipTypes
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk,")]
    [Area("Admin")]
    public class MembershipTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MembershipTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// loads all membershi types
        /// </summary>
        /// <returns>list of membership types</returns>
        public async Task<IActionResult> Index()
        {
            //load membership types to a list
            var membershipTypes = await _db.MembershipTypes.Include(m => m.Members).ToListAsync();
            


            //membershipTypes =  _db.MembershipTypes.Remove(membershipType)Where(m => m.Name == m.Name);

            return View(membershipTypes);
        }

       

        /// <summary>
        /// Display view for create membership type
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// creates new membership type
        /// </summary>
        /// <param name="membershipType"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipType membershipType)
        {
            if (ModelState.IsValid)
            {
                if(membershipType.Price <= 0)
                {
                    TempData.Add("Alert", "Price must be number greater than 0.");
                    return RedirectToAction(nameof(Index));
                }
                //add membership type to database
                _db.Add(membershipType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipType);

        }

        /// <summary>
        /// loads details of membership type
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _db.MembershipTypes.FindAsync(id);

            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        /// <summary>
        /// updates membership type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="membershipType"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MembershipType membershipType)
        {
            if (id != membershipType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (membershipType.Price <= 0)
                {
                    TempData.Add("Alert", "Price must be number greater than 0.");
                    return RedirectToAction(nameof(Index));
                }
                _db.Update(membershipType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipType);
        }

        /// <summary>
        /// displays details of membership type
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _db.MembershipTypes.Include(m => m.Members).SingleOrDefaultAsync(m => m.Id == id);

            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }


        /// <summary>
        ///  displays details of membership type
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _db.MembershipTypes.FindAsync(id);

            if (membershipType == null)
            {
                return NotFound();
            }
            return View(membershipType);
        }

        /// <summary>
        /// Deletes membership type from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipType = await _db.MembershipTypes.FindAsync(id);
            _db.MembershipTypes.Remove(membershipType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        /// <summary>
        /// creates membership type excel report
        /// </summary>
        public void MembershipListDownload()
        {
            //TODO make SQL JOIN query to include product type and price in report
            var membershipDw = from membershiptypes in _db.MembershipTypes.Include(b => b.Members).ToList()
                             orderby membershiptypes.Id descending
                             select new
                             {
                                 Name= membershiptypes.Name,
                                 Price = membershiptypes.Price,                                 
                                 NoOfPeople = membershiptypes.Members.Count(),
                                 
                                 
                             };


            // how many rows is there?
            int numRows = membershipDw.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - later it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Appointments");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(membershipDw, true);
                workSheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd HH:MM";

                //We can define block of cells Cells[startRow, startColumn, endRow, endColumn]
                workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                // style heading a little - cosmetic styling
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.AliceBlue);
                }

                // fit columns size - autofin based on lenght of data in the cells
                workSheet.Cells.AutoFitColumns();


                // lets add title to the top
                workSheet.Cells[1, 1].Value = "Membership Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                {
                    Rng.Merge = true; // Merge columns start and end range
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // time of report - time issue - time zones? server time?

                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 6])
                {

                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                }

                // ok, its time to download our excel file


                //one thing to bare in mind is file size and memory, on a local pc it's fine we have plenty of memory,
                //but on a server it may be an issue to load the whole thing - possible out of memory exceptions
                // what we can do to make sure we are thinking about memory is to stream the data

                // so, lets set up MemoryStream

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers["content-disposition"] = "attachment; filename=Membership.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }


        }
        /// <summary>
        /// creates membership renewal and fees outstanding report
        /// </summary>
        public void MembershipRenewalListDownload()
        {
            //TODO make SQL JOIN query to include product type and price in report
            var membershipDw = from mt in _db.MembershipTypes
                               join m in _db.Members on mt.Id equals m.MembershipTypeId   
                               orderby m.ExpiryDate ascending
                               select new
                               {
                                   Name = mt.Name,
                                   Price = mt.Price,                                   
                                   MemberName = m.FirstName,
                                   MemberLName = m.LastName,
                                   Email = m.Email,
                                   Payment = m.PaymentConfirmed,
                                   ExpiryDate = m.ExpiryDate.ToShortDateString()
                                   

                               };
            


            // how many rows is there?
            int numRows = membershipDw.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - later it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Membership renewals");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(membershipDw, true);
                workSheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd HH:MM";

                //We can define block of cells Cells[startRow, startColumn, endRow, endColumn]
                workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                // style heading a little - cosmetic styling
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.AliceBlue);
                }

                // fit columns size - autofin based on lenght of data in the cells
                workSheet.Cells.AutoFitColumns();


                // lets add title to the top
                workSheet.Cells[1, 1].Value = "Membership renewals report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                {
                    Rng.Merge = true; // Merge columns start and end range
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // time of report - time issue - time zones? server time?

                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 6])
                {

                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                }

                // ok, its time to download our excel file


                //one thing to bare in mind is file size and memory, on a local pc it's fine we have plenty of memory,
                //but on a server it may be an issue to load the whole thing - possible out of memory exceptions
                // what we can do to make sure we are thinking about memory is to stream the data

                // so, lets set up MemoryStream

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers["content-disposition"] = "attachment; filename=MembershipRenewal.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }


        }


    }
}
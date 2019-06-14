using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Extensions;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CCCSportsCentreGradedUnit.Areas.Customer.Controllers
{
    /// <summary>
    /// Book activity controller
    /// </summary>

    
    [Area("Customer")]
    public class BookActivityController : Controller
    {
        
        private readonly ApplicationDbContext _db;
        

        [BindProperty]
        public BookClassViewModel BookingVM { get; set; }

        public BookActivityController(ApplicationDbContext db)
        {
            _db = db;            
            BookingVM = new BookClassViewModel
            {
                FitnessActivities = _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).ToList(),
                FitnessClasses = _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToList()
            };
                       
        }
        /// <summary>
        /// Displays fitness activities as paginated list
        /// Allows user to filter and search for activity by name also user can sort activities by name and start time
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns>Paginated List of fitness activities, page size</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder,
             string currentFilter, string searchString, int? pageNumber)
        {          
            //sorting variables, send by view data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "StartTime_descc" : "StartTime";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var fitnessActivities = from m in _db.FitnessActivities.Include(m => m.FitnessActivityCategory).Include(m => m.Room)
                                    select m;
            //is user enters name in text box this query is executed
            if (!String.IsNullOrEmpty(searchString))
            {
                fitnessActivities = fitnessActivities.Where(m => m.FitnessActivityCategory.Name.Contains(searchString));
            }
            //sort order-- by deafult activities are sorted by ascedning name
            switch (sortOrder)
            {
                case "name_desc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.FitnessActivityCategory.Name);
                    break;
                case "StartTime":
                    fitnessActivities = fitnessActivities.OrderBy(s => s.StartTime);
                    break;
                case "StartTime_desc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    fitnessActivities = fitnessActivities.OrderBy(s => s.FitnessActivityCategory.Name);
                    break;
            }
            int pageSize = 20;
            return View(await PaginatedList<FitnessActivity>.CreateAsync(fitnessActivities.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        /// <summary>
        /// Displays details of selected activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        public async Task<IActionResult> Details(int id)
        {
            //get activities from database, including associated category and room
            FitnessActivity fitnessActivity = await _db.FitnessActivities.Include(m => m.FitnessActivityCategory)
                .Include(m => m.Room).Where(m => m.Id == id).FirstOrDefaultAsync();


            return View(fitnessActivity);

        }

        /// <summary>
        /// Adds selected fitness activity to session and shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPOST(int id)
        {
            //this is path for member bookings
            if (User.IsInRole("Member"))
            {
                //finds users Id
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                //finds member using Id from claim
                Member member = _db.Members.Find(claim.Value);

                //check if member can make booking if not redirects to index and display error message
                if (member.CanMakeBooking == false)
                {
                    TempData.Add("Alert", "You can't book until you pay for membership fee! If you have paid for membership contact with our staff");
                    return RedirectToAction("Index", "Booking");
                }

            }
            //if user is staff or if member was found and can make bookings
            //create list of integers, save it  as session and add Id to the session
            List<int> lstActivityShoppingCart = HttpContext.Session.Get<List<int>>("ssActivityShoppingCart");
            if (lstActivityShoppingCart == null)
            {
                lstActivityShoppingCart = new List<int>();
            }
            lstActivityShoppingCart.Add(id);

            HttpContext.Session.Set("ssActivityShoppingCart", lstActivityShoppingCart);
            //display success message
            TempData.Add("Success", "You have added item to cart");
            return RedirectToAction("Index", "BookActivity", new { area = "Customer" });
        }

        /// <summary>
        /// Removes activity from shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects to index page</returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        public IActionResult Remove(int id)
        {
            List<int> lstActivityShoppingCart = HttpContext.Session.Get<List<int>>("ssActivityShoppingCart");
            if (lstActivityShoppingCart.Count > 0)
            {
                if (lstActivityShoppingCart.Contains(id))
                {
                    lstActivityShoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssActivityShoppingCart", lstActivityShoppingCart);

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Creates excel report about revenue from activities
        /// </summary>
        [Authorize(Roles = "Admin, Manager")]
        public void ActivitiesReportListDownload()
        {
         

            //create new list of activityReport (ViewModel) 
            List<ActivityReport> aReports = new List<ActivityReport>();
            //create array of string-- this will store activity names
            string[] types = new string[10000];
            //int for index
            int i = 0;

            //position of next value from the array
            int increment = 0;

            //finds if the name exists in the array
            int findExists = 0;
           

            // db query
            var bookingsDw = from ab in _db.FitnessActivityBookings
                             //join b in _db.Bookings on ab.BookingId equals b.Id
                             //join fa in _db.FitnessActivities on ab.FitnessActivity.FitnessActivityCategory.Name equals fa.FitnessActivityCategory.Name
                             join m in _db.Members on ab.Booking.MemberId equals m.Id
                             //where ab.FitnessActivity.FitnessActivityCategory.Name.Equals(fa.FitnessActivityCategory.Name)
                            // where ab.FitnessActivity.FitnessActivityCategory.Name = m.Bookings.Where(c=>c.Id == b.)
                             select new
                             {
                                 ActivityName = ab.FitnessActivity.FitnessActivityCategory.Name,
                                 ActivityPrice = ab.FitnessActivity.Price,

                                 NoOfBookings = m.Bookings.Count(),
                                 ActivityIncome = ab.FitnessActivity.Price * m.Bookings.Count(), 
                             };

            //loops for each item in query
            foreach(var item in bookingsDw)
            {
                //if postion in array is 0 -- first item
                if (increment == 0)
                {   //add activity name on to the first position
                    types[increment] = item.ActivityName;
                    increment++;
                    continue;
                }
                //loops through all items
                for(i = 0 ; i <= increment; i++)
                {   //if item is in array- continue
                    if (types[i] == item.ActivityName)
                    {
                        continue;
                    }
                    else
                    {   //add 1 to findexist
                        findExists++;
                    }
                }
                //findexists is eqaul to i
                if(findExists == i)
                {   //add activity name on postion of increment to array
                    types[increment] = item.ActivityName;
                    //add 1 to increment
                    increment++;                    
                }
                //reset find exist
                findExists = 0;
            }
            //loops through types array and adds each item to the aReports list
            for (i = 0; i < increment; i++)
            {
                aReports.Add(new ActivityReport { ActivityName = types[i] });
            }
            
            //outer loop, loops through items from db query
            foreach (var item in bookingsDw)
            {              
                //inner loop, loops through items in List
                foreach(var ar in aReports)
                {   //check if name in the list equals name in item from query
                    if(ar.ActivityName == item.ActivityName)
                    {   //if the names are the same increase no of bookings
                        ar.NoOfBookings++;
                        //and add price to income from this type of activity
                        ar.ActivityIncome += item.ActivityPrice;
                    }                    
                }
            }


            // how many rows is there?
            int numRows = aReports.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - later it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Activities");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(aReports, true);
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
                workSheet.Cells[1, 1].Value = "Activity revenue Report";
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

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers["content-disposition"] = "attachment; filename=ActivityReport.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }


        }
    }
}
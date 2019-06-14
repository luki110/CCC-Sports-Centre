using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CCCSportsCentreGradedUnit.Extensions;
using System.Security.Claims;
using Stripe;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
//using DinkToPdf;
//using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CCCSportsCentreGradedUnit.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _db;
       // private readonly IConverter _converter;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public BookClassViewModel BookingVM { get; set; }

        public BookingController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            //_converter = converter;
            _emailSender = emailSender;
            BookingVM = new BookClassViewModel
            {
                FitnessClasses = _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToList()
            };

        }
        /// <summary>
        /// Loads all fitness classes, allows user to search, sort, displays 20 first items rest goes to next page thanks to iteration
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString, int? pageNumber)
        {
            //view data for sorting
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
            //view data for searching
            ViewData["CurrentFilter"] = searchString;
            var fitnessClasses = from m in _db.FitnessClasses.Include(m => m.FitnessClassCategory).Include(m => m.Room)
                                 select m;

            //search query
            if (!String.IsNullOrEmpty(searchString))
            {
                fitnessClasses = fitnessClasses.Where(m => m.FitnessClassCategory.Name.Contains(searchString));
            }

            //sort querries
            switch (sortOrder)
            {
                case "name_desc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.FitnessClassCategory.Name);
                    break;
                case "StartTime":
                    fitnessClasses = fitnessClasses.OrderBy(s => s.StartTime);
                    break;
                case "StartTime_desc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    fitnessClasses = fitnessClasses.OrderBy(s => s.FitnessClassCategory.Name);
                    break;
            }
            int pageSize = 20;
            return View(await PaginatedList<FitnessClass>.CreateAsync(fitnessClasses.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        /// <summary>
        /// gets details of selected fitness class
        /// </summary>
        /// <param name="id"></param>
        /// <returns>fitness class</returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        //GET
        public async Task<IActionResult> Details(int id)
        {
            FitnessClass fitnessClass = await _db.FitnessClasses.Include(m => m.FitnessClassCategory).Include(m => m.Room).Where(m => m.Id == id).FirstOrDefaultAsync();


            return View(fitnessClass);
        }

        /// <summary>
        /// checks if user is member, also checks if user can make booking and if he already booked this class
        /// add fitness class to the basket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        //POST
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPOST(int id)
        {
            if (User.IsInRole("Member"))
            {
                //gets users id
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                //finds user
                Member member = _db.Members.Find(claim.Value);

                //if user didn't pay for membership
                if (member.CanMakeBooking == false)
                {
                    TempData.Add("Alert", "You can't book until you pay for membership fee! If you have paid for membership contact with our staff");
                    return RedirectToAction("Index", "Booking");
                }

                //find all fitness class bookings
                var bookingClassList = await _db.FitnessClassBookings.Include(m => m.Booking).Include(m => m.FitnessClass).ToListAsync();

                //foreach bookingclass
                foreach (FitnessClassBooking bookingClassExists in bookingClassList)
                {
                   
                    //find if member have booked this class
                    if (bookingClassExists.FitnessClassId == id && bookingClassExists.Booking.MemberId == member.Id)
                    {

                        TempData.Add("Alert", "You have already booked this class");

                        return RedirectToAction("Index", "Booking");
                    }
                }
            }

            //adds id of fitness class to session
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>();
            }
            lstShoppingCart.Add(id);

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            //display success message
            TempData.Add("Success", "You have added item to cart");
            return RedirectToAction("Index", "Booking", new { area = "Customer" });
        }

        /// <summary>
        /// Removes selected fitness class from basket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        public IActionResult Remove(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            return RedirectToAction(nameof(Index));
        }

        //public IActionResult ViewUserBookings()
        //{
        //    System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        //    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //    Member member = _db.Members.Find(claim.Value);

        //    MemberBookings MemberBookingVM = new MemberBookings
        //    {
        //        FitnessActivityBookings = _db.FitnessActivityBookings.Include(m => m.FitnessActivity.FitnessActivityCategory).Include(m => m.FitnessActivity.Room).Include(m => m.Booking).ToList(),
        //        FitnessClassBookings = _db.FitnessClassBookings.Include(m => m.FitnessClass.FitnessClassCategory).Include(m=>m.FitnessClass.Room).Include(m => m.Booking).ToList()

        //    };

        //    MemberBookingVM.Member = member;


        //    return View(MemberBookingVM);
        //}

            /// <summary>
            /// For member it will display only bookings made by him. For staff member it will display all bookings
            /// It allows user to search booking by members name, email, phonenumber, booking date, bookingdId
            /// </summary>
            /// <param name="searchName"></param>
            /// <param name="searchEmail"></param>
            /// <param name="searchPhone"></param>
            /// <param name="searchDate"></param>
            /// <param name="searchBookingId"></param>
            /// <returns>view model -- bookings</returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
        public IActionResult ViewUserBookings(string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null, string searchBookingId = null)
        {
            //finds users Id
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            
            MemberBookingViewModel bookingVM = new MemberBookingViewModel
            {
                Bookings = new List<Models.Booking>()
            };


            //adds loaded bookings from database to bookings in view model
            bookingVM.Bookings = _db.Bookings.Include(b => b.Member).ToList();

            //if user is member load only bookings made by him
            if (User.IsInRole("Member"))
            {
                bookingVM.Bookings = bookingVM.Bookings.Where(a => a.MemberId == claim.Value).ToList();
            }
            //if user enter value in search name text box
            if (searchName != null)
            {
                bookingVM.Bookings = bookingVM.Bookings.Where(b => b.Member.LastName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            //if user enter value in search email text box
            if (searchEmail != null)
            {
                bookingVM.Bookings = bookingVM.Bookings.Where(b => b.Member.Email.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            //if user enter value in search phone number text box
            if (searchPhone != null)
            {
                bookingVM.Bookings = bookingVM.Bookings.Where(b => b.Member.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            //if user enter value in search date text box
            if (searchDate != null)
            {
                try
                {   //conerts string to date time
                    DateTime bookDate = Convert.ToDateTime(searchDate);
                    bookingVM.Bookings = bookingVM.Bookings.Where(b => b.BookingDate.ToShortDateString().Equals(bookDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }

            }
            // serach by booking id
            if (searchBookingId != null)
            {
                try
                {
                    //try to convert string to int
                    int bookingId = Convert.ToInt32(searchBookingId);
                    bookingVM.Bookings = bookingVM.Bookings.Where(b => b.Id == bookingId).ToList();
                }
                catch (Exception ex)
                {

                }

            }

            return View(bookingVM);
        }
        /// <summary>
        /// shows details of selected booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns>view model - booked class, booked activity, member's details</returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]        
        public async Task<IActionResult> BookingDetails(int? id)
        {
            //if id is null
            if (id == null)
            {
                return NotFound();
            }
            //database query finds fitness classes booked under this booking Id
            var fitnessClassList = (IEnumerable<FitnessClass>)(from c in _db.FitnessClasses
                                                               join b in _db.FitnessClassBookings
                                                               on c.Id equals b.FitnessClassId
                                                               where b.BookingId == id
                                                               select c).Include("FitnessClassCategory");
            //database query finds fitness activities booked under this booking id
            var fitnessActivityList = (IEnumerable<FitnessActivity>)(from a in _db.FitnessActivities
                                                                     join b in _db.FitnessActivityBookings
                                                                     on a.Id equals b.FitnessActivityId
                                                                     where b.BookingId == id
                                                                     select a).Include("FitnessActivityCategory");

            //adds all the above lists to view model
            BookingDetailsViewModel objBookingVM = new BookingDetailsViewModel
            {
                Booking = _db.Bookings.Include(b => b.Member).Where(b => b.Id == id).FirstOrDefault(),
                FitnessClasses = fitnessClassList.ToList(),
                FitnessActivities = fitnessActivityList.ToList(),
            };
            return View(objBookingVM);
        }
        /// <summary>
        /// get method for deleting booking, loads all classes and activities booked under this booking Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>view model -- booked fitness classes and activities also member</returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //database query finds fitness classes booked under this booking Id
            var fitnessClassList = (IEnumerable<FitnessClass>)(from c in _db.FitnessClasses
                                                               join b in _db.FitnessClassBookings
                                                               on c.Id equals b.FitnessClassId
                                                               where b.BookingId == id
                                                               select c).Include("FitnessClassCategory");

            //database query finds fitness activities booked under this booking id
            var fitnessActivityList = (IEnumerable<FitnessActivity>)(from a in _db.FitnessActivities
                                                                     join b in _db.FitnessActivityBookings
                                                                     on a.Id equals b.FitnessActivityId
                                                                     where b.BookingId == id
                                                                     select a).Include("FitnessActivityCategory");


            BookingDetailsViewModel objBookingVM = new BookingDetailsViewModel
            {
                Booking = _db.Bookings.Include(b => b.Member).Where(b => b.Id == id).FirstOrDefault(),
                FitnessClasses = fitnessClassList.ToList(),
                FitnessActivities = fitnessActivityList.ToList(),
            };
            //checks if it's to late for canceling the activity
            foreach (FitnessActivity a in objBookingVM.FitnessActivities)
            {

                if (a.StartDate == DateTime.Today && (a.StartTime.Hour - DateTime.Now.Hour ) <= 1 || a.StartDate < DateTime.Today)
                {
                    TempData.Add("Alert", "It's to late to cancel this booking!");
                    return RedirectToAction(nameof(ViewUserBookings));
                }

            }
            //checks if it's to late for canceling the fitness class
            foreach (FitnessClass c in objBookingVM.FitnessClasses)
            {

                if (c.StartDate == DateTime.Today && (c.StartTime.Hour - DateTime.Now.Hour) <= 1 || c.StartDate < DateTime.Today)
                {
                    TempData.Add("Alert", "It's to late to cancel this booking!");
                    return RedirectToAction(nameof(ViewUserBookings));
                }

            }


            return View(objBookingVM);
        }
        /// <summary>
        /// deletes booking, and returns money to member
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find booking by id include member
            var booking = await _db.Bookings.Where(b=>b.Id == id).Include(b=>b.Member).FirstOrDefaultAsync();

            
            //finds fitness activities booked with this bookied id
            var ab = _db.FitnessActivityBookings.Where(a => a.BookingId == booking.Id).Include(a => a.FitnessActivity).Include(a => a.Booking).ToList();
            //finds fitness classes booked with this bookied id
            var cb = _db.FitnessClassBookings.Where(a => a.BookingId == booking.Id).Include(a => a.FitnessClass).Include(a => a.Booking).ToList();

            //foreach found activity in fitness activity booking make activity available 
            foreach (FitnessActivityBooking a in ab)
            {
                a.FitnessActivity.Available = true;
                await _db.SaveChangesAsync();
            }

            //foreach found fitness class in fitness class booking take out no of people booked
            foreach (FitnessClassBooking c in cb)
            {
                c.FitnessClass.NoOfPeopleBooked--;
                //if fitness class was not available
                if (c.FitnessClass.Available == false)
                {
                    //make it available
                    c.FitnessClass.Available = true;
                }
                //save changes
                await _db.SaveChangesAsync();
            }

            //remove booking from database
            _db.Bookings.Remove(booking);


            //stripe refund 
            var options = new RefundCreateOptions
            {   
                //finds amount 
                Amount = Convert.ToInt32(booking.BookingTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,
                ChargeId = booking.BookingPaymentId
            };

            var service = new RefundService();
            //refunds money
            Refund refund = service.Create(options);
            await _db.SaveChangesAsync();
            //send email
            await _emailSender.SendEmailAsync(booking.Member.Email, "Booking canceled",
                       $"Your booking for booking number: {booking.Id} made on {booking.BookingDate} has been canceled successfuly." +                      
                       $" £{booking.BookingTotal} has been returned to your account. " +
                       $"Regards CCC Sport Centre Team");

            TempData.Add("Success", "Booking canceled successfully!");
            return RedirectToAction(nameof(ViewUserBookings));

        }


        /// <summary>
        /// create excel bookings report
        /// </summary>
        [Authorize(Roles = "Admin, Manager")]
        public void AppListDownload()
        {
            //TODO make  JOIN query to include product type and price in report
            var bookingsDw = from bookings in _db.Bookings.Include(b => b.Member)
                             orderby bookings.BookingDate descending
                             where bookings.BookingDate == DateTime.Today
                             select new
                             {
                                 Date = bookings.BookingDate,
                                 FirstName = bookings.Member.FirstName,
                                 LastName = bookings.Member.LastName,
                                 Email = bookings.Member.Email,
                                 Phone = bookings.Member.PhoneNumber,
                                 Total = bookings.BookingTotal

                             };


            // how many rows is there?
            int numRows = bookingsDw.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - later it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Appointments");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(bookingsDw, true);
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
                workSheet.Cells[1, 1].Value = "Appointment Report";
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
                    Response.Headers["content-disposition"] = "attachment; filename=BookingsReport.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }
           
        }
        /// <summary>
        /// for pdf report --- not working :(
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult BookingReport()
        {
            var bookings = _db.Bookings.Include(b => b.Member).ToList();


            return View(bookings);
        }
        /// <summary>
        /// for pdf report --- not working :(
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "Admin, Manager")]
        //[HttpGet]
        //public IActionResult CreatePDF()
        //{
        //    var globalSettings = new GlobalSettings
        //    {
        //        ColorMode = ColorMode.Color,
        //        Orientation = Orientation.Portrait,
        //        PaperSize = PaperKind.A4,
        //        Margins = new MarginSettings { Top = 10 },
        //        DocumentTitle = "PDF Report"
        //    };

        //    var objectSettings = new ObjectSettings
        //    {
        //        PagesCount = true,
        //        Page = "https://localhost:44335/Customer/Booking/BookingReport/"
        //    };

        //    var pdf = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = globalSettings,
        //        Objects = { objectSettings }
        //    };

        //    var file = _converter.Convert(pdf);
        //    return File(file, "application/pdf", "BookingReport.pdf");
        //}

        /// <summary>
        /// Create revenue by class report
        /// </summary>
        [Authorize(Roles = "Admin, Manager")]
        public void ClassesReportListDownload()
        {


            //create new list of activityReport (ViewModel) 
            List<ActivityReport> cReports = new List<ActivityReport>();
            //create array of string-- this will store activity names
            string[] types = new string[10000];
            //int for index
            int i = 0;

            //position of next value from the array
            int increment = 0;

            //finds if the name exists in the array
            int findExists = 0;


            // db query
            var bookingsDw = from ab in _db.FitnessClassBookings
                                 //join b in _db.Bookings on ab.BookingId equals b.Id
                                 //join fa in _db.FitnessActivities on ab.FitnessActivity.FitnessActivityCategory.Name equals fa.FitnessActivityCategory.Name
                             join m in _db.Members on ab.Booking.MemberId equals m.Id
                             //where ab.FitnessActivity.FitnessActivityCategory.Name.Equals(fa.FitnessActivityCategory.Name)
                             // where ab.FitnessActivity.FitnessActivityCategory.Name = m.Bookings.Where(c=>c.Id == b.)
                             select new
                             {
                                 ClassName = ab.FitnessClass.FitnessClassCategory.Name,
                                 ClassPrice = ab.FitnessClass.Price,

                                 NoOfBookings = m.Bookings.Count(),
                                 ClassIncome = ab.FitnessClass.Price * m.Bookings.Count(),
                             };

            //loops for each item in query
            foreach (var item in bookingsDw)
            {
                //if postion in array is 0 -- first item
                if (increment == 0)
                {   //add class name on to the first position
                    types[increment] = item.ClassName;
                    increment++;
                    continue;
                }
                //loops through all items
                for (i = 0; i <= increment; i++)
                {   //if item is in array- continue
                    if (types[i] == item.ClassName)
                    {
                        continue;
                    }
                    else
                    {   //add 1 to findexist
                        findExists++;
                    }
                }
                //findexists is eqaul to i
                if (findExists == i)
                {   //add class name on postion of increment to array
                    types[increment] = item.ClassName;
                    //add 1 to increment
                    increment++;
                }
                //reset find exist
                findExists = 0;
            }
            //loops through types array and adds each item to the aReports list
            for (i = 0; i < increment; i++)
            {
                cReports.Add(new ActivityReport { ActivityName = types[i] });
            }

            //outer loop, loops through items from db query
            foreach (var item in bookingsDw)
            {
                //inner loop, loops through items in List
                foreach (var cr in cReports)
                {   //check if name in the list equals name in item from query
                    if (cr.ActivityName == item.ClassName)
                    {   //if the names are the same increase no of bookings
                        cr.NoOfBookings++;
                        //and add price to income from this type of class
                        cr.ActivityIncome += item.ClassPrice;
                    }
                }
            }


            // how many rows is there?
            int numRows = cReports.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Classes");

                // add data to our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(cReports, true);
                workSheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd HH:MM";

                // define block of cells Cells[startRow, startColumn, endRow, endColumn]
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
                workSheet.Cells[1, 1].Value = "Class revenue Report";
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
                    Response.Headers["content-disposition"] = "attachment; filename=ClassesReport.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }


        }

    }
}
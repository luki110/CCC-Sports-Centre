using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCCSportsCentreGradedUnit.Areas.Admin.Controllers
{
    /// <summary>
    /// Fitness class controller allows to perform crud opertaions on Fitness class
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant")]
    [Area("Admin")]
    public class FitnessClassesController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public FitnessClassViewModel FitnessClassVM { get; set; }


        public FitnessClassesController(ApplicationDbContext db)
        {
            _db = db;
            FitnessClassVM = new FitnessClassViewModel
            {
                Rooms = _db.Rooms.ToList(),
                FitnessClassCategories = _db.FitnessClassCategories.ToList(),
                FitnessClass = new FitnessClass()
            };
        }
        /// <summary>
        /// Loads all classes to paginated list,  allows to search and sort classes
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns>list of fintess classes</returns>
        public async Task<IActionResult> Index(string sortOrder,
             string currentFilter, string searchString, int? pageNumber)
        {
            //sort view data
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TimeSortParm"] = sortOrder == "Time" ? "StartTime_descc" : "Time";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "StartDate_descc" : "Date";
            ViewData["RoomSortParm"] = sortOrder == "Room" ? "RoomName_descc" : "Room";
            //if search string is null-- nothing has been searched
            if (searchString != null)
            {
                pageNumber = 1;
            }
            //if something been searched
            else
            {
                searchString = currentFilter;
            }
            //pass search string to currentfilter view data
            ViewData["CurrentFilter"] = searchString;
            var fitnessClasses = from m in _db.FitnessClasses.Include(m => m.FitnessClassCategory).Include(m => m.Room)
                                 select m;
            //if search string is not null find following
            if (!String.IsNullOrEmpty(searchString))
            {
                fitnessClasses = fitnessClasses.Where(m => m.FitnessClassCategory.Name.Contains(searchString)
                                                           || m.Room.Name.Contains(searchString)
                                                           || m.StartDate.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.FitnessClassCategory.Name);
                    break;
                case "StartTime":
                    fitnessClasses = fitnessClasses.OrderBy(s => s.StartTime.Hour);
                    break;
                case "StartTime_desc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.StartTime.Hour);
                    break;
                case "StartDate_descc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.StartDate);
                    break;
                case "StartDate":
                    fitnessClasses = fitnessClasses.OrderBy(s => s.StartDate);
                    break;
                case "RoomName_descc":
                    fitnessClasses = fitnessClasses.OrderByDescending(s => s.Room.Name);
                    break;
                case "Room":
                    fitnessClasses = fitnessClasses.OrderBy(s => s.Room.Name);
                    break;
                default:
                    fitnessClasses = fitnessClasses.OrderBy(s => s.FitnessClassCategory.Name);
                    break;
            }
            //loads 10 items per page
            int pageSize = 10;
            return View(await PaginatedList<FitnessClass>.CreateAsync(fitnessClasses.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //public async Task<IActionResult> Index()
        //{
        //    var fitnessClass = _db.FitnessClasses.Include(m => m.Room).Include(m=>m.FitnessClassCategory).OrderBy(m =>m.StartDate);

        //    return View(await fitnessClass.ToListAsync());
        //}

        /// <summary>
        /// create get method
        /// </summary>
        /// <returns> view</returns>
        public IActionResult Create()
        {
            return View(FitnessClassVM);
        }

        /// <summary>
        /// Post method to create fitness class
        /// </summary>
        /// <param name="fitnessClass"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(FitnessClass fitnessClass)
        {
            if (!ModelState.IsValid)
            {
                TempData.Add("Alert", "Something went wrong!");
                return RedirectToAction(nameof(Index));
            }

            //add start time to start date
            FitnessClassVM.FitnessClass.StartTime = FitnessClassVM.FitnessClass.StartDate
                                                    .AddHours(FitnessClassVM.FitnessClass.StartTime.Hour)
                                                    .AddMinutes(FitnessClassVM.FitnessClass.StartTime.Minute);
            //add duration to start time to get end time
            FitnessClassVM.FitnessClass.EndTime = FitnessClassVM.FitnessClass.StartTime.AddMinutes(fitnessClass.Duration);

            
            FitnessClassVM.FitnessClass.NoOfPeopleBooked = 0;
            //check if class is not created in the past
            if (fitnessClass.StartDate <= DateTime.Today && FitnessClassVM.FitnessClass.StartTime < DateTime.Now)
            {
                // if so display error message
                TempData.Add("Alert", "You can't add class in the past. Try again this time with correct date.");
                return RedirectToAction(nameof(Index));
            }
            //check if duration and price are less or equal to 0
            if (FitnessClassVM.FitnessClass.Duration <= 0 || FitnessClassVM.FitnessClass.Price <= 0)
            {
                TempData.Add("Alert", "Price and duration must be numbers bigger than 0, try again.");
                return RedirectToAction(nameof(Index));
            }
            //load fitness classes from db
            var fitnessClasses = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToListAsync();
            //loop through list of fitness classes to check if the room is already occupied
            foreach (FitnessClass fitclass in fitnessClasses)
            {
                if (FitnessClassVM.FitnessClass.StartDate == fitclass.StartDate && FitnessClassVM.FitnessClass.StartTime == fitclass.StartTime &&
                    FitnessClassVM.FitnessClass.RoomId == fitclass.RoomId)
                {
                    TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room.");
                    return RedirectToAction(nameof(Index));
                }

                //if full hall was booked at selected time
                //if(FitnessClassVM.FitnessClass.StartDate == fitclass.StartDate && FitnessClassVM.FitnessClass.StartTime == fitclass.StartTime &&
                //    fitclass.Room.Name.Contains("Full"))
                //{
                //    TempData.Add("Alert", "Whole hall is booked at this time");
                //    return RedirectToAction(nameof(Index));
                //}
            }
            //load fitness activities from db
            var fitnessActivities = await _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).ToListAsync();
            //loop through list of fitness activities to check if the room is already occupied
            foreach (FitnessActivity fitActivity in fitnessActivities)
            {
                if (FitnessClassVM.FitnessClass.StartDate == fitActivity.StartDate && FitnessClassVM.FitnessClass.StartTime == fitActivity.StartTime &&
                    FitnessClassVM.FitnessClass.RoomId == fitActivity.RoomId)
                {
                    TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room.");
                    return RedirectToAction(nameof(Index));
                }
                //if full hall was booked at selected time
                if (FitnessClassVM.FitnessClass.StartDate == fitActivity.StartDate && FitnessClassVM.FitnessClass.StartTime == fitActivity.StartTime &&
                    fitActivity.Room.Name.Contains("Full"))
                {
                    TempData.Add("Alert", "Whole hall is booked at this time.");
                    return RedirectToAction(nameof(Index));
                }
            }
            //add fitness activity to database
            _db.FitnessClasses.Add(FitnessClassVM.FitnessClass);
            await _db.SaveChangesAsync();
            //var fitnessActivties = await _db.FitnessActivities.ToListAsync();

            //foreach (FitnessActivity fitnessActivity in fitnessActivties)
            //{
            //    if (FitnessClassVM.FitnessClass.Id == fitnessClass.Id)
            //    {
            //        FitnessClassVM.FitnessClass.Id++;
            //    }
            //}
            //_db.FitnessClasses.Remove(FitnessClassVM.FitnessClass);
            //await _db.SaveChangesAsync();
            //_db.FitnessClasses.Add(FitnessClassVM.FitnessClass);
            //await _db.SaveChangesAsync();

            //display success message
            TempData.Add("Success", "Fitness class created successfuly.");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// loads detials of class and checks if it was booked
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //gets details of class with passed in id
            FitnessClassVM.FitnessClass = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            //gets list of fitness class booking
            var cbookings = await _db.FitnessClassBookings.Include(m => m.FitnessClass).ToListAsync();

            //loops through the list
            foreach (var c in cbookings)
            {   
                //if any booking has the same id as passed in Id it means that it was booked 
                if (c.FitnessClassId == id)
                {
                    TempData.Add("Alert", "You can't edit this, someone have booked this clas");
                    return RedirectToAction(nameof(Index));
                }
            }

            if (FitnessClassVM == null)
            {
                return NotFound();
            }
            return View(FitnessClassVM);
        }

        /// <summary>
        /// updates fitness class
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            if (ModelState.IsValid)
            {
                //updates data of activity with given id
                var fitnessClassfromDb = _db.FitnessClasses.Where(m => m.Id == FitnessClassVM.FitnessClass.Id)
                    .Include(m => m.FitnessClassCategory).Include(m => m.Room).FirstOrDefault();
                fitnessClassfromDb.FitnessClassCategoryId = FitnessClassVM.FitnessClass.FitnessClassCategoryId;
                fitnessClassfromDb.StartDate = FitnessClassVM.FitnessClass.StartDate;
                fitnessClassfromDb.StartTime = FitnessClassVM.FitnessClass.StartDate
                                                    .AddHours(FitnessClassVM.FitnessClass.StartTime.Hour)
                                                    .AddMinutes(FitnessClassVM.FitnessClass.StartTime.Minute);
                fitnessClassfromDb.Price = FitnessClassVM.FitnessClass.Price;
                fitnessClassfromDb.Duration = FitnessClassVM.FitnessClass.Duration;
                fitnessClassfromDb.EndTime = FitnessClassVM.FitnessClass.StartTime.AddMinutes(FitnessClassVM.FitnessClass.Duration);
                fitnessClassfromDb.RoomId = FitnessClassVM.FitnessClass.RoomId;
                fitnessClassfromDb.Available = FitnessClassVM.FitnessClass.Available;


                //to check if updated date in not in past
                if (FitnessClassVM.FitnessClass.StartDate <= DateTime.Today && FitnessClassVM.FitnessClass.StartTime < DateTime.Now)
                {
                    TempData.Add("Alert", "You can't add class in the past. Try again, this time with correct date and time :)");
                    return RedirectToAction(nameof(Index));
                }
                //check if duration and price are bigger than 0
                if (FitnessClassVM.FitnessClass.Duration <= 0 || FitnessClassVM.FitnessClass.Price <= 0)
                {
                    TempData.Add("Alert", "Price and duration must be numbers bigger than 0, try again");
                    return RedirectToAction(nameof(Index));
                }
                //loads classes from db
                var fitnessClasses = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToListAsync();
                //finds if room is occupied in selected date and time
                foreach (FitnessClass fitclass in fitnessClasses)
                {
                    if (FitnessClassVM.FitnessClass.Id != fitclass.Id && FitnessClassVM.FitnessClass.StartDate == fitclass.StartDate &&
                        FitnessClassVM.FitnessClass.StartTime.Hour == fitclass.StartTime.Hour &&
                        FitnessClassVM.FitnessClass.RoomId == fitclass.RoomId)
                    {
                        TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time");
                        return RedirectToAction(nameof(Index));
                    }
                }
                //loads activities from db
                var fitnessActivities = await _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).ToListAsync();
                //finds if room is occupied in selected date and time, allows edit if activity Id is equal to Id 
                foreach (FitnessActivity fitActivity in fitnessActivities)
                {
                    if (FitnessClassVM.FitnessClass.StartDate == fitActivity.StartDate && FitnessClassVM.FitnessClass.StartTime.Hour == fitActivity.StartTime.Hour &&
                        FitnessClassVM.FitnessClass.RoomId == fitActivity.RoomId)
                    {
                        TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room.");
                        return RedirectToAction(nameof(Index));
                    }
                }
                //save changes
                await _db.SaveChangesAsync();
                TempData.Add("Success", "Update was successful");
                return RedirectToAction(nameof(Index));

            }

            TempData.Add("Alert", "Something went wrong, try again");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// loads details of class
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //gets details of class with passed in id
            FitnessClassVM.FitnessClass = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory)
                .SingleOrDefaultAsync(m => m.Id == id);


            if (FitnessClassVM == null)
            {
                return NotFound();
            }
            return View(FitnessClassVM);
        }


        /// <summary>
        /// loads detials of class and checks if it was booked
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData.Add("Alert", "Not found");
                return NotFound();
            }
            //gets details of class with passed in id
            FitnessClassVM.FitnessClass = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory)
                .SingleOrDefaultAsync(m => m.Id == id);

            //gets list class bookings
            var cbookings = await _db.FitnessClassBookings.Include(m => m.FitnessClass).ToListAsync();
            //loop through that list
            foreach (var a in cbookings)
            {
                //if activity id from list is the same as the one passed in it means that someone have booked
                if (a.FitnessClassId == id)
                {
                    TempData.Add("Alert", "You can't delete this, someone have booked this class");
                    return RedirectToAction(nameof(Index));
                }
            }


            if (FitnessClassVM == null)
            {
                TempData.Add("Alert", "Not found");
                return NotFound();
            }
            return View(FitnessClassVM);
        }

        /// <summary>
        /// Deletes activity from db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find fitness class in db using passed in id
            FitnessClass fitnessClass = await _db.FitnessClasses.FindAsync(id);
            //if class in null
            if (fitnessClass == null)
            {
                //display error
                TempData.Add("Alert", "Not found");
                return NotFound();
            }
            else
            {
                //remove class from db
                _db.FitnessClasses.Remove(fitnessClass);
                await _db.SaveChangesAsync();
                TempData.Add("Success", "Deleted successfuly");
                return RedirectToAction(nameof(Index));
            }

        }

    }
}
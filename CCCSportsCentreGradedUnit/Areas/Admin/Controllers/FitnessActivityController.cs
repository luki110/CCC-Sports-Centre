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
    /// Fitness Activity controller allows to perform crud opertaions o nfitness activities
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant")]
    [Area("Admin")]
    public class FitnessActivityController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public FitnessActivityViewModel FitnessActivityVM { get; set; }

        public FitnessActivityController(ApplicationDbContext db)
        {
            _db = db;
            FitnessActivityVM = new FitnessActivityViewModel
            {
                Rooms = _db.Rooms.ToList(),
                FitnessActivityCategories = _db.FitnessActivityCategories.ToList(),
                FitnessActivity = new FitnessActivity()
            };
        }

        /// <summary>
        /// Loads all activities to paginated list,  allows to search and sort activities
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns>list of activities</returns>
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
            var fitnessActivities = from m in _db.FitnessActivities.Include(m => m.FitnessActivityCategory).Include(m => m.Room)
                                    select m;
            //if search string is not null find following
            if (!String.IsNullOrEmpty(searchString))
            {
                fitnessActivities = fitnessActivities.Where(m => m.FitnessActivityCategory.Name.Contains(searchString)
                                                           || m.Room.Name.Contains(searchString)
                                                           || m.StartDate.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.FitnessActivityCategory.Name);
                    break;
                case "StartTime":
                    fitnessActivities = fitnessActivities.OrderBy(s => s.StartTime.Hour);
                    break;
                case "StartTime_desc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.StartTime.Hour);
                    break;
                case "StartDate_descc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.StartDate);
                    break;
                case "StartDate":
                    fitnessActivities = fitnessActivities.OrderBy(s => s.StartDate);
                    break;
                case "RoomName_descc":
                    fitnessActivities = fitnessActivities.OrderByDescending(s => s.Room.Name);
                    break;
                case "Room":
                    fitnessActivities = fitnessActivities.OrderBy(s => s.Room.Name);
                    break;
                default:
                    fitnessActivities = fitnessActivities.OrderBy(s => s.FitnessActivityCategory.Name);
                    break;
            }
            //loads 10 items per page
            int pageSize = 10;
            return View(await PaginatedList<FitnessActivity>.CreateAsync(fitnessActivities.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //public async Task<IActionResult> Index()
        //{
        //    var fitnessActivity = _db.FitnessActivities.Include(m => m.Room).Include(m=>m.FitnessActivityCategory).OrderBy(m => m.StartDate);


        //    return View(await fitnessActivity.ToListAsync());
        //}

        /// <summary>
        /// create get method
        /// </summary>
        /// <returns> view</returns>
        public IActionResult Create()
        {

            return View(FitnessActivityVM);
        }

        /// <summary>
        /// Post method to create fitness activity
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                TempData.Add("Alert", "Something went wrong!");
                return RedirectToAction(nameof(Index));
            }
            //check if duration and price are less or equal to 0
            if (FitnessActivityVM.FitnessActivity.Duration <= 0 || FitnessActivityVM.FitnessActivity.Price <= 0)
            {   // if so display error message
                TempData.Add("Alert", "Price and duration must be numbers bigger than 0, try again");
                return RedirectToAction(nameof(Index)); 
            }
            //add start time to start date
            FitnessActivityVM.FitnessActivity.StartTime = FitnessActivityVM.FitnessActivity.StartDate
                                                            .AddHours(FitnessActivityVM.FitnessActivity.StartTime.Hour)
                                                            .AddMinutes(FitnessActivityVM.FitnessActivity.StartTime.Minute);
            //add duration to start time to get end time
            FitnessActivityVM.FitnessActivity.EndTime = FitnessActivityVM.FitnessActivity.StartTime.AddMinutes(FitnessActivityVM.FitnessActivity.Duration);

            
            //check if activity is not created in the past
            if (FitnessActivityVM.FitnessActivity.StartDate <= DateTime.Today && FitnessActivityVM.FitnessActivity.StartTime < DateTime.Now)
            {
                TempData.Add("Alert", "You can't create activity in past");
                return RedirectToAction(nameof(Index));
            }
            //load fitness activities from db
            var fitnessActivities = await _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).ToListAsync();

            //loop through list of fitness activities to check if the room is already occupied
            foreach (FitnessActivity fitactivity in fitnessActivities)
            {
                if (FitnessActivityVM.FitnessActivity.StartDate == fitactivity.StartDate && FitnessActivityVM.FitnessActivity.StartTime.Hour == fitactivity.StartTime.Hour &&
                    FitnessActivityVM.FitnessActivity.RoomId == fitactivity.RoomId && FitnessActivityVM.FitnessActivity.Id != fitactivity.Id)
                {
                    TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room");
                    return RedirectToAction(nameof(Index));
                }
            }
            //load fitness classes from db
            var fitnessClasses = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToListAsync();
            //loop through list of fitness classes to check if the room is already occupied
            foreach (FitnessClass fitclass in fitnessClasses)
            {
                if (FitnessActivityVM.FitnessActivity.StartDate == fitclass.StartDate && FitnessActivityVM.FitnessActivity.StartTime == fitclass.StartTime &&
                    FitnessActivityVM.FitnessActivity.RoomId == fitclass.RoomId)
                {
                    TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room.");
                    return RedirectToAction(nameof(Index));
                }
            }
            //add fitness activity to database
            _db.FitnessActivities.Add(FitnessActivityVM.FitnessActivity);
            await _db.SaveChangesAsync();     
            //display success message
            TempData.Add("Success", "Activity created successfuly");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// load details of activity
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FitnessActivityVM.FitnessActivity = await _db.FitnessActivities.Include(m => m.Room).Include(m=>m.FitnessActivityCategory).SingleOrDefaultAsync(m => m.Id == id);


            if (FitnessActivityVM == null)
            {
                return NotFound();
            }
            return View(FitnessActivityVM);
        }

        /// <summary>
        /// updates fitness activity
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
                var fitnessActivityfromDb = _db.FitnessActivities.Where(m => m.Id == FitnessActivityVM.FitnessActivity.Id)
                    .Include(m=>m.FitnessActivityCategory).Include(m=>m.Room).FirstOrDefault();
                fitnessActivityfromDb.FitnessActivityCategoryId = FitnessActivityVM.FitnessActivity.FitnessActivityCategoryId;
                fitnessActivityfromDb.StartDate = FitnessActivityVM.FitnessActivity.StartDate;               
                fitnessActivityfromDb.Price = FitnessActivityVM.FitnessActivity.Price;
                fitnessActivityfromDb.Duration = FitnessActivityVM.FitnessActivity.Duration;
                fitnessActivityfromDb.StartTime = FitnessActivityVM.FitnessActivity.StartDate
                                                            .AddHours(FitnessActivityVM.FitnessActivity.StartTime.Hour)
                                                            .AddMinutes(FitnessActivityVM.FitnessActivity.StartTime.Minute);
                fitnessActivityfromDb.EndTime = FitnessActivityVM.FitnessActivity.StartTime.AddMinutes(FitnessActivityVM.FitnessActivity.Duration);
                fitnessActivityfromDb.RoomId = FitnessActivityVM.FitnessActivity.RoomId;
                fitnessActivityfromDb.Available = FitnessActivityVM.FitnessActivity.Available;

                //get activity bookings from database
                var abookings = await _db.FitnessActivityBookings.Include(m => m.FitnessActivity).ToListAsync();

                //loops through the list of activiy bookings to find if it was booked by someone if yes display error message ang redirect to index page
                foreach (var a in abookings)
                {
                    if (a.FitnessActivityId == id)
                    {
                        TempData.Add("Alert", "You can't edit this, someone have booked this activity");
                        return RedirectToAction(nameof(Index));
                    }
                }

                //check if duration and price are bigger than 0
                if (FitnessActivityVM.FitnessActivity.Duration <= 0 || FitnessActivityVM.FitnessActivity.Price <= 0)
                {
                    TempData.Add("Alert", "Price and duration must be numbers bigger than 0, try again");
                    return RedirectToAction(nameof(Index));
                }
                //to check if updated date in not in past
                if (FitnessActivityVM.FitnessActivity.StartDate <= DateTime.Today && FitnessActivityVM.FitnessActivity.StartTime < DateTime.Now)
                {
                    TempData.Add("Alert", "You can't create activity in past");
                    return RedirectToAction(nameof(Index));
                }
                //loads activities from db
                var fitnessActivities = await _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).ToListAsync();

                //finds if room is occupied in selected date and time, allows edit if activity Id is equal to Id 
                foreach (FitnessActivity fitactivity in fitnessActivities)
                {
                    if (FitnessActivityVM.FitnessActivity.Id != fitactivity.Id && FitnessActivityVM.FitnessActivity.StartDate == fitactivity.StartDate 
                        && FitnessActivityVM.FitnessActivity.StartTime.Hour == fitactivity.StartTime.Hour &&
                        FitnessActivityVM.FitnessActivity.RoomId == fitactivity.RoomId)
                    {
                        TempData.Add("Alert", "This room is not available at selcted time slot, please select different start time or room");
                        return RedirectToAction(nameof(Index));
                    }
                }
                //loads classes from db
                var fitnessClasses = await _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).ToListAsync();
                //finds if room is occupied in selected date and time
                foreach (FitnessClass fitclass in fitnessClasses)
                {
                    if (FitnessActivityVM.FitnessActivity.StartDate == fitclass.StartDate && FitnessActivityVM.FitnessActivity.StartTime.Hour == fitclass.StartTime.Hour &&
                        FitnessActivityVM.FitnessActivity.RoomId == fitclass.RoomId)
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

            //unknown error
            TempData.Add("Alert", "Something went wrong, try again");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// loads details of activity
        /// </summary>
        /// <returns> fitness activity view model</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            FitnessActivityVM.FitnessActivity = await _db.FitnessActivities.Include(m => m.Room).Include(m=> m.FitnessActivityCategory).SingleOrDefaultAsync(m => m.Id == id);


            if (FitnessActivityVM == null)
            {
                return NotFound();
            }
            return View(FitnessActivityVM);
        }


        /// <summary>
        /// loads detials of activitity and checks if it was booked
        /// </summary>
        /// <returns>fitness activity view model</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData.Add("Alert", "Not found");
                return NotFound();
            }

            FitnessActivityVM.FitnessActivity = await _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).SingleOrDefaultAsync(m => m.Id == id);

            //gets list activty bookings
            var abookings = await _db.FitnessActivityBookings.Include(m => m.FitnessActivity).ToListAsync();
            //loop through that list
            foreach(var a in abookings)
            {
                //if activity id from list is the same as the one passed in it means that someone have booked this activity so staff member can't delete it
                if(a.FitnessActivityId == id)
                {
                    TempData.Add("Alert", "You can't delete this, someone have booked this activity");
                    return RedirectToAction(nameof(Index));
                }
            }

            if (FitnessActivityVM == null)
            {
                TempData.Add("Alert", "Not found");
                return NotFound();
            }
            return View(FitnessActivityVM);
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
            FitnessActivity fitnessActivity = await _db.FitnessActivities.FindAsync(id);
            //if class in null
            if (fitnessActivity == null)
            {
                //display error
                TempData.Add("Alert", "Not found");
                return NotFound();
            }
            else
            {
                //delete activity from database
                _db.FitnessActivities.Remove(fitnessActivity);
                await _db.SaveChangesAsync();
                TempData.Add("Success", "Deleted successfuly");
                return RedirectToAction(nameof(Index));
            }

        }

    }
}
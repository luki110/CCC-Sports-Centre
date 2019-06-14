using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCCSportsCentreGradedUnit.Areas.Admin.Controllers
{
    /// <summary>
    /// Eployees controller allows to perform crud opertaions on staff members
    /// </summary>
    [Authorize(Roles = "Admin, Manager")]
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public StaffModel Createmodel { get; set; }

        public Staff EditModel { get; set; }

        public EmployeesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;

        }

        /// <summary>
        /// Loads all employees to a paginated list, allows to search and sort employees
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns>list of employees</returns>
        public async Task<IActionResult> Index(string sortOrder,
                   string currentFilter, string searchString, int? pageNumber)
        {
            //sort viewData
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["JobSortParm"] = sortOrder == "JobTitle" ? "job_desc" : "JobTitle";

            //dispay all items if nothing was searched
            if (searchString != null)
            {
                pageNumber = 1;
            }
            //display items for searched value
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var staffs = from m in _db.Staffs
                         select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                staffs = staffs.Where(m => m.LastName.Contains(searchString)
                                       || m.FirstName.Contains(searchString)
                                       || m.Email.Contains(searchString)
                                       || m.JobTitle.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    staffs = staffs.OrderByDescending(s => s.LastName);
                    break;
                case "JobTitle":
                    staffs = staffs.OrderBy(s => s.JobTitle);
                    break;
                case "job_desc":
                    staffs = staffs.OrderByDescending(s => s.JobTitle);
                    break;
                default:
                    staffs = staffs.OrderBy(s => s.LastName);
                    break;
            }
            //display 5 items per page
            int pageSize = 5;
            return View(await PaginatedList<Staff>.CreateAsync(staffs.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /// <summary>
        /// get method for creating staff
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {

            return View();
        }
        /// <summary>
        /// creates new employee
        /// </summary>
        /// <param name="Createmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreateStaff(StaffModel Createmodel)
        {
            if (ModelState.IsValid)
            {
                Staff employee = new Staff
                {
                    Email = Createmodel.Email,
                    UserName = Createmodel.Email,
                    PhoneNumber = Createmodel.PhoneNumber,
                    FirstName = Createmodel.FirstName,
                    LastName = Createmodel.LastName,
                    HouseNumber = Createmodel.HouseNumber,
                    Street = Createmodel.Street,
                    City = Createmodel.City,
                    Country = Createmodel.Country,
                    PostCode = Createmodel.PostCode,
                    RoleType = Createmodel.RoleType,
                    JobTitle = Createmodel.RoleType.ToString(),
                    EmergencyContact = Createmodel.EmergencyContact,
                    EmergencyContDetails = Createmodel.EmergencyContDetails,
                    CurrentQualification = Createmodel.CurrentQualification,

                };
                //create new employee with password
                var result = await _userManager.CreateAsync(employee, Createmodel.Password);

                //add employee to role
                await _userManager.AddToRoleAsync(employee, Createmodel.RoleType.ToString());
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        /// <summary>
        /// loads employee's detials
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _db.Staffs.FindAsync(id);


            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        /// <summary>
        /// updates employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="EditModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email, FirstName, LastName, PhoneNumber, HouseNumber, Street," +
            "City, PostCode, Country, JobTitle, EmergencyContact, EmergencyContDetails, CurrentQualification, RoleType" )] Staff EditModel)
        {
            //if (id != EditModel.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {

                var staff = await _db.Staffs.FindAsync(id);
                staff.Email = EditModel.Email;
                staff.FirstName = EditModel.FirstName;
                staff.LastName = EditModel.LastName;
                staff.HouseNumber = EditModel.HouseNumber;
                staff.Street = EditModel.Street;
                staff.PhoneNumber = EditModel.PhoneNumber;
                staff.PostCode = EditModel.PostCode;
                staff.City = EditModel.City;
                staff.Country = EditModel.Country;
                staff.RoleType = EditModel.RoleType;
                staff.JobTitle = EditModel.RoleType.ToString();
                staff.PhoneNumber = EditModel.PhoneNumber;
                staff.EmergencyContact = EditModel.EmergencyContact;
                staff.EmergencyContDetails = EditModel.EmergencyContDetails;
                staff.CurrentQualification = EditModel.CurrentQualification;
                staff.RoleType = EditModel.RoleType;

                //change role
                await _userManager.AddToRoleAsync(staff, EditModel.RoleType.ToString());

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(EditModel);
        }

        /// <summary>
        /// loads employee's details 
        /// </summary>
        /// <returns> staff</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _db.Staffs.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }


        /// <summary>
        ///loads employee's details 
        /// </summary>
        /// <returns> staff</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _db.Staffs.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        /// <summary>
        /// Deletes employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var staff = await _db.Staffs.FindAsync(id);
            _db.Staffs.Remove(staff);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
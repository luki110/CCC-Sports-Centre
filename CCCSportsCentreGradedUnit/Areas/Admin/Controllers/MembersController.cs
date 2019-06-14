using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CCCSportsCentreGradedUnit.Areas.Admin.Controllers
{
    /// <summary>
    /// Members controller allows to perform read and update opertaions on members
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff")]
    [Area("Admin")]
    public class MembersController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public Member Membermodel { get; set; }

        public MembersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        /// <summary>
        /// Loads all members to a paginated list, allows to search and sort members
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageNumber"></param>
        /// <returns>list of members</returns>
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter, string searchString,int? pageNumber)
        {
            //sort viewData
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["MembershipSortParm"] = sortOrder == "Membership" ? "membership_desc" : "Date";

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
            var members = from m in _db.Members.Include(m=>m.MembershipType)
                           select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                members = members.Where(m => m.LastName.Contains(searchString)
                                       || m.FirstName.Contains(searchString)
                                       || m.Email.Contains(searchString)
                                       || m.MembershipType.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    members = members.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    members = members.OrderBy(s => s.RegistrationDate);
                    break;
                case "date_desc":
                    members = members.OrderByDescending(s => s.RegistrationDate);
                    break;
                case "membership_desc":
                    members = members.OrderByDescending(s => s.MembershipType);
                    break;
                case "Membership":
                    members = members.OrderBy(s => s.MembershipType);
                    break;
                default:
                    members = members.OrderBy(s => s.LastName);
                    break;
            }
            //displays 5 items per page
            int pageSize = 5;
            return View(await PaginatedList<Member>.CreateAsync(members.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /// <summary>
        /// loads member's details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>member</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _db.Members.Include(m => m.MembershipType).SingleOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        /// <summary>
        /// Updates member's details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>member</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(string id)
        {
           

            if (ModelState.IsValid)
            {
                //var memberFromDb = _db.Members.Where(m => m.Id.Equals(Membermodel.Id)).Include(m=>m.MembershipType).SingleOrDefaultAsync(m => m.Id == id);
                //find member by id
                var member = await _db.Members.Include(m => m.MembershipType).SingleOrDefaultAsync(m => m.Id == id);
                //update member using vied model values
                member.Email = Membermodel.Email;
                member.FirstName = Membermodel.FirstName;
                member.LastName = Membermodel.LastName;
                member.MemberTitle = Membermodel.MemberTitle;
                member.HouseNumber = Membermodel.HouseNumber;
                member.Street = Membermodel.Street;
                member.PostCode = Membermodel.PostCode;
                member.City = Membermodel.City;
                member.Country = Membermodel.Country;
                member.GenderType = Membermodel.GenderType;
                member.PhoneNumber = Membermodel.PhoneNumber;

                //save changes
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Membermodel);
        }

        /// <summary>
        /// load member's details
        /// </summary>
        /// <returns> member</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _db.Members.Include(m=>m.MembershipType).SingleOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        //i have commented this out as I'm not sure if i should allow staff to delete members or just to lock their accounts
        /// <summary>
        /// get method
        /// </summary>
        /// <returns> view</returns>
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var member = await _db.Members.FindAsync(id);

        //    if (member == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(member);
        //}

        ////Post Delete action method
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var member = await _db.Members.FindAsync(id);
        //     member.LockoutEnd = DateTime.Now.AddYears(100);

        //    //_db.Members.Remove(member);
        //    await _db.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));

        //}

        /// <summary>
        /// suspends user from booking
        /// </summary>
        /// <returns> view</returns>
        public IActionResult SuspendMember(string id)
        {
            var member = _db.Members.Include(m => m.MembershipType).Where(m => m.Id == id).FirstOrDefault();
            member.CanMakeBooking = false;
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult UnSuspendMember(string id)
        {
            var member = _db.Members.Include(m => m.MembershipType).Where(m => m.Id == id).FirstOrDefault();
            if(member.PaymentConfirmed== true)
            {
                member.CanMakeBooking = true;
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData.Add("Alert", "This member haven't pay for membership!");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
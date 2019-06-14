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
    /// Room controller allows to perform crud opertaions on rooms
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant")]
    [Area("Admin")]
    public class RoomController : Controller
    {

        private readonly ApplicationDbContext _db;

       

        public RoomController(ApplicationDbContext db)
        {
            _db = db;           

        }
        /// <summary>
        /// loads list of rooms
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            
            return View(_db.Rooms.ToList());

        }

        /// <summary>
        /// get method for creating room
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Post method for creating room, adds new room to database
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _db.Add(room);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);

        }

        /// <summary>
        /// loads details of room from database
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessActivitiy = await _db.Rooms.FindAsync(id);

            if (fitnessActivitiy == null)
            {
                return NotFound();
            }
            return View(fitnessActivitiy);
        }

        /// <summary>
        /// updates details of room
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(room);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        /// <summary>
        /// loads details of room
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _db.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }


        /// <summary>
        /// get details of room
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rooms = await _db.Rooms.FindAsync(id);

            if (rooms == null)
            {
                return NotFound();
            }
            return View(rooms);
        }

        /// <summary>
        /// Deletes room from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rooms = await _db.Rooms.FindAsync(id);
            _db.Rooms.Remove(rooms);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }



    }
}
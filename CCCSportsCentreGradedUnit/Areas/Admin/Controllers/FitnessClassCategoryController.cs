using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CCCSportsCentreGradedUnit.Areas.Admin.Controllers
{
    /// <summary>
    /// Fitness classCategory controller allows to perform crud opertaions on Fitness classCategories
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant")]
    [Area("Admin")]
    public class FitnessClassCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;
        public FitnessClassCategoryController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// loads list of fitness class categories
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            return View(_db.FitnessClassCategories.ToList());

        }

        /// <summary>
        /// get method for creating fitness class category
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// creates new class category, also handles image upload
        /// </summary>
        /// <param name="fitnessClassCategory"></param>
        /// <returns>new fitness class category</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessClassCategory fitnessClassCategory)
        {
            if (ModelState.IsValid)
            {
                //add class category to db
                _db.Add(fitnessClassCategory);
                //save changes
                await _db.SaveChangesAsync();

                //image being saved
                string webRootPath = _hostingEnvironment.WebRootPath;

                // files will get files uploaded from the View
                var files = HttpContext.Request.Form.Files;

                //get category from database
                var categoryFromDb = _db.FitnessClassCategories.Find(fitnessClassCategory.Id);

                //check for file upload
                if (files.Count > 0)
                {
                    //if image has been uploaded                  get path to folder
                    var uploads = Path.Combine(webRootPath, SD.ImageClassFolder);

                    //get extension of the file
                    var extension = Path.GetExtension(files[0].FileName);

                    //using filestream copy uploaded file to the server and rename file to Id of new class category
                    using (var filesstream = new FileStream(Path.Combine(uploads, fitnessClassCategory.Id+extension), FileMode.Create))
                    {
                        //copy first item to filestream
                        files[0].CopyTo(filesstream);
                    }

                    //create path to the image
                    categoryFromDb.Image = @"\" + SD.ImageClassFolder + @"\" + fitnessClassCategory.Id + extension;

                }
                else
                {
                    //when user does not upload image
                    //get path to default image
                    var uploads = Path.Combine(webRootPath, SD.ImageClassFolder + @"\" + SD.DefaultClassImage);
                    //copy the default image to folder and rename it with activity category Id
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageClassFolder + @"\" + fitnessClassCategory.Id + ".jpg");
                    //create path to the image
                    categoryFromDb.Image = @"\" + SD.ImageClassFolder + @"\" + fitnessClassCategory.Id + ".jpg";
                }
                //save changes
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(fitnessClassCategory);

        }

        /// <summary>
        ///loads classs category details
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClassCategory = await _db.FitnessClassCategories.FindAsync(id);

            if (fitnessClassCategory == null)
            {
                return NotFound();
            }
            return View(fitnessClassCategory);
        }

        /// <summary>
        /// Updates fitness class category, also handles image upload if image is uploaded
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fitnessClassCategory"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessClassCategory fitnessClassCategory)
        {
            if (id != fitnessClassCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //get webRootPath in case image has to be changed
                string webrootPath = _hostingEnvironment.WebRootPath;

                //get files that were uploaded, if any
                var files = HttpContext.Request.Form.Files;

                //load category from database 
                var categoryFromDb = _db.FitnessClassCategories.Where(m => m.Id == fitnessClassCategory.Id).FirstOrDefault();

                //if user uploads a new image
                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image 
                    //this will retrieve the folder where image is uploaded or where image already exists
                    var uploads = Path.Combine(webrootPath, SD.ImageClassFolder);
                    //find extension of the images
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(categoryFromDb.Image);

                    //if old image exists
                    if (System.IO.File.Exists(Path.Combine(uploads, fitnessClassCategory.Id + extension_old)))
                    {
                        //delete old image
                        System.IO.File.Delete(Path.Combine(uploads, fitnessClassCategory.Id + extension_old));
                    }
                    //upload new file and rename it with id of fitnessActivity and extension
                    using (var filesstream = new FileStream(Path.Combine(uploads, fitnessClassCategory.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filesstream);
                    }
                    //save path to the image as activityCategory.image
                    fitnessClassCategory.Image = @"\" + SD.ImageClassFolder + @"\" + fitnessClassCategory.Id + extension_new;
                }
                //check if file was uploaded if yes update it
                if (fitnessClassCategory.Image != null)
                {
                    categoryFromDb.Image = fitnessClassCategory.Image;
                }
                //update rest of properties
                categoryFromDb.Name = fitnessClassCategory.Name;
                categoryFromDb.Description = fitnessClassCategory.Description;

                //_db.Update(fitnessClassCategory);
                //save changes
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessClassCategory);
        }

        /// <summary>
        /// load details of activity category
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClassCategory = await _db.FitnessClassCategories.FindAsync(id);

            if (fitnessClassCategory == null)
            {
                return NotFound();
            }
            return View(fitnessClassCategory);
        }


        /// <summary>
        /// loads details of activity category
        /// </summary>
        /// <returns> class category</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClassCategory = await _db.FitnessClassCategories.FindAsync(id);

            if (fitnessClassCategory == null)
            {
                return NotFound();
            }
            return View(fitnessClassCategory);
        }

        /// <summary>
        /// delete class category and it's image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var fitnessClassCategory = await _db.FitnessClassCategories.FindAsync(id);

            if (fitnessClassCategory == null)
            {
                return NotFound();
            }
            else
            {
                //get path to image folder 
                var uploads = Path.Combine(webRootPath, SD.ImageClassFolder);
                //get extension
                var extension = Path.GetExtension(fitnessClassCategory.Image);

                //if file exists
                if (System.IO.File.Exists(Path.Combine(uploads, fitnessClassCategory.Id + extension)))
                {
                    //DELETE IT
                    System.IO.File.Delete(Path.Combine(uploads, fitnessClassCategory.Id + extension));
                }
                //remove this cateogry from db
                _db.FitnessClassCategories.Remove(fitnessClassCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
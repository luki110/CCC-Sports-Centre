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
    /// Fitness ActivityCategory controller allows to perform crud opertaions on Fitness ActivityCategories
    /// </summary>
    [Authorize(Roles = "Admin, Manager, ManagerAssistant")]
    [Area("Admin")]
    public class FitnessActivityCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IHostingEnvironment _hostingEnvironment;

        public FitnessActivityCategoryController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;     
        }
        /// <summary>
        /// loads list of fitness activities categories
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            return View(_db.FitnessActivityCategories.ToList());

        }

        /// <summary>
        /// get method for creating fitness activity category
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// creates new activity category, also handles image upload
        /// </summary>
        /// <param name="fitnessActivityCategory"></param>
        /// <returns>new fitness activity category</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessActivityCategory fitnessActivityCategory)
        {
            if (ModelState.IsValid)
            {
                //add activity category to db
                _db.Add(fitnessActivityCategory);
                //save changes
                await _db.SaveChangesAsync();

                //Image being saved
                string webRootPath = _hostingEnvironment.WebRootPath;

                // files will get files uploaded from the View
                var files = HttpContext.Request.Form.Files;
                
                //get category from database
                var categoryFromDb = _db.FitnessActivityCategories.Find(fitnessActivityCategory.Id);

                //check for uploaded files
                if (files.Count != 0)
                {
                    //if image has been uploaded                 get path to folder
                    var uploads = Path.Combine(webRootPath, SD.ImageActivityFolder);
                    
                    //get extension of the file
                    var extension = Path.GetExtension(files[0].FileName);

                    //using filestream copy uploaded file to the server and rename file to Id of new activity category
                    using (var filesstream = new FileStream(Path.Combine(uploads, fitnessActivityCategory.Id + extension), FileMode.Create))
                    {
                        //copy first item to filestream
                        files[0].CopyTo(filesstream);
                    }
                    //create path to the image
                    categoryFromDb.Image = @"\" + SD.ImageActivityFolder + @"\" + fitnessActivityCategory.Id + extension;

                }
                else
                {
                    //when user does not upload image
                    //get path to default image
                    var uploads = Path.Combine(webRootPath, SD.ImageActivityFolder + @"\" + SD.DefaultActivityImage);
                    //copy the default image to folder and rename it with activity category Id
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageActivityFolder + @"\" + fitnessActivityCategory.Id + ".jpg");
                    //create path to the image
                    categoryFromDb.Image = @"\" + SD.ImageActivityFolder + @"\" + fitnessActivityCategory.Id + ".jpg";
                }
                //save changes
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(fitnessActivityCategory);

        }

        /// <summary>
        /// loads activity category details
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessActivityCategory = await _db.FitnessActivityCategories.FindAsync(id);

            if (fitnessActivityCategory == null)
            {
                return NotFound();
            }
            return View(fitnessActivityCategory);
        }

        /// <summary>
        /// Updates fitness activity category, also handles image upload if image is uploaded
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fitnessActivityCategory"></param>
        /// <returns>updated activity category</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessActivityCategory fitnessActivityCategory)
        {
            if (id != fitnessActivityCategory.Id)
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
                var categoryFromDb = _db.FitnessActivityCategories.Where(m => m.Id == fitnessActivityCategory.Id).FirstOrDefault();

                //if user uploads a new image
                if (files.Count > 0 && files[0] != null)
                {
                    //this will retrieve the folder where image is uploaded or where image already exists
                    var uploads = Path.Combine(webrootPath, SD.ImageActivityFolder);

                    //find extension of the images
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(categoryFromDb.Image);

                    //if old image exists
                    if (System.IO.File.Exists(Path.Combine(uploads, fitnessActivityCategory.Id + extension_old)))
                    {
                        //delete old image
                        System.IO.File.Delete(Path.Combine(uploads, fitnessActivityCategory.Id + extension_old));
                    }
                    //upload new file and rename it with id of fitnessActivity and extension
                    using (var filesstream = new FileStream(Path.Combine(uploads, fitnessActivityCategory.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filesstream);
                    }
                    //save path to the image as activityCategory.image
                    fitnessActivityCategory.Image = @"\" + SD.ImageActivityFolder + @"\" + fitnessActivityCategory.Id + extension_new;
                }
                //check if file was uploaded if yes update it
                if (fitnessActivityCategory.Image != null)
                {
                    categoryFromDb.Image = fitnessActivityCategory.Image;
                }
                //update rest of properties
                categoryFromDb.Name = fitnessActivityCategory.Name;
                categoryFromDb.Description = fitnessActivityCategory.Description;

                //_db.Update(fitnessClassCategory);
                //save changes
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }

            return View(fitnessActivityCategory);
        }

        /// <summary>
        /// load details of activity category
        /// </summary>
        /// <param name="id"></param>
        /// <returns> actvity category</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessActivityCategory = await _db.FitnessActivityCategories.FindAsync(id);

            if (fitnessActivityCategory == null)
            {
                return NotFound();
            }
            return View(fitnessActivityCategory);
        }


        /// <summary>
        /// loads details of activity category
        /// </summary>
        /// <returns> view</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessActivityCategory = await _db.FitnessActivityCategories.FindAsync(id);

            if (fitnessActivityCategory == null)
            {
                return NotFound();
            }
            return View(fitnessActivityCategory);
        }

        /// <summary>
        /// delete activity category and it's image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            FitnessActivityCategory fitnessActivityCategory = await _db.FitnessActivityCategories.FindAsync(id);

            if(fitnessActivityCategory == null)
            {
                return NotFound();
            }
            else
            {
                //get path to image folder 
                var uploads = Path.Combine(webRootPath, SD.ImageActivityFolder);
                //get extension 
                var extension = Path.GetExtension(fitnessActivityCategory.Image);
                //if file exists
                if(System.IO.File.Exists(Path.Combine(uploads, fitnessActivityCategory.Id + extension)))
                {
                    //DELETE IT
                    System.IO.File.Delete(Path.Combine(uploads, fitnessActivityCategory.Id + extension));
                }
                //remove this cateogry from db
                _db.FitnessActivityCategories.Remove(fitnessActivityCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
           
            
        }
    }
}
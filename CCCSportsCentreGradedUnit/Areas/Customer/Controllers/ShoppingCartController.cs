using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Extensions;
using CCCSportsCentreGradedUnit.Models;
using CCCSportsCentreGradedUnit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CCCSportsCentreGradedUnit.Areas.Customer.Controllers
{
    [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, BookingClerk, Staff, Member")]
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public BookClassViewModel BookClassVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
            BookClassVM = new BookClassViewModel
            {
                FitnessClasses = new List<Models.FitnessClass>(),
                FitnessActivities = new List<Models.FitnessActivity>()
            };

        }

        /// <summary>
        /// displays items added to shopping cart
        /// </summary>
        /// <returns>view model</returns>
        public async Task<IActionResult> Index()
        {
            //find member
            var userId = _userManager.GetUserId(HttpContext.User);
            Member member = _db.Members.Find(userId);
            BookClassVM.Member = member;
            //assign session varibles to list of integers
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            List<int> lstActivityShoppingCart = HttpContext.Session.Get<List<int>>("ssActivityShoppingCart");

            //check if this list is empty
            if (lstShoppingCart != null)
            {

                foreach (int cartItem in lstShoppingCart)
                {   //foreach item in the list find fitness class
                    FitnessClass fitnessClass = _db.FitnessClasses.Include(p => p.FitnessClassCategory).Include(p => p.Room).Where(p => p.Id == cartItem).FirstOrDefault();

                    //if found fitness class is not null
                    if (fitnessClass != null)
                    {
                        //add fitness class to view model
                        BookClassVM.FitnessClasses.Add(fitnessClass);

                        //add price of fitness class to total cost of booking
                        BookClassVM.BookingTotal += fitnessClass.Price;
                    }
                }
            }
            //check if this list is empty
            if (lstActivityShoppingCart != null)
            {
                foreach (int cartItem in lstActivityShoppingCart)
                {
                    //foreach item in the list find fitness activity
                    FitnessActivity fitnessActivity = _db.FitnessActivities.Include(p => p.FitnessActivityCategory).Include(p => p.Room).Where(p => p.Id == cartItem).FirstOrDefault();

                    //if found fitness activity is not null
                    if (fitnessActivity != null)
                    {
                        //add fitness activity to view model
                        BookClassVM.FitnessActivities.Add(fitnessActivity);

                        //add price of fitness activity to total cost of booking
                        BookClassVM.BookingTotal += fitnessActivity.Price;
                    }
                }
            }

            return View(BookClassVM);
        }
        /// <summary>
        /// creates new booking, add this booking to database, handles payment, sends email
        /// </summary>
        /// <param name="stripeEmail"></param>
        /// <param name="stripeToken"></param>
        /// <returns>booking</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(string stripeEmail, string stripeToken)
        {
            //create member variable
            Member member;
            //if user is member
            if (User.IsInRole("Member"))
            {
                //find member's id
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                //find member in database by Id
                member = _db.Members.Find(claim.Value);
                //if email provided during payment is different than users email
                if (member.Email != stripeEmail)
                {
                    //display error message
                    TempData.Add("Alert", "Wrong email! Please enter your email");
                    return RedirectToAction(nameof(Index));
                }
            }
            //if user is not member
            else
            {
                //find member by email provided during payment
                member = _db.Members.Where(m => m.Email == stripeEmail).FirstOrDefault();
                //if member is null--means wrong email
                if (member == null)
                {
                    //display error message
                    TempData.Add("Alert", "Email not found in database!");
                    return RedirectToAction(nameof(Index));
                }
                //check if member can make booking
                else if (member.CanMakeBooking == false)
                {
                    //if false display error message
                    TempData.Add("Alert", "This user havent payed membership fee!");
                    return RedirectToAction(nameof(Index));
                }

            }

            //if all above was ok assign member to view model
            BookClassVM.Member = member;

            //create new booking
            Booking booking = new Booking
            {
                BookingDate = DateTime.Now,
                MemberId = member.Id,
                Member = member

            };
            //create list of integers and pass in contents of session (fitness class)
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            //if session was not empty
            if (lstCartItems != null)
            {
                //loop through list
                foreach (int id in lstCartItems)
                {
                    //find fitness class by Id -- item from list
                    var fitnessClass = _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).SingleOrDefault(m => m.Id == id);

                    //find fitnes class bookings
                    var bookingClassList = await _db.FitnessClassBookings.Include(m => m.Booking).Include(m => m.FitnessClass).ToListAsync();

                    //loop through list of fitnessclassbookings to find if the member have already booked this class
                    foreach (FitnessClassBooking bookingClassExists in bookingClassList)
                    {
                        //check if fitness class id is the same as id from session AND if member id (who have booked this class) is the same as member id (who is trying to make booking)                       
                        if (bookingClassExists.FitnessClassId == id && bookingClassExists.Booking.MemberId == member.Id)
                        {
                            //display error message and navigate back to index page
                            TempData.Add("Alert", "This user already booked this class");

                            return RedirectToAction(nameof(Index));
                        }
                    }
                    //if found fitness class in not null
                    if (fitnessClass != null)
                    {   //add fitness class to the total of booking
                        booking.BookingTotal += fitnessClass.Price;
                        //increment number of people booked this class
                        fitnessClass.NoOfPeopleBooked++;
                        //if people booked reach the maximum capacity of room
                        if (fitnessClass.NoOfPeopleBooked == fitnessClass.Room.Capacity)
                        {
                            //make this class unvaialale
                            fitnessClass.Available = false;
                        }
                    }

                }
            }
            //create list of integers and pass in contents of session (fitness activity)
            List<int> lstActivityShoppingCart = HttpContext.Session.Get<List<int>>("ssActivityShoppingCart");
            //if session was not null
            if (lstActivityShoppingCart != null)
            {
                //loop through each id in the list
                foreach (int id in lstActivityShoppingCart)
                {
                    //find fitness acitivity by Id 
                    var fitnessActivity = _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).SingleOrDefault(m => m.Id == id);

                    //if fitness activity in not null
                    if (fitnessActivity != null)
                    {
                        //add price to total amount of booking
                        booking.BookingTotal += fitnessActivity.Price;

                        //make this activity unavailalble -- no one will be able to book it
                        fitnessActivity.Available = false;
                    }
                }
            }
            //add booking to view model
            BookClassVM.Booking = booking;
            _db.Bookings.Add(booking);
            int bookingId = booking.Id;


            //Stripe Logic
            if (stripeToken != null)
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    SourceToken = stripeToken
                });
                //create charge with folowing attributes
                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(booking.BookingTotal * 100),
                    Description = "Booking Id: " + booking.Id,
                    Currency = "gbp",
                    CustomerId = customer.Id

                });
                //payment id is needed for refunds
                booking.BookingPaymentId = charge.Id;
                //if payment was successful
                if (charge.Status.ToLower() == "succeeded")
                {
                    booking.PaymentDate = DateTime.Now;
                    booking.IsPaymentConfirmed = true;
                    //check if list of Id's of fitnessClasses is empty (session)
                    if (lstCartItems != null)
                    {
                        //loop through each item in the list
                        foreach (int id in lstCartItems)
                        {
                            //find fitness class by id
                            var fitnessClass = _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).SingleOrDefault(m => m.Id == id);
                            //if not null
                            if (fitnessClass != null)
                            {
                                //create new fitnesclassBooking
                                FitnessClassBooking fitnessClassBooking = new FitnessClassBooking
                                {
                                    BookingId = bookingId,
                                    FitnessClassId = id
                                };
                                //add this to database
                                _db.FitnessClassBookings.Add(fitnessClassBooking);
                            }

                        }
                    }
                    //check if list of Id's of fitnessActivities is empty (session)
                    if (lstActivityShoppingCart != null)
                    {
                        //loop through each item in the list
                        foreach (int id in lstActivityShoppingCart)
                        {
                            //find fitness activity by id
                            var fitnessActivity = _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).SingleOrDefault(m => m.Id == id);
                            //if not null
                            if (fitnessActivity != null)
                            {
                                //create new fitnesActivityBooking
                                FitnessActivityBooking fitnessActivityBooking = new FitnessActivityBooking
                                {
                                    BookingId = bookingId,
                                    FitnessActivityId = id
                                };
                                //add this to database
                                _db.FitnessActivityBookings.Add(fitnessActivityBooking);
                            }
                        }
                    }

                    //save changes
                    _db.SaveChanges();

                    //this will get fitness activities bookings from db if they were made in this booking just to display them in email
                    var ab = _db.FitnessActivityBookings.Where(a => a.BookingId == booking.Id).Include(a => a.Booking).Include(a => a.FitnessActivity).ToList();
                    List<FitnessActivity> activities = new List<FitnessActivity>();
                    foreach (FitnessActivityBooking a in ab)
                    {
                        FitnessActivity activity = a.FitnessActivity;

                        activities.Add(activity);
                    }
                    //this will get fitness classes bookings from db if they were made in this booking just to display them in email
                    var cb = _db.FitnessClassBookings.Where(a => a.BookingId == booking.Id).Include(a => a.Booking).Include(a => a.FitnessClass).ToList();
                    List<FitnessClass> classes = new List<FitnessClass>();

                    foreach (FitnessClassBooking c in cb)
                    {
                        FitnessClass ftclass = c.FitnessClass;

                        classes.Add(ftclass);
                    }
                    //if activity bookings list was empty send only classes in the email
                    if (activities.Count == 0)
                    {
                        await _emailSender.SendEmailAsync(member.Email, "Booking confirmation",
                         $"Your booking for booking number: {booking.Id} made on {booking.BookingDate} has been completed successfuly." +
                         $"You have booked: {classes.FirstOrDefault().FitnessClassCategory.Name} see you on {classes.FirstOrDefault().StartDate} at" +
                         $"{classes.FirstOrDefault().StartTime} " +
                         $" You have paid {booking.BookingTotal}");
                    }
                    //if classes bookings list was empty send only activities in the email
                    else if (classes.Count == 0)
                    {
                        await _emailSender.SendEmailAsync(member.Email, "Booking confirmation",
                        $"Your booking for booking number: {booking.Id} made on {booking.BookingDate} has been completed successfuly." +
                        $"You have booked: {activities.FirstOrDefault().FitnessActivityCategory.Name}see you on {activities.FirstOrDefault().StartDate} at" +
                        $"{activities.FirstOrDefault().StartTime} " +
                        $" You have paid {booking.BookingTotal}");
                    }
                    //send both in the email
                    else
                    {
                        await _emailSender.SendEmailAsync(member.Email, "Booking confirmation",
                       $"Your booking for booking number: {booking.Id} made on {booking.BookingDate} has been completed successfuly." +
                       $"You have booked: {activities.FirstOrDefault().FitnessActivityCategory.Name} this will take place on {activities.FirstOrDefault().StartDate} at " +
                       $"{activities.FirstOrDefault().StartTime} and {classes.FirstOrDefault().FitnessClassCategory.Name} this will take place on {classes.FirstOrDefault().StartDate} at" +
                        $"{classes.FirstOrDefault().StartTime} " +
                       $" You have paid {booking.BookingTotal}");
                    }

                    //empty both sessions
                    lstCartItems = new List<int>();
                    lstActivityShoppingCart = new List<int>();
                    HttpContext.Session.Set("ssShoppingCart", lstCartItems);
                    HttpContext.Session.Set("ssActivityShoppingCart", lstActivityShoppingCart);
                }

            }

            return RedirectToAction("BookingConfirmation", "ShoppingCart", new { Id = booking.Id });
        }

        /// <summary>
        /// Removes fitness class from shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveClass(int id)
        {
            //gets session
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            //loop through each item in the list
            foreach (int classId in lstCartItems)
            {
                //find fitness class
                var fitnessClass = _db.FitnessClasses.Include(m => m.Room).Include(m => m.FitnessClassCategory).SingleOrDefault(m => m.Id == id);
                //if fitness class is not null
                if (fitnessClass != null)
                {
                    //remove id from session and shopping cart
                    lstCartItems.Remove(id);
                    HttpContext.Session.Set("ssShoppingCart", lstCartItems);
                    return RedirectToAction(nameof(Index));
                }
            }



            //if (lstActivityCartItems.Count > 0 && lstActivityCartItems.Contains(id))
            //{
            //    if (lstActivityCartItems.Contains(id))
            //    {
            //        lstActivityCartItems.Remove(id);
            //        HttpContext.Session.Set("ssActivityShoppingCart", lstActivityCartItems);
            //        return RedirectToAction(nameof(Index));
            //    }

            //}

            return RedirectToAction(nameof(Index));

        }

        /// <summary>
        /// Removes fitness activity from shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveActivity(int id)
        {
            //gets session
            List<int> lstActivityCartItems = HttpContext.Session.Get<List<int>>("ssActivityShoppingCart");
            //loop through each item in the list
            foreach (int activityId in lstActivityCartItems)
            {
                //find fitness activity
                var fitnessActivity = _db.FitnessActivities.Include(m => m.Room).Include(m => m.FitnessActivityCategory).SingleOrDefault(m => m.Id == id);

                //if fitness activity is not null
                if (fitnessActivity != null)
                {
                    //remove id from session and shopping cart
                    lstActivityCartItems.Remove(id);
                    HttpContext.Session.Set("ssActivityShoppingCart", lstActivityCartItems);
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// display booking confirmation, what was booked and by who
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View model</returns>
        public IActionResult BookingConfirmation(int id)
        {
            //assign loaded booking from db to booking in view model
            BookClassVM.Booking = _db.Bookings.Include(a => a.Member).Where(a => a.Id == id).FirstOrDefault();

            //get list of fitness class bookings where fitnesclassbooking.boking id is the same as passed in id
            List<FitnessClassBooking> objFitnessClass = _db.FitnessClassBookings.Where(c => c.BookingId == id).ToList();

            //if there were any bookings
            if (objFitnessClass != null)
            {
                //loop through the list
                foreach (FitnessClassBooking classBooking in objFitnessClass)
                {   
                    //add every fitness class from db to view model-- where fitness class id is the same as fitness class id in the list
                    BookClassVM.FitnessClasses.Add(_db.FitnessClasses.Include(c => c.FitnessClassCategory).Include(c => c.Room)
                                                                .Where(c => c.Id == classBooking.FitnessClassId).FirstOrDefault());
                }
            }
            //get list of fitness activity bookings where fitnesActivityBooking.boking id is the same as passed in id
            List<FitnessActivityBooking> objFitnessActivity = _db.FitnessActivityBookings.Where(p => p.BookingId == id).ToList();

            //if there were any bookings
            if (objFitnessActivity != null)
            {
                //loop through the list
                foreach (FitnessActivityBooking activityBooking in objFitnessActivity)
                {
                    //add every fitness activity from db to view model-- where fitness activity id is the same as fitness activity id in the list
                    BookClassVM.FitnessActivities.Add(_db.FitnessActivities.Include(p => p.FitnessActivityCategory).Include(p => p.Room)
                        .Where(p => p.Id == activityBooking.FitnessActivityId).FirstOrDefault());
                }
            }



            return View(BookClassVM);
        }
    }
}

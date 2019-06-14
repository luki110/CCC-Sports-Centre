using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CCCSportsCentreGradedUnit.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin, Manager, ManagerAssistant, MembershipClerk, Member")]
    [Area("Identity")]
    
    public class MembershipPaymentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        public MembershipPaymentController(ApplicationDbContext db, IEmailSender emailSender )
        {
            _db = db;
            _emailSender = emailSender;
        }
        /// <summary>
        /// finds member's Id and loads his data from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>logged in member</returns>
        public async Task<IActionResult> Index(string id)
        {
            //create member variable
            Member member;
            //this is when user uses this method
            if (User.IsInRole("Member"))
            {
                //finds members id
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                //loads member's data from db using id
                 member = await _db.Members.Include(m => m.MembershipType).SingleOrDefaultAsync(m => m.Id == claim.Value);
            }
            //this is when member will come to the center to pay for membership and staff have handle his request, not working yet
            else
            {
                 member = await _db.Members.Include(m => m.MembershipType).SingleOrDefaultAsync(m => m.Id == id);
            }
            
            return View(member);
        }

        /// <summary>
        /// Handles membership fee payment
        /// </summary>
        /// <param name="stripeEmail"></param>
        /// <param name="stripeToken"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost(string stripeEmail, string stripeToken)
        {
            Member member;
            if (User.IsInRole("Member"))
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                member = _db.Members.Find(claim.Value);
            }
            else
            {
                member =  _db.Members.Where(m => m.Email == stripeEmail).Include(m=>m.MembershipType).FirstOrDefault();
            }
            //if provided email is not the same as stored in database for this user
            if(member.Email != stripeEmail)
            {
                TempData.Add("Alert", "Wrong email!");
                return RedirectToAction(nameof(Index));
            }
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
                var membershipList = _db.MembershipTypes.Find(member.MembershipTypeId);
                //multiply price by 12 as all payments are annualy 
                int price = Convert.ToInt32(membershipList.Price) * 12;

                var charge = charges.Create(new ChargeCreateOptions
                {

                    Amount = (price * 100),
                    Description = "Registration at CCC Sports Centre",
                    Currency = "gbp",
                    CustomerId = customer.Id
                });
                //payment id is needed for refunds... this feature is not available yet
                member.PaymentId = charge.Id;
                
                //if payment was successful
                if (charge.Status.ToLower() == "succeeded")
                {
                    
                    member.PaymentConfirmed = true;
                    member.PaymentDate = DateTime.Now;
                    member.CanMakeBooking = true;
                    member.ExpiryDate = member.PaymentDate.AddYears(1);
                    //send email
                    _emailSender.SendEmailAsync(member.Email, "Membership payment confirmation",
                        $"Welcome {member.FirstName} {member.LastName} in our sport centre your payment has been successful. " +
                        $"You have paid {price}, please keep in mind that you will have to pay your membership fee annualy. " +
                        $"Your membership will expire {member.ExpiryDate} until then you can make bookings. "+
                        $"Kind regards CCC Sports Team.");                       
                    _db.SaveChangesAsync();
                    TempData.Add("Success", "You can now book classes and activities. Enjoy!");
                    return RedirectToAction(nameof(Index));
                }

                return View();
            }
            return View();
        }
    }
}
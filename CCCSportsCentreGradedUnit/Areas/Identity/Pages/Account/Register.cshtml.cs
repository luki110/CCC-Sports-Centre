using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CCCSportsCentreGradedUnit.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CCCSportsCentreGradedUnit.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;


        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;

        }
        public string ReturnUrl { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [EnumDataType(typeof(Title))]
            [Display(Name = "Title")]
            public Title MemberTitle { get; set; }


            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and at max {1} characters long.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and at max {1} characters long.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "House Number")]
            public string HouseNumber { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(50)]
            public string Street { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(50)]
            public string City { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(50)]
            public string Country { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [MaxLength(15)]
            [Display(Name = "Post Code")]
            public string PostCode { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(Name = "Date of birth")]
            public DateTime BirthDate { get; set; }


            [Required]
            [EnumDataType(typeof(Gender))]
            [Display(Name = "Gender")]
            public Gender GenderType { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime RegistrationDate { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime Expirydate { get; set; }

            [Required]
            [Display(Name = "Select Membership Option")]
            public string MembershipTypeName { get; set; }

            //public int MembershipTypeId { get; set; }


            //public IFormFile AvatarImage { get; set; }
        }


        public void OnGet(string returnUrl = null)
        {

            ReturnUrl = returnUrl;
        }
        /// <summary>
        /// register method
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");
            //string url = Url.Page("MembershipPayment/Index");
            //returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                //var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var member = new Member
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    MemberTitle = Input.MemberTitle,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    HouseNumber = Input.HouseNumber,
                    Street = Input.Street,
                    City = Input.City,
                    Country = Input.Country,
                    PostCode = Input.PostCode,
                    BirthDate = Input.BirthDate,
                    Age = DateTime.Today.Year - Input.BirthDate.Year,
                    GenderType = Input.GenderType,
                    RegistrationDate = DateTime.Now,                    
                    CanMakeBooking = false,                    
                    MembershipType = new MembershipType()
                    {
                        Name = Input.MembershipTypeName
                    } 

                    //Input.MembershipTypeId


                  


                };

                //var membershipTypes = await _db.MembershipTypes.ToListAsync();
               
                
                //    foreach(MembershipType type in membershipTypes)
                //    {
                //        if(type.Id == Input.MembershipTypeId)
                //        {
                //                 member.MembershipType = type;
                //        }
                //    }
                //check if member isn't to old for junior membership
                if(member.Age > 16 && member.MembershipType.Name.Equals("Junior"))
                {
                    TempData.Add("Alert", "You are to old for Junior membership!");
                    return Page();
                }
               

                //creates new member
                var result = await _userManager.CreateAsync(member, Input.Password);

                //all this is because during registration new membership type is created and to delete it below actions have to be performed
                //checks what membership was selected
                if (Input.MembershipTypeName.Equals("Adult") || Input.MembershipTypeName.Equals("Junior") || Input.MembershipTypeName.Equals("Family"))
                {
                    //loads membership types from data base
                    var membershipTypes = await _db.MembershipTypes.Include(m => m.Members).ToListAsync();
                    //var membership = _db.MembershipTypes
                    //var members = _db.Members.Include(m => m.MembershipType);
                    //var memberFromDb = members.ForEachAsync(m => m.Id);
                    string name = Input.MembershipTypeName;
                    foreach (MembershipType item in membershipTypes)
                    {                       

                        //if name of selecte membership type is the same as the name of one loaded from db
                        if (name.Equals(item.Name))
                        {
                            _db.MembershipTypes.Find(item.Id);
                            //if item has price
                            if(item.Price != 0)
                            {      //assign the membership type to member
                                member.MembershipType = item;
                                member.MembershipTypeId = item.Id;
                                await _db.SaveChangesAsync();
                            }
                            //if item has no price
                            else
                            {   //delete it
                                _db.MembershipTypes.Remove(item);
                            }

                        }

                    }
                    //var membershipType = _db.MembershipTypes.Find(member.MembershipTypeId);
                    //_db.MembershipTypes.Remove(membershipType);

                    //Input.MembershipType = membershipType.Name;


                    //var membershipTypeFromDb = _db.MembershipTypes.Local.Contains(membershipType);

                    //Member memberFromDb = _db.Members.Find(member.Id);
                    //_db.MembershipTypes.Add(memberFromDb.MembershipType);
                    //_db.MembershipTypes.Add();

                }
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(member, "Member");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(member);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = member.Id, code = code },
                        protocol: Request.Scheme);
                    var price = member.MembershipType.Price * 12;
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Welcome {member.FirstName} {member.LastName}, please be aware that in order to make bookings you have to pay for your membership." +
                        $"You selected {member.MembershipType.Name} so you have to pay £ {price}" +
                        $"You can do it in you account at our website" +
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //await _signInManager.SignInAsync(member, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
            
        }
    }
}

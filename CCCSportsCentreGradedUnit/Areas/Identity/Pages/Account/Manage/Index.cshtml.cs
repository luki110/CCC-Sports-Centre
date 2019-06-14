using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CCCSportsCentreGradedUnit.Data;
using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCCSportsCentreGradedUnit.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _db = db;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

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
            
            
            [Display(Name = "Job title")]
            public string JobTitle { get; set; }
            
            [Display(Name = "Emergency Contact Name")]
            public string EmergencyContact { get; set; }
            
            [Display(Name = "Emergency Contact Phone Number")]
            public string EmergencyContDetails { get; set; }

            
            [Display(Name = "Current Qualification")]
            public string CurrentQualification { get; set; }

        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Member"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var member = _db.Members.Find(user.Id);
                var userName = await _userManager.GetUserNameAsync(user);
                var email = await _userManager.GetEmailAsync(user);
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);



                Username = userName;

                Input = new InputModel
                {
                    Email = member.Email,
                    PhoneNumber = phoneNumber,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    HouseNumber = member.HouseNumber,
                    Street = member.Street,
                    City = member.City,
                    Country = member.Country,
                    PostCode = member.PostCode,
                    BirthDate = member.BirthDate,
                    MemberTitle = member.MemberTitle,
                    GenderType = member.GenderType,


                };

                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                return Page();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var staff = _db.Staffs.Find(user.Id);
                var userName = await _userManager.GetUserNameAsync(user);
                var email = await _userManager.GetEmailAsync(user);
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);



                Username = userName;

                Input = new InputModel
                {
                    Email = staff.Email,
                    PhoneNumber = phoneNumber,
                    FirstName = staff.FirstName,
                    LastName = staff.LastName,
                    HouseNumber = staff.HouseNumber,
                    Street = staff.Street,
                    City = staff.City,
                    Country = staff.Country,
                    PostCode = staff.PostCode,
                    JobTitle = staff.JobTitle,
                    EmergencyContact = staff.EmergencyContact,
                    EmergencyContDetails = staff.EmergencyContDetails,
                    CurrentQualification = staff.CurrentQualification,


                };

                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                return Page();
            }
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Member"))
            {
                var member = _db.Members.Find(user.Id);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var email = await _userManager.GetEmailAsync(user);
                if (Input.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }

                member.FirstName = Input.FirstName;
                member.LastName = Input.LastName;
                member.HouseNumber = Input.HouseNumber;
                member.Street = Input.Street;
                member.PostCode = Input.PostCode;
                member.City = Input.City;
                member.Country = Input.Country;
                member.BirthDate = Input.BirthDate;
                member.MemberTitle = Input.MemberTitle;
                member.GenderType = Input.GenderType;





                _db.SaveChanges();
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            else
            {
                var staff = _db.Staffs.Find(user.Id);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var email = await _userManager.GetEmailAsync(user);
                if (Input.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }

                staff.FirstName = Input.FirstName;
                staff.LastName = Input.LastName;
                staff.HouseNumber = Input.HouseNumber;
                staff.Street = Input.Street;
                staff.PostCode = Input.PostCode;
                staff.City = Input.City;
                staff.Country = Input.Country;
                staff.JobTitle = Input.JobTitle;
                staff.EmergencyContact = Input.EmergencyContact;
                staff.EmergencyContDetails = Input.EmergencyContDetails;
                staff.CurrentQualification = Input.CurrentQualification;




                _db.SaveChanges();
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            
            
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}

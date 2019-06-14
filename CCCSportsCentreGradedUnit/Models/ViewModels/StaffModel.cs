using CCCSportsCentreGradedUnit.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{
    /// <summary>
    /// model for creating new staff member
    /// </summary>
    public class StaffModel
    {
        /// <summary>
        /// stores email
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        /// <summary>
        /// stores phone number
        /// </summary>
        [Required]    
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// stores password
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// confrim password, must be the same as password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// stores first name
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and at max {1} characters long.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// stores last name
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and at max {1} characters long.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        /// <summary>
        /// stores house number
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "House Number")]
        public string HouseNumber { get; set; }
        /// <summary>
        /// stores street
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string Street { get; set; }
        /// <summary>
        /// stores city 
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string City { get; set; }
        /// <summary>
        /// stores country
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string Country { get; set; }
        /// <summary>
        /// stores post code
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(15)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
        /// <summary>
        /// stores jobtitle
        /// </summary>
        [Required]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; }
        /// <summary>
        /// stores emergency contact
        /// </summary>
        [Required]
        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContact { get; set; }

        //stores emergency contact phone number
        [Required]
        [Display(Name = "Emergency Contact Phone Number")]
        public string EmergencyContDetails { get; set; }
        /// <summary>
        /// sotres current qualifiaction
        /// </summary>
        [Required]
        [Display(Name = "Current Qualification")]
        public string CurrentQualification { get; set; }
        /// <summary>
        /// stores role
        /// </summary>
        [Required]
        public Role RoleType { get; set; }
    }

}

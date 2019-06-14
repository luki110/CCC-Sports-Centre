using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// Member class
    /// </summary>
    public class Member : ApplicationUser
    {
        /// <summary>
        /// stores member's title
        /// </summary>
        [EnumDataType(typeof(Title))]
        public Title MemberTitle { get; set; }

        /// <summary>
        /// stores members birth date
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date of birth")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// stores member's age
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// stores member's gender
        /// </summary>
        [EnumDataType(typeof(Gender))]
        public Gender GenderType { get; set; }
        /// <summary>
        /// stores member's registration date
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// stores when member's account will expire
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// property to check if member can make booking
        /// </summary>
        public bool CanMakeBooking { get; set; }

        /// <summary>
        /// stores payment date 
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// stores information if member have payed for membership fee
        /// </summary>
        public bool PaymentConfirmed { get; set; }
        /// <summary>
        /// stores payment id
        /// </summary>
        public string PaymentId { get; set; }
        /// <summary>
        /// avatar image should store path to image, not used in this version of project
        /// </summary>
        public string AvatarImage { get; set; }

        //navigational properties

        /// <summary>
        /// stores membership id
        /// </summary>
        [Display(Name = "Membership Type")]
        public int MembershipTypeId { get; set; }
        /// <summary>
        /// this is foreign key to membershiptype table
        /// </summary>
        [ForeignKey("MembershipTypeId")]
        public virtual MembershipType MembershipType { get; set; }



        /// <summary>
        /// constructor with hashset of bookings
        /// </summary>
        public Member()
        {
            Bookings = new HashSet<Booking>();
        }
        /// <summary>
        /// collection of bookings for each user
        /// </summary>
        public ICollection<Booking> Bookings { get; set; }
    }
    /* public enum Membership
     {
         Adult,
         Junior,
         Family,
         OneDayAdult,
         OneDayJunior
     }*/
    /// <summary>
    /// Gender Enumeration 
    /// </summary>
    public enum Gender
    {
        Male,
        Female
    }
    /// <summary>
    /// Title enumeration
    /// </summary>
    public enum Title
    {
        Mr,
        Mrs,
        Miss,
        Ms
    }
}

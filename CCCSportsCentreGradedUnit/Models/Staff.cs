using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// Staff class, have properties specific only for staff
    /// </summary>
    public class Staff : ApplicationUser
    {
        /// <summary>
        /// gets or sets job title
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// gets or sets Emergency contact
        /// </summary>
        public string EmergencyContact { get; set; }
        /// <summary>
        /// stores emergency contact phone number
        /// </summary>
        public string EmergencyContDetails { get; set; }
        /// <summary>
        /// stores current quialification
        /// </summary>
        public string CurrentQualification { get; set; }

        /// <summary>
        /// stores role
        /// </summary>
        [EnumDataType(typeof(Role))]
        public Role RoleType { get; set; }
    }
    /// <summary>
    /// Role enumertaion
    /// </summary>
    public enum Role
    {
        Admin,
        Member,
        Staff,
        BookingClerk,
        MembershipClerk,
        Manager,
        ManagerAssistant
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }
        /// <summary>
        /// stores First name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// stores last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// stores house number
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// stores street
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// stores city
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// stores post code
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// stores country
        /// </summary>
        public string Country { get; set; }
    }
}

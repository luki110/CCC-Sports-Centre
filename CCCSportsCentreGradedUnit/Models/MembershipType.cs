using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// membership Type class
    /// </summary>
    public class MembershipType
    {
        /// <summary>
        /// stores membershiptype id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// stores name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// stores price
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// stores all members that bought this membership type
        /// </summary>
        public ICollection<Member> Members { get; set; }

    }
}

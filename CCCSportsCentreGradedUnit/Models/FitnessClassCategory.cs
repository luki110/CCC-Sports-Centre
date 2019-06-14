using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// fitnessClasscategory class
    /// </summary>
    public class FitnessClassCategory
    {
        /// <summary>
        /// stores id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// stores name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// stores description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// stores path to the image 
        /// </summary>
        public string Image { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    public class FitnessActivityCategory
    {
        /// <summary>
        /// stores id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// stores name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// stores description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// stores path to image
        /// </summary>
        public string Image { get; set; }


    }
}

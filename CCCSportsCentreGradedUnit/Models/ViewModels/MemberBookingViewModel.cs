using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{/// <summary>
/// this view model is used to display list of bookings for member or all users(for staff)
/// </summary>
    public class MemberBookingViewModel
    {      

            /// <summary>
            /// list of bookings
            /// </summary>
        public List<Booking> Bookings { get; set; }
    }
}

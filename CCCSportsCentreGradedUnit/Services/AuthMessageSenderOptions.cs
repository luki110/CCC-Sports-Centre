using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit
{
    /// <summary>
    /// class to get API key and user for SendGrid
    /// </summary>
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib
{
    public partial class User
    {
        public string UserFLName
        {
            get
            {
                return FirstName+" "+LastName;
            }
        }
        public string StatusLabel
        {
            get
            {
                if (Status == 1)
                {
                    return "Active";
                }
                return "Inactive";
            }
        }
    }
}

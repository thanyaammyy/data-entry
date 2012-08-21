using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class UserHelper
    {
        public static int GetUser(string email)
        {
            var hdc = new HotelDataEntryDataContext();
            return hdc.Users.Count(item => item.Email == email);
        }
    }
}

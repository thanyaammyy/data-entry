using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class PropertyHelper
    {
        public static IEnumerable<Property> ListCompany()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listCompany = new List<Property> { new Property() { PropertyId = 0, PropertyName = "Select Company" } };
                listCompany.AddRange(hdc.Properties.ToList());
                return listCompany;
            }
        }

        public static IEnumerable<Property> ListAlterCompany(int propertyid)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listAlterCompany = new List<Property> { new Property() { PropertyId = 0, PropertyName = "Select Alter Company" } };
                listAlterCompany.AddRange(hdc.Properties.Where(item => item.PropertyId != propertyid).ToList());
                return listAlterCompany;
            }
        }
    }
}

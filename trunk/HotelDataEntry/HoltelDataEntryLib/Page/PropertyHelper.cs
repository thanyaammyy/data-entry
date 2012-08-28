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
        public static IEnumerable<object> ListAllCompany()
        {
            var hdc = new HotelDataEntryDataContext();
            IEnumerable<object> listCompany = null;

            listCompany = (from company in hdc.Properties
                           join brand in hdc.Brands on company.BrandId equals brand.BrandId
                           join currency in hdc.Currencies on company.CurrencyId equals currency.CurrencyId
                           select new
                           {
                               company.PropertyId,
                               company.PropertyName,
                               company.PropertyCode,
                               company.StatusLabel,
                               brand.BrandName,
                               currency.CurrencyCode
                           }).ToList();
            return listCompany;
        }
    }
}

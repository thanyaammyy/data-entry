using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                var listCompany = new List<Property> { new Property() { PropertyId = 0, PropertyName = "Select a Company", PropertyCode = "Select a Company"} };
                listCompany.AddRange(hdc.Properties.ToList());
                return listCompany;
            }
        }

        public static List<Property> ListAlterCompany(int propertyid)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listAlterCompany = new List<Property> { new Property() { PropertyId = 0, PropertyName = "Select a Company", PropertyCode = "Select a Company"} };
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

        public static void AddProperty(Property property)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                hdc.Properties.InsertOnSubmit(new HotelDataEntryLib.Property
                {
                    PropertyCode = property.PropertyCode,
                    PropertyName = property.PropertyName,
                    BrandId = property.BrandId,
                    CurrencyId = property.CurrencyId,
                    UpdateDateTime = DateTime.Now,
                    Status = property.Status
                });

                try
                {
                    hdc.SubmitChanges();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateProperty(Property property)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var prop = hdc.Properties.Single(item => item.PropertyId == property.PropertyId);

                prop.PropertyName =property.PropertyName;
                prop.PropertyCode = property.PropertyCode;
                prop.BrandId = property.BrandId;
                prop.CurrencyId = property.CurrencyId;
                prop.Status = property.Status;
                prop.UpdateDateTime = DateTime.Now;

                try
                {
                    hdc.SubmitChanges();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw;
                    }
                }
            }
        }

        public static void DeleteProperty(int propertyId)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var property = hdc.Properties.Single(item => item.PropertyId == propertyId);
                hdc.Properties.DeleteOnSubmit(property);
                try
                {
                    hdc.SubmitChanges();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw;
                    }
                }
            }
        }
    }
}

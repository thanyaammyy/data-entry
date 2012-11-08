using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class PropertyHelper
    {
        public static Property GetProperty(int propId)
        {
            return new HotelDataEntryDataContext().Properties.Single(item => item.PropertyId == propId);
        }

        public static Property GetProperty(string propCode)
        {
            return new HotelDataEntryDataContext().Properties.Single(item => item.PropertyCode == propCode);
        }

        public static List<Property> ListProperites()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listCompany = new List<Property> { new Property() { PropertyId = 0, PropertyName = "Select a Property", PropertyCode = "Select a Property"} };
                listCompany.AddRange(hdc.Properties.ToList());
                return listCompany;
            }
        }

        public static List<Property> AccessProperty(int userId)
        {
            var listAccessProperty = new List<Property>();
            listAccessProperty.Add(new Property() { PropertyId = 0, PropertyName = "Select a Property", PropertyCode = "Select a Property" });
            if(userId>0)
            {
                var user = UserHelper.GetUserInfo(userId);
                var str = user.AccessProperties;
                
                if (str.Contains("N/A") || string.IsNullOrEmpty(str))
                {
                    return listAccessProperty;
                }
                if (str.Contains("OHG"))
                {
                    listAccessProperty.AddRange(Properites().Where(item=>item.PropertyCode!="OHG"));
                }
                else
                {
                    var prop = str.Split(',');
                    if (prop.Length == 1) listAccessProperty.RemoveAt(0);
                    listAccessProperty.AddRange(prop.Select(GetProperty));
                }
            }
            return listAccessProperty;
        }

        public static List<Property> Properites()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                return hdc.Properties.OrderBy(item=>item.PropertyCode).ToList();
            }
        }

        public static IEnumerable<object> ListAllProperties()
        {
            var hdc = new HotelDataEntryDataContext();
            IEnumerable<object> listCompany = null;

            listCompany = (from company in hdc.Properties
                           join currency in hdc.Currencies on company.CurrencyId equals currency.CurrencyId
                           select new
                           {
                               company.PropertyId,
                               company.PropertyName,
                               company.PropertyCode,
                               company.StatusLabel,
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

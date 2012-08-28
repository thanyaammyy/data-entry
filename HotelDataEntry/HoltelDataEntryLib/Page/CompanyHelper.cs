using System.Collections.Generic;
using System.Linq;


namespace HotelDataEntryLib.Page
{
    public static class CompanyHelper
    {
        public static IEnumerable<object> ListCompany()
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
                                          company.Status,
                                          brand.BrandName,
                                          currency.CurrencyCode
                                      }).ToList();
            return listCompany;
        }
    }
}

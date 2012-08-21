using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class BrandHelper
    {
        public static IEnumerable<Brand> ListBrand()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listBrand = new List<Brand> {new Brand() {BrandId = 0, BrandName = "Select Brand", Email = ""}};
                listBrand.AddRange(hdc.Brands.ToList());
                return listBrand;
            }
        } 
    }
}

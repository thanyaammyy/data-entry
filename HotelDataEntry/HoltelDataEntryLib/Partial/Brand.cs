
namespace HotelDataEntryLib
{
    public partial class Brand
    {
        public string BrandCodeWithName
        {
            get
            {
                return BrandId==0?BrandName:BrandCode + "--" + BrandName;
            }
        }
    }
}

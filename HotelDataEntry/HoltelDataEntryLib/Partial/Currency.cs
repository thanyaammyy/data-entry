
namespace HotelDataEntryLib
{
    public partial class Currency
    {
        public string StatusLabel
        {
            get
            {
                if ( Status== 1)
                {
                    return "Active";
                }
                return "Inactive";
            }
        }

        public string IsBaseLabel
        {
            get
            {
                if (IsBase == 1)
                {
                    return "True";
                }
                return "False";
            }
        }
    }
}

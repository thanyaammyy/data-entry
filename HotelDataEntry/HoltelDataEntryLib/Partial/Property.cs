using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib
{
    public partial class Property
    {
        public string PropertyCodeWithName
        {
            get
            {
                return PropertyId == 0 ? PropertyName : PropertyCode + "--" + PropertyName;
            }
        }
    }
}

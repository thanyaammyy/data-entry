﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
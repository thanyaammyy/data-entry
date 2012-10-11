using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelDataEntryLib.Page
{
    public static class PermissionHelper
    {
        public static List<Permission> ListPermissions()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var listPermissions = new List<Permission> { new Permission() { PermissionId = 0, PermissionName = "Select a permission", PermissionCode = 0 } };
                listPermissions.AddRange(hdc.Permissions.ToList());
                return listPermissions;
            }
        }
    }
}

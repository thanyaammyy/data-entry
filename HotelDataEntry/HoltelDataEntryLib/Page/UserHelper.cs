using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelDataEntryLib.Page
{
    public static class UserHelper
    {
        public static IEnumerable<object> ListUser()
        {
            var hdc = new HotelDataEntryDataContext();
            IEnumerable<object> listUser= null;

            listUser = (from user in hdc.Users
                           join property1 in hdc.Properties on user.PropertyId equals property1.PropertyId
                           join property2 in hdc.Properties on user.AlterPropertyId equals property2.PropertyId
                           join permission in hdc.Permissions on user.PermissionId equals permission.PermissionId
                           select new
                           {
                               user.UserId,
                               property1.PropertyCode, 
                               AlterCompany = property2.PropertyCode,
                               user.FirstName,
                               user.LastName,
                               user.Email,
                               permission.PermissionId,
                               user.StatusLabel
                           }).ToList();
            return listUser;
        }

        public static User GetUser(string email)
        {
            var user = new User();
            using(var hdc = new HotelDataEntryDataContext())
            {
                var count = hdc.Users.Count(item => item.Email == email);
                if(count !=0)
                {
                    user = hdc.Users.Single(item => item.Email == email);
                }
            }
            return user;
        }

        public static User GetUserInfo(int userId)
        {
            var hdc = new HotelDataEntryDataContext();
            return hdc.Users.Single(item => item.UserId == userId);
        }

        public static void AddUserProfile(User user)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                hdc.Users.InsertOnSubmit(user);
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

        public static void UpdateUserProfile(User user)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var getUser = (from dataItem in hdc.Users
                                     where dataItem.UserId == user.UserId
                                     select dataItem).First();
                getUser.FirstName = user.FirstName;
                getUser.LastName = user.LastName;
                getUser.Email = user.Email;
                getUser.PropertyId = user.PropertyId;
                getUser.AlterPropertyId = user.AlterPropertyId;
                getUser.UpdateDateTime = DateTime.Now;
                getUser.PermissionId = user.PermissionId;
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

        public static void DeleteUserProfile(int userId)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var user = hdc.Users.Single(item => item.UserId == userId);
                hdc.Users.DeleteOnSubmit(user);
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

        public static string GetAlterCompany(int id, int userId)
        {
            string selected;
            using (var hdc = new HotelDataEntryDataContext())
            {
                var propertyId = hdc.Users.Single(item => item.UserId == userId).AlterPropertyId;
                selected = propertyId == id ? "selected" : "";
            }
            return selected;
        }
    }
}

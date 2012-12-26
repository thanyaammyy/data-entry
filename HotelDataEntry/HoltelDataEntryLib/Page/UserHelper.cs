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
                        join property in hdc.Properties on user.PropertyId equals property.PropertyId
                        join permission in hdc.Permissions on user.PermissionId equals permission.PermissionId
                        select new
                        {
                            user.UserId,
                            property.PropertyCode,
                            user.AccessProperties,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            permission.PermissionName,
                            user.Username,
                            user.Position,
                            user.StatusLabel
                        }).ToList();
            return listUser;
        }

        public static User GetUser(string username)
        {
            var user = new User();
            using(var hdc = new HotelDataEntryDataContext())
            {
                var count = hdc.Users.Count(item => item.Username == username);
                if(count !=0)
                {
                    user = hdc.Users.Single(item => item.Username == username);
                }
            }
            return user;
        }

        public static bool IsUserExist(string username)
        {
            var user = false;
            using (var hdc = new HotelDataEntryDataContext())
            {
                var count = hdc.Users.Count(item => item.Username == username);
                if (count != 0)
                {
                    user = true;
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
                user.UpdateDateTime = DateTime.Now;
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
                getUser.UpdateDateTime = DateTime.Now;
                getUser.PermissionId = user.PermissionId;
                getUser.Status = user.Status;
                getUser.Position = user.Position;
                getUser.AccessProperties = user.AccessProperties;
                getUser.Username = user.Username;
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
    }
}

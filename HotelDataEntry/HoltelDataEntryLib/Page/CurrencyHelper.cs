using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using HotelDataEntryLib;

namespace HotelDataEntryLib.Page
{
    public static class CurrencyHelper
    {
        public static List<Currency> ListCurreny()
        {
            var currencyList = new HotelDataEntryDataContext().Currencies.ToList();
            return currencyList;
        } 

        public static void AddCurrency(Currency currency)
        {
            if (currency.IsBase == 1)
            {
                UpdateIsBaseCurrency();
            }
            using (var hdc = new HotelDataEntryDataContext())
            {
                hdc.Currencies.InsertOnSubmit(new HotelDataEntryLib.Currency
                                                  {
                                                      CurrencyCode = currency.CurrencyCode,
                                                      CurrencyName = currency.CurrencyName,
                                                      UpdateDateTime = DateTime.Now,
                                                      Status = currency.Status,
                                                      ConversionRate = currency.ConversionRate,
                                                      IsBase = currency.IsBase
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

        public static void UpdateIsBaseCurrency()
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var baseCurrency = hdc.Currencies.Single(item => item.IsBase == 1);
                if (baseCurrency != null)
                {
                    baseCurrency.IsBase = 0;
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

        public static void UpdateCurrency(Currency currency)
        {
            if (currency.IsBase == 1)
            {
                UpdateIsBaseCurrency();
            }
            using (var hdc = new HotelDataEntryDataContext())
            {
                var cur = hdc.Currencies.Single(item => item.CurrencyId == currency.CurrencyId);
                cur.CurrencyName = currency.CurrencyName;
                cur.CurrencyCode = currency.CurrencyCode;
                cur.Status = currency.Status;
                cur.ConversionRate = currency.ConversionRate;
                cur.UpdateDateTime = DateTime.Now;
                cur.IsBase = currency.IsBase;

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

        public static void DeleteCurrency(int currencyId)
        {
            using (var hdc = new HotelDataEntryDataContext())
            {
                var currency = hdc.Currencies.Single(item => item.CurrencyId == currencyId);
                hdc.Currencies.DeleteOnSubmit(currency);
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

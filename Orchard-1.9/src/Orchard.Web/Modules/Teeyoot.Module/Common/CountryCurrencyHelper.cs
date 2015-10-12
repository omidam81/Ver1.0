using System;
using Teeyoot.Localization;

namespace Teeyoot.Module.Common
{
    public static class CountryCurrencyHelper
    {
        public static string GetCountryCurrencyCode(Country country)
        {
            switch (country)
            {
                case Country.Indonesia:
                    return "IDR";
                case Country.Singapore:
                    return "SGD";
                case Country.Malaysia:
                    return "RM";
                case Country.Other:
                case Country.Unknown:
                    return "USD";
                default:
                    throw new ArgumentOutOfRangeException("country", country, null);
            }
        }
    }
}
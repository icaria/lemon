using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Csla;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace Lemon.Base
{
    public class StringUtil
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //This should only be used on file names, not path names
        public static string SanitizeFileName(string filename)
        {
            //Code modified from http://stackoverflow.com/questions/309485/c-sanitize-file-name

            //These characters are valid in paths, but not not in file names
            char[] InvalidPathChars = new char[] { '\\', '/', ':' };
            char[] InvalidFileChars = System.IO.Path.GetInvalidFileNameChars();

            string invalidChars = Regex.Escape(new string(InvalidPathChars.Union(InvalidFileChars).ToArray()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return Regex.Replace(filename, invalidReStr, "_");
        }

        //Same as before, but force showing a plus sign in front of positive numbers
        public static string GetSignedFormattedDecimal(decimal d, int maxDecimalPlaces)
        {
            return (d > 0 ? "+" : "") + GetFormattedDecimal(d, maxDecimalPlaces);
        }

        public static string GetFormattedDecimal(decimal d, int maxDecimalPlaces)
        {
            //This previously crashed when there was no decimal place showing (especially when maxDecimalPlaces was 0)
            //Handle this using a special case now
            if (maxDecimalPlaces == 0)
            {
                return Math.Round(d).ToString();
            }

            string s = d.ToString("N" + maxDecimalPlaces);

            char dec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            // cut off any excessive zeros
            s = s.TrimEnd('0');
            if (s.Length > 0 && s[s.Length - 1] == dec)  // if after cutting off excessive zeros, the last char is the decimal
                return s.Substring(0, s.Length - 1);  // return the number without the decimal
            else
                return s;
        }

        public static string GetFormattedDate(DateTime? date)
        {
            return date == null ? "" : date.Value.ToShortDateString();
        }

        public static string GetFormattedDateAndTime(DateTime? date)
        {
            return date == null ? "" : GetFormattedDate(date) + " " + date.Value.ToShortTimeString();
        }

        public static string GetCultureInvariantFormattedDate(DateTime? date)
        {
            return date == null ? "" : date.Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetQuantity(decimal? Quantity)
        {
            return StringUtil.GetFormattedDecimal((decimal)Quantity, 4);
        }

        public static string GetPercentageFigure(decimal? PercentageFigure)
        {
            return PercentageFigure == null ? "" : ((Decimal)PercentageFigure).ToString("0.## \\%");
        }

        public static string GetZeroPaddedNumber(int value, int minDigits)
        {
            bool neg = value < 0;
            if (neg) value = -value;

            string s = value.ToString();
            if (s.Length < minDigits)
                s = new string('0', minDigits - s.Length) + s;

            if (neg)
                s = "-" + s;

            return s;
        }

        /// <summary>
        /// Shows the City, State, and Country all on one line
        /// </summary>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        private static string CityStateCountry(string city, string state, string country)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(city == null ? "" : city);
            if (sb.Length > 0 && (state != null || country != null))
                sb.Append(", ");
            sb.Append(state == null ? "" : state);
            if (sb.Length > 0 && country != null)
                sb.Append(", ");
            sb.Append(country == null ? "" : country);
            if (sb.Length == 0) return null;

            return sb.ToString();
        }

        /// <summary>
        /// Shows the City, State on one line
        /// </summary>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string CityState(string city, string state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(city == null ? "" : city);
            if (sb.Length > 0 && (state != null))
                sb.Append(", ");
            sb.Append(state == null ? "" : state);

            if (sb.Length == 0) return null;

            return sb.ToString();
        }

        private static string CountryPostalCode(string country, string postalCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(country == null ? "" : country);
            if (sb.Length > 0 && postalCode != null)
                sb.Append("   ");
            sb.Append(postalCode == null ? "" : postalCode);

            if (sb.Length == 0) return null;
            return sb.ToString();
        }

        public static string[] GetFormattedAddress(AddressStruct address)
        {
            return GetFormattedAddress(address.Address1, address.Address2, address.City, address.State, address.Country, address.PostalCode, address.AddressRemarks);
        }

        /// <summary>
        /// Formats the address fields into 5 lines of text returned as an array of strings.
        /// </summary>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static string[] GetFormattedAddress(string address1, string address2, string city, string state, string country, string postalCode, string remarks)
        {
            if (address1 == string.Empty) address1 = null;
            if (address2 == string.Empty) address2 = null;
            if (city == string.Empty) city = null;
            if (state == string.Empty) state = null;
            if (country == string.Empty) country = null;
            if (postalCode == string.Empty) postalCode = null;
            if (remarks == string.Empty) remarks = null;

            //Count the number of lines we'd ideally like to have
            int numLinesDesired = 0;
            if (address1 != null) ++numLinesDesired;
            if (address2 != null) ++numLinesDesired;
            if (CityState(city, state) != null) ++numLinesDesired;
            if (country != null) ++numLinesDesired;
            if (postalCode != null) ++numLinesDesired;
            if (remarks != null) ++numLinesDesired;

            Debug.Assert(numLinesDesired <= 6);

            string[] output = new string[5];  //output string array
            int line = 0;  //Current line number

            if (numLinesDesired == 6)
            {
                //We need to combine 6 lines into 5 lines.  Do it as follows:
                //Address1
                //Address2
                //City, State
                //Country   Postal Code 
                //Remarks
                output[line++] = address1;
                output[line++] = address2;
                output[line++] = CityState(city, state);
                output[line++] = CountryPostalCode(country, postalCode);
                output[line++] = remarks;
            }
            else  //We have 5 or less lines, so we can just put everything on a line by itself
            {
                if (address1 != null)
                    output[line++] = address1;
                if (address2 != null)
                    output[line++] = address2;
                string cs = CityState(city, state);
                if (cs != null)
                    output[line++] = cs;
                if (country != null)
                    output[line++] = country;
                if (postalCode != null)
                    output[line++] = postalCode;
                if (remarks != null)
                    output[line++] = remarks;
            }

            //Fill up any blank lines with empty strings
            for (; line < 5; ++line)
            {
                output[line] = string.Empty;
            }
            return output;
        }

        public static string RemoveWhitespace(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static bool EqualsWithNullOrEmpty(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2);
            return s1 == s2;
        }

        public static string GetPaddedLabelAndQuantity(string s1, string s2, int width1, int width2)
        {
            string formatString = "{0," + width1.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}  {1," + width2.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}\n";
            return string.Format(formatString, s1, s2);
        }

        //Find the best match item in a list.
        //1.  Exact matches are prioritized over prefix matches.
        //2.  Case-sensitive matches are prioritized over case-insensitive matches.
        //3.  Items higher on the list are prioritized.
        public static T FindBestMatch<T>(string text, IEnumerable<T> list, Func<T, string> getName) where T : class
        {
            T caseMatch = null;  //The first exact case-insensitive match
            T prefix = null;     //The first case-sensitive prefix match
            T casePrefix = null; //The first case-insensitive prefix match

            foreach (var o in list)
            {
                var name = getName(o);
                if (name == null) continue;

                if (name.Equals(text, StringComparison.CurrentCulture))
                {
                    return o;  //Since this is the top priority, we can return immediately if we find this.
                }
                if (caseMatch == null && name.Equals(text, StringComparison.CurrentCultureIgnoreCase))
                    caseMatch = o;
                if (prefix == null && name.StartsWith(text, StringComparison.CurrentCulture))
                    prefix = o;
                if (casePrefix == null && name.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                    casePrefix = o;
            }

            return caseMatch ?? prefix ?? casePrefix;
        }

    }
}

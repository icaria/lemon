using System.Collections.Generic;
using System.Text;

namespace Lemon.Base
{
    public struct AddressStruct
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string AddressRemarks { get; set; }
        public int? AddressType { get; set; }

        public static AddressStruct EmptyAddress
        {
            get
            {
                return new AddressStruct
                {
                    Address1 = "",
                    Address2 = "",
                    City = "",
                    State = "",
                    Country = "",
                    PostalCode = "",
                    AddressRemarks = "",
                    AddressType = null
                };
            }

        }

        public AddressStruct(string address1, string address2, string city, string state, string country, string postalCode, string remarks, int? type) : this()
        {
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            AddressRemarks = remarks;
            AddressType = type;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address1) &&
                string.IsNullOrEmpty(Address2) &&
                string.IsNullOrEmpty(City) &&
                string.IsNullOrEmpty(State) &&
                string.IsNullOrEmpty(Country) &&
                string.IsNullOrEmpty(PostalCode) &&
                string.IsNullOrEmpty(AddressRemarks) &&
                AddressType == null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is AddressStruct)) return false;
            return Equals((AddressStruct)obj);
        }

        public bool Equals(AddressStruct obj)
        {
            return Address1 == obj.Address1 &&
                Address2 == obj.Address2 &&
                City == obj.City &&
                State == obj.State &&
                Country == obj.Country &&
                PostalCode == obj.PostalCode &&
                AddressRemarks == obj.AddressRemarks &&
                AddressType == obj.AddressType;
        }

        public override int GetHashCode()
        {
            return (Address1 ?? "").GetHashCode() ^
                (Address2 ?? "").GetHashCode() ^
                (City ?? "").GetHashCode() ^
                (State ?? "").GetHashCode() ^
                (Country ?? "").GetHashCode() ^
                (PostalCode ?? "").GetHashCode() ^
                (AddressRemarks ?? "").GetHashCode();
        }

        public static bool operator ==(AddressStruct a1, AddressStruct a2)
        {
            return a1.Equals(a2);
        }

        public static bool operator !=(AddressStruct a1, AddressStruct a2)
        {
            return !(a1.Equals(a2));
        }

    }
}

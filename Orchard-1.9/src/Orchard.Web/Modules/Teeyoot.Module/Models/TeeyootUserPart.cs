using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class TeeyootUserPart : ContentPart<TeeyootUserPartRecord>
    {
        public DateTime CreatedUtc
        {
            get
            {
                return Retrieve(p => p.CreatedUtc);
            }
            set
            {
                Store(p => p.CreatedUtc, value);
            }
        }
        public string PublicName
        {
            get
            {
                return Retrieve(p => p.PublicName);
            }
            set
            {
                Store(p => p.PublicName, value);
            }
        }
        public string PhoneNumber
        {
            get
            {
                return Retrieve(p => p.PhoneNumber);
            }
            set
            {
                Store(p => p.PhoneNumber, value);
            }
        }

        public string Street
        {
            get
            {
                return Retrieve(p => p.Street);
            }
            set
            {
                Store(p => p.Street, value);
            }
        }

        public string Suit
        {
            get
            {
                return Retrieve(p => p.Suit);
            }
            set
            {
                Store(p => p.Suit, value);
            }
        }

        public string City
        {
            get
            {
                return Retrieve(p => p.City);
            }
            set
            {
                Store(p => p.City, value);
            }
        }

        public string State
        {
            get
            {
                return Retrieve(p => p.State);
            }
            set
            {
                Store(p => p.State, value);
            }
        }

        public string Zip
        {
            get
            {
                return Retrieve(p => p.Zip);
            }
            set
            {
                Store(p => p.Zip, value);
            }
        }

        public string Country
        {
            get
            {
                return Retrieve(p => p.Country);
            }
            set
            {
                Store(p => p.Country, value);
            }
        }

        public string TeeyootUserCulture
        {
            get
            {
                return Retrieve(p => p.TeeyootUserCulture);
            }
            set
            {
                Store(p => p.TeeyootUserCulture, value);
            }
        }
    }
}
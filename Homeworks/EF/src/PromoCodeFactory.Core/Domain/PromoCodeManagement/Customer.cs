using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        // Списки Preferences и Promocodes 
        public ICollection<Preference> Preferences { get; set; }

        public IEnumerable<PromoCode> PromoCodes { get; set; }
    }
}
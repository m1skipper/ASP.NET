using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class CustomerPreference
        : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid PreferenceId { get; set; }
    }
}
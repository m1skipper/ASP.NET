using System;
using System.Collections.Generic;


namespace Pcf.GivingToCustomer.GraphQLHost.GraphQL
{
    public class CustomerType
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<PreferenceType> Preferences { get; set; }
        public List<PromoCodeType> PromoCodes { get; set; }
    }

    public class CreateOrEditCustomerType
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Guid> PreferenceIds { get; set; }
    }

    public class PreferenceType
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class PromoCodeType
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string ServiceInfo { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public Guid PartnerId { get; set; }
    }

    public class GivePromoCodeToCustomerType
    {
        public string ServiceInfo { get; set; }
        public string PartnerId { get; set; }
        public string PromoCodeId { get; set; }
        public string PromoCode { get; set; }
        public string PreferenceId { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string PartnerManagerId { get; set; }
    }

    public class GivePromoCodeToCustomerResultType
    {
        public string Code { get; set; }
    }
}
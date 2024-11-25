using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementations.Mapping
{
    public class CustomerMappingsProfile : Profile
    {
        public CustomerMappingsProfile()
        {
            CreateMap<CreateOrEditCustomerRequest, Customer>()
                        .ForMember(d => d.Id, map => map.Ignore())
                        .ForMember(d => d.PromoCodes, map => map.Ignore())
                        .ForMember(d => d.Preferences, map => map.Ignore());

            CreateMap<PromoCode, PromoCodeShortResponse>();

            CreateMap<Customer, CustomerResponse>()
                  .ForMember(dest => dest.PromoCodes, opt => opt.MapFrom(src => src.PromoCodes))
                  .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences.ToList()));

            CreateMap<Customer, CustomerShortResponse>();

            CreateMap<Preference, PreferenceResponse>();
            CreateMap<CreateOrEditPreferenceRequest, Preference>()
                .ForMember(d => d.Id, map => map.Ignore());
        }
    }
}

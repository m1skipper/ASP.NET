using System.Net.Http;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Integration.Dto;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using MassTransit;
using System;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerMassTransitGateway
        : IGivingPromoCodeToCustomerGateway
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public GivingPromoCodeToCustomerMassTransitGateway(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var dto = new GivePromoCodeToCustomerDto()
            {
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId
            };

            Console.WriteLine($"ReceivingFromPartner Publish {promoCode.PartnerManagerId}");

            await _publishEndpoint.Publish(dto);
        }
    }
}
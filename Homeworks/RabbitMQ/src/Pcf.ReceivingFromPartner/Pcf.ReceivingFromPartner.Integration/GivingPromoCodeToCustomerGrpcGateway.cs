using System.Net.Http;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Integration.Dto;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using System.Threading.Channels;

using Grpc.Net.Client;
using System;
using Pcf.GivingToCustomer.GrpcHost;


namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGrpcGateway
        : IGivingPromoCodeToCustomerGateway
    {
        private readonly HttpClient _httpClient;

        public GivingPromoCodeToCustomerGrpcGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var request = new GivePromoCodeToCustomerRequest()
            {
                PartnerId = promoCode.Partner.Id.ToString(),
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId.ToString(),
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId.ToString()
            };

            //var response = await _httpClient.PostAsJsonAsync("api/v1/promocodes", dto);
            //response.EnsureSuccessStatusCode();

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new CustomersService.CustomersServiceClient(channel);
            GivePromoCodeToCustomerResponse reply = await client.GivePromoCodeToCustomerAsync(request);
        }
    }
}
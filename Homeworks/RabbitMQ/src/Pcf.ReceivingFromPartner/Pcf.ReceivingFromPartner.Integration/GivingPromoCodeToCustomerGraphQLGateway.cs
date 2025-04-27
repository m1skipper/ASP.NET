using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;

using Pcf.GivingToCustomer.GrpcHost;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGrapgQLGateway
        : IGivingPromoCodeToCustomerGateway
    {
        public GivingPromoCodeToCustomerGrapgQLGateway()
        {
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

            //
            // https://github.com/graphql-dotnet/graphql-client

            var graphQLClient = new GraphQLHttpClient(
                "http://localhost:8093/graphql",
                new NewtonsoftJsonSerializer());

            var graphQLrequest = new GraphQLRequest
            {
                Query = $@"
                    mutation {{
                      givePromoCodeToCustomer(request: {{
                            serviceInfo: ""{request.ServiceInfo}"",
                            partnerId: ""{request.PartnerId}"",
                            promoCodeId: ""{request.PromoCodeId}"",
                            promoCode: ""{request.PromoCode}"",
                            preferenceId: ""{request.PreferenceId}"",
                            beginDate: ""{request.BeginDate}"",
                            endDate: ""{request.EndDate}"",
                            partnerManagerId: ""{request.PartnerManagerId}""
                      }}) 
                      {{
                         code
                      }}
                    }}
                    ",

            };

            var graphQLResponse = await graphQLClient.SendMutationAsync<object>(graphQLrequest);
        }
    }
}
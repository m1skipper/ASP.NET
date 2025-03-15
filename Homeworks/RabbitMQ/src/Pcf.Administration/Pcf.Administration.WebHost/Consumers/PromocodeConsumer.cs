using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.Administration.Core.Services;
using Pcf.ReceivingFromPartner.Integration.Dto;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromocodeConsumer : IConsumer<GivePromoCodeToCustomerDto>
    {
        private readonly AppliedPromocodesService _appliedPromocodesService;

        public PromocodeConsumer(AppliedPromocodesService appliedPromocodesService)
        {
            _appliedPromocodesService = appliedPromocodesService;
        }

        public async Task Consume(ConsumeContext<GivePromoCodeToCustomerDto> context)
        {
            Console.WriteLine($"Administration Consume {context.Message.PartnerManagerId}");
            if (context.Message.PartnerManagerId != null)
            {
                System.Guid id = (System.Guid)context.Message.PartnerManagerId;
                await _appliedPromocodesService.UpdateAppliedPromocodesAsync(id);
            }
        }
    }
}

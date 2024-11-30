using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Linq;

namespace PromoCodeFactory.WebHost.Helpers
{
    public static class PartnerPromoCodeLimitHelper
    {
        public static PartnerPromoCodeLimit GetPartnerPromoCodeActiveLimit(Partner partner)
        {
            var utcNow = DateTime.UtcNow;
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue
                && utcNow < x.EndDate);
            return activeLimit;
        }

        public static PartnerPromoCodeLimit CancelPartnerPromoCodeActiveLimit(Partner partner)
        {
            PartnerPromoCodeLimit activeLimit = GetPartnerPromoCodeActiveLimit(partner);
            if(activeLimit != null)
            {
                // Если партнеру выставляется лимит, то мы 
                // должны обнулить количество промокодов, которые партнер выдал,
                // если лимит закончился, то количество не обнуляется
                partner.NumberIssuedPromoCodes = 0;

                // Отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.UtcNow;
            }
            return activeLimit;
        }
    }
}

using AutoFixture.AutoMoq;
using AutoFixture;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace PromoCodeFactory.UnitTests.WebHost.Helpers.PartnerPromoCodeLimitHelper
{
    public class GetPartnerPromoCodeActiveLimitTest
    {
        private readonly IFixture _fixture;

        public GetPartnerPromoCodeActiveLimitTest()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));
        }

        [Fact]
        public void GetPartnerPromoCodeActiveLimitTest_LimitExpiredAndCanceled_ReturnsNull()
        {
            // Arrange
            Partner partner = _fixture.Build<Partner>().Without(p => p.PartnerLimits).Create();
            var expiredLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow - TimeSpan.FromDays(1));
            var canceledLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow + TimeSpan.FromDays(1));
            canceledLimit.CancelDate = DateTime.UtcNow;
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { expiredLimit, canceledLimit };

            // Act
            var activeLimit = PromoCodeFactory.WebHost.Helpers.PartnerPromoCodeLimitHelper.GetPartnerPromoCodeActiveLimit(partner);

            // Assert
            activeLimit.Should().BeNull();
        }

        [Fact]
        public void GetPartnerPromoCodeActiveLimitTest_ManyLimits_ReturnsActualLimit()
        {
            // Arrange
            Partner partner = _fixture.Build<Partner>().Without(p => p.PartnerLimits).Create();
            var expiredLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow - TimeSpan.FromDays(1));
            var canceledLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow + TimeSpan.FromDays(1));
            canceledLimit.CancelDate = DateTime.UtcNow;
            var actialLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow + TimeSpan.FromDays(1));
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { expiredLimit, canceledLimit, actialLimit };

            // Act
            var returnLimit = PromoCodeFactory.WebHost.Helpers.PartnerPromoCodeLimitHelper.GetPartnerPromoCodeActiveLimit(partner);

            // Assert
            returnLimit.Should().NotBeNull();
            returnLimit.Id.Should().Be(actialLimit.Id);
        }

        private PartnerPromoCodeLimit CreateBasePartnerPromoCodeLimit(Partner partner, DateTime date)
        {
            return _fixture.Build<PartnerPromoCodeLimit>()
                    .With(l => l.Partner, partner)
                    .With(l => l.PartnerId, partner.Id)
                    .With(l => l.EndDate, date)
                    .Without(l => l.CancelDate)
                    .Create();
        }
    }
}
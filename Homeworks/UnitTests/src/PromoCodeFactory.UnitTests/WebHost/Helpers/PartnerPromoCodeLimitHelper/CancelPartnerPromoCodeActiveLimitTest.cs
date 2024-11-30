using AutoFixture.AutoMoq;
using AutoFixture;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace PromoCodeFactory.UnitTests.WebHost.Helpers.PartnerPromoCodeLimitHelper
{
    public class CancelPartnerPromoCodeActiveLimitTest
    {
        private readonly IFixture _fixture;

        public CancelPartnerPromoCodeActiveLimitTest()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));
        }

        [Fact]
        public void CancelPartnerPromoCodeActiveLimitTest_LimitExpired_ReturnsNull()
        {
            // Arrange
            Partner partner = _fixture.Build<Partner>().Without(p => p.PartnerLimits).Create();
            var expiredLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow - TimeSpan.FromDays(1));
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { expiredLimit };

            // Act
            var resultCanceled = PromoCodeFactory.WebHost.Helpers.PartnerPromoCodeLimitHelper.CancelPartnerPromoCodeActiveLimit(partner);

            // Assert
            resultCanceled.Should().BeNull();
        }

        [Fact]
        public void CancelPartnerPromoCodeActiveLimitTest_LimitActual_ReturnsNotNull()
        {
            // Arrange
            Partner partner = _fixture.Build<Partner>().Without(p => p.PartnerLimits).Create();
            var actialLimit = CreateBasePartnerPromoCodeLimit(partner, DateTime.UtcNow + TimeSpan.FromDays(1));
            partner.PartnerLimits = new List<PartnerPromoCodeLimit> { actialLimit };

            // Act
            var resultCanceled = PromoCodeFactory.WebHost.Helpers.PartnerPromoCodeLimitHelper.CancelPartnerPromoCodeActiveLimit(partner);

            // Assert
            resultCanceled.Should().NotBeNull();
            resultCanceled.CancelDate.Should().NotBeNull();
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
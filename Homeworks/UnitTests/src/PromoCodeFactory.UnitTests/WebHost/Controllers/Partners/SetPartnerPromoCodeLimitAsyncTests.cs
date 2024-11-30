using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;
using FluentAssertions;
using PromoCodeFactory.WebHost.Models;
using AutoFixture.Xunit2;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;
        private readonly IFixture _fixture;
        
        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        // ИмяЕдиницыТестирования_Условие_ОжидаемыйРезультат
        // Нужно протестировать следующие Test Cases для установки партнеру(класс Partner) лимита(класс PartnerLimit) метод SetPartnerPromoCodeLimitAsync в PartnersController):

        // 1. Если партнер не найден, то также нужно выдать ошибку 404;
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            Partner nullPartner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(nullPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        // 2. Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotActive_ReturnsBadRequest(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            Partner dbPartner = _fixture.Build<Partner>().Without(p=>p.PartnerLimits).Create();
            dbPartner.Id = partnerId;
            dbPartner.IsActive = false;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().BeEquivalentTo("Данный партнер не активен");
        }

        // 3.Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes
        // если лимит закончился, то количество не обнуляется;
        // Часть 1. Лимит не закончился, количество обнуляется
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerLimitNotEnded_ResetPromocodes(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            var dtNow = DateTime.UtcNow;
            var dtDayBefore = dtNow - TimeSpan.FromDays(1);
            var dtDayAfter = dtNow + TimeSpan.FromDays(1);

            int numberPromocodes = 10;

            request.EndDate = dtDayAfter;

            Partner dbPartner = _fixture.Build<Partner>()
                .With(p=> p.Id, partnerId)
                .With(p => p.IsActive, true)
                .With(p => p.NumberIssuedPromoCodes, numberPromocodes)
                .Without(p => p.PartnerLimits).Create();

            PartnerPromoCodeLimit limit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.PartnerId, partnerId)
                .With(p => p.Partner, dbPartner)
                .With(p => p.EndDate, dtDayAfter) // !!! Время лимита ещё не вышло
                .Without(p => p.CancelDate)
                .Create();
            dbPartner.PartnerLimits = new List<PartnerPromoCodeLimit>() { limit };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();

            // если лимит закончился, то количество не обнуляется
            dbPartner.NumberIssuedPromoCodes.Should().Be(0); // !!! Сбросили количество промокодов
        }

        // 3.Если партнеру выставляется лимит, то мы должны обнулить количество промокодов, которые партнер выдал NumberIssuedPromoCodes
        // если лимит закончился, то количество не обнуляется;
        // Часть 2. Лимит закончился, количество остаётся прежним
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerLimitEnded_DontTouchPromocodes(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            var dtNow = DateTime.UtcNow;
            var dtDayBefore = dtNow - TimeSpan.FromDays(1);
            var dtDayAfter = dtNow + TimeSpan.FromDays(1);
            int numberPromocodes = 10;

            request.EndDate = dtDayAfter;

            Partner dbPartner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .With(p => p.NumberIssuedPromoCodes, numberPromocodes)
                .Without(p => p.PartnerLimits).Create();

            PartnerPromoCodeLimit limit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.PartnerId, partnerId)
                .With(p => p.Partner, dbPartner)
                .With(p => p.EndDate, dtDayBefore) // !!! Уже вышло время лимита
                .Without(p => p.CancelDate)
                .Create();
            dbPartner.PartnerLimits = new List<PartnerPromoCodeLimit>() { limit };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();

            // если лимит закончился, то количество не обнуляется
            dbPartner.NumberIssuedPromoCodes.Should().Be(numberPromocodes);
        }

        // 4. При установке лимита нужно отключить предыдущий лимит
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_CancelPreviousLimit(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            var dtNow = DateTime.UtcNow;
            var dtDayBefore = dtNow - TimeSpan.FromDays(1);
            var dtDayAfter = dtNow + TimeSpan.FromDays(1);

            request.EndDate = dtDayAfter;

            Partner dbPartner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .With(p => p.NumberIssuedPromoCodes, 1)
                .Without(p => p.PartnerLimits).Create();

            PartnerPromoCodeLimit expiredLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.PartnerId, partnerId)
                .With(p => p.Partner, dbPartner)
                .With(p => p.EndDate, dtDayBefore)
                .Without(p => p.CancelDate)
                .Create();

            PartnerPromoCodeLimit actualLimit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(p => p.PartnerId, partnerId)
                .With(p => p.Partner, dbPartner)
                .With(p => p.EndDate, dtDayAfter)
                .Without(p => p.CancelDate)
                .Create();
            dbPartner.PartnerLimits = new List<PartnerPromoCodeLimit>() { expiredLimit, actualLimit };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();

            // Предыдущий лимит должен быть отменён
            actualLimit.CancelDate.Should().NotBeNull();
        }

        // 5. Лимит должен быть больше 0;
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_LimitMustBePositive(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            request.Limit = 0;
            
            Partner dbPartner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .Without(p => p.PartnerLimits)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().BeEquivalentTo("Лимит должен быть больше 0");
        }

        // 6. Нужно убедиться, что сохранили новый лимит в базу данных (это нужно проверить Unit-тестом);
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimit_RepositoryUpdated(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            Partner dbPartner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .Without(p => p.PartnerLimits).Create();
            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();

            IDictionary<string, object> values = ((CreatedAtActionResult)result).RouteValues;
            values["id"].Should().BeEquivalentTo(dbPartner.Id);
            Guid createdLimitId = dbPartner.PartnerLimits.FirstOrDefault().Id;
            values["limitid"].Should().BeEquivalentTo(createdLimitId);
            _partnersRepositoryMock.Verify(rep => rep.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
            _partnersRepositoryMock.Verify(rep => rep.UpdateAsync(dbPartner), Times.Once());
        }

        // 7. Если у класса Partner, PartnerLimits null, то падал контроллер. Не падать, а создавать коллекцию
        [Theory, AutoData]
        public async Task SetPartnerPromoCodeLimitAsync_SetLimitNullPartnerLimits_Ok(Guid partnerId, SetPartnerPromoCodeLimitRequest request)
        {
            // Arrange
            Partner dbPartner = _fixture.Build<Partner>()
                .With(p => p.Id, partnerId)
                .With(p => p.IsActive, true)
                .Without(p => p.PartnerLimits).Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(dbPartner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
        }
    }
}
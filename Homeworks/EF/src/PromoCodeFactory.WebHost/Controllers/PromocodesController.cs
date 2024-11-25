using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository<PromoCode> _promocodes;
        private readonly IRepository<Preference> _preferences;
        private readonly IRepository<Customer> _customers;
        private readonly IRepository<CustomerPreference> _customerPreferences;

        public PromocodesController(IRepository<Customer> customers, IRepository<PromoCode> promocodes, IRepository<Preference> preferences, IRepository<CustomerPreference> customerPreferences, IMapper mapper)
        {
            _promocodes = promocodes;
            _customerPreferences = customerPreferences;
            _preferences = preferences;
            _customers = customers;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await _promocodes.GetAllAsync();
            var result = _mapper.Map<IEnumerable<PromoCode>, IEnumerable<PromoCodeShortResponse>>(promocodes);
            return Ok(result);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            // Метод должен сохранять новый промокод в базе данных и находить клиентов с
            // переданным предпочтением и добавлять им данный промокод.
            PromoCode promocode = await _promocodes.GetAll().FirstOrDefaultAsync(p => p.Code == request.PromoCode);
            if (promocode != null)
                throw new Exception($"Промокод {promocode.Code} уже зарегистрирован в базе данных");
            promocode = new();
            promocode.Code = request.PromoCode;
            promocode.PartnerName = request.PartnerName;
            promocode.ServiceInfo = request.ServiceInfo;
            promocode.BeginDate = DateTime.UtcNow;
            promocode.EndDate = promocode.BeginDate + TimeSpan.FromDays(31);

            Preference preference = await _preferences.GetAll().FirstOrDefaultAsync(p => p.Name == request.Preference);
            List<CustomerPreference> customerPreferences = await _customerPreferences.GetAll()
                    .Where(cp => cp.PreferenceId == preference.Id).ToListAsync();

            List<Guid> customerIds = customerPreferences.Select(cp => cp.CustomerId).Distinct().ToList();

            _promocodes.Add(promocode);

            if (customerIds.Count == 0)
                throw new Exception($"Нет пользователя с предпочтениями {request.Preference}");

            foreach (Guid id in customerIds)
            {
                var customer = await _customers.GetByIdAsync(id);
                customer.PromoCodes = new List<PromoCode>() { promocode };
            }

            await _promocodes.SaveChangesAsync();
            await _customers.SaveChangesAsync();
            
            return Ok();
        }
    }
}
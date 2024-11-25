using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Customer> _customers;
        private readonly IRepository<Preference> _preferences;

        public CustomersController(IRepository<Customer> customers, IRepository<Preference> preferences, IMapper mapper)
        {
            _customers = customers;
            _mapper = mapper;
            _preferences = preferences;
        }

        /// <summary>
        /// Получить список всех покупателей с основными свойствами
        /// </summary>
        /// <returns>Список классов основных свойств покупателей</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customers.GetAll().Include(c=>c.Preferences).ToListAsync();
            var shortCustomers = _mapper.Map<IEnumerable<Customer>, List<CustomerShortResponse>>(customers);
            return Ok(shortCustomers);
        }

        /// <summary>
        /// Получить полную информацию о конкретном покупателе
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <returns>Класс полной информации о покупателе, предпочтениях и промокодах</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customers.GetByIdAsync(id);
            await _customers.LoadCollectionAsync(customer, "Preferences");
            await _customers.LoadCollectionAsync(customer, "PromoCodes");
            //var customer = await _customers.GetAll().Include(c => c.Preferences).Include(c => c.PromoCodes).FirstOrDefaultAsync(c=>c.Id==id);
            var customerResponse = _mapper.Map<Customer, CustomerResponse>(customer);            
            
            return Ok(customerResponse);
        }

        /// <summary>
        /// Создать нового покупателя
        /// </summary>
        /// <param name="request">Класс информации о покупателе</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            Customer c = _mapper.Map<CreateOrEditCustomerRequest, Customer>(request);
            c.Preferences = new List<Preference>();
            _customers.Add(c);

            // Руками, допустим так. А как через mapper??
            // Х.з. как делать присвоение многие ко многим
            var newPreferences = _preferences.GetAll().Where(p => request.PreferenceIds.Contains(p.Id));
            foreach(var pref in newPreferences) c.Preferences.Add(pref);

            await _customers.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Отредактировать информацию о покупателе
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <param name="request">Информация о покупателе. Предпочтения задаются, как список id предпочтений</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            Customer c = await _customers.GetByIdAsync(id);
            _mapper.Map(request, c);

            // TODO: Руками, допустим так. А как через mapper?? Как в mapper использовать другие IRepository.
            var newPreferences = _preferences.GetAll().Where(p => request.PreferenceIds.Contains(p.Id));
            foreach (var pref in newPreferences) c.Preferences.Add(pref);

            await _customers.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Удалить покупателя
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            Customer c = await _customers.GetByIdAsync(id);
            await _customers.LoadCollectionAsync(c, "Preferences");
            await _customers.LoadCollectionAsync(c, "PromoCodes");
            _customers.Delete(c);
            await _customers.SaveChangesAsync();
            return Ok();
        }
    }
}
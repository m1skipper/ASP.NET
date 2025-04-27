using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.GraphQLHost.GraphQL
{
    public class CustomersQuery
    {
        public async Task<List<CustomerType>> GetCustomers([Service] IRepository<Customer> _customerRepository)
        {
            var customers = await _customerRepository.GetAllAsync();
            var response = customers.Select(Map).ToList();
            return response;
        }

        public async Task<CustomerType> GetCustomer([Service] IRepository<Customer> _customerRepository, Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            var response = Map(customer);
            return response;
        }

        public static CustomerType Map(Customer customer)
        {
            var customerType = new CustomerType();
            customerType.Id = customer.Id;
            customerType.Email = customer.Email;
            customerType.FirstName = customer.FirstName;
            customerType.LastName = customer.LastName;
            customerType.Preferences = customer.Preferences?.Select(x => new PreferenceType()
            {
                Id = x.PreferenceId,
                Name = x.Preference.Name
            }).ToList();

            customerType.PromoCodes = customer.PromoCodes?.Select(x => new PromoCodeType()
            {
                Id = x.PromoCode.Id,
                Code = x.PromoCode.Code,
                BeginDate = x.PromoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.PromoCode.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = x.PromoCode.PartnerId,
                ServiceInfo = x.PromoCode.ServiceInfo
            }).ToList();

            return customerType;
        }
    }

}

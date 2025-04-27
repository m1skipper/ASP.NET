using Grpc.Core;
using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Models;
using Pcf.GivingToCustomer.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.GraphQLHost.GraphQL
{
    public class CustomersMutation
    {
        public async Task<CustomerType> CreateCustomer(
            [Service] IRepository<Customer> customerRepository,
            [Service] IRepository<Preference> preferenceRepository,
            CreateOrEditCustomerType customerType)
        {
            var preferences = await preferenceRepository
                .GetRangeByIdsAsync(customerType.PreferenceIds);

            Customer customer = Map(customerType, preferences);

            await customerRepository.AddAsync(customer);
            return CustomersQuery.Map(customer);
        }

        public async Task<CustomerType> EditCustomer(
            [Service] IRepository<Customer> customerRepository,
            [Service] IRepository<Preference> preferenceRepository,
            Guid id, 
            CreateOrEditCustomerType customerType)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer != null)
            {
                var preferences = await preferenceRepository.GetRangeByIdsAsync(customerType.PreferenceIds);

                Map(customerType, preferences, customer);

                await customerRepository.UpdateAsync(customer);
                return CustomersQuery.Map(customer);
            }
            return null;
        }

        public async Task<CustomerType> DeleteCustomer([Service] IRepository<Customer> customerRepository, Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer != null)
                await customerRepository.DeleteAsync(customer);

            return CustomersQuery.Map(customer);
        }

        public async Task<GivePromoCodeToCustomerResultType> GivePromoCodeToCustomer(
            [Service]PromocodesService promocodesService, 
            GivePromoCodeToCustomerType request)
        {
            Console.WriteLine($"GivingToCustomer GraphQL {request.PartnerId}");

            GivePromoCodeRequest serviceRequest = new()
            {
                ServiceInfo = request.ServiceInfo,
                PartnerId = Guid.Parse(request.PartnerId),
                PromoCode = request.PromoCode,
                PreferenceId = Guid.Parse(request.PreferenceId),
                BeginDate = request.BeginDate,
                EndDate = request.EndDate
            };

            var result = await promocodesService.GivePromoCodesToCustomersWithPreferenceAsync(serviceRequest);

            return new GivePromoCodeToCustomerResultType()
            {
                Code = result.Code
            };
        }

        #region Mapping
        public static Customer Map(CreateOrEditCustomerType model, IEnumerable<Preference> preferences, Customer customer = null)
        {
            if (customer == null)
            {
                customer = new Customer();
                customer.Id = Guid.NewGuid();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferences.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                Preference = x,
                PreferenceId = x.Id
            }).ToList();

            return customer;
        }
        #endregion
    }
}

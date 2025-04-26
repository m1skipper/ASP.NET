using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using System.Linq;
using Preference = Pcf.GivingToCustomer.Core.Domain.Preference;
using Pcf.GivingToCustomer.Core.Models;
using Pcf.GivingToCustomer.Core.Services;

namespace Pcf.GivingToCustomer.GrpcHost.Services
{
    public class CustomersGrpcService : CustomersService.CustomersServiceBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly PromocodesService _promocodesService;

        public CustomersGrpcService(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository,
            PromocodesService promocodesService)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _promocodesService = promocodesService;
        }

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns></returns>
        public override async Task<CustomerShortResponseList> GetCustomers(VoidRequest request, ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id.ToString(),
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            var responseList = new CustomerShortResponseList();
            responseList.Customers.AddRange(response);

            return responseList;
        }

        /// <summary>
        /// Получить клиента по id
        /// </summary>
        public override async Task<CustomerResponse> GetCustomer(CustomerRequest request, ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id));
            var response = MapCustomer(customer);
            return response;
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <returns></returns>
        public override async Task<CustomerResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
        {
            var listIds = request.PreferenceIds.Select(p => Guid.Parse(p)).ToList();

            //Получаем предпочтения из бд и сохраняем большой объект
            var preferences = await _preferenceRepository
                .GetRangeByIdsAsync(listIds);

            Customer customer = MapFromModel(request, preferences);

            await _customerRepository.AddAsync(customer);

            var response = MapCustomer(customer);
            return response;
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        public override async Task<VoidResponse> EditCustomer(EditCustomerRequest request, ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id));

            if (customer != null)
            {
                var listIds = request.PreferenceIds.Select(p => Guid.Parse(p)).ToList();
                var preferences = await _preferenceRepository.GetRangeByIdsAsync(listIds);

                MapFromModel(request, preferences, customer);

                await _customerRepository.UpdateAsync(customer);
            }
            return new VoidResponse();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        public override async Task<VoidResponse> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
        {
            var customer = await _customerRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (customer != null)
                await _customerRepository.DeleteAsync(customer);

            return new VoidResponse();
        }

        public override async Task<GivePromoCodeToCustomerResponse> GivePromoCodeToCustomer(GivePromoCodeToCustomerRequest grec, ServerCallContext context)
        {        
            Console.WriteLine($"GivingToCustomer grpc {grec.PartnerId}");

            GivePromoCodeRequest request = new()
            {
                ServiceInfo = grec.ServiceInfo,
                PartnerId = Guid.Parse(grec.PartnerId),
                PromoCode = grec.PromoCode,
                PreferenceId = Guid.Parse(grec.PreferenceId),
                BeginDate = grec.BeginDate,
                EndDate = grec.EndDate
            };

            var result = await _promocodesService.GivePromoCodesToCustomersWithPreferenceAsync(request);
            return MapPromoCode(result);
        }

        #region Mappers
        private PreferenceResponse MapPreference(CustomerPreference preference)
        {
            var response = new PreferenceResponse();
            response.Id = preference.PreferenceId.ToString();
            response.Name = preference.Preference.Name;
            return response;
        }

        private PromoCodeShortResponse MapPromoCode(PromoCodeCustomer promoCode)
        {
            var response = new PromoCodeShortResponse();
            response.Id = promoCode.Id.ToString();
            response.Code = promoCode.PromoCode.Code;
            response.ServiceInfo = promoCode.PromoCode.ServiceInfo;
            response.BeginDate = promoCode.PromoCode.BeginDate.ToString();
            response.EndDate = promoCode.PromoCode.EndDate.ToString();
            response.PartnerId = promoCode.PromoCode.PartnerId.ToString();
            return response;
        }

        private GivePromoCodeToCustomerResponse MapPromoCode(PromoCode promoCode)
        {
            var response = new GivePromoCodeToCustomerResponse();
            response.Code = promoCode.Code;
            return response;
        }

        private CustomerResponse MapCustomer(Customer customer)
        {
            var response = new CustomerResponse();
            response.FirstName = customer.FirstName;

            response.Id = customer.Id.ToString();
            response.FirstName = customer.FirstName;
            response.LastName = customer.LastName;
            response.Email = customer.Email;

            if (customer.Preferences != null)
                response.Preferences.AddRange(customer.Preferences.Select(MapPreference));

            if (customer.PromoCodes != null)
                response.PromoCodes.AddRange(customer.PromoCodes.Select(MapPromoCode));
            return response;
        }

        public static Customer MapFromModel(CreateCustomerRequest model, IEnumerable<Preference> preferences)
        {
            var customer = new Customer();
            customer.Id = Guid.NewGuid();

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

        public static Customer MapFromModel(EditCustomerRequest model, IEnumerable<Preference> preferences, Customer customer)
        {
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
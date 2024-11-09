using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            EmployeeResponse employeeModel = CreateResponseModel(employee);
            return employeeModel;
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <remarks>
        /// В параметрах запроса можно дополнительно указать данные создаваемого сотрудника
        /// </remarks>
        /// <returns>Возвращает полную информацию о созданном сотруднике</returns>
        [HttpPost]
        public async Task<EmployeeResponse> CreateEmployeeAsync(string firstName = null, string lastName = null, string email = null)
        {
            Employee employee = new Employee();
            employee.FirstName = firstName ?? "";
            employee.LastName = lastName ?? "";
            employee.Email = email ?? "";
            employee.Roles = new();

            await _employeeRepository.CreateAsync(employee);
            EmployeeResponse response = CreateResponseModel(employee);
            return response;
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>        
        /// <returns>Нет возвращаемого значения</returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            if (await _employeeRepository.GetByIdAsync(id) == null)
                return NotFound();
            await _employeeRepository.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Обновить информацию о сотруднике
        /// </summary>
        /// <remarks>Изменяемые свойства передаются через параметры запроса. Часть может быть не заполнена</remarks>
        /// <returns>Возвращает полную обновленную информацию о сотруднике</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployeeAsync(Guid id, string firstName = null, string lastName = null, string email = null)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            if (firstName != null)
                employee.FirstName = firstName;
            if (lastName != null)
                employee.LastName = lastName;
            if (email != null)
                employee.Email = email;

            // AppliedPromocodesCount не даем менять напрямую, потому что это число видимо выставляется каким то сервисом.
            // По ролям вопрос?, кто назначает роли, тот же сервис, что и с сотрудником работает, или какой-то другой по назначению прав.
            // Там могут быть разные права доступа.

            return CreateResponseModel(employee);
        }

        #region Helpers
        private EmployeeResponse CreateResponseModel(Employee employee)
        {
            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            return employeeModel;
        }
        #endregion
    }
}
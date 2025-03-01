using System;
using System.Threading.Tasks;
using FluentAssertions;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.WebHost.Controllers;
using Xunit;

namespace Pcf.Administration.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(DatabaseCollection.DbCollection)]
    public class EmployeesControllerTests: IClassFixture<DatabaseFixture>
    {
        private MongoRepository<Employee> _employeesRepository;
        private MongoRepository<Role> _rolesRepository;
        private EmployeesController _employeesController;

        public EmployeesControllerTests(DatabaseFixture databaseFixture)
        {
            _employeesRepository = new MongoRepository<Employee>(databaseFixture.DbContext);
            _rolesRepository = new MongoRepository<Role>(databaseFixture.DbContext);
            _employeesController = new EmployeesController(_employeesRepository, _rolesRepository);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ExistedEmployee_ExpectedId()
        {
            //Arrange
            var expectedEmployeeId = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f");

            //Act
            var result = await _employeesController.GetEmployeeByIdAsync(expectedEmployeeId);

            //Assert
            result.Value.Id.Should().Be(expectedEmployeeId);
        }
    }
}
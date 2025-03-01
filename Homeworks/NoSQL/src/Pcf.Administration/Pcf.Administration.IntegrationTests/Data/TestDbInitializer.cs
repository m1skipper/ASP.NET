using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.IntegrationTests.Data
{
    public class TestDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public TestDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            _dataContext.DropDatabase();

            _dataContext.Roles.InsertMany(FakeDataFactory.Roles);
            _dataContext.Employees.InsertMany(FakeDataFactory.Employees);
        }

        public void CleanDb()
        {
            _dataContext.DropDatabase();
        }
    }
}
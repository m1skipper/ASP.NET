
using MongoDB.Driver;
using Pcf.Administration.DataAccess.Repositories;
using SharpCompress.Common;
using System.Xml.Linq;

namespace Pcf.Administration.DataAccess.Data
{
    public class MongoDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;

        public MongoDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            _dataContext.DropDatabase();

            _dataContext.Roles.InsertMany(FakeDataFactory.Roles);
            _dataContext.Employees.InsertMany(FakeDataFactory.Employees);
        }
    }
}
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.DataAccess
{
    public class DataContext
    {
        public readonly IMongoDatabase Database;
        
        private readonly MongoClient _client;
        private readonly string _databaseName;

        public IMongoCollection<Employee> Employees => Database.GetCollection<Employee>(nameof(Employee));
        public IMongoCollection<Role> Roles => Database.GetCollection<Role>(nameof(Role));

        public DataContext(MongoDBSettings mongoDBSettings)
        {
            _client = new MongoClient(mongoDBSettings.Connection);
            _databaseName = mongoDBSettings.DatabaseName;
            Database = _client.GetDatabase(mongoDBSettings.DatabaseName);
        }

        public void DropDatabase()
        {
            _client.DropDatabase(_databaseName);
        }
    }
}

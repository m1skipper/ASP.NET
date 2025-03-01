using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.IntegrationTests
{
    public class TestDataContext
        : DataContext
    {
        public TestDataContext() : base(new MongoDBSettings()
        {
            Connection = "mongodb://admin:docker@localhost:27018/",
            DatabaseName = "testAdministrationDb"
        })
        {
        }
    }
}
using System;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Pcf.Administration.IntegrationTests.Data;
using System.Threading;

namespace Pcf.Administration.IntegrationTests
{
    public class DatabaseFixture: IDisposable
    {
        private readonly TestDbInitializer _testDbInitializer;
        private static bool _runOnce;
        
        public DatabaseFixture()
        {
            Setup();

            DbContext = new TestDataContext();

            _testDbInitializer= new TestDbInitializer(DbContext);
            _testDbInitializer.InitializeDb();
        }

        public void Setup()
        {
            if (_runOnce == false)
            {
                _runOnce = true;
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
        }

        public void Dispose()
        {
            _testDbInitializer.CleanDb();
        }

        public TestDataContext DbContext { get; private set; }
    }
}
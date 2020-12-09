namespace Serenity.Test.Core.Caching.BatchGenerationUpdaterTest
{
    using FakeItEasy;
    using Serenity.Abstractions;
    using Serenity.Data;
    using Serenity.Services;
    using Serenity.Testing;
    using System.Data;
    using Xunit;

    public class BatchGenerationUpdaterTests
    {

        [Fact]
        public void TwoLevelCache_BatchGenerationUpdateMustUseTwoLevelCachedAttributeForLinkedRows()
        {
            using (new MunqContext())
            {
                var localCache = A.Fake<ILocalCache>();
                var distributedCache = A.Fake<IDistributedCache>();
                var dbConnection = A.Fake<IDbConnection>();
                var dbTransaction = A.Fake<IDbTransaction>();
                var unitOfWork = A.Fake<UnitOfWork>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(localCache);
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(distributedCache);

                A.CallTo(() => dbConnection.BeginTransaction()).Returns(dbTransaction);
                A.CallTo(() => dbConnection.State).Returns(ConnectionState.Open);
                A.CallTo(() => dbTransaction.Commit()).Invokes(() => { });
                A.CallTo(() => dbTransaction.Dispose()).Invokes(() => { });

                var row = new CacheTestRow();
                BatchGenerationUpdater.OnCommit(unitOfWork, row);
                unitOfWork.Commit();

                A.CallTo(() => localCache.Remove("Default.CacheTests")).MustHaveHappenedOnceExactly();
                A.CallTo(() => localCache.Remove("Default.CacheLinkedTests")).MustHaveHappenedOnceExactly();
                A.CallTo(() => distributedCache.Set<object>("Default.CacheTests", null)).MustHaveHappenedOnceExactly();
                A.CallTo(() => distributedCache.Set<object>("Default.CacheLinkedTests", null)).MustHaveHappenedOnceExactly();
            }
        }
    }
}
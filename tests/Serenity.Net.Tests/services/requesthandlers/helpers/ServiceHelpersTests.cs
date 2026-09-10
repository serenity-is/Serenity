namespace Serenity.Services;

public class ServiceHelpersTests
{
    public class TreeOrderingTests
    {
        private class Node(int id, int? parentId)
        {
            public int Id { get; } = id;
            public int? ParentId { get; } = parentId;
        }

        private static List<int> SortIds(IEnumerable<Node> nodes)
        {
            return [.. TreeOrdering.Sort(nodes, x => x.Id, x => x.ParentId).Select(x => x.Id)];
        }

        [Fact]
        public void Sort_Returns_Empty_For_Empty_List()
        {
            Assert.Empty(SortIds([]));
        }

        [Fact]
        public void Sort_Orders_Parents_Before_Children()
        {
            var result = SortIds([
                new Node(3, 2),
                new Node(1, null),
                new Node(2, 1)
            ]);

            Assert.Equal([1, 2, 3], result);
        }

        [Fact]
        public void Sort_Treats_Orphans_As_Roots()
        {
            var result = SortIds([
                new Node(2, 99),
                new Node(1, null)
            ]);

            Assert.Equal([2, 1], result);
        }

        [Fact]
        public void Sort_Handles_Cycles_Without_Infinite_Loop()
        {
            var result = SortIds([
                new Node(1, null),
                new Node(2, 1),
                new Node(1, 2)
            ]);

            Assert.Equal([1, 2, 1], result);
        }
    }

    public class ServiceHelperTests
    {
        [Fact]
        public void SetSkipTakeTotal_Copies_Skip_And_Take()
        {
            var response = new ListResponse<IdNameRow>();
            var query = new SqlQuery().Skip(5).Take(10);

            response.SetSkipTakeTotal(query);

            Assert.Equal(5, response.Skip);
            Assert.Equal(10, response.Take);
        }

        [Fact]
        public void SetSkipTakeTotal_Sets_TotalCount_When_No_Take()
        {
            var response = new ListResponse<IdNameRow>();
            response.Entities.Add(new IdNameRow { ID = 1 });
            response.Entities.Add(new IdNameRow { ID = 2 });
            var query = new SqlQuery().Skip(3);

            response.SetSkipTakeTotal(query);

            Assert.Equal(3, response.Skip);
            Assert.Equal(0, response.Take);
            Assert.Equal(5, response.TotalCount);
        }
    }

    public class TwoLevelCacheInvalidationExtensionsTests
    {
        [TwoLevelCached("Key1", "Key2")]
        [TwoLevelCached(typeof(LinkedRow))]
        private class CachedRow : Row<CachedRow.RowFields>, IRow
        {
            public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

            public class RowFields : RowFieldsBase
            {
#pragma warning disable CS0649
                public Int32Field ID;
#pragma warning restore CS0649
            }
        }

        private class LinkedRow : Row<LinkedRow.RowFields>, IRow
        {
            public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

            public class RowFields : RowFieldsBase
            {
#pragma warning disable CS0649
                public Int32Field ID;
#pragma warning restore CS0649
            }
        }

        private class PlainRow : Row<PlainRow.RowFields>, IRow
        {
            public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

            public class RowFields : RowFieldsBase
            {
#pragma warning disable CS0649
                public Int32Field ID;
#pragma warning restore CS0649
            }
        }

        [Fact]
        public void InvalidateOnCommit_Throws_For_Null_Cache()
        {
            Assert.Throws<ArgumentNullException>(() =>
                TwoLevelCacheInvalidationExtensions.InvalidateOnCommit(null, new MockUnitOfWork(), "Key"));
        }

        [Fact]
        public void InvalidateOnCommit_Throws_For_Empty_GroupKey()
        {
            var cache = new NullTwoLevelCache();
            Assert.Throws<ArgumentNullException>(() =>
                cache.InvalidateOnCommit(new MockUnitOfWork(), (string)null));
            Assert.Throws<ArgumentNullException>(() =>
                cache.InvalidateOnCommit(new MockUnitOfWork(), ""));
        }

        [Fact]
        public void InvalidateOnCommit_Registers_And_Invokes_Updater()
        {
            var cache = new NullTwoLevelCache();
            var uow = new MockUnitOfWork();

            cache.InvalidateOnCommit(uow, "Key");

            Assert.NotEmpty(uow.OnCommitInvocationList);
            uow.Commit();
        }

        [Fact]
        public void InvalidateOnCommit_Throws_For_Null_Fields()
        {
            var cache = new NullTwoLevelCache();
            Assert.Throws<ArgumentNullException>(() =>
                cache.InvalidateOnCommit(new MockUnitOfWork(), (RowFieldsBase)null));
        }

        [Fact]
        public void InvalidateOnCommit_Fields_Processes_TwoLevelCached_Attributes()
        {
            var cache = new NullTwoLevelCache();
            var uow = new MockUnitOfWork();

            cache.InvalidateOnCommit(uow, new CachedRow().GetFields());

            Assert.NotEmpty(uow.OnCommitInvocationList);
            uow.Commit();
        }

        [Fact]
        public void InvalidateOnCommit_Fields_Without_Declaring_Attributes()
        {
            var cache = new NullTwoLevelCache();
            var uow = new MockUnitOfWork();

            cache.InvalidateOnCommit(uow, new LinkedRow().GetFields());

            Assert.NotEmpty(uow.OnCommitInvocationList);
        }

        [Fact]
        public void InvalidateOnCommit_Throws_For_Null_Row()
        {
            var cache = new NullTwoLevelCache();
            Assert.Throws<ArgumentNullException>(() =>
                cache.InvalidateOnCommit(new MockUnitOfWork(), (IRow)null));
        }

        [Fact]
        public void InvalidateOnCommit_Row_Processes_TwoLevelCached_Attributes()
        {
            var cache = new NullTwoLevelCache();
            var uow = new MockUnitOfWork();

            cache.InvalidateOnCommit(uow, new CachedRow());

            Assert.NotEmpty(uow.OnCommitInvocationList);
            uow.Commit();
        }

        [Fact]
        public void InvalidateOnCommit_Row_Without_Declaring_Attributes()
        {
            var cache = new NullTwoLevelCache();
            var uow = new MockUnitOfWork();

            cache.InvalidateOnCommit(uow, new PlainRow());

            Assert.NotEmpty(uow.OnCommitInvocationList);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using EFCoreHooks.Attributes;
using EFCoreHooks.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EFCoreHooks.Test
{
    public class FakeModel
    {
        public int ID { get; set; }
    }

    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        public DbSet<FakeModel> FakeModels { get; set; }
    }

    public class DbHookManagerFixture
    {
        public DbHookManager<OnBeforeCreate> DbHookManager { get; }

        public FakeDbContext FakeDbContext { get; }

        public DbHookManagerFixture()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<FakeDbContext>().UseInMemoryDatabase("fake_db").Options;

            var logger = provider.GetRequiredService<ILogger<DbHookManager<OnBeforeCreate>>>();
            DbHookManager = new DbHookManager<OnBeforeCreate>(logger, provider);
            FakeDbContext = new FakeDbContext(options);

            DbHookManager.InitializeForContext(FakeDbContext);
        }
    }

    public class DbHookManagerTest : IClassFixture<DbHookManagerFixture>, IDisposable
    {
        private readonly ITestOutputHelper _log;
        private readonly DbHookManagerFixture _fixture;

        public DbHookManagerTest(ITestOutputHelper log, DbHookManagerFixture fixture)
        {
            _log = log;
            _fixture = fixture;
        }
        
        public void Dispose()
        {
            _fixture.FakeDbContext.Dispose();
        }

        [OnBeforeCreate(typeof(FakeModel))]
        public static void TestBeforeCreateHook()
        {
        }

        [OnBeforeCreate(typeof(FakeModel))]
        public static async Task TestBeforeCreateHookAsync(FakeModel model, FakeDbContext context)
        {
            if (model.ID == 1)
            {
                context.Add(new FakeModel());
                await Task.Delay(TimeSpan.FromSeconds(2));
                await context.SaveChangesAsync();
            }
            
        }

        [Fact]
        public void TestAsyncHooks()
        {
            var context = _fixture.FakeDbContext;
            context.FakeModels.Add(new FakeModel());
            var entityEntry = context.ChangeTracker.Entries().First();
            _fixture.DbHookManager.ExecuteForEntity(context, entityEntry);
        }

        
    }
}
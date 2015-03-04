using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Data;
using Serenity.Logging;
using Serenity.Test.Data;
using Serenity.Testing;
using System;
using System.Threading;
using Xunit;

namespace Serenity.Test.Logging
{
    public partial class SqlLoggerTests
    {
        private DbTestContext NewDbTestContext()
        {
            return new DbTestContext(DbOverride.New<SerenityDbScript>("Serenity", "DBSerenity"));
        }

        [Fact]
        public void SqlLogger_WorksProperly()
        {
            using (new MunqContext())
            {
                var fld = SystemLogRow.Fields;

                var registrar = Dependency.Resolve<IDependencyRegistrar>();
                var fakeConfig = A.Fake<IConfigurationRepository>();
                A.CallTo(() => fakeConfig.Load(null))
                    .WithAnyArguments()
                    .ReturnsLazily((Type t) => 
                        JSON.ParseTolerant(JSON.Stringify(new
                        {
                            Level = "Debug",
                            ConnectionKey = "Serenity",
                            InsertCommand = new SqlInsert(fld.TableName)
                                .SetTo(fld.EventDate, "@date")
                                .SetTo(fld.LogLevel, "@level")
                                .SetTo(fld.LogMessage, "@message")
                                .SetTo(fld.Exception, "@exception")
                                .SetTo(fld.SourceType, "@source")
                                .ToString()
                        }), t));

                registrar.RegisterInstance<IConfigurationRepository>("Application", fakeConfig);

                using (var context = NewDbTestContext())
                {
                    context.ExpireOverrides();
                    var logger = new SqlLogger();
                    registrar.RegisterInstance<ILogger>(logger);

                    Log.MinimumLevel = LoggingLevel.Debug;
                    Log.Debug("Hello1", new Exception("SomeException1"), this.GetType());
                    Log.Info("Hello2", new Exception("SomeException2"), this.GetType());
                    Log.Warn("Hello3", new Exception("SomeException3"), this.GetType());

                    using (var connection = SqlConnections.NewByKey("Serenity"))
                    {
                        logger.Flush();
                        var items = connection.List<SystemLogRow>(q => q.SelectTableFields().OrderBy(fld.ID, desc: true).Take(3));

                        Assert.Equal(3, items.Count);

                        var item = items[2];
                        Assert.Equal("Hello1", item.LogMessage);
                        Assert.Contains("SomeException1", item.Exception);
                        Assert.Equal("Debug", item.LogLevel);
                        Assert.Equal(this.GetType().FullName, item.SourceType);
                        Assert.True(DateTime.Now.Subtract(item.EventDate.Value).TotalSeconds < 60);

                        item = items[1];
                        Assert.Equal("Hello2", item.LogMessage);
                        Assert.Contains("SomeException2", item.Exception);
                        Assert.Equal("Info", item.LogLevel);
                        Assert.Equal(this.GetType().FullName, item.SourceType);
                        Assert.True(DateTime.Now.Subtract(item.EventDate.Value).TotalSeconds < 60);

                        item = items[0];
                        Assert.Equal("Hello3", item.LogMessage);
                        Assert.Contains("SomeException3", item.Exception);
                        Assert.Equal("Warn", item.LogLevel);
                        Assert.Equal(this.GetType().FullName, item.SourceType);
                        Assert.True(DateTime.Now.Subtract(item.EventDate.Value).TotalSeconds < 60);
                    }
                }
            }
        }
    }
}
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
                    registrar.RegisterInstance<ILogger>(new SqlLogger());

                    Log.MinimumLevel = LoggingLevel.Debug;
                    Log.Debug("Hello", new Exception("SomeMessage"), this.GetType());

                    Thread.Sleep(50);

                    using (var connection = SqlConnections.NewByKey("Serenity"))
                    {
                        var item = connection.First<SystemLogRow>(q => q.SelectTableFields().OrderBy(fld.ID, desc: true).Take(1));

                        Assert.Equal("Hello", item.LogMessage);
                        Assert.Contains("SomeMessage", item.Exception);
                        Assert.Equal("Debug", item.LogLevel);
                        Assert.Equal(this.GetType().FullName, item.SourceType);
                        Assert.True(DateTime.Now.Subtract(item.EventDate.Value).TotalSeconds < 60);
                    }
                }
            }
        }
    }
}
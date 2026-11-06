using FluentMigrator;
using Tags = FluentMigrator.TagsAttribute;

namespace Serenity.Extensions;

public class MigrationTagUtilsTests
{
    private static bool Matches(Type type, params string[] tags) =>
        MigrationTagUtils.MatchesTags(type, tags);

    [TargetDB("Default")]
    private class TaggedWithDefaultDB
    {
    }

    [Tags("db:Default")]
    private class TaggedWithHandWrittenDefaultDB
    {
    }

    [Tags("db:Default", "db:Northwind")]
    private class TaggedWithTwoHandWrittenDatabases
    {
    }

    [TargetDB("ProDataSources")]
    private class TaggedWithProDataSources
    {
    }

    [TargetDB("ProDataSources")]
    [Tags("env:Staging")]
    private class TaggedWithProDataSourcesAndStaging
    {
    }

    [TargetDB("ProDataSources")]
    [Tags(TagBehavior.RequireAll, "env:Staging", "env:Production")]
    private class TaggedWithTwoEnvironments
    {
    }

    [TargetDB("Default")]
    [Tags("Core")]
    private class TaggedWithDefaultDBAndCore
    {
    }

    [DefaultDB]
    private class TaggedWithDefaultDBAttribute
    {
    }

    [Tags("Core")]
    private class TaggedWithCore
    {
    }

    [Tags("Core")]
    [Tags("OtherDB")]
    private class TaggedWithCoreAndLegacyOtherDB
    {
    }

    [Tags(TagBehavior.RequireAny, "OtherDB", "Core")]
    private class TaggedWithMixedLegacyAndPlainTags
    {
    }

    [Tags("DefaultDB")]
    private class TaggedWithLegacyDefaultDB
    {
    }

    [Tags(TagBehavior.RequireAny, "NorthwindDB")]
    private class TaggedWithLegacyNorthwindDB
    {
    }

    private class Untagged
    {
    }

    [Tags("env:BackupDB")]
    private class TaggedWithBackupDBEnvironment
    {
    }

    [TargetDB("AnalyticsDB")]
    private class TaggedWithAnalyticsDB
    {
    }

    [TargetDB("Default")]
    private class TaggedDefaultBase
    {
    }

    private class DerivedFromTaggedDefault : TaggedDefaultBase
    {
    }

    [Tags(TagBehavior.RequireAny, "env:Staging")]
    private interface IStagingMigration
    {
    }

    private class ImplementsStagingMigration : IStagingMigration
    {
    }

    [Theory]
    [InlineData("db:Default", "db")]
    [InlineData("db:ProDataSources", "db")]
    [InlineData("env:Staging", "env")]
    [InlineData("tenant:1", "tenant")]
    [InlineData("DefaultDB", "")]
    [InlineData("BackupDB", "")]
    [InlineData("Core", "")]
    [InlineData(":Default", "")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void GetGroupReturnsTheTagDimension(string? tag, string expected)
    {
        Assert.Equal(expected, MigrationTagUtils.GetGroup(tag));
    }

    [Fact]
    public void UntaggedTypeDoesNotMatchAnything()
    {
        Assert.False(Matches(typeof(Untagged), "db:Default"));
        Assert.False(Matches(typeof(Untagged), "Core"));
        Assert.False(MigrationTagUtils.MatchesTags(typeof(Untagged), null));
        Assert.False(Matches(typeof(Untagged)));
    }

    [Fact]
    public void TaggedTypeDoesNotMatchWithoutRequestedTags()
    {
        Assert.False(MigrationTagUtils.MatchesTags(typeof(TaggedWithDefaultDB), null));
        Assert.False(Matches(typeof(TaggedWithDefaultDB)));
    }

    [Theory]
    [InlineData("db:Default")]
    [InlineData("db:Northwind", "db:Default")]
    public void TargetDBMatchesWhenItsDatabaseIsRequested(params string[] tags)
    {
        Assert.True(Matches(typeof(TaggedWithDefaultDB), tags));
    }

    [Theory]
    [InlineData("db:Northwind")]
    [InlineData("db:Default", "db:ProFeatures")]
    [InlineData("db:Other", "env:Staging")]
    public void TargetDBDoesNotMatchWhenAnotherDatabaseIsRequested(params string[] tags)
    {
        Assert.False(Matches(typeof(TaggedWithProDataSources), tags));
    }

    [Fact]
    public void TargetDBMatchesWhenAnyOfTheRequestedDatabasesIsItsOwn()
    {
        // this is the shape of the requested tags produced by DataMigrations, a union of
        // all connection keys that resolve to the database being migrated
        Assert.True(Matches(typeof(TaggedWithProDataSources),
            "db:Default", "db:ProFeatures", "db:ProDataSources"));
    }

    [Fact]
    public void TargetDBMatchesTheLegacyTagForm()
    {
        // the legacy form is normalized to the db: form on both sides, so a migration written
        // with old style tags keeps matching a runner that requests the new form
        Assert.True(Matches(typeof(TaggedWithDefaultDB), "DefaultDB"));
        Assert.True(Matches(typeof(TaggedWithDefaultDB), "DefaultDB", "Core"));
        Assert.False(Matches(typeof(TaggedWithDefaultDB), "NorthwindDB"));
    }

    [Fact]
    public void TargetDBMatchesThroughTheDerivedAttribute()
    {
        Assert.True(Matches(typeof(TaggedWithDefaultDBAttribute), "db:Default"));
        Assert.True(Matches(typeof(TaggedWithDefaultDBAttribute), "DefaultDB"));
    }

    [Fact]
    public void TargetDBMatchesThroughInheritance()
    {
        Assert.True(Matches(typeof(DerivedFromTaggedDefault), "db:Default"));
        Assert.False(Matches(typeof(DerivedFromTaggedDefault), "db:Northwind"));
    }

    [Fact]
    public void EnvironmentTagMatchesThroughInterfaces()
    {
        Assert.True(Matches(typeof(ImplementsStagingMigration), "env:Staging"));
        Assert.False(Matches(typeof(ImplementsStagingMigration), "env:Production"));
    }

    [Fact]
    public void AllDeclaredDimensionsHaveToMatch()
    {
        Assert.True(Matches(typeof(TaggedWithProDataSourcesAndStaging),
            "db:ProDataSources", "env:Staging"));

        // the database matches, but the environment is not requested (fail closed)
        Assert.False(Matches(typeof(TaggedWithProDataSourcesAndStaging), "db:ProDataSources"));

        // the database matches, but another environment is requested
        Assert.False(Matches(typeof(TaggedWithProDataSourcesAndStaging),
            "db:ProDataSources", "env:Production"));

        // the environment matches, but another database is requested
        Assert.False(Matches(typeof(TaggedWithProDataSourcesAndStaging),
            "db:Default", "env:Staging"));
    }

    [Fact]
    public void UndeclaredDimensionsInTheRequestAreIgnored()
    {
        Assert.True(Matches(typeof(TaggedWithDefaultDB), "db:Default", "env:Production"));
    }

    [Fact]
    public void MultipleValuesOfADimensionAreAllowedLikeFluentMigratorDoes()
    {
        // RequireAll means the requested tags have to be a subset of the declared ones, so a
        // migration declaring two environments runs in both of them
        Assert.True(Matches(typeof(TaggedWithTwoEnvironments), "db:ProDataSources", "env:Staging"));
        Assert.True(Matches(typeof(TaggedWithTwoEnvironments), "db:ProDataSources", "env:Production"));
        Assert.False(Matches(typeof(TaggedWithTwoEnvironments), "db:ProDataSources", "env:Testing"));
    }

    [Fact]
    public void TagsWithoutADimensionBehaveLikeFluentMigratorDoes()
    {
        Assert.True(Matches(typeof(TaggedWithCore), "Core"));
        Assert.True(Matches(typeof(TaggedWithCore), "Core", "db:Default"));

        Assert.False(Matches(typeof(TaggedWithCore), "Other"));
        Assert.False(Matches(typeof(TaggedWithCore), "db:Default"));
    }

    [Fact]
    public void TagsWithoutADimensionHaveToMatchToo()
    {
        Assert.True(Matches(typeof(TaggedWithDefaultDBAndCore), "db:Default", "Core"));
        Assert.False(Matches(typeof(TaggedWithDefaultDBAndCore), "db:Default"));
    }

    [Fact]
    public void HandWrittenDatabaseTagsAreAPartitionToo()
    {
        // [Tags("db:Default")] is RequireAll by FluentMigrator's default, but the database
        // dimension is a partition, so it should not require every requested database
        Assert.True(Matches(typeof(TaggedWithHandWrittenDefaultDB),
            "db:Default", "db:ProFeatures", "db:ProDataSources"));
        Assert.False(Matches(typeof(TaggedWithHandWrittenDefaultDB), "db:ProFeatures"));

        Assert.True(Matches(typeof(TaggedWithTwoHandWrittenDatabases), "db:Northwind"));
        Assert.True(Matches(typeof(TaggedWithTwoHandWrittenDatabases),
            "db:Default", "db:ProFeatures"));
        Assert.False(Matches(typeof(TaggedWithTwoHandWrittenDatabases), "db:Other"));
    }

    [Fact]
    public void LegacyTagsDeclaredWithoutTargetDBAttributeStillMatch()
    {
        // third party or older migrations may declare the legacy form by hand instead of using
        // TargetDBAttribute, so the legacy tags are normalized to the db: form too
        Assert.True(Matches(typeof(TaggedWithLegacyDefaultDB), "db:Default"));
        Assert.True(Matches(typeof(TaggedWithLegacyNorthwindDB), "db:Northwind"));
        Assert.False(Matches(typeof(TaggedWithLegacyNorthwindDB), "db:Default"));
    }

    [Fact]
    public void RequestedLegacyTagsAreNormalizedToTheDatabaseDimension()
    {
        // a requested legacy tag is translated to its db: form, so it matches database tags
        // and is no longer an ordinary tag that could block tags without a dimension
        Assert.True(Matches(typeof(TaggedWithDefaultDB), "DefaultDB"));
        Assert.True(Matches(typeof(TaggedWithCore), "DefaultDB", "Core"));
        Assert.True(Matches(typeof(TaggedWithCore), "Core"));
    }

    [Fact]
    public void LegacySuffixIsNotNormalizedOnOtherDimensions()
    {
        // an environment named BackupDB is not a database tag, so the suffix is only stripped
        // for tags that have no dimension
        Assert.Equal("env", MigrationTagUtils.GetGroup("env:BackupDB"));
        Assert.True(Matches(typeof(TaggedWithBackupDBEnvironment), "env:BackupDB"));
        Assert.False(Matches(typeof(TaggedWithBackupDBEnvironment), "db:Backup"));
    }

    [Fact]
    public void LegacySuffixIsStrippedOnlyOnce()
    {
        // TargetDBAttribute appends DB to the key, so the AnalyticsDB connection yields
        // AnalyticsDBDB (legacy) and db:AnalyticsDB (canonical), and both normalize the same
        Assert.True(Matches(typeof(TaggedWithAnalyticsDB), "db:AnalyticsDB"));
        Assert.True(Matches(typeof(TaggedWithAnalyticsDB), "AnalyticsDBDB"));
        Assert.False(Matches(typeof(TaggedWithAnalyticsDB), "db:Analytics"));
    }

    [Fact]
    public void LegacyTagsOnlyMatchWhenTheirDatabaseIsRequested()
    {
        // the normalized form still has to be requested, so a legacy tag is inert otherwise
        Assert.False(Matches(typeof(TaggedWithLegacyDefaultDB), "db:Northwind"));
        Assert.False(Matches(typeof(TaggedWithLegacyDefaultDB), "NorthwindDB"));
        Assert.False(Matches(typeof(TaggedWithLegacyDefaultDB), "Core"));
    }

    [Fact]
    public void LegacyTagsStayOrdinaryTagsWhenTheirDatabaseIsNotRequested()
    {
        // db:Other is not requested, so OtherDB is not treated as a database tag and does not
        // add a database requirement that the runner never asked for
        Assert.True(Matches(typeof(TaggedWithCoreAndLegacyOtherDB), "Core"));
        Assert.False(Matches(typeof(TaggedWithCoreAndLegacyOtherDB), "db:Other"));
    }

    [Fact]
    public void LegacyTagsBecomeDatabaseTagsWhenTheirDatabaseIsRequested()
    {
        // db:Other is requested, so the legacy OtherDB tag is a database tag now, and the
        // tag without a dimension that the migration declares still has to match
        Assert.True(Matches(typeof(TaggedWithCoreAndLegacyOtherDB), "db:Other", "Core"));
        Assert.False(Matches(typeof(TaggedWithCoreAndLegacyOtherDB), "db:Other"));
    }

    [Fact]
    public void MixedLegacyAndPlainTagsInRequireAnyAttributeMatchEveryDimension()
    {
        // when db:Other is requested, the legacy tag becomes a database tag, so the plain tag
        // in the same attribute still has to match: a RequireAny attribute cannot OR dimensions
        Assert.True(Matches(typeof(TaggedWithMixedLegacyAndPlainTags), "db:Other", "Core"));
        Assert.False(Matches(typeof(TaggedWithMixedLegacyAndPlainTags), "db:Other"));

        // when db:Other is not requested, the legacy tag is an ordinary tag again, so the
        // attribute keeps matching on any of its tags, as it did before tag dimensions
        Assert.True(Matches(typeof(TaggedWithMixedLegacyAndPlainTags), "Core"));
    }

    [Fact]
    public void CreateMatcherReturnsAMatchingFunction()
    {
        var matcher = MigrationTagUtils.CreateMatcher();

        Assert.True(matcher(typeof(TaggedWithProDataSourcesAndStaging), ["db:ProDataSources", "env:Staging"]));
        Assert.False(matcher(typeof(TaggedWithProDataSourcesAndStaging), ["db:ProDataSources"]));
        Assert.False(matcher(typeof(Untagged), ["db:ProDataSources"]));
    }
}

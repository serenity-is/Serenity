namespace Serenity.CodeGenerator;

public partial class RestoreNodeTypesJsonBuilderTests
{
    const string origJson = """
{
  "name": "test.web",
  "dependencies": {
    "@ser/xlib": "1.2.3",
    "@ser/xwise": "wx:*",
    "@ser/xt": "wx:*",
    "@ser/sg": "abc:*",
    "prct": "ct:s"
  },
  "devDependencies": {
    "tsb": "w:*",
    "tu": "w:*"
  },
  "scripts": {
    "a": "x.js",
    "b:c": "x --watch",
    "t": "x run"
  },
  "private": true,
  "type": "module"
}
""";

    [Fact]
    public void Handles_Pretty_Formatting_Correctly()
    {
        OrderedDictionary<string, object> dict = JSON.Parse<OrderedDictionary<string, object>>(origJson);

        var builder = new RestoreNodeTypesTask.JsonBuilder();
        builder.AppendDictionary(dict);
        var actual = builder.ToString();
        Assert.Equal(origJson, actual);
    }
}

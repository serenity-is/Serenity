using Serenity.CodeGeneration;

namespace Serenity.Net.CodeGenerator.Tests;

public class LocalTextResourcesTests
{
    [Fact]
    public void Reads_The_Keys_Of_A_File_With_Their_Text()
    {
        var texts = LocalTextResources.Read("""
            {
              "__comment__": "Auto generated from the assembly to use as a template. Don't modify.",
              "Db.Northwind.Customer.Fax": "Fax",
              "Db.Northwind.Customer.City": "City",
              "Site.AdvancedSamples.ProCustomers.TabOrders": "Orders"
            }
            """);

        Assert.Equal(3, texts.Count);
        Assert.Equal("Fax", texts["Db.Northwind.Customer.Fax"]);
        Assert.Equal("Orders", texts["Site.AdvancedSamples.ProCustomers.TabOrders"]);
    }

    [Fact]
    public void Reads_The_Keys_Of_A_File_That_Nests_Them()
    {
        var texts = LocalTextResources.Read("""
            {
              "Db": {
                "Northwind": {
                  "Customer": {
                    "Fax": "Fax"
                  }
                }
              }
            }
            """);

        Assert.Equal("Fax", Assert.Single(texts).Value);
        Assert.Equal("Db.Northwind.Customer.Fax", texts.Keys.Single());
    }

    [Fact]
    public void Reads_A_Key_That_A_Translation_Leaves_Empty_As_One_Without_Text()
    {
        var texts = LocalTextResources.Read("""
            {
              "Db.Northwind.Customer.Fax": "Fax",
              "Db.Northwind.Customer.City": null,
              "Db.Northwind.Customer.Country": ""
            }
            """);

        Assert.Equal("Fax", texts["Db.Northwind.Customer.Fax"]);
        Assert.Null(texts["Db.Northwind.Customer.City"]);
        Assert.Equal("", texts["Db.Northwind.Customer.Country"]);
    }

    [Fact]
    public void Merges_The_Keys_Of_The_Packages_Of_A_Project()
    {
        var texts = LocalTextResources.Merge([
            LocalTextResources.Read("""{ "Site.Layout.Logout": "Logout", "Db.A.B": "B" }"""),
            LocalTextResources.Read("""{ "Site.Layout.Logout": "", "Dataexplorer.X": "X" }""")
        ]);

        Assert.Equal(3, texts.Count);
        // the first text that one of the packages gives a key is the one that it keeps
        Assert.Equal("Logout", texts["Site.Layout.Logout"]);
        Assert.Equal("X", texts["Dataexplorer.X"]);
    }

    [Fact]
    public void Reads_No_Keys_Of_An_Empty_File()
    {
        Assert.Empty(LocalTextResources.Read(null));
        Assert.Empty(LocalTextResources.Read(""));
        Assert.Empty(LocalTextResources.Read("{}"));
    }
}

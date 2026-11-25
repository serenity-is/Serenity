namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.HarcodedValues", Permission = Northwind.PermissionKeys.General)]
public class HardcodedValuesForm
{
    [DisplayText("Some Value")]
    [HardcodedValuesEditor]
    public string? SomeValue { get; set; }
}
namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.HarcodedValues")]
public class HardcodedValuesForm
{
    [DisplayText("Some Value")]
    [HardcodedValuesEditor]
    public string? SomeValue { get; set; }
}
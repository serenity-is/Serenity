namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.HarcodedValues")]
public class HardcodedValuesForm
{
    [DisplayName("Some Value")]
    [HardcodedValuesEditor]
    public String SomeValue { get; set; }
}
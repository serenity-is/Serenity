// Types shaped like sergen's generated form/columns classes for an entity in the
// Default module: a root (project) namespace, the module namespace, and the
// .Forms / .Columns namespace. Their keys are inferred from that metadata.
namespace Acme.Default.Forms
{
    [Serenity.ComponentModel.FormScript(InferKey = true)]
    public class VersionInfoForm
    {
    }

    [Serenity.ComponentModel.FormScript(InferKey = false)]
    public class ExplicitFalseForm
    {
    }
}

namespace Acme.Default.Columns
{
    [Serenity.ComponentModel.ColumnsScript(InferKey = true)]
    public class VersionInfoColumns
    {
    }

    [Serenity.ComponentModel.ColumnsScript(InferKey = false)]
    public class ExplicitFalseColumns
    {
    }
}

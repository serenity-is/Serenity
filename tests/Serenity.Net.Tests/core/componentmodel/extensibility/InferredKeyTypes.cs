// Types shaped like generated form/columns classes: a root (project) namespace,
// the module namespace and the .Forms / .Columns / .Lookups namespace. Their keys
// are inferred from that metadata, or from a [Module] attribute when present.
namespace Acme.Core.Forms
{
    [FormScript(InferKey = true)]
    public class VersionInfoForm
    {
    }

    public class SaveRequest
    {
    }
}

namespace Acme.Core.Columns
{
    [ColumnsScript(InferKey = true)]
    public class VersionInfoColumns
    {
    }
}

namespace Acme.Core.Lookups
{
    [FormScript(InferKey = true)]
    public class CategoryLookupForm
    {
    }
}

namespace Acme.Core.Modules
{
    [Module("SomeModule")]
    [FormScript(InferKey = true)]
    public class ModuleForm
    {
    }

    [Module("SomeModule")]
    [FormScript(InferKey = true)]
    [BasedOnRow(typeof(ModuleRow))]
    public class BasedForm
    {
    }

    [Module("SomeModule")]
    public class ModuleRow
    {
    }
}

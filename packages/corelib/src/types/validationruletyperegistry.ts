import { isAssignableFrom, ValidationRule } from "../base";
import { BaseTypeRegistry } from "./basetyperegistry";

class ValidationRuleTypeRegistryImpl extends BaseTypeRegistry<typeof ValidationRule> {
    constructor() {
        super({
            loadKind: "validationRule",
            defaultSuffix: "ValidationRule"
        });
    }

    protected override isMatchingType(type: any): boolean {
        return typeof type === "function" && type !== ValidationRule && isAssignableFrom(ValidationRule, type);
    }

    protected override loadError(key: string) {
        throw new Error(`The validation rule class "${key}" was not found.

Ensure that the rule class derives from ValidationRule and includes a line similar to the following:
static override [Symbol.typeInfo] = this.registerRule("MyProject.MyModule.");

Make it implement LazyLoadInclude, so it is loaded on demand by the forms that use it, and ensure that the layout head
includes @Html.LazyTypes(), or side-effect import it from the page that uses the form.`);
    }
}

/**
 * The registry of the validation rule classes, see {@link ValidationRule}, which finds them by their type keys, loading the
 * ones implementing LazyLoadInclude on demand.
 */
export const ValidationRuleTypeRegistry = new ValidationRuleTypeRegistryImpl();

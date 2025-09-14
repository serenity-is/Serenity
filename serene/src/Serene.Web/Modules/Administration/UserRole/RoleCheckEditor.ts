import { CheckTreeEditor, CheckTreeItem, GridUtils, isEmptyOrNull, stripDiacritics } from "@serenity-is/corelib";
import { RoleRow } from "../../ServerTypes/Administration/RoleRow";
import { nsAdministration } from "../../ServerTypes/Namespaces";

export class RoleCheckEditor extends CheckTreeEditor<CheckTreeItem<any>, any> {
    static [Symbol.typeInfo] = this.registerEditor(nsAdministration);

    private searchText: string;

    constructor(props: any) {
        super(props);
    }

    protected createToolbarExtensions() {
        super.createToolbarExtensions();

        GridUtils.addQuickSearchInputCustom(this.toolbar.element, (field, text) => {
            this.searchText = stripDiacritics(text || '').toUpperCase();
            this.view.setItems(this.view.getItems(), true);
        });
    }

    protected getButtons() {
        return [];
    }

    protected getTreeItems() {
        return (RoleRow as any).getLookup().items.map(role => <CheckTreeItem<any>>{
            id: role.RoleId.toString(),
            text: role.RoleName
        });
    }

    protected onViewFilter(item) {
        return super.onViewFilter(item) &&
            (isEmptyOrNull(this.searchText) ||
                stripDiacritics(item.text || '')
                    .toUpperCase().indexOf(this.searchText) >= 0);
    }
}

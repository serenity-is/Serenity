namespace Serenity {

    @Decorators.registerClass('PropertyGrid')
    export class PropertyGrid extends Widget<PropertyGridOptions> {

        private items: PropertyItem[];
        private editors: UI.EditorRefs;
        private propertyGrid: UI.IntraPropertyGrid;

        constructor(div: JQuery, opt: PropertyGridOptions) {
            super(div, opt);

            if (opt.mode == null)
                opt.mode = 1;

            div.addClass('s-PropertyGrid');
            this.items = this.options.items || [];

            this.renderIntraGrid();
        }

        private renderIntraGrid() {
            if (!this.element || !this.element.length)
                return;

            var props = Q.extend({}, this.options);
            if (this.editors == null)
                this.editors = new UI.EditorRefs(this.props.setRef);
            props.setRef = this.editors.setRef;
            this.propertyGrid = ReactDOM.render(React.createElement(UI.IntraPropertyGrid, props), this.element[0]);
        }

        destroy() {

            if (this.editors != null && this.element != null && this.element.length) {
                this.editors = null;
                ReactDOM.unmountComponentAtNode(this.element[0]);
            }

            Serenity.Widget.prototype.destroy.call(this);
        }

        get_mode(): PropertyGridMode {
            return this.options.mode;
        }

        set_mode(value: PropertyGridMode) {
            if(this.options.mode !== value) {
                this.options.mode = value;
                this.renderIntraGrid();
            }
        }

        // Obsolete
        static loadEditorValue(editor: Serenity.Widget<any>,
            item: PropertyItem, source: any): void {
        }

        // Obsolete
        static saveEditorValue(editor: Serenity.Widget<any>,
            item: PropertyItem, target: any): void {

            EditorUtils.saveValue(editor, item, target);
        }

        // Obsolete
        private static setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void {
            EditorUtils.setReadOnly(widget, isReadOnly);
        }

        // Obsolete
        private static setReadonly(elements: JQuery, isReadOnly: boolean): JQuery {
            return EditorUtils.setReadonly(elements, isReadOnly);
        }

        // Obsolete
        private static setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void {
            EditorUtils.setRequired(widget, isRequired);
        }

        load(source: any): void {
            this.propertyGrid.loadFrom(source, this.editors);
        }

        save(target: any): void {
            this.propertyGrid.saveTo(target, this.editors);
        }

        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void {
            //for (var i = 0; i < this.editors.length; i++) {
            //    var item = this.items[i];
            //    var editor = this.editors[i];
            //    callback(item, editor);
            //}
        }
    }

    export declare const enum PropertyGridMode {
        insert = 1,
        update = 2
    }


}
namespace Serenity {

    @Serenity.Decorators.registerEditor([IBooleanValue])
    @Serenity.Decorators.element('<input type="checkbox"/>')
    export class BooleanEditor extends Widget<any> {

        constructor(input: JQuery) {
            super(input);

            input.removeClass("flexify");
        }

        public get value(): boolean {
            return this.element.is(":checked");
        }

        protected get_value(): boolean {
            return this.value;
        }

        public set value(value: boolean) {
            this.element.prop("checked", !!value);
        }

        protected set_value(value: boolean): void {
            this.value = value;
        }
    }
}
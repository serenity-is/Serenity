namespace Serenity.UI {

    export class Dialog<TOptions = object> extends Serenity.TemplatedDialog<TOptions> {
        protected view: any;
        protected entityId: any;
        protected entity: any;

        constructor(options?: TOptions) {
            super(options);

            this.mountView();
        }

        public loadByIdAndOpenDialog(entityId: any, asPanel?: boolean) {
            this.entityId = entityId;
            this.dialogOpen(asPanel);
            this.mountView();
        }

        public loadEntityAndOpenDialog(entity: any, asPanel?: boolean): void {
            this.entityId = null;
            this.entity = entity;
            this.dialogOpen(asPanel);
            this.mountView();
        }

        public loadNewAndOpenDialog(asPanel?: boolean) {
            this.entityId = null;
            this.entity = {};
            this.dialogOpen(asPanel);
            this.mountView();
        }

        private mountView() {
            if (!this.element || !this.element.length)
                return;

            var props = Q.extend({}, this.options);
            this.view = ReactDOM.render(this.render() as any, this.element[0]);
        }

        getTemplate() {
            return '<div class="s-DialogContent"></div>';
        }
    }
}
namespace Serenity {

    @Serenity.Decorators.registerClass()
    export class TemplatedWidget<TOptions> extends Widget<TOptions> {

        protected idPrefix: string;
        private static templateNames: Q.Dictionary<string> = {};

        constructor(container: JQuery, options?: TOptions) {
            super(container, options);

            this.idPrefix = this.uniqueName + '_';

            var widgetMarkup = this.getTemplate().replace(new RegExp('~_', 'g'), this.idPrefix);

            // for compability with older templates based on JsRender
            var end = 0;
            while (true) {
                var idx = widgetMarkup.indexOf('{{text:"', end);
                if (idx < 0)
                    break;
                var end = widgetMarkup.indexOf('"}}', idx);
                if (end < 0)
                    break;
                var key = widgetMarkup.substr(idx + 8, end - idx - 8);
                var text = Q.text(key);
                widgetMarkup = widgetMarkup.substr(0, idx) + text + widgetMarkup.substr(end + 3);
                end = idx + text.length;
            }

            this.element.html(widgetMarkup);
        }

        protected byId(id: string): JQuery {
            return $('#' + this.idPrefix + id);
        }

        private byID<TWidget>(id: string, type: { new (...args: any[]): TWidget }) {
            return Serenity.WX.getWidget(this.byId(id), type);
        }

        private static noGeneric(s: string): string {
            var dollar = s.indexOf('$');
            if (dollar >= 0) {
                return s.substr(0, dollar);
            }
            return s;
        }

        private getDefaultTemplateName(): string {
            return TemplatedWidget.noGeneric((ss as any).getTypeName(
                (ss as any).getInstanceType(this)));
        }

        protected getTemplateName(): string {

            var type = (ss as any).getInstanceType(this);
            var fullName = (ss as any).getTypeFullName(type);

            var templateNames = TemplatedWidget.templateNames;

            var cachedName = TemplatedWidget.templateNames[fullName];
            if (cachedName != null) {
                return cachedName;
            }
            
            while (type && type !== Serenity.Widget) {
                var name = TemplatedWidget.noGeneric((ss as any).getTypeFullName(type));

                for (let k of Q.Config.rootNamespaces) {
                    if (Q.startsWith(name, k + '.')) {
                        name = name.substr(k.length + 1);
                        break;
                    }
                }

                if (Q.canLoadScriptData('Template.' + name)) {
                    templateNames[fullName] = name;
                    return name;
                }

                name = Q.replaceAll(name, '.', '_');
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    templateNames[fullName] = name;
                    return name;
                }

                name = TemplatedWidget.noGeneric((ss as any).getTypeName(type));
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    TemplatedWidget.templateNames[fullName] = name;
                    return name;
                }

                type = (ss as any).getBaseType(type);
            }

            templateNames[fullName] = cachedName = this.getDefaultTemplateName();
            return cachedName;
        }

        protected getFallbackTemplate(): string {
            return null;
        }

        protected getTemplate(): string {
            var templateName = this.getTemplateName();

            var script = $('script#Template_' + templateName);
            if (script.length > 0) {
                return script.html();
            }

            let template: string;
            if (!Q.canLoadScriptData('Template.' + templateName) &&
                this.getDefaultTemplateName() == templateName) {
                template = this.getFallbackTemplate();
                if (template != null)
                    return template;
            }
                
            template = Q.getTemplate(templateName);

            if (template == null) {
                throw new Error(Q.format(
                    "Can't locate template for widget '{0}' with name '{1}'!",
                    (ss as any).getTypeName((ss as any).getInstanceType(this)), templateName));
            }

            return template;
        }
    }
}
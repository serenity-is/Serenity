namespace Serenity {

    @Serenity.Decorators.registerClass()
    export class TemplatedWidget<TOptions> extends Widget<TOptions> {

        protected idPrefix: string;
        private static templateNames: Q.Dictionary<string> = {};

        constructor(container: JQuery, options?: TOptions) {
            super(container, options);

            this.idPrefix = this.uniqueName + '_';

            var widgetMarkup = this.getTemplate().replace(new RegExp('~_', 'g'), this.idPrefix);
            widgetMarkup = Q.jsRender(widgetMarkup);
            this.element.html(widgetMarkup);
        }

        protected byId(id: string): JQuery {
            return $('#' + this.idPrefix + id);
        }

        private byID<TWidget>(id: string, type: { new (...args: any[]): TWidget }) {
            return Serenity.WX.getWidget(this.byId(id), type);
        }

        protected getTemplateName(): string {

            var noGeneric = function (s: string) {
                var dollar = s.indexOf('$');
                if (dollar >= 0) {
                    return s.substr(0, dollar);
                }
                return s;
            };

            var type = (ss as any).getInstanceType(this);
            var fullName = (ss as any).getTypeFullName(type);

            var templateNames = TemplatedWidget.templateNames;

            var cachedName = TemplatedWidget.templateNames[fullName];
            if (cachedName != null) {
                return cachedName;
            }
            
            while (type && type !== Serenity.Widget) {
                var name = noGeneric((ss as any).getTypeFullName(type));

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

                name = noGeneric((ss as any).getTypeName(type));
                if (Q.canLoadScriptData('Template.' + name) ||
                    $('script#Template_' + name).length > 0) {
                    TemplatedWidget.templateNames[fullName] = name;
                    return name;
                }

                type = (ss as any).getBaseType(type);
            }

            templateNames[fullName] = cachedName = noGeneric((ss as any).getTypeName(
                (ss as any).getInstanceType(this)));

            return cachedName;
        }

        protected getTemplate(): string {
            var templateName = this.getTemplateName();

            var script = $('script#Template_' + templateName);
            if (script.length > 0) {
                return script.html();
            }

            let template = Q.getTemplate(templateName);

            if (template == null) {
                throw new Error(Q.format(
                    "Can't locate template for widget '{0}' with name '{1}'!",
                    (ss as any).getTypeName((ss as any).getInstanceType(this)), templateName));
            }

            return template;
        }
    }
}
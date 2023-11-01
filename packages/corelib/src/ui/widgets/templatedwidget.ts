import { Decorators } from "../../decorators";
import { canLoadScriptData, Config, format, getBaseType, getInstanceType, getTemplate, getTypeFullName, getTypeShortName, replaceAll, startsWith, localText } from "../../q";
import { Widget } from "./widget";

@Decorators.registerClass("Serenity.TemplatedWidget")
export class TemplatedWidget<TOptions> extends Widget<TOptions> {

    private static templateNames: { [key: string]: string } = {};

    protected byId(id: string): JQuery {
        return $('#' + this.idPrefix + id, this.element);
    }

    private byID<TWidget>(id: string, type: { new(...args: any[]): TWidget }) {
        return this.byId(id).getWidget(type);
    }

    private static noGeneric(s: string): string {
        var dollar = s.indexOf('$');
        if (dollar >= 0) {
            return s.substr(0, dollar);
        }
        return s;
    }

    private getDefaultTemplateName(): string {
        return TemplatedWidget.noGeneric(getTypeShortName(
            getInstanceType(this)));
    }

    protected getTemplateName(): string {

        var type = getInstanceType(this);
        var fullName = getTypeFullName(type);

        var templateNames = TemplatedWidget.templateNames;

        var cachedName = TemplatedWidget.templateNames[fullName];
        if (cachedName != null) {
            return cachedName;
        }

        while (type && type !== Widget) {
            var name = TemplatedWidget.noGeneric(getTypeFullName(type));

            for (let k of Config.rootNamespaces) {
                if (startsWith(name, k + '.')) {
                    name = name.substr(k.length + 1);
                    break;
                }
            }

            if (canLoadScriptData('Template.' + name)) {
                templateNames[fullName] = name;
                return name;
            }

            name = replaceAll(name, '.', '_');
            if (canLoadScriptData('Template.' + name) ||
                $('script#Template_' + name).length > 0) {
                templateNames[fullName] = name;
                return name;
            }

            name = TemplatedWidget.noGeneric(getTypeShortName(type));
            if (canLoadScriptData('Template.' + name) ||
                $('script#Template_' + name).length > 0) {
                TemplatedWidget.templateNames[fullName] = name;
                return name;
            }

            type = getBaseType(type);
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
        if (!canLoadScriptData('Template.' + templateName) &&
            this.getDefaultTemplateName() == templateName) {
            template = this.getFallbackTemplate();
            if (template != null)
                return template;
        }

        template = getTemplate(templateName);

        if (template == null) {
            throw new Error(format(
                "Can't locate template for widget '{0}' with name '{1}'!",
                getTypeFullName(getInstanceType(this)), templateName));
        }

        return template;
    }

    protected renderContents(): void {
        var widgetMarkup = this.getTemplate().replace(new RegExp('~_', 'g'), this.idPrefix);

        // for compatibility with older templates based on JsRender
        var end = 0;
        while (true) {
            var idx = widgetMarkup.indexOf('{{text:"', end);
            if (idx < 0)
                break;
            var end = widgetMarkup.indexOf('"}}', idx);
            if (end < 0)
                break;
            var key = widgetMarkup.substr(idx + 8, end - idx - 8);
            var txt = localText(key);
            widgetMarkup = widgetMarkup.substr(0, idx) + txt + widgetMarkup.substr(end + 3);
            end = idx + txt.length;
        }

        this.element.html(widgetMarkup);
    }

    protected useIdPrefix(): IdPrefixType {
        return useIdPrefix(this.idPrefix);
    }
}

type IdPrefixType = { [key: string]: string, Form: string, Tabs: string, Toolbar: string, PropertyGrid: string };

export function useIdPrefix(prefix: string): IdPrefixType {
    return new Proxy({ _: prefix ?? '' }, idPrefixHandler);
}

const idPrefixHandler = {
    get(target: any, p: string) {
        if (p.startsWith('#'))
            return '#' + target._ + p.substring(1);

        return target._ + p;
    }
};
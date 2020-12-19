import { Widget } from "./Widget";
import { registerClass } from "../../Decorators";
import { text } from "../../Q/LocalText";
import { startsWith, replaceAll } from "../../Q/Strings";
import { format } from "../../Q/Formatting";
import { getBaseType, getInstanceType, getTypeName, getTypeFullName } from "../../Q/TypeSystem";
import { Config } from "../../Q/Config";
import { canLoadScriptData, getTemplate } from "../../Q/ScriptData";

@registerClass("Serenity.TemplatedWidget")
export class TemplatedWidget<TOptions> extends Widget<TOptions> {

    protected idPrefix: string;
    private static templateNames: { [key: string]: string } = {};

    constructor(container: JQuery, options?: TOptions) {
        super(container, options);

        this.idPrefix = this.uniqueName + '_';

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
            var txt = text(key);
            widgetMarkup = widgetMarkup.substr(0, idx) + txt + widgetMarkup.substr(end + 3);
            end = idx + txt.length;
        }

        this.element.html(widgetMarkup);
    }

    protected byId(id: string): JQuery {
        return $('#' + this.idPrefix + id);
    }

    private byID<TWidget>(id: string, type: { new (...args: any[]): TWidget }) {
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
        return TemplatedWidget.noGeneric(getTypeName(
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

            name = TemplatedWidget.noGeneric(getTypeName(type));
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
                getTypeName(getInstanceType(this)), templateName));
        }

        return template;
    }
}
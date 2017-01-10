namespace Q {

    export function addOption(select: JQuery, key: string, text: string) {
        $('<option/>').val(key).text(text).appendTo(select);
    }

    export function addEmptyOption(select: JQuery) {
        addOption(select, '', text("Controls.SelectEditor.EmptyItemText"));
    }

    export function clearOptions(select: JQuery) {
        select.html('');
    }

    export function findElementWithRelativeId(element: JQuery, relativeId: string) {
        let elementId = element.attr('id');
        if (isEmptyOrNull(elementId)) {
            return $('#' + relativeId);
        }

        let result = $(elementId + relativeId);
        if (result.length > 0) {
            return result;
        }

        result = $(elementId + '_' + relativeId);

        if (result.length > 0) {
            return result;
        }

        while (true) {
            let idx = elementId.lastIndexOf('_');
            if (idx <= 0) {
                return $('#' + relativeId);
            }

            elementId = elementId.substr(0, idx);
            result = $('#' + elementId + '_' + relativeId);

            if (result.length > 0) {
                return result;
            }
        }
    }

    function htmlEncoder(a: string): string {
        switch (a) {
            case '&': return '&amp;';
            case '>': return '&gt;';
            case '<': return '&lt;';
        }
        return a;
    }

    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    export function htmlEncode(s: any): string {
        let text = (s == null ? '' : s.toString());
        if ((new RegExp('[><&]', 'g')).test(text)) {
            return text.replace(new RegExp('[><&]', 'g'), htmlEncoder);
        }
        return text;
    }

    export function jsRender(markup: string, data?: any) {
        if (!markup || markup.indexOf('{{') < 0) {
            return markup;
        }

        if (!($ as any).templates || !($ as any).views) {
            throw new ss.Exception('Please make sure that jsrender.js is included in the page!');
        }

        data = data || {};
        var template = ($ as any).templates(markup);
        ($ as any).views.converters({
            text: Q.text
        }, template);

        return template.render(data);
    }

    export function log(m: any) {
        (<any>window).console && (<any>window).console.log(m);
    }

    export function newBodyDiv(): JQuery {
        return $('<div/>').appendTo(document.body);
    }

    export function outerHtml(element: JQuery) {
        return $('<i/>').append(element.eq(0).clone()).html();
    }
}
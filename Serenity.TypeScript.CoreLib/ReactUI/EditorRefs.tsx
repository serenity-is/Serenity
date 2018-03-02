namespace Serenity.UI {

    export class EditorRefs {

        private refFunctions: Q.Dictionary<(ref: any) => void> = Object.create(null);
        private refs: Q.Dictionary<any> = Object.create(null);

        constructor(private inner?: (name: string, ref: any) => void) {
            this.setRef = this.setRef.bind(this);
            this.ref = this.ref.bind(this);
        }

        getRef(name: string) {
            return this.refs[name];
        }

        setRef(name: string, ref: any) {
            this.refs[name] = ref;
            if (this.inner != null)
                this.inner(name, ref);
        }

        ref(name: string): ((ref: any) => void) {
            var func = this.refFunctions[name];
            if (func != null)
                return func;

            var setRef = this.setRef;
            func = (function (ref: any) {
                setRef(name, ref);
            });

            this.refFunctions[name] = func;
            return func;
        }

        loadFrom(source: any, names?: string[]) {

            names = Q.coalesce(names, Object.keys(this.refs));

            for (var name of names) {
                var editor = this.refs[name];
                if (editor == null)
                    continue;

                if (editor.isWidgetWrapper) {
                    editor = editor.widget;
                    if (editor)
                        Serenity.EditorUtils.loadValue(editor, { name: name }, source);
                }
                else if (editor.nodeType) {
                    source[name] = editor.value;
                }
                else if (editor.element) {
                    Serenity.EditorUtils.loadValue(editor, { name: name }, source);
                }
            }
        }

        saveTo(target: any, names?: string[], ignoreOneWay?: boolean) {

            names = Q.coalesce(names, Object.keys(this.refs));

            for (var name of names) {
                var editor = this.refs[name];
                if (editor.isWidgetWrapper) {
                    if (ignoreOneWay || !editor.props.oneWay) {
                        editor = editor.widget;
                        if (editor)
                            Serenity.EditorUtils.saveValue(editor, { name: name }, target);
                    }
                }
                else if (editor.nodeType) {
                    if (ignoreOneWay ||
                        !(editor as HTMLElement).dataset ||
                        !(editor as HTMLElement).dataset.oneWay)
                        target[name] = editor.value;
                }
                else if (editor.element) {
                    if (ignoreOneWay ||
                        !editor.options ||
                        !editor.options.oneWay) {
                        Serenity.EditorUtils.saveValue(editor, { name: name }, target);
                    }
                }
            }
        }
    }
}
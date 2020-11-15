namespace Serenity {
    export namespace EditorTypeRegistry {
        let knownTypes: Q.Dictionary<WidgetClass>;

        export function get(key: string): WidgetClass {

            if (Q.isEmptyOrNull(key)) {
                throw new Q.ArgumentNullException('key');
            }

            initialize();

            var editorType = knownTypes[key.toLowerCase()];
            if (editorType == null) {

                var type = Q.getType(key) ?? Q.getType(key, globalObj);
                if (type != null) {
                    knownTypes[key.toLowerCase()] = type as any;
                    return type as any;
                }

                throw new Q.Exception(Q.format("Can't find {0} editor type!", key));
            }
            return editorType;

        }

        function initialize(): void {

            if (knownTypes != null)
                return;

            knownTypes = {};

            for (var type of Q.getTypes()) {

                var fullName = Q.getTypeFullName(type).toLowerCase();
                knownTypes[fullName] = type;

                var editorAttr = Q.getAttributes(type, Serenity.EditorAttribute, false);
                if (editorAttr != null && editorAttr.length > 0) {
                    var attrKey = editorAttr[0].key;
                    if (!Q.isEmptyOrNull(attrKey)) {
                        knownTypes[attrKey.toLowerCase()] = type;
                    }
                }

                for (var k of Q.Config.rootNamespaces) {
                    if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                        var kx = fullName.substr(k.length + 1).toLowerCase();
                        if (knownTypes[kx] == null) {
                            knownTypes[kx] = type;
                        }
                    }
                }
            }

            setTypeKeysWithoutEditorSuffix();
        }

        export function reset(): void {
            knownTypes = null;
        }

        function setTypeKeysWithoutEditorSuffix() {
            var suffix = 'editor';
            var keys = Object.keys(knownTypes);
            for (var k of keys) {
                setWithoutSuffix(k, knownTypes[k]);
            }
        }

        function setWithoutSuffix(key: string, t: WidgetClass) {
            var suffix = 'editor';

            if (!Q.endsWith(key, suffix))
                return;

            var p = key.substr(0, key.length - suffix.length);

            if (Q.isEmptyOrNull(p))
                return;

            if (knownTypes[p] != null)
                return;

            knownTypes[p] = knownTypes[key];
        }
    }
}
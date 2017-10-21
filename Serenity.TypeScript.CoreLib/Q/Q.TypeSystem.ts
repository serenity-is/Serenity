
namespace Q {

    export function prop(type: any, name: string, getter?: string, setter?: string) {
        getter = getter || "get_" + name;
        setter = setter || "set_" + name;

        Object.defineProperty(type.prototype, name, {
            get: function () {
                return this[getter]();
            },
            set: function (value) {
                return this[setter](value);
            },
            configurable: true,
            enumerable: true
        });
    }

    function enumerateTypes(global: any, namespaces: string[], callback: (type: any, fullName: string) => void): void {
        function scan(root: any, fullName: string, depth: number) {
            if (!root)
                return;

            if ($.isArray(root) ||
                root instanceof Date)
                return;

            var t = typeof (root);

            if (t == "string" ||
                t == "number")
                return;

            if ($.isFunction(root) || (root.__enum && root.__register))
                callback(root, fullName);

            if (depth > 3)
                return;

            for (var k of Object.keys(root)) {
                if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                    continue;

                if (k.indexOf('$') >= 0)
                    continue;

                if (k == "prototype")
                    continue;

                scan(root[k], fullName + '.' + k, depth + 1);
            }
        }

        for (var nsRoot of namespaces) {
            if (nsRoot == null || !nsRoot.length) {
                continue;
            }

            if (nsRoot.indexOf('.') >= 0) {
                let g = global;
                let parts = nsRoot.split('.');
                for (var p of parts) {
                    if (!p.length)
                        continue;

                    g = g[p];
                    if (!g)
                        continue;
                }
                scan(g, nsRoot, 0);
            }

            scan(global[nsRoot], nsRoot, 0);
        }
    }

    //all browsers seem to show some unhandled exception message so don't enable this for now
    //window.addEventListener('unhandledrejection', function (e: any) {
    //    var error = e.reason || e;
    //    log(e);
    //    log((error.get_stack && error.get_stack()) || error.stack);
    //});

    //window.addEventListener('error', function (e: any) {
    //    var error = (e.error | e) as any;
    //    log(e);
    //    log((error.get_stack && error.get_stack()) || error.stack);
    //});

    (function (global: any) {

        // fake assembly for typescript apps
        (ss as any).initAssembly({}, 'App', {});

        // for backward compability, avoid!
        global.Q$Externals = Q;
        global.Q$Config = Q.Config;
        global.Q$Culture = Q.Culture;
        global.Q$Lookup = Q.Lookup;
        global.Q$ScriptData = Q.ScriptData;
        global.Q$LT = Q.LT;

        function initializeTypes() {
            enumerateTypes(global, Q.Config.rootNamespaces, function (obj, fullName) {
                // probably Saltaralle class
                if (obj.hasOwnProperty("__typeName") &&
                    !obj.__register)
                    return;

                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    fullName.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter]
                }

                if (!obj.__class) {
                    var baseType = (ss as any).getBaseType(obj);
                    if (baseType.__class)
                        obj.__class = true;
                }

                if (obj.__class || obj.__enum) {
                    obj.__typeName = fullName;
                    if (!obj.__assembly) {
                        obj.__assembly = ss.__assemblies['App'];
                    }
                    obj.__assembly.__types[fullName] = obj;
                }

                delete obj.__register;
            });
        }

        $(function () {
            initializeTypes();

            setMobileDeviceMode();
            $(global).bind('resize', function () {
                setMobileDeviceMode();
            });
        });
    })(window || {});
}
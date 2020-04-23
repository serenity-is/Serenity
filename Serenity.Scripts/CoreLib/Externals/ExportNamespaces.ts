if (globalObj != null) {

	function copyToGlobal(src: any, target: any) {
		for (var n in src) {
			if (src.hasOwnProperty(n))
				target[n] = Q[n];
		}
	}

	globalObj.ss ? copyToGlobal(Q, globalObj.ss) : globalObj.ss = Q;
	globalObj.Q ? copyToGlobal(Q, globalObj.Q) : globalObj.Q = Q;
	globalObj.Serenity ? copyToGlobal(Serenity, globalObj.Serenity) : globalObj.Serenity = Serenity;
}

namespace Q {
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

    function initializeTypes() {
        enumerateTypes(globalObj, Q.Config.rootNamespaces, function (obj, fullName) {
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
                var baseType = Q.getBaseType(obj);
                if (baseType && baseType.__class)
                    obj.__class = true;
            }

            if (obj.__class || obj.__enum || obj.__interface) {
                obj.__typeName = fullName;
                Q.types[fullName] = obj;
            }

            delete obj.__register;
        });
    }

    $(function () {
        initializeTypes();

        setMobileDeviceMode();
        globalObj && $(globalObj).bind('resize', function () {
            setMobileDeviceMode();
        });
    });
}
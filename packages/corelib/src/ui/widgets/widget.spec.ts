import { Widget } from "./widget";
import { Config } from "../../q/config";

test('Serenity Widget must return class name without root namespace', function() {
    var oldNamespaces = Config.rootNamespaces;
    Config.rootNamespaces = [];
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            __typeName : "Serenity.Demo.Northwind.CustomerGrid" 
        }
    });
    Config.rootNamespaces = oldNamespaces;
    expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Serenity Widget must work with serenity root namespaces', function() {
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            __typeName : "Serenity.Demo.Northwind.CustomerGrid" 
        }
    });

    expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Project Widget must work with serenity root namespaces', function() {
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            __typeName : "StartSharp.Demo.Northwind.CustomerGrid" 
        }
    });

    expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Project Widget must work with project root namespaces', function() {
    var oldNamespaces = Config.rootNamespaces;
    Config.rootNamespaces.push("StartSharp");
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            __typeName : "StartSharp.Demo.Northwind.CustomerGrid" 
        }
    });
    Config.rootNamespaces = oldNamespaces;
    expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-Demo-Northwind-CustomerGrid s-CustomerGrid");
});


test('Project Widget must work without root namespaces', function() {
    var oldNamespaces = Config.rootNamespaces;
    Config.rootNamespaces = [];
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            __typeName : "StartSharp.Demo.Northwind.CustomerGrid" 
        }
    });
    Config.rootNamespaces = oldNamespaces;
    expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-CustomerGrid");
});
import { Config, classTypeInfo } from "../../base";
import { Widget, useIdPrefix } from "./widget";

test('Serenity Widget must return class name without root namespace', function() {
    var oldNamespaces = Config.rootNamespaces;
    Config.rootNamespaces = [];
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            typeInfo : classTypeInfo("Serenity.Demo.Northwind.CustomerGrid") 
        }
    });
    Config.rootNamespaces = oldNamespaces;
    expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Serenity Widget must work with serenity root namespaces', function() {
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            typeInfo : classTypeInfo("Serenity.Demo.Northwind.CustomerGrid") 
        }
    });

    expect(generatedClassNames).toBe("s-Serenity-Demo-Northwind-CustomerGrid s-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Project Widget must work with serenity root namespaces', function() {
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            typeInfo : classTypeInfo("StartSharp.Demo.Northwind.CustomerGrid")
        }
    });

    expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

test('Project Widget must work with project root namespaces', function() {
    var oldNamespaces = Config.rootNamespaces;
    Config.rootNamespaces.push("StartSharp");
    var generatedClassNames = Widget.prototype["getCssClass"].call({ 
        constructor : { 
            typeInfo : classTypeInfo("StartSharp.Demo.Northwind.CustomerGrid") 
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
            typeInfo : classTypeInfo("StartSharp.Demo.Northwind.CustomerGrid") 
        }
    });
    Config.rootNamespaces = oldNamespaces;
    expect(generatedClassNames).toBe("s-StartSharp-Demo-Northwind-CustomerGrid s-CustomerGrid");
});

describe('useIdPrefix', () => {

    it('uses passed prefix', () => {
        var id = useIdPrefix('my_');

        expect(id._).toBe('my__');
        expect(id._x).toBe('my__x');
        expect(id.Form).toBe('my_Form');
        expect(id.PropertyGrid).toBe('my_PropertyGrid');
        expect(id.something).toBe('my_something');
    });

    it('handles hashes differently for in-page href generation', () => {
        var id = useIdPrefix('my_');

        expect(id["#_"]).toBe('#my__');
        expect(id["#_x"]).toBe('#my__x');
        expect(id["#Form"]).toBe('#my_Form');
        expect(id["#PropertyGrid"]).toBe('#my_PropertyGrid');
        expect(id["#something"]).toBe('#my_something');
    });

});
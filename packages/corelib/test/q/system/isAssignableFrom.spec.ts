import { Decorators } from "@/decorators";
import { isAssignableFrom } from "@/q/system";

test("interfaces can also be matched by their registration names", function() {
    expect(isAssignableFrom(Module1.ISome, Module1Class)).toBe(true);
    expect(isAssignableFrom(Module1.ISome, CopyModule1Class)).toBe(true);
    expect(isAssignableFrom(CopyModule1.ISome, Module1Class)).toBe(true);
    expect(isAssignableFrom(CopyModule1.ISome, CopyModule1Class)).toBe(true);
});

test("interfaces are matched by their registration names even when class name is different", function() {
    @Decorators.registerInterface("ISome")
    class ISomeMeSome {
    }
    expect(isAssignableFrom(ISomeMeSome, Module1Class)).toBe(true);
    expect(isAssignableFrom(ISomeMeSome, CopyModule1Class)).toBe(true);
});

test("interfaces with different class names and registration names won't match", function() {
    
    @Decorators.registerInterface("IOther")
    class IOther {
    }

    expect(isAssignableFrom(IOther, Module1Class)).toBe(false);
    expect(isAssignableFrom(IOther, CopyModule1Class)).toBe(false);
});

test("interfaces with same class names but different registration names won't match", function() {

    @Decorators.registerInterface("ISomeDiff")
    class ISome {
    }

    @Decorators.registerClass("X", [ISome])
    class X {
    }

    expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
});

test("classes that are not registered as interfaces won't match", function() {

    @Decorators.registerClass("ISome")
    class ISome {
    }

    @Decorators.registerClass("X", [ISome])
    class X {
    }

    expect(isAssignableFrom(Module1.ISome, X)).toBe(false);
});

namespace Module1 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

namespace CopyModule1 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

@Decorators.registerClass("SomeClassUsingCopy1", [Module1.ISome])
class Module1Class {
}

@Decorators.registerClass("SomeClassUsingCopy2", [CopyModule1.ISome])
class CopyModule1Class {
}
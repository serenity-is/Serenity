import { Decorators } from "@/decorators";
import { isAssignableFrom } from "@/q/system";

test("interfaces can also be matched by their registration names", function() {
    expect(isAssignableFrom(Module1Copy1.ISome, SomeClassUsingCopy1)).toBe(true);
    expect(isAssignableFrom(Module1Copy1.ISome, SomeClassUsingCopy2)).toBe(true);
    expect(isAssignableFrom(Module1Copy2.ISome, SomeClassUsingCopy1)).toBe(true);
    expect(isAssignableFrom(Module1Copy2.ISome, SomeClassUsingCopy2)).toBe(true);
});

test("interfaces are matched by their registration names even when class name is different", function() {
    @Decorators.registerInterface("ISome")
    class ISomeMeSome {
    }
    expect(isAssignableFrom(ISomeMeSome, SomeClassUsingCopy1)).toBe(true);
    expect(isAssignableFrom(ISomeMeSome, SomeClassUsingCopy2)).toBe(true);
});

test("interfaces with different class names and registration names won't match", function() {
    
    @Decorators.registerInterface("IOther")
    class IOther {
    }

    expect(isAssignableFrom(IOther, SomeClassUsingCopy1)).toBe(false);
    expect(isAssignableFrom(IOther, SomeClassUsingCopy2)).toBe(false);
});

test("interfaces with same class names but different registration names won't match", function() {

    @Decorators.registerInterface("ISomeDiff")
    class ISome {
    }

    @Decorators.registerClass("X", [ISome])
    class X {
    }

    expect(isAssignableFrom(Module1Copy1.ISome, X)).toBe(false);
});

test("classes that are not registered as interfaces won't match", function() {

    @Decorators.registerClass("ISome")
    class ISome {
    }

    @Decorators.registerClass("X", [ISome])
    class X {
    }

    expect(isAssignableFrom(Module1Copy1.ISome, X)).toBe(false);
});

namespace Module1Copy1 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

namespace Module1Copy2 {
    @Decorators.registerInterface("ISome")
    export class ISome {
    }
}

@Decorators.registerClass("SomeClassUsingCopy1", [Module1Copy1.ISome])
class SomeClassUsingCopy1 {
}

@Decorators.registerClass("SomeClassUsingCopy2", [Module1Copy2.ISome])
class SomeClassUsingCopy2 {
}
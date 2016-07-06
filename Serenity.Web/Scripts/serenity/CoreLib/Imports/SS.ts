// this class will go obsolete once all code is ported to TypeScript
// prefer alternative methods under Q
declare namespace ss {
    interface AssemblyReg {
        name: string;
        __types: ClassReg[];
    }

    interface ClassReg {
        __register: boolean;
        __class: boolean;
        __assembly: AssemblyReg;
        __interfaces: any[];
    }

    let __assemblies: { [name: string]: AssemblyReg; };

    class Exception {
        constructor(msg: string);
    }

    class NotSupportedException extends Exception {
        constructor(msg: string);
    }
}

declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        constructor(displayName: string);
        displayName: string;
    }
}
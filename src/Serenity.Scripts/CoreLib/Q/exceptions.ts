export class Exception extends Error {
    constructor(message: string) {
        super(message);
        this.name = "Exception";
    }
}

export class NullReferenceException extends Exception {
    constructor(message?: string) {
        super(message || 'Object is null.');
        this.name = "NullReferenceException";
    }
}

export class ArgumentNullException extends Exception {
    constructor(paramName: string, message?: string) {
        super((message || 'Value cannot be null.') + '\nParameter name: ' + paramName);
        this.name = "ArgumentNullException";
    }
}

export class ArgumentOutOfRangeException extends Exception {
    constructor(paramName: string, message?: string) {
        super((message ?? 'Value is out of range.') +
            (paramName ? ('\nParameter name: ' + paramName) : ""));
        this.name = "ArgumentNullException";
    }
}

export class InvalidCastException extends Exception {
    constructor(message: string) {
        super(message);
        this.name = "InvalidCastException";
    }
}
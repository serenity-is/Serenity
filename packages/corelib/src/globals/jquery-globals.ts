declare global {
    interface JQueryStatic {
        extend<T>(target: T, object1?: T, ...objectN: T[]): T;
        toJSON(obj: any): string;
    }
}

export { }
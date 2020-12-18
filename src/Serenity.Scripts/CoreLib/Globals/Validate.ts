export {}

declare global {
    namespace JQueryValidation {
        interface ValidationOptions {
            normalizer?: (v: string) => string;
        }
    }
}
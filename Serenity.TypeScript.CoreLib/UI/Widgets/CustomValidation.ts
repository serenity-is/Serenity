declare namespace Serenity {

    namespace CustomValidation {
        function registerValidationMethods(): void;
    }

    type CustomValidationRule = (element: JQuery) => string;

}
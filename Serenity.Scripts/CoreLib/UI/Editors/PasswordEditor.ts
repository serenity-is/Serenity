namespace Serenity {
    
    @Decorators.registerEditor('Serenity.PasswordEditor')
    export class PasswordEditor extends StringEditor {
        constructor(input: JQuery) {
            super(input);

            input.attr('type', 'password');
        }
    }

}
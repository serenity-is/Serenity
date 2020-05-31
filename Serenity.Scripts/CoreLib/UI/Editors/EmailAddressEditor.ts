namespace Serenity {
    
    @Decorators.registerEditor('Serenity.EmailAddressEditor')
    export class EmailAddressEditor extends Serenity.StringEditor {
        constructor(input: JQuery) {
            super(input);

            input.attr('type', 'email')
                .addClass('email');
        }
    }
}
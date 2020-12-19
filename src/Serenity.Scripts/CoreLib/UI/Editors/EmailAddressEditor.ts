namespace Serenity {
    
    @registerEditor('Serenity.EmailAddressEditor')
    export class EmailAddressEditor extends StringEditor {
        constructor(input: JQuery) {
            super(input);

            input.attr('type', 'email')
                .addClass('email');
        }
    }
}
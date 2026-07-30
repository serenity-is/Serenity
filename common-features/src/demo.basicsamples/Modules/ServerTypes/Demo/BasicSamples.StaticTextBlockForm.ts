import { initFormType, PrefixedContext, StringEditor } from "@serenity-is/corelib";
import { StaticTextBlock } from "@serenity-is/extensions";

export interface StaticTextBlockForm {
    StaticText: StaticTextBlock;
    SomeInput: StringEditor;
    HtmlList: StaticTextBlock;
    FromLocalText: StaticTextBlock;
    DisplayFieldValue: StaticTextBlock;
}

export class StaticTextBlockForm extends PrefixedContext {
    static readonly formKey = 'BasicSamples.StaticTextBlock';
    declare private static init: boolean;

    constructor(...args: ConstructorParameters<typeof PrefixedContext>) {
        super(...args);

        if (!StaticTextBlockForm.init) {
            StaticTextBlockForm.init = true;

            initFormType(StaticTextBlockForm, [
                'StaticText', StaticTextBlock,
                'SomeInput', StringEditor,
                'HtmlList', StaticTextBlock,
                'FromLocalText', StaticTextBlock,
                'DisplayFieldValue', StaticTextBlock
            ]);
        }
    }
}
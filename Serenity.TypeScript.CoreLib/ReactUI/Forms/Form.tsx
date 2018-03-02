namespace Serenity.UI {

    export class Form extends ValidateForm {
        render() {
            return (
                <div className="flex-layout">
                    <div className="s-Form">
                        {super.render()}
                    </div>
                </div>
            );
        }
    }

}
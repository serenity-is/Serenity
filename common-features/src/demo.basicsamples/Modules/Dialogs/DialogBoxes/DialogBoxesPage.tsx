import { alertDialog, confirmDialog, informationDialog, notifyError, notifyInfo, notifySuccess, successDialog, warningDialog } from "@serenity-is/corelib";

export default function pageInit() {

    const confirmAndButtonsClick = () => confirmDialog(
        // here we demonstrate how you can detect which button user has clicked
        // second parameter is Yes handler and it is called only when user clicks Yes.
        // third parameter has some additional options, that you should only use when needed            
        "Click one of buttons, or close dialog with [x] on top right, i'll tell you what you did!",
        () => notifySuccess("You clicked YES. Great!"), {
        onNo: () => notifyInfo("You clicked NO. Why?"),
        onClose: (result) => { !result && notifyError("You clicked X. Operation is cancelled. Will try again?") }
    });

    const confirmWithCustomTitleClick = () => confirmDialog("This confirmation has a custom title",
        () => notifySuccess("Allright!"), {
        title: 'Some Custom Confirmation Title'
    });

    const alertClick = () => alertDialog("Houston, we got a problem!");

    const informationClick = () => informationDialog("What a nice day", () => notifySuccess("No problem!"));

    const successClick = () => successDialog("Operation complete", () => notifySuccess("Well done!"));

    const warningClick = () => warningDialog("Hey, be careful!");

    const alertWithHtmlClick = () => alertDialog(
        `<div style="white-space: initial">
            <h4>Here is some HTML content!</h4>
            <ul>
                <li>Item 1</li>
                <li>Item 2</li>
                <li>Visit <a href="https://serenity.is/" target="_blank">https://serenity.is/</a>!</li>
            </ul>
        </div>`, {
        htmlEncode: false
    });

    document.getElementById("SampleRoot").append(<>
        <button class="btn btn-block btn-primary" onClick={confirmAndButtonsClick}>Confirm Dialog and Buttons</button>
        <button class="btn btn-block btn-primary" onClick={confirmWithCustomTitleClick}>Confirm With Custom Title</button>
        <button class="btn btn-block btn-info" onClick={informationClick}>Information</button>
        <button class="btn btn-block btn-success" onClick={successClick}>Success</button>
        <button class="btn btn-block btn-warning" onClick={warningClick}>Warning</button>
        <button class="btn btn-block btn-danger" onClick={alertClick}>Alert</button>
        <button class="btn btn-block btn-danger" onClick={alertWithHtmlClick }>Alert with HTML Content</button>
    </>)
}
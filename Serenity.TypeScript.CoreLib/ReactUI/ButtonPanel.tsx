namespace Serenity.UI {

    export class ButtonPanel extends React.Component {
        render() {
            return (
                <div className="align-right" style={{ marginTop: "10px", paddingRight: "24px" }}>
                    {this.props.children}
                </div>
            )
        }
    }

}

namespace Q {

    export namespace ErrorHandling {

        export function showServiceError(error: Serenity.ServiceError) {
            let msg: any;
            if (error == null) {
                msg = '??ERROR??';
            }
            else {
                msg = error.Message;
                if (msg == null) {
                    msg = error.Code;
                }
            }

            Q.alert(msg);
        }
    }
}
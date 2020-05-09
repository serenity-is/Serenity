namespace Q {
    export namespace Config {
        /**
         * This is the root path of your application. If your application resides under http://localhost/mysite/,
         * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
         * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
         */
        export let applicationPath = '/';

        if (typeof document !== 'undefined') {
            var pathLink = document.querySelector('link#ApplicationPath') as HTMLLinkElement;
            if (pathLink != null) {
                applicationPath = pathLink.href;
            }
        }

        /**
         * Set this to true, to enable responsive dialogs by default, without having to add Serenity.Decorators.responsive()"
         * on dialog classes manually. It's false by default for backward compability.
         */
        export let responsiveDialogs = false;

        /**
         * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default for message dialogs
         */
        export let bootstrapMessages = false;

        /**
         * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
         * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
         * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
         *
         * You should usually add your application root namespace to this list in ScriptInitialization.ts file.
         */
        export let rootNamespaces = ['Serenity'];

        /**
         * This is an optional method for handling when user is not logged in. If a users session is expired 
         * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
         * you may intercept it and notify user about this situation and ask if she wants to login again...
         */
        export let notLoggedInHandler: Function = null;
    }
}
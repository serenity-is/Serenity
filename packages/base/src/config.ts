var Config = {
    /**
     * This is the root path of your application. If your application resides under http://localhost/mysite/,
     * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
     * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
     */
    applicationPath: '/',

    /**
     * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
     */
    emailAllowOnlyAscii: true,

    /**
     * @Obsolete defaulted to false before for backward compatibility, now it is true by default
     */
    responsiveDialogs: true,

    /**
     * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default for message dialogs
     */
    bootstrapMessages: false,

    /**
     * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
     * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
     * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
     *
     * You should usually add your application root namespace to this list in ScriptInit(ialization).ts file.
     */
    rootNamespaces: ['Serenity'],

    /**
     * This is an optional method for handling when user is not logged in. If a users session is expired 
     * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
     * you may intercept it and notify user about this situation and ask if she wants to login again...
     */
    notLoggedInHandler: <Function>null
}

if (typeof document !== 'undefined') {
    var pathLink = document.querySelector('link#ApplicationPath') as HTMLLinkElement;
    if (pathLink != null) {
        Config.applicationPath = pathLink.getAttribute('href');
    }
}

export { Config };

namespace Q {
    export namespace Config {
        export let applicationPath = '/';
        export let emailAllowOnlyAscii = true;
        export let rootNamespaces = ['Serenity'];
        export let notLoggedInHandler: Function = null;

        var pathLink = $('link#ApplicationPath');
        if (pathLink.length > 0) {
            applicationPath = pathLink.attr('href');
        }
    }
}
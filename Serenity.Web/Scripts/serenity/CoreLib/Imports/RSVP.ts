declare class RSVP<TResult> {
    constructor(constructor: (p1: (p1: any) => void, p2: any) => void);
}

declare module RSVP {
    function on(handler: (e: any) => void);
    function resolve(): Thenable<any>;
}
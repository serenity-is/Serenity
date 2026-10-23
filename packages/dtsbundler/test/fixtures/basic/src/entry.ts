import { TypeA } from './utils';
import { TypeB } from './internal';
import { External } from 'ext-lib';

/** The public API of the fixture. */
export interface PublicApi {
    a: TypeA;
    b: TypeB;
    ext: External;
}

/** A widget that holds the API. */
export class Widget {
    constructor(public api: PublicApi) {
    }
}

export type SomeType = TypeA | TypeB;

export { TypeA as LocalTypeA };
export * from './internal';
import './augment';

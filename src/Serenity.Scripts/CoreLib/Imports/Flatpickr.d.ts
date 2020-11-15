import { FlatpickrFn as _flatpickrFn } from 'flatpickr/dist/types/instance';

declare global {
    export type FlatpickrFn = _flatpickrFn;
    export var flatpickr: FlatpickrFn;
}
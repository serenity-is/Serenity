/** A procedure that takes no arguments and returns void */
export type Action = () => void;
/** A function that takes no arguments and returns a value of type T */
export type Func<T> = () => T;
/** A predicate that takes an argument of type T and returns a boolean */
export type Predicate<T> = (arg: T) => boolean;
/** A procedure that takes an argument of type T and returns void */
export type Proc<T> = (arg: T) => void;

export { };

/**
 * A single line in a filter panel, describing one filter condition.
 */
export interface FilterLine {
    /** Field name being filtered. */
    field?: string;
    /** Operator key. */
    operator?: string;
    /** Whether this line is OR-combined with the previous line. */
    isOr?: boolean;
    /** Whether this line opens a parenthesis group. */
    leftParen?: boolean;
    /** Whether this line closes a parenthesis group. */
    rightParen?: boolean;
    /** Validation error message, if any. */
    validationError?: string;
    /** The criteria expression for this line. */
    criteria?: any[];
    /** Display text for this line. */
    displayText?: string;
    /** Persisted editor state. */
    state?: any;
}
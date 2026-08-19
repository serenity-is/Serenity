import { Criteria, formatDate, nsSerenity, parseISODateTime } from "../../base";
import { DateEditor } from "../editors/dateeditor";
import { DateTimeEditor } from "../editors/datetimeeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { CriteriaWithText } from "./criteriawithtext";
import { FilterOperator } from "./filteroperator";

/**
 * Filtering handler for date-time fields using a date-time editor.
 */
export class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a date-time filtering handler.
     */
    constructor() {
        super(DateTimeEditor)
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }

    /**
     * Returns the criteria for the current operator, handling day-boundary comparisons.
     * @returns The criteria with display text.
     */
    override getCriteria() {
        var result: CriteriaWithText = {};

        switch (this.get_operator().key) {
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge': {
                {
                    var text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    var date = parseISODateTime(this.getEditorValue());
                    date = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                    var next = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);
                    var criteria = Criteria(this.getCriteriaField());
                    var dateValue = formatDate(date, 'yyyy-MM-dd');
                    var nextValue = formatDate(next, 'yyyy-MM-dd');
                    switch (this.get_operator().key) {
                        case 'eq': {
                            result.criteria = Criteria.and(criteria.ge(dateValue), criteria.lt(nextValue));
                            return result;
                        }
                        case 'ne': {
                            result.criteria = Criteria.paren(Criteria.or(criteria.lt(dateValue), criteria.ge(nextValue)));
                            return result;
                        }
                        case 'lt': {
                            result.criteria = criteria.lt(dateValue);
                            return result;
                        }
                        case 'le': {
                            result.criteria = criteria.lt(nextValue);
                            return result;
                        }
                        case 'gt': {
                            result.criteria = criteria.ge(nextValue);
                            return result;
                        }
                        case 'ge': {
                            result.criteria = criteria.ge(dateValue);
                            return result;
                        }
                    }
                }
                break;
            }
        }

        return super.getCriteria();
    }
}

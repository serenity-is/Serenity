declare namespace Serenity {

    namespace FilterOperators {
        let isTrue: string;
        let isFalse: string;
        let contains: string;
        let startsWith: string;
        let EQ: string;
        let NE: string;
        let GT: string;
        let GE: string;
        let LT: string;
        let LE: string;
        let BW: string;
        let IN: string;
        let isNull: string;
        let isNotNull: string;
        let toCriteriaOperator: { [key: string]: string };
    }

}
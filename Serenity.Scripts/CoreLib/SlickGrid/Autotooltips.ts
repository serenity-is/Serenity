
declare namespace Slick {
    class AutoTooltips {
        constructor(options: Slick.AutoTooltipsOptions);
    }

    interface AutoTooltipsOptions {
        enableForHeaderCells?: boolean;
        enableForCells?: boolean;
        maxToolTipLength?: number;
    }
}
import { Widget } from "./widget";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";

if (typeof $ !== "undefined" && $.fn) {
    $.fn.flexHeightOnly = function (flexY: number = 1) {
        return this.flexWidthHeight(0, flexY);
    }

    $.fn.flexWidthOnly = function (flexX: number = 1) {
        return this.flexWidthHeight(flexX, 0);
    }

    $.fn.flexWidthHeight = function (flexX: number = 1, flexY: number = 1) {
        return this.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
    }

    $.fn.flexX = function (flexX: number): JQuery {
        return this.data('flex-x', flexX);
    }

    $.fn.flexY = function (flexY: number): JQuery {
        return this.data('flex-y', flexY);
    }
}

export class Flexify extends Widget<FlexifyOptions> {
    private xDifference = 0;
    private yDifference = 0;
    constructor(container: JQuery, options: FlexifyOptions) {
        super(container, options);

        LazyLoadHelper.executeOnceWhenShown(container, () => {
            this.storeInitialSize();
        });
    }

    storeInitialSize() {
        if (!!this.element.data('flexify-init')) {
            return;
        }

        var designWidth = this.options.designWidth;
        if (designWidth == null)
            designWidth = this.element.width();
        this.element.data('flexify-width', designWidth);

        var designHeight = this.options.designHeight;
        if (designHeight == null)
            designHeight = this.element.height();
        this.element.data('flexify-height', designHeight);

        this.element.data('flexify-init', true);

        this.element.on('resize.' + this.uniqueName, () =>
            this.resizeElements());

        this.element.on('resizestop.' + this.uniqueName, () =>
            this.resizeElements());
        
        var tabs = this.element.find('.ui-tabs');
        if (tabs.length > 0) {
            tabs.on('tabsactivate.' + this.uniqueName, () =>
                this.resizeElements());
        }

        if (this.options.designWidth != null || this.options.designHeight != null) 
            this.resizeElements();
    }

    private getXFactor(element: JQuery): number {
        var xFactor = null;
        if (this.options.getXFactor != null)
            xFactor = this.options.getXFactor(element);

        if (xFactor == null)
            xFactor = element.data('flex-x');

        return xFactor ?? 1;
    }

    private getYFactor(element: JQuery): number {
        var yFactor = null;

        if (this.options.getYFactor != null) 
            yFactor = this.options.getYFactor(element);

        if (yFactor == null) 
            yFactor = element.data('flex-y');

        return yFactor ?? 0;
    }

    private resizeElements(): void {
        var width = this.element.width();
        var initialWidth = this.element.data('flexify-width');
        if (initialWidth == null) {
            this.element.data('flexify-width', width);
            initialWidth = width;
        }

        var height = this.element.height();
        var initialHeight = this.element.data('flexify-height');
        if (initialHeight == null) {
            this.element.data('flexify-height', height);
            initialHeight = height;
        }

        this.xDifference = width - initialWidth;
        this.yDifference = height - initialHeight;

        var containers = this.element;
        var tabPanels = this.element.find('.ui-tabs-panel');

        if (tabPanels.length > 0) {
            containers = tabPanels.filter(':visible');
        }

        containers.find('.flexify')
            .add(tabPanels.filter('.flexify:visible'))
            .each((i, e) => {
                this.resizeElement($(e as any));
            });
    }

    private resizeElement(element: JQuery): void {
        var xFactor = this.getXFactor(element);
        if(xFactor !== 0) {
            var initialWidth = element.data('flexify-width');
            if (initialWidth == null) {
                var width = element.width();
                element.data('flexify-width', width);
                initialWidth = width;
            }

            element.width(initialWidth + xFactor * this.xDifference | 0);
        }

        var yFactor = this.getYFactor(element);
        if (yFactor !== 0) {
            var initialHeight = element.data('flexify-height');
            if (initialHeight == null) {
                var height = element.height();
                element.data('flexify-height', height);
                initialHeight = height;
            }
            element.height(initialHeight + yFactor * this.yDifference | 0);
        }

        if (element.hasClass('require-layout')) {
            element.triggerHandler('layout');
        }
    }
}

export interface FlexifyOptions {
    getXFactor?: (p1: JQuery) => any;
    getYFactor?: (p1: JQuery) => any;
    designWidth?: any;
    designHeight?: any;
}
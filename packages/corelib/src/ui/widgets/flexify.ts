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
        super({ element: container, ...options });

        LazyLoadHelper.executeOnceWhenShown(this.domNode, () => {
            this.storeInitialSize();
        });
    }

    storeInitialSize() {
        if (!!$(this.domNode).data('flexify-init')) {
            return;
        }

        var designWidth = this.options.designWidth;
        if (designWidth == null)
            designWidth = $(this.domNode).width();
        $(this.domNode).data('flexify-width', designWidth);

        var designHeight = this.options.designHeight;
        if (designHeight == null)
            designHeight = $(this.domNode).height();
        $(this.domNode).data('flexify-height', designHeight);

        $(this.domNode).data('flexify-init', true);

        $(this.domNode).on('resize.' + this.uniqueName, () =>
            this.resizeElements());

        $(this.domNode).on('resizestop.' + this.uniqueName, () =>
            this.resizeElements());
        
        var tabs = $(this.domNode).find('.ui-tabs');
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
        var width = $(this.domNode).width();
        var initialWidth = $(this.domNode).data('flexify-width');
        if (initialWidth == null) {
            $(this.domNode).data('flexify-width', width);
            initialWidth = width;
        }

        var height = $(this.domNode).height();
        var initialHeight = $(this.domNode).data('flexify-height');
        if (initialHeight == null) {
            $(this.domNode).data('flexify-height', height);
            initialHeight = height;
        }

        this.xDifference = width - initialWidth;
        this.yDifference = height - initialHeight;

        var containers = $(this.domNode);
        var tabPanels = $(this.domNode).find('.ui-tabs-panel');

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
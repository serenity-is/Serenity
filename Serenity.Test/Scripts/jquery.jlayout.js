/**
 * @preserve jLayout JQuery Plugin v0.17
 *
 * Licensed under the new BSD License.
 * Copyright 2008-2009 Bram Stein
 * All rights reserved.
 */
/*global jQuery jLayout*/
if (jQuery && jLayout) {
	(function ($) {
		/**
		 * This wraps jQuery objects in another object that supplies
		 * the methods required for the layout algorithms.
		 */
		function wrap(item, resize) {
			var that = {};

			$.each(['min', 'max'], function (i, name) {
				that[name + 'imumSize'] = function (value) {
                    var l = item.data('jlayout');
                    
					if (l) {
						return l[name + 'imum'](that);
					} else {
						return item[name + 'Size'](value);
					}
				};
			});

			$.extend(that, {
				doLayout: function () {
                    var l = item.data('jlayout');
                    
					if (l) {
                        l.layout(that);
					}
					item.css({position: 'absolute'});
				},
				isVisible: function () {
					return item.isVisible();
				},
				insets: function () {
					var p = item.padding(),
						b = item.border();

					return {
                        'top': p.top, 
						'bottom': p.bottom + b.bottom + b.top, 
						'left': p.left, 
						'right': p.right + b.right + b.left
                    };
				},
				bounds: function (value) {
					var tmp = {};

					if (value) {
						if (typeof value.x === 'number') {
							tmp.left = value.x;
						}
						if (typeof value.y === 'number') {
							tmp.top = value.y;
						}
						if (typeof value.width === 'number') {
							tmp.width = (value.width - (item.outerWidth(true) - item.width()));
							tmp.width = (tmp.width >= 0) ? tmp.width : 0;
						}
						if (typeof value.height === 'number') {
							tmp.height = value.height - (item.outerHeight(true) - item.height());
							tmp.height = (tmp.height >= 0) ? tmp.height : 0;
						}
						item.css(tmp);
						return item;
					} else {
						tmp = item.position();
						return {
							'x': tmp.left,
							'y': tmp.top,
							'width': item.outerWidth(false),
							'height': item.outerHeight(false)
                        };
					}
				},
				preferredSize: function () {
					var minSize,
						maxSize,
						margin = item.margin(),
						size = {width: 0, height: 0},
                        l = item.data('jlayout');

					if (l && resize) {
						size = l.preferred(that);

						minSize = that.minimumSize();
						maxSize = that.maximumSize();

						size.width += margin.left + margin.right;
						size.height += margin.top + margin.bottom;

						if (size.width < minSize.width || size.height < minSize.height) {
							size.width = Math.max(size.width, minSize.width);
							size.height = Math.max(size.height, minSize.height);
						} else if (size.width > maxSize.width || size.height > maxSize.height) {
							size.width = Math.min(size.width, maxSize.width);
							size.height = Math.min(size.height, maxSize.height);
						}
					} else {
                        size = that.bounds();
						size.width += margin.left + margin.right;
						size.height += margin.top + margin.bottom;
					}
					return size;
				}
			});
			return that;
		}

		$.fn.layout = function (options) {
			var opts = $.extend({}, $.fn.layout.defaults, options);
			return $.each(this, function () {
				var element = $(this),
					o = ( element.data('layout') ) ? $.extend(opts, element.data('layout')) : opts,
					elementWrapper = wrap(element, o.resize);

				if (o.type === 'border' && typeof jLayout.border !== 'undefined') {                
					$.each(['north', 'south', 'west', 'east', 'center'], function (i, name) {
						if (element.children().hasClass(name)) {
							o[name] = wrap(element.children('.' + name + ':first'));
						}
					});
					element.data('jlayout', jLayout.border(o));
				} else if (o.type === 'grid' && typeof jLayout.grid !== 'undefined') {
					o.items = [];
					element.children().each(function (i) {
						if (!$(this).hasClass('ui-resizable-handle')) {
							o.items[i] = wrap($(this));
						}
					});
					element.data('jlayout', jLayout.grid(o));
				} else if (o.type === 'flexGrid' && typeof jLayout.flexGrid !== 'undefined') {
					o.items = [];
					element.children().each(function (i) {
						if (!$(this).hasClass('ui-resizable-handle')) {
							o.items[i] = wrap($(this));
						}
					});
					element.data('jlayout', jLayout.flexGrid(o));
				} else if (o.type === 'column' && typeof jLayout.column !== 'undefined') {
					o.items = [];
					element.children().each(function (i) {
						if (!$(this).hasClass('ui-resizable-handle')) {
							o.items[i] = wrap($(this));
						}
					});
					element.data('jlayout', jLayout.column(o));
				} else if (o.type === 'flow' && typeof jLayout.flow !== 'undefined') {
					o.items = [];
					element.children().each(function (i) {
						if (!$(this).hasClass('ui-resizable-handle')) {
							o.items[i] = wrap($(this));
						}
					});
					element.data('jlayout', jLayout.flow(o));					
				}
                
				if (o.resize) {
					elementWrapper.bounds(elementWrapper.preferredSize());
				}
                
				elementWrapper.doLayout();
				element.css({position: 'relative'});
				if ($.ui !== undefined) {
					element.addClass('ui-widget');
				}
			});
		};

		$.fn.layout.defaults = {
			resize: true,
			type: 'grid'
		};
	}(jQuery));
}

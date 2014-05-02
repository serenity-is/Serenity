/**
 * @preserve jLayout Border Layout - JavaScript Layout Algorithms v0.4
 *
 * Licensed under the new BSD License.
 * Copyright 2008-2009, Bram Stein
 * All rights reserved.
 */
/*global jLayout:true */
(function () {
	jLayout = (typeof jLayout === 'undefined') ? {} : jLayout;

	jLayout.border = function (spec) {
		var my = {},
			that = {},
			east = spec.east,
			west = spec.west,
			north = spec.north,
			south = spec.south,
			center = spec.center;

		my.hgap = spec.hgap || 0;
		my.vgap = spec.vgap || 0;

		that.items = function () {
			var items = [];
			if (east) {
				items.push(east);
			}

			if (west) {
				items.push(west);
			}

			if (north) {
				items.push(north);
			}

			if (south) {
				items.push(south);
			}

			if (center) {
				items.push(center);
			}
			return items;
		};		

		that.layout = function (container) {
			var size = container.bounds(),
				insets = container.insets(),
				top = insets.top,
				bottom = size.height - insets.bottom,
				left = insets.left,
				right = size.width - insets.right,
				tmp;

			if (north && north.isVisible()) {
				tmp = north.preferredSize();
				north.bounds({'x': left, 'y': top, 'width': right - left, 'height': tmp.height});
				north.doLayout();

				top += tmp.height + my.vgap;
			}
			if (south && south.isVisible()) {
				tmp = south.preferredSize();
				south.bounds({'x': left, 'y': bottom - tmp.height, 'width': right - left, 'height': tmp.height});
				south.doLayout();

				bottom -= tmp.height + my.vgap;
			}
			if (east && east.isVisible()) {
				tmp = east.preferredSize();
				east.bounds({'x': right - tmp.width, 'y': top, 'width': tmp.width, 'height': bottom - top});
				east.doLayout();

				right -= tmp.width + my.hgap;
			}
			if (west && west.isVisible()) {
				tmp = west.preferredSize();
				west.bounds({'x': left, 'y': top, 'width': tmp.width, 'height': bottom - top});
				west.doLayout();

				left += tmp.width + my.hgap;
			}
			if (center && center.isVisible()) {
				center.bounds({'x': left, 'y': top, 'width': right - left, 'height': bottom - top});
				center.doLayout();
			}
			return container;
		};

		function typeLayout(type) {
			return function (container) {
				var insets = container.insets(),
					width = 0,
					height = 0,
					type_size;

				if (east && east.isVisible()) {
					type_size = east[type + 'Size']();
					width += type_size.width + my.hgap;
					height = type_size.height;
				}
				if (west && west.isVisible()) {
					type_size = west[type + 'Size']();
					width += type_size.width + my.hgap;
					height = Math.max(type_size.height, height);
				}
				if (center && center.isVisible()) {
					type_size = center[type + 'Size']();
					width += type_size.width;
					height = Math.max(type_size.height, height);
				}
				if (north && north.isVisible()) {
					type_size = north[type + 'Size']();
					width = Math.max(type_size.width, width);
					height += type_size.height + my.vgap;
				}
				if (south && south.isVisible()) {
					type_size = south[type + 'Size']();
					width = Math.max(type_size.width, width);
					height += type_size.height + my.vgap;
				}

				return {
					'width': width + insets.left + insets.right, 
					'height': height + insets.top + insets.bottom
				};
			};
		}
		that.preferred = typeLayout('preferred');
		that.minimum = typeLayout('minimum');
		that.maximum = typeLayout('maximum');
		return that;
	};
}());

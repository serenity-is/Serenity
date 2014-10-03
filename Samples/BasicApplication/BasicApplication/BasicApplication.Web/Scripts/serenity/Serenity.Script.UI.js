(function() {
	'use strict';
	var $asm = {};
	global.Serenity = global.Serenity || {};
	global.Serenity.ComponentModel = global.Serenity.ComponentModel || {};
	global.Serenity.Reporting = global.Serenity.Reporting || {};
	global.System = global.System || {};
	global.System.ComponentModel = global.System.ComponentModel || {};
	ss.initAssembly($asm, 'Serenity.Script.UI');
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.BooleanEditor
	var $Serenity_BooleanEditor = function(input) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, input, new Object());
		input.removeClass('flexify');
	};
	$Serenity_BooleanEditor.__typeName = 'Serenity.BooleanEditor';
	global.Serenity.BooleanEditor = $Serenity_BooleanEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CheckListEditor
	var $Serenity_CheckListEditor = function(div, opt) {
		this.$list = null;
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_CheckListEditorOptions]).call(this, div, opt);
		div.addClass('s-CheckListEditor');
		this.$list = $('<ul/>').appendTo(div);
		this.updateItems();
	};
	$Serenity_CheckListEditor.__typeName = 'Serenity.CheckListEditor';
	global.Serenity.CheckListEditor = $Serenity_CheckListEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CheckListEditorOptions
	var $Serenity_CheckListEditorOptions = function() {
	};
	$Serenity_CheckListEditorOptions.__typeName = 'Serenity.CheckListEditorOptions';
	$Serenity_CheckListEditorOptions.createInstance = function() {
		return $Serenity_CheckListEditorOptions.$ctor();
	};
	$Serenity_CheckListEditorOptions.$ctor = function() {
		var $this = {};
		$this.items = null;
		$this.selectAllOptionText = null;
		$this.selectAllOptionText = 'Tümünü Seç';
		$this.items = [];
		return $this;
	};
	global.Serenity.CheckListEditorOptions = $Serenity_CheckListEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CheckTreeEditor
	var $Serenity_CheckTreeEditor$1 = function(TOptions) {
		var $type = function(div, opt) {
			ss.makeGenericType($Serenity_CheckTreeEditor$2, [Object, TOptions]).call(this, div, opt);
		};
		ss.registerGenericClassInstance($type, $Serenity_CheckTreeEditor$1, [TOptions], {}, function() {
			return ss.makeGenericType($Serenity_CheckTreeEditor$2, [Object, TOptions]);
		}, function() {
			return [$Serenity_IDataGrid, $Serenity_IGetEditValue, $Serenity_ISetEditValue];
		});
		return $type;
	};
	$Serenity_CheckTreeEditor$1.__typeName = 'Serenity.CheckTreeEditor$1';
	ss.initGenericClass($Serenity_CheckTreeEditor$1, $asm, 1);
	global.Serenity.CheckTreeEditor$1 = $Serenity_CheckTreeEditor$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CheckTreeEditor
	var $Serenity_CheckTreeEditor$2 = function(TItem, TOptions) {
		var $type = function(div, opt) {
			this.$byId = null;
			ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).call(this, div, opt);
			div.addClass('s-CheckTreeEditor');
			this.updateItems();
		};
		ss.registerGenericClassInstance($type, $Serenity_CheckTreeEditor$2, [TItem, TOptions], {
			getItems: function() {
				return [];
			},
			updateItems: function() {
				var items = this.getItems();
				var itemById = {};
				for (var i = 0; i < items.length; i++) {
					var item = items[i];
					item.children = [];
					if (!Q.isEmptyOrNull(item.id)) {
						itemById[item.id] = item;
					}
					if (!Q.isEmptyOrNull(item.parentId)) {
						var parent = itemById[item.parentId];
						if (ss.isValue(parent)) {
							ss.add(parent.children, item);
						}
					}
				}
				this.view.addData({ Entities: items, Skip: 0, Take: 0, TotalCount: items.length });
				this.updateSelectAll();
				this.updateFlags();
			},
			getEditValue: function(property, target) {
				target[property.name] = this.get_value();
			},
			setEditValue: function(source, property) {
				var list = ss.safeCast(source[property.name], Array);
				if (ss.isValue(list)) {
					this.set_value(list);
				}
			},
			getButtons: function() {
				var selectAllText = this.getSelectAllText();
				if (Q.isEmptyOrNull(selectAllText)) {
					return null;
				}
				var self = this;
				var $t1 = [];
				ss.add($t1, $Serenity_GridSelectAllButtonHelper.define(TItem).call(null, function() {
					return self;
				}, function(x) {
					return x.id;
				}, function(x1) {
					return x1.isSelected;
				}, ss.mkdel(this, function(x2, v) {
					if (x2.isSelected !== v) {
						x2.isSelected = v;
						this.itemSelectedChanged(x2);
					}
				}), null, ss.mkdel(this, function() {
					this.updateFlags();
				})));
				null;
				return $t1;
			},
			itemSelectedChanged: function(item) {
			},
			getSelectAllText: function() {
				return 'Tümünü Seç';
			},
			isThreeStateHierarchy: function() {
				return false;
			},
			createSlickGrid: function() {
				this.element.addClass('slick-no-cell-border').addClass('slick-no-odd-even');
				var result = ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).prototype.createSlickGrid.call(this);
				this.element.addClass('slick-hide-header');
				result.resizeCanvas();
				return result;
			},
			onViewFilter: function(item) {
				if (!ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).prototype.onViewFilter.call(this, item)) {
					return false;
				}
				var items = this.view.getItems();
				var self = this;
				return Serenity.SlickTreeHelper.filterCustom(item, function(x) {
					if (ss.isNullOrUndefined(x.parentId)) {
						return null;
					}
					if (ss.isNullOrUndefined(self.$byId)) {
						self.$byId = {};
						for (var i = 0; i < items.length; i++) {
							var o = items[i];
							if (ss.isValue(o.id)) {
								self.$byId[o.id] = o;
							}
						}
					}
					return self.$byId[x.parentId];
				});
			},
			getInitialCollapse: function() {
				return false;
			},
			onViewProcessData: function(response) {
				response = ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).prototype.onViewProcessData.call(this, response);
				this.$byId = null;
				Serenity.SlickTreeHelper.setIndents(response.Entities, function(x) {
					return x.id;
				}, function(x1) {
					return x1.parentId;
				}, this.getInitialCollapse());
				return response;
			},
			onClick: function(e, row, cell) {
				ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).prototype.onClick.call(this, e, row, cell);
				if (!e.isDefaultPrevented()) {
					Serenity.SlickTreeHelper.toggleClick(TItem).call(null, e, row, cell, this.view, function(x) {
						return x.id;
					});
				}
				if (e.isDefaultPrevented()) {
					return;
				}
				var target = $(e.target);
				if (target.hasClass('check-box')) {
					var checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');
					var item = this.view.rows[row];
					var anyChanged = item.isSelected !== !checkedOrPartial;
					this.view.beginUpdate();
					try {
						if (item.isSelected !== !checkedOrPartial) {
							item.isSelected = !checkedOrPartial;
							this.view.updateItem(item.id, item);
							this.itemSelectedChanged(item);
						}
						anyChanged = this.$setAllSubTreeSelected(item, item.isSelected) | anyChanged;
						this.updateSelectAll();
						this.updateFlags();
					}
					finally {
						this.view.endUpdate();
					}
					if (anyChanged) {
						this.element.triggerHandler('change');
					}
				}
			},
			updateSelectAll: function() {
				$Serenity_GridSelectAllButtonHelper.update(TItem).call(null, this, function(x) {
					return x.isSelected;
				});
			},
			updateFlags: function() {
				var view = this.view;
				var items = view.getItems();
				var threeState = this.isThreeStateHierarchy();
				if (!threeState) {
					return;
				}
				view.beginUpdate();
				try {
					for (var i = 0; i < items.length; i++) {
						var item = items[i];
						if (ss.isNullOrUndefined(item.children) || item.children.length === 0) {
							var allsel = this.getDescendantsSelected(item);
							if (allsel !== item.isAllDescendantsSelected) {
								item.isAllDescendantsSelected = allsel;
								view.updateItem(item.id, item);
							}
							continue;
						}
						var allSelected = this.allDescendantsSelected(item);
						var selected = allSelected || this.anyDescendantsSelected(item);
						if (allSelected !== item.isAllDescendantsSelected || selected !== item.isSelected) {
							var selectedChange = item.isSelected !== selected;
							item.isAllDescendantsSelected = allSelected;
							item.isSelected = selected;
							view.updateItem(item.id, item);
							if (selectedChange) {
								this.itemSelectedChanged(item);
							}
						}
					}
				}
				finally {
					view.endUpdate();
				}
			},
			getDescendantsSelected: function(item) {
				return true;
			},
			$setAllSubTreeSelected: function(item, selected) {
				var result = false;
				for (var i = 0; i < item.children.length; i++) {
					var sub = item.children[i];
					if (sub.isSelected !== selected) {
						result = true;
						sub.isSelected = selected;
						this.view.updateItem(sub.id, sub);
						this.itemSelectedChanged(sub);
					}
					if (sub.children.length > 0) {
						result = this.$setAllSubTreeSelected(sub, selected) | result;
					}
				}
				return result;
			},
			$allItemsSelected: function() {
				for (var $t1 = 0; $t1 < this.view.rows.length; $t1++) {
					var row = this.view.rows[$t1];
					if (!row.isSelected) {
						return false;
					}
				}
				return this.view.rows.length > 0;
			},
			allDescendantsSelected: function(item) {
				if (item.children.length > 0) {
					for (var i = 0; i < item.children.length; i++) {
						var sub = item.children[i];
						if (!sub.isSelected) {
							return false;
						}
						if (!this.allDescendantsSelected(sub)) {
							return false;
						}
					}
				}
				return true;
			},
			anyDescendantsSelected: function(item) {
				if (item.children.length > 0) {
					for (var i = 0; i < item.children.length; i++) {
						var sub = item.children[i];
						if (sub.isSelected) {
							return true;
						}
						if (this.anyDescendantsSelected(sub)) {
							return true;
						}
					}
				}
				return false;
			},
			getColumns: function() {
				var self = this;
				var $t1 = [];
				ss.add($t1, { field: 'text', name: 'Kayıt', width: 80, format: Serenity.SlickFormatting.treeToggle(TItem).call(null, function() {
					return self.view;
				}, function(x) {
					return x.id;
				}, ss.mkdel(this, function(ctx) {
					var cls = 'check-box';
					var item = ss.cast(ctx.item, TItem);
					var threeState = this.isThreeStateHierarchy();
					if (item.isSelected) {
						if (threeState && !item.isAllDescendantsSelected) {
							cls += ' partial';
						}
						else {
							cls += ' checked';
						}
					}
					return '<span class="' + cls + '"></span>' + this.getItemText(ctx);
				})) });
				null;
				return $t1;
			},
			getItemText: function(ctx) {
				return Q.htmlEncode(ctx.value);
			},
			getSlickOptions: function() {
				var opt = ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]).prototype.getSlickOptions.call(this);
				opt.forceFitColumns = true;
				return opt;
			},
			get_value: function() {
				var list = [];
				var items = this.get_view().getItems();
				for (var i = 0; i < items.length; i++) {
					if (items[i].isSelected) {
						ss.add(list, items[i].id);
					}
				}
				return list;
			},
			set_value: function(value) {
				var selected = {};
				if (ss.isValue(value)) {
					for (var i = 0; i < value.length; i++) {
						selected[value[i]] = true;
					}
				}
				this.view.beginUpdate();
				try {
					var items = this.get_view().getItems();
					for (var i1 = 0; i1 < items.length; i1++) {
						var item = items[i1];
						var select = selected[item.id];
						if (select !== item.isSelected) {
							item.isSelected = select;
							this.view.updateItem(item.id, item);
						}
					}
					this.updateSelectAll();
					this.updateFlags();
				}
				finally {
					this.view.endUpdate();
				}
			}
		}, function() {
			return ss.makeGenericType($Serenity_DataGrid$2, [TItem, TOptions]);
		}, function() {
			return [$Serenity_IDataGrid, $Serenity_IGetEditValue, $Serenity_ISetEditValue];
		});
		ss.setMetadata($type, { attr: [new Serenity.ElementAttribute('<div/>'), new Serenity.IdPropertyAttribute('id')] });
		return $type;
	};
	$Serenity_CheckTreeEditor$2.__typeName = 'Serenity.CheckTreeEditor$2';
	ss.initGenericClass($Serenity_CheckTreeEditor$2, $asm, 2);
	global.Serenity.CheckTreeEditor$2 = $Serenity_CheckTreeEditor$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CustomValidation
	var $Serenity_CustomValidation = function() {
	};
	$Serenity_CustomValidation.__typeName = 'Serenity.CustomValidation';
	$Serenity_CustomValidation.registerValidationMethods = function() {
		if (ss.staticEquals($.validator.methods['customValidate'], null)) {
			$.validator.addMethod('customValidate', function(value, element) {
				var result = this.optional(element);
				if (ss.isNullOrUndefined(element) || !!result) {
					return result;
				}
				var events = $._data(element, 'events');
				if (!!ss.isNullOrUndefined(events)) {
					return true;
				}
				var handlers = events.customValidate;
				if (!!(ss.isNullOrUndefined(handlers) || ss.referenceEquals(handlers.length, 0))) {
					return true;
				}
				var el = $(element);
				for (var i = 0; !!(i < handlers.length); i++) {
					var handler = ss.safeCast(handlers[i].handler, Function);
					if (!ss.staticEquals(handler, null)) {
						var message = handler(el);
						if (ss.isValue(message)) {
							el.data('customValidationMessage', message);
							return false;
						}
					}
				}
				return true;
			}, function(o, e) {
				return $(e).data('customValidationMessage');
			});
		}
	};
	global.Serenity.CustomValidation = $Serenity_CustomValidation;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DataGrid
	var $Serenity_DataGrid$1 = function(TItem) {
		var $type = function(container) {
			ss.makeGenericType($Serenity_DataGrid$2, [TItem, Object]).call(this, container, null);
		};
		ss.registerGenericClassInstance($type, $Serenity_DataGrid$1, [TItem], {}, function() {
			return ss.makeGenericType($Serenity_DataGrid$2, [TItem, Object]);
		}, function() {
			return [$Serenity_IDataGrid];
		});
		return $type;
	};
	$Serenity_DataGrid$1.__typeName = 'Serenity.DataGrid$1';
	ss.initGenericClass($Serenity_DataGrid$1, $asm, 1);
	global.Serenity.DataGrid$1 = $Serenity_DataGrid$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DataGrid
	var $Serenity_DataGrid$2 = function(TItem, TOptions) {
		var $type = function(container, opt) {
			this.titleDiv = null;
			this.toolbar = null;
			this.view = null;
			this.slickContainer = null;
			this.slickGrid = null;
			this.idFieldName = null;
			this.isActiveFieldName = null;
			this.localTextPrefix = null;
			this.$isDisabled = false;
			this.$slickGridOnSort = null;
			this.$slickGridOnClick = null;
			ss.makeGenericType($Serenity_Widget$1, [TOptions]).call(this, container, opt);
			var self = this;
			this.element.addClass('s-DataGrid').html('');
			this.element.addClass('s-' + ss.getTypeName(ss.getInstanceType(this)));
			this.element.addClass('require-layout').bind('layout', function() {
				self.layout();
			});
			this.set_title(this.getInitialTitle());
			var buttons = this.getButtons();
			if (ss.isValue(buttons)) {
				this.createToolbar(buttons);
			}
			this.slickContainer = this.createSlickContainer();
			this.view = this.createView();
			this.slickGrid = this.createSlickGrid();
			if (this.usePager()) {
				this.createPager();
			}
			this.bindToSlickEvents();
			this.bindToViewEvents();
			if (ss.isValue(buttons)) {
				this.createToolbarExtensions();
			}
			this.updateDisabledState();
			if (this.populateWhenVisible()) {
				Serenity.LazyLoadHelper.executeEverytimeWhenShown(this.element, function() {
					self.$refreshIfNeeded();
				}, false);
				if (this.element.is(':visible')) {
					this.view.populate();
				}
			}
			else {
				this.view.populate();
			}
		};
		ss.registerGenericClassInstance($type, $Serenity_DataGrid$2, [TItem, TOptions], {
			layout: function() {
				if (!this.element.is(':visible')) {
					return;
				}
				if (ss.isNullOrUndefined(this.slickContainer)) {
					return;
				}
				Q.layoutFillHeight(this.slickContainer);
				if (ss.isValue(this.slickGrid)) {
					this.slickGrid.resizeCanvas();
				}
			},
			getInitialTitle: function() {
				return null;
			},
			createToolbarExtensions: function() {
			},
			createIncludeDeletedButton: function() {
				if (!Q.isEmptyOrNull(this.getIsActiveFieldName())) {
					$Serenity_GridUtils.addIncludeDeletedToggle(this.toolbar.get_element(), this.view, null, false);
				}
			},
			getQuickSearchFields: function() {
				return null;
			},
			createQuickSearchInput: function() {
				$Serenity_GridUtils.addQuickSearchInput(TItem).call(null, this.toolbar.get_element(), this.view, this.getQuickSearchFields());
			},
			destroy: function() {
				if (ss.isValue(this.toolbar)) {
					this.toolbar.destroy();
					this.toolbar = null;
				}
				if (ss.isValue(this.slickGrid)) {
					if (!ss.staticEquals(this.$slickGridOnSort, null)) {
						this.slickGrid.onSort.unsubscribe(this.$slickGridOnSort);
						this.$slickGridOnSort = null;
					}
					if (!ss.staticEquals(this.$slickGridOnClick, null)) {
						this.slickGrid.onSort.unsubscribe(this.$slickGridOnClick);
						this.$slickGridOnClick = null;
					}
					this.slickGrid.destroy();
					this.slickGrid = null;
				}
				if (ss.isValue(this.view)) {
					this.view.onRowsChanged.clear();
					this.view.onRowCountChanged.clear();
					this.view.onSubmit = null;
					this.view.setFilter(null);
					var viewRows = this.view.rows;
					if (!!ss.isValue(viewRows)) {
						viewRows.getItemMetadata = null;
					}
					this.view = null;
				}
				this.titleDiv = null;
				$Serenity_Widget.prototype.destroy.call(this);
			},
			getItemCssClass: function(item, index) {
				var activeFieldName = this.getIsActiveFieldName();
				if (Q.isEmptyOrNull(activeFieldName)) {
					return null;
				}
				var value = item[activeFieldName];
				if (ss.isNullOrUndefined(value)) {
					return null;
				}
				if (typeof(value) === 'number') {
					if (Serenity.IdExtensions.isNegativeId(value)) {
						return 'deleted';
					}
					else if (value === 0) {
						return 'inactive';
					}
				}
				else if (typeof(value) === 'boolean') {
					if (value === false) {
						return 'deleted';
					}
				}
				return null;
			},
			getItemMetadata: function(item, index) {
				var itemClass = this.getItemCssClass(item, index);
				if (Q.isEmptyOrNull(itemClass)) {
					return new Object();
				}
				return { cssClasses: itemClass };
			},
			postProcessColumns: function(columns) {
				Serenity.SlickHelper.setDefaults(columns, this.getLocalTextPrefix());
				return columns;
			},
			createSlickGrid: function() {
				var slickColumns = this.postProcessColumns(this.getColumns());
				var slickOptions = this.getSlickOptions();
				var self = this;
				var viewRows = this.view.rows;
				viewRows.getItemMetadata = function(index) {
					var item = self.view.rows[index];
					return self.getItemMetadata(item, index);
				};
				var grid = new Slick.Grid(this.slickContainer, ss.cast(viewRows, Array), slickColumns, slickOptions);
				grid.registerPlugin(new Slick.AutoTooltips({ enableForHeaderCells: true }));
				grid.setSortColumns(this.getDefaultSortBy().map(function(s) {
					var x = {};
					if (ss.isValue(s) && ss.endsWithString(s.toLowerCase(), ' DESC')) {
						x.columnId = s.substr(0, s.length - 5);
						x.sortAsc = false;
					}
					else {
						x.columnId = s;
						x.sortAsc = true;
					}
					return x;
				}));
				return grid;
			},
			get_items: function() {
				return this.view.getItems();
			},
			set_items: function(value) {
				this.view.setItems(value, true);
			},
			bindToSlickEvents: function() {
				var self = this;
				this.$slickGridOnSort = function(e, p) {
					self.view.populateLock();
					try {
						var sortBy = [];
						if (!!p.multiColumnSort) {
							for (var i = 0; !!(i < p.sortCols.length); i++) {
								var x = p.sortCols[i];
								var $t1 = x.sortCol;
								if (ss.isNullOrUndefined($t1)) {
									$t1 = {};
								}
								var col = $t1;
								ss.add(sortBy, ss.cast(col.field + (!!x.sortAsc ? '' : ' DESC'), String));
							}
						}
						else {
							var $t2 = p.sortCol;
							if (ss.isNullOrUndefined($t2)) {
								$t2 = {};
							}
							var col1 = $t2;
							ss.add(sortBy, ss.cast(col1.field + (!!p.sortAsc ? '' : ' DESC'), String));
						}
						self.view.sortBy = sortBy;
					}
					finally {
						self.view.populateUnlock();
					}
					self.view.populate();
				};
				this.slickGrid.onSort.subscribe(this.$slickGridOnSort);
				this.$slickGridOnClick = function(e1, p1) {
					self.onClick(e1, ss.unbox(ss.cast(p1.row, ss.Int32)), ss.unbox(ss.cast(p1.cell, ss.Int32)));
				};
				this.slickGrid.onClick.subscribe(this.$slickGridOnClick);
			},
			getAddButtonCaption: function() {
				return 'Yeni';
			},
			getButtons: function() {
				return [];
			},
			editItem: function(entityOrId) {
			},
			onClick: function(e, row, cell) {
				var target = $(e.target);
				if (target.hasClass('s-' + this.getItemType() + 'Link')) {
					e.preventDefault();
					this.editItem(Serenity.SlickFormatting.getItemId$1(target));
				}
			},
			$viewRowCountChanged: function(e, d) {
				this.slickGrid.updateRowCount();
				this.slickGrid.render();
			},
			$viewRowsChanged: function(e, rows) {
				if (!!ss.isNullOrUndefined(rows)) {
					this.slickGrid.invalidate();
					this.markupReady();
				}
				else {
					this.slickGrid.invalidate();
					this.slickGrid.render();
				}
			},
			bindToViewEvents: function() {
				var self = this;
				this.view.onRowCountChanged.subscribe(function(e, d) {
					self.$viewRowCountChanged(e, d);
				});
				this.view.onRowsChanged.subscribe(function(e1, d1) {
					self.$viewRowsChanged(e1, d1);
				});
				this.view.onSubmit = function(view) {
					return self.onViewSubmit();
				};
				this.view.setFilter(function(item, view1) {
					return self.onViewFilter(item);
				});
				this.view.onProcessData = function(response, view2) {
					return self.onViewProcessData(response);
				};
			},
			onViewProcessData: function(response) {
				return response;
			},
			onViewFilter: function(item) {
				return true;
			},
			getIncludeColumns: function(include) {
				var $t1 = this.slickGrid.getColumns();
				for (var $t2 = 0; $t2 < $t1.length; $t2++) {
					var column = $t1[$t2];
					if (ss.isValue(column.field)) {
						include[column.field] = true;
					}
				}
			},
			setIncludeColumnsParameter: function() {
				var include = {};
				if (!!!ss.isNullOrUndefined(this.view.params.IncludeColumns)) {
					var $t1 = ss.cast(this.view.params.IncludeColumns, Array);
					for (var $t2 = 0; $t2 < $t1.length; $t2++) {
						var key = $t1[$t2];
						include[key] = true;
					}
				}
				this.getIncludeColumns(include);
				var array = null;
				if (ss.getKeyCount(include) > 0) {
					array = [];
					var $t3 = ss.getEnumerator(Object.keys(include));
					try {
						while ($t3.moveNext()) {
							var key1 = $t3.current();
							ss.add(array, key1);
						}
					}
					finally {
						$t3.dispose();
					}
				}
				this.view.params.IncludeColumns = array;
			},
			onViewSubmit: function() {
				if (this.get_isDisabled() || !this.getGridCanLoad()) {
					this.view.setItems([], true);
					return false;
				}
				this.setIncludeColumnsParameter();
				return true;
			},
			markupReady: function() {
			},
			createSlickContainer: function() {
				return $('<div class="grid-container"></div>').appendTo(this.element);
			},
			createView: function() {
				var opt = this.getViewOptions();
				return new Slick.Data.RemoteView(opt);
			},
			getDefaultSortBy: function() {
				var $t1 = [];
				ss.add($t1, this.getIdFieldName());
				null;
				return $t1;
			},
			usePager: function() {
				return false;
			},
			populateWhenVisible: function() {
				return false;
			},
			createPager: function() {
				var pagerDiv = $('<div></div>').appendTo(this.element);
				pagerDiv.slickPager({ view: this.view, rowsPerPage: 20, rowsPerPageOptions: [20, 100, 500, 2500] });
			},
			getViewOptions: function() {
				var opt = {};
				opt.idField = this.getIdFieldName();
				opt.sortBy = this.getDefaultSortBy();
				if (!this.usePager()) {
					opt.rowsPerPage = 0;
				}
				else {
					opt.rowsPerPage = 100;
				}
				return opt;
			},
			createToolbar: function(buttons) {
				var toolbarDiv = $('<div class="grid-toolbar"></div>').appendTo(this.get_element());
				this.toolbar = new $Serenity_Toolbar(toolbarDiv, { buttons: buttons });
			},
			get_title: function() {
				if (ss.isNullOrUndefined(this.titleDiv)) {
					return null;
				}
				return this.titleDiv.children().text();
			},
			set_title: function(value) {
				if (!ss.referenceEquals(value, this.get_title())) {
					if (ss.isNullOrUndefined(value)) {
						if (ss.isValue(this.titleDiv)) {
							this.titleDiv.remove();
							this.titleDiv = null;
						}
					}
					else if (ss.isNullOrUndefined(this.titleDiv)) {
						this.titleDiv = $('<div class="grid-title"><div class="title-text"></div></div>').prependTo(this.get_element());
						this.titleDiv.children().text(value);
					}
					this.layout();
				}
			},
			getItemType: function() {
				return 'Item';
			},
			itemLink: function(itemType, idField, text, cssClass) {
				var $t1 = itemType;
				if (ss.isNullOrUndefined($t1)) {
					$t1 = this.getItemType();
				}
				itemType = $t1;
				var $t2 = idField;
				if (ss.isNullOrUndefined($t2)) {
					$t2 = this.getIdFieldName();
				}
				idField = $t2;
				return Serenity.SlickFormatting.itemLink(itemType, idField, text, cssClass);
			},
			getColumns: function() {
				return [];
			},
			getSlickOptions: function() {
				var opt = {};
				opt.multiSelect = false;
				opt.multiColumnSort = true;
				opt.enableCellNavigation = false;
				opt.headerRowHeight = 30;
				opt.rowHeight = 27;
				return opt;
			},
			populateLock: function() {
				this.view.populateLock();
			},
			populateUnlock: function() {
				this.view.populateUnlock();
			},
			getGridCanLoad: function() {
				return true;
			},
			refresh: function() {
				if (!this.populateWhenVisible()) {
					this.internalRefresh();
					return;
				}
				if (this.slickContainer.is(':visible')) {
					this.slickContainer.data('needsRefresh', false);
					this.internalRefresh();
					return;
				}
				this.slickContainer.data('needsRefresh', true);
			},
			$refreshIfNeeded: function() {
				if (!!this.slickContainer.data('needsRefresh')) {
					this.slickContainer.data('needsRefresh', false);
					this.internalRefresh();
				}
			},
			internalRefresh: function() {
				this.view.populate();
			},
			get_isDisabled: function() {
				return this.$isDisabled;
			},
			set_isDisabled: function(value) {
				if (this.$isDisabled !== value) {
					this.$isDisabled = value;
					if (this.$isDisabled) {
						this.view.setItems([], true);
					}
					this.updateDisabledState();
				}
			},
			getLocalTextPrefix: function() {
				if (ss.isNullOrUndefined(this.localTextPrefix)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.LocalTextPrefixAttribute, true);
					if (attributes.length >= 1) {
						this.localTextPrefix = attributes[0].get_value();
					}
					else {
						this.localTextPrefix = '';
					}
				}
				return this.localTextPrefix;
			},
			getIdFieldName: function() {
				if (ss.isNullOrUndefined(this.idFieldName)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IdPropertyAttribute, true);
					if (attributes.length === 1) {
						this.idFieldName = attributes[0].get_value();
					}
					else {
						this.idFieldName = 'ID';
					}
				}
				return this.idFieldName;
			},
			getIsActiveFieldName: function() {
				if (ss.isNullOrUndefined(this.isActiveFieldName)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IsActivePropertyAttribute, true);
					if (attributes.length === 1) {
						this.isActiveFieldName = attributes[0].get_value();
					}
					this.isActiveFieldName = '';
				}
				return this.isActiveFieldName;
			},
			updateDisabledState: function() {
				this.slickContainer.toggleClass('ui-state-disabled', !!this.get_isDisabled());
			},
			resizeCanvas: function() {
				this.slickGrid.resizeCanvas();
			},
			subDialogDataChange: function() {
				this.refresh();
			},
			get_view: function() {
				return this.view;
			},
			get_slickGrid: function() {
				return this.slickGrid;
			}
		}, function() {
			return ss.makeGenericType($Serenity_Widget$1, [TOptions]);
		}, function() {
			return [$Serenity_IDataGrid];
		});
		return $type;
	};
	$Serenity_DataGrid$2.__typeName = 'Serenity.DataGrid$2';
	ss.initGenericClass($Serenity_DataGrid$2, $asm, 2);
	global.Serenity.DataGrid$2 = $Serenity_DataGrid$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateEditor
	var $Serenity_DateEditor = function(input) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, input, new Object());
		input.addClass('dateQ');
		input.datepicker({
			showOn: 'button',
			beforeShow: function() {
				return !input.hasClass('readonly');
			}
		});
	};
	$Serenity_DateEditor.__typeName = 'Serenity.DateEditor';
	$Serenity_DateEditor.defaultAutoNumericOptions = function() {
		return { aDec: Q$Culture.decimalSeparator, altDec: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aSep: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aPad: true };
	};
	global.Serenity.DateEditor = $Serenity_DateEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateTimeEditor
	var $Serenity_DateTimeEditor = function(input) {
		this.$time = null;
		$Serenity_Widget.call(this, input);
		input.addClass('dateQ s-DateTimeEditor').datepicker({ showOn: 'button' });
		this.$time = $('<select/>').addClass('editor s-DateTimeEditor time').insertAfter(input.next('.ui-datepicker-trigger'));
		var $t1 = $Serenity_DateTimeEditor.$getTimeOptions(null, null, 23, 59, 30);
		for (var $t2 = 0; $t2 < $t1.length; $t2++) {
			var t = $t1[$t2];
			Q.addOption(this.$time, t, t);
		}
	};
	$Serenity_DateTimeEditor.__typeName = 'Serenity.DateTimeEditor';
	$Serenity_DateTimeEditor.$getTimeOptions = function(fromHour, fromMin, toHour, toMin, stepMins) {
		var list = [];
		fromHour = ss.coalesce(fromHour, 0);
		if (ss.isNullOrUndefined(toHour)) {
			toHour = 0;
		}
		else if (ss.Nullable$1.ge(toHour, 23)) {
			toHour = 23;
		}
		if (ss.isNullOrUndefined(fromMin)) {
			fromMin = 0;
		}
		if (ss.isNullOrUndefined(toMin)) {
			toMin = 0;
		}
		else if (ss.Nullable$1.ge(toMin, 60)) {
			toMin = 59;
		}
		var hour = fromHour;
		var min = fromMin;
		while (true) {
			if (ss.Nullable$1.gt(hour, toHour) || ss.referenceEquals(hour, toHour) && ss.Nullable$1.gt(min, toMin)) {
				break;
			}
			var t = (ss.Nullable$1.ge(hour, 10) ? '' : '0') + hour + ':' + (ss.Nullable$1.ge(min, 10) ? '' : '0') + min;
			ss.add(list, t);
			min = ss.Nullable$1.add(min, stepMins);
			if (ss.Nullable$1.ge(min, 60)) {
				min = ss.Nullable$1.sub(min, 60);
				hour = ss.Nullable$1.add(hour, 1);
			}
		}
		return list;
	};
	$Serenity_DateTimeEditor.defaultAutoNumericOptions = function() {
		return { aDec: Q$Culture.decimalSeparator, altDec: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aSep: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aPad: true };
	};
	global.Serenity.DateTimeEditor = $Serenity_DateTimeEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateYearEditor
	var $Serenity_DateYearEditor = function(hidden, opt) {
		$Serenity_SelectEditor.call(this, hidden, opt);
		this.updateItems();
	};
	$Serenity_DateYearEditor.__typeName = 'Serenity.DateYearEditor';
	global.Serenity.DateYearEditor = $Serenity_DateYearEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateYearEditorOptions
	var $Serenity_DateYearEditorOptions = function() {
	};
	$Serenity_DateYearEditorOptions.__typeName = 'Serenity.DateYearEditorOptions';
	$Serenity_DateYearEditorOptions.createInstance = function() {
		return $Serenity_DateYearEditorOptions.$ctor();
	};
	$Serenity_DateYearEditorOptions.$ctor = function() {
		var $this = $Serenity_SelectEditorOptions.$ctor();
		$this.minYear = null;
		$this.maxYear = null;
		$this.descending = false;
		$this.minYear = '-10';
		$this.maxYear = '+0';
		return $this;
	};
	global.Serenity.DateYearEditorOptions = $Serenity_DateYearEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DecimalEditor
	var $Serenity_DecimalEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_DecimalEditorOptions]).call(this, input, opt);
		input.addClass('decimalQ');
		var numericOptions = $.extend($Serenity_DecimalEditor.defaultAutoNumericOptions(), { vMin: this.options.minValue, vMax: this.options.maxValue });
		if (ss.isValue(this.options.decimals)) {
			numericOptions.mDec = ss.unbox(this.options.decimals);
		}
		if (ss.isValue(this.options.padDecimals)) {
			numericOptions.aPad = ss.unbox(this.options.padDecimals);
		}
		input.autoNumeric(numericOptions);
	};
	$Serenity_DecimalEditor.__typeName = 'Serenity.DecimalEditor';
	$Serenity_DecimalEditor.defaultAutoNumericOptions = function() {
		return { aDec: Q$Culture.decimalSeparator, altDec: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aSep: ((Q$Culture.decimalSeparator === '.') ? ',' : '.'), aPad: true };
	};
	global.Serenity.DecimalEditor = $Serenity_DecimalEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DecimalEditorOptions
	var $Serenity_DecimalEditorOptions = function() {
	};
	$Serenity_DecimalEditorOptions.__typeName = 'Serenity.DecimalEditorOptions';
	$Serenity_DecimalEditorOptions.createInstance = function() {
		return $Serenity_DecimalEditorOptions.$ctor();
	};
	$Serenity_DecimalEditorOptions.$ctor = function() {
		var $this = {};
		$this.minValue = null;
		$this.maxValue = null;
		$this.decimals = null;
		$this.padDecimals = null;
		$this.minValue = '0.00';
		$this.maxValue = '999999999999.99';
		return $this;
	};
	global.Serenity.DecimalEditorOptions = $Serenity_DecimalEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DialogExtensions
	var $Serenity_DialogExtensions = function() {
	};
	$Serenity_DialogExtensions.__typeName = 'Serenity.DialogExtensions';
	$Serenity_DialogExtensions.dialogFlexify = function(dialog) {
		var flexify = new $Serenity_Flexify(dialog.closest('.ui-dialog'), {});
		return dialog;
	};
	$Serenity_DialogExtensions.dialogResizable = function(dialog, w, h, mw, mh) {
		var dlg = dialog.dialog();
		dlg.dialog('option', 'resizable', true);
		if (ss.isValue(mw)) {
			dlg.dialog('option', 'minWidth', ss.unbox(mw));
		}
		if (ss.isValue(w)) {
			dlg.dialog('option', 'width', ss.unbox(w));
		}
		if (ss.isValue(mh)) {
			dlg.dialog('option', 'minHeight', ss.unbox(mh));
		}
		if (ss.isValue(h)) {
			dlg.dialog('option', 'height', ss.unbox(h));
		}
		return dialog;
	};
	$Serenity_DialogExtensions.dialogMaximizable = function(dialog) {
		dialog.dialogExtend({ closable: true, maximizable: true, dblclick: 'maximize', icons: { maximize: 'ui-icon-maximize-window' } });
		return dialog;
	};
	$Serenity_DialogExtensions.dialogCloseOnEnter = function(dialog) {
		dialog.bind('keydown', function(e) {
			if (e.which !== $Serenity_DialogExtensions.$enterKeyCode) {
				return;
			}
			var tagName = e.target.tagName.toLowerCase();
			if (tagName === 'button' || tagName === 'select' || tagName === 'textarea' || tagName === 'input' && e.target.getAttribute('type') === 'button') {
				return;
			}
			var dlg = $(this);
			if (!dlg.hasClass('ui-dialog')) {
				dlg = dlg.closest('.ui-dialog');
			}
			var buttons = dlg.children('.ui-dialog-buttonpane').find('button');
			if (buttons.length > 0) {
				var defaultButton = buttons.find('.default-button');
				if (defaultButton.length > 0) {
					buttons = defaultButton;
				}
			}
			var button = buttons.eq(0);
			if (!button.is(':disabled')) {
				e.preventDefault();
				button.trigger('click');
			}
		});
		return dialog;
	};
	global.Serenity.DialogExtensions = $Serenity_DialogExtensions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EditorTypeCache
	var $Serenity_EditorTypeCache = function() {
	};
	$Serenity_EditorTypeCache.__typeName = 'Serenity.EditorTypeCache';
	$Serenity_EditorTypeCache.$registerTypesInNamespace = function(ns) {
		var nsObj = window.window;
		var $t1 = ss.netSplit(ns, [46].map(function(i) {
			return String.fromCharCode(i);
		}), null, 1);
		for (var $t2 = 0; $t2 < $t1.length; $t2++) {
			var x = $t1[$t2];
			nsObj = nsObj[x];
			if (ss.isNullOrUndefined(nsObj)) {
				return;
			}
		}
		var $t3 = Object.keys(nsObj);
		for (var $t4 = 0; $t4 < $t3.length; $t4++) {
			var k = $t3[$t4];
			var obj = nsObj[k];
			if (ss.isNullOrUndefined(obj)) {
				continue;
			}
			var name = ns + '.' + k;
			$Serenity_EditorTypeCache.$visited[name] = true;
			if (typeof(obj) === 'function') {
				var type = ss.getType(name);
				if (ss.isNullOrUndefined(type)) {
					continue;
				}
				var attr = ss.getAttributes(type, Serenity.EditorAttribute, true);
				if (ss.isValue(attr) && attr.length > 0) {
					$Serenity_EditorTypeCache.$registerType(type);
				}
			}
			else {
				$Serenity_EditorTypeCache.$registerTypesInNamespace(name);
				continue;
			}
		}
	};
	$Serenity_EditorTypeCache.$registerType = function(type) {
		var name = ss.getTypeFullName(type);
		var idx = name.indexOf(String.fromCharCode(46));
		if (idx >= 0) {
			name = name.substr(idx + 1);
		}
		var info = { type: type };
		var displayAttr = ss.getAttributes(type, $System_ComponentModel_DisplayNameAttribute, true);
		if (ss.isValue(displayAttr) && displayAttr.length > 0) {
			info.displayName = ss.cast(displayAttr[0], $System_ComponentModel_DisplayNameAttribute).get_displayName();
		}
		else {
			info.displayName = ss.getTypeFullName(type);
		}
		var optionsAttr = ss.getAttributes(type, Serenity.OptionsTypeAttribute, true);
		if (ss.isValue(optionsAttr) && optionsAttr.length > 0) {
			info.optionsType = ss.cast(optionsAttr[0], Serenity.OptionsTypeAttribute).get_optionsType();
		}
		$Serenity_EditorTypeCache.$registeredTypes[name] = info;
	};
	$Serenity_EditorTypeCache.get_registeredTypes = function() {
		if (ss.isNullOrUndefined($Serenity_EditorTypeCache.$registeredTypes)) {
			$Serenity_EditorTypeCache.$visited = {};
			$Serenity_EditorTypeCache.$registeredTypes = {};
			for (var $t1 = 0; $t1 < Q$Config.rootNamespaces.length; $t1++) {
				var ns = Q$Config.rootNamespaces[$t1];
				$Serenity_EditorTypeCache.$registerTypesInNamespace(ns);
			}
		}
		return $Serenity_EditorTypeCache.$registeredTypes;
	};
	global.Serenity.EditorTypeCache = $Serenity_EditorTypeCache;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EditorTypeEditor
	var $Serenity_EditorTypeEditor = function(select) {
		var $t1 = $Serenity_SelectEditorOptions.$ctor();
		$t1.emptyOptionText = '--seçiniz--';
		$Serenity_SelectEditor.call(this, select, $t1);
	};
	$Serenity_EditorTypeEditor.__typeName = 'Serenity.EditorTypeEditor';
	global.Serenity.EditorTypeEditor = $Serenity_EditorTypeEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EmailEditor
	var $Serenity_EmailEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_EmailEditorOptions]).call(this, input, opt);
		$Serenity_EmailEditor.registerValidationMethods();
		input.addClass('emailuser').removeClass('flexify');
		var spanAt = $('<span/>').text('@').addClass('emailat').insertAfter(input);
		var domain = $('<input type="text"/>').addClass('emaildomain').addClass('flexify').insertAfter(spanAt);
		domain.bind('blur.' + this.uniqueName, function() {
			var validator = domain.closest('form').data('validator');
			if (ss.isValue(validator)) {
				validator.element(input[0]);
			}
		});
		if (!Q.isEmptyOrNull(this.options.domain)) {
			domain.val(this.options.domain);
		}
		if (this.options.readOnlyDomain) {
			domain.attr('readonly', 'readonly').addClass('disabled').attr('tabindex', '-1');
		}
		input.bind('keypress.' + this.uniqueName, ss.mkdel(this, function(e) {
			if (e.which === 64) {
				e.preventDefault();
				if (!this.options.readOnlyDomain) {
					domain.focus();
					domain.select();
				}
			}
		}));
		domain.bind('keypress.' + this.uniqueName, function(e1) {
			if (e1.which === 64) {
				e1.preventDefault();
			}
		});
	};
	$Serenity_EmailEditor.__typeName = 'Serenity.EmailEditor';
	$Serenity_EmailEditor.registerValidationMethods = function() {
		if (ss.staticEquals($.validator.methods['emailuser'], null)) {
			$.validator.addMethod('emailuser', function(value, element) {
				var domain = $(element).nextAll('.emaildomain');
				if (domain.length > 0 && ss.isNullOrUndefined(domain.attr('readonly'))) {
					if (this.optional(element) && this.optional(domain[0])) {
						return true;
					}
					value = value + '@' + domain.val();
					if (Q$Config.emailAllowOnlyAscii) {
						return (new RegExp("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")).test(value);
					}
					return (new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))$", 'i')).test(value);
				}
				else {
					if (Q$Config.emailAllowOnlyAscii) {
						return this.optional(element) || (new RegExp("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+$")).test(value);
					}
					return this.optional(element) || (new RegExp("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))$", 'i')).test(value);
				}
			}, 'Lütfen geçerli bir e-posta adresi giriniz!');
		}
	};
	global.Serenity.EmailEditor = $Serenity_EmailEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EmailEditorOptions
	var $Serenity_EmailEditorOptions = function() {
	};
	$Serenity_EmailEditorOptions.__typeName = 'Serenity.EmailEditorOptions';
	$Serenity_EmailEditorOptions.createInstance = function() {
		return $Serenity_EmailEditorOptions.$ctor();
	};
	$Serenity_EmailEditorOptions.$ctor = function() {
		var $this = {};
		$this.domain = null;
		$this.readOnlyDomain = false;
		return $this;
	};
	global.Serenity.EmailEditorOptions = $Serenity_EmailEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EntityDialog
	var $Serenity_EntityDialog$1 = function(TEntity) {
		var $type = function() {
			ss.makeGenericType($Serenity_EntityDialog$2, [TEntity, Object]).$ctor1.call(this, null);
		};
		ss.registerGenericClassInstance($type, $Serenity_EntityDialog$1, [TEntity], {}, function() {
			return ss.makeGenericType($Serenity_EntityDialog$2, [TEntity, Object]);
		}, function() {
			return [$Serenity_IDialog];
		});
		return $type;
	};
	$Serenity_EntityDialog$1.__typeName = 'Serenity.EntityDialog$1';
	ss.initGenericClass($Serenity_EntityDialog$1, $asm, 1);
	global.Serenity.EntityDialog$1 = $Serenity_EntityDialog$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EntityDialog
	var $Serenity_EntityDialog$2 = function(TEntity, TOptions) {
		var $type = function() {
			$type.$ctor2.call(this, Q.newBodyDiv(), null);
		};
		$type.$ctor1 = function(opt) {
			$type.$ctor2.call(this, Q.newBodyDiv(), opt);
		};
		$type.$ctor2 = function(div, opt) {
			this.$entity = null;
			this.$entityId = null;
			this.$entityType = null;
			this.$localTextPrefix = null;
			this.$entitySingular = null;
			this.$entityIdField = null;
			this.$entityNameField = null;
			this.$entityIsActiveField = null;
			this.$formKey = null;
			this.$service = null;
			this.localizationGrid = null;
			this.localizationSelect = null;
			this.propertyGrid = null;
			this.saveAndCloseButton = null;
			this.applyChangesButton = null;
			this.deleteButton = null;
			this.undeleteButton = null;
			this.cloneButton = null;
			ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).$ctor2.call(this, div, opt);
			if (!this.isAsyncWidget()) {
				this.$initPropertyGrid();
				this.$initLocalizationGrid();
			}
		};
		$type.$getLanguages = function() {
			var $t1 = [];
			ss.add($t1, { item1: '', item2: 'Türkçe' });
			null;
			ss.add($t1, { item1: 1033, item2: 'English' });
			null;
			ss.add($t1, { item1: 3082, item2: 'Espanol' });
			null;
			return $t1;
		};
		ss.registerGenericClassInstance($type, $Serenity_EntityDialog$2, [TEntity, TOptions], {
			initializeAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, $t2, $t3;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]).prototype.initializeAsync.call(this);
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$t1.getAwaitedResult();
									$t2 = this.$initPropertyGridAsync();
									$state = 2;
									$t2.continueWith($sm);
									return;
								}
								case 2: {
									$state = -1;
									$t2.getAwaitedResult();
									$t3 = this.$initLocalizationGridAsync();
									$state = 3;
									$t3.continueWith($sm);
									return;
								}
								case 3: {
									$state = -1;
									$t3.getAwaitedResult();
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t4) {
						$tcs.setException(ss.Exception.wrap($t4));
					}
				});
				$sm();
				return $tcs.task;
			},
			destroy: function() {
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.destroy();
					this.propertyGrid = null;
				}
				if (ss.isValue(this.localizationGrid)) {
					this.localizationGrid.destroy();
					this.localizationGrid = null;
				}
				this.undeleteButton = null;
				this.applyChangesButton = null;
				this.deleteButton = null;
				this.saveAndCloseButton = null;
				ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).prototype.destroy.call(this);
			},
			get_entity: function() {
				return this.$entity;
			},
			set_entity: function(value) {
				this.$entity = value || ss.createInstance(TEntity);
			},
			get_entityId: function() {
				return this.$entityId;
			},
			set_entityId: function(value) {
				this.$entityId = value;
			},
			getEntityNameFieldValue: function() {
				return ss.coalesce(this.get_entity()[this.getEntityNameField()], '').toString();
			},
			getEntityTitle: function() {
				if (!this.get_isEditMode()) {
					return ss.formatString(Texts$Controls$EntityDialog.NewRecordTitle.get(), this.getEntitySingular());
				}
				else {
					var title = ss.coalesce(this.getEntityNameFieldValue(), '');
					return ss.formatString(Texts$Controls$EntityDialog.EditRecordTitle.get(), this.getEntitySingular(), (Q.isEmptyOrNull(title) ? '' : (' (' + title + ')')));
				}
			},
			updateTitle: function() {
				if (!this.isPanel) {
					this.element.dialog().dialog('option', 'title', this.getEntityTitle());
				}
			},
			getTemplateName: function() {
				var templateName = ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]).prototype.getTemplateName.call(this);
				if (!Q.canLoadScriptData('Template.' + templateName) && Q.canLoadScriptData('Template.EntityDialog')) {
					return 'EntityDialog';
				}
				return templateName;
			},
			get_isCloneMode: function() {
				return ss.isValue(this.get_entityId()) && Serenity.IdExtensions.isNegativeId(ss.unbox(this.get_entityId()));
			},
			get_isEditMode: function() {
				return ss.isValue(this.get_entityId()) && Serenity.IdExtensions.isPositiveId(ss.unbox(this.get_entityId()));
			},
			get_isDeleted: function() {
				if (ss.isNullOrUndefined(this.get_entityId())) {
					return false;
				}
				var value = this.get_entity()[this.getEntityIsActiveField()];
				if (ss.isNullOrUndefined(value)) {
					return false;
				}
				return Serenity.IdExtensions.isNegativeId(ss.unbox(value));
			},
			get_isNew: function() {
				return ss.isNullOrUndefined(this.get_entityId());
			},
			get_isNewOrDeleted: function() {
				return this.get_isNew() || this.get_isDeleted();
			},
			getDeleteOptions: function(callback) {
				return {};
			},
			deleteHandler: function(options, callback) {
				Q.serviceCall(options);
			},
			doDelete: function(callback) {
				var self = this;
				var baseOptions = {};
				baseOptions.service = this.getService() + '/Delete';
				var request = {};
				request.EntityId = this.get_entityId();
				baseOptions.request = request;
				baseOptions.onSuccess = ss.mkdel(this, function(response) {
					self.onDeleteSuccess(response);
					if (!ss.staticEquals(callback, null)) {
						callback(response);
					}
					self.element.triggerHandler('ondatachange', [{ entityId: request.EntityId, entity: this.$entity, type: 'delete' }]);
				});
				var thisOptions = this.getDeleteOptions(callback);
				var finalOptions = $.extend(baseOptions, thisOptions);
				this.deleteHandler(finalOptions, callback);
			},
			onDeleteSuccess: function(response) {
			},
			getEntityType: function() {
				if (ss.isNullOrUndefined(this.$entityType)) {
					var typeAttributes = ss.getAttributes(ss.getInstanceType(this), Serenity.EntityTypeAttribute, true);
					if (typeAttributes.length === 1) {
						this.$entityType = typeAttributes[0].get_value();
						return this.$entityType;
					}
					// remove global namespace
					var name = ss.getTypeFullName(ss.getInstanceType(this));
					var px = name.indexOf('.');
					if (px >= 0) {
						name = name.substring(px + 1);
					}
					// don't like this kind of convention, make it obsolete soon...
					if (ss.endsWithString(name, 'Dialog') || ss.endsWithString(name, 'Control')) {
						name = name.substr(0, name.length - 6);
					}
					else if (ss.endsWithString(name, 'Panel')) {
						name = name.substr(0, name.length - 5);
					}
					this.$entityType = name;
				}
				return this.$entityType;
			},
			getFormKey: function() {
				if (ss.isNullOrUndefined(this.$formKey)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.FormKeyAttribute, true);
					if (attributes.length >= 1) {
						this.$formKey = attributes[0].get_value();
					}
					else {
						this.$formKey = this.getEntityType();
					}
				}
				return this.$formKey;
			},
			getLocalTextPrefix: function() {
				if (ss.isNullOrUndefined(this.$localTextPrefix)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.LocalTextPrefixAttribute, true);
					if (attributes.length >= 1) {
						this.$localTextPrefix = attributes[0].get_value();
					}
					else {
						this.$localTextPrefix = this.getEntityType();
					}
					this.$localTextPrefix = 'Db.' + this.$localTextPrefix + '.';
				}
				return this.$localTextPrefix;
			},
			getEntitySingular: function() {
				if (ss.isNullOrUndefined(this.$entitySingular)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.ItemNameAttribute, true);
					if (attributes.length >= 1) {
						this.$entitySingular = attributes[0].get_value();
						this.$entitySingular = Q$LT.getDefault(this.$entitySingular, this.$entitySingular);
					}
					else {
						var $t1 = Q.tryGetText(this.getLocalTextPrefix() + 'EntitySingular');
						if (ss.isNullOrUndefined($t1)) {
							$t1 = this.getEntityType();
						}
						this.$entitySingular = $t1;
					}
				}
				return this.$entitySingular;
			},
			getEntityNameField: function() {
				if (ss.isNullOrUndefined(this.$entityNameField)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.NamePropertyAttribute, true);
					if (attributes.length >= 1) {
						this.$entityNameField = attributes[0].get_value();
					}
					else {
						this.$entityNameField = 'Name';
					}
				}
				return this.$entityNameField;
			},
			getEntityIdField: function() {
				if (ss.isNullOrUndefined(this.$entityIdField)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IdPropertyAttribute, true);
					if (attributes.length >= 1) {
						this.$entityIdField = attributes[0].get_value();
					}
					else {
						this.$entityIdField = 'ID';
					}
				}
				return this.$entityIdField;
			},
			getEntityIsActiveField: function() {
				if (ss.isNullOrUndefined(this.$entityIsActiveField)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.IsActivePropertyAttribute, true);
					if (attributes.length >= 1) {
						this.$entityIsActiveField = attributes[0].get_value();
					}
					else {
						this.$entityIsActiveField = 'IsActive';
					}
				}
				return this.$entityIsActiveField;
			},
			getService: function() {
				if (ss.isNullOrUndefined(this.$service)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.ServiceAttribute, true);
					if (attributes.length >= 1) {
						this.$service = attributes[0].get_value();
					}
					else {
						this.$service = ss.replaceAllString(this.getEntityType(), String.fromCharCode(46), String.fromCharCode(47));
					}
				}
				return this.$service;
			},
			loadNewAndOpenDialog: function() {
				this.loadResponse({});
				if (!this.isPanel) {
					this.element.dialog().dialog('open');
				}
			},
			loadEntityAndOpenDialog: function(entity) {
				this.loadResponse({ Entity: entity });
				if (!this.isPanel) {
					this.element.dialog().dialog('open');
				}
			},
			loadResponse: function(data) {
				data = data || {};
				this.onLoadingData(data);
				var entity = data.Entity || ss.createInstance(TEntity);
				this.beforeLoadEntity(entity);
				this.loadEntity(entity);
				this.set_entity(entity);
				this.afterLoadEntity();
			},
			loadEntity: function(entity) {
				var idField = this.getEntityIdField();
				if (ss.isValue(idField)) {
					this.set_entityId(Serenity.IdExtensions.convertToId(entity[idField]));
				}
				this.set_entity(entity);
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.set_mode((this.get_isEditMode() ? 1 : 0));
					this.propertyGrid.load(entity);
				}
			},
			beforeLoadEntity: function(entity) {
			},
			afterLoadEntity: function() {
				this.updateInterface();
				this.updateTitle();
			},
			loadByIdAndOpenDialog: function(entityId) {
				var self = this;
				this.loadById(entityId, ss.mkdel(this, function(response) {
					window.setTimeout(ss.mkdel(this, function() {
						if (!this.isPanel) {
							self.element.dialog().dialog('open');
						}
					}), 0);
				}));
			},
			onLoadingData: function(data) {
			},
			getLoadByIdOptions: function(id, callback) {
				return {};
			},
			getLoadByIdRequest: function(id) {
				var request = {};
				request.EntityId = id;
				return request;
			},
			reloadById: function() {
				this.loadById(ss.unbox(this.get_entityId()), null);
			},
			loadById: function(id, callback) {
				var baseOptions = {};
				baseOptions.service = this.getService() + '/Retrieve';
				baseOptions.blockUI = true;
				baseOptions.request = this.getLoadByIdRequest(id);
				var self = this;
				baseOptions.onSuccess = function(response) {
					self.loadResponse(response);
					if (!ss.staticEquals(callback, null)) {
						callback(response);
					}
				};
				baseOptions.onCleanup = function() {
					if (ss.isValue(self.validator)) {
						Q$Externals.validatorAbortHandler(self.validator);
					}
				};
				var thisOptions = this.getLoadByIdOptions(id, callback);
				var finalOptions = $.extend(baseOptions, thisOptions);
				this.loadByIdHandler(finalOptions, callback);
			},
			loadByIdHandler: function(options, callback) {
				Q.serviceCall(options);
			},
			$initLocalizationGrid: function() {
				var pgDiv = this.byId$1('PropertyGrid');
				if (pgDiv.length <= 0) {
					return;
				}
				var pgOptions = this.getPropertyGridOptions();
				this.$initLocalizationGridCommon(pgOptions);
			},
			$initLocalizationGridAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), pgDiv, $t1, pgOptions;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									pgDiv = this.byId$1('PropertyGrid');
									if (pgDiv.length <= 0) {
										$tcs.setResult(null);
										return;
									}
									$t1 = this.getPropertyGridOptionsAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									pgOptions = $t1.getAwaitedResult();
									this.$initLocalizationGridCommon(pgOptions);
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			$initLocalizationGridCommon: function(pgOptions) {
				var pgDiv = this.byId$1('PropertyGrid');
				var anyLocalizable = false;
				for (var $t1 = 0; $t1 < pgOptions.items.length; $t1++) {
					var item = pgOptions.items[$t1];
					if (item.localizable) {
						anyLocalizable = true;
					}
				}
				if (!anyLocalizable) {
					return;
				}
				var localGridDiv = $('<div/>').attr('id', this.get_idPrefix() + 'LocalizationGrid').hide().insertAfter(pgDiv);
				pgOptions.idPrefix = this.idPrefix + 'Localization_';
				var items = [];
				for (var $t2 = 0; $t2 < pgOptions.items.length; $t2++) {
					var item1 = pgOptions.items[$t2];
					if (item1.localizable) {
						var copy = $.extend({}, item1);
						copy.insertable = true;
						copy.updatable = true;
						copy.oneWay = false;
						copy.required = false;
						copy.localizable = false;
						copy.defaultValue = null;
						ss.add(items, copy);
					}
				}
				pgOptions.items = items;
				this.localizationGrid = new $Serenity_PropertyGrid(localGridDiv, pgOptions);
				localGridDiv.addClass('s-LocalizationGrid');
				var self = this;
				this.localizationSelect = $('<select/>').addClass('s-LocalizationSelect').appendTo(this.toolbar.get_element()).change(function(e) {
					self.$localizationSelectChange(e);
				});
				var $t3 = ss.getEnumerator($type.$getLanguages());
				try {
					while ($t3.moveNext()) {
						var k = $t3.current();
						Q.addOption(this.localizationSelect, k.item1.toString(), k.item2);
					}
				}
				finally {
					$t3.dispose();
				}
			},
			get_$isLocalizationMode: function() {
				return this.get_isEditMode() && !this.get_isCloneMode() && ss.isValue(this.localizationSelect) && !Q.isEmptyOrNull(this.localizationSelect.val());
			},
			$localizationSelectChange: function(e) {
				this.updateInterface();
				if (this.get_$isLocalizationMode()) {
					this.$loadLocalization();
				}
			},
			$loadLocalization: function() {
				var self = this;
				var opt = {};
				opt.service = this.getService() + '/RetrieveLocalization';
				opt.blockUI = true;
				opt.request = { EntityId: ss.unbox(this.get_entityId()), CultureId: parseInt(this.localizationSelect.val(), 10) };
				opt.onSuccess = function(response) {
					var valueByName = {};
					self.localizationGrid.load(self.get_entity());
					self.localizationGrid.enumerateItems(function(item, widget) {
						if (widget.get_element().is(':input')) {
							valueByName[item.name] = widget.get_element().val();
						}
					});
					self.localizationGrid.load(response.Entity);
					self.localizationGrid.enumerateItems(function(item1, widget1) {
						if (widget1.get_element().is(':input')) {
							var hint = valueByName[item1.name];
							if (ss.isValue(hint) && hint.length > 0) {
								widget1.get_element().attr('title', 'Türkçe Metin: ' + hint).attr('placeholder', hint);
							}
						}
					});
				};
				Q.serviceCall(opt);
			},
			$saveLocalization: function() {
				if (!this.validateForm()) {
					return;
				}
				var opt = {};
				opt.service = this.getService() + '/UpdateLocalization';
				opt.onSuccess = function(response) {
				};
				var entity = ss.createInstance(TEntity);
				this.localizationGrid.save(entity);
				var idField = this.getEntityIdField();
				if (ss.isValue(idField)) {
					entity[idField] = this.get_entityId();
				}
				opt.request = { CultureId: parseInt(this.localizationSelect.val(), 10), Entity: entity };
				Q.serviceCall(opt);
			},
			$initPropertyGrid: function() {
				var pgDiv = this.byId$1('PropertyGrid');
				if (pgDiv.length <= 0) {
					return;
				}
				var pgOptions = this.getPropertyGridOptions();
				this.propertyGrid = new $Serenity_PropertyGrid(pgDiv, pgOptions);
			},
			$initPropertyGridAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), pgDiv, $t1, pgOptions;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									pgDiv = this.byId$1('PropertyGrid');
									if (pgDiv.length <= 0) {
										$tcs.setResult(null);
										return;
									}
									$t1 = this.getPropertyGridOptionsAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									pgOptions = $t1.getAwaitedResult();
									this.propertyGrid = new $Serenity_PropertyGrid(pgDiv, pgOptions);
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			getPropertyItemsAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), formKey, $t1;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									formKey = this.getFormKey();
									$t1 = Q.getFormAsync(formKey);
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$tcs.setResult($t1.getAwaitedResult());
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			getPropertyItems: function() {
				var formKey = this.getFormKey();
				return Q.getForm(formKey);
			},
			getPropertyGridOptions: function() {
				var $t1 = $Serenity_PropertyGridOptions.$ctor();
				$t1.idPrefix = this.idPrefix;
				$t1.items = this.getPropertyItems();
				$t1.mode = 0;
				$t1.localTextPrefix = 'Forms.' + this.getFormKey() + '.';
				return $t1;
			},
			getPropertyGridOptionsAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, $t2;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = $Serenity_PropertyGridOptions.$ctor();
									$t1.idPrefix = this.idPrefix;
									$t2 = this.getPropertyItemsAsync();
									$state = 1;
									$t2.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$t1.items = $t2.getAwaitedResult();
									$t1.mode = 0;
									$tcs.setResult($t1);
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t3) {
						$tcs.setException(ss.Exception.wrap($t3));
					}
				});
				$sm();
				return $tcs.task;
			},
			validateBeforeSave: function() {
				return true;
			},
			getSaveOptions: function(callback) {
				var self = this;
				var opt = {};
				opt.service = this.getService() + '/' + (this.get_isEditMode() ? 'Update' : 'Create');
				opt.onSuccess = ss.mkdel(this, function(response) {
					self.onSaveSuccess(response);
					if (!ss.staticEquals(callback, null)) {
						callback(response);
					}
					var $t2 = (self.get_isEditMode() ? 'update' : 'create');
					var $t1 = opt.request;
					if (ss.isNullOrUndefined($t1)) {
						$t1 = new Object();
					}
					var $t5 = $t1.Entity;
					var $t4;
					if (self.get_isEditMode()) {
						$t4 = this.get_entityId();
					}
					else {
						var $t3 = response;
						if (ss.isNullOrUndefined($t3)) {
							$t3 = new Object();
						}
						$t4 = $t3.EntityId;
					}
					var dci = { type: $t2, entity: $t5, entityId: $t4 };
					self.element.triggerHandler('ondatachange', [dci]);
				});
				opt.onCleanup = function() {
					if (ss.isValue(self.validator)) {
						Q$Externals.validatorAbortHandler(self.validator);
					}
				};
				opt.request = this.getSaveRequest();
				return opt;
			},
			getSaveEntity: function() {
				var entity = ss.createInstance(TEntity);
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.save(entity);
				}
				if (this.get_isEditMode()) {
					var idField = this.getEntityIdField();
					if (ss.isValue(idField)) {
						entity[idField] = this.get_entityId();
					}
				}
				return entity;
			},
			getSaveRequest: function() {
				var entity = this.getSaveEntity();
				var req = {};
				req.Entity = entity;
				return req;
			},
			onSaveSuccess: function(response) {
			},
			save_SubmitHandler: function(callback) {
				var options = this.getSaveOptions(callback);
				this.saveHandler(options, callback);
			},
			save: function(callback) {
				var self = this;
				return $Serenity_ValidationHelper.submit(this.byId$1('Form'), function() {
					return self.validateBeforeSave();
				}, function() {
					self.save_SubmitHandler(callback);
				});
			},
			saveHandler: function(options, callback) {
				Q.serviceCall(options);
			},
			initToolbar: function() {
				ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).prototype.initToolbar.call(this);
				if (ss.isNullOrUndefined(this.toolbar)) {
					return;
				}
				this.saveAndCloseButton = this.toolbar.findButton('save-and-close-button');
				this.applyChangesButton = this.toolbar.findButton('apply-changes-button');
				this.deleteButton = this.toolbar.findButton('delete-button');
				this.undeleteButton = this.toolbar.findButton('undo-delete-button');
				this.cloneButton = this.toolbar.findButton('clone-button');
			},
			showSaveSuccessMessage: function(response) {
				Q.notifySuccess(Texts$Controls$EntityDialog.SaveSuccessMessage.get());
			},
			getToolbarButtons: function() {
				var list = [];
				var self = this;
				if (!this.isPanel) {
					ss.add(list, {
						title: Texts$Controls$EntityDialog.SaveButton.get(),
						cssClass: 'save-and-close-button',
						onClick: function() {
							self.save(function(response) {
								self.element.dialog().dialog('close');
							});
						}
					});
				}
				ss.add(list, { title: (this.isPanel ? Texts$Controls$EntityDialog.SaveButton : Q$LT.empty).get(), hint: (this.isPanel ? Texts$Controls$EntityDialog.SaveButton : Texts$Controls$EntityDialog.ApplyChangesButton).get(), cssClass: 'apply-changes-button', onClick: ss.mkdel(this, function() {
					if (self.get_$isLocalizationMode()) {
						self.$saveLocalization();
						return;
					}
					self.save(ss.mkdel(this, function(response1) {
						if (self.get_isEditMode()) {
							self.loadById(self.get_entityId(), null);
						}
						else {
							self.loadById(response1.EntityId, null);
						}
						this.showSaveSuccessMessage(response1);
					}));
				}) });
				if (!this.isPanel) {
					ss.add(list, {
						title: Texts$Controls$EntityDialog.DeleteButton.get(),
						cssClass: 'delete-button',
						onClick: function() {
							Q.confirm(Texts$Controls$EntityDialog.DeleteConfirmation.get(), function() {
								self.doDelete(function() {
									self.element.dialog().dialog('close');
								});
							});
						}
					});
					ss.add(list, {
						title: Texts$Controls$EntityDialog.UndeleteButton.get(),
						cssClass: 'undo-delete-button',
						onClick: function() {
							if (self.get_isDeleted()) {
								Q.confirm(Texts$Controls$EntityDialog.UndeleteConfirmation.get(), function() {
									self.undelete(function() {
										self.loadById(self.get_entityId(), null);
									});
								});
							}
						}
					});
					ss.add(list, { title: Texts$Controls$EntityDialog.CloneButton.get(), cssClass: 'clone-button', onClick: ss.mkdel(this, function() {
						if (!self.get_isEditMode()) {
							return;
						}
						var cloneEntity = this.getCloningEntity();
						var cloneDialog = new (ss.getInstanceType(this))(new Object());
						$Serenity_SubDialogHelper.bubbleDataChange($Serenity_SubDialogHelper.cascade(cloneDialog, this.element), this, true).loadEntityAndOpenDialog(cloneEntity);
					}) });
				}
				return list;
			},
			getCloningEntity: function() {
				var clone = ss.createInstance(TEntity);
				clone = $.extend(clone, this.get_entity());
				var idField = this.getEntityIdField();
				if (!Q.isEmptyOrNull(idField)) {
					delete clone[idField];
				}
				var isActiveField = this.getEntityIsActiveField();
				if (!Q.isEmptyOrNull(isActiveField)) {
					delete clone[isActiveField];
				}
				return clone;
			},
			updateInterface: function() {
				var isDeleted = this.get_isDeleted();
				var isLocalizationMode = this.get_$isLocalizationMode();
				if (ss.isValue(this.deleteButton)) {
					this.deleteButton.toggle(!isLocalizationMode && this.get_isEditMode() && !isDeleted);
				}
				if (ss.isValue(this.undeleteButton)) {
					this.undeleteButton.toggle(!isLocalizationMode && this.get_isEditMode() && isDeleted);
				}
				if (ss.isValue(this.saveAndCloseButton)) {
					this.saveAndCloseButton.toggle(!isLocalizationMode && !isDeleted);
					this.saveAndCloseButton.find('.button-inner').text((this.get_isNew() ? Texts$Controls$EntityDialog.SaveButton : Texts$Controls$EntityDialog.UpdateButton).get());
				}
				if (ss.isValue(this.applyChangesButton)) {
					this.applyChangesButton.toggle(isLocalizationMode || !isDeleted);
				}
				if (ss.isValue(this.cloneButton)) {
					this.cloneButton.toggle(false);
				}
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.get_element().toggle(!isLocalizationMode);
				}
				if (ss.isValue(this.localizationGrid)) {
					this.localizationGrid.get_element().toggle(isLocalizationMode);
				}
				if (ss.isValue(this.localizationSelect)) {
					this.localizationSelect.toggle(this.get_isEditMode() && !this.get_isCloneMode());
				}
				if (ss.isValue(this.tabs)) {
					Serenity.TabsExtensions.setDisabled(this.tabs, 'Log', this.get_isNewOrDeleted());
				}
			},
			getUndeleteOptions: function(callback) {
				return {};
			},
			undeleteHandler: function(options, callback) {
				Q.serviceCall(options);
			},
			undelete: function(callback) {
				var self = this;
				var baseOptions = {};
				baseOptions.service = this.getService() + '/Undelete';
				var request = {};
				request.EntityId = this.get_entityId();
				baseOptions.request = request;
				baseOptions.onSuccess = ss.mkdel(this, function(response) {
					if (!ss.staticEquals(callback, null)) {
						callback(response);
					}
					self.element.triggerHandler('ondatachange', [{ entityId: self.get_entityId(), entity: this.$entity, type: 'undelete' }]);
				});
				var thisOptions = this.getUndeleteOptions(callback);
				var finalOptions = $.extend(baseOptions, thisOptions);
				this.undeleteHandler(finalOptions, callback);
			}
		}, function() {
			return ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]);
		}, function() {
			return [$Serenity_IDialog];
		});
		$type.$ctor1.prototype = $type.$ctor2.prototype = $type.prototype;
		return $type;
	};
	$Serenity_EntityDialog$2.__typeName = 'Serenity.EntityDialog$2';
	ss.initGenericClass($Serenity_EntityDialog$2, $asm, 2);
	global.Serenity.EntityDialog$2 = $Serenity_EntityDialog$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EntityGrid
	var $Serenity_EntityGrid$1 = function(TEntity) {
		var $type = function(container) {
			ss.makeGenericType($Serenity_EntityGrid$2, [TEntity, Object]).call(this, container, null);
		};
		ss.registerGenericClassInstance($type, $Serenity_EntityGrid$1, [TEntity], {}, function() {
			return ss.makeGenericType($Serenity_EntityGrid$2, [TEntity, Object]);
		}, function() {
			return [$Serenity_IDataGrid];
		});
		return $type;
	};
	$Serenity_EntityGrid$1.__typeName = 'Serenity.EntityGrid$1';
	ss.initGenericClass($Serenity_EntityGrid$1, $asm, 1);
	global.Serenity.EntityGrid$1 = $Serenity_EntityGrid$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EntityGrid
	var $Serenity_EntityGrid$2 = function(TEntity, TOptions) {
		var $type = function(container, opt) {
			this.$entityType = null;
			this.$entityPlural = null;
			this.$entitySingular = null;
			this.$service = null;
			this.$entityDialogType = null;
			ss.makeGenericType($Serenity_DataGrid$2, [TEntity, TOptions]).call(this, container, opt);
		};
		ss.registerGenericClassInstance($type, $Serenity_EntityGrid$2, [TEntity, TOptions], {
			usePager: function() {
				return true;
			},
			createToolbarExtensions: function() {
				this.createIncludeDeletedButton();
				this.createQuickSearchInput();
			},
			getInitialTitle: function() {
				return this.getEntityPlural();
			},
			getLocalTextPrefix: function() {
				if (ss.isNullOrUndefined(this.localTextPrefix)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.LocalTextPrefixAttribute, true);
					if (attributes.length >= 1) {
						this.localTextPrefix = attributes[0].get_value();
					}
					else {
						this.localTextPrefix = this.getEntityType();
					}
					this.localTextPrefix = 'Db.' + this.localTextPrefix + '.';
				}
				return this.localTextPrefix;
			},
			getEntityType: function() {
				if (ss.isNullOrUndefined(this.$entityType)) {
					var typeAttributes = ss.getAttributes(ss.getInstanceType(this), Serenity.EntityTypeAttribute, true);
					if (typeAttributes.length === 1) {
						this.$entityType = typeAttributes[0].get_value();
						return this.$entityType;
					}
					// typeof(TEntity).Name'i kullanamayız, TEntity genelde Serializable ve Imported olduğundan dolayı tipi Object e karşılık geliyor!
					// remove global namespace
					var name = ss.getTypeFullName(ss.getInstanceType(this));
					var px = name.indexOf('.');
					if (px >= 0) {
						name = name.substring(px + 1);
					}
					if (ss.endsWithString(name, 'Grid')) {
						name = name.substr(0, name.length - 4);
					}
					else if (ss.endsWithString(name, 'SubGrid')) {
						name = name.substr(0, name.length - 7);
					}
					this.$entityType = name;
				}
				return this.$entityType;
			},
			getEntityPlural: function() {
				if (ss.isNullOrUndefined(this.$entityPlural)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), $System_ComponentModel_DisplayNameAttribute, true);
					if (attributes.length >= 1) {
						this.$entityPlural = attributes[0].get_displayName();
						this.$entityPlural = Q$LT.getDefault(this.$entityPlural, this.$entityPlural);
					}
					else {
						var $t1 = Q.tryGetText(this.getLocalTextPrefix() + 'EntityPlural');
						if (ss.isNullOrUndefined($t1)) {
							$t1 = this.getEntityType();
						}
						this.$entityPlural = $t1;
					}
				}
				return this.$entityPlural;
			},
			getEntitySingular: function() {
				if (ss.isNullOrUndefined(this.$entitySingular)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.ItemNameAttribute, true);
					if (attributes.length >= 1) {
						this.$entitySingular = attributes[0].get_value();
						this.$entitySingular = Q$LT.getDefault(this.$entitySingular, this.$entitySingular);
					}
					else {
						var $t1 = Q.tryGetText(this.getLocalTextPrefix() + 'EntitySingular');
						if (ss.isNullOrUndefined($t1)) {
							$t1 = this.getEntityType();
						}
						this.$entitySingular = $t1;
					}
				}
				return this.$entitySingular;
			},
			getAddButtonCaption: function() {
				return ss.formatString(Texts$Controls$EntityGrid.NewButton.get(), this.getEntitySingular());
			},
			getButtons: function() {
				var self = this;
				var buttons = [];
				ss.add(buttons, {
					title: this.getAddButtonCaption(),
					cssClass: 'add-button',
					onClick: function() {
						self.addButtonClick();
					}
				});
				ss.add(buttons, this.newRefreshButton());
				return buttons;
			},
			newRefreshButton: function() {
				var self = this;
				return {
					title: Texts$Controls$EntityGrid.RefreshButton.get(),
					cssClass: 'refresh-button',
					onClick: function() {
						self.refresh();
					}
				};
			},
			addButtonClick: function() {
				this.editItem(ss.createInstance(TEntity));
			},
			editItem: function(entityOrId) {
				var dialog = this.createEntityDialog();
				var scriptType = typeof(entityOrId);
				if (scriptType === 'string' || scriptType === 'number') {
					dialog.loadByIdAndOpenDialog(entityOrId);
				}
				else {
					var entity = entityOrId || ss.createInstance(TEntity);
					dialog.loadEntityAndOpenDialog(entity);
				}
			},
			getService: function() {
				if (ss.isNullOrUndefined(this.$service)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.ServiceAttribute, true);
					if (attributes.length >= 1) {
						this.$service = attributes[0].get_value();
					}
					else {
						this.$service = ss.replaceAllString(this.getEntityType(), String.fromCharCode(46), String.fromCharCode(47));
					}
				}
				return this.$service;
			},
			getViewOptions: function() {
				var opt = ss.makeGenericType($Serenity_DataGrid$2, [TEntity, TOptions]).prototype.getViewOptions.call(this);
				opt.url = Q.resolveUrl('~/Services/' + this.getService() + '/List');
				return opt;
			},
			getItemType: function() {
				return ss.replaceAllString(this.getEntityType(), String.fromCharCode(46), String.fromCharCode(45));
			},
			initEntityDialog: function(dialog) {
				var self = this;
				$Serenity_SubDialogHelper.bindToDataChange(dialog, this, function(e, dci) {
					self.subDialogDataChange();
				}, true);
			},
			createEntityDialog: function() {
				var dialogClass = this.getEntityDialogType();
				var dialog = ss.cast(new dialogClass(this.getEntityDialogOptions()), $Serenity_Widget);
				this.initEntityDialog(dialog);
				return dialog;
			},
			getEntityDialogOptions: function() {
				return {};
			},
			getEntityDialogType: function() {
				if (ss.isNullOrUndefined(this.$entityDialogType)) {
					var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.DialogTypeAttribute, true);
					if (attributes.length >= 1) {
						this.$entityDialogType = attributes[0].get_value();
					}
					else {
						var entityClass = this.getEntityType();
						var typeName = entityClass + 'Dialog';
						var dialogType = null;
						for (var $t1 = 0; $t1 < Q$Config.rootNamespaces.length; $t1++) {
							var ns = Q$Config.rootNamespaces[$t1];
							dialogType = ss.getType(ns + '.' + typeName);
							if (ss.isValue(dialogType)) {
								break;
							}
						}
						if (ss.isNullOrUndefined(dialogType)) {
							throw new ss.Exception(typeName + ' dialog class is not found!');
						}
						this.$entityDialogType = dialogType;
					}
				}
				return this.$entityDialogType;
			}
		}, function() {
			return ss.makeGenericType($Serenity_DataGrid$2, [TEntity, TOptions]);
		}, function() {
			return [$Serenity_IDataGrid];
		});
		return $type;
	};
	$Serenity_EntityGrid$2.__typeName = 'Serenity.EntityGrid$2';
	ss.initGenericClass($Serenity_EntityGrid$2, $asm, 2);
	global.Serenity.EntityGrid$2 = $Serenity_EntityGrid$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FilterField
	var $Serenity_FilterField = function() {
		this.name = null;
		this.title = null;
		this.handler = null;
	};
	$Serenity_FilterField.__typeName = 'Serenity.FilterField';
	global.Serenity.FilterField = $Serenity_FilterField;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FilterLine
	var $Serenity_FilterLine = function() {
		this.field = null;
		this.title = null;
		this.operator = null;
		this.isOr = false;
		this.leftParen = false;
		this.rightParen = false;
		this.validationError = null;
		this.value = null;
		this.value2 = null;
		this.values = null;
		this.displayText = null;
	};
	$Serenity_FilterLine.__typeName = 'Serenity.FilterLine';
	global.Serenity.FilterLine = $Serenity_FilterLine;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FilterPanel
	var $Serenity_FilterPanel = function(div, opt) {
		this.$rowsDiv = null;
		this.$fieldByName = null;
		this.$currentFilter = null;
		ss.makeGenericType($Serenity_TemplatedWidget$1, [$Serenity_FilterPanelOptions]).call(this, div, opt);
		this.$fieldByName = {};
		this.get_element().addClass('s-FilterPanel');
		this.$rowsDiv = this.byId$1('Rows');
		this.$initButtons();
		this.$initFieldByName();
		this.$bindSearchToEnterKey();
		this.$updateButtons();
	};
	$Serenity_FilterPanel.__typeName = 'Serenity.FilterPanel';
	global.Serenity.FilterPanel = $Serenity_FilterPanel;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FilterPanelOptions
	var $Serenity_FilterPanelOptions = function() {
		this.fields = null;
		this.filterChange = null;
	};
	$Serenity_FilterPanelOptions.__typeName = 'Serenity.FilterPanelOptions';
	global.Serenity.FilterPanelOptions = $Serenity_FilterPanelOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Flexify
	var $Serenity_Flexify = function(container, options) {
		this.$xDifference = 0;
		this.$yDifference = 0;
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, container, options);
		var self = this;
		Serenity.LazyLoadHelper.executeOnceWhenShown(container, function() {
			self.$storeInitialSize();
		});
	};
	$Serenity_Flexify.__typeName = 'Serenity.Flexify';
	global.Serenity.Flexify = $Serenity_Flexify;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FlexifyExtensions
	var $Serenity_FLX = function() {
	};
	$Serenity_FLX.__typeName = 'Serenity.FLX';
	$Serenity_FLX.flexHeightOnly = function(element, flexY) {
		return element.addClass('flexify').data('flex-y', flexY).data('flex-x', 0);
	};
	$Serenity_FLX.flexWidthOnly = function(element, flexX) {
		return element.addClass('flexify').data('flex-x', flexX).data('flex-y', 0);
	};
	$Serenity_FLX.flexWidthHeight = function(element, flexX, flexY) {
		return element.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
	};
	$Serenity_FLX.flexXFactor = function(element, flexX) {
		return element.data('flex-x', flexX);
	};
	$Serenity_FLX.flexYFactor = function(element, flexY) {
		return element.data('flex-y', flexY);
	};
	global.Serenity.FLX = $Serenity_FLX;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.GridSelectAllButtonHelper
	var $Serenity_GridSelectAllButtonHelper = function() {
	};
	$Serenity_GridSelectAllButtonHelper.__typeName = 'Serenity.GridSelectAllButtonHelper';
	$Serenity_GridSelectAllButtonHelper.update = function(TItem) {
		return function(grid, getSelected) {
			var grd = grid;
			var toolbar = grd.get_element().children('.s-Toolbar');
			if (toolbar.length === 0) {
				return;
			}
			var btn = $Serenity_WX.getWidget($Serenity_Toolbar).call(null, toolbar).findButton('select-all-button');
			var items = grd.get_view().getItems();
			btn.toggleClass('checked', items.length > 0 && !items.some(function(x) {
				return !getSelected(x);
			}));
		};
	};
	$Serenity_GridSelectAllButtonHelper.define = function(TItem) {
		return function(getGrid, getId, getSelected, setSelected, text, onClick) {
			return {
				title: ss.coalesce(text, 'Tümünü Seç'),
				cssClass: 'select-all-button',
				onClick: function() {
					var grid = getGrid();
					var view = grid.get_view();
					var btn = $Serenity_WX.getWidget($Serenity_Toolbar).call(null, grid.get_element().children('.s-Toolbar')).findButton('select-all-button');
					var makeSelected = !btn.hasClass('checked');
					view.beginUpdate();
					try {
						var $t1 = view.getItems();
						for (var $t2 = 0; $t2 < $t1.length; $t2++) {
							var item = $t1[$t2];
							setSelected(item, makeSelected);
							view.updateItem(getId(item), item);
						}
						if (!ss.staticEquals(onClick, null)) {
							onClick();
						}
					}
					finally {
						view.endUpdate();
					}
					btn.toggleClass('checked', makeSelected);
				}
			};
		};
	};
	global.Serenity.GridSelectAllButtonHelper = $Serenity_GridSelectAllButtonHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.GridUtils
	var $Serenity_GridUtils = function() {
	};
	$Serenity_GridUtils.__typeName = 'Serenity.GridUtils';
	$Serenity_GridUtils.addToggleButton = function(toolDiv, cssClass, callback, hint, initial) {
		var div = $('<div><a href="#"></a></div>').addClass('s-ToggleButton').addClass(cssClass).prependTo(toolDiv);
		div.children('a').click(function(e) {
			e.preventDefault();
			div.toggleClass('pressed');
			var pressed = div.hasClass('pressed');
			if (!ss.staticEquals(callback, null)) {
				callback(pressed);
			}
		}).attr('title', ss.coalesce(hint, ''));
		if (initial) {
			div.addClass('pressed');
		}
	};
	$Serenity_GridUtils.addIncludeDeletedToggle = function(toolDiv, view, hint, initial) {
		var includeDeleted = false;
		var oldSubmit = view.onSubmit;
		view.onSubmit = function(v) {
			v.params.IncludeDeleted = includeDeleted;
			if (!ss.staticEquals(oldSubmit, null)) {
				return oldSubmit(v);
			}
			return true;
		};
		var $t1 = hint;
		if (ss.isNullOrUndefined($t1)) {
			$t1 = Texts$Controls$EntityGrid.IncludeDeletedToggle.get();
		}
		$Serenity_GridUtils.addToggleButton(toolDiv, 's-IncludeDeletedToggle', function(pressed) {
			includeDeleted = pressed;
			view.seekToPage = 1;
			view.populate();
		}, $t1, initial);
		toolDiv.bind('remove', function() {
			view.onSubmit = null;
			oldSubmit = null;
		});
	};
	$Serenity_GridUtils.addQuickSearchInput = function(TEntity) {
		return function(toolDiv, view, fields) {
			var oldSubmit = view.onSubmit;
			var searchText = '';
			var searchField = '';
			view.onSubmit = function(v) {
				if (ss.isValue(searchText) && searchText.length > 0) {
					v.params.ContainsText = searchText;
				}
				else {
					delete v.params['ContainsText'];
				}
				if (ss.isValue(searchField) && searchField.length > 0) {
					v.params.ContainsField = searchField;
				}
				else {
					delete v.params['ContainsField'];
				}
				if (!ss.staticEquals(oldSubmit, null)) {
					return oldSubmit(v);
				}
				return true;
			};
			$Serenity_GridUtils.addQuickSearchInputCustom(toolDiv, function(field, query) {
				searchText = query;
				searchField = field;
				view.seekToPage = 1;
				view.populate();
			}, fields);
		};
	};
	$Serenity_GridUtils.addQuickSearchInputCustom = function(container, onSearch, fields) {
		var input = $('<div><input type="text"/></div>').addClass('s-QuickSearchBar').prependTo(container);
		if (ss.isValue(fields) && fields.length > 0) {
			input.addClass('has-quick-search-fields');
		}
		input = input.children();
		var $t1 = $Serenity_QuickSearchInputOptions.$ctor();
		$t1.fields = fields;
		$t1.onSearch = onSearch;
		new $Serenity_QuickSearchInput(input, $t1);
	};
	$Serenity_GridUtils.makeOrderable = function(grid, handleMove) {
		var moveRowsPlugin = new Slick.RowMoveManager({ cancelEditOnDrag: true });
		moveRowsPlugin.onBeforeMoveRows.subscribe(function(e, data) {
			for (var i = 0; !!(i < data.rows.length); i++) {
				if (!!(ss.referenceEquals(data.rows[i], data.insertBefore) || ss.referenceEquals(data.rows[i], data.insertBefore - 1))) {
					e.stopPropagation();
					return false;
				}
			}
			return true;
		});
		moveRowsPlugin.onMoveRows.subscribe(function(e1, data1) {
			var rows = ss.cast(data1.rows, Array);
			var insertBefore = ss.unbox(ss.cast(data1.insertBefore, ss.Int32));
			handleMove(rows, insertBefore);
			try {
				grid.setSelectedRows([]);
			}
			catch ($t1) {
			}
		});
		grid.registerPlugin(moveRowsPlugin);
	};
	$Serenity_GridUtils.makeOrderableWithUpdateRequest = function(TItem, TOptions) {
		return function(grid, getId, getDisplayOrder, service, getUpdateRequest) {
			$Serenity_GridUtils.makeOrderable(grid.get_slickGrid(), function(rows, insertBefore) {
				if (rows.length === 0) {
					return;
				}
				var order;
				var index = insertBefore;
				if (index < 0) {
					order = 1;
				}
				else if (insertBefore >= grid.get_view().rows.length) {
					order = ss.coalesce(getDisplayOrder(grid.get_view().rows[grid.get_view().rows.length - 1]), 0);
					if (order === 0) {
						order = insertBefore + 1;
					}
					else {
						order = order + 1;
					}
				}
				else {
					order = ss.coalesce(getDisplayOrder(grid.get_view().rows[insertBefore]), 0);
					if (order === 0) {
						order = insertBefore + 1;
					}
				}
				var i = 0;
				var next = null;
				next = function() {
					Q.serviceCall({
						service: service,
						request: getUpdateRequest(getId(grid.get_view().rows[rows[i]]), order++),
						onSuccess: function(response) {
							i++;
							if (i < rows.length) {
								next();
							}
							else {
								grid.get_view().populate();
							}
						}
					});
				};
				next();
			});
		};
	};
	global.Serenity.GridUtils = $Serenity_GridUtils;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.HtmlContentEditor
	var $Serenity_HtmlContentEditor = function(textArea, opt) {
		this.$instanceReady = false;
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_HtmlContentEditorOptions]).call(this, textArea, opt);
		$Serenity_HtmlContentEditor.$includeCKEditor();
		var id = textArea.attr('id');
		if (Q.isTrimmedEmpty(id)) {
			textArea.attr('id', this.uniqueName);
			id = this.uniqueName;
		}
		if (ss.isValue(this.options.cols)) {
			textArea.attr('cols', ss.unbox(this.options.cols).toString());
		}
		if (ss.isValue(this.options.rows)) {
			textArea.attr('rows', ss.unbox(this.options.rows).toString());
		}
		var self = this;
		var config = this.getConfig();
		CKEDITOR.replace(id, config);
		$Serenity_WX.addValidationRule(this, this.uniqueName, function(e) {
			if (e.hasClass('required')) {
				var value = Q.trimToNull(self.get_value());
				if (ss.isNullOrUndefined(value)) {
					return Q.text('Validation.Required');
				}
			}
			return null;
		});
	};
	$Serenity_HtmlContentEditor.__typeName = 'Serenity.HtmlContentEditor';
	$Serenity_HtmlContentEditor.$includeCKEditor = function() {
		var window2 = window.window;
		if (!!ss.isValue(window2.CKEDITOR)) {
			return;
		}
		var script = $('CKEditorScript');
		if (script.length > 0) {
			return;
		}
		$('<script/>').attr('type', 'text/javascript').attr('id', 'CKEditorScript').attr('src', Q.resolveUrl('~/Scripts/CKEditor/ckeditor.js')).appendTo(window.document.head);
	};
	global.Serenity.HtmlContentEditor = $Serenity_HtmlContentEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.HtmlContentEditorOptions
	var $Serenity_HtmlContentEditorOptions = function() {
	};
	$Serenity_HtmlContentEditorOptions.__typeName = 'Serenity.HtmlContentEditorOptions';
	$Serenity_HtmlContentEditorOptions.createInstance = function() {
		return $Serenity_HtmlContentEditorOptions.$ctor();
	};
	$Serenity_HtmlContentEditorOptions.$ctor = function() {
		var $this = {};
		$this.cols = null;
		$this.rows = null;
		$this.cols = 80;
		$this.rows = 6;
		return $this;
	};
	global.Serenity.HtmlContentEditorOptions = $Serenity_HtmlContentEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.HtmlReportContentEditor
	var $Serenity_HtmlReportContentEditor = function(textArea, opt) {
		$Serenity_HtmlContentEditor.call(this, textArea, opt);
	};
	$Serenity_HtmlReportContentEditor.__typeName = 'Serenity.HtmlReportContentEditor';
	global.Serenity.HtmlReportContentEditor = $Serenity_HtmlReportContentEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IBooleanValue
	var $Serenity_IBooleanValue = function() {
	};
	$Serenity_IBooleanValue.__typeName = 'Serenity.IBooleanValue';
	global.Serenity.IBooleanValue = $Serenity_IBooleanValue;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IDataGrid
	var $Serenity_IDataGrid = function() {
	};
	$Serenity_IDataGrid.__typeName = 'Serenity.IDataGrid';
	global.Serenity.IDataGrid = $Serenity_IDataGrid;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IDialog
	var $Serenity_IDialog = function() {
	};
	$Serenity_IDialog.__typeName = 'Serenity.IDialog';
	global.Serenity.IDialog = $Serenity_IDialog;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IDoubleValue
	var $Serenity_IDoubleValue = function() {
	};
	$Serenity_IDoubleValue.__typeName = 'Serenity.IDoubleValue';
	global.Serenity.IDoubleValue = $Serenity_IDoubleValue;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IFilterHandler
	var $Serenity_IFilterHandler = function() {
	};
	$Serenity_IFilterHandler.__typeName = 'Serenity.IFilterHandler';
	global.Serenity.IFilterHandler = $Serenity_IFilterHandler;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IGetEditValue
	var $Serenity_IGetEditValue = function() {
	};
	$Serenity_IGetEditValue.__typeName = 'Serenity.IGetEditValue';
	global.Serenity.IGetEditValue = $Serenity_IGetEditValue;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ImageUploadEditor
	var $Serenity_ImageUploadEditor = function(div, opt) {
		this.entity = null;
		this.toolbar = null;
		this.fileSymbols = null;
		this.uploadInput = null;
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_ImageUploadEditorOptions]).call(this, div, opt);
		div.addClass('s-ImageUploadEditor');
		if (Q.isEmptyOrNull(this.options.originalNameProperty)) {
			div.addClass('hide-original-name');
		}
		var self = this;
		this.toolbar = new $Serenity_Toolbar($('<div/>').appendTo(this.get_element()), { buttons: this.getToolButtons() });
		var progress = $('<div><div></div></div>').addClass('upload-progress').prependTo(this.toolbar.get_element());
		var addFileButton = this.toolbar.findButton('add-file-button');
		this.uploadInput = $Serenity_UploadHelper.addUploadInput({ container: addFileButton, inputName: this.uniqueName, progress: progress, fileDone: ss.mkdel(this, function(response, name, data) {
			if (!$Serenity_UploadHelper.checkImageConstraints(response, this.options)) {
				return;
			}
			var $t1 = new $Serenity_UploadedFile();
			$t1.set_originalName(name);
			$t1.set_filename(response.TemporaryFile);
			var newEntity = $t1;
			self.entity = newEntity;
			self.populate();
			self.updateInterface();
		}) });
		this.fileSymbols = $('<ul/>').appendTo(this.element);
		this.updateInterface();
	};
	$Serenity_ImageUploadEditor.__typeName = 'Serenity.ImageUploadEditor';
	global.Serenity.ImageUploadEditor = $Serenity_ImageUploadEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ImageUploadEditorOptions
	var $Serenity_ImageUploadEditorOptions = function() {
	};
	$Serenity_ImageUploadEditorOptions.__typeName = 'Serenity.ImageUploadEditorOptions';
	$Serenity_ImageUploadEditorOptions.createInstance = function() {
		return $Serenity_ImageUploadEditorOptions.$ctor();
	};
	$Serenity_ImageUploadEditorOptions.$ctor = function() {
		var $this = {};
		$this.minWidth = 0;
		$this.maxWidth = 0;
		$this.minHeight = 0;
		$this.maxHeight = 0;
		$this.minSize = 0;
		$this.maxSize = 0;
		$this.originalNameProperty = null;
		return $this;
	};
	global.Serenity.ImageUploadEditorOptions = $Serenity_ImageUploadEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IntegerEditor
	var $Serenity_IntegerEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_IntegerEditorOptions]).call(this, input, opt);
		input.addClass('decimalQ');
		var numericOptions = $.extend($Serenity_DecimalEditor.defaultAutoNumericOptions(), { vMin: this.options.minValue, vMax: this.options.maxValue, aSep: null });
		input.autoNumeric(numericOptions);
	};
	$Serenity_IntegerEditor.__typeName = 'Serenity.IntegerEditor';
	global.Serenity.IntegerEditor = $Serenity_IntegerEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IntegerEditorOptions
	var $Serenity_IntegerEditorOptions = function() {
	};
	$Serenity_IntegerEditorOptions.__typeName = 'Serenity.IntegerEditorOptions';
	$Serenity_IntegerEditorOptions.createInstance = function() {
		return $Serenity_IntegerEditorOptions.$ctor();
	};
	$Serenity_IntegerEditorOptions.$ctor = function() {
		var $this = {};
		$this.minValue = 0;
		$this.maxValue = 0;
		$this.minValue = 0;
		$this.maxValue = 2147483647;
		return $this;
	};
	global.Serenity.IntegerEditorOptions = $Serenity_IntegerEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IReadOnly
	var $Serenity_IReadOnly = function() {
	};
	$Serenity_IReadOnly.__typeName = 'Serenity.IReadOnly';
	global.Serenity.IReadOnly = $Serenity_IReadOnly;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ISetEditValue
	var $Serenity_ISetEditValue = function() {
	};
	$Serenity_ISetEditValue.__typeName = 'Serenity.ISetEditValue';
	global.Serenity.ISetEditValue = $Serenity_ISetEditValue;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IStringValue
	var $Serenity_IStringValue = function() {
	};
	$Serenity_IStringValue.__typeName = 'Serenity.IStringValue';
	global.Serenity.IStringValue = $Serenity_IStringValue;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IValidateRequired
	var $Serenity_IValidateRequired = function() {
	};
	$Serenity_IValidateRequired.__typeName = 'Serenity.IValidateRequired';
	global.Serenity.IValidateRequired = $Serenity_IValidateRequired;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.JsRender
	var $Serenity_JsRender = function() {
	};
	$Serenity_JsRender.__typeName = 'Serenity.JsRender';
	$Serenity_JsRender.render = function(markup, data) {
		if (ss.isNullOrUndefined(markup) || markup.indexOf('{{') < 0) {
			return markup;
		}
		if (!!(ss.isNullOrUndefined($.templates) || ss.isNullOrUndefined($.views))) {
			throw new ss.Exception('Please make sure that jsrender.js is included in the page!');
		}
		var $t1 = data;
		if (ss.isNullOrUndefined($t1)) {
			$t1 = {};
		}
		data = $t1;
		var template = $.templates(markup);
		$.views.converters({
			text: function(s) {
				return Q.text(s);
			}
		}, template);
		return ss.cast(template.render(data), String);
	};
	global.Serenity.JsRender = $Serenity_JsRender;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LookupEditor
	var $Serenity_LookupEditor = function(hidden, opt) {
		ss.makeGenericType($Serenity_LookupEditorBase$2, [$Serenity_LookupEditorOptions, Object]).call(this, hidden, opt);
	};
	$Serenity_LookupEditor.__typeName = 'Serenity.LookupEditor';
	global.Serenity.LookupEditor = $Serenity_LookupEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LookupEditorBase
	var $Serenity_LookupEditorBase$1 = function(TItem) {
		var $type = function(hidden) {
			ss.makeGenericType($Serenity_LookupEditorBase$2, [Object, TItem]).call(this, hidden, null);
		};
		ss.registerGenericClassInstance($type, $Serenity_LookupEditorBase$1, [TItem], {}, function() {
			return ss.makeGenericType($Serenity_LookupEditorBase$2, [Object, TItem]);
		}, function() {
			return [$Serenity_IStringValue];
		});
		return $type;
	};
	$Serenity_LookupEditorBase$1.__typeName = 'Serenity.LookupEditorBase$1';
	ss.initGenericClass($Serenity_LookupEditorBase$1, $asm, 1);
	global.Serenity.LookupEditorBase$1 = $Serenity_LookupEditorBase$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LookupEditorBase
	var $Serenity_LookupEditorBase$2 = function(TOptions, TItem) {
		var $type = function(hidden, opt) {
			ss.makeGenericType($Serenity_Select2Editor$2, [TOptions, TItem]).call(this, hidden, opt);
			var self = this;
			if (!this.isAsyncWidget()) {
				this.updateItems();
				Q$ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, function() {
					self.updateItems();
				});
			}
		};
		ss.registerGenericClassInstance($type, $Serenity_LookupEditorBase$2, [TOptions, TItem], {
			initializeAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), self, $t1;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									self = this;
									$t1 = this.updateItemsAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$t1.getAwaitedResult();
									Q$ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, function() {
										self.updateItemsAsync().start();
									});
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			destroy: function() {
				Q$ScriptData.unbindFromChange(this.uniqueName);
				this.element.select2('destroy');
				$Serenity_Widget.prototype.destroy.call(this);
			},
			getLookupKey: function() {
				var key = ss.getTypeFullName(ss.getInstanceType(this));
				var idx = key.indexOf('.');
				if (idx >= 0) {
					key = key.substring(idx + 1);
				}
				if (ss.endsWithString(key, 'Editor')) {
					key = key.substr(0, key.length - 6);
				}
				return key;
			},
			getLookup: function() {
				return Q.getLookup(this.getLookupKey());
			},
			getLookupAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = Q.getLookupAsync(this.getLookupKey());
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$tcs.setResult($t1.getAwaitedResult());
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			getItems: function(lookup) {
				return lookup.get_items();
			},
			getItemText: function(item, lookup) {
				var textValue = (!ss.staticEquals(lookup.get_textFormatter(), null) ? lookup.get_textFormatter()(item) : item[lookup.get_textField()]);
				return (ss.isNullOrUndefined(textValue) ? '' : textValue.toString());
			},
			getItemDisabled: function(item, lookup) {
				return false;
			},
			updateItems: function() {
				var lookup = this.getLookup();
				this.clearItems();
				var items = this.getItems(lookup);
				var $t1 = ss.getEnumerator(items);
				try {
					while ($t1.moveNext()) {
						var item = $t1.current();
						var text = this.getItemText(ss.cast(item, TItem), lookup);
						var disabled = this.getItemDisabled(ss.cast(item, TItem), lookup);
						var idValue = item[lookup.get_idField()];
						var id = (ss.isNullOrUndefined(idValue) ? '' : idValue.toString());
						ss.add(this.items, { id: id, text: ss.cast(text, String), source: item, disabled: !!disabled });
					}
				}
				finally {
					$t1.dispose();
				}
			},
			updateItemsAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, lookup, items, $t2, item, text, disabled, idValue, id;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = this.getLookupAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									lookup = $t1.getAwaitedResult();
									this.clearItems();
									items = this.getItems(lookup);
									$t2 = ss.getEnumerator(items);
									try {
										while ($t2.moveNext()) {
											item = $t2.current();
											text = this.getItemText(ss.cast(item, TItem), lookup);
											disabled = this.getItemDisabled(ss.cast(item, TItem), lookup);
											idValue = item[lookup.get_idField()];
											id = (ss.isNullOrUndefined(idValue) ? '' : idValue.toString());
											ss.add(this.items, { id: id, text: ss.cast(text, String), source: item, disabled: !!disabled });
										}
									}
									finally {
										$t2.dispose();
									}
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t3) {
						$tcs.setException(ss.Exception.wrap($t3));
					}
				});
				$sm();
				return $tcs.task;
			}
		}, function() {
			return ss.makeGenericType($Serenity_Select2Editor$2, [TOptions, TItem]);
		}, function() {
			return [$Serenity_IStringValue];
		});
		ss.setMetadata($type, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
		return $type;
	};
	$Serenity_LookupEditorBase$2.__typeName = 'Serenity.LookupEditorBase$2';
	ss.initGenericClass($Serenity_LookupEditorBase$2, $asm, 2);
	global.Serenity.LookupEditorBase$2 = $Serenity_LookupEditorBase$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LookupEditorOptions
	var $Serenity_LookupEditorOptions = function() {
	};
	$Serenity_LookupEditorOptions.__typeName = 'Serenity.LookupEditorOptions';
	$Serenity_LookupEditorOptions.createInstance = function() {
		return $Serenity_LookupEditorOptions.$ctor();
	};
	$Serenity_LookupEditorOptions.$ctor = function() {
		var $this = {};
		$this.lookupKey = null;
		return $this;
	};
	global.Serenity.LookupEditorOptions = $Serenity_LookupEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.MaskedEditor
	var $Serenity_MaskedEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_MaskedEditorOptions]).call(this, input, opt);
		input.mask(this.options.mask, { placeholder: this.options.placeholder });
	};
	$Serenity_MaskedEditor.__typeName = 'Serenity.MaskedEditor';
	global.Serenity.MaskedEditor = $Serenity_MaskedEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.MaskedEditorOptions
	var $Serenity_MaskedEditorOptions = function() {
	};
	$Serenity_MaskedEditorOptions.__typeName = 'Serenity.MaskedEditorOptions';
	$Serenity_MaskedEditorOptions.createInstance = function() {
		return $Serenity_MaskedEditorOptions.$ctor();
	};
	$Serenity_MaskedEditorOptions.$ctor = function() {
		var $this = {};
		$this.mask = null;
		$this.placeholder = null;
		$this.mask = '';
		$this.placeholder = '_';
		return $this;
	};
	global.Serenity.MaskedEditorOptions = $Serenity_MaskedEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PanelAttribute
	var $Serenity_PanelAttribute = function() {
	};
	$Serenity_PanelAttribute.__typeName = 'Serenity.PanelAttribute';
	global.Serenity.PanelAttribute = $Serenity_PanelAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PasswordEditor
	var $Serenity_PasswordEditor = function(input) {
		$Serenity_StringEditor.call(this, input);
		input.attr('type', 'password');
	};
	$Serenity_PasswordEditor.__typeName = 'Serenity.PasswordEditor';
	global.Serenity.PasswordEditor = $Serenity_PasswordEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PersonNameEditor
	var $Serenity_PersonNameEditor = function(input) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, input, new Object());
		$Serenity_WX.addValidationRule(this, this.uniqueName, ss.mkdel(this, function() {
			if (!(new RegExp('^[ A-Za-zıİğĞöÖüÜşŞÇç]+$')).test(Q.trimToEmpty(this.get_value()))) {
				return 'Lütfen sadece harflerden oluşan bir metin giriniz!';
			}
			return null;
		}));
	};
	$Serenity_PersonNameEditor.__typeName = 'Serenity.PersonNameEditor';
	global.Serenity.PersonNameEditor = $Serenity_PersonNameEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PhoneEditor
	var $Serenity_PhoneEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_PhoneEditorOptions]).call(this, input, opt);
		var self = this;
		$Serenity_WX.addValidationRule(this, this.uniqueName, ss.mkdel(this, function(e) {
			var value = Q.trimToNull(this.get_value());
			if (ss.isNullOrUndefined(value)) {
				return null;
			}
			return $Serenity_PhoneEditor.$validate(value, this.options.multiple, this.options.internal, this.options.mobile);
		}));
		var hint = (this.options.internal ? "Dahili telefon numarası '456, 8930, 12345' formatlarında" : (this.options.mobile ? "Cep telefonu numarası '(533) 342 01 89' formatında" : "Telefon numarası '(216) 432 10 98' formatında"));
		if (this.options.multiple) {
			hint = ss.replaceAllString(hint, 'numarası', 'numaraları') + ' ve birden fazlaysa virgülle ayrılarak ';
		}
		hint += ' girilmelidir.';
		input.attr('title', hint);
		input.bind('change', ss.mkdel(this, function(e1) {
			if (!$Serenity_WX.hasOriginalEvent(e1)) {
				return;
			}
			this.formatValue();
		}));
		input.bind('blur', ss.mkdel(this, function(e2) {
			if (this.element.hasClass('valid')) {
				this.formatValue();
			}
		}));
	};
	$Serenity_PhoneEditor.__typeName = 'Serenity.PhoneEditor';
	$Serenity_PhoneEditor.$formatMulti = function(phone, format) {
		var phones = ss.replaceAllString(phone, String.fromCharCode(59), String.fromCharCode(44)).split(String.fromCharCode(44));
		var result = '';
		for (var $t1 = 0; $t1 < phones.length; $t1++) {
			var x = phones[$t1];
			var s = Q.trimToNull(x);
			if (ss.isNullOrUndefined(s)) {
				continue;
			}
			if (result.length > 0) {
				result += ', ';
			}
			result += format(s);
		}
		return result;
	};
	$Serenity_PhoneEditor.$formatPhoneTurkey = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidPhoneTurkey(phone)) {
			return phone;
		}
		phone = ss.replaceAllString(ss.replaceAllString(ss.replaceAllString(phone, ' ', ''), '(', ''), ')', '');
		if (ss.startsWithString(phone, '0')) {
			phone = phone.substring(1);
		}
		phone = '(' + phone.substr(0, 3) + ') ' + phone.substr(3, 3) + ' ' + phone.substr(6, 2) + ' ' + phone.substr(8, 2);
		return phone;
	};
	$Serenity_PhoneEditor.$formatPhoneTurkeyMulti = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidPhoneTurkeyMulti(phone)) {
			return phone;
		}
		return $Serenity_PhoneEditor.$formatMulti(phone, $Serenity_PhoneEditor.$formatPhoneTurkey);
	};
	$Serenity_PhoneEditor.$formatMobileTurkey = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidMobileTurkey(phone)) {
			return phone;
		}
		return $Serenity_PhoneEditor.$formatPhoneTurkey(phone);
	};
	$Serenity_PhoneEditor.$formatMobileTurkeyMulti = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidMobileTurkeyMulti(phone)) {
			return phone;
		}
		return $Serenity_PhoneEditor.$formatPhoneTurkeyMulti(phone);
	};
	$Serenity_PhoneEditor.$formatPhoneInternal = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidPhoneInternal(phone)) {
			return phone;
		}
		return phone.trim();
	};
	$Serenity_PhoneEditor.$formatPhoneInternalMulti = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidPhoneInternalMulti(phone)) {
			return phone;
		}
		return $Serenity_PhoneEditor.$formatMulti(phone, $Serenity_PhoneEditor.$formatPhoneInternal);
	};
	$Serenity_PhoneEditor.$isValidPhoneTurkeyMulti = function(phone) {
		return $Serenity_PhoneEditor.$isValidMulti(phone, $Serenity_PhoneEditor.$isValidPhoneTurkey);
	};
	$Serenity_PhoneEditor.$isValidMobileTurkeyMulti = function(phone) {
		return $Serenity_PhoneEditor.$isValidMulti(phone, $Serenity_PhoneEditor.$isValidMobileTurkey);
	};
	$Serenity_PhoneEditor.$isValidPhoneInternalMulti = function(phone) {
		return $Serenity_PhoneEditor.$isValidMulti(phone, $Serenity_PhoneEditor.$isValidPhoneInternal);
	};
	$Serenity_PhoneEditor.$validate = function(phone, isMultiple, isInternal, isMobile) {
		var validateFunc;
		if (isInternal) {
			validateFunc = $Serenity_PhoneEditor.$isValidPhoneInternal;
		}
		else if (isMobile) {
			validateFunc = $Serenity_PhoneEditor.$isValidMobileTurkey;
		}
		else {
			validateFunc = $Serenity_PhoneEditor.$isValidPhoneTurkey;
		}
		var valid = (isMultiple ? $Serenity_PhoneEditor.$isValidMulti(phone, validateFunc) : validateFunc(phone));
		if (valid) {
			return null;
		}
		if (isMultiple) {
			if (isInternal) {
				return "Dahili telefon numaraları '4567' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";
			}
			if (isMobile) {
				return "Telefon numaraları '(533) 342 01 89' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";
			}
			return "Telefon numaları '(216) 432 10 98' formatında ve birden fazlaysa virgülle ayrılarak girilmelidir!";
		}
		else {
			if (isInternal) {
				return "Dahili telefon numarası '4567' formatında girilmelidir!";
			}
			if (isMobile) {
				return "Telefon numarası '(533) 342 01 89' formatında girilmelidir!";
			}
			return "Telefon numarası '(216) 432 10 98' formatında girilmelidir!";
		}
	};
	$Serenity_PhoneEditor.$isValidPhoneTurkey = function(phone) {
		if (Q.isEmptyOrNull(phone)) {
			return false;
		}
		phone = ss.replaceAllString(phone, ' ', '');
		if (phone.length < 10) {
			return false;
		}
		if (ss.startsWithString(phone, '0')) {
			phone = phone.substring(1);
		}
		if (ss.startsWithString(phone, '(') && phone.charCodeAt(4) === 41) {
			phone = phone.substr(1, 3) + phone.substring(5);
		}
		if (phone.length !== 10) {
			return false;
		}
		if (ss.startsWithString(phone, '0')) {
			return false;
		}
		for (var i = 0; i < phone.length; i++) {
			var c = phone.charCodeAt(i);
			if (c < 48 || c > 57) {
				return false;
			}
		}
		return true;
	};
	$Serenity_PhoneEditor.$isValidMobileTurkey = function(phone) {
		if (!$Serenity_PhoneEditor.$isValidPhoneTurkey(phone)) {
			return false;
		}
		phone = ss.trimStartString(phone);
		phone = ss.replaceAllString(phone, ' ', '');
		var lookIndex = 0;
		if (ss.startsWithString(phone, '0')) {
			lookIndex++;
		}
		if (phone.charCodeAt(lookIndex) === 53 || phone.charCodeAt(lookIndex) === 40 && phone.charCodeAt(lookIndex + 1) === 53) {
			return true;
		}
		return false;
	};
	$Serenity_PhoneEditor.$isValidPhoneInternal = function(phone) {
		if (Q.isEmptyOrNull(phone)) {
			return false;
		}
		phone = phone.trim();
		if (phone.length < 2 || phone.length > 5) {
			return false;
		}
		for (var i = 0; i < phone.length; i++) {
			var c = phone.charCodeAt(i);
			if (c < 48 || c > 57) {
				return false;
			}
		}
		return true;
	};
	$Serenity_PhoneEditor.$isValidMulti = function(phone, check) {
		if (Q.isEmptyOrNull(phone)) {
			return false;
		}
		var phones = ss.replaceAllString(phone, String.fromCharCode(59), String.fromCharCode(44)).split(String.fromCharCode(44));
		var anyValid = false;
		for (var $t1 = 0; $t1 < phones.length; $t1++) {
			var x = phones[$t1];
			var s = Q.trimToNull(x);
			if (ss.isNullOrUndefined(s)) {
				continue;
			}
			if (!check(s)) {
				return false;
			}
			anyValid = true;
		}
		if (!anyValid) {
			return false;
		}
		return true;
	};
	global.Serenity.PhoneEditor = $Serenity_PhoneEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PhoneEditorOptions
	var $Serenity_PhoneEditorOptions = function() {
	};
	$Serenity_PhoneEditorOptions.__typeName = 'Serenity.PhoneEditorOptions';
	$Serenity_PhoneEditorOptions.createInstance = function() {
		return $Serenity_PhoneEditorOptions.$ctor();
	};
	$Serenity_PhoneEditorOptions.$ctor = function() {
		var $this = {};
		$this.multiple = false;
		$this.internal = false;
		$this.mobile = false;
		return $this;
	};
	global.Serenity.PhoneEditorOptions = $Serenity_PhoneEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PopupMenuButton
	var $Serenity_PopupMenuButton = function(div, opt) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, div, opt);
		var self = this;
		div.addClass('s-PopupMenuButton');
		div.click(ss.mkdel(this, function(e) {
			e.preventDefault();
			e.stopPropagation();
			if (!ss.staticEquals(this.options.onPopup, null)) {
				this.options.onPopup();
			}
			var menu = self.options.menu;
			menu.show().position({ my: ss.coalesce(this.options.positionMy, 'left top'), at: ss.coalesce(this.options.positionAt, 'left bottom'), of: self.element });
			var uq = self.uniqueName;
			$(document).one('click.' + uq, function(x) {
				menu.hide();
			});
		}));
		self.options.menu.hide().appendTo(document.body).addClass('s-PopupMenu').menu();
	};
	$Serenity_PopupMenuButton.__typeName = 'Serenity.PopupMenuButton';
	global.Serenity.PopupMenuButton = $Serenity_PopupMenuButton;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PopupToolButton
	var $Serenity_PopupToolButton = function(div, opt) {
		$Serenity_PopupMenuButton.call(this, div, opt);
		div.addClass('s-PopupToolButton');
		$('<b/>').appendTo(div.children('.button-outer').children('span'));
	};
	$Serenity_PopupToolButton.__typeName = 'Serenity.PopupToolButton';
	global.Serenity.PopupToolButton = $Serenity_PopupToolButton;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PrefixedContext
	var $Serenity_PrefixedContext = function(idPrefix) {
		this.$idPrefix = null;
		Serenity.ScriptContext.call(this);
		this.$idPrefix = idPrefix;
	};
	$Serenity_PrefixedContext.__typeName = 'Serenity.PrefixedContext';
	global.Serenity.PrefixedContext = $Serenity_PrefixedContext;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyDialog
	var $Serenity_PropertyDialog$1 = function(TEntity) {
		var $type = function() {
			ss.makeGenericType($Serenity_PropertyDialog$2, [TEntity, Object]).$ctor1.call(this, null);
		};
		ss.registerGenericClassInstance($type, $Serenity_PropertyDialog$1, [TEntity], {}, function() {
			return ss.makeGenericType($Serenity_PropertyDialog$2, [TEntity, Object]);
		}, function() {
			return [$Serenity_IDialog];
		});
		return $type;
	};
	$Serenity_PropertyDialog$1.__typeName = 'Serenity.PropertyDialog$1';
	ss.initGenericClass($Serenity_PropertyDialog$1, $asm, 1);
	global.Serenity.PropertyDialog$1 = $Serenity_PropertyDialog$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyDialog
	var $Serenity_PropertyDialog$2 = function(TEntity, TOptions) {
		var $type = function() {
			$type.$ctor2.call(this, Q.newBodyDiv(), null);
		};
		$type.$ctor1 = function(opt) {
			$type.$ctor2.call(this, Q.newBodyDiv(), opt);
		};
		$type.$ctor2 = function(div, opt) {
			this.$entity = null;
			this.$entityId = null;
			this.propertyGrid = null;
			ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).$ctor2.call(this, div, opt);
			if (!this.isAsyncWidget()) {
				this.$initPropertyGrid();
				this.loadInitialEntity();
			}
		};
		ss.registerGenericClassInstance($type, $Serenity_PropertyDialog$2, [TEntity, TOptions], {
			initializeAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, $t2;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]).prototype.initializeAsync.call(this);
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$t1.getAwaitedResult();
									$t2 = this.$initPropertyGridAsync();
									$state = 2;
									$t2.continueWith($sm);
									return;
								}
								case 2: {
									$state = -1;
									$t2.getAwaitedResult();
									this.loadInitialEntity();
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t3) {
						$tcs.setException(ss.Exception.wrap($t3));
					}
				});
				$sm();
				return $tcs.task;
			},
			loadInitialEntity: function() {
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.load(new Object());
				}
			},
			getDialogOptions: function() {
				var opt = ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).prototype.getDialogOptions.call(this);
				opt.buttons = this.getDialogButtons();
				opt.width = 400;
				opt.title = this.getDialogTitle();
				return opt;
			},
			getDialogTitle: function() {
				return '';
			},
			okClick: function() {
				if (!this.validateBeforeSave()) {
					return;
				}
				this.okClickValidated();
			},
			okClickValidated: function() {
				this.dialogClose();
			},
			cancelClick: function() {
				this.dialogClose();
			},
			getDialogButtons: function() {
				var $t1 = [];
				ss.add($t1, { text: 'Tamam', click: ss.mkdel(this, this.okClick) });
				null;
				ss.add($t1, { text: 'İptal', click: ss.mkdel(this, this.cancelClick) });
				null;
				return $t1;
			},
			destroy: function() {
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.destroy();
					this.propertyGrid = null;
				}
				if (ss.isValue(this.validator)) {
					this.byId$1('Form').remove();
					this.validator = null;
				}
				ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).prototype.destroy.call(this);
			},
			get_entity: function() {
				return this.$entity;
			},
			set_entity: function(value) {
				this.$entity = value || ss.createInstance(TEntity);
			},
			get_entityId: function() {
				return this.$entityId;
			},
			set_entityId: function(value) {
				this.$entityId = value;
			},
			updateTitle: function() {
			},
			onDialogOpen: function() {
				ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]).prototype.onDialogOpen.call(this);
			},
			getTemplateName: function() {
				var templateName = ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]).prototype.getTemplateName.call(this);
				if (!Q.canLoadScriptData('Template.' + templateName) && Q.canLoadScriptData('Template.PropertyDialog')) {
					return 'PropertyDialog';
				}
				return templateName;
			},
			$initPropertyGrid: function() {
				var pgDiv = this.byId$1('PropertyGrid');
				if (pgDiv.length <= 0) {
					return;
				}
				var pgOptions = this.getPropertyGridOptions();
				this.propertyGrid = new $Serenity_PropertyGrid(pgDiv, pgOptions);
			},
			$initPropertyGridAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), pgDiv, $t1, pgOptions;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									pgDiv = this.byId$1('PropertyGrid');
									if (pgDiv.length <= 0) {
										$tcs.setResult(null);
										return;
									}
									$t1 = this.getPropertyGridOptionsAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									pgOptions = $t1.getAwaitedResult();
									this.propertyGrid = new $Serenity_PropertyGrid(pgDiv, pgOptions);
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			getFormKey: function() {
				var attributes = ss.getAttributes(ss.getInstanceType(this), Serenity.FormKeyAttribute, true);
				if (attributes.length >= 1) {
					return attributes[0].get_value();
				}
				else {
					var name = ss.getTypeFullName(ss.getInstanceType(this));
					var px = name.indexOf('.');
					if (px >= 0) {
						name = name.substring(px + 1);
					}
					if (ss.endsWithString(name, 'Dialog')) {
						name = name.substr(0, name.length - 6);
					}
					else if (ss.endsWithString(name, 'Panel')) {
						name = name.substr(0, name.length - 5);
					}
					return name;
				}
			},
			getPropertyItems: function() {
				var formKey = this.getFormKey();
				return Q.getForm(formKey);
			},
			getPropertyItemsAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), formKey, $t1;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									formKey = this.getFormKey();
									$t1 = Q.getFormAsync(formKey);
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$tcs.setResult($t1.getAwaitedResult());
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			getPropertyGridOptions: function() {
				var $t1 = $Serenity_PropertyGridOptions.$ctor();
				$t1.idPrefix = this.idPrefix;
				$t1.items = this.getPropertyItems();
				$t1.mode = 0;
				$t1.useCategories = false;
				$t1.localTextPrefix = 'Forms.' + this.getFormKey() + '.';
				return $t1;
			},
			getPropertyGridOptionsAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, $t2;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = $Serenity_PropertyGridOptions.$ctor();
									$t1.idPrefix = this.idPrefix;
									$t2 = this.getPropertyItemsAsync();
									$state = 1;
									$t2.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									$t1.items = $t2.getAwaitedResult();
									$t1.mode = 0;
									$t1.useCategories = false;
									$tcs.setResult($t1);
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t3) {
						$tcs.setException(ss.Exception.wrap($t3));
					}
				});
				$sm();
				return $tcs.task;
			},
			validateBeforeSave: function() {
				return this.validator.form();
			},
			getSaveEntity: function() {
				var entity = ss.createInstance(TEntity);
				if (ss.isValue(this.propertyGrid)) {
					this.propertyGrid.save(entity);
				}
				return entity;
			}
		}, function() {
			return ss.makeGenericType($Serenity_TemplatedDialog$1, [TOptions]);
		}, function() {
			return [$Serenity_IDialog];
		});
		$type.$ctor1.prototype = $type.$ctor2.prototype = $type.prototype;
		return $type;
	};
	$Serenity_PropertyDialog$2.__typeName = 'Serenity.PropertyDialog$2';
	ss.initGenericClass($Serenity_PropertyDialog$2, $asm, 2);
	global.Serenity.PropertyDialog$2 = $Serenity_PropertyDialog$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyEditorHelper
	var $Serenity_PropertyEditorHelper = function() {
	};
	$Serenity_PropertyEditorHelper.__typeName = 'Serenity.PropertyEditorHelper';
	$Serenity_PropertyEditorHelper.getPropertyItemsFor = function(type) {
		if (ss.isNullOrUndefined(type)) {
			throw new ss.Exception('type');
		}
		var list = [];
		var $t1 = ss.getMembers(type, 31, 20);
		for (var $t2 = 0; $t2 < $t1.length; $t2++) {
			var member = $t1[$t2];
			var pi = {};
			if (member.type !== 16 && member.type !== 4) {
				continue;
			}
			var hiddenAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_HiddenAttribute);
			});
			if (hiddenAttribute.length > 0) {
				continue;
			}
			var displayNameAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $System_ComponentModel_DisplayNameAttribute);
			});
			if (displayNameAttribute.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla başlık belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			var hintAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_HintAttribute);
			});
			if (hintAttribute.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla ipucu belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			var placeholderAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_PlaceholderAttribute);
			});
			if (placeholderAttribute.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla placeholder belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			var memberType = ((member.type === 16) ? ss.cast(member, ss.isValue(member) && member.type === 16).returnType : ss.cast(member, ss.isValue(member) && member.type === 4).returnType);
			if (member.type === 16) {
				var p = ss.cast(member, ss.isValue(member) && member.type === 16);
				if (ss.isNullOrUndefined(p.fname)) {
					continue;
				}
				pi.name = p.fname;
			}
			else if (member.type === 4) {
				var f = ss.cast(member, ss.isValue(member) && member.type === 4);
				pi.name = f.sname;
			}
			else {
				continue;
			}
			var categoryAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_CategoryAttribute);
			});
			if (categoryAttribute.length === 1) {
				pi.category = ss.cast(categoryAttribute[0], $Serenity_ComponentModel_CategoryAttribute).get_category();
			}
			else if (categoryAttribute.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla kategori belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			var cssClassAttr = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_CssClassAttribute);
			});
			if (cssClassAttr.length === 1) {
				pi.cssClass = ss.cast(cssClassAttr[0], $Serenity_ComponentModel_CssClassAttribute).get_cssClass();
			}
			else if (cssClassAttr.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla css class belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			if ((member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_OneWayAttribute);
			}).length > 0) {
				pi.oneWay = true;
			}
			if ((member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_ReadOnlyAttribute);
			}).length > 0) {
				pi.readOnly = true;
			}
			if (displayNameAttribute.length > 0) {
				pi.title = ss.cast(displayNameAttribute[0], $System_ComponentModel_DisplayNameAttribute).get_displayName();
			}
			if (hintAttribute.length > 0) {
				pi.hint = ss.cast(hintAttribute[0], $Serenity_ComponentModel_HintAttribute).get_hint();
			}
			if (placeholderAttribute.length > 0) {
				pi.placeholder = ss.cast(placeholderAttribute[0], $Serenity_ComponentModel_PlaceholderAttribute).get_value();
			}
			if (ss.isNullOrUndefined(pi.title)) {
				pi.title = pi.name;
			}
			var defaultValueAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_DefaultValueAttribute);
			});
			if (defaultValueAttribute.length === 1) {
				pi.defaultValue = ss.cast(defaultValueAttribute[0], $Serenity_ComponentModel_DefaultValueAttribute).get_value();
			}
			var insertableAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_InsertableAttribute);
			});
			if (insertableAttribute.length > 0) {
				pi.insertable = insertableAttribute[0].get_value();
			}
			else {
				pi.insertable = true;
			}
			var updatableAttribute = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_UpdatableAttribute);
			});
			if (updatableAttribute.length > 0) {
				pi.updatable = updatableAttribute[0].get_value();
			}
			else {
				pi.updatable = true;
			}
			var typeAttrArray = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_EditorTypeAttribute);
			});
			if (typeAttrArray.length > 1) {
				throw new ss.Exception(ss.formatString('{0}.{1} için birden fazla editör tipi belirlenmiş!', ss.getTypeName(type), pi.name));
			}
			var nullableType = memberType;
			//Nullable.GetUnderlyingType(memberType);
			var enumType = null;
			if (ss.isEnum(memberType)) {
				enumType = memberType;
			}
			else if (ss.isValue(nullableType) && ss.isEnum(nullableType)) {
				enumType = nullableType;
			}
			if (typeAttrArray.length === 0) {
				if (ss.isValue(enumType)) {
					pi.editorType = 'Select';
				}
				else if (ss.referenceEquals(memberType, ss.JsDate)) {
					pi.editorType = 'Date';
				}
				else if (ss.referenceEquals(memberType, Boolean)) {
					pi.editorType = 'Boolean';
				}
				else if (ss.referenceEquals(memberType, Number) || ss.referenceEquals(memberType, Number)) {
					pi.editorType = 'Decimal';
				}
				else if (ss.referenceEquals(memberType, ss.Int32)) {
					pi.editorType = 'Integer';
				}
				else {
					pi.editorType = 'String';
				}
			}
			else {
				var et = ss.cast(typeAttrArray[0], $Serenity_ComponentModel_EditorTypeAttribute);
				pi.editorType = et.editorType;
				et.setParams(pi.editorParams);
			}
			if (ss.isValue(enumType)) {
				//List<string[]> options = new List<string[]>();
				//foreach (var val in Enum.GetValues(enumType))
				//{
				//    string key = Enum.GetName(enumType, val);
				//    string text = ValueFormatters.FormatEnum(enumType, val);
				//    options.Add(new string[] { key, text });
				//}
				//if (memberType == typeof(DayOfWeek)) // şimdilik tek bir özel durum
				//{
				//    options.Add(options[0]);
				//    options.RemoveAt(0);
				//}
				//pi.EditorParams["items"] = options.ToArray();
			}
			var reqAttr = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_RequiredAttribute);
			});
			if (reqAttr.length > 0) {
				pi.required = reqAttr[0].get_isRequired();
			}
			var maxLengthAttr = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_MaxLengthAttribute);
			});
			if (maxLengthAttr.length > 0) {
				pi.maxLength = maxLengthAttr.get_maxLength();
				pi.editorParams['maxLength'] = pi.maxLength;
			}
			var $t3 = (member.attr || []).filter(function(a) {
				return ss.isInstanceOfType(a, $Serenity_ComponentModel_EditorOptionAttribute);
			});
			for (var $t4 = 0; $t4 < $t3.length; $t4++) {
				var param = $t3[$t4];
				var key = param.get_key();
				if (ss.isValue(key) && key.length >= 1) {
					key = key.substr(0, 1).toLowerCase() + key.substring(1);
				}
				pi.editorParams[key] = param.get_value();
			}
			ss.add(list, pi);
		}
		return list;
	};
	global.Serenity.PropertyEditorHelper = $Serenity_PropertyEditorHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyGrid
	var $Serenity_PropertyGrid = function(div, opt) {
		this.$editors = null;
		this.$items = null;
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_PropertyGridOptions]).call(this, div, opt);
		if (!ss.isValue(opt.mode)) {
			opt.mode = 0;
		}
		this.$items = this.options.items || [];
		div.addClass('s-PropertyGrid');
		this.$editors = [];
		var categoryIndexes = {};
		var categoriesDiv = div;
		if (this.options.useCategories) {
			var linkContainer = $('<div/>').addClass('category-links');
			categoryIndexes = this.$createCategoryLinks(linkContainer, this.$items);
			if (ss.getKeyCount(categoryIndexes) > 1) {
				linkContainer.appendTo(div);
			}
			else {
				linkContainer.find('a.category-link').unbind('click', $Serenity_PropertyGrid.$categoryLinkClick).remove();
			}
			categoriesDiv = $('<div/>').addClass('categories').appendTo(div);
		}
		var fieldContainer = categoriesDiv;
		var priorCategory = null;
		for (var i = 0; i < this.$items.length; i++) {
			var item = this.$items[i];
			if (this.options.useCategories && !ss.referenceEquals(priorCategory, item.category)) {
				var categoryDiv = this.$createCategoryDiv(categoriesDiv, categoryIndexes, item.category);
				if (ss.isNullOrUndefined(priorCategory)) {
					categoryDiv.addClass('first-category');
				}
				priorCategory = item.category;
				fieldContainer = categoryDiv;
			}
			var editor = this.$createField(fieldContainer, item);
			this.$editors[i] = editor;
		}
		this.$updateReadOnly();
	};
	$Serenity_PropertyGrid.__typeName = 'Serenity.PropertyGrid';
	$Serenity_PropertyGrid.$getEditorType = function(editorTypeKey) {
		if (ss.isNullOrUndefined(editorTypeKey)) {
			throw new ss.ArgumentNullException('editorTypeKey');
		}
		if (!ss.keyExists($Serenity_PropertyGrid.$knownEditorTypes, editorTypeKey)) {
			var editorType = null;
			for (var $t1 = 0; $t1 < Q$Config.rootNamespaces.length; $t1++) {
				var ns = Q$Config.rootNamespaces[$t1];
				var withoutSuffix = ss.getType(ns + '.' + editorTypeKey);
				var withSuffix = ss.getType(ns + '.' + editorTypeKey + 'Editor');
				editorType = withoutSuffix || withSuffix;
				if (ss.isValue(withoutSuffix) && ss.isValue(withSuffix) && !ss.isAssignableFrom($Serenity_Widget, withoutSuffix) && ss.isAssignableFrom($Serenity_Widget, withSuffix)) {
					editorType = withSuffix;
				}
				if (ss.isValue(editorType)) {
					break;
				}
			}
			if (ss.isValue(editorType)) {
				if (!ss.isAssignableFrom($Serenity_Widget, editorType)) {
					throw new ss.Exception(ss.formatString('{0} editor type is not a subclass of Widget', ss.getTypeFullName(editorType)));
				}
				$Serenity_PropertyGrid.$knownEditorTypes[editorTypeKey] = editorType;
				return editorType;
			}
			else {
				throw new ss.Exception(ss.formatString("PropertyGrid: Can't find {0} editor type!", editorTypeKey));
			}
		}
		else {
			return $Serenity_PropertyGrid.$knownEditorTypes[editorTypeKey];
		}
	};
	$Serenity_PropertyGrid.$categoryLinkClick = function(e) {
		e.preventDefault();
		var title = $('a[name=' + e.target.getAttribute('href').toString().substr(1) + ']');
		var animate = function() {
			title.fadeTo(100, 0.5, function() {
				title.fadeTo(100, 1, function() {
				});
			});
		};
		var intoView = title.closest('.category');
		if (intoView.closest(':scrollable(both)').length === 0) {
			animate();
		}
		else {
			intoView.scrollintoview({ duration: 'fast', direction: 'y', complete: animate });
		}
	};
	$Serenity_PropertyGrid.$setMaxLength = function(widget, maxLength) {
		if (widget.get_element().is(':input')) {
			if (maxLength > 0) {
				widget.get_element().attr('maxlength', maxLength.toString());
			}
			else {
				widget.get_element().removeAttr('maxlength');
			}
		}
	};
	$Serenity_PropertyGrid.setRequired = function(widget, isRequired) {
		var req = ss.safeCast(widget, $Serenity_IValidateRequired);
		if (ss.isValue(req)) {
			req.set_required(isRequired);
		}
		else if (widget.get_element().is(':input')) {
			widget.get_element().toggleClass('required', !!isRequired);
		}
		var gridField = $Serenity_WX.getGridField(widget);
		var hasSupItem = Enumerable.from(gridField.find('sup').get()).any();
		if (isRequired && !hasSupItem) {
			$('<sup>*</sup>').attr('title', 'Bu alan zorunludur').prependTo(gridField.find('.caption')[0]);
		}
		else if (!isRequired && hasSupItem) {
			$(gridField.find('sup')[0]).remove();
		}
	};
	$Serenity_PropertyGrid.setReadOnly = function(widget, isReadOnly) {
		var readOnly = ss.safeCast(widget, $Serenity_IReadOnly);
		if (ss.isValue(readOnly)) {
			readOnly.set_readOnly(isReadOnly);
		}
		else if (widget.get_element().is(':input')) {
			$Serenity_PropertyGrid.setReadOnly$1(widget.get_element(), isReadOnly);
		}
	};
	$Serenity_PropertyGrid.setReadOnly$1 = function(elements, isReadOnly) {
		elements.each(function(index, el) {
			var elx = $(el);
			var type = elx.attr('type');
			if (elx.is('select') || type === 'radio' || type === 'checkbox') {
				if (isReadOnly) {
					elx.addClass('readonly').attr('disabled', 'disabled');
				}
				else {
					elx.removeClass('readonly').removeAttr('disabled');
				}
			}
			else if (isReadOnly) {
				elx.addClass('readonly').attr('readonly', 'readonly');
			}
			else {
				elx.removeClass('readonly').removeAttr('readonly');
			}
			return true;
		});
		return elements;
	};
	global.Serenity.PropertyGrid = $Serenity_PropertyGrid;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyGridMode
	var $Serenity_PropertyGridMode = function() {
	};
	$Serenity_PropertyGridMode.__typeName = 'Serenity.PropertyGridMode';
	global.Serenity.PropertyGridMode = $Serenity_PropertyGridMode;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PropertyGridOptions
	var $Serenity_PropertyGridOptions = function() {
	};
	$Serenity_PropertyGridOptions.__typeName = 'Serenity.PropertyGridOptions';
	$Serenity_PropertyGridOptions.createInstance = function() {
		return $Serenity_PropertyGridOptions.$ctor();
	};
	$Serenity_PropertyGridOptions.$ctor = function() {
		var $this = {};
		$this.idPrefix = null;
		$this.items = null;
		$this.useCategories = false;
		$this.categoryOrder = null;
		$this.defaultCategory = null;
		$this.localTextPrefix = null;
		$this.mode = 0;
		$this.useCategories = true;
		$this.defaultCategory = Texts$Controls$PropertyGrid.DefaultCategory.get();
		return $this;
	};
	global.Serenity.PropertyGridOptions = $Serenity_PropertyGridOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.QuickSearchInput
	var $Serenity_QuickSearchInput = function(input, opt) {
		this.$lastValue = null;
		this.$timer = 0;
		this.$field = null;
		this.$fieldChanged = false;
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_QuickSearchInputOptions]).call(this, input, opt);
		input.attr('title', Texts$Controls$QuickSearch.Hint.get()).attr('placeholder', Texts$Controls$QuickSearch.Placeholder.get());
		this.$lastValue = Q.trim(ss.coalesce(input.val(), ''));
		var self = this;
		this.element.bind('keyup.' + this.uniqueName, function() {
			self.checkIfValueChanged();
		});
		this.element.bind('change.' + this.uniqueName, function() {
			self.checkIfValueChanged();
		});
		$('<span><i></i></span>').addClass('quick-search-icon').insertBefore(input);
		if (ss.isValue(this.options.fields) && this.options.fields.length > 0) {
			var a = $('<a/>').addClass('quick-search-field').attr('title', Texts$Controls$QuickSearch.FieldSelection.get()).insertBefore(input);
			var menu = $('<ul></ul>').css('width', '120px');
			for (var $t1 = 0; $t1 < this.options.fields.length; $t1++) {
				var item = this.options.fields[$t1];
				var field = { $: item };
				$('<li><a/></li>').appendTo(menu).children().attr('href', '#').text(ss.coalesce(item.title, '')).click(ss.mkdel({ field: field, $this: this }, function(e) {
					e.preventDefault();
					this.$this.$fieldChanged = !ss.referenceEquals(self.$field, this.field.$);
					self.$field = this.field.$;
					this.$this.$updateInputPlaceHolder();
					this.$this.checkIfValueChanged();
				}));
			}
			new $Serenity_PopupMenuButton(a, { positionMy: 'right top', positionAt: 'right bottom', menu: menu });
			this.$field = this.options.fields[0];
			this.$updateInputPlaceHolder();
		}
	};
	$Serenity_QuickSearchInput.__typeName = 'Serenity.QuickSearchInput';
	global.Serenity.QuickSearchInput = $Serenity_QuickSearchInput;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.QuickSearchInputOptions
	var $Serenity_QuickSearchInputOptions = function() {
	};
	$Serenity_QuickSearchInputOptions.__typeName = 'Serenity.QuickSearchInputOptions';
	$Serenity_QuickSearchInputOptions.createInstance = function() {
		return $Serenity_QuickSearchInputOptions.$ctor();
	};
	$Serenity_QuickSearchInputOptions.$ctor = function() {
		var $this = {};
		$this.typeDelay = 0;
		$this.loadingParentClass = null;
		$this.filteredParentClass = null;
		$this.onSearch = null;
		$this.fields = null;
		$this.typeDelay = 250;
		$this.loadingParentClass = 's-QuickSearchLoading';
		$this.filteredParentClass = 's-QuickSearchFiltered';
		return $this;
	};
	global.Serenity.QuickSearchInputOptions = $Serenity_QuickSearchInputOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ReflectionUtils
	var $Serenity_ReflectionUtils = function() {
	};
	$Serenity_ReflectionUtils.__typeName = 'Serenity.ReflectionUtils';
	$Serenity_ReflectionUtils.getPropertyValue = function(o, property) {
		var d = o;
		var getter = d['get_' + property];
		if (!!!(typeof(getter) === 'undefined')) {
			return getter.apply(o);
		}
		var camelCase = $Serenity_ReflectionUtils.makeCamelCase(property);
		getter = d['get_' + camelCase];
		if (!!!(typeof(getter) === 'undefined')) {
			return getter.apply(o);
		}
		return d[camelCase];
	};
	$Serenity_ReflectionUtils.setPropertyValue = function(o, property, value) {
		var d = o;
		var setter = d['set_' + property];
		if (!!!(typeof(setter) === 'undefined')) {
			setter.apply(o, [value]);
			return;
		}
		var camelCase = $Serenity_ReflectionUtils.makeCamelCase(property);
		setter = d['set_' + camelCase];
		if (!!!(typeof(setter) === 'undefined')) {
			setter.apply(o, [value]);
			return;
		}
		d[camelCase] = value;
	};
	$Serenity_ReflectionUtils.makeCamelCase = function(s) {
		if (ss.isNullOrEmptyString(s)) {
			return s;
		}
		if (s === 'ID') {
			return 'id';
		}
		var hasNonUppercase = false;
		var numUppercaseChars = 0;
		for (var index = 0; index < s.length; index++) {
			if (s.charCodeAt(index) >= 65 && s.charCodeAt(index) <= 90) {
				numUppercaseChars++;
			}
			else {
				hasNonUppercase = true;
				break;
			}
		}
		if (!hasNonUppercase && s.length !== 1 || numUppercaseChars === 0) {
			return s;
		}
		else if (numUppercaseChars > 1) {
			return s.substr(0, numUppercaseChars - 1).toLowerCase() + s.substr(numUppercaseChars - 1);
		}
		else if (s.length === 1) {
			return s.toLowerCase();
		}
		else {
			return s.substr(0, 1).toLowerCase() + s.substr(1);
		}
	};
	global.Serenity.ReflectionUtils = $Serenity_ReflectionUtils;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Select2AjaxEditor
	var $Serenity_Select2AjaxEditor$2 = function(TOptions, TItem) {
		var $type = function(hidden, opt) {
			this.pageSize = 50;
			ss.makeGenericType($Serenity_Widget$1, [TOptions]).call(this, hidden, opt);
			var emptyItemText = this.emptyItemText();
			if (ss.isValue(emptyItemText)) {
				hidden.attr('placeholder', emptyItemText);
			}
			hidden.select2(this.getSelect2Options());
			hidden.attr('type', 'text');
			// jquery validate to work
			hidden.bind('change.' + this.uniqueName, function(e, x) {
				if (!!($Serenity_WX.hasOriginalEvent(e) || !x)) {
					if (ss.isValue($Serenity_ValidationHelper.getValidator(hidden))) {
						hidden.valid();
					}
				}
			});
		};
		ss.registerGenericClassInstance($type, $Serenity_Select2AjaxEditor$2, [TOptions, TItem], {
			emptyItemText: function() {
				return ss.coalesce(this.element.attr('placeholder'), '--seçiniz--');
			},
			getService: function() {
				throw new ss.NotImplementedException();
			},
			query: function(request, callback) {
				var options = {
					service: this.getService() + '/List',
					request: request,
					onSuccess: function(response) {
						callback(response);
					}
				};
				this.executeQuery(options);
			},
			executeQuery: function(options) {
				Q.serviceCall(options);
			},
			queryByKey: function(key, callback) {
				var options = {
					service: this.getService() + '/Retrieve',
					request: { EntityId: key },
					onSuccess: function(response) {
						callback(response.Entity);
					}
				};
				this.executeQueryByKey(options);
			},
			executeQueryByKey: function(options) {
				Q.serviceCall(options);
			},
			getItemKey: null,
			getItemText: null,
			getTypeDelay: function() {
				return 250;
			},
			getSelect2Options: function() {
				var emptyItemText = this.emptyItemText();
				var queryTimeout = 0;
				return { minimumResultsForSearch: 10, placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null), allowClear: ss.isValue(emptyItemText), query: ss.mkdel(this, function(query) {
					var request = { ContainsText: Q.trimToNull(query.term), Skip: (query.page - 1) * this.pageSize, Take: this.pageSize };
					if (queryTimeout !== 0) {
						window.clearTimeout(queryTimeout);
					}
					queryTimeout = window.setTimeout(ss.mkdel(this, function() {
						this.query(request, ss.mkdel(this, function(response) {
							query.callback({ results: Enumerable.from(response.Entities).select(ss.mkdel(this, function(x) {
								return { id: this.getItemKey(x), text: this.getItemText(x), source: x };
							})).toArray(), more: response.TotalCount >= query.page * this.pageSize });
						}));
					}), this.getTypeDelay());
				}), initSelection: ss.mkdel(this, function(element, callback) {
					var val = element.val();
					if (Q.isEmptyOrNull(val)) {
						callback(null);
						return;
					}
					this.queryByKey(val, ss.mkdel(this, function(result) {
						callback((ss.isNullOrUndefined(result) ? null : { id: this.getItemKey(result), text: this.getItemText(result), source: result }));
					}));
				}) };
			},
			addInplaceCreate: function(title) {
				var self = this;
				$('<a><b/></a>').addClass('inplace-button inplace-create').attr('title', title).insertAfter(this.element).click(function(e) {
					self.inplaceCreateClick(e);
				});
				this.get_select2Container().add(this.element).addClass('has-inplace-button');
			},
			inplaceCreateClick: function(e) {
			},
			get_select2Container: function() {
				return this.element.prevAll('.select2-container');
			},
			get_value: function() {
				return ss.safeCast(this.element.select2('val'), String);
			},
			set_value: function(value) {
				if (!ss.referenceEquals(value, this.get_value())) {
					this.element.select2('val', value).triggerHandler('change', [true]);
				}
			}
		}, function() {
			return ss.makeGenericType($Serenity_Widget$1, [TOptions]);
		}, function() {
			return [$Serenity_IStringValue];
		});
		ss.setMetadata($type, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
		return $type;
	};
	$Serenity_Select2AjaxEditor$2.__typeName = 'Serenity.Select2AjaxEditor$2';
	ss.initGenericClass($Serenity_Select2AjaxEditor$2, $asm, 2);
	global.Serenity.Select2AjaxEditor$2 = $Serenity_Select2AjaxEditor$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Select2Editor
	var $Serenity_Select2Editor$2 = function(TOptions, TItem) {
		var $type = function(hidden, opt) {
			this.items = null;
			this.pageSize = 100;
			ss.makeGenericType($Serenity_Widget$1, [TOptions]).call(this, hidden, opt);
			this.items = [];
			var emptyItemText = this.emptyItemText();
			if (ss.isValue(emptyItemText)) {
				hidden.attr('placeholder', emptyItemText);
			}
			hidden.select2(this.getSelect2Options());
			hidden.attr('type', 'text');
			// jquery validate to work
			hidden.bind('change.' + this.uniqueName, function(e, x) {
				if (!!($Serenity_WX.hasOriginalEvent(e) || !x)) {
					if (ss.isValue($Serenity_ValidationHelper.getValidator(hidden))) {
						hidden.valid();
					}
				}
			});
		};
		ss.registerGenericClassInstance($type, $Serenity_Select2Editor$2, [TOptions, TItem], {
			emptyItemText: function() {
				return ss.coalesce(this.element.attr('placeholder'), '--seçiniz--');
			},
			getSelect2Options: function() {
				var emptyItemText = this.emptyItemText();
				return { data: this.items, minimumResultsForSearch: 10, placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null), allowClear: ss.isValue(emptyItemText), query: ss.mkdel(this, function(query) {
					var term = (Q.isEmptyOrNull(query.term) ? '' : Select2.util.stripDiacritics(ss.coalesce(query.term, '')).toUpperCase());
					var results = this.items.filter(function(item) {
						return ss.isNullOrUndefined(term) || ss.startsWithString(Select2.util.stripDiacritics(ss.coalesce(item.text, '')).toUpperCase(), term);
					});
					ss.arrayAddRange(results, this.items.filter(function(item1) {
						return ss.isValue(term) && !ss.startsWithString(Select2.util.stripDiacritics(ss.coalesce(item1.text, '')).toUpperCase(), term) && Select2.util.stripDiacritics(ss.coalesce(item1.text, '')).toUpperCase().indexOf(term) >= 0;
					}));
					query.callback({ results: results.slice((query.page - 1) * this.pageSize, query.page * this.pageSize), more: results.length >= query.page * this.pageSize });
				}), initSelection: ss.mkdel(this, function(element, callback) {
					var val = element.val();
					var item2 = null;
					for (var i = 0; i < this.items.length; i++) {
						var x = this.items[i];
						if (ss.referenceEquals(x.id, val)) {
							item2 = x;
							break;
						}
					}
					callback(item2);
				}) };
			},
			clearItems: function() {
				ss.clear(this.items);
			},
			addItem: function(key, text, source, disabled) {
				ss.add(this.items, { id: key, text: text, source: source, disabled: disabled });
			},
			addInplaceCreate: function(title) {
				var self = this;
				$('<a><b/></a>').addClass('inplace-button inplace-create').attr('title', title).insertAfter(this.element).click(function(e) {
					self.inplaceCreateClick(e);
				});
				this.get_select2Container().add(this.element).addClass('has-inplace-button');
			},
			inplaceCreateClick: function(e) {
			},
			get_select2Container: function() {
				return this.element.prevAll('.select2-container');
			},
			get_items: function() {
				return this.items;
			},
			get_value: function() {
				return ss.safeCast(this.element.select2('val'), String);
			},
			set_value: function(value) {
				if (!ss.referenceEquals(value, this.get_value())) {
					this.element.select2('val', value).triggerHandler('change', [true]);
				}
			},
			get_text: function() {
				var value = ss.coalesce(this.get_value(), '').toString();
				var item = this.items.filter(function(s) {
					return ss.referenceEquals(s.id, value);
				})[0];
				return (ss.isValue(item) ? item.text : null);
			}
		}, function() {
			return ss.makeGenericType($Serenity_Widget$1, [TOptions]);
		}, function() {
			return [$Serenity_IStringValue];
		});
		ss.setMetadata($type, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
		return $type;
	};
	$Serenity_Select2Editor$2.__typeName = 'Serenity.Select2Editor$2';
	ss.initGenericClass($Serenity_Select2Editor$2, $asm, 2);
	global.Serenity.Select2Editor$2 = $Serenity_Select2Editor$2;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SelectEditor
	var $Serenity_SelectEditor = function(hidden, opt) {
		ss.makeGenericType($Serenity_Select2Editor$2, [$Serenity_SelectEditorOptions, Object]).call(this, hidden, opt);
		this.updateItems();
	};
	$Serenity_SelectEditor.__typeName = 'Serenity.SelectEditor';
	global.Serenity.SelectEditor = $Serenity_SelectEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SelectEditorOptions
	var $Serenity_SelectEditorOptions = function() {
	};
	$Serenity_SelectEditorOptions.__typeName = 'Serenity.SelectEditorOptions';
	$Serenity_SelectEditorOptions.createInstance = function() {
		return $Serenity_SelectEditorOptions.$ctor();
	};
	$Serenity_SelectEditorOptions.$ctor = function() {
		var $this = {};
		$this.items = null;
		$this.emptyOptionText = null;
		$this.emptyOptionText = '--seçiniz--';
		$this.items = [];
		return $this;
	};
	global.Serenity.SelectEditorOptions = $Serenity_SelectEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.StringEditor
	var $Serenity_StringEditor = function(input) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, input, new Object());
	};
	$Serenity_StringEditor.__typeName = 'Serenity.StringEditor';
	global.Serenity.StringEditor = $Serenity_StringEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.StringInflector
	var $Serenity_StringInflector = function() {
	};
	$Serenity_StringInflector.__typeName = 'Serenity.StringInflector';
	$Serenity_StringInflector.$initialize = function() {
		if ($Serenity_StringInflector.$initialized) {
			return;
		}
		var plural = [];
		ss.add(plural, [new RegExp('move', 'i'), 'moves']);
		ss.add(plural, [new RegExp('sex', 'i'), 'sexes']);
		ss.add(plural, [new RegExp('child', 'i'), 'children']);
		ss.add(plural, [new RegExp('man', 'i'), 'men']);
		ss.add(plural, [new RegExp('foot', 'i'), 'feet']);
		ss.add(plural, [new RegExp('person', 'i'), 'people']);
		ss.add(plural, [new RegExp('taxon', 'i'), 'taxa']);
		ss.add(plural, [new RegExp('(quiz)', 'i'), '$1zes']);
		ss.add(plural, [new RegExp('^(ox)$', 'i'), '$1en']);
		ss.add(plural, [new RegExp('(m|l)ouse$', 'i'), '$1ice']);
		ss.add(plural, [new RegExp('(matr|vert|ind|suff)ix|ex$', 'i'), '$1ices']);
		ss.add(plural, [new RegExp('(x|ch|ss|sh)$', 'i'), '$1es']);
		ss.add(plural, [new RegExp('([^aeiouy]|qu)y$', 'i'), '$1ies']);
		ss.add(plural, [new RegExp('(?:([^f])fe|([lr])f)$', 'i'), '$1$2ves']);
		ss.add(plural, [new RegExp('sis$', 'i'), 'ses']);
		ss.add(plural, [new RegExp('([ti]|addend)um$', 'i'), '$1a']);
		ss.add(plural, [new RegExp('(alumn|formul)a$', 'i'), '$1ae']);
		ss.add(plural, [new RegExp('(buffal|tomat|her)o$', 'i'), '$1oes']);
		ss.add(plural, [new RegExp('(bu)s$', 'i'), '$1ses']);
		ss.add(plural, [new RegExp('(alias|status)$', 'i'), '$1es']);
		ss.add(plural, [new RegExp('(octop|vir)us$', 'i'), '$1i']);
		ss.add(plural, [new RegExp('(gen)us$', 'i'), '$1era']);
		ss.add(plural, [new RegExp('(ax|test)is$', 'i'), '$1es']);
		ss.add(plural, [new RegExp('s$', 'i'), 's']);
		ss.add(plural, [new RegExp('$', 'i'), 's']);
		var singular = [];
		ss.add(singular, [new RegExp('cookies$', 'i'), 'cookie']);
		ss.add(singular, [new RegExp('moves$', 'i'), 'move']);
		ss.add(singular, [new RegExp('sexes$', 'i'), 'sex']);
		ss.add(singular, [new RegExp('children$', 'i'), 'child']);
		ss.add(singular, [new RegExp('men$', 'i'), 'man']);
		ss.add(singular, [new RegExp('feet$', 'i'), 'foot']);
		ss.add(singular, [new RegExp('people$', 'i'), 'person']);
		ss.add(singular, [new RegExp('taxa$', 'i'), 'taxon']);
		ss.add(singular, [new RegExp('databases$', 'i'), 'database']);
		ss.add(singular, [new RegExp('(quiz)zes$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(matr|suff)ices$', 'i'), '$1ix']);
		ss.add(singular, [new RegExp('(vert|ind)ices$', 'i'), '$1ex']);
		ss.add(singular, [new RegExp('^(ox)en', 'i'), '$1']);
		ss.add(singular, [new RegExp('(alias|status)es$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(tomato|hero|buffalo)es$', 'i'), '$1']);
		ss.add(singular, [new RegExp('([octop|vir])i$', 'i'), '$1us']);
		ss.add(singular, [new RegExp('(gen)era$', 'i'), '$1us']);
		ss.add(singular, [new RegExp('(cris|ax|test)es$', 'i'), '$1is']);
		ss.add(singular, [new RegExp('(shoe)s$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(o)es$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(bus)es$', 'i'), '$1']);
		ss.add(singular, [new RegExp('([m|l])ice$', 'i'), '$1ouse']);
		ss.add(singular, [new RegExp('(x|ch|ss|sh)es$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(m)ovies$', 'i'), '$1ovie']);
		ss.add(singular, [new RegExp('(s)eries$', 'i'), '$1eries']);
		ss.add(singular, [new RegExp('([^aeiouy]|qu)ies$', 'i'), '$1y']);
		ss.add(singular, [new RegExp('([lr])ves$', 'i'), '$1f']);
		ss.add(singular, [new RegExp('(tive)s$', 'i'), '$1']);
		ss.add(singular, [new RegExp('(hive)s$', 'i'), '$1']);
		ss.add(singular, [new RegExp('([^f])ves$', 'i'), '$1fe']);
		ss.add(singular, [new RegExp('(^analy)ses$', 'i'), '$1sis']);
		ss.add(singular, [new RegExp('((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$', 'i'), '$1\\2sis']);
		ss.add(singular, [new RegExp('([ti]|addend)a$', 'i'), '$1um']);
		ss.add(singular, [new RegExp('(alumn|formul)ae$', 'i'), '$1a']);
		ss.add(singular, [new RegExp('(n)ews$', 'i'), '$1ews']);
		ss.add(singular, [new RegExp('(.*)s$', 'i'), '$1']);
		var countable = [];
		ss.add(countable, 'aircraft');
		ss.add(countable, 'cannon');
		ss.add(countable, 'deer');
		ss.add(countable, 'equipment');
		ss.add(countable, 'fish');
		ss.add(countable, 'information');
		ss.add(countable, 'money');
		ss.add(countable, 'moose');
		ss.add(countable, 'rice');
		ss.add(countable, 'series');
		ss.add(countable, 'sheep');
		ss.add(countable, 'species');
		ss.add(countable, 'swin');
		$Serenity_StringInflector.$plural = plural;
		$Serenity_StringInflector.$singular = singular;
		$Serenity_StringInflector.$countable = countable;
		$Serenity_StringInflector.$initialized = true;
	};
	$Serenity_StringInflector.pluralize = function(word) {
		$Serenity_StringInflector.$initialize();
		if (ss.isValue($Serenity_StringInflector.$pluralizeCache) && ss.keyExists($Serenity_StringInflector.$pluralizeCache, word)) {
			return $Serenity_StringInflector.$pluralizeCache[word];
		}
		var result = word;
		for (var $t1 = 0; $t1 < $Serenity_StringInflector.$plural.length; $t1++) {
			var p = $Serenity_StringInflector.$plural[$t1];
			var r = ss.cast(p[0], RegExp);
			if (r.test(word)) {
				result = word.replace(r, ss.cast(p[1], String));
				break;
			}
		}
		for (var $t2 = 0; $t2 < $Serenity_StringInflector.$countable.length; $t2++) {
			var c = $Serenity_StringInflector.$countable[$t2];
			if (ss.referenceEquals(word, c)) {
				result = c;
				break;
			}
		}
		$Serenity_StringInflector.$pluralizeCache = $Serenity_StringInflector.$pluralizeCache || {};
		$Serenity_StringInflector.$pluralizeCache[word] = result;
		return result;
	};
	$Serenity_StringInflector.singularize = function(word) {
		$Serenity_StringInflector.$initialize();
		if (ss.isValue($Serenity_StringInflector.$singularizeCache) && ss.keyExists($Serenity_StringInflector.$singularizeCache, word)) {
			return $Serenity_StringInflector.$singularizeCache[word];
		}
		var result = word;
		for (var $t1 = 0; $t1 < $Serenity_StringInflector.$singular.length; $t1++) {
			var p = $Serenity_StringInflector.$singular[$t1];
			var r = ss.cast(p[0], RegExp);
			if (r.test(word)) {
				result = word.replace(r, ss.cast(p[1], String));
				break;
			}
		}
		for (var $t2 = 0; $t2 < $Serenity_StringInflector.$countable.length; $t2++) {
			var c = $Serenity_StringInflector.$countable[$t2];
			if (ss.referenceEquals(word, c)) {
				result = c;
				break;
			}
		}
		$Serenity_StringInflector.$singularizeCache = $Serenity_StringInflector.$singularizeCache || {};
		$Serenity_StringInflector.$singularizeCache[word] = result;
		return result;
	};
	$Serenity_StringInflector.isSingular = function(word) {
		$Serenity_StringInflector.$initialize();
		return ss.referenceEquals($Serenity_StringInflector.singularize($Serenity_StringInflector.pluralize(word)).toLowerCase(), word.toLowerCase());
	};
	$Serenity_StringInflector.isPlural = function(word) {
		$Serenity_StringInflector.$initialize();
		return ss.referenceEquals($Serenity_StringInflector.pluralize($Serenity_StringInflector.singularize(word)).toLowerCase(), word.toLowerCase());
	};
	global.Serenity.StringInflector = $Serenity_StringInflector;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SubDialogHelper
	var $Serenity_SubDialogHelper = function() {
	};
	$Serenity_SubDialogHelper.__typeName = 'Serenity.SubDialogHelper';
	$Serenity_SubDialogHelper.bindToDataChange = function(dialog, owner, dataChange, useTimeout) {
		var widgetName = owner.get_widgetName();
		dialog.get_element().bind('ondatachange.' + widgetName, function(e, dci) {
			if (useTimeout) {
				window.setTimeout(function() {
					dataChange(e, dci);
				}, 0);
			}
			else {
				dataChange(e, dci);
			}
		}).bind('remove.' + widgetName, function() {
			dialog.get_element().unbind('ondatachange.' + widgetName);
		});
		return dialog;
	};
	$Serenity_SubDialogHelper.triggerDataChange = function(dialog) {
		dialog.get_element().triggerHandler('ondatachange');
		return dialog;
	};
	$Serenity_SubDialogHelper.triggerDataChange$1 = function(element) {
		element.triggerHandler('ondatachange');
		return element;
	};
	$Serenity_SubDialogHelper.bubbleDataChange = function(dialog, owner, useTimeout) {
		return $Serenity_SubDialogHelper.bindToDataChange(dialog, owner, function(e, dci) {
			owner.get_element().triggerHandler('ondatachange');
		}, useTimeout);
	};
	$Serenity_SubDialogHelper.cascade = function(cascadedDialog, ofElement) {
		cascadedDialog.get_element().dialog().dialog('option', 'position', $Serenity_SubDialogHelper.cascadedDialogOffset(ofElement));
		return cascadedDialog;
	};
	$Serenity_SubDialogHelper.cascadedDialogOffset = function(element) {
		return { my: 'left top', at: 'left+20 top+20', of: element[0] };
	};
	global.Serenity.SubDialogHelper = $Serenity_SubDialogHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TemplatedDialog
	var $Serenity_TemplatedDialog = function() {
		ss.makeGenericType($Serenity_TemplatedDialog$1, [Object]).$ctor1.call(this, null);
	};
	$Serenity_TemplatedDialog.__typeName = 'Serenity.TemplatedDialog';
	global.Serenity.TemplatedDialog = $Serenity_TemplatedDialog;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TemplatedDialog
	var $Serenity_TemplatedDialog$1 = function(TOptions) {
		var $type = function() {
			$type.$ctor2.call(this, Q.newBodyDiv(), null);
		};
		$type.$ctor2 = function(div, opt) {
			this.isPanel = false;
			this.validator = null;
			this.tabs = null;
			this.toolbar = null;
			ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]).call(this, div, opt);
			this.isPanel = ss.getAttributes(ss.getInstanceType(this), $Serenity_PanelAttribute, true).length > 0;
			if (!this.isPanel) {
				this.initDialog();
			}
			this.initValidator();
			this.initTabs();
			this.initToolbar();
		};
		$type.$ctor1 = function(opt) {
			$type.$ctor2.call(this, Q.newBodyDiv(), opt);
		};
		$type.$getCssSize = function(element, name, size) {
			size.$ = 0;
			var cssSize = element.css(name);
			if (!ss.isValue(cssSize)) {
				return false;
			}
			if (!ss.endsWithString(cssSize, 'px')) {
				return false;
			}
			cssSize = cssSize.substr(0, cssSize.length - 2);
			if (!ss.Int32.tryParse(cssSize, size) || size.$ === 0) {
				return false;
			}
			return true;
		};
		$type.$applyCssSizes = function(opt, dialogClass) {
			var size = {};
			var dialog = $('<div/>').hide().addClass(dialogClass).appendTo(document.body);
			try {
				var sizeHelper = $('<div/>').addClass('size').appendTo(dialog);
				if ($type.$getCssSize(sizeHelper, 'minWidth', size)) {
					opt.minWidth = size.$;
				}
				if ($type.$getCssSize(sizeHelper, 'width', size)) {
					opt.width = size.$;
				}
				if ($type.$getCssSize(sizeHelper, 'height', size)) {
					opt.height = size.$;
				}
				if ($type.$getCssSize(sizeHelper, 'minHeight', size)) {
					opt.minHeight = size.$;
				}
			}
			finally {
				dialog.remove();
			}
		};
		ss.registerGenericClassInstance($type, $Serenity_TemplatedDialog$1, [TOptions], {
			destroy: function() {
				if (ss.isValue(this.tabs)) {
					this.tabs.tabs('destroy');
				}
				if (ss.isValue(this.toolbar)) {
					this.toolbar.destroy();
					this.toolbar = null;
				}
				if (ss.isValue(this.validator)) {
					this.byId$1('Form').remove();
					this.validator = null;
				}
				if (!this.isPanel) {
					this.element.dialog().dialog('destroy');
				}
				$Serenity_Widget.prototype.destroy.call(this);
			},
			initDialog: function() {
				this.element.dialog(this.getDialogOptions());
				var self = this;
				this.element.bind('dialogopen.' + this.uniqueName, function() {
					self.onDialogOpen();
				});
				this.element.bind('dialogclose.' + this.uniqueName, function() {
					self.onDialogClose();
				});
			},
			initToolbar: function() {
				var toolbarDiv = this.byId$1('Toolbar');
				if (toolbarDiv.length === 0) {
					return;
				}
				var opt = { buttons: this.getToolbarButtons() };
				this.toolbar = new $Serenity_Toolbar(toolbarDiv, opt);
			},
			getToolbarButtons: function() {
				return [];
			},
			getValidatorOptions: function() {
				return {};
			},
			initValidator: function() {
				var form = this.byId$1('Form');
				if (form.length > 0) {
					var valOptions = this.getValidatorOptions();
					this.validator = form.validate(Q$Externals.validateOptions(valOptions));
				}
			},
			resetValidation: function() {
				if (ss.isValue(this.validator)) {
					this.validator.resetAll();
				}
			},
			validateForm: function() {
				return ss.isNullOrUndefined(this.validator) || !!this.validator.form();
			},
			dialogOpen: function() {
				this.element.dialog().dialog('open');
			},
			onDialogOpen: function() {
				$(':input:eq(0)', this.element).focus();
				this.arrange();
				if (ss.isValue(this.tabs)) {
					this.tabs.tabs('option', 'active', 0);
				}
			},
			arrange: function() {
				this.element.find('.require-layout').filter(':visible').each(function(i, e) {
					$(e).triggerHandler('layout');
				});
			},
			onDialogClose: function() {
				$(document).trigger('click');
				// for tooltips etc.
				if (ss.isValue($.qtip)) {
					$(document.body).children('.qtip').each(function(index, el) {
						$(el).qtip('hide');
					});
				}
				var self = this;
				window.setTimeout(function() {
					var element = self.element;
					self.destroy();
					element.remove();
				}, 0);
			},
			addCssClass: function() {
				if (ss.getAttributes(ss.getInstanceType(this), $Serenity_PanelAttribute, true).length > 0) {
					$Serenity_Widget.prototype.addCssClass.call(this);
				}
				// will add css class to ui-dialog container, not content element
			},
			getDialogOptions: function() {
				var opt = {};
				var dialogClass = 's-Dialog s-' + ss.getTypeName(ss.getInstanceType(this));
				opt.dialogClass = dialogClass;
				opt.width = 920;
				$type.$applyCssSizes(opt, dialogClass);
				opt.autoOpen = false;
				opt.resizable = false;
				opt.modal = true;
				opt.position = { my: 'center', at: 'center', of: $(window.window) };
				return opt;
			},
			dialogClose: function() {
				this.element.dialog().dialog('close');
			},
			initTabs: function() {
				var tabsDiv = this.byId$1('Tabs');
				if (tabsDiv.length === 0) {
					return;
				}
				this.tabs = tabsDiv.tabs({});
			},
			get_idPrefix: function() {
				return this.idPrefix;
			}
		}, function() {
			return ss.makeGenericType($Serenity_TemplatedWidget$1, [TOptions]);
		}, function() {
			return [$Serenity_IDialog];
		});
		$type.$ctor2.prototype = $type.$ctor1.prototype = $type.prototype;
		return $type;
	};
	$Serenity_TemplatedDialog$1.__typeName = 'Serenity.TemplatedDialog$1';
	ss.initGenericClass($Serenity_TemplatedDialog$1, $asm, 1);
	global.Serenity.TemplatedDialog$1 = $Serenity_TemplatedDialog$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TemplatedWidget
	var $Serenity_TemplatedWidget = function(element) {
		ss.makeGenericType($Serenity_TemplatedWidget$1, [Object]).call(this, element, null);
	};
	$Serenity_TemplatedWidget.__typeName = 'Serenity.TemplatedWidget';
	global.Serenity.TemplatedWidget = $Serenity_TemplatedWidget;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TemplatedWidget
	var $Serenity_TemplatedWidget$1 = function(TOptions) {
		var $type = function(element, opt) {
			this.idPrefix = null;
			ss.makeGenericType($Serenity_Widget$1, [TOptions]).call(this, element, opt);
			this.idPrefix = this.uniqueName + '_';
			if (!this.isAsyncWidget()) {
				var widgetMarkup = this.getTemplate().replace(new RegExp('~_', 'g'), this.idPrefix);
				widgetMarkup = $Serenity_JsRender.render(widgetMarkup, null);
				this.element.html(widgetMarkup);
			}
		};
		ss.registerGenericClassInstance($type, $Serenity_TemplatedWidget$1, [TOptions], {
			initializeAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1, widgetMarkup;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									$t1 = this.getTemplateAsync();
									$state = 1;
									$t1.continueWith($sm);
									return;
								}
								case 1: {
									$state = -1;
									widgetMarkup = $t1.getAwaitedResult().replace(new RegExp('~_', 'g'), this.idPrefix);
									widgetMarkup = $Serenity_JsRender.render(widgetMarkup, null);
									this.element.html(widgetMarkup);
									$state = -1;
									break $sm1;
								}
								default: {
									break $sm1;
								}
							}
						}
						$tcs.setResult(null);
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			},
			byId$1: function(id) {
				return $('#' + this.idPrefix + id);
			},
			byId: function(TWidget) {
				return function(id) {
					return $Serenity_WX.getWidget(TWidget).call(null, this.byId$1(id));
				};
			},
			getTemplateName: function() {
				return ss.getTypeName(ss.getInstanceType(this));
			},
			getTemplate: function() {
				var templateName = this.getTemplateName();
				var template;
				var script = $('script#Template_' + templateName);
				if (script.length > 0) {
					template = script.html();
				}
				else {
					template = Q.getTemplate(templateName);
					if (!ss.isValue(template)) {
						throw new ss.Exception(ss.formatString("Can't locate template for widget '{0}' with name '{1}'!", ss.getTypeName(ss.getInstanceType(this)), templateName));
					}
				}
				return template;
			},
			getTemplateAsync: function() {
				var $state = 0, $tcs = new ss.TaskCompletionSource(), templateName, template, script, $t1;
				var $sm = ss.mkdel(this, function() {
					try {
						$sm1:
						for (;;) {
							switch ($state) {
								case 0: {
									$state = -1;
									templateName = this.getTemplateName();
									script = $('script#Template_' + templateName);
									if (script.length > 0) {
										template = script.html();
										$state = 1;
										continue $sm1;
									}
									else {
										$t1 = Q.getTemplateAsync(templateName);
										$state = 2;
										$t1.continueWith($sm);
										return;
									}
								}
								case 2: {
									$state = -1;
									template = $t1.getAwaitedResult();
									if (!ss.isValue(template)) {
										throw new ss.Exception(ss.formatString("Can't locate template for widget '{0}' with name '{1}'!", ss.getTypeName(ss.getInstanceType(this)), templateName));
									}
									$state = 1;
									continue $sm1;
								}
								case 1: {
									$state = -1;
									$tcs.setResult(template);
									return;
								}
								default: {
									break $sm1;
								}
							}
						}
					}
					catch ($t2) {
						$tcs.setException(ss.Exception.wrap($t2));
					}
				});
				$sm();
				return $tcs.task;
			}
		}, function() {
			return ss.makeGenericType($Serenity_Widget$1, [TOptions]);
		}, function() {
			return [];
		});
		return $type;
	};
	$Serenity_TemplatedWidget$1.__typeName = 'Serenity.TemplatedWidget$1';
	ss.initGenericClass($Serenity_TemplatedWidget$1, $asm, 1);
	global.Serenity.TemplatedWidget$1 = $Serenity_TemplatedWidget$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TextAreaEditor
	var $Serenity_TextAreaEditor = function(input, opt) {
		ss.makeGenericType($Serenity_Widget$1, [$Serenity_TextAreaEditorOptions]).call(this, input, opt);
		if (this.options.cols !== 0) {
			input.attr('cols', this.options.cols.toString());
		}
		if (this.options.rows !== 0) {
			input.attr('rows', this.options.rows.toString());
		}
	};
	$Serenity_TextAreaEditor.__typeName = 'Serenity.TextAreaEditor';
	global.Serenity.TextAreaEditor = $Serenity_TextAreaEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TextAreaEditorOptions
	var $Serenity_TextAreaEditorOptions = function() {
	};
	$Serenity_TextAreaEditorOptions.__typeName = 'Serenity.TextAreaEditorOptions';
	$Serenity_TextAreaEditorOptions.createInstance = function() {
		return $Serenity_TextAreaEditorOptions.$ctor();
	};
	$Serenity_TextAreaEditorOptions.$ctor = function() {
		var $this = {};
		$this.cols = 0;
		$this.rows = 0;
		$this.cols = 80;
		$this.rows = 6;
		return $this;
	};
	global.Serenity.TextAreaEditorOptions = $Serenity_TextAreaEditorOptions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Toolbar
	var $Serenity_Toolbar = function(div, options) {
		ss.makeGenericType($Serenity_Widget$1, [Object]).call(this, div, options);
		this.element.addClass('s-Toolbar').html('<div class="tool-buttons"><div class="buttons-outer"><div class="buttons-inner"></div></div></div>');
		var container = $('div.buttons-inner', this.element);
		var buttons = this.options.buttons;
		for (var i = 0; i < buttons.length; i++) {
			this.$createButton(container, buttons[i]);
		}
	};
	$Serenity_Toolbar.__typeName = 'Serenity.Toolbar';
	global.Serenity.Toolbar = $Serenity_Toolbar;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.UploadedFile
	var $Serenity_UploadedFile = function() {
		this.$1$FilenameField = null;
		this.$1$OriginalNameField = null;
	};
	$Serenity_UploadedFile.__typeName = 'Serenity.UploadedFile';
	global.Serenity.UploadedFile = $Serenity_UploadedFile;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.UploadHelper
	var $Serenity_UploadHelper = function() {
	};
	$Serenity_UploadHelper.__typeName = 'Serenity.UploadHelper';
	$Serenity_UploadHelper.addUploadInput = function(options) {
		options.container.addClass('fileinput-button');
		var uploadInput = $('<input/>').attr('type', 'file').attr('name', options.inputName + '[]').attr('data-url', Q.resolveUrl('~/File/TemporaryUpload')).attr('multiple', 'multiple').appendTo(options.container);
		if (options.allowMultiple) {
			uploadInput.attr('multiple', 'multiple');
		}
		uploadInput.fileupload({
			dataType: 'json',
			done: function(e, data) {
				var response = data.result;
				if (!ss.staticEquals(options.fileDone, null)) {
					options.fileDone(response, ss.cast(data.files[0].name, String), data);
				}
			},
			start: function() {
				Q.blockUI(null);
				if (ss.isValue(options.progress)) {
					options.progress.show();
				}
			},
			stop: function() {
				Q.blockUndo();
				if (ss.isValue(options.progress)) {
					options.progress.hide();
				}
			},
			progress: function(e1, data1) {
				if (ss.isValue(options.progress)) {
					var percent = ss.unbox(ss.cast(data1.loaded, Number)) / ss.unbox(ss.cast(data1.total, Number)) * 100;
					options.progress.children().css('width', percent.toString() + '%');
				}
			}
		});
		return uploadInput;
	};
	$Serenity_UploadHelper.checkImageConstraints = function(file, opt) {
		if (!file.IsImage) {
			Q.alert('Yüklemeye çalıştığınız dosya bir resim değil!');
			return false;
		}
		if (opt.minSize > 0 && file.Size < opt.minSize) {
			Q.alert(ss.formatString('Yükleyeceğiniz dosya en az {0} boyutunda olmalı!', opt.minSize));
			return false;
		}
		if (opt.maxSize > 0 && file.Size > opt.maxSize) {
			Q.alert(ss.formatString('Yükleyeceğiniz dosya en çok {0} boyutunda olabilir!', opt.maxSize));
			return false;
		}
		if (opt.minWidth > 0 && file.Width < opt.minWidth) {
			Q.alert(ss.formatString('Yükleyeceğiniz resim en az {0} genişliğinde olmalı!', opt.minWidth));
			return false;
		}
		if (opt.maxWidth > 0 && file.Width > opt.maxWidth) {
			Q.alert(ss.formatString('Yükleyeceğiniz dosya en çok {0} genişliğinde olabilir!', opt.maxWidth));
			return false;
		}
		if (opt.minHeight > 0 && file.Height < opt.minHeight) {
			Q.alert(ss.formatString('Yükleyeceğiniz resim en az {0} yüksekliğinde olmalı!', opt.minHeight));
			return false;
		}
		if (opt.maxHeight > 0 && file.Height > opt.maxHeight) {
			Q.alert(ss.formatString('Yükleyeceğiniz dosya en çok {0} yüksekliğinde olabilir!', opt.maxHeight));
			return false;
		}
		return true;
	};
	$Serenity_UploadHelper.fileNameSizeDisplay = function(name, bytes) {
		return name + ' (' + $Serenity_UploadHelper.fileSizeDisplay(bytes) + ')';
	};
	$Serenity_UploadHelper.fileSizeDisplay = function(bytes) {
		var byteSize = ss.round(bytes * 100 / 1024) * 0.01;
		var suffix = 'KB';
		if (byteSize > 1000) {
			byteSize = ss.round(byteSize * 0.001 * 100) * 0.01;
			suffix = 'MB';
		}
		var sizeParts = byteSize.toString().split(String.fromCharCode(46));
		var value;
		if (sizeParts.length > 1) {
			value = sizeParts[0] + '.' + sizeParts[1].substr(0, 2);
		}
		else {
			value = sizeParts[0];
		}
		return value + ' ' + suffix;
	};
	$Serenity_UploadHelper.hasImageExtension = function(filename) {
		if (ss.isNullOrUndefined(filename)) {
			return false;
		}
		filename = filename.toLowerCase();
		return ss.endsWithString(filename, '.jpg') || ss.endsWithString(filename, '.jpeg') || ss.endsWithString(filename, '.gif') || ss.endsWithString(filename, '.png');
	};
	$Serenity_UploadHelper.thumbFileName = function(filename) {
		filename = ss.coalesce(filename, '');
		var idx = filename.lastIndexOf('.');
		if (idx >= 0) {
			filename = filename.substr(0, idx);
		}
		return filename + '_t.jpg';
	};
	$Serenity_UploadHelper.dbFileUrl = function(filename) {
		filename = ss.replaceAllString(ss.coalesce(filename, ''), '\\', '/');
		return Q.resolveUrl('~/upload/') + filename;
	};
	$Serenity_UploadHelper.colorBox = function(link, options) {
		link.colorbox({ current: 'resim {current} / {total}', previous: 'önceki', next: 'sonraki', close: 'kapat' });
	};
	$Serenity_UploadHelper.populateFileSymbols = function(container, items, displayOriginalName) {
		items = items || [];
		container.html('');
		for (var index = 0; index < items.length; index++) {
			var item = items[index];
			var li = $('<li/>').addClass('file-item').data('index', index);
			var isImage = $Serenity_UploadHelper.hasImageExtension(item.get_filename());
			if (isImage) {
				li.addClass('file-image');
			}
			else {
				li.addClass('file-binary');
			}
			var editLink = '#' + index;
			var thumb = $('<a/>').addClass('thumb').appendTo(li);
			var originalName = ss.coalesce(item.get_originalName(), '');
			if (isImage) {
				thumb.attr('href', $Serenity_UploadHelper.dbFileUrl(item.get_filename()));
				thumb.attr('target', '_blank');
				if (!Q.isEmptyOrNull(originalName)) {
					thumb.attr('title', originalName);
				}
				thumb.css('backgroundImage', 'url(' + $Serenity_UploadHelper.dbFileUrl($Serenity_UploadHelper.thumbFileName(item.get_filename())) + ')');
				$Serenity_UploadHelper.colorBox(thumb, new Object());
			}
			if (displayOriginalName) {
				$('<div/>').addClass('filename').text(originalName).attr('title', originalName).appendTo(li);
			}
			li.appendTo(container);
		}
		;
	};
	global.Serenity.UploadHelper = $Serenity_UploadHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.URLEditor
	var $Serenity_URLEditor = function(input) {
		$Serenity_StringEditor.call(this, input);
		input.addClass('url').attr('title', "URL 'http://www.site.com/sayfa' formatında girilmelidir.");
		input.bind('blur.' + this.uniqueName, function() {
			var validator = input.closest('form').data('validator');
			if (ss.isNullOrUndefined(validator)) {
				return;
			}
			if (!input.hasClass('error')) {
				return;
			}
			var value = Q.trimToNull(input.val());
			if (ss.isNullOrUndefined(value)) {
				return;
			}
			value = 'http://' + value;
			if (!!ss.referenceEquals($.validator.methods['url'].apply(validator, [value, input[0]]), true)) {
				input.val(value);
				validator.element(input[0]);
			}
		});
	};
	$Serenity_URLEditor.__typeName = 'Serenity.URLEditor';
	global.Serenity.URLEditor = $Serenity_URLEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ValidationHelper
	var $Serenity_ValidationHelper = function() {
	};
	$Serenity_ValidationHelper.__typeName = 'Serenity.ValidationHelper';
	$Serenity_ValidationHelper.asyncSubmit = function(form, validateBeforeSave, submitHandler) {
		var validator = form.validate();
		var valSettings = validator.settings;
		if (!!ss.isValue(valSettings.abortHandler)) {
			return false;
		}
		if (!ss.staticEquals(validateBeforeSave, null) && validateBeforeSave() === false) {
			return false;
		}
		valSettings['abortHandler'] = Q$Externals.validatorAbortHandler;
		valSettings['submitHandler'] = function() {
			if (!ss.staticEquals(submitHandler, null)) {
				submitHandler();
			}
			return false;
		};
		form.trigger('submit');
		return true;
	};
	$Serenity_ValidationHelper.submit = function(form, validateBeforeSave, submitHandler) {
		var validator = form.validate();
		var valSettings = validator.settings;
		if (!!ss.isValue(valSettings.abortHandler)) {
			return false;
		}
		if (!ss.staticEquals(validateBeforeSave, null) && validateBeforeSave() === false) {
			return false;
		}
		if (!validator.form()) {
			return false;
		}
		if (!ss.staticEquals(submitHandler, null)) {
			submitHandler();
		}
		return true;
	};
	$Serenity_ValidationHelper.getValidator = function(element) {
		var form = element.closest('form');
		if (form.length === 0) {
			return null;
		}
		return form.data('validator');
	};
	global.Serenity.ValidationHelper = $Serenity_ValidationHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Widget
	var $Serenity_Widget = function(element) {
		this.widgetName = null;
		this.uniqueName = null;
		this.element = null;
		Serenity.ScriptContext.call(this);
		this.element = element;
		this.widgetName = $Serenity_WX.getWidgetName(ss.getInstanceType(this));
		this.uniqueName = this.widgetName + ($Serenity_Widget.$nextWidgetNumber++).toString();
		if (ss.isValue(element.data(this.widgetName))) {
			throw new ss.Exception(ss.formatString("The element already has widget '{0}'!", this.widgetName));
		}
		var self = this;
		element.bind('remove.' + this.widgetName, function(e) {
			self.destroy();
		}).data(this.widgetName, this);
		this.addCssClass();
		this.onInit();
	};
	$Serenity_Widget.__typeName = 'Serenity.Widget';
	global.Serenity.Widget = $Serenity_Widget;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Widget
	var $Serenity_Widget$1 = function(TOptions) {
		var $type = function(element, opt) {
			this.options = null;
			$Serenity_Widget.call(this, element);
			this.options = opt || ss.createInstance(TOptions);
		};
		ss.registerGenericClassInstance($type, $Serenity_Widget$1, [TOptions], {}, function() {
			return $Serenity_Widget;
		}, function() {
			return [];
		});
		return $type;
	};
	$Serenity_Widget$1.__typeName = 'Serenity.Widget$1';
	ss.initGenericClass($Serenity_Widget$1, $asm, 1);
	global.Serenity.Widget$1 = $Serenity_Widget$1;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.WidgetExtensions
	var $Serenity_WX = function() {
	};
	$Serenity_WX.__typeName = 'Serenity.WX';
	$Serenity_WX.getWidget = function(TWidget) {
		return function(element) {
			var widget = $Serenity_WX.tryGetWidget(TWidget).call(null, element);
			if (ss.isNullOrUndefined(widget)) {
				throw new ss.Exception(ss.formatString("Element has no widget of type '{0}'!", $Serenity_WX.getWidgetName(TWidget)));
			}
			return widget;
		};
	};
	$Serenity_WX.getWidget$1 = function(element, widgetType) {
		var widget = $Serenity_WX.tryGetWidget$1(element, widgetType);
		if (ss.isNullOrUndefined(widget)) {
			throw new ss.Exception(ss.formatString("Element has no widget of type '{0}'!", $Serenity_WX.getWidgetName(widgetType)));
		}
		return widget;
	};
	$Serenity_WX.tryGetWidget = function(TWidget) {
		return function(element) {
			if (ss.isNullOrUndefined(element)) {
				throw new ss.Exception("Argument 'element' is null!");
			}
			var widgetName = $Serenity_WX.getWidgetName(TWidget);
			return ss.safeCast(element.data(widgetName), TWidget);
		};
	};
	$Serenity_WX.tryGetWidget$1 = function(element, widgetType) {
		if (ss.isNullOrUndefined(widgetType)) {
			throw new ss.Exception("Argument 'widgetType' is null!");
		}
		if (ss.isNullOrUndefined(element)) {
			throw new ss.Exception("Argument 'element' is null!");
		}
		var widgetName = $Serenity_WX.getWidgetName(widgetType);
		var widget = element.data(widgetName);
		if (ss.isValue(widget) && !ss.isAssignableFrom(widgetType, ss.getInstanceType(widget))) {
			return null;
		}
		return widget;
	};
	$Serenity_WX.getWidgetName = function(type) {
		return ss.replaceAllString(ss.getTypeFullName(type), '.', '_');
	};
	$Serenity_WX.hasOriginalEvent = function(e) {
		return !!!(typeof(e.originalEvent) === 'undefined');
	};
	$Serenity_WX.validateElement = function(validator, widget) {
		return validator.element(widget.get_element()[0]);
	};
	$Serenity_WX.change = function(widget, handler) {
		widget.get_element().bind('change.' + widget.get_uniqueName(), handler);
	};
	$Serenity_WX.changeSelect2 = function(widget, handler) {
		widget.get_element().bind('change.' + widget.get_uniqueName(), function(e, x) {
			if (!!($Serenity_WX.hasOriginalEvent(e) || !x)) {
				handler(e);
			}
		});
	};
	$Serenity_WX.getGridField = function(widget) {
		return widget.get_element().closest('.field');
	};
	$Serenity_WX.addValidationRule = function(widget, eventClass, rule) {
		return $Serenity_WX.addValidationRule$1(widget.get_element(), eventClass, rule);
	};
	$Serenity_WX.addValidationRule$1 = function(element, eventClass, rule) {
		if (element.length === 0) {
			return element;
		}
		if (ss.staticEquals(rule, null)) {
			throw new ss.Exception('rule is null!');
		}
		element.addClass('customValidate').bind('customValidate.' + eventClass, rule);
		return element;
	};
	$Serenity_WX.removeValidationRule = function(element, eventClass) {
		element.unbind('customValidate.' + eventClass);
		return element;
	};
	$Serenity_WX.createElementFor = function(TEditor) {
		return function() {
			return $Serenity_WX.createElementFor$1(TEditor);
		};
	};
	$Serenity_WX.createElementFor$1 = function(editorType) {
		var elementAttr = ss.getAttributes(editorType, Serenity.ElementAttribute, true);
		var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].get_html() : '<input/>');
		return $(elementHtml);
	};
	$Serenity_WX.create = function(TWidget) {
		return function(initElement, options) {
			var element = $Serenity_WX.createElementFor$1(TWidget);
			if (!ss.staticEquals(initElement, null)) {
				initElement(element);
			}
			return ss.cast(new TWidget(element, options), TWidget);
		};
	};
	global.Serenity.WX = $Serenity_WX;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.CategoryAttribute
	var $Serenity_ComponentModel_CategoryAttribute = function(category) {
		this.$2$CategoryField = null;
		this.set_category(category);
	};
	$Serenity_ComponentModel_CategoryAttribute.__typeName = 'Serenity.ComponentModel.CategoryAttribute';
	global.Serenity.ComponentModel.CategoryAttribute = $Serenity_ComponentModel_CategoryAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.CssClassAttribute
	var $Serenity_ComponentModel_CssClassAttribute = function(cssClass) {
		this.$2$CssClassField = null;
		this.set_cssClass(cssClass);
	};
	$Serenity_ComponentModel_CssClassAttribute.__typeName = 'Serenity.ComponentModel.CssClassAttribute';
	global.Serenity.ComponentModel.CssClassAttribute = $Serenity_ComponentModel_CssClassAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.DefaultValueAttribute
	var $Serenity_ComponentModel_DefaultValueAttribute = function(defaultValue) {
		this.$2$ValueField = null;
		this.set_value(defaultValue);
	};
	$Serenity_ComponentModel_DefaultValueAttribute.__typeName = 'Serenity.ComponentModel.DefaultValueAttribute';
	global.Serenity.ComponentModel.DefaultValueAttribute = $Serenity_ComponentModel_DefaultValueAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.EditorOptionAttribute
	var $Serenity_ComponentModel_EditorOptionAttribute = function(key, value) {
		this.$2$KeyField = null;
		this.$2$ValueField = null;
		this.set_key(key);
		this.set_value(value);
	};
	$Serenity_ComponentModel_EditorOptionAttribute.__typeName = 'Serenity.ComponentModel.EditorOptionAttribute';
	global.Serenity.ComponentModel.EditorOptionAttribute = $Serenity_ComponentModel_EditorOptionAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.EditorTypeAttribute
	var $Serenity_ComponentModel_EditorTypeAttribute = function(editorType) {
		$Serenity_ComponentModel_EditorTypeAttributeBase.call(this, editorType);
	};
	$Serenity_ComponentModel_EditorTypeAttribute.__typeName = 'Serenity.ComponentModel.EditorTypeAttribute';
	global.Serenity.ComponentModel.EditorTypeAttribute = $Serenity_ComponentModel_EditorTypeAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.EditorTypeAttributeBase
	var $Serenity_ComponentModel_EditorTypeAttributeBase = function(type) {
		this.editorType = null;
		this.editorType = type;
	};
	$Serenity_ComponentModel_EditorTypeAttributeBase.__typeName = 'Serenity.ComponentModel.EditorTypeAttributeBase';
	global.Serenity.ComponentModel.EditorTypeAttributeBase = $Serenity_ComponentModel_EditorTypeAttributeBase;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.HiddenAttribute
	var $Serenity_ComponentModel_HiddenAttribute = function() {
	};
	$Serenity_ComponentModel_HiddenAttribute.__typeName = 'Serenity.ComponentModel.HiddenAttribute';
	global.Serenity.ComponentModel.HiddenAttribute = $Serenity_ComponentModel_HiddenAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.HintAttribute
	var $Serenity_ComponentModel_HintAttribute = function(hint) {
		this.$2$HintField = null;
		this.set_hint(hint);
	};
	$Serenity_ComponentModel_HintAttribute.__typeName = 'Serenity.ComponentModel.HintAttribute';
	global.Serenity.ComponentModel.HintAttribute = $Serenity_ComponentModel_HintAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.InsertableAttribute
	var $Serenity_ComponentModel_InsertableAttribute = function(insertable) {
		this.$2$ValueField = false;
		this.set_value(insertable);
	};
	$Serenity_ComponentModel_InsertableAttribute.__typeName = 'Serenity.ComponentModel.InsertableAttribute';
	global.Serenity.ComponentModel.InsertableAttribute = $Serenity_ComponentModel_InsertableAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.MaxLengthAttribute
	var $Serenity_ComponentModel_MaxLengthAttribute = function(maxLength) {
		this.$2$MaxLengthField = 0;
		this.set_maxLength(maxLength);
	};
	$Serenity_ComponentModel_MaxLengthAttribute.__typeName = 'Serenity.ComponentModel.MaxLengthAttribute';
	global.Serenity.ComponentModel.MaxLengthAttribute = $Serenity_ComponentModel_MaxLengthAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.OneWayAttribute
	var $Serenity_ComponentModel_OneWayAttribute = function() {
	};
	$Serenity_ComponentModel_OneWayAttribute.__typeName = 'Serenity.ComponentModel.OneWayAttribute';
	global.Serenity.ComponentModel.OneWayAttribute = $Serenity_ComponentModel_OneWayAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.OptionAttribute
	var $Serenity_ComponentModel_OptionAttribute = function() {
	};
	$Serenity_ComponentModel_OptionAttribute.__typeName = 'Serenity.ComponentModel.OptionAttribute';
	global.Serenity.ComponentModel.OptionAttribute = $Serenity_ComponentModel_OptionAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.PlaceholderAttribute
	var $Serenity_ComponentModel_PlaceholderAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_ComponentModel_PlaceholderAttribute.__typeName = 'Serenity.ComponentModel.PlaceholderAttribute';
	global.Serenity.ComponentModel.PlaceholderAttribute = $Serenity_ComponentModel_PlaceholderAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.ReadOnlyAttribute
	var $Serenity_ComponentModel_ReadOnlyAttribute = function(readOnly) {
		this.$2$ValueField = false;
		this.set_value(readOnly);
	};
	$Serenity_ComponentModel_ReadOnlyAttribute.__typeName = 'Serenity.ComponentModel.ReadOnlyAttribute';
	global.Serenity.ComponentModel.ReadOnlyAttribute = $Serenity_ComponentModel_ReadOnlyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.RequiredAttribute
	var $Serenity_ComponentModel_RequiredAttribute = function(isRequired) {
		this.$2$IsRequiredField = false;
		this.set_isRequired(isRequired);
	};
	$Serenity_ComponentModel_RequiredAttribute.__typeName = 'Serenity.ComponentModel.RequiredAttribute';
	global.Serenity.ComponentModel.RequiredAttribute = $Serenity_ComponentModel_RequiredAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.UpdatableAttribute
	var $Serenity_ComponentModel_UpdatableAttribute = function(updatable) {
		this.$2$ValueField = false;
		this.set_value(updatable);
	};
	$Serenity_ComponentModel_UpdatableAttribute.__typeName = 'Serenity.ComponentModel.UpdatableAttribute';
	global.Serenity.ComponentModel.UpdatableAttribute = $Serenity_ComponentModel_UpdatableAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Reporting.ReportDesignPanel
	var $Serenity_Reporting_ReportDesignPanel = function(element, options) {
		this.$5$ReportKeyField = null;
		ss.makeGenericType($Serenity_TemplatedWidget$1, [Object]).call(this, element, options);
		element.addClass('s-ReportDesignPanel');
		this.byId$1('AddButton').click(ss.mkdel(this, this.addButtonClick));
		this.byId$1('EditButton').click(ss.mkdel(this, this.editButtonClick));
		this.get_element().find('a').toggle(false);
		//Utils.HasRight("ReportDesign"));
	};
	$Serenity_Reporting_ReportDesignPanel.__typeName = 'Serenity.Reporting.ReportDesignPanel';
	global.Serenity.Reporting.ReportDesignPanel = $Serenity_Reporting_ReportDesignPanel;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Reporting.ReportDialog
	var $Serenity_Reporting_ReportDialog = function(opt) {
		this.$propertyItems = null;
		this.$propertyGrid = null;
		this.$reportKey = null;
		ss.makeGenericType($Serenity_TemplatedDialog$1, [Object]).$ctor1.call(this, opt);
		if (ss.isValue(opt.reportKey)) {
			this.loadReport(opt.reportKey);
		}
	};
	$Serenity_Reporting_ReportDialog.__typeName = 'Serenity.Reporting.ReportDialog';
	global.Serenity.Reporting.ReportDialog = $Serenity_Reporting_ReportDialog;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Reporting.ReportPage
	var $Serenity_Reporting_ReportPage = function(div) {
		$Serenity_Widget.call(this, div);
		$('.report-link').click(ss.mkdel(this, this.$reportLinkClick));
		$('div.line').click(ss.mkdel(this, this.$categoryClick));
		var self = this;
		var $t2 = $('#QuickSearchInput');
		var $t1 = $Serenity_QuickSearchInputOptions.$ctor();
		$t1.onSearch = function(field, text) {
			self.$updateMatchFlags(text);
		};
		new $Serenity_QuickSearchInput($t2, $t1);
	};
	$Serenity_Reporting_ReportPage.__typeName = 'Serenity.Reporting.ReportPage';
	global.Serenity.Reporting.ReportPage = $Serenity_Reporting_ReportPage;
	////////////////////////////////////////////////////////////////////////////////
	// System.ComponentModel.DisplayNameAttribute
	var $System_ComponentModel_DisplayNameAttribute = function(displayName) {
		this.$2$DisplayNameField = null;
		this.set_displayName(displayName);
	};
	$System_ComponentModel_DisplayNameAttribute.__typeName = 'System.ComponentModel.DisplayNameAttribute';
	global.System.ComponentModel.DisplayNameAttribute = $System_ComponentModel_DisplayNameAttribute;
	ss.initClass($Serenity_Widget, $asm, {
		isAsyncWidget: function() {
			return ss.isValue(ss.getMembers(ss.getInstanceType(this), 8, 8 | 256, 'createAsync'));
		},
		destroy: function() {
			this.element.removeClass('s-' + ss.getTypeName(ss.getInstanceType(this)));
			this.element.unbind('.' + this.widgetName).removeData(this.widgetName);
			this.element = null;
		},
		addCssClass: function() {
			this.element.addClass('s-' + ss.getTypeName(ss.getInstanceType(this)));
		},
		onInit: function() {
		},
		get_element: function() {
			return this.element;
		},
		get_uniqueName: function() {
			return this.uniqueName;
		},
		get_widgetName: function() {
			return this.widgetName;
		}
	}, Serenity.ScriptContext);
	ss.initInterface($Serenity_IBooleanValue, $asm, { get_value: null, set_value: null });
	ss.initClass($Serenity_BooleanEditor, $asm, {
		get_value: function() {
			return this.element.is(':checked');
		},
		set_value: function(value) {
			this.get_element().prop('checked', !!value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]), [$Serenity_IBooleanValue]);
	ss.initInterface($Serenity_IGetEditValue, $asm, { getEditValue: null });
	ss.initInterface($Serenity_ISetEditValue, $asm, { setEditValue: null });
	ss.initClass($Serenity_CheckListEditor, $asm, {
		getItems: function() {
			return this.options.items || [];
		},
		updateItems: function() {
			var items = this.getItems();
			//ClearItems();
			//
			//if (items.Count > 0)
			//{
			//bool isStrings = Script.TypeOf(items[0]) == "string";
			//
			//foreach (dynamic item in items)
			//{
			//string key = isStrings ? item : item[0];
			//string text = isStrings ? item : item[1] ?? item[0];
			//AddItem(key, text);
			//}
			//}
		},
		getEditValue: function(property, target) {
		},
		setEditValue: function(source, property) {
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_CheckListEditorOptions]), [$Serenity_IGetEditValue, $Serenity_ISetEditValue]);
	ss.initClass($Serenity_CheckListEditorOptions, $asm, {});
	ss.initInterface($Serenity_IDataGrid, $asm, {});
	ss.initClass($Serenity_CustomValidation, $asm, {});
	ss.initInterface($Serenity_IStringValue, $asm, { get_value: null, set_value: null });
	ss.initInterface($Serenity_IReadOnly, $asm, { get_readOnly: null, set_readOnly: null });
	ss.initClass($Serenity_DateEditor, $asm, {
		get_value: function() {
			var value = this.element.val().trim();
			if (ss.isValue(value) && value.length === 0) {
				return null;
			}
			return Q.formatDate(Q$Externals.parseDate(value), 'yyyy-MM-dd');
		},
		set_value: function(value) {
			if (ss.isNullOrUndefined(value)) {
				this.element.val('');
			}
			else if (value === 'Today') {
				this.element.val(Q.formatDate(ss.today(), null));
			}
			else {
				this.element.val(Q.formatDate(Q$Externals.parseISODateTime(value), null));
			}
		},
		get_readOnly: function() {
			return this.element.hasClass('readonly');
		},
		set_readOnly: function(value) {
			if (value !== this.get_readOnly()) {
				if (value) {
					this.element.addClass('readonly').attr('readonly', 'readonly');
					this.element.nextAll('.ui-datepicker-trigger').css('opacity', '0.1');
				}
				else {
					this.element.removeClass('readonly').removeAttr('readonly');
					this.element.nextAll('.ui-datepicker-trigger').css('opacity', '1');
				}
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]), [$Serenity_IStringValue, $Serenity_IReadOnly]);
	ss.initClass($Serenity_DateTimeEditor, $asm, {
		get_value: function() {
			var value = this.element.val().trim();
			if (ss.isValue(value) && value.length === 0) {
				return null;
			}
			var datePart = Q.formatDate(Q$Externals.parseDate(value), 'yyyy-MM-dd');
			var timePart = this.$time.val();
			return datePart + 'T' + timePart + ':00.000';
		},
		set_value: function(value) {
			if (ss.isNullOrUndefined(value)) {
				this.element.val('');
				this.$time.val('00:00');
			}
			else if (value === 'Today') {
				this.element.val(Q.formatDate(ss.today(), null));
				this.$time.val('00:00');
			}
			else {
				var val = Q$Externals.parseISODateTime(value);
				this.element.val(Q.formatDate(val, null));
				this.$time.val(Q.formatDate(val, 'HH:mm'));
			}
		}
	}, $Serenity_Widget, [$Serenity_IStringValue]);
	ss.initClass($Serenity_SelectEditor, $asm, {
		getItems: function() {
			return this.options.items || [];
		},
		emptyItemText: function() {
			return this.options.emptyOptionText;
		},
		updateItems: function() {
			var items = this.getItems();
			this.clearItems();
			if (items.length > 0) {
				var isStrings = typeof(items[0]) === 'string';
				for (var $t1 = 0; $t1 < items.length; $t1++) {
					var item = items[$t1];
					var key = ss.cast((isStrings ? item : item[0]), String);
					var $t3;
					if (isStrings) {
						$t3 = item;
					}
					else {
						var $t2 = item[1];
						if (ss.isNullOrUndefined($t2)) {
							$t2 = item[0];
						}
						$t3 = $t2;
					}
					var text = ss.cast($t3, String);
					this.addItem(key, text, item, false);
				}
			}
		}
	}, ss.makeGenericType($Serenity_Select2Editor$2, [$Serenity_SelectEditorOptions, Object]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_DateYearEditor, $asm, {
		getItems: function() {
			var opt = this.options;
			if (ss.isValue(opt.items) && opt.items.length >= 1) {
				return opt.items;
			}
			var years = [];
			var minYear = (new Date()).getFullYear();
			var maxYear = (new Date()).getFullYear();
			if (ss.isValue(opt.minYear)) {
				opt.minYear = opt.minYear.toString();
				if (ss.startsWithString(opt.minYear, '-')) {
					minYear -= parseInt(opt.minYear.substr(1), 10);
				}
				else if (ss.startsWithString(opt.minYear, '+')) {
					minYear += parseInt(opt.minYear.substr(1), 10);
				}
				else {
					minYear = parseInt(opt.minYear, 10);
				}
			}
			if (ss.isValue(opt.maxYear)) {
				opt.maxYear = opt.maxYear.toString();
				if (ss.startsWithString(opt.maxYear, '-')) {
					maxYear -= parseInt(opt.maxYear.substr(1), 10);
				}
				else if (ss.startsWithString(opt.maxYear, '+')) {
					maxYear += parseInt(opt.maxYear.substr(1), 10);
				}
				else {
					maxYear = parseInt(opt.maxYear, 10);
				}
			}
			if (opt.descending) {
				for (var i = maxYear; i >= minYear; i--) {
					ss.add(years, i.toString());
				}
			}
			else {
				for (var i1 = minYear; i1 <= maxYear; i1++) {
					ss.add(years, i1.toString());
				}
			}
			return years;
		},
		emptyItemText: function() {
			return this.options.emptyOptionText;
		}
	}, $Serenity_SelectEditor, [$Serenity_IStringValue]);
	ss.initClass($Serenity_SelectEditorOptions, $asm, {});
	ss.initClass($Serenity_DateYearEditorOptions, $asm, {}, $Serenity_SelectEditorOptions);
	ss.initInterface($Serenity_IDoubleValue, $asm, { get_value: null, set_value: null });
	ss.initClass($Serenity_DecimalEditor, $asm, {
		get_value: function() {
			var val = this.element.autoNumeric('get');
			if (!!(ss.isNullOrUndefined(val) || ss.referenceEquals(val.Length, 0))) {
				return null;
			}
			else {
				return ss.cast(parseFloat(ss.cast(val, String)), Number);
			}
		},
		set_value: function(value) {
			if (ss.isNullOrUndefined(value) || value === '') {
				this.element.val('');
			}
			else {
				this.element.autoNumeric('set', value);
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_DecimalEditorOptions]), [$Serenity_IDoubleValue]);
	ss.initClass($Serenity_DecimalEditorOptions, $asm, {});
	ss.initClass($Serenity_DialogExtensions, $asm, {});
	ss.initClass($Serenity_EditorTypeCache, $asm, {});
	ss.initClass($Serenity_EditorTypeEditor, $asm, {
		getItems: function() {
			if (ss.isNullOrUndefined($Serenity_EditorTypeEditor.$editorTypeList)) {
				$Serenity_EditorTypeEditor.$editorTypeList = [];
				var $t1 = new ss.ObjectEnumerator($Serenity_EditorTypeCache.get_registeredTypes());
				try {
					while ($t1.moveNext()) {
						var info = $t1.current();
						ss.add($Serenity_EditorTypeEditor.$editorTypeList, [info.key, info.value.displayName]);
					}
				}
				finally {
					$t1.dispose();
				}
				$Serenity_EditorTypeEditor.$editorTypeList.sort(function(x, y) {
					var xn = x[1];
					var yn = y[1];
					return Q$Externals.turkishLocaleCompare(xn, yn);
				});
			}
			return $Serenity_EditorTypeEditor.$editorTypeList;
		}
	}, $Serenity_SelectEditor, [$Serenity_IStringValue]);
	ss.initClass($Serenity_EmailEditor, $asm, {
		get_value: function() {
			var domain = this.element.nextAll('.emaildomain');
			var value = this.element.val();
			var domainValue = domain.val();
			if (Q.isEmptyOrNull(value)) {
				if (this.options.readOnlyDomain || Q.isEmptyOrNull(domainValue)) {
					return '';
				}
				return '@' + domainValue;
			}
			return value + '@' + domainValue;
		},
		set_value: function(value) {
			var domain = this.element.nextAll('.emaildomain');
			value = Q.trimToNull(value);
			if (ss.isNullOrUndefined(value)) {
				if (!this.options.readOnlyDomain) {
					domain.val('');
				}
				this.element.val('');
			}
			else {
				var parts = value.split('@');
				if (parts.length > 1) {
					if (!this.options.readOnlyDomain) {
						domain.val(parts[1]);
						this.element.val(parts[0]);
					}
					else if (!Q.isEmptyOrNull(this.options.domain)) {
						if (!ss.referenceEquals(parts[1], this.options.domain)) {
							this.element.val(value);
						}
						else {
							this.element.val(parts[0]);
						}
					}
					else {
						this.element.val(parts[0]);
					}
				}
				else {
					this.element.val(parts[0]);
				}
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_EmailEditorOptions]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_EmailEditorOptions, $asm, {});
	ss.initInterface($Serenity_IDialog, $asm, {});
	ss.initClass($Serenity_FilterField, $asm, {});
	ss.initClass($Serenity_FilterLine, $asm, {});
	ss.initClass($Serenity_FilterPanel, $asm, {
		$initFieldByName: function() {
			if (ss.isValue(this.options.fields)) {
				for (var $t1 = 0; $t1 < this.options.fields.length; $t1++) {
					var field = this.options.fields[$t1];
					this.$fieldByName[field.name] = field;
				}
			}
		},
		$initButtons: function() {
			this.byId$1('AddButton').text(Q.text('Controls.FilterPanel.AddFilter')).click(ss.mkdel(this, this.$addButtonClick));
			this.byId$1('SearchButton').text(Q.text('Controls.FilterPanel.SearchButton')).click(ss.mkdel(this, this.$searchButtonClick));
			this.byId$1('ResetButton').text(Q.text('Controls.FilterPanel.ResetButton')).click(ss.mkdel(this, this.$resetButtonClick));
		},
		$bindSearchToEnterKey: function() {
			this.get_element().bind('keypress', ss.mkdel(this, function(e) {
				if (e.which !== 13) {
					return;
				}
				if (this.$rowsDiv.children().length === 0) {
					return;
				}
				this.$search();
			}));
		},
		$addButtonClick: function(e) {
			e.preventDefault();
			this.$addEmptyRow();
		},
		$findEmptyRow: function() {
			var result = null;
			this.$rowsDiv.children().each(function(index, row) {
				var fieldSelect = $(row).children('div.f').children('select');
				if (fieldSelect.length === 0) {
					return true;
				}
				var val = fieldSelect.val();
				if (ss.isNullOrUndefined(val) || val.length === 0) {
					result = $(row);
					return false;
				}
				return true;
			});
			return result;
		},
		$addEmptyRow: function() {
			var emptyRow = this.$findEmptyRow();
			if (ss.isValue(emptyRow)) {
				return emptyRow;
			}
			var isLastRowOr = this.$rowsDiv.children().last().children('a.andor').hasClass('or');
			var row = $($Serenity_FilterPanel.rowTemplate).appendTo(this.$rowsDiv);
			var parenDiv = row.children('div.l').hide();
			parenDiv.children('a.leftparen, a.rightparen').click(ss.mkdel(this, this.$leftRightParenClick));
			var andor = parenDiv.children('a.andor').attr('title', Q.text('Controls.FilterPanel.ChangeAndOr'));
			if (isLastRowOr) {
				andor.addClass('or').text(Q.text('Controls.FilterPanel.Or'));
			}
			else {
				andor.text(Q.text('Controls.FilterPanel.And'));
			}
			andor.click(ss.mkdel(this, this.$andOrClick));
			row.children('a.delete').attr('title', Q.text('Controls.FilterPanel.RemoveField')).click(ss.mkdel(this, this.$deleteRowClick));
			var fieldSel = row.children('div.f').children('select');
			fieldSel.change(ss.mkdel(this, this.$onRowFieldChange));
			this.$populateFieldList(fieldSel);
			this.$updateParens();
			this.$updateButtons();
			this.$onHeightChange();
			fieldSel.focus();
			return row;
		},
		$onRowFieldChange: function(e) {
			var row = $(e.target).closest('div.row');
			this.$rowFieldChange(row);
			var opSelect = row.children('div.o').children('select').focus();
			try {
				opSelect.focus();
			}
			catch ($t1) {
			}
		},
		$populateFieldList: function(select) {
			if (select.length === 0) {
				return;
			}
			var $t1 = select[0];
			var sel = ss.cast($t1, ss.isValue($t1) && (ss.isInstanceOfType($t1, Element) && $t1.tagName === 'SELECT'));
			Q.addOption(select, '', Q.text('Controls.FilterPanel.SelectField'));
			var fields = this.options.fields;
			if (ss.isValue(fields)) {
				for (var $t2 = 0; $t2 < fields.length; $t2++) {
					var field = fields[$t2];
					var $t4 = field.name;
					var $t3 = field.title;
					if (ss.isNullOrUndefined($t3)) {
						$t3 = field.name;
					}
					Q.addOption(select, $t4, $t3);
				}
			}
		},
		$removeFilterHandler: function(row) {
			row.data('FilterHandler', null);
			row.data('FilterHandlerField', null);
		},
		$getFieldFor: function(row) {
			if (row.length === 0) {
				return null;
			}
			var fieldName = row.children('div.f').children('select').val();
			if (ss.isNullOrUndefined(fieldName) || fieldName === '') {
				return null;
			}
			var field = this.$fieldByName[fieldName];
			return field;
		},
		$getFilterHandlerFor: function(row) {
			var field = this.$getFieldFor(row);
			if (ss.isNullOrUndefined(field)) {
				return null;
			}
			var handler = ss.cast(row.data('FilterHandler'), $Serenity_IFilterHandler);
			var handlerField = row.data('FilterHandlerField');
			if (ss.isValue(handler)) {
				if (!ss.referenceEquals(handlerField, field.name)) {
					row.data('FilterHandler', null);
					handler = null;
				}
				else {
					return handler;
				}
			}
			var handlerType = ss.getType('Sinerji.' + ss.coalesce(field.handler, '??') + 'FilterHandler');
			if (ss.isNullOrUndefined(handlerType)) {
				throw new ss.Exception(ss.formatString('FilterHandler type Sinerji.{0}FilterHandler is not defined!', field.handler));
			}
			var editorDiv = row.children('div.v');
			handler = ss.cast(new handlerType(editorDiv, field), $Serenity_IFilterHandler);
			return handler;
		},
		$populateOperatorList: function(select) {
			var row = select.closest('div.row');
			var handler = this.$getFilterHandlerFor(row);
			if (ss.isNullOrUndefined(handler)) {
				return;
			}
			var operators = handler.getOperators();
			var $t1 = select[0];
			var sel = ss.cast($t1, ss.isValue($t1) && (ss.isInstanceOfType($t1, Element) && $t1.tagName === 'SELECT'));
			if (ss.isValue(operators)) {
				for (var $t2 = 0; $t2 < operators.length; $t2++) {
					var op = operators[$t2];
					Q.addOption(select, op, handler.operatorTitle(op));
				}
			}
		},
		$onRowOperatorChange: function(e) {
			var row = $(e.target).closest('div.row');
			this.$rowOperatorChange(row);
			var firstInput = row.children('div.v').find(':input:visible').first();
			try {
				firstInput.focus();
			}
			catch ($t1) {
			}
			;
		},
		$rowFieldChange: function(row) {
			var select = row.children('div.f').children('select');
			var fieldName = select.val();
			var isEmpty = ss.isNullOrUndefined(fieldName) || fieldName === '';
			// if a field is selected and first option is "---please select---", remove it
			if (!isEmpty) {
				var $t1 = select.children('option').first()[0];
				var firstOption = ss.cast($t1, ss.isValue($t1) && (ss.isInstanceOfType($t1, Element) && $t1.tagName === 'OPTION'));
				if (ss.isNullOrUndefined(firstOption.value) || firstOption.value === '') {
					$(firstOption).remove();
				}
			}
			var opDiv = row.children('div.o');
			var opSelect = opDiv.children('select');
			if (opSelect.length === 0) {
				opSelect = $('<select/>').appendTo(opDiv).change(ss.mkdel(this, this.$onRowOperatorChange));
			}
			else {
				Q.clearOptions(opSelect);
			}
			this.$removeFilterHandler(row);
			this.$populateOperatorList(opSelect);
			this.$rowOperatorChange(row);
			this.$updateParens();
			this.$updateButtons();
		},
		$rowOperatorChange: function(row) {
			if (row.length === 0) {
				return;
			}
			var editorDiv = row.children('div.v');
			editorDiv.html('');
			var handler = this.$getFilterHandlerFor(row);
			if (ss.isNullOrUndefined(handler)) {
				return;
			}
			var op = row.children('div.o').children('select').val();
			if (ss.isNullOrUndefined(op) || op === '') {
				return;
			}
			handler.createEditor(op);
		},
		$deleteRowClick: function(e) {
			e.preventDefault();
			var row = $(e.target).closest('div.row');
			row.remove();
			if (this.$rowsDiv.children().length === 0) {
				this.$search();
			}
			this.$updateParens();
			this.$updateButtons();
			this.$onHeightChange();
		},
		$andOrClick: function(e) {
			e.preventDefault();
			var andor = $(e.target).toggleClass('or');
			andor.text(Q.text('Controls.FilterPanel.' + (andor.hasClass('or') ? 'Or' : 'And')));
		},
		$leftRightParenClick: function(e) {
			e.preventDefault();
			$(e.target).toggleClass('active');
			this.$updateParens();
		},
		$updateParens: function() {
			var rows = this.$rowsDiv.children();
			if (rows.length === 0) {
				return;
			}
			rows.removeClass('paren-start');
			rows.removeClass('paren-end');
			rows.children('div.l').css('display', ((rows.length === 1) ? 'none' : 'block'));
			rows.first().children('div.l').children('a.rightparen, a.andor').css('visibility', 'hidden');
			for (var i = 1; i < rows.length; i++) {
				var row = rows.eq(i);
				row.children('div.l').css('display', 'block').children('a.lefparen, a.andor').css('visibility', 'visible');
			}
			var inParen = false;
			for (var i1 = 0; i1 < rows.length; i1++) {
				var row1 = rows.eq(i1);
				var divParen = row1.children('div.l');
				var lp = divParen.children('a.leftparen');
				var rp = divParen.children('a.rightparen');
				if (rp.hasClass('active') && inParen) {
					inParen = false;
					if (i1 > 0) {
						rows.eq(i1 - 1).addClass('paren-end');
					}
				}
				if (lp.hasClass('active')) {
					inParen = true;
					if (i1 > 0) {
						row1.addClass('paren-start');
					}
				}
			}
		},
		$updateButtons: function() {
			this.byId$1('SearchButton').toggle(this.$rowsDiv.children().length >= 1);
			this.byId$1('ResetButton').toggle(this.$rowsDiv.children().length >= 1 || ss.isValue(this.$currentFilter));
		},
		$onHeightChange: function() {
			//if (Options.HeightChange != null)
			//    Options.HeightChange();
		},
		$searchButtonClick: function(e) {
			e.preventDefault();
			this.$search();
		},
		$search: function() {
			var filterLines = [];
			var filterText = '';
			var errorText = null;
			var row = null;
			this.$rowsDiv.children().children('div.v').children('span.error').remove();
			var inParens = false;
			for (var i = 0; i < this.$rowsDiv.children().length; i++) {
				row = this.$rowsDiv.children().eq(i);
				var handler = this.$getFilterHandlerFor(row);
				if (ss.isNullOrUndefined(handler)) {
					continue;
				}
				var field = this.$getFieldFor(row);
				var op = row.children('div.o').children('select').val();
				if (ss.isNullOrUndefined(op) || op.length === 0) {
					errorText = Q.text('Controls.FilterPanel.InvalidOperator');
					break;
				}
				var lineEx = new $Serenity_FilterLine();
				lineEx.field = field.name;
				var $t1 = field.title;
				if (ss.isNullOrUndefined($t1)) {
					$t1 = field.name;
				}
				lineEx.title = $t1;
				lineEx.operator = op;
				lineEx.isOr = row.children('div.l').children('a.andor').hasClass('or');
				lineEx.leftParen = row.children('div.l').children('a.leftparen').hasClass('active');
				lineEx.rightParen = row.children('div.l').children('a.rightparen').hasClass('active');
				handler.toFilterLine(lineEx);
				if (ss.isValue(lineEx.validationError)) {
					errorText = lineEx.validationError;
					break;
				}
				var line = new $Serenity_FilterLine();
				line.field = lineEx.field;
				line.operator = lineEx.operator;
				if (ss.isValue(lineEx.value)) {
					line.value = lineEx.value;
				}
				if (ss.isValue(lineEx.value2)) {
					line.value2 = lineEx.value2;
				}
				if (ss.isValue(lineEx.values) && lineEx.values.length > 0) {
					line.values = lineEx.values;
				}
				if (lineEx.leftParen) {
					line.leftParen = 1;
				}
				if (lineEx.rightParen) {
					line.rightParen = 1;
				}
				if (lineEx.isOr) {
					line.isOr = 1;
				}
				ss.add(filterLines, line);
				if (inParens && (lineEx.rightParen || lineEx.leftParen)) {
					filterText += ')';
					inParens = false;
				}
				if (filterText.length > 0) {
					filterText += ' ' + Q.text('Controls.FilterPanel.' + (lineEx.isOr ? 'Or' : 'And')) + ' ';
				}
				if (lineEx.leftParen) {
					filterText += '(';
					inParens = true;
				}
				filterText += lineEx.displayText;
			}
			// if an error occured, display it, otherwise set current filters
			if (ss.isValue(errorText)) {
				$('<span/>').addClass('error').text(errorText).appendTo(row.children('div.v'));
				row.children('div.v').find('input:first').focus();
				return;
			}
			if (filterLines.length === 0) {
				this.$setCurrentFilter(null, null);
			}
			else {
				this.$setCurrentFilter(filterLines, filterText);
			}
		},
		$resetButtonClick: function(e) {
			e.preventDefault();
			this.$rowsDiv.empty();
			this.$setCurrentFilter(null, null);
			this.$updateParens();
			this.$updateButtons();
			this.$onHeightChange();
		},
		$setCurrentFilter: function(value, text) {
			if (!ss.referenceEquals($.toJSON(value), $.toJSON(this.$currentFilter))) {
				if (ss.isValue(value)) {
					this.$currentFilter = value;
					text = ss.formatString(Q.text('Controls.FilterPanel.CurrentFilter'), text);
					this.byId$1('DisplayText').text(text).show();
				}
				else {
					this.$currentFilter = null;
					this.byId$1('DisplayText').text('').hide();
				}
				this.$updateParens();
				this.$updateButtons();
				this.onFilterChange();
				this.$onHeightChange();
			}
		},
		get_currentFilter: function() {
			return this.$currentFilter;
		},
		getTemplateAsync: function() {
			var $state = 0, $tcs = new ss.TaskCompletionSource(), $t1;
			var $sm = function() {
				try {
					$sm1:
					for (;;) {
						switch ($state) {
							case 0: {
								$state = -1;
								$t1 = ss.Task.fromResult($Serenity_FilterPanel.panelTemplate);
								$state = 1;
								$t1.continueWith($sm);
								return;
							}
							case 1: {
								$state = -1;
								$tcs.setResult($t1.getAwaitedResult());
								return;
							}
							default: {
								break $sm1;
							}
						}
					}
				}
				catch ($t2) {
					$tcs.setException(ss.Exception.wrap($t2));
				}
			};
			$sm();
			return $tcs.task;
		},
		onFilterChange: function() {
			if (!ss.staticEquals(this.options.filterChange, null)) {
				this.options.filterChange(this.$currentFilter);
			}
		}
	}, ss.makeGenericType($Serenity_TemplatedWidget$1, [$Serenity_FilterPanelOptions]));
	ss.initClass($Serenity_FilterPanelOptions, $asm, {});
	ss.initClass($Serenity_Flexify, $asm, {
		$storeInitialSize: function() {
			if (!!this.element.data('flexify-init')) {
				return;
			}
			var $t2 = this.element;
			var $t1 = this.options.designWidth;
			if (ss.isNullOrUndefined($t1)) {
				$t1 = this.element.width();
			}
			$t2.data('flexify-width', $t1);
			var $t4 = this.element;
			var $t3 = this.options.designHeight;
			if (ss.isNullOrUndefined($t3)) {
				$t3 = this.element.height();
			}
			$t4.data('flexify-height', $t3);
			this.element.data('flexify-init', true);
			var self = this;
			this.element.bind('resize.' + this.uniqueName, function(e, ui) {
				self.$resizeElements();
			});
			this.element.bind('resizestop.' + this.uniqueName, function(e1, ui1) {
				self.$resizeElements();
			});
			var tabs = this.element.find('.ui-tabs');
			if (tabs.length > 0) {
				tabs.bind('tabsactivate.' + this.uniqueName, function() {
					self.$resizeElements();
				});
			}
			if (ss.isValue(this.options.designWidth) || ss.isValue(this.options.designHeight)) {
				self.$resizeElements();
			}
		},
		$getXFactor: function(element) {
			var xFactor = null;
			if (!ss.staticEquals(this.options.getXFactor, null)) {
				xFactor = this.options.getXFactor(element);
			}
			if (ss.isNullOrUndefined(xFactor)) {
				xFactor = element.data('flex-x');
			}
			return ss.coalesce(xFactor, 1);
		},
		$getYFactor: function(element) {
			var yFactor = null;
			if (!ss.staticEquals(this.options.getYFactor, null)) {
				yFactor = this.options.getYFactor(element);
			}
			if (ss.isNullOrUndefined(yFactor)) {
				yFactor = element.data('flex-y');
			}
			return ss.coalesce(yFactor, 0);
		},
		$resizeElements: function() {
			var width = this.element.width();
			var initialWidth = this.element.data('flexify-width');
			if (ss.isNullOrUndefined(initialWidth)) {
				this.element.data('flexify-width', width);
				initialWidth = width;
			}
			var height = this.element.height();
			var initialHeight = this.element.data('flexify-height');
			if (ss.isNullOrUndefined(initialHeight)) {
				this.element.data('flexify-height', height);
				initialHeight = height;
			}
			this.$xDifference = width - ss.unbox(initialWidth);
			this.$yDifference = height - ss.unbox(initialHeight);
			var self = this;
			var containers = this.element;
			var tabPanels = this.element.find('.ui-tabs-panel');
			if (tabPanels.length > 0) {
				containers = tabPanels.filter(':visible');
			}
			containers.find('.flexify').add(tabPanels.filter('.flexify:visible')).each(function(i, e) {
				self.$resizeElement($(e));
			});
		},
		$resizeElement: function(element) {
			var xFactor = this.$getXFactor(element);
			if (xFactor !== 0) {
				var initialWidth = element.data('flexify-width');
				if (ss.isNullOrUndefined(initialWidth)) {
					var width = element.width();
					element.data('flexify-width', width);
					initialWidth = width;
				}
				element.width(ss.unbox(initialWidth) + xFactor * this.$xDifference | 0);
			}
			var yFactor = this.$getYFactor(element);
			if (yFactor !== 0) {
				var initialHeight = element.data('flexify-height');
				if (ss.isNullOrUndefined(initialHeight)) {
					var height = element.height();
					element.data('flexify-height', height);
					initialHeight = height;
				}
				element.height(ss.unbox(initialHeight) + yFactor * this.$yDifference | 0);
			}
			if (element.hasClass('require-layout')) {
				element.triggerHandler('layout');
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]));
	ss.initClass($Serenity_FLX, $asm, {});
	ss.initClass($Serenity_GridSelectAllButtonHelper, $asm, {});
	ss.initClass($Serenity_GridUtils, $asm, {});
	ss.initClass($Serenity_HtmlContentEditor, $asm, {
		instanceReady: function(x) {
			this.$instanceReady = true;
			$(x.editor.container.$).addClass(this.element.attr('class'));
			this.element.addClass('select2-offscreen').css('display', 'block');
			// validasyonun çalışması için
			x.editor.setData(this.element.val());
		},
		getConfig: function() {
			var self = this;
			return { customConfig: '', language: 'tr', bodyClass: 's-HtmlContentBody', on: {
				instanceReady: function(x) {
					self.instanceReady(x);
				},
				change: function(x1) {
					x1.editor.updateElement();
					self.get_element().triggerHandler('change');
				}
			}, toolbarGroups: [{ name: 'clipboard', groups: ['clipboard', 'undo'] }, { name: 'editing', groups: ['find', 'selection', 'spellchecker'] }, { name: 'insert', groups: ['links', 'insert', 'blocks', 'bidi', 'list', 'indent'] }, { name: 'forms', groups: ['forms', 'mode', 'document', 'doctools', 'others', 'about', 'tools'] }, { name: 'colors' }, { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] }, { name: 'align' }, { name: 'styles' }], removeButtons: 'SpecialChar,Anchor,Subscript,Styles', format_tags: 'p;h1;h2;h3;pre', removeDialogTabs: 'image:advanced;link:advanced', contentsCss: Q.resolveUrl('~/content/site/site.htmlcontent.css'), entities: false, entities_latin: false, entities_greek: false, autoUpdateElement: true, height: ((ss.isNullOrUndefined(this.options.rows) || ss.referenceEquals(this.options.rows, 0)) ? null : (ss.Nullable$1.mul(this.options.rows, 20) + 'px')) };
		},
		$getEditorInstance: function() {
			var id = this.element.attr('id');
			return CKEDITOR.instances[id];
		},
		destroy: function() {
			var instance = this.$getEditorInstance();
			if (ss.isValue(instance)) {
				instance.destroy();
			}
			$Serenity_Widget.prototype.destroy.call(this);
		},
		get_value: function() {
			var instance = this.$getEditorInstance();
			if (this.$instanceReady && ss.isValue(instance)) {
				return instance.getData();
			}
			else {
				return this.element.val();
			}
		},
		set_value: function(value) {
			var instance = this.$getEditorInstance();
			if (this.$instanceReady && ss.isValue(instance)) {
				instance.setData(value);
			}
			else {
				this.element.val(value);
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_HtmlContentEditorOptions]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_HtmlContentEditorOptions, $asm, {});
	ss.initClass($Serenity_HtmlReportContentEditor, $asm, {
		getConfig: function() {
			var config = $Serenity_HtmlContentEditor.prototype.getConfig.call(this);
			config.removeButtons += ',Image,Table,HorizontalRule,Anchor,Blockquote,CreatePlaceholder,BGColor,JustifyLeft,JustifyCenter,JustifyRight,JustifyBlock,Superscript';
			return config;
		}
	}, $Serenity_HtmlContentEditor, [$Serenity_IStringValue]);
	ss.initInterface($Serenity_IFilterHandler, $asm, { getOperators: null, operatorTitle: null, operatorFormat: null, createEditor: null, toFilterLine: null });
	ss.initClass($Serenity_ImageUploadEditor, $asm, {
		getToolButtons: function() {
			var self = this;
			var $t1 = [];
			ss.add($t1, {
				title: 'Dosya Seç',
				cssClass: 'add-file-button',
				onClick: function() {
				}
			});
			null;
			ss.add($t1, {
				title: 'Kaldır',
				cssClass: 'delete-button',
				onClick: function() {
					self.entity = null;
					self.populate();
					self.updateInterface();
				}
			});
			null;
			return $t1;
		},
		populate: function() {
			var displayOriginalName = !Q.isTrimmedEmpty(this.options.originalNameProperty);
			if (ss.isNullOrUndefined(this.entity)) {
				$Serenity_UploadHelper.populateFileSymbols(this.fileSymbols, null, displayOriginalName);
			}
			else {
				var $t2 = this.fileSymbols;
				var $t1 = [];
				ss.add($t1, this.entity);
				null;
				$Serenity_UploadHelper.populateFileSymbols($t2, $t1, displayOriginalName);
			}
		},
		updateInterface: function() {
			var addButton = this.toolbar.findButton('add-file-button');
			var delButton = this.toolbar.findButton('delete-button');
			addButton.toggleClass('disabled', this.get_readOnly());
			delButton.toggleClass('disabled', this.get_readOnly() || ss.isNullOrUndefined(this.entity));
		},
		get_readOnly: function() {
			return !ss.isNullOrUndefined(this.uploadInput.attr('disabled'));
		},
		set_readOnly: function(value) {
			if (this.get_readOnly() !== value) {
				if (value) {
					this.uploadInput.attr('disabled', 'disabled');
				}
				else {
					this.uploadInput.removeAttr('disabled');
				}
				this.updateInterface();
			}
		},
		get_value: function() {
			if (ss.isNullOrUndefined(this.entity)) {
				return null;
			}
			var copy = $.extend(new $Serenity_UploadedFile(), this.entity);
			return copy;
		},
		set_value: function(value) {
			if (ss.isValue(value)) {
				if (ss.isNullOrUndefined(value.get_filename())) {
					this.entity = null;
				}
				else {
					this.entity = $.extend(new $Serenity_UploadedFile(), value);
				}
			}
			else {
				this.entity = null;
			}
			this.populate();
			this.updateInterface();
		},
		getEditValue: function(property, target) {
			target[property.name] = (ss.isNullOrUndefined(this.entity) ? null : Q.trimToNull(this.entity.get_filename()));
		},
		setEditValue: function(source, property) {
			var value = new $Serenity_UploadedFile();
			value.set_filename(ss.cast(source[property.name], String));
			value.set_originalName(ss.cast(source[this.options.originalNameProperty], String));
			this.set_value(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_ImageUploadEditorOptions]), [$Serenity_IGetEditValue, $Serenity_ISetEditValue, $Serenity_IReadOnly]);
	ss.initClass($Serenity_ImageUploadEditorOptions, $asm, {});
	ss.initClass($Serenity_IntegerEditor, $asm, {
		get_value$1: function() {
			var val = this.element.autoNumeric('get');
			if (!!Q.isTrimmedEmpty(ss.cast(val, String))) {
				return null;
			}
			else {
				return ss.cast(parseInt(ss.cast(val, String), 10), ss.Int32);
			}
		},
		set_value$1: function(value) {
			if (ss.isNullOrUndefined(value) || value === '') {
				this.element.val('');
			}
			else {
				this.element.autoNumeric('set', value);
			}
		},
		get_value: function() {
			return this.get_value$1();
		},
		set_value: function(value) {
			this.set_value$1(ss.Int32.trunc(value));
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_IntegerEditorOptions]), [$Serenity_IDoubleValue]);
	ss.initClass($Serenity_IntegerEditorOptions, $asm, {});
	ss.initInterface($Serenity_IValidateRequired, $asm, { get_required: null, set_required: null });
	ss.initClass($Serenity_JsRender, $asm, {});
	ss.initClass($Serenity_LookupEditor, $asm, {
		getLookupKey: function() {
			var $t1 = this.options.lookupKey;
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.makeGenericType($Serenity_LookupEditorBase$2, [$Serenity_LookupEditorOptions, Object]).prototype.getLookupKey.call(this);
			}
			return $t1;
		}
	}, ss.makeGenericType($Serenity_LookupEditorBase$2, [$Serenity_LookupEditorOptions, Object]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_LookupEditorOptions, $asm, {});
	ss.initClass($Serenity_MaskedEditor, $asm, {
		get_value: function() {
			this.element.triggerHandler('blur.mask');
			return this.element.val();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_MaskedEditorOptions]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_MaskedEditorOptions, $asm, {});
	ss.initClass($Serenity_PanelAttribute, $asm, {});
	ss.initClass($Serenity_StringEditor, $asm, {
		get_value: function() {
			return this.element.val();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_PasswordEditor, $asm, {}, $Serenity_StringEditor, [$Serenity_IStringValue]);
	ss.initClass($Serenity_PersonNameEditor, $asm, {
		get_value: function() {
			return this.element.val();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_PhoneEditor, $asm, {
		formatValue: function() {
			this.element.val(this.getFormattedValue());
		},
		getFormattedValue: function() {
			var value = this.element.val();
			if (!this.options.multiple && !this.options.internal && !this.options.mobile) {
				return $Serenity_PhoneEditor.$formatPhoneTurkey(value);
			}
			else if (this.options.multiple && !this.options.mobile && !this.options.internal) {
				return $Serenity_PhoneEditor.$formatPhoneTurkeyMulti(value);
			}
			else if (this.options.mobile && !this.options.multiple) {
				return $Serenity_PhoneEditor.$formatMobileTurkey(value);
			}
			else if (this.options.mobile && this.options.multiple) {
				return $Serenity_PhoneEditor.$formatMobileTurkeyMulti(value);
			}
			else if (this.options.internal && !this.options.multiple) {
				return $Serenity_PhoneEditor.$formatPhoneInternal(value);
			}
			else if (this.options.internal && this.options.multiple) {
				return $Serenity_PhoneEditor.$formatPhoneInternalMulti(value);
			}
			return value;
		},
		get_value: function() {
			return this.getFormattedValue();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_PhoneEditorOptions]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_PhoneEditorOptions, $asm, {});
	ss.initClass($Serenity_PopupMenuButton, $asm, {
		destroy: function() {
			if (ss.isValue(this.options.menu)) {
				this.options.menu.remove();
			}
			$Serenity_Widget.prototype.destroy.call(this);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]));
	ss.initClass($Serenity_PopupToolButton, $asm, {}, $Serenity_PopupMenuButton);
	ss.initClass($Serenity_PrefixedContext, $asm, {
		byId$1: function(id) {
			return $('#' + this.$idPrefix + id);
		},
		byId: function(TWidget) {
			return function(id) {
				return $Serenity_WX.getWidget(TWidget).call(null, this.byId$1(id));
			};
		},
		get_idPrefix: function() {
			return this.$idPrefix;
		}
	}, Serenity.ScriptContext);
	ss.initClass($Serenity_PropertyEditorHelper, $asm, {});
	ss.initClass($Serenity_PropertyGrid, $asm, {
		destroy: function() {
			if (ss.isValue(this.$editors)) {
				for (var i = 0; i < this.$editors.length; i++) {
					this.$editors[i].destroy();
				}
				this.$editors = null;
			}
			this.element.find('a.category-link').unbind('click', $Serenity_PropertyGrid.$categoryLinkClick).remove();
			$Serenity_Widget.prototype.destroy.call(this);
		},
		$createCategoryDiv: function(categoriesDiv, categoryIndexes, category) {
			var categoryDiv = $('<div/>').addClass('category').appendTo(categoriesDiv);
			$('<div/>').addClass('category-title').append($('<a/>').addClass('category-anchor').text(category).attr('name', this.options.idPrefix + 'Category' + categoryIndexes[category].toString())).appendTo(categoryDiv);
			return categoryDiv;
		},
		$determineText: function(name, text, suffix) {
			if (ss.isValue(text) && !ss.startsWithString(text, '`')) {
				var local = Q.tryGetText(text);
				if (ss.isValue(local)) {
					return local;
				}
			}
			if (ss.isValue(text) && ss.startsWithString(text, '`')) {
				text = text.substr(1);
			}
			if (!Q.isEmptyOrNull(this.options.localTextPrefix)) {
				var local1 = Q.tryGetText(this.options.localTextPrefix + name + suffix);
				if (ss.isValue(local1)) {
					return local1;
				}
			}
			return text;
		},
		$createField: function(container, item) {
			var fieldDiv = $('<div/>').addClass('field').addClass(item.name).data('PropertyItem', item).appendTo(container);
			if (!ss.isNullOrEmptyString(item.cssClass)) {
				fieldDiv.addClass(item.cssClass);
			}
			var editorId = this.options.idPrefix + item.name;
			var title = this.$determineText(item.name, item.title, '');
			var hint = this.$determineText(item.name, item.hint, 'Hint');
			var placeHolder = this.$determineText(item.name, item.placeholder, 'Placeholder');
			var $t2 = $('<label/>').addClass('caption').attr('for', editorId);
			var $t1 = hint;
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.coalesce(title, '');
			}
			var label = $t2.attr('title', $t1).html(ss.coalesce(title, '')).appendTo(fieldDiv);
			if (item.required) {
				$('<sup>*</sup>').attr('title', Texts$Controls$PropertyGrid.RequiredHint.get()).prependTo(label);
			}
			var editorType = $Serenity_PropertyGrid.$getEditorType(item.editorType);
			var elementAttr = ss.getAttributes(editorType, Serenity.ElementAttribute, true);
			var elementHtml = ((elementAttr.length > 0) ? elementAttr[0].get_html() : '<input/>');
			var element = $Serenity_WX.createElementFor$1(editorType).addClass('editor').addClass('flexify').attr('id', editorId).appendTo(fieldDiv);
			if (element.is(':input')) {
				element.attr('name', ss.coalesce(item.name, ''));
			}
			if (!Q.isEmptyOrNull(placeHolder)) {
				element.attr('placeholder', placeHolder);
			}
			var editorParams = item.editorParams;
			var optionsType = null;
			var optionsAttr = ss.getAttributes(editorType, Serenity.OptionsTypeAttribute, true);
			if (ss.isValue(optionsAttr) && optionsAttr.length > 0) {
				optionsType = optionsAttr[0].get_optionsType();
			}
			var editor;
			if (ss.isValue(optionsType)) {
				editorParams = $.extend(ss.createInstance(optionsType), item.editorParams);
				editor = ss.cast(new editorType(element, editorParams), $Serenity_Widget);
			}
			else {
				editorParams = $.extend(new Object(), item.editorParams);
				editor = ss.cast(new editorType(element, editorParams), $Serenity_Widget);
			}
			if (ss.isValue(item.editorParams)) {
				var props = ss.getMembers(ss.getInstanceType(editor), 16, 20);
				var propByName = Enumerable.from(props).where(function(x) {
					return !!x.setter && ((x.attr || []).filter(function(a) {
						return ss.isInstanceOfType(a, $Serenity_ComponentModel_OptionAttribute);
					}).length > 0 || (x.attr || []).filter(function(a) {
						return ss.isInstanceOfType(a, $System_ComponentModel_DisplayNameAttribute);
					}).length > 0);
				}).toDictionary(function(x1) {
					return $Serenity_ReflectionUtils.makeCamelCase(x1.name);
				}, null, String, Object);
				var $t3 = ss.getEnumerator(Object.keys(item.editorParams));
				try {
					while ($t3.moveNext()) {
						var k = $t3.current();
						var p = {};
						if (propByName.tryGetValue($Serenity_ReflectionUtils.makeCamelCase(k), p)) {
							ss.midel(p.$.setter, editor)(item.editorParams[k]);
						}
					}
				}
				finally {
					$t3.dispose();
				}
			}
			if (ss.isInstanceOfType(editor, $Serenity_BooleanEditor)) {
				label.removeAttr('for');
			}
			if (ss.isValue(item.maxLength)) {
				$Serenity_PropertyGrid.$setMaxLength(editor, ss.unbox(item.maxLength));
			}
			$('<div/>').addClass('vx').appendTo(fieldDiv);
			$('<div/>').addClass('clear').appendTo(fieldDiv);
			return editor;
		},
		$getCategoryOrder: function() {
			var $t1 = Q.trimToNull(this.options.categoryOrder);
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.coalesce(this.options.defaultCategory, '');
			}
			var split = $t1.split(';');
			var order = 0;
			var result = {};
			for (var $t2 = 0; $t2 < split.length; $t2++) {
				var s = split[$t2];
				var x = Q.trimToNull(s);
				if (ss.isNullOrUndefined(x)) {
					continue;
				}
				result[x] = order++;
			}
			return result;
		},
		$createCategoryLinks: function(container, items) {
			var idx = 0;
			var itemIndex = {};
			for (var $t1 = 0; $t1 < items.length; $t1++) {
				var x = items[$t1];
				var $t2 = x.category;
				if (ss.isNullOrUndefined($t2)) {
					$t2 = ss.coalesce(this.options.defaultCategory, '');
				}
				x.category = $t2;
				itemIndex[x.name] = idx++;
			}
			var self = this;
			var categoryOrder = null;
			items.sort(ss.mkdel(this, function(x1, y) {
				var c = 0;
				if (!ss.referenceEquals(x1.category, y.category)) {
					if (ss.isValue(categoryOrder) || ss.isValue(this.options.categoryOrder)) {
						categoryOrder = categoryOrder || this.$getCategoryOrder();
						var c1 = categoryOrder[x1.category];
						var c2 = categoryOrder[y.category];
						if (ss.isValue(c1) && ss.isValue(c2)) {
							c = ss.unbox(c1) - ss.unbox(c2);
						}
						else if (ss.isValue(c1)) {
							c = -1;
						}
						else if (ss.isValue(c2)) {
							c = 1;
						}
					}
				}
				if (c === 0) {
					c = ss.compareStrings(x1.category, y.category);
				}
				if (c === 0) {
					c = ss.compare(itemIndex[x1.name], itemIndex[y.name]);
				}
				return c;
			}));
			var categoryIndexes = {};
			for (var i = 0; i < items.length; i++) {
				var item = items[i];
				if (!ss.keyExists(categoryIndexes, item.category)) {
					var index = ss.getKeyCount(categoryIndexes) + 1;
					categoryIndexes[item.category] = index;
					if (index > 1) {
						$('<span/>').addClass('separator').text('|').prependTo(container);
					}
					$('<a/>').addClass('category-link').text(item.category).attr('tabindex', '-1').attr('href', '#' + this.options.idPrefix + 'Category' + index.toString()).click($Serenity_PropertyGrid.$categoryLinkClick).prependTo(container);
				}
			}
			$('<div/>').addClass('clear').appendTo(container);
			return categoryIndexes;
		},
		get_editors: function() {
			return this.$editors;
		},
		get_items: function() {
			return this.$items;
		},
		get_idPrefix: function() {
			return this.options.idPrefix;
		},
		get_mode: function() {
			return this.options.mode;
		},
		set_mode: function(value) {
			if (this.options.mode !== value) {
				this.options.mode = value;
				this.$updateReadOnly();
			}
		},
		load: function(source) {
			for (var i = 0; i < this.$editors.length; i++) {
				var item = this.$items[i];
				var editor = this.$editors[i];
				if (!!(this.get_mode() === 0 && !ss.isNullOrUndefined(item.defaultValue) && typeof(source[item.name]) === 'undefined')) {
					source[item.name] = item.defaultValue;
				}
				var setEditValue = ss.safeCast(editor, $Serenity_ISetEditValue);
				if (ss.isValue(setEditValue)) {
					setEditValue.setEditValue(source, item);
				}
				var stringValue = ss.safeCast(editor, $Serenity_IStringValue);
				if (ss.isValue(stringValue)) {
					var value = source[item.name];
					if (!!ss.isValue(value)) {
						value = value.toString();
					}
					stringValue.set_value(ss.cast(value, String));
				}
				else {
					var booleanValue = ss.safeCast(editor, $Serenity_IBooleanValue);
					if (ss.isValue(booleanValue)) {
						var value1 = source[item.name];
						if (typeof(value1) === 'number') {
							booleanValue.set_value(Serenity.IdExtensions.isPositiveId(value1));
						}
						else {
							booleanValue.set_value(!!value1);
						}
					}
					else {
						var doubleValue = ss.safeCast(editor, $Serenity_IDoubleValue);
						if (ss.isValue(doubleValue)) {
							var d = source[item.name];
							if (!!(ss.isNullOrUndefined(d) || ss.isInstanceOfType(d, String) && Q.isTrimmedEmpty(ss.cast(d, String)))) {
								doubleValue.set_value(null);
							}
							else if (ss.isInstanceOfType(d, String)) {
								doubleValue.set_value(ss.cast(Q.parseDecimal(ss.cast(d, String)), Number));
							}
							else if (ss.isInstanceOfType(d, Boolean)) {
								doubleValue.set_value((!!d ? 1 : 0));
							}
							else {
								doubleValue.set_value(ss.cast(d, Number));
							}
						}
						else if (editor.get_element().is(':input')) {
							var v = source[item.name];
							if (!!!ss.isValue(v)) {
								editor.get_element().val('');
							}
							else {
								editor.get_element().val(v);
							}
						}
					}
				}
			}
		},
		save: function(target) {
			for (var i = 0; i < this.$editors.length; i++) {
				var item = this.$items[i];
				if (!item.oneWay && !(this.get_mode() === 0 && item.insertable === false) && !(this.get_mode() === 1 && item.updatable === false)) {
					var editor = this.$editors[i];
					var getEditValue = ss.safeCast(editor, $Serenity_IGetEditValue);
					if (ss.isValue(getEditValue)) {
						getEditValue.getEditValue(item, target);
					}
					else {
						var stringValue = ss.safeCast(editor, $Serenity_IStringValue);
						if (ss.isValue(stringValue)) {
							target[item.name] = stringValue.get_value();
						}
						else {
							var booleanValue = ss.safeCast(editor, $Serenity_IBooleanValue);
							if (ss.isValue(booleanValue)) {
								target[item.name] = booleanValue.get_value();
							}
							else {
								var doubleValue = ss.safeCast(editor, $Serenity_IDoubleValue);
								if (ss.isValue(doubleValue)) {
									var value = doubleValue.get_value();
									target[item.name] = (isNaN(value) ? null : value);
								}
								else if (editor.get_element().is(':input')) {
									target[item.name] = editor.get_element().val();
								}
							}
						}
					}
				}
			}
		},
		$updateReadOnly: function() {
			for (var i = 0; i < this.$editors.length; i++) {
				var item = this.$items[i];
				var editor = this.$editors[i];
				var readOnly = item.readOnly || this.get_mode() === 0 && item.insertable === false || this.get_mode() === 1 && item.updatable === false;
				$Serenity_PropertyGrid.setReadOnly(editor, readOnly);
				$Serenity_PropertyGrid.setRequired(editor, !readOnly && item.required && item.editorType !== 'Boolean');
			}
		},
		enumerateItems: function(callback) {
			for (var i = 0; i < this.$editors.length; i++) {
				var item = this.$items[i];
				var editor = this.$editors[i];
				callback(item, editor);
			}
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_PropertyGridOptions]));
	ss.initEnum($Serenity_PropertyGridMode, $asm, { insert: 0, update: 1 });
	ss.initClass($Serenity_PropertyGridOptions, $asm, {});
	ss.initClass($Serenity_QuickSearchInput, $asm, {
		$updateInputPlaceHolder: function() {
			var qsf = this.element.prevAll('.quick-search-field');
			if (ss.isValue(this.$field)) {
				qsf.text(this.$field.title);
			}
			else {
				qsf.text('');
			}
		},
		checkIfValueChanged: function() {
			var value = Q.trim(ss.coalesce(this.element.val(), ''));
			if (ss.referenceEquals(value, this.$lastValue) && (!this.$fieldChanged || Q.isEmptyOrNull(value))) {
				this.$fieldChanged = false;
				return;
			}
			this.$fieldChanged = false;
			this.element.parent().toggleClass(ss.coalesce(this.options.filteredParentClass, ''), !!(value.length > 0));
			this.element.addClass(ss.coalesce(this.options.loadingParentClass, ''));
			if (!!this.$timer) {
				window.clearTimeout(this.$timer);
			}
			var self = this;
			this.$timer = window.setTimeout(ss.mkdel(this, function() {
				if (!ss.staticEquals(self.options.onSearch, null)) {
					self.options.onSearch(((ss.isValue(this.$field) && !Q.isEmptyOrNull(this.$field.name)) ? this.$field.name : null), value);
				}
				self.element.removeClass(ss.coalesce(self.options.loadingParentClass, ''));
			}), this.options.typeDelay);
			this.$lastValue = value;
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_QuickSearchInputOptions]));
	ss.initClass($Serenity_QuickSearchInputOptions, $asm, {});
	ss.initClass($Serenity_ReflectionUtils, $asm, {});
	ss.initClass($Serenity_StringInflector, $asm, {});
	ss.initClass($Serenity_SubDialogHelper, $asm, {});
	ss.initClass($Serenity_TemplatedDialog, $asm, {}, ss.makeGenericType($Serenity_TemplatedDialog$1, [Object]), [$Serenity_IDialog]);
	ss.initClass($Serenity_TemplatedWidget, $asm, {}, ss.makeGenericType($Serenity_TemplatedWidget$1, [Object]));
	ss.initClass($Serenity_TextAreaEditor, $asm, {
		get_value: function() {
			return this.element.val();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [$Serenity_TextAreaEditorOptions]), [$Serenity_IStringValue]);
	ss.initClass($Serenity_TextAreaEditorOptions, $asm, {});
	ss.initClass($Serenity_Toolbar, $asm, {
		$createButton: function(container, b) {
			var cssClass = ss.coalesce(b.cssClass, '');
			var btn = $('<div class="tool-button"><div class="button-outer"><span class="button-inner"></span></div></div>').appendTo(container);
			btn.addClass(cssClass);
			if (!Q.isEmptyOrNull(b.hint)) {
				btn.attr('title', b.hint);
			}
			btn.click(function(e) {
				if (btn.hasClass('disabled')) {
					return;
				}
				b.onClick(e);
			});
			var text = b.title;
			if (b.htmlEncode !== false) {
				text = Q.htmlEncode(b.title);
			}
			if (ss.isNullOrUndefined(text) || text.length === 0) {
				btn.addClass('no-text');
			}
			else {
				btn.find('span').html(text);
			}
		},
		destroy: function() {
			this.element.find('div.tool-button').unbind('click');
			$Serenity_Widget.prototype.destroy.call(this);
		},
		findButton: function(className) {
			if (ss.isValue(className) && ss.startsWithString(className, '.')) {
				className = className.substr(1);
			}
			return $('div.tool-button.' + className, this.element);
		}
	}, ss.makeGenericType($Serenity_Widget$1, [Object]));
	ss.initClass($Serenity_UploadedFile, $asm, {
		get_filename: function() {
			return this.$1$FilenameField;
		},
		set_filename: function(value) {
			this.$1$FilenameField = value;
		},
		get_originalName: function() {
			return this.$1$OriginalNameField;
		},
		set_originalName: function(value) {
			this.$1$OriginalNameField = value;
		}
	});
	ss.initClass($Serenity_UploadHelper, $asm, {});
	ss.initClass($Serenity_URLEditor, $asm, {}, $Serenity_StringEditor, [$Serenity_IStringValue]);
	ss.initClass($Serenity_ValidationHelper, $asm, {});
	ss.initClass($Serenity_WX, $asm, {});
	ss.initClass($Serenity_ComponentModel_CategoryAttribute, $asm, {
		get_category: function() {
			return this.$2$CategoryField;
		},
		set_category: function(value) {
			this.$2$CategoryField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_CssClassAttribute, $asm, {
		get_cssClass: function() {
			return this.$2$CssClassField;
		},
		set_cssClass: function(value) {
			this.$2$CssClassField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_DefaultValueAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_EditorOptionAttribute, $asm, {
		get_key: function() {
			return this.$2$KeyField;
		},
		set_key: function(value) {
			this.$2$KeyField = value;
		},
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_EditorTypeAttributeBase, $asm, {
		setParams: function(editorParams) {
		}
	});
	ss.initClass($Serenity_ComponentModel_EditorTypeAttribute, $asm, {}, $Serenity_ComponentModel_EditorTypeAttributeBase);
	ss.initClass($Serenity_ComponentModel_HiddenAttribute, $asm, {});
	ss.initClass($Serenity_ComponentModel_HintAttribute, $asm, {
		get_hint: function() {
			return this.$2$HintField;
		},
		set_hint: function(value) {
			this.$2$HintField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_InsertableAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_MaxLengthAttribute, $asm, {
		get_maxLength: function() {
			return this.$2$MaxLengthField;
		},
		set_maxLength: function(value) {
			this.$2$MaxLengthField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_OneWayAttribute, $asm, {});
	ss.initClass($Serenity_ComponentModel_OptionAttribute, $asm, {});
	ss.initClass($Serenity_ComponentModel_PlaceholderAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_ReadOnlyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_RequiredAttribute, $asm, {
		get_isRequired: function() {
			return this.$2$IsRequiredField;
		},
		set_isRequired: function(value) {
			this.$2$IsRequiredField = value;
		}
	});
	ss.initClass($Serenity_ComponentModel_UpdatableAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_Reporting_ReportDesignPanel, $asm, {
		addButtonClick: function(e) {
			e.preventDefault();
			//if (String.IsNullOrEmpty(ReportKey))
			//    return;
			//var dialog = new ReportDesignDialog(new ReportDesignDialogOptions());
			//Utils.BindToDataChange(dialog.Element, delegate
			//{
			//    RefreshDesignList();
			//});
			//dialog.LoadDataAndOpenDialog(new RetrieveResponse
			//{
			//    Entity = new ReportDesignEntity
			//    {
			//        ReportKey = ReportKey,
			//        ReportDesign = this.ById("DesignList").children("option").length > 0 ? "" : "[Varsayılan]"
			//    }
			//});
		},
		editButtonClick: function(e) {
			e.preventDefault();
			//var designId = SelectedDesignId;
			//if (designId == null)
			//    return;
			//var dialog = new ReportDesignDialog(new ReportDesignDialogOptions());
			//Utils.BindToDataChange(dialog.Element, delegate
			//{
			//    RefreshDesignList();
			//});
			//dialog.LoadByIdAndOpenDialog(designId.Value);
		},
		refreshDesignList: function() {
			Q.serviceCall({ service: 'ReportDesign/List', request: { ReportKey: this.get_reportKey() }, onSuccess: ss.mkdel(this, function(r) {
				this.setDesignList(r.Entities);
			}) });
		},
		setDesignList: function(designs) {
			var dl = this.byId$1('DesignList');
			Q.clearOptions(dl);
			for (var $t1 = 0; $t1 < designs.length; $t1++) {
				var design = designs[$t1];
				Q.addOption(dl, design.DesignId, design.DesignId);
			}
		},
		get_reportKey: function() {
			return this.$5$ReportKeyField;
		},
		set_reportKey: function(value) {
			this.$5$ReportKeyField = value;
		},
		get_selectedDesignId: function() {
			return Q.trimToNull(this.byId$1('DesignList').val());
		}
	}, ss.makeGenericType($Serenity_TemplatedWidget$1, [Object]));
	ss.initClass($Serenity_Reporting_ReportDialog, $asm, {
		createPropertyGrid: function() {
			if (ss.isValue(this.$propertyGrid)) {
				this.byId$1('PropertyGrid').html('').attr('class', '');
				this.$propertyGrid = null;
			}
			var $t2 = this.byId$1('PropertyGrid');
			var $t1 = $Serenity_PropertyGridOptions.$ctor();
			$t1.idPrefix = this.get_idPrefix();
			$t1.useCategories = true;
			$t1.items = this.$propertyItems;
			this.$propertyGrid = new $Serenity_PropertyGrid($t2, $t1);
		},
		loadReport: function(reportKey) {
			Q.serviceCall({ service: 'Report/Retrieve', request: { ReportKey: reportKey }, onSuccess: ss.mkdel(this, function(response) {
				this.$reportKey = ss.coalesce(response.ReportKey, reportKey);
				this.$propertyItems = response.Properties || [];
				this.get_element().dialog().dialog('option', 'title', response.Title);
				this.createPropertyGrid();
				var $t2 = this.$propertyGrid;
				var $t1 = response.InitialSettings;
				if (ss.isNullOrUndefined($t1)) {
					$t1 = new Object();
				}
				$t2.load($t1);
				this.toolbar.findButton('print-preview-button').toggle(!response.IsDataOnlyReport);
				this.toolbar.findButton('export-pdf-button').toggle(!response.IsDataOnlyReport);
				this.toolbar.findButton('export-docx-button').toggle(!response.IsDataOnlyReport);
				this.dialogOpen();
			}) });
		},
		executeReport: function(targetFrame, exportType) {
			if (!this.validateForm()) {
				return;
			}
			var parameters = new Object();
			this.$propertyGrid.save(parameters);
			Q$Externals.postToService({ service: 'Report/Execute', request: { ReportKey: this.$reportKey, DesignId: 'Default', ExportType: exportType, Parameters: parameters }, target: targetFrame });
		},
		getToolbarButtons: function() {
			var $t1 = [];
			ss.add($t1, { title: 'Önizleme', cssClass: 'print-preview-button', onClick: ss.mkdel(this, function() {
				this.executeReport('_blank', null);
			}) });
			null;
			ss.add($t1, { title: 'PDF', cssClass: 'export-pdf-button', onClick: ss.mkdel(this, function() {
				this.executeReport('', 'Pdf');
			}) });
			null;
			ss.add($t1, { title: 'Excel', cssClass: 'export-xlsx-button', onClick: ss.mkdel(this, function() {
				this.executeReport('', 'Xlsx');
			}) });
			null;
			ss.add($t1, { title: 'Word', cssClass: 'export-docx-button', onClick: ss.mkdel(this, function() {
				this.executeReport('', 'Docx');
			}) });
			null;
			return $t1;
		}
	}, ss.makeGenericType($Serenity_TemplatedDialog$1, [Object]), [$Serenity_IDialog]);
	ss.initClass($Serenity_Reporting_ReportPage, $asm, {
		$updateMatchFlags: function(text) {
			var liList = $('#ReportList').find('li').removeClass('non-match');
			text = Q.trimToNull(text);
			if (ss.isNullOrUndefined(text)) {
				liList.children('ul').hide();
				liList.show().removeClass('expanded');
				return;
			}
			var parts = ss.netSplit(text, [44, 32].map(function(i) {
				return String.fromCharCode(i);
			}), null, 1);
			for (var i = 0; i < parts.length; i++) {
				parts[i] = Q.trimToNull(Select2.util.stripDiacritics(parts[i]).toUpperCase());
			}
			var reportItems = liList.filter('.report-item');
			reportItems.each(function(i1, e) {
				var x = $(e);
				var title = Select2.util.stripDiacritics(ss.coalesce(x.text(), '').toUpperCase());
				for (var $t1 = 0; $t1 < parts.length; $t1++) {
					var p = parts[$t1];
					if (ss.isValue(p) && !(title.indexOf(p) !== -1)) {
						x.addClass('non-match');
						break;
					}
				}
			});
			var matchingItems = reportItems.not('.non-match');
			var visibles = matchingItems.parents('li').add(matchingItems);
			var nonVisibles = liList.not(visibles);
			nonVisibles.hide().addClass('non-match');
			visibles.show();
			if (visibles.length <= 100) {
				liList.children('ul').show();
				liList.addClass('expanded');
			}
		},
		$categoryClick: function(e) {
			var li = $(e.target).closest('li');
			if (li.hasClass('expanded')) {
				li.find('ul').hide('fast');
				li.removeClass('expanded');
				li.find('li').removeClass('expanded');
			}
			else {
				li.addClass('expanded');
				li.children('ul').show('fast');
				if (li.children('ul').children('li').length === 1 && !li.children('ul').children('li').hasClass('expanded')) {
					li.children('ul').children('li').children('.line').click();
				}
			}
		},
		$reportLinkClick: function(e) {
			e.preventDefault();
			var dialog = new $Serenity_Reporting_ReportDialog({ reportKey: $(e.target).data('key') });
		}
	}, $Serenity_Widget);
	ss.initClass($System_ComponentModel_DisplayNameAttribute, $asm, {
		get_displayName: function() {
			return this.$2$DisplayNameField;
		},
		set_displayName: function(value) {
			this.$2$DisplayNameField = value;
		}
	});
	ss.setMetadata($Serenity_BooleanEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Checkbox'), new Serenity.ElementAttribute('<input type="checkbox"/>')] });
	ss.setMetadata($Serenity_CheckListEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute("Checkbox'lı Liste"), new Serenity.OptionsTypeAttribute($Serenity_CheckListEditorOptions), new Serenity.ElementAttribute('<ul/>')] });
	ss.setMetadata($Serenity_CheckListEditorOptions, { members: [{ attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Items', type: 16, returnType: Array, getter: { name: 'get_Items', type: 8, params: [], returnType: Array, fget: 'items' }, setter: { name: 'set_Items', type: 8, params: [Array], returnType: Object, fset: 'items' }, fname: 'items' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Tümünü Seç Metni')], name: 'SelectAllOptionText', type: 16, returnType: String, getter: { name: 'get_SelectAllOptionText', type: 8, params: [], returnType: String, fget: 'selectAllOptionText' }, setter: { name: 'set_SelectAllOptionText', type: 8, params: [String], returnType: Object, fset: 'selectAllOptionText' }, fname: 'selectAllOptionText' }] });
	ss.setMetadata($Serenity_CheckTreeEditor$2, { attr: [new Serenity.ElementAttribute('<div/>'), new Serenity.IdPropertyAttribute('id')] });
	ss.setMetadata($Serenity_DateEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Tarih'), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_DateTimeEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Tarih/Zaman'), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_DateYearEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Yıl'), new Serenity.OptionsTypeAttribute($Serenity_DateYearEditorOptions), new Serenity.ElementAttribute('<input type="hidden"/>')] });
	ss.setMetadata($Serenity_DecimalEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Ondalıklı Sayı'), new Serenity.OptionsTypeAttribute($Serenity_DecimalEditorOptions), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_DecimalEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Ondalık'), new $Serenity_ComponentModel_EditorTypeAttribute('Integer')], name: 'Decimals', type: 16, returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), getter: { name: 'get_Decimals', type: 8, params: [], returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), fget: 'decimals' }, setter: { name: 'set_Decimals', type: 8, params: [ss.makeGenericType(ss.Nullable$1, [ss.Int32])], returnType: Object, fset: 'decimals' }, fname: 'decimals' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Max Değer')], name: 'MaxValue', type: 16, returnType: String, getter: { name: 'get_MaxValue', type: 8, params: [], returnType: String, fget: 'maxValue' }, setter: { name: 'set_MaxValue', type: 8, params: [String], returnType: Object, fset: 'maxValue' }, fname: 'maxValue' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Min Değer')], name: 'MinValue', type: 16, returnType: String, getter: { name: 'get_MinValue', type: 8, params: [], returnType: String, fget: 'minValue' }, setter: { name: 'set_MinValue', type: 8, params: [String], returnType: Object, fset: 'minValue' }, fname: 'minValue' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Ondalıkları Sıfırla Doldur'), new $Serenity_ComponentModel_EditorTypeAttribute('Boolean')], name: 'PadDecimals', type: 16, returnType: ss.makeGenericType(ss.Nullable$1, [Boolean]), getter: { name: 'get_PadDecimals', type: 8, params: [], returnType: ss.makeGenericType(ss.Nullable$1, [Boolean]), fget: 'padDecimals' }, setter: { name: 'set_PadDecimals', type: 8, params: [ss.makeGenericType(ss.Nullable$1, [Boolean])], returnType: Object, fset: 'padDecimals' }, fname: 'padDecimals' }] });
	ss.setMetadata($Serenity_EditorTypeEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Editör Tipi'), new Serenity.ElementAttribute('<input type="hidden" />')] });
	ss.setMetadata($Serenity_EmailEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('E-posta'), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_EmailEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Etki Alanı')], name: 'Domain', type: 16, returnType: String, getter: { name: 'get_Domain', type: 8, params: [], returnType: String, fget: 'domain' }, setter: { name: 'set_Domain', type: 8, params: [String], returnType: Object, fset: 'domain' }, fname: 'domain' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Etki Alanı Salt Okunur')], name: 'ReadOnlyDomain', type: 16, returnType: Boolean, getter: { name: 'get_ReadOnlyDomain', type: 8, params: [], returnType: Boolean, fget: 'readOnlyDomain' }, setter: { name: 'set_ReadOnlyDomain', type: 8, params: [Boolean], returnType: Object, fset: 'readOnlyDomain' }, fname: 'readOnlyDomain' }] });
	ss.setMetadata($Serenity_HtmlContentEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Html İçerik'), new Serenity.OptionsTypeAttribute($Serenity_HtmlContentEditorOptions), new Serenity.ElementAttribute('<textarea />')] });
	ss.setMetadata($Serenity_HtmlContentEditorOptions, { members: [{ attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Cols', type: 16, returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), getter: { name: 'get_Cols', type: 8, params: [], returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), fget: 'cols' }, setter: { name: 'set_Cols', type: 8, params: [ss.makeGenericType(ss.Nullable$1, [ss.Int32])], returnType: Object, fset: 'cols' }, fname: 'cols' }, { attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Rows', type: 16, returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), getter: { name: 'get_Rows', type: 8, params: [], returnType: ss.makeGenericType(ss.Nullable$1, [ss.Int32]), fget: 'rows' }, setter: { name: 'set_Rows', type: 8, params: [ss.makeGenericType(ss.Nullable$1, [ss.Int32])], returnType: Object, fset: 'rows' }, fname: 'rows' }] });
	ss.setMetadata($Serenity_HtmlReportContentEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Html İçerik (Rapor Uyumlu Kısıtlı Set)'), new Serenity.OptionsTypeAttribute($Serenity_HtmlContentEditorOptions), new Serenity.ElementAttribute('<textarea />')] });
	ss.setMetadata($Serenity_ImageUploadEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Resim Yükleme'), new Serenity.OptionsTypeAttribute($Serenity_ImageUploadEditorOptions), new Serenity.ElementAttribute('<div/>')] });
	ss.setMetadata($Serenity_ImageUploadEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Maksimum Yükseklik')], name: 'MaxHeight', type: 16, returnType: ss.Int32, getter: { name: 'get_MaxHeight', type: 8, params: [], returnType: ss.Int32, fget: 'maxHeight' }, setter: { name: 'set_MaxHeight', type: 8, params: [ss.Int32], returnType: Object, fset: 'maxHeight' }, fname: 'maxHeight' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Maksimum Boyut')], name: 'MaxSize', type: 16, returnType: ss.Int32, getter: { name: 'get_MaxSize', type: 8, params: [], returnType: ss.Int32, fget: 'maxSize' }, setter: { name: 'set_MaxSize', type: 8, params: [ss.Int32], returnType: Object, fset: 'maxSize' }, fname: 'maxSize' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Maksimum Genişlik')], name: 'MaxWidth', type: 16, returnType: ss.Int32, getter: { name: 'get_MaxWidth', type: 8, params: [], returnType: ss.Int32, fget: 'maxWidth' }, setter: { name: 'set_MaxWidth', type: 8, params: [ss.Int32], returnType: Object, fset: 'maxWidth' }, fname: 'maxWidth' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Minimum Yükseklik')], name: 'MinHeight', type: 16, returnType: ss.Int32, getter: { name: 'get_MinHeight', type: 8, params: [], returnType: ss.Int32, fget: 'minHeight' }, setter: { name: 'set_MinHeight', type: 8, params: [ss.Int32], returnType: Object, fset: 'minHeight' }, fname: 'minHeight' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Minimum Boyut')], name: 'MinSize', type: 16, returnType: ss.Int32, getter: { name: 'get_MinSize', type: 8, params: [], returnType: ss.Int32, fget: 'minSize' }, setter: { name: 'set_MinSize', type: 8, params: [ss.Int32], returnType: Object, fset: 'minSize' }, fname: 'minSize' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Minimum Genişlik')], name: 'MinWidth', type: 16, returnType: ss.Int32, getter: { name: 'get_MinWidth', type: 8, params: [], returnType: ss.Int32, fget: 'minWidth' }, setter: { name: 'set_MinWidth', type: 8, params: [ss.Int32], returnType: Object, fset: 'minWidth' }, fname: 'minWidth' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Orjinal Dosya Adı Alanı')], name: 'OriginalNameProperty', type: 16, returnType: String, getter: { name: 'get_OriginalNameProperty', type: 8, params: [], returnType: String, fget: 'originalNameProperty' }, setter: { name: 'set_OriginalNameProperty', type: 8, params: [String], returnType: Object, fset: 'originalNameProperty' }, fname: 'originalNameProperty' }] });
	ss.setMetadata($Serenity_IntegerEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Tamsayı'), new Serenity.OptionsTypeAttribute($Serenity_IntegerEditorOptions), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_IntegerEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Max Değer')], name: 'MaxValue', type: 16, returnType: ss.Int32, getter: { name: 'get_MaxValue', type: 8, params: [], returnType: ss.Int32, fget: 'maxValue' }, setter: { name: 'set_MaxValue', type: 8, params: [ss.Int32], returnType: Object, fset: 'maxValue' }, fname: 'maxValue' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Min Değer')], name: 'MinValue', type: 16, returnType: ss.Int32, getter: { name: 'get_MinValue', type: 8, params: [], returnType: ss.Int32, fget: 'minValue' }, setter: { name: 'set_MinValue', type: 8, params: [ss.Int32], returnType: Object, fset: 'minValue' }, fname: 'minValue' }] });
	ss.setMetadata($Serenity_LookupEditor, { attr: [new Serenity.EditorAttribute(), new Serenity.OptionsTypeAttribute($Serenity_LookupEditorOptions)] });
	ss.setMetadata($Serenity_LookupEditorBase$2, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
	ss.setMetadata($Serenity_MaskedEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Maskeli Giriş'), new Serenity.OptionsTypeAttribute($Serenity_MaskedEditorOptions), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_MaskedEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Giriş Maskesi')], name: 'Mask', type: 16, returnType: String, getter: { name: 'get_Mask', type: 8, params: [], returnType: String, fget: 'mask' }, setter: { name: 'set_Mask', type: 8, params: [String], returnType: Object, fset: 'mask' }, fname: 'mask' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Yer Tutucu Karakter')], name: 'Placeholder', type: 16, returnType: String, getter: { name: 'get_Placeholder', type: 8, params: [], returnType: String, fget: 'placeholder' }, setter: { name: 'set_Placeholder', type: 8, params: [String], returnType: Object, fset: 'placeholder' }, fname: 'placeholder' }] });
	ss.setMetadata($Serenity_PasswordEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Şifre')] });
	ss.setMetadata($Serenity_PersonNameEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Kişi İsim'), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_PhoneEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Telefon'), new Serenity.OptionsTypeAttribute($Serenity_PhoneEditorOptions), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_PhoneEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Dahili Telefon')], name: 'Internal', type: 16, returnType: Boolean, getter: { name: 'get_Internal', type: 8, params: [], returnType: Boolean, fget: 'internal' }, setter: { name: 'set_Internal', type: 8, params: [Boolean], returnType: Object, fset: 'internal' }, fname: 'internal' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Cep Telefonu')], name: 'Mobile', type: 16, returnType: Boolean, getter: { name: 'get_Mobile', type: 8, params: [], returnType: Boolean, fget: 'mobile' }, setter: { name: 'set_Mobile', type: 8, params: [Boolean], returnType: Object, fset: 'mobile' }, fname: 'mobile' }, { attr: [new $System_ComponentModel_DisplayNameAttribute('Birden Çok Girişe İzin Ver')], name: 'Multiple', type: 16, returnType: Boolean, getter: { name: 'get_Multiple', type: 8, params: [], returnType: Boolean, fget: 'multiple' }, setter: { name: 'set_Multiple', type: 8, params: [Boolean], returnType: Object, fset: 'multiple' }, fname: 'multiple' }] });
	ss.setMetadata($Serenity_Select2AjaxEditor$2, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
	ss.setMetadata($Serenity_Select2Editor$2, { attr: [new Serenity.ElementAttribute('<input type="hidden"/>')] });
	ss.setMetadata($Serenity_SelectEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Açılır Liste'), new Serenity.OptionsTypeAttribute($Serenity_SelectEditorOptions), new Serenity.ElementAttribute('<input type="hidden"/>')] });
	ss.setMetadata($Serenity_SelectEditorOptions, { members: [{ attr: [new $System_ComponentModel_DisplayNameAttribute('Boş Eleman Metni')], name: 'EmptyOptionText', type: 16, returnType: String, getter: { name: 'get_EmptyOptionText', type: 8, params: [], returnType: String, fget: 'emptyOptionText' }, setter: { name: 'set_EmptyOptionText', type: 8, params: [String], returnType: Object, fset: 'emptyOptionText' }, fname: 'emptyOptionText' }, { attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Items', type: 16, returnType: Array, getter: { name: 'get_Items', type: 8, params: [], returnType: Array, fget: 'items' }, setter: { name: 'set_Items', type: 8, params: [Array], returnType: Object, fset: 'items' }, fname: 'items' }] });
	ss.setMetadata($Serenity_StringEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Metin'), new Serenity.ElementAttribute('<input type="text"/>')] });
	ss.setMetadata($Serenity_TextAreaEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('Çok Satırlı Metin'), new Serenity.OptionsTypeAttribute($Serenity_TextAreaEditorOptions), new Serenity.ElementAttribute('<textarea />')] });
	ss.setMetadata($Serenity_TextAreaEditorOptions, { members: [{ attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Cols', type: 16, returnType: ss.Int32, getter: { name: 'get_Cols', type: 8, params: [], returnType: ss.Int32, fget: 'cols' }, setter: { name: 'set_Cols', type: 8, params: [ss.Int32], returnType: Object, fset: 'cols' }, fname: 'cols' }, { attr: [new $Serenity_ComponentModel_HiddenAttribute()], name: 'Rows', type: 16, returnType: ss.Int32, getter: { name: 'get_Rows', type: 8, params: [], returnType: ss.Int32, fget: 'rows' }, setter: { name: 'set_Rows', type: 8, params: [ss.Int32], returnType: Object, fset: 'rows' }, fname: 'rows' }] });
	ss.setMetadata($Serenity_URLEditor, { attr: [new Serenity.EditorAttribute(), new $System_ComponentModel_DisplayNameAttribute('URL')] });
	ss.setMetadata($Serenity_ComponentModel_EditorOptionAttribute, { attrAllowMultiple: true });
	$Serenity_Widget.$nextWidgetNumber = 0;
	$Serenity_DialogExtensions.$enterKeyCode = 13;
	$Serenity_EditorTypeCache.$visited = null;
	$Serenity_EditorTypeCache.$registeredTypes = null;
	$Serenity_EditorTypeEditor.$editorTypeList = null;
	$Serenity_PropertyGrid.$knownEditorTypes = null;
	$Serenity_PropertyGrid.$knownEditorTypes = {};
	$Serenity_FilterPanel.panelTemplate = '<div id="~_Rows" class="rows"></div><div id="~_Buttons" class="buttons"><button id="~_AddButton" class="add"></button><button id="~_SearchButton" class="search"></button><button id="~_ResetButton" class="reset"></button></div><div style="clear: both"></div><div id="~_DisplayText" class="display" style="display: none;"></div>';
	$Serenity_FilterPanel.rowTemplate = '<div class="row"><a class="delete"><span></span></a><div class="l"><a class="rightparen" href="#">)</a><a class="andor" href="#"></a><a class="leftparen" href="#">(</a></div><div class="f"><select></select></div><div class="o"></div><div class="v"></div><div style="clear: both"></div></div>';
	$Serenity_StringInflector.$plural = null;
	$Serenity_StringInflector.$singular = null;
	$Serenity_StringInflector.$countable = null;
	$Serenity_StringInflector.$pluralizeCache = null;
	$Serenity_StringInflector.$singularizeCache = null;
	$Serenity_StringInflector.$initialized = false;
})();

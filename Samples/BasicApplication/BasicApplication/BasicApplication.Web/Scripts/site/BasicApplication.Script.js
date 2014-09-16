(function() {
	'use strict';
	var $asm = {};
	global.BasicApplication = global.BasicApplication || {};
	global.BasicApplication.Administration = global.BasicApplication.Administration || {};
	global.BasicApplication.Common = global.BasicApplication.Common || {};
	global.BasicApplication.Membership = global.BasicApplication.Membership || {};
	global.BasicApplication.Northwind = global.BasicApplication.Northwind || {};
	ss.initAssembly($asm, 'BasicApplication.Script');
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.ScriptInitialization
	var $BasicApplication_ScriptInitialization = function() {
	};
	$BasicApplication_ScriptInitialization.__typeName = 'BasicApplication.ScriptInitialization';
	global.BasicApplication.ScriptInitialization = $BasicApplication_ScriptInitialization;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.UserDialog
	var $BasicApplication_Administration_UserDialog = function() {
		this.$form = null;
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
		this.$form = new $BasicApplication_Administration_UserForm(this.get_idPrefix());
		Serenity.WX.addValidationRule(this.$form.get_password(), this.uniqueName, ss.mkdel(this, function(e) {
			if (this.$form.get_password().get_value().length < 7) {
				return 'Password must be at least 7 characters!';
			}
			return null;
		}));
		Serenity.WX.addValidationRule(this.$form.get_passwordConfirm(), this.uniqueName, ss.mkdel(this, function(e1) {
			if (!ss.referenceEquals(this.$form.get_password().get_value(), this.$form.get_passwordConfirm().get_value())) {
				return "The passwords entered doesn't match!";
			}
			return null;
		}));
	};
	$BasicApplication_Administration_UserDialog.__typeName = 'BasicApplication.Administration.UserDialog';
	global.BasicApplication.Administration.UserDialog = $BasicApplication_Administration_UserDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.UserForm
	var $BasicApplication_Administration_UserForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Administration_UserForm.__typeName = 'BasicApplication.Administration.UserForm';
	global.BasicApplication.Administration.UserForm = $BasicApplication_Administration_UserForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.UserGrid
	var $BasicApplication_Administration_UserGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Administration_UserGrid.__typeName = 'BasicApplication.Administration.UserGrid';
	global.BasicApplication.Administration.UserGrid = $BasicApplication_Administration_UserGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.UserService
	var $BasicApplication_Administration_UserService = function() {
	};
	$BasicApplication_Administration_UserService.__typeName = 'BasicApplication.Administration.UserService';
	$BasicApplication_Administration_UserService.create = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/Create', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.update = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/Update', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.delete$1 = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/Delete', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.undelete = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/Undelete', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.retrieve = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.list = function(request, onSuccess, options) {
		Q.serviceRequest('Administration/User/List', request, onSuccess, options);
	};
	global.BasicApplication.Administration.UserService = $BasicApplication_Administration_UserService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Common.SidebarSearch
	var $BasicApplication_Common_SidebarSearch = function(input, menuUL) {
		this.$menuUL = null;
		Serenity.Widget.call(this, input);
		var self = this;
		var $t1 = Serenity.QuickSearchInputOptions.$ctor();
		$t1.onSearch = function(field, text) {
			self.$updateMatchFlags(text);
		};
		new Serenity.QuickSearchInput(input, $t1);
		this.$menuUL = menuUL;
	};
	$BasicApplication_Common_SidebarSearch.__typeName = 'BasicApplication.Common.SidebarSearch';
	global.BasicApplication.Common.SidebarSearch = $BasicApplication_Common_SidebarSearch;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Membership.LoginForm
	var $BasicApplication_Membership_LoginForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Membership_LoginForm.__typeName = 'BasicApplication.Membership.LoginForm';
	global.BasicApplication.Membership.LoginForm = $BasicApplication_Membership_LoginForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Membership.LoginPanel
	var $BasicApplication_Membership_LoginPanel = function() {
		ss.makeGenericType(Serenity.PropertyDialog$1, [Object]).call(this);
		this.byId$1('LoginButton').click(ss.thisFix(ss.mkdel(this, function(s, e) {
			e.preventDefault();
			if (!this.validateForm()) {
				return;
			}
			var request = this.getSaveEntity();
			Q.serviceCall({
				url: Q.resolveUrl('~/Account/Login'),
				request: request,
				onSuccess: function(response) {
					var q = Q$Externals.parseQueryString();
					var $t1 = q['returnUrl'];
					if (ss.isNullOrUndefined($t1)) {
						$t1 = q['ReturnUrl'];
					}
					var r = $t1;
					if (!ss.isNullOrEmptyString(r)) {
						window.location.href = r;
					}
					else {
						window.location.href = Q.resolveUrl('~/');
					}
				}
			});
		})));
	};
	$BasicApplication_Membership_LoginPanel.__typeName = 'BasicApplication.Membership.LoginPanel';
	global.BasicApplication.Membership.LoginPanel = $BasicApplication_Membership_LoginPanel;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CategoryDialog
	var $BasicApplication_Northwind_CategoryDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_CategoryDialog.__typeName = 'BasicApplication.Northwind.CategoryDialog';
	global.BasicApplication.Northwind.CategoryDialog = $BasicApplication_Northwind_CategoryDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CategoryGrid
	var $BasicApplication_Northwind_CategoryGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_CategoryGrid.__typeName = 'BasicApplication.Northwind.CategoryGrid';
	global.BasicApplication.Northwind.CategoryGrid = $BasicApplication_Northwind_CategoryGrid;
	ss.initClass($BasicApplication_ScriptInitialization, $asm, {});
	ss.initClass($BasicApplication_Administration_UserDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Administration_UserForm, $asm, {
		get_username: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Username');
		},
		get_displayName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'DisplayName');
		},
		get_email: function() {
			return this.byId(Serenity.EmailEditor).call(this, 'Email');
		},
		get_password: function() {
			return this.byId(Serenity.PasswordEditor).call(this, 'Password');
		},
		get_passwordConfirm: function() {
			return this.byId(Serenity.PasswordEditor).call(this, 'PasswordConfirm');
		},
		get_source: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Source');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Administration_UserGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			ss.add(columns, { field: 'UserId', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			ss.add(columns, { field: 'Username', width: 150, format: this.itemLink(null, null, null, null) });
			ss.add(columns, { field: 'DisplayName', width: 150 });
			ss.add(columns, { field: 'Email', width: 250 });
			ss.add(columns, { field: 'Source', width: 100 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Administration_UserService, $asm, {});
	ss.initClass($BasicApplication_Common_SidebarSearch, $asm, {
		$updateMatchFlags: function(text) {
			var liList = this.$menuUL.find('li').removeClass('non-match');
			text = Q.trimToNull(text);
			if (ss.isNullOrUndefined(text)) {
				liList.removeClass('active');
				liList.show();
				liList.children('ul').addClass('collapse');
				return;
			}
			var parts = ss.netSplit(text, [44, 32].map(function(i) {
				return String.fromCharCode(i);
			}), null, 1);
			for (var i = 0; i < parts.length; i++) {
				parts[i] = Q.trimToNull(Select2.util.stripDiacritics(parts[i]).toUpperCase());
			}
			var items = liList;
			items.each(function(i1, e) {
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
			var matchingItems = items.not('.non-match');
			var visibles = matchingItems.parents('li').add(matchingItems);
			var nonVisibles = liList.not(visibles);
			nonVisibles.hide().addClass('non-match');
			visibles.show();
			liList.children('ul').removeClass('collapse');
		}
	}, Serenity.Widget);
	ss.initClass($BasicApplication_Membership_LoginForm, $asm, {
		get_username: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Username');
		},
		get_password: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Password');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Membership_LoginPanel, $asm, {}, ss.makeGenericType(Serenity.PropertyDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Northwind_CategoryDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Northwind_CategoryGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			ss.add(columns, { field: 'CategoryID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			ss.add(columns, { field: 'CategoryName', width: 250, format: this.itemLink(null, null, null, null) });
			ss.add(columns, { field: 'Description', width: 450 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.setMetadata($BasicApplication_Administration_UserDialog, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.FormKeyAttribute('Administration.User'), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Administration_UserGrid, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.DialogTypeAttribute($BasicApplication_Administration_UserDialog), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Membership_LoginPanel, { attr: [new Serenity.PanelAttribute(), new Serenity.FormKeyAttribute('Membership.Login')] });
	ss.setMetadata($BasicApplication_Northwind_CategoryDialog, { attr: [new Serenity.IdPropertyAttribute('CategoryID'), new Serenity.NamePropertyAttribute('CategoryName'), new Serenity.FormKeyAttribute('Northwind.Category'), new Serenity.LocalTextPrefixAttribute('Northwind.Category'), new Serenity.ServiceAttribute('Northwind/Category')] });
	ss.setMetadata($BasicApplication_Northwind_CategoryGrid, { attr: [new Serenity.IdPropertyAttribute('CategoryID'), new Serenity.NamePropertyAttribute('CategoryName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_CategoryDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Category'), new Serenity.ServiceAttribute('Northwind/Category')] });
	ss.add(Q$Config.rootNamespaces, 'BasicApplication');
})();

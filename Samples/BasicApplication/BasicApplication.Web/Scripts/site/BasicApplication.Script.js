(function() {
	'use strict';
	var $asm = {};
	global.BasicApplication = global.BasicApplication || {};
	global.BasicApplication.Administration = global.BasicApplication.Administration || {};
	global.BasicApplication.Membership = global.BasicApplication.Membership || {};
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
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
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
	ss.initClass($BasicApplication_ScriptInitialization, $asm, {});
	ss.initClass($BasicApplication_Administration_UserDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Administration_UserForm, $asm, {
		get_username: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Username');
		},
		get_source: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Source');
		},
		get_passwordHash: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PasswordHash');
		},
		get_passwordSalt: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PasswordSalt');
		},
		get_insertDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'InsertDate');
		},
		get_insertUserId: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'InsertUserId');
		},
		get_isActive: function() {
			return this.byId(Serenity.StringEditor).call(this, 'IsActive');
		},
		get_updateDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'UpdateDate');
		},
		get_updateUserId: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'UpdateUserId');
		},
		get_displayName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'DisplayName');
		},
		get_email: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Email');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Administration_UserGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			ss.add(columns, { field: 'UserId', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			ss.add(columns, { field: 'Username', width: 200, format: this.itemLink(null, null, null, null) });
			ss.add(columns, { field: 'Source', width: 80 });
			ss.add(columns, { field: 'PasswordHash', width: 80 });
			ss.add(columns, { field: 'PasswordSalt', width: 80 });
			ss.add(columns, { field: 'DisplayName', width: 80 });
			ss.add(columns, { field: 'Email', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Administration_UserService, $asm, {});
	ss.initClass($BasicApplication_Membership_LoginPanel, $asm, {}, ss.makeGenericType(Serenity.PropertyDialog$1, [Object]), [Serenity.IDialog]);
	ss.setMetadata($BasicApplication_Administration_UserDialog, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.FormKeyAttribute('Administration.User'), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Administration_UserGrid, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.DialogTypeAttribute($BasicApplication_Administration_UserDialog), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Membership_LoginPanel, { attr: [new Serenity.FormKeyAttribute('Membership.User'), new Serenity.PanelAttribute()] });
	ss.add(Q$Config.rootNamespaces, 'BasicApplication');
})();

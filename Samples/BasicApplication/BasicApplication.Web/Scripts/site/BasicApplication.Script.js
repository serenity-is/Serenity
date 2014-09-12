(function() {
	'use strict';
	var $asm = {};
	global.BasicApplication = global.BasicApplication || {};
	global.BasicApplication.Administration = global.BasicApplication.Administration || {};
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
	// BasicApplication.Administration.UserGrid
	var $BasicApplication_Administration_UserGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Administration_UserGrid.__typeName = 'BasicApplication.Administration.UserGrid';
	global.BasicApplication.Administration.UserGrid = $BasicApplication_Administration_UserGrid;
	ss.initClass($BasicApplication_ScriptInitialization, $asm, {});
	ss.initClass($BasicApplication_Administration_UserDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Administration_UserGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			ss.add(columns, { field: 'UserId', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			ss.add(columns, { field: 'Username', width: 200, format: this.itemLink(null, null, null, null) });
			ss.add(columns, { field: 'Source', width: 80 });
			ss.add(columns, { field: 'PasswordHash', width: 80 });
			ss.add(columns, { field: 'PasswordSalt', width: 80 });
			ss.add(columns, { field: 'InsertDate', width: 80 });
			ss.add(columns, { field: 'InsertUserId', width: 80 });
			ss.add(columns, { field: 'IsActive', width: 80 });
			ss.add(columns, { field: 'UpdateDate', width: 80 });
			ss.add(columns, { field: 'UpdateUserId', width: 80 });
			ss.add(columns, { field: 'DisplayName', width: 80 });
			ss.add(columns, { field: 'Email', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.setMetadata($BasicApplication_Administration_UserDialog, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.FormKeyAttribute('Administration.User'), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Administration_UserGrid, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.DialogTypeAttribute($BasicApplication_Administration_UserDialog), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.add(Q$Config.rootNamespaces, 'BasicApplication');
})();

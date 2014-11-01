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
	// BasicApplication.Administration.LanguageDialog
	var $BasicApplication_Administration_LanguageDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Administration_LanguageDialog.__typeName = 'BasicApplication.Administration.LanguageDialog';
	global.BasicApplication.Administration.LanguageDialog = $BasicApplication_Administration_LanguageDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.LanguageForm
	var $BasicApplication_Administration_LanguageForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Administration_LanguageForm.__typeName = 'BasicApplication.Administration.LanguageForm';
	global.BasicApplication.Administration.LanguageForm = $BasicApplication_Administration_LanguageForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.LanguageGrid
	var $BasicApplication_Administration_LanguageGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Administration_LanguageGrid.__typeName = 'BasicApplication.Administration.LanguageGrid';
	global.BasicApplication.Administration.LanguageGrid = $BasicApplication_Administration_LanguageGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.LanguageService
	var $BasicApplication_Administration_LanguageService = function() {
	};
	$BasicApplication_Administration_LanguageService.__typeName = 'BasicApplication.Administration.LanguageService';
	$BasicApplication_Administration_LanguageService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Language/Create', request, onSuccess, options);
	};
	$BasicApplication_Administration_LanguageService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Language/Update', request, onSuccess, options);
	};
	$BasicApplication_Administration_LanguageService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Language/Delete', request, onSuccess, options);
	};
	$BasicApplication_Administration_LanguageService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Language/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Administration_LanguageService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Language/List', request, onSuccess, options);
	};
	global.BasicApplication.Administration.LanguageService = $BasicApplication_Administration_LanguageService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.TranslationForm
	var $BasicApplication_Administration_TranslationForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Administration_TranslationForm.__typeName = 'BasicApplication.Administration.TranslationForm';
	global.BasicApplication.Administration.TranslationForm = $BasicApplication_Administration_TranslationForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.TranslationGrid
	var $BasicApplication_Administration_TranslationGrid = function(container) {
		this.$searchText = null;
		this.$sourceLanguage = null;
		this.$targetLanguage = null;
		this.$targetLanguageKey = null;
		this.$hasChanges = false;
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
		this.element.on('keyup.' + this.uniqueName + ' change.' + this.uniqueName, 'input.custom-text', ss.mkdel(this, function(e) {
			var value = Q.trimToNull($(e.target).val());
			if (value === '') {
				value = null;
			}
			this.view.getItemById($(e.target).data('key')).CustomText = value;
			this.$hasChanges = true;
		}));
	};
	$BasicApplication_Administration_TranslationGrid.__typeName = 'BasicApplication.Administration.TranslationGrid';
	global.BasicApplication.Administration.TranslationGrid = $BasicApplication_Administration_TranslationGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.TranslationService
	var $BasicApplication_Administration_TranslationService = function() {
	};
	$BasicApplication_Administration_TranslationService.__typeName = 'BasicApplication.Administration.TranslationService';
	$BasicApplication_Administration_TranslationService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Translation/List', request, onSuccess, options);
	};
	$BasicApplication_Administration_TranslationService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/Translation/Update', request, onSuccess, options);
	};
	global.BasicApplication.Administration.TranslationService = $BasicApplication_Administration_TranslationService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Administration.UserDialog
	var $BasicApplication_Administration_UserDialog = function() {
		this.$form = null;
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
		this.$form = new $BasicApplication_Administration_UserForm(this.get_idPrefix());
		Serenity.VX.addValidationRule(this.$form.get_password(), this.uniqueName, ss.mkdel(this, function(e) {
			if (this.$form.get_password().get_value().length < 7) {
				return 'Password must be at least 7 characters!';
			}
			return null;
		}));
		Serenity.VX.addValidationRule(this.$form.get_passwordConfirm(), this.uniqueName, ss.mkdel(this, function(e1) {
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
		return Q.serviceRequest('Administration/User/Create', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/User/Update', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/User/Delete', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.undelete = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/User/Undelete', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/User/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Administration_UserService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Administration/User/List', request, onSuccess, options);
	};
	global.BasicApplication.Administration.UserService = $BasicApplication_Administration_UserService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Common.CascadedEditorHelper
	var $BasicApplication_Common_CascadedEditorHelper$2 = function(TWidget, TParentWidget) {
		var $type = function(widget, getParentValue, updateItems) {
			this.$widget = null;
			this.$updateItems = null;
			this.$parentID = null;
			this.$parentValue = null;
			this.$getParentValue = null;
			this.$widget = widget;
			this.$updateItems = updateItems;
			this.$getParentValue = getParentValue;
		};
		ss.registerGenericClassInstance($type, $BasicApplication_Common_CascadedEditorHelper$2, [TWidget, TParentWidget], {
			bindToParent: function() {
				if (Q.isEmptyOrNull(this.get_parentID())) {
					return;
				}
				var parent = Serenity.WX.tryGetWidget(TParentWidget).call(null, Q.findElementWithRelativeId(this.$widget.get_element(), this.get_parentID()));
				if (ss.isValue(parent)) {
					parent.get_element().bind('change.' + this.$widget.get_uniqueName(), ss.mkdel(this, function() {
						this.set_parentValue(this.$getParentValue(parent));
					}));
				}
			},
			unbindFromParent: function() {
				if (Q.isEmptyOrNull(this.get_parentID())) {
					return;
				}
				var parent = Serenity.WX.tryGetWidget(TParentWidget).call(null, Q.findElementWithRelativeId(this.$widget.get_element(), this.get_parentID()));
				if (ss.isValue(parent)) {
					parent.get_element().unbind('.' + this.$widget.get_uniqueName());
				}
			},
			get_parentID: function() {
				return this.$parentID;
			},
			set_parentID: function(value) {
				if (!ss.referenceEquals(this.$parentID, value)) {
					this.unbindFromParent();
					this.$parentID = value;
					this.bindToParent();
					this.$updateItems();
				}
			},
			get_parentValue: function() {
				return this.$parentValue;
			},
			set_parentValue: function(value) {
				if (!ss.referenceEquals(ss.coalesce(this.$parentValue, '').toString(), ss.coalesce(value, '').toString())) {
					this.$parentValue = value;
					this.$updateItems();
				}
			}
		}, function() {
			return null;
		}, function() {
			return [];
		});
		return $type;
	};
	$BasicApplication_Common_CascadedEditorHelper$2.__typeName = 'BasicApplication.Common.CascadedEditorHelper$2';
	ss.initGenericClass($BasicApplication_Common_CascadedEditorHelper$2, $asm, 2);
	global.BasicApplication.Common.CascadedEditorHelper$2 = $BasicApplication_Common_CascadedEditorHelper$2;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Common.LanguageSelection
	var $BasicApplication_Common_LanguageSelection = function(hidden, currentLanguage) {
		this.$currentLanguage = null;
		ss.makeGenericType(Serenity.LookupEditorBase$1, [Object]).call(this, hidden);
		this.$currentLanguage = ss.coalesce(currentLanguage, 'en');
		this.set_value('en');
		var self = this;
		Serenity.WX.changeSelect2(this, function(e) {
			$.cookie('LanguagePreference', self.get_value(), { path: Q$Config.applicationPath });
			window.location.reload(true);
		});
	};
	$BasicApplication_Common_LanguageSelection.__typeName = 'BasicApplication.Common.LanguageSelection';
	global.BasicApplication.Common.LanguageSelection = $BasicApplication_Common_LanguageSelection;
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
	// BasicApplication.Northwind.CategoryForm
	var $BasicApplication_Northwind_CategoryForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_CategoryForm.__typeName = 'BasicApplication.Northwind.CategoryForm';
	global.BasicApplication.Northwind.CategoryForm = $BasicApplication_Northwind_CategoryForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CategoryGrid
	var $BasicApplication_Northwind_CategoryGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_CategoryGrid.__typeName = 'BasicApplication.Northwind.CategoryGrid';
	global.BasicApplication.Northwind.CategoryGrid = $BasicApplication_Northwind_CategoryGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CategoryService
	var $BasicApplication_Northwind_CategoryService = function() {
	};
	$BasicApplication_Northwind_CategoryService.__typeName = 'BasicApplication.Northwind.CategoryService';
	$BasicApplication_Northwind_CategoryService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Category/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CategoryService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Category/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CategoryService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Category/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CategoryService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Category/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CategoryService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Category/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.CategoryService = $BasicApplication_Northwind_CategoryService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerCustomerDemoDialog
	var $BasicApplication_Northwind_CustomerCustomerDemoDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoDialog.__typeName = 'BasicApplication.Northwind.CustomerCustomerDemoDialog';
	global.BasicApplication.Northwind.CustomerCustomerDemoDialog = $BasicApplication_Northwind_CustomerCustomerDemoDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerCustomerDemoForm
	var $BasicApplication_Northwind_CustomerCustomerDemoForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoForm.__typeName = 'BasicApplication.Northwind.CustomerCustomerDemoForm';
	global.BasicApplication.Northwind.CustomerCustomerDemoForm = $BasicApplication_Northwind_CustomerCustomerDemoForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerCustomerDemoGrid
	var $BasicApplication_Northwind_CustomerCustomerDemoGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoGrid.__typeName = 'BasicApplication.Northwind.CustomerCustomerDemoGrid';
	global.BasicApplication.Northwind.CustomerCustomerDemoGrid = $BasicApplication_Northwind_CustomerCustomerDemoGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerCustomerDemoService
	var $BasicApplication_Northwind_CustomerCustomerDemoService = function() {
	};
	$BasicApplication_Northwind_CustomerCustomerDemoService.__typeName = 'BasicApplication.Northwind.CustomerCustomerDemoService';
	$BasicApplication_Northwind_CustomerCustomerDemoService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerCustomerDemo/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerCustomerDemo/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerCustomerDemo/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerCustomerDemo/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerCustomerDemoService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerCustomerDemo/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.CustomerCustomerDemoService = $BasicApplication_Northwind_CustomerCustomerDemoService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerDemographicDialog
	var $BasicApplication_Northwind_CustomerDemographicDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_CustomerDemographicDialog.__typeName = 'BasicApplication.Northwind.CustomerDemographicDialog';
	global.BasicApplication.Northwind.CustomerDemographicDialog = $BasicApplication_Northwind_CustomerDemographicDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerDemographicForm
	var $BasicApplication_Northwind_CustomerDemographicForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_CustomerDemographicForm.__typeName = 'BasicApplication.Northwind.CustomerDemographicForm';
	global.BasicApplication.Northwind.CustomerDemographicForm = $BasicApplication_Northwind_CustomerDemographicForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerDemographicGrid
	var $BasicApplication_Northwind_CustomerDemographicGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_CustomerDemographicGrid.__typeName = 'BasicApplication.Northwind.CustomerDemographicGrid';
	global.BasicApplication.Northwind.CustomerDemographicGrid = $BasicApplication_Northwind_CustomerDemographicGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerDemographicService
	var $BasicApplication_Northwind_CustomerDemographicService = function() {
	};
	$BasicApplication_Northwind_CustomerDemographicService.__typeName = 'BasicApplication.Northwind.CustomerDemographicService';
	$BasicApplication_Northwind_CustomerDemographicService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerDemographic/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerDemographicService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerDemographic/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerDemographicService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerDemographic/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerDemographicService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerDemographic/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerDemographicService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/CustomerDemographic/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.CustomerDemographicService = $BasicApplication_Northwind_CustomerDemographicService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerDialog
	var $BasicApplication_Northwind_CustomerDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_CustomerDialog.__typeName = 'BasicApplication.Northwind.CustomerDialog';
	global.BasicApplication.Northwind.CustomerDialog = $BasicApplication_Northwind_CustomerDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerForm
	var $BasicApplication_Northwind_CustomerForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_CustomerForm.__typeName = 'BasicApplication.Northwind.CustomerForm';
	global.BasicApplication.Northwind.CustomerForm = $BasicApplication_Northwind_CustomerForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerGrid
	var $BasicApplication_Northwind_CustomerGrid = function(container) {
		this.$country = null;
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_CustomerGrid.__typeName = 'BasicApplication.Northwind.CustomerGrid';
	global.BasicApplication.Northwind.CustomerGrid = $BasicApplication_Northwind_CustomerGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.CustomerService
	var $BasicApplication_Northwind_CustomerService = function() {
	};
	$BasicApplication_Northwind_CustomerService.__typeName = 'BasicApplication.Northwind.CustomerService';
	$BasicApplication_Northwind_CustomerService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Customer/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Customer/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Customer/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Customer/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_CustomerService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Customer/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.CustomerService = $BasicApplication_Northwind_CustomerService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeDialog
	var $BasicApplication_Northwind_EmployeeDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_EmployeeDialog.__typeName = 'BasicApplication.Northwind.EmployeeDialog';
	global.BasicApplication.Northwind.EmployeeDialog = $BasicApplication_Northwind_EmployeeDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeForm
	var $BasicApplication_Northwind_EmployeeForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_EmployeeForm.__typeName = 'BasicApplication.Northwind.EmployeeForm';
	global.BasicApplication.Northwind.EmployeeForm = $BasicApplication_Northwind_EmployeeForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeGrid
	var $BasicApplication_Northwind_EmployeeGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_EmployeeGrid.__typeName = 'BasicApplication.Northwind.EmployeeGrid';
	global.BasicApplication.Northwind.EmployeeGrid = $BasicApplication_Northwind_EmployeeGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeService
	var $BasicApplication_Northwind_EmployeeService = function() {
	};
	$BasicApplication_Northwind_EmployeeService.__typeName = 'BasicApplication.Northwind.EmployeeService';
	$BasicApplication_Northwind_EmployeeService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Employee/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Employee/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Employee/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Employee/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Employee/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.EmployeeService = $BasicApplication_Northwind_EmployeeService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeTerritoryDialog
	var $BasicApplication_Northwind_EmployeeTerritoryDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_EmployeeTerritoryDialog.__typeName = 'BasicApplication.Northwind.EmployeeTerritoryDialog';
	global.BasicApplication.Northwind.EmployeeTerritoryDialog = $BasicApplication_Northwind_EmployeeTerritoryDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeTerritoryForm
	var $BasicApplication_Northwind_EmployeeTerritoryForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_EmployeeTerritoryForm.__typeName = 'BasicApplication.Northwind.EmployeeTerritoryForm';
	global.BasicApplication.Northwind.EmployeeTerritoryForm = $BasicApplication_Northwind_EmployeeTerritoryForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeTerritoryGrid
	var $BasicApplication_Northwind_EmployeeTerritoryGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_EmployeeTerritoryGrid.__typeName = 'BasicApplication.Northwind.EmployeeTerritoryGrid';
	global.BasicApplication.Northwind.EmployeeTerritoryGrid = $BasicApplication_Northwind_EmployeeTerritoryGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.EmployeeTerritoryService
	var $BasicApplication_Northwind_EmployeeTerritoryService = function() {
	};
	$BasicApplication_Northwind_EmployeeTerritoryService.__typeName = 'BasicApplication.Northwind.EmployeeTerritoryService';
	$BasicApplication_Northwind_EmployeeTerritoryService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/EmployeeTerritory/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeTerritoryService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/EmployeeTerritory/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeTerritoryService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/EmployeeTerritory/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeTerritoryService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/EmployeeTerritory/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_EmployeeTerritoryService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/EmployeeTerritory/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.EmployeeTerritoryService = $BasicApplication_Northwind_EmployeeTerritoryService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderDetailDialog
	var $BasicApplication_Northwind_OrderDetailDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_OrderDetailDialog.__typeName = 'BasicApplication.Northwind.OrderDetailDialog';
	global.BasicApplication.Northwind.OrderDetailDialog = $BasicApplication_Northwind_OrderDetailDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderDetailForm
	var $BasicApplication_Northwind_OrderDetailForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_OrderDetailForm.__typeName = 'BasicApplication.Northwind.OrderDetailForm';
	global.BasicApplication.Northwind.OrderDetailForm = $BasicApplication_Northwind_OrderDetailForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderDetailGrid
	var $BasicApplication_Northwind_OrderDetailGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_OrderDetailGrid.__typeName = 'BasicApplication.Northwind.OrderDetailGrid';
	global.BasicApplication.Northwind.OrderDetailGrid = $BasicApplication_Northwind_OrderDetailGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderDetailService
	var $BasicApplication_Northwind_OrderDetailService = function() {
	};
	$BasicApplication_Northwind_OrderDetailService.__typeName = 'BasicApplication.Northwind.OrderDetailService';
	$BasicApplication_Northwind_OrderDetailService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/OrderDetail/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderDetailService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/OrderDetail/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderDetailService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/OrderDetail/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderDetailService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/OrderDetail/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderDetailService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/OrderDetail/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.OrderDetailService = $BasicApplication_Northwind_OrderDetailService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderDialog
	var $BasicApplication_Northwind_OrderDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_OrderDialog.__typeName = 'BasicApplication.Northwind.OrderDialog';
	global.BasicApplication.Northwind.OrderDialog = $BasicApplication_Northwind_OrderDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderForm
	var $BasicApplication_Northwind_OrderForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_OrderForm.__typeName = 'BasicApplication.Northwind.OrderForm';
	global.BasicApplication.Northwind.OrderForm = $BasicApplication_Northwind_OrderForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderGrid
	var $BasicApplication_Northwind_OrderGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_OrderGrid.__typeName = 'BasicApplication.Northwind.OrderGrid';
	global.BasicApplication.Northwind.OrderGrid = $BasicApplication_Northwind_OrderGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.OrderService
	var $BasicApplication_Northwind_OrderService = function() {
	};
	$BasicApplication_Northwind_OrderService.__typeName = 'BasicApplication.Northwind.OrderService';
	$BasicApplication_Northwind_OrderService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Order/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Order/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Order/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Order/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_OrderService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Order/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.OrderService = $BasicApplication_Northwind_OrderService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.PhoneEditor
	var $BasicApplication_Northwind_PhoneEditor = function(input) {
		this.$5$MultipleField = false;
		Serenity.StringEditor.call(this, input);
		Serenity.VX.addValidationRule(this, this.uniqueName, ss.mkdel(this, function(e) {
			var value = Q.trimToNull(this.get_value());
			if (ss.isNullOrUndefined(value)) {
				return null;
			}
			return $BasicApplication_Northwind_PhoneEditor.$validate(value, this.get_multiple());
		}));
		input.bind('change', ss.mkdel(this, function(e1) {
			if (!Serenity.WX.hasOriginalEvent(e1)) {
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
	$BasicApplication_Northwind_PhoneEditor.__typeName = 'BasicApplication.Northwind.PhoneEditor';
	$BasicApplication_Northwind_PhoneEditor.$validate = function(phone, isMultiple) {
		var valid = (isMultiple ? $BasicApplication_Northwind_PhoneEditor.$isValidMulti(phone, $BasicApplication_Northwind_PhoneEditor.$isValidPhone) : $BasicApplication_Northwind_PhoneEditor.$isValidPhone(phone));
		if (valid) {
			return null;
		}
		return Q.text((isMultiple ? 'Validation.NorthwindPhoneMultiple' : 'Validation.NorthwindPhone'));
	};
	$BasicApplication_Northwind_PhoneEditor.$isValidPhone = function(phone) {
		if (Q.isEmptyOrNull(phone)) {
			return false;
		}
		phone = ss.replaceAllString(ss.replaceAllString(phone, ' ', ''), '-', '');
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
	$BasicApplication_Northwind_PhoneEditor.$formatPhone = function(phone) {
		if (!$BasicApplication_Northwind_PhoneEditor.$isValidPhone(phone)) {
			return phone;
		}
		phone = ss.replaceAllString(ss.replaceAllString(ss.replaceAllString(ss.replaceAllString(phone, ' ', ''), '-', ''), '(', ''), ')', '');
		if (ss.startsWithString(phone, '0')) {
			phone = phone.substring(1);
		}
		phone = '(' + phone.substr(0, 3) + ') ' + phone.substr(3, 3) + '-' + phone.substr(6, 2) + phone.substr(8, 2);
		return phone;
	};
	$BasicApplication_Northwind_PhoneEditor.$formatMulti = function(phone, format) {
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
	$BasicApplication_Northwind_PhoneEditor.$isValidMulti = function(phone, check) {
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
	global.BasicApplication.Northwind.PhoneEditor = $BasicApplication_Northwind_PhoneEditor;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ProductDialog
	var $BasicApplication_Northwind_ProductDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_ProductDialog.__typeName = 'BasicApplication.Northwind.ProductDialog';
	global.BasicApplication.Northwind.ProductDialog = $BasicApplication_Northwind_ProductDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ProductForm
	var $BasicApplication_Northwind_ProductForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_ProductForm.__typeName = 'BasicApplication.Northwind.ProductForm';
	global.BasicApplication.Northwind.ProductForm = $BasicApplication_Northwind_ProductForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ProductGrid
	var $BasicApplication_Northwind_ProductGrid = function(container) {
		this.$supplier = null;
		this.$category = null;
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_ProductGrid.__typeName = 'BasicApplication.Northwind.ProductGrid';
	global.BasicApplication.Northwind.ProductGrid = $BasicApplication_Northwind_ProductGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ProductService
	var $BasicApplication_Northwind_ProductService = function() {
	};
	$BasicApplication_Northwind_ProductService.__typeName = 'BasicApplication.Northwind.ProductService';
	$BasicApplication_Northwind_ProductService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Product/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ProductService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Product/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ProductService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Product/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ProductService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Product/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ProductService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Product/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.ProductService = $BasicApplication_Northwind_ProductService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.RegionDialog
	var $BasicApplication_Northwind_RegionDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_RegionDialog.__typeName = 'BasicApplication.Northwind.RegionDialog';
	global.BasicApplication.Northwind.RegionDialog = $BasicApplication_Northwind_RegionDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.RegionForm
	var $BasicApplication_Northwind_RegionForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_RegionForm.__typeName = 'BasicApplication.Northwind.RegionForm';
	global.BasicApplication.Northwind.RegionForm = $BasicApplication_Northwind_RegionForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.RegionGrid
	var $BasicApplication_Northwind_RegionGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_RegionGrid.__typeName = 'BasicApplication.Northwind.RegionGrid';
	global.BasicApplication.Northwind.RegionGrid = $BasicApplication_Northwind_RegionGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.RegionService
	var $BasicApplication_Northwind_RegionService = function() {
	};
	$BasicApplication_Northwind_RegionService.__typeName = 'BasicApplication.Northwind.RegionService';
	$BasicApplication_Northwind_RegionService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Region/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_RegionService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Region/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_RegionService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Region/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_RegionService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Region/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_RegionService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Region/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.RegionService = $BasicApplication_Northwind_RegionService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ShipperDialog
	var $BasicApplication_Northwind_ShipperDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_ShipperDialog.__typeName = 'BasicApplication.Northwind.ShipperDialog';
	global.BasicApplication.Northwind.ShipperDialog = $BasicApplication_Northwind_ShipperDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ShipperForm
	var $BasicApplication_Northwind_ShipperForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_ShipperForm.__typeName = 'BasicApplication.Northwind.ShipperForm';
	global.BasicApplication.Northwind.ShipperForm = $BasicApplication_Northwind_ShipperForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ShipperGrid
	var $BasicApplication_Northwind_ShipperGrid = function(container) {
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_ShipperGrid.__typeName = 'BasicApplication.Northwind.ShipperGrid';
	global.BasicApplication.Northwind.ShipperGrid = $BasicApplication_Northwind_ShipperGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.ShipperService
	var $BasicApplication_Northwind_ShipperService = function() {
	};
	$BasicApplication_Northwind_ShipperService.__typeName = 'BasicApplication.Northwind.ShipperService';
	$BasicApplication_Northwind_ShipperService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Shipper/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ShipperService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Shipper/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ShipperService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Shipper/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ShipperService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Shipper/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_ShipperService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Shipper/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.ShipperService = $BasicApplication_Northwind_ShipperService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.SupplierDialog
	var $BasicApplication_Northwind_SupplierDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_SupplierDialog.__typeName = 'BasicApplication.Northwind.SupplierDialog';
	global.BasicApplication.Northwind.SupplierDialog = $BasicApplication_Northwind_SupplierDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.SupplierForm
	var $BasicApplication_Northwind_SupplierForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_SupplierForm.__typeName = 'BasicApplication.Northwind.SupplierForm';
	global.BasicApplication.Northwind.SupplierForm = $BasicApplication_Northwind_SupplierForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.SupplierGrid
	var $BasicApplication_Northwind_SupplierGrid = function(container) {
		this.$country = null;
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_SupplierGrid.__typeName = 'BasicApplication.Northwind.SupplierGrid';
	global.BasicApplication.Northwind.SupplierGrid = $BasicApplication_Northwind_SupplierGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.SupplierService
	var $BasicApplication_Northwind_SupplierService = function() {
	};
	$BasicApplication_Northwind_SupplierService.__typeName = 'BasicApplication.Northwind.SupplierService';
	$BasicApplication_Northwind_SupplierService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Supplier/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_SupplierService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Supplier/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_SupplierService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Supplier/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_SupplierService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Supplier/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_SupplierService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Supplier/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.SupplierService = $BasicApplication_Northwind_SupplierService;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.TerritoryDialog
	var $BasicApplication_Northwind_TerritoryDialog = function() {
		ss.makeGenericType(Serenity.EntityDialog$1, [Object]).call(this);
	};
	$BasicApplication_Northwind_TerritoryDialog.__typeName = 'BasicApplication.Northwind.TerritoryDialog';
	global.BasicApplication.Northwind.TerritoryDialog = $BasicApplication_Northwind_TerritoryDialog;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.TerritoryForm
	var $BasicApplication_Northwind_TerritoryForm = function(idPrefix) {
		Serenity.PrefixedContext.call(this, idPrefix);
	};
	$BasicApplication_Northwind_TerritoryForm.__typeName = 'BasicApplication.Northwind.TerritoryForm';
	global.BasicApplication.Northwind.TerritoryForm = $BasicApplication_Northwind_TerritoryForm;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.TerritoryGrid
	var $BasicApplication_Northwind_TerritoryGrid = function(container) {
		this.$region = null;
		ss.makeGenericType(Serenity.EntityGrid$1, [Object]).call(this, container);
	};
	$BasicApplication_Northwind_TerritoryGrid.__typeName = 'BasicApplication.Northwind.TerritoryGrid';
	global.BasicApplication.Northwind.TerritoryGrid = $BasicApplication_Northwind_TerritoryGrid;
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.Northwind.TerritoryService
	var $BasicApplication_Northwind_TerritoryService = function() {
	};
	$BasicApplication_Northwind_TerritoryService.__typeName = 'BasicApplication.Northwind.TerritoryService';
	$BasicApplication_Northwind_TerritoryService.create = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Territory/Create', request, onSuccess, options);
	};
	$BasicApplication_Northwind_TerritoryService.update = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Territory/Update', request, onSuccess, options);
	};
	$BasicApplication_Northwind_TerritoryService.delete$1 = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Territory/Delete', request, onSuccess, options);
	};
	$BasicApplication_Northwind_TerritoryService.retrieve = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Territory/Retrieve', request, onSuccess, options);
	};
	$BasicApplication_Northwind_TerritoryService.list = function(request, onSuccess, options) {
		return Q.serviceRequest('Northwind/Territory/List', request, onSuccess, options);
	};
	global.BasicApplication.Northwind.TerritoryService = $BasicApplication_Northwind_TerritoryService;
	ss.initClass($BasicApplication_ScriptInitialization, $asm, {});
	ss.initClass($BasicApplication_Administration_LanguageDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Administration_LanguageForm, $asm, {
		get_languageId: function() {
			return this.byId(Serenity.StringEditor).call(this, 'LanguageId');
		},
		get_languageName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'LanguageName');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Administration_LanguageGrid, $asm, {}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Administration_LanguageService, $asm, {});
	ss.initClass($BasicApplication_Administration_TranslationForm, $asm, {
		get_textKey: function() {
			return this.byId(Serenity.StringEditor).call(this, 'TextKey');
		},
		get_languageId: function() {
			return this.byId(Serenity.StringEditor).call(this, 'LanguageId');
		},
		get_translation: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Translation');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Administration_TranslationGrid, $asm, {
		onClick: function(e, row, cell) {
			ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onClick.call(this, e, row, cell);
			if (e.isDefaultPrevented()) {
				return;
			}
			if ($(e.target).hasClass('source-text')) {
				e.preventDefault();
				var item = this.view.rows[row];
				var done = ss.mkdel(this, function() {
					item.CustomText = item.SourceText;
					this.view.updateItem(item.Key, item);
					this.$hasChanges = true;
				});
				if (Q.isTrimmedEmpty(item.CustomText) || ss.referenceEquals(Q.trimToEmpty(item.CustomText), Q.trimToEmpty(item.SourceText))) {
					done();
					return;
				}
				Q.confirm(Q.text('Db.Administration.Translation.OverrideConfirmation'), done);
			}
			if ($(e.target).hasClass('target-text')) {
				e.preventDefault();
				var item1 = this.view.rows[row];
				var done1 = ss.mkdel(this, function() {
					item1.CustomText = item1.TargetText;
					this.view.updateItem(item1.Key, item1);
					this.$hasChanges = true;
				});
				if (Q.isTrimmedEmpty(item1.CustomText) || ss.referenceEquals(Q.trimToEmpty(item1.CustomText), Q.trimToEmpty(item1.TargetText))) {
					done1();
					return;
				}
				Q.confirm(Q.text('Db.Administration.Translation.OverrideConfirmation'), done1);
			}
		},
		getColumnsAsync: function() {
			var columns = [];
			columns.push({ field: 'Key', width: 300, sortable: false });
			columns.push({
				field: 'SourceText',
				width: 300,
				sortable: false,
				format: function(ctx) {
					return Q.outerHtml($('<a/>').addClass('source-text').text(ss.coalesce(ss.cast(ctx.value, String), '')));
				}
			});
			columns.push({
				field: 'CustomText',
				width: 300,
				sortable: false,
				format: function(ctx1) {
					return Q.outerHtml($('<input/>').addClass('custom-text').attr('value', ss.cast(ctx1.value, String)).attr('type', 'text').attr('data-key', ss.cast(ctx1.item.Key, String)));
				}
			});
			columns.push({
				field: 'TargetText',
				width: 300,
				sortable: false,
				format: function(ctx2) {
					return Q.outerHtml($('<a/>').addClass('target-text').text(ss.coalesce(ss.cast(ctx2.value, String), '')));
				}
			});
			return RSVP.resolve(columns);
		},
		createToolbarExtensions: function() {
			ss.makeGenericType(Serenity.EntityGrid$2, [Object, Object]).prototype.createToolbarExtensions.call(this);
			var $t2 = ss.mkdel(this, function(e) {
				e.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Administration.Translation.SourceLanguage') + ' ---');
			});
			var $t1 = Serenity.LookupEditorOptions.$ctor();
			$t1.lookupKey = 'Administration.Language';
			this.$sourceLanguage = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t2, $t1, null);
			Serenity.WX.changeSelect2(this.$sourceLanguage, ss.mkdel(this, function(e1) {
				if (this.$hasChanges) {
					this.saveChanges(this.$targetLanguageKey).then(ss.mkdel(this, this.refresh), null);
				}
				else {
					this.refresh();
				}
			}));
			var $t4 = ss.mkdel(this, function(e2) {
				e2.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Administration.Translation.TargetLanguage') + ' ---');
			});
			var $t3 = Serenity.LookupEditorOptions.$ctor();
			$t3.lookupKey = 'Administration.Language';
			this.$targetLanguage = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t4, $t3, null);
			Serenity.WX.changeSelect2(this.$targetLanguage, ss.mkdel(this, function(e3) {
				if (this.$hasChanges) {
					this.saveChanges(this.$targetLanguageKey).then(ss.mkdel(this, this.refresh), null);
				}
				else {
					this.refresh();
				}
			}));
		},
		saveChanges: function(language) {
			var translations = {};
			var $t1 = this.view.getItems();
			for (var $t2 = 0; $t2 < $t1.length; $t2++) {
				var item = $t1[$t2];
				translations[item.Key] = item.CustomText;
			}
			return RSVP.resolve($BasicApplication_Administration_TranslationService.update({ TargetLanguageID: language, Translations: translations }, null, null)).then(ss.mkdel(this, function() {
				this.$hasChanges = false;
				Q.notifySuccess('User translations in "' + language + '" language are saved to "user.texts.' + language + '.json" ' + 'file under "~/script/site/texts/user/"');
			}), null);
		},
		onViewSubmit: function() {
			var request = this.view.params;
			request.SourceLanguageID = this.$sourceLanguage.get_value();
			this.$targetLanguageKey = ss.coalesce(this.$targetLanguage.get_value(), '');
			request.TargetLanguageID = this.$targetLanguageKey;
			this.$hasChanges = false;
			return ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewSubmit.call(this);
		},
		getButtons: function() {
			var $t1 = [];
			$t1.push({ title: 'Save Changes', onClick: ss.mkdel(this, function(e) {
				this.saveChanges(this.$targetLanguageKey).then(ss.mkdel(this, this.refresh), null);
			}), cssClass: 'apply-changes-button' });
			return $t1;
		},
		createQuickSearchInput: function() {
			Serenity.GridUtils.addQuickSearchInputCustom(this.toolbar.get_element(), ss.mkdel(this, function(field, searchText) {
				this.$searchText = searchText;
				this.view.setItems(this.view.getItems(), true);
			}), null);
		},
		onViewFilter: function(item) {
			if (!ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewFilter.call(this, item)) {
				return false;
			}
			if (Q.isEmptyOrNull(this.$searchText)) {
				return true;
			}
			var searching = Select2.util.stripDiacritics(this.$searchText).toLowerCase();
			if (Q.isEmptyOrNull(searching)) {
				return true;
			}
			if (Select2.util.stripDiacritics(ss.coalesce(item.Key, '')).toLowerCase().indexOf(searching) >= 0) {
				return true;
			}
			if (Select2.util.stripDiacritics(ss.coalesce(item.SourceText, '')).toLowerCase().indexOf(searching) >= 0) {
				return true;
			}
			if (Select2.util.stripDiacritics(ss.coalesce(item.TargetText, '')).toLowerCase().indexOf(searching) >= 0) {
				return true;
			}
			if (Select2.util.stripDiacritics(ss.coalesce(item.CustomText, '')).toLowerCase().indexOf(searching) >= 0) {
				return true;
			}
			return false;
		},
		usePager: function() {
			return false;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Administration_TranslationService, $asm, {});
	ss.initClass($BasicApplication_Administration_UserDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog]);
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
			columns.push({ field: 'UserId', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'Username', width: 150, format: this.itemLink(null, null, null, null) });
			columns.push({ field: 'DisplayName', width: 150 });
			columns.push({ field: 'Email', width: 250 });
			columns.push({ field: 'Source', width: 100 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Administration_UserService, $asm, {});
	ss.initClass($BasicApplication_Common_LanguageSelection, $asm, {
		getLookupAsync: function() {
			return ss.makeGenericType(Serenity.LookupEditorBase$2, [Object, Object]).prototype.getLookupAsync.call(this).then(ss.mkdel(this, function(x) {
				if (!Enumerable.from(x.get_items()).any(ss.mkdel(this, function(z) {
					return ss.referenceEquals(z.LanguageId, this.$currentLanguage);
				}))) {
					var idx = this.$currentLanguage.lastIndexOf('-');
					if (idx >= 0) {
						this.$currentLanguage = this.$currentLanguage.substr(0, idx);
						if (!Enumerable.from(x.get_items()).any(ss.mkdel(this, function(z1) {
							return ss.referenceEquals(z1.LanguageId, this.$currentLanguage);
						}))) {
							this.$currentLanguage = 'en';
						}
					}
					else {
						this.$currentLanguage = 'en';
					}
				}
				return x;
			}), null);
		},
		updateItemsAsync: function() {
			return ss.makeGenericType(Serenity.LookupEditorBase$2, [Object, Object]).prototype.updateItemsAsync.call(this).then(ss.mkdel(this, function() {
				this.set_value(this.$currentLanguage);
			}), null);
		},
		getLookupKey: function() {
			return 'Administration.Language';
		},
		emptyItemText: function() {
			return null;
		}
	}, ss.makeGenericType(Serenity.LookupEditorBase$1, [Object]), [Serenity.IStringValue, Serenity.IAsyncInit]);
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
			return this.byId(Serenity.PasswordEditor).call(this, 'Password');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Membership_LoginPanel, $asm, {}, ss.makeGenericType(Serenity.PropertyDialog$1, [Object]), [Serenity.IDialog]);
	ss.initClass($BasicApplication_Northwind_CategoryDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CategoryForm, $asm, {
		get_categoryName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CategoryName');
		},
		get_description: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Description');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_CategoryGrid, $asm, {}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CategoryService, $asm, {});
	ss.initClass($BasicApplication_Northwind_CustomerCustomerDemoDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CustomerCustomerDemoForm, $asm, {
		get_customerID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerID');
		},
		get_customerTypeID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerTypeID');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_CustomerCustomerDemoGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'ID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'CustomerID', width: 200, format: this.itemLink(null, null, null, null) });
			columns.push({ field: 'CustomerTypeID', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_CustomerCustomerDemoService, $asm, {});
	ss.initClass($BasicApplication_Northwind_CustomerDemographicDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CustomerDemographicForm, $asm, {
		get_customerTypeID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerTypeID');
		},
		get_customerDesc: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerDesc');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_CustomerDemographicGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'ID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'CustomerTypeID', width: 200, format: this.itemLink(null, null, null, null) });
			columns.push({ field: 'CustomerDesc', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_CustomerDemographicService, $asm, {});
	ss.initClass($BasicApplication_Northwind_CustomerDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CustomerForm, $asm, {
		get_customerID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerID');
		},
		get_companyName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CompanyName');
		},
		get_contactName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ContactName');
		},
		get_contactTitle: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ContactTitle');
		},
		get_address: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Address');
		},
		get_city: function() {
			return this.byId(Serenity.StringEditor).call(this, 'City');
		},
		get_region: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Region');
		},
		get_postalCode: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PostalCode');
		},
		get_country: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Country');
		},
		get_phone: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Phone');
		},
		get_fax: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Fax');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_CustomerGrid, $asm, {
		createToolbarExtensions: function() {
			ss.makeGenericType(Serenity.EntityGrid$2, [Object, Object]).prototype.createToolbarExtensions.call(this);
			var $t2 = ss.mkdel(this, function(e) {
				e.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Northwind.Customer.Country') + ' ---');
			});
			var $t1 = Serenity.LookupEditorOptions.$ctor();
			$t1.lookupKey = 'Northwind.CustomerCountry';
			this.$country = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t2, $t1, null);
			Serenity.WX.change(this.$country, ss.mkdel(this, function(e1) {
				this.refresh();
			}));
		},
		onViewSubmit: function() {
			if (!ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewSubmit.call(this)) {
				return false;
			}
			var req = this.view.params;
			req.EqualityFilter = req.EqualityFilter || {};
			req.EqualityFilter['Country'] = this.$country.get_value();
			return true;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_CustomerService, $asm, {});
	ss.initClass($BasicApplication_Northwind_EmployeeDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_EmployeeForm, $asm, {
		get_lastName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'LastName');
		},
		get_firstName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'FirstName');
		},
		get_title: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Title');
		},
		get_titleOfCourtesy: function() {
			return this.byId(Serenity.StringEditor).call(this, 'TitleOfCourtesy');
		},
		get_birthDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'BirthDate');
		},
		get_hireDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'HireDate');
		},
		get_address: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Address');
		},
		get_city: function() {
			return this.byId(Serenity.StringEditor).call(this, 'City');
		},
		get_region: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Region');
		},
		get_postalCode: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PostalCode');
		},
		get_country: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Country');
		},
		get_homePhone: function() {
			return this.byId(Serenity.StringEditor).call(this, 'HomePhone');
		},
		get_extension: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Extension');
		},
		get_photo: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Photo');
		},
		get_notes: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Notes');
		},
		get_reportsTo: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'ReportsTo');
		},
		get_photoPath: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PhotoPath');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_EmployeeGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'EmployeeID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'LastName', width: 200, format: this.itemLink(null, null, null, null) });
			columns.push({ field: 'FirstName', width: 80 });
			columns.push({ field: 'Title', width: 80 });
			columns.push({ field: 'TitleOfCourtesy', width: 80 });
			columns.push({ field: 'BirthDate', width: 80 });
			columns.push({ field: 'HireDate', width: 80 });
			columns.push({ field: 'Address', width: 80 });
			columns.push({ field: 'City', width: 80 });
			columns.push({ field: 'Region', width: 80 });
			columns.push({ field: 'PostalCode', width: 80 });
			columns.push({ field: 'Country', width: 80 });
			columns.push({ field: 'HomePhone', width: 80 });
			columns.push({ field: 'Extension', width: 80 });
			columns.push({ field: 'Photo', width: 80 });
			columns.push({ field: 'Notes', width: 80 });
			columns.push({ field: 'ReportsTo', width: 80 });
			columns.push({ field: 'PhotoPath', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_EmployeeService, $asm, {});
	ss.initClass($BasicApplication_Northwind_EmployeeTerritoryDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_EmployeeTerritoryForm, $asm, {
		get_territoryID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'TerritoryID');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_EmployeeTerritoryGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'EmployeeID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'TerritoryID', width: 200, format: this.itemLink(null, null, null, null) });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_EmployeeTerritoryService, $asm, {});
	ss.initClass($BasicApplication_Northwind_OrderDetailDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_OrderDetailForm, $asm, {
		get_productID: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'ProductID');
		},
		get_unitPrice: function() {
			return this.byId(Serenity.DecimalEditor).call(this, 'UnitPrice');
		},
		get_quantity: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Quantity');
		},
		get_discount: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Discount');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_OrderDetailGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'OrderID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'ProductID', width: 80 });
			columns.push({ field: 'UnitPrice', width: 80 });
			columns.push({ field: 'Quantity', width: 80 });
			columns.push({ field: 'Discount', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_OrderDetailService, $asm, {});
	ss.initClass($BasicApplication_Northwind_OrderDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_OrderForm, $asm, {
		get_customerID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CustomerID');
		},
		get_employeeID: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'EmployeeID');
		},
		get_orderDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'OrderDate');
		},
		get_requiredDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'RequiredDate');
		},
		get_shippedDate: function() {
			return this.byId(Serenity.DateEditor).call(this, 'ShippedDate');
		},
		get_shipVia: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'ShipVia');
		},
		get_freight: function() {
			return this.byId(Serenity.DecimalEditor).call(this, 'Freight');
		},
		get_shipName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipName');
		},
		get_shipAddress: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipAddress');
		},
		get_shipCity: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipCity');
		},
		get_shipRegion: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipRegion');
		},
		get_shipPostalCode: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipPostalCode');
		},
		get_shipCountry: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ShipCountry');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_OrderGrid, $asm, {
		getColumns: function() {
			var columns = ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.getColumns.call(this);
			columns.push({ field: 'OrderID', width: 55, cssClass: 'align-right', name: Q.text('Db.Shared.RecordId') });
			columns.push({ field: 'CustomerID', width: 200, format: this.itemLink(null, null, null, null) });
			columns.push({ field: 'EmployeeID', width: 80 });
			columns.push({ field: 'OrderDate', width: 80 });
			columns.push({ field: 'RequiredDate', width: 80 });
			columns.push({ field: 'ShippedDate', width: 80 });
			columns.push({ field: 'ShipVia', width: 80 });
			columns.push({ field: 'Freight', width: 80 });
			columns.push({ field: 'ShipName', width: 80 });
			columns.push({ field: 'ShipAddress', width: 80 });
			columns.push({ field: 'ShipCity', width: 80 });
			columns.push({ field: 'ShipRegion', width: 80 });
			columns.push({ field: 'ShipPostalCode', width: 80 });
			columns.push({ field: 'ShipCountry', width: 80 });
			return columns;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid]);
	ss.initClass($BasicApplication_Northwind_OrderService, $asm, {});
	ss.initClass($BasicApplication_Northwind_PhoneEditor, $asm, {
		formatValue: function() {
			this.element.val(this.getFormattedValue());
		},
		getFormattedValue: function() {
			var value = this.element.val();
			if (this.get_multiple()) {
				return $BasicApplication_Northwind_PhoneEditor.$formatMulti(value, $BasicApplication_Northwind_PhoneEditor.$formatPhone);
			}
			return $BasicApplication_Northwind_PhoneEditor.$formatPhone(value);
		},
		get_multiple: function() {
			return this.$5$MultipleField;
		},
		set_multiple: function(value) {
			this.$5$MultipleField = value;
		},
		get_value: function() {
			return this.getFormattedValue();
		},
		set_value: function(value) {
			this.element.val(value);
		}
	}, Serenity.StringEditor, [Serenity.IStringValue]);
	ss.initClass($BasicApplication_Northwind_ProductDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_ProductForm, $asm, {
		get_productName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ProductName');
		},
		get_discontinued: function() {
			return this.byId(Serenity.BooleanEditor).call(this, 'Discontinued');
		},
		get_supplierID: function() {
			return this.byId(Serenity.LookupEditor).call(this, 'SupplierID');
		},
		get_categoryID: function() {
			return this.byId(Serenity.LookupEditor).call(this, 'CategoryID');
		},
		get_quantityPerUnit: function() {
			return this.byId(Serenity.StringEditor).call(this, 'QuantityPerUnit');
		},
		get_unitPrice: function() {
			return this.byId(Serenity.DecimalEditor).call(this, 'UnitPrice');
		},
		get_unitsInStock: function() {
			return this.byId(Serenity.StringEditor).call(this, 'UnitsInStock');
		},
		get_unitsOnOrder: function() {
			return this.byId(Serenity.StringEditor).call(this, 'UnitsOnOrder');
		},
		get_reorderLevel: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ReorderLevel');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_ProductGrid, $asm, {
		createToolbarExtensions: function() {
			ss.makeGenericType(Serenity.EntityGrid$2, [Object, Object]).prototype.createToolbarExtensions.call(this);
			var $t2 = ss.mkdel(this, function(e) {
				e.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Northwind.Product.SupplierCompanyName') + ' ---');
			});
			var $t1 = Serenity.LookupEditorOptions.$ctor();
			$t1.lookupKey = 'Northwind.Supplier';
			this.$supplier = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t2, $t1, null);
			Serenity.WX.change(this.$supplier, ss.mkdel(this, function(e1) {
				this.refresh();
			}));
			var $t4 = ss.mkdel(this, function(e2) {
				e2.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Northwind.Product.CategoryName') + ' ---');
			});
			var $t3 = Serenity.LookupEditorOptions.$ctor();
			$t3.lookupKey = 'Northwind.Category';
			this.$category = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t4, $t3, null);
			Serenity.WX.change(this.$category, ss.mkdel(this, function(e3) {
				this.refresh();
			}));
		},
		onViewSubmit: function() {
			if (!ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewSubmit.call(this)) {
				return false;
			}
			var req = this.view.params;
			req.EqualityFilter = req.EqualityFilter || {};
			req.EqualityFilter['SupplierID'] = Serenity.IdExtensions.convertToId(this.$supplier.get_value());
			req.EqualityFilter['CategoryID'] = Serenity.IdExtensions.convertToId(this.$category.get_value());
			return true;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_ProductService, $asm, {});
	ss.initClass($BasicApplication_Northwind_RegionDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_RegionForm, $asm, {
		get_regionID: function() {
			return this.byId(Serenity.IntegerEditor).call(this, 'RegionID');
		},
		get_regionDescription: function() {
			return this.byId(Serenity.StringEditor).call(this, 'RegionDescription');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_RegionGrid, $asm, {}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_RegionService, $asm, {});
	ss.initClass($BasicApplication_Northwind_ShipperDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_ShipperForm, $asm, {
		get_companyName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CompanyName');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_ShipperGrid, $asm, {}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_ShipperService, $asm, {});
	ss.initClass($BasicApplication_Northwind_SupplierDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_SupplierForm, $asm, {
		get_companyName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'CompanyName');
		},
		get_contactName: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ContactName');
		},
		get_contactTitle: function() {
			return this.byId(Serenity.StringEditor).call(this, 'ContactTitle');
		},
		get_address: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Address');
		},
		get_city: function() {
			return this.byId(Serenity.StringEditor).call(this, 'City');
		},
		get_region: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Region');
		},
		get_postalCode: function() {
			return this.byId(Serenity.StringEditor).call(this, 'PostalCode');
		},
		get_country: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Country');
		},
		get_phone: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Phone');
		},
		get_fax: function() {
			return this.byId(Serenity.StringEditor).call(this, 'Fax');
		},
		get_homePage: function() {
			return this.byId(Serenity.StringEditor).call(this, 'HomePage');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_SupplierGrid, $asm, {
		createToolbarExtensions: function() {
			ss.makeGenericType(Serenity.EntityGrid$2, [Object, Object]).prototype.createToolbarExtensions.call(this);
			var $t2 = ss.mkdel(this, function(e) {
				e.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Northwind.Supplier.Country') + ' ---');
			});
			var $t1 = Serenity.LookupEditorOptions.$ctor();
			$t1.lookupKey = 'Northwind.SupplierCountry';
			this.$country = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t2, $t1, null);
			Serenity.WX.change(this.$country, ss.mkdel(this, function(e1) {
				this.refresh();
			}));
		},
		onViewSubmit: function() {
			if (!ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewSubmit.call(this)) {
				return false;
			}
			var req = this.view.params;
			req.EqualityFilter = req.EqualityFilter || {};
			req.EqualityFilter['Country'] = this.$country.get_value();
			return true;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_SupplierService, $asm, {});
	ss.initClass($BasicApplication_Northwind_TerritoryDialog, $asm, {}, ss.makeGenericType(Serenity.EntityDialog$1, [Object]), [Serenity.IDialog, Serenity.IEditDialog, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_TerritoryForm, $asm, {
		get_territoryID: function() {
			return this.byId(Serenity.StringEditor).call(this, 'TerritoryID');
		},
		get_territoryDescription: function() {
			return this.byId(Serenity.StringEditor).call(this, 'TerritoryDescription');
		},
		get_regionID: function() {
			return this.byId(Serenity.LookupEditor).call(this, 'RegionID');
		}
	}, Serenity.PrefixedContext);
	ss.initClass($BasicApplication_Northwind_TerritoryGrid, $asm, {
		createToolbarExtensions: function() {
			ss.makeGenericType(Serenity.EntityGrid$2, [Object, Object]).prototype.createToolbarExtensions.call(this);
			var $t2 = ss.mkdel(this, function(e) {
				e.appendTo(this.toolbar.get_element()).attr('placeholder', '--- ' + Q.text('Db.Northwind.Territory.RegionDescription') + ' ---');
			});
			var $t1 = Serenity.LookupEditorOptions.$ctor();
			$t1.lookupKey = 'Northwind.Region';
			this.$region = Serenity.Widget.create(Serenity.LookupEditor).call(null, $t2, $t1, null);
			Serenity.WX.change(this.$region, ss.mkdel(this, function(e1) {
				this.refresh();
			}));
		},
		onViewSubmit: function() {
			if (!ss.makeGenericType(Serenity.DataGrid$2, [Object, Object]).prototype.onViewSubmit.call(this)) {
				return false;
			}
			var req = this.view.params;
			req.EqualityFilter = req.EqualityFilter || {};
			req.EqualityFilter['RegionID'] = Serenity.IdExtensions.convertToId(this.$region.get_value());
			return true;
		}
	}, ss.makeGenericType(Serenity.EntityGrid$1, [Object]), [Serenity.IDataGrid, Serenity.IAsyncInit]);
	ss.initClass($BasicApplication_Northwind_TerritoryService, $asm, {});
	ss.setMetadata($BasicApplication_Administration_LanguageDialog, { attr: [new Serenity.IdPropertyAttribute('Id'), new Serenity.NamePropertyAttribute('LanguageName'), new Serenity.FormKeyAttribute('Administration.Language'), new Serenity.LocalTextPrefixAttribute('Administration.Language'), new Serenity.ServiceAttribute('Administration/Language')] });
	ss.setMetadata($BasicApplication_Administration_LanguageGrid, { attr: [new Serenity.ColumnsKeyAttribute('Administration.Language'), new Serenity.IdPropertyAttribute('Id'), new Serenity.NamePropertyAttribute('LanguageName'), new Serenity.DialogTypeAttribute($BasicApplication_Administration_LanguageDialog), new Serenity.LocalTextPrefixAttribute('Administration.Language'), new Serenity.ServiceAttribute('Administration/Language')] });
	ss.setMetadata($BasicApplication_Administration_TranslationGrid, { attr: [new Serenity.ColumnsKeyAttribute('Administration.Translation'), new Serenity.IdPropertyAttribute('Key'), new Serenity.LocalTextPrefixAttribute('Administration.Translation'), new Serenity.ServiceAttribute('Administration/Translation')] });
	ss.setMetadata($BasicApplication_Administration_UserDialog, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.FormKeyAttribute('Administration.User'), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Administration_UserGrid, { attr: [new Serenity.IdPropertyAttribute('UserId'), new Serenity.NamePropertyAttribute('Username'), new Serenity.IsActivePropertyAttribute('IsActive'), new Serenity.DialogTypeAttribute($BasicApplication_Administration_UserDialog), new Serenity.LocalTextPrefixAttribute('Administration.User'), new Serenity.ServiceAttribute('Administration/User')] });
	ss.setMetadata($BasicApplication_Membership_LoginPanel, { attr: [new Serenity.PanelAttribute(), new Serenity.FormKeyAttribute('Membership.Login')] });
	ss.setMetadata($BasicApplication_Northwind_CategoryDialog, { attr: [new Serenity.IdPropertyAttribute('CategoryID'), new Serenity.NamePropertyAttribute('CategoryName'), new Serenity.FormKeyAttribute('Northwind.Category'), new Serenity.LocalTextPrefixAttribute('Northwind.Category'), new Serenity.ServiceAttribute('Northwind/Category')] });
	ss.setMetadata($BasicApplication_Northwind_CategoryGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Category'), new Serenity.IdPropertyAttribute('CategoryID'), new Serenity.NamePropertyAttribute('CategoryName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_CategoryDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Category'), new Serenity.ServiceAttribute('Northwind/Category')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerCustomerDemoDialog, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.FormKeyAttribute('Northwind.CustomerCustomerDemo'), new Serenity.LocalTextPrefixAttribute('Northwind.CustomerCustomerDemo'), new Serenity.ServiceAttribute('Northwind/CustomerCustomerDemo')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerCustomerDemoGrid, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_CustomerCustomerDemoDialog), new Serenity.LocalTextPrefixAttribute('Northwind.CustomerCustomerDemo'), new Serenity.ServiceAttribute('Northwind/CustomerCustomerDemo')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerDemographicDialog, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerTypeID'), new Serenity.FormKeyAttribute('Northwind.CustomerDemographic'), new Serenity.LocalTextPrefixAttribute('Northwind.CustomerDemographic'), new Serenity.ServiceAttribute('Northwind/CustomerDemographic')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerDemographicGrid, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerTypeID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_CustomerDemographicDialog), new Serenity.LocalTextPrefixAttribute('Northwind.CustomerDemographic'), new Serenity.ServiceAttribute('Northwind/CustomerDemographic')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerDialog, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.FormKeyAttribute('Northwind.Customer'), new Serenity.LocalTextPrefixAttribute('Northwind.Customer'), new Serenity.ServiceAttribute('Northwind/Customer')] });
	ss.setMetadata($BasicApplication_Northwind_CustomerGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Customer'), new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_CustomerDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Customer'), new Serenity.ServiceAttribute('Northwind/Customer')] });
	ss.setMetadata($BasicApplication_Northwind_EmployeeDialog, { attr: [new Serenity.IdPropertyAttribute('EmployeeID'), new Serenity.NamePropertyAttribute('LastName'), new Serenity.FormKeyAttribute('Northwind.Employee'), new Serenity.LocalTextPrefixAttribute('Northwind.Employee'), new Serenity.ServiceAttribute('Northwind/Employee')] });
	ss.setMetadata($BasicApplication_Northwind_EmployeeGrid, { attr: [new Serenity.IdPropertyAttribute('EmployeeID'), new Serenity.NamePropertyAttribute('LastName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_EmployeeDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Employee'), new Serenity.ServiceAttribute('Northwind/Employee')] });
	ss.setMetadata($BasicApplication_Northwind_EmployeeTerritoryDialog, { attr: [new Serenity.IdPropertyAttribute('EmployeeID'), new Serenity.NamePropertyAttribute('TerritoryID'), new Serenity.FormKeyAttribute('Northwind.EmployeeTerritory'), new Serenity.LocalTextPrefixAttribute('Northwind.EmployeeTerritory'), new Serenity.ServiceAttribute('Northwind/EmployeeTerritory')] });
	ss.setMetadata($BasicApplication_Northwind_EmployeeTerritoryGrid, { attr: [new Serenity.IdPropertyAttribute('EmployeeID'), new Serenity.NamePropertyAttribute('TerritoryID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_EmployeeTerritoryDialog), new Serenity.LocalTextPrefixAttribute('Northwind.EmployeeTerritory'), new Serenity.ServiceAttribute('Northwind/EmployeeTerritory')] });
	ss.setMetadata($BasicApplication_Northwind_OrderDetailDialog, { attr: [new Serenity.IdPropertyAttribute('OrderID'), new Serenity.FormKeyAttribute('Northwind.OrderDetail'), new Serenity.LocalTextPrefixAttribute('Northwind.OrderDetail'), new Serenity.ServiceAttribute('Northwind/OrderDetail')] });
	ss.setMetadata($BasicApplication_Northwind_OrderDetailGrid, { attr: [new Serenity.IdPropertyAttribute('OrderID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_OrderDetailDialog), new Serenity.LocalTextPrefixAttribute('Northwind.OrderDetail'), new Serenity.ServiceAttribute('Northwind/OrderDetail')] });
	ss.setMetadata($BasicApplication_Northwind_OrderDialog, { attr: [new Serenity.IdPropertyAttribute('OrderID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.FormKeyAttribute('Northwind.Order'), new Serenity.LocalTextPrefixAttribute('Northwind.Order'), new Serenity.ServiceAttribute('Northwind/Order')] });
	ss.setMetadata($BasicApplication_Northwind_OrderGrid, { attr: [new Serenity.IdPropertyAttribute('OrderID'), new Serenity.NamePropertyAttribute('CustomerID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_OrderDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Order'), new Serenity.ServiceAttribute('Northwind/Order')] });
	ss.setMetadata($BasicApplication_Northwind_PhoneEditor, { attr: [new Serenity.EditorAttribute()], members: [{ attr: [new Serenity.ComponentModel.OptionAttribute()], name: 'Multiple', type: 16, returnType: Boolean, getter: { name: 'get_Multiple', type: 8, sname: 'get_multiple', returnType: Boolean, params: [] }, setter: { name: 'set_Multiple', type: 8, sname: 'set_multiple', returnType: Object, params: [Boolean] } }] });
	ss.setMetadata($BasicApplication_Northwind_ProductDialog, { attr: [new Serenity.IdPropertyAttribute('ProductID'), new Serenity.NamePropertyAttribute('ProductName'), new Serenity.FormKeyAttribute('Northwind.Product'), new Serenity.LocalTextPrefixAttribute('Northwind.Product'), new Serenity.ServiceAttribute('Northwind/Product')] });
	ss.setMetadata($BasicApplication_Northwind_ProductGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Product'), new Serenity.IdPropertyAttribute('ProductID'), new Serenity.NamePropertyAttribute('ProductName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_ProductDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Product'), new Serenity.ServiceAttribute('Northwind/Product')] });
	ss.setMetadata($BasicApplication_Northwind_RegionDialog, { attr: [new Serenity.IdPropertyAttribute('RegionID'), new Serenity.NamePropertyAttribute('RegionDescription'), new Serenity.FormKeyAttribute('Northwind.Region'), new Serenity.LocalTextPrefixAttribute('Northwind.Region'), new Serenity.ServiceAttribute('Northwind/Region')] });
	ss.setMetadata($BasicApplication_Northwind_RegionGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Region'), new Serenity.IdPropertyAttribute('RegionID'), new Serenity.NamePropertyAttribute('RegionDescription'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_RegionDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Region'), new Serenity.ServiceAttribute('Northwind/Region')] });
	ss.setMetadata($BasicApplication_Northwind_ShipperDialog, { attr: [new Serenity.IdPropertyAttribute('ShipperID'), new Serenity.NamePropertyAttribute('CompanyName'), new Serenity.FormKeyAttribute('Northwind.Shipper'), new Serenity.LocalTextPrefixAttribute('Northwind.Shipper'), new Serenity.ServiceAttribute('Northwind/Shipper')] });
	ss.setMetadata($BasicApplication_Northwind_ShipperGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Shipper'), new Serenity.IdPropertyAttribute('ShipperID'), new Serenity.NamePropertyAttribute('CompanyName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_ShipperDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Shipper'), new Serenity.ServiceAttribute('Northwind/Shipper')] });
	ss.setMetadata($BasicApplication_Northwind_SupplierDialog, { attr: [new Serenity.IdPropertyAttribute('SupplierID'), new Serenity.NamePropertyAttribute('CompanyName'), new Serenity.FormKeyAttribute('Northwind.Supplier'), new Serenity.LocalTextPrefixAttribute('Northwind.Supplier'), new Serenity.ServiceAttribute('Northwind/Supplier')] });
	ss.setMetadata($BasicApplication_Northwind_SupplierGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Supplier'), new Serenity.IdPropertyAttribute('SupplierID'), new Serenity.NamePropertyAttribute('CompanyName'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_SupplierDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Supplier'), new Serenity.ServiceAttribute('Northwind/Supplier')] });
	ss.setMetadata($BasicApplication_Northwind_TerritoryDialog, { attr: [new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('TerritoryID'), new Serenity.FormKeyAttribute('Northwind.Territory'), new Serenity.LocalTextPrefixAttribute('Northwind.Territory'), new Serenity.ServiceAttribute('Northwind/Territory')] });
	ss.setMetadata($BasicApplication_Northwind_TerritoryGrid, { attr: [new Serenity.ColumnsKeyAttribute('Northwind.Territory'), new Serenity.IdPropertyAttribute('ID'), new Serenity.NamePropertyAttribute('TerritoryID'), new Serenity.DialogTypeAttribute($BasicApplication_Northwind_TerritoryDialog), new Serenity.LocalTextPrefixAttribute('Northwind.Territory'), new Serenity.ServiceAttribute('Northwind/Territory')] });
	(function() {
		Q$Config.rootNamespaces.push('BasicApplication');
	})();
})();

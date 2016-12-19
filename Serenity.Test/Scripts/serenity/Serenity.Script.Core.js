(function() {
	'use strict';
	var $asm = {};
	global.Serenity = global.Serenity || {};
	global.Serenity.ComponentModel = global.Serenity.ComponentModel || {};
	ss.initAssembly($asm, 'Serenity.Script.Core');
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Q
	var $Q = function() {
	};
	$Q.__typeName = 'Q';
	$Q.$blockUIWithCheck = function(options) {
		if ($Q.$blockUICount > 0) {
			$Q.$blockUICount++;
			return;
		}
		$.blockUI(options);
		$Q.$blockUICount++;
	};
	$Q.blockUI = function(options) {
		options = $.extend({ baseZ: 2000, message: '', overlayCSS: { opacity: '0.0', zIndex: 2000, cursor: 'wait' }, fadeOut: 0 }, options);
		if (options.useTimeout) {
			window.setTimeout(function() {
				$Q.$blockUIWithCheck(options);
			}, 0);
		}
		else {
			$Q.$blockUIWithCheck(options);
		}
	};
	$Q.blockUndo = function() {
		if ($Q.$blockUICount > 1) {
			$Q.$blockUICount--;
			return;
		}
		$Q.$blockUICount--;
		$.unblockUI({ fadeOut: 0 });
	};
	$Q.formatDate = function(date, format) {
		if (ss.staticEquals(date, null)) {
			return '';
		}
		if (ss.isNullOrUndefined(format)) {
			format = $Q$Culture.dateFormat;
		}
		var pad = function(i) {
			return ss.padLeftString(i.toString(), 2, 48);
		};
		return format.replace(new RegExp('dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/', 'g'), function(fmt) {
			switch (fmt) {
				case '/': {
					return $Q$Culture.dateSeparator;
				}
				case 'hh': {
					return pad(((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12)));
				}
				case 'h': {
					return ((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12));
				}
				case 'HH': {
					return pad(date.getHours());
				}
				case 'H': {
					return date.getHours();
				}
				case 'mm': {
					return pad(date.getMinutes());
				}
				case 'm': {
					return date.getMinutes();
				}
				case 'ss': {
					return pad(date.getSeconds());
				}
				case 's': {
					return date.getSeconds();
				}
				case 'yyyy': {
					return date.getFullYear();
				}
				case 'yy': {
					return date.getFullYear().toString().substr(2, 4);
				}
				case 'dddd': {
					return ss.cast(date.GetDayName(), String);
				}
				case 'ddd': {
					return ss.cast(date.GetDayName(true), String);
				}
				case 'dd': {
					return pad(date.getDate());
				}
				case 'd': {
					return date.getDate().toString();
				}
				case 'MM': {
					return pad(date.getMonth() + 1);
				}
				case 'M': {
					return date.getMonth() + 1;
				}
				case 't': {
					return ((date.getHours() < 12) ? 'A' : 'P');
				}
				case 'tt': {
					return ((date.getHours() < 12) ? 'AM' : 'PM');
				}
				case 'fff': {
					return ss.padLeftString(date.getMilliseconds().toString(), 3, 48);
				}
				case 'zzz':
				case 'zz':
				case 'z': {
					return '';
				}
				default: {
					return fmt;
				}
			}
		});
	};
	$Q.parseDate = function(value) {
		return Q$Externals.parseDate(value);
	};
	$Q.parseISODateTime = function(value) {
		return Q$Externals.parseISODateTime(value);
	};
	$Q.alert = function(message, options) {
		Q$Externals.alertDialog(message, options);
	};
	$Q.warning = function(message, options) {
		Q$Externals.alertDialog(message, $.extend({ title: $Texts$Dialogs.WarningTitle.get(), dialogClass: 's-MessageDialog s-WarningDialog' }, options));
	};
	$Q.confirm = function(message, onYes, options) {
		Q$Externals.confirmDialog(message, onYes, options);
	};
	$Q.information = function(message, onOk, options) {
		Q$Externals.confirmDialog(message, onOk, $.extend({ title: $Texts$Dialogs.InformationTitle.get(), yesButton: $Texts$Dialogs.OkButton.get(), noButton: null, dialogClass: 's-MessageDialog s-InformationDialog' }, options));
	};
	$Q.tryCatch = function(fail, callback) {
		if (ss.staticEquals(fail, null)) {
			callback();
			return;
		}
		try {
			callback();
		}
		catch ($t1) {
			var ex = ss.Exception.wrap($t1);
			fail(ex);
		}
	};
	$Q.htmlEncode = function(value) {
		var text = (ss.isNullOrUndefined(value) ? '' : value.toString());
		if ((new RegExp('[><&]', 'g')).test(text)) {
			return text.replace(new RegExp('[><&]', 'g'), $Q.$htmlEncodeReplacer);
		}
		return text;
	};
	$Q.newBodyDiv = function() {
		return $('<div/>').appendTo(document.body);
	};
	$Q.$htmlEncodeReplacer = function(a) {
		switch (a) {
			case '&': {
				return '&amp;';
			}
			case '>': {
				return '&gt;';
			}
			case '<': {
				return '&lt;';
			}
		}
		return a;
	};
	$Q.clearOptions = function(select) {
		select.html('');
	};
	$Q.addEmptyOption = function(select) {
		$Q.addOption(select, '', $Texts$Controls$SelectEditor.EmptyItemText.get());
	};
	$Q.addOption = function(select, key, text) {
		$('<option/>').val(key).text(text).appendTo(select);
	};
	$Q.findElementWithRelativeId = function(element, relativeId) {
		var elementId = element.attr('id');
		if ($Q.isEmptyOrNull(elementId)) {
			return $('#' + relativeId);
		}
		var result = $(elementId + relativeId);
		if (result.length > 0) {
			return result;
		}
		result = $(elementId + '_' + relativeId);
		if (result.length > 0) {
			return result;
		}
		while (true) {
			var idx = elementId.lastIndexOf(String.fromCharCode(95));
			if (idx <= 0) {
				return $('#' + relativeId);
			}
			elementId = elementId.substr(0, idx);
			result = $('#' + elementId + '_' + relativeId);
			if (result.length > 0) {
				return result;
			}
		}
	};
	$Q.outerHtml = function(element) {
		return $('<i/>').append(element.eq(0).clone()).html();
	};
	$Q.layoutFillHeightValue = function(element) {
		var h = 0;
		element.parent().children().not(element).each(function(i, e) {
			var q = $(e);
			if (q.is(':visible')) {
				h += q.outerHeight(true);
			}
		});
		h = element.parent().height() - h;
		h = h - (element.outerHeight(true) - element.height());
		return h;
	};
	$Q.layoutFillHeight = function(element) {
		var h = $Q.layoutFillHeightValue(element);
		var n = h + 'px';
		if (!ss.referenceEquals(element.css('height'), n)) {
			element.css('height', n);
		}
	};
	$Q.initFullHeightGridPage = function(gridDiv) {
		$('body').addClass('full-height-page');
		var layout = function() {
			if (gridDiv.parent().hasClass('page-content')) {
				gridDiv.css('height', '1px').css('overflow', 'hidden');
			}
			$Q.layoutFillHeight(gridDiv);
			gridDiv.triggerHandler('layout');
		};
		if ($('body').hasClass('has-layout-event')) {
			$('body').bind('layout', layout);
		}
		else if (!!ss.isValue(window.window.Metronic)) {
			window.window.Metronic.addResizeHandler(layout);
		}
		else {
			$(window).resize(layout);
		}
		layout(null);
	};
	$Q.addFullHeightResizeHandler = function(handler) {
		$('body').addClass('full-height-page');
		var layout = function() {
			var avail;
			try {
				avail = parseInt(ss.coalesce($('.page-content').css('min-height'), '0')) - parseInt(ss.coalesce($('.page-content').css('padding-top'), '0')) - parseInt(ss.coalesce($('.page-content').css('padding-bottom'), '0'));
			}
			catch ($t1) {
				avail = 100;
			}
			handler(avail);
		};
		if (!!ss.isValue(window.window.Metronic)) {
			window.window.Metronic.addResizeHandler(layout);
		}
		else {
			$(window).resize(layout);
		}
		layout(null);
	};
	$Q.triggerLayoutOnShow = function(element) {
		$Serenity_LazyLoadHelper.executeEverytimeWhenShown(element, function() {
			element.triggerHandler('layout');
		}, true);
	};
	$Q.autoFullHeight = function(element) {
		element.css('height', '100%');
		$Q.triggerLayoutOnShow(element);
	};
	$Q.notifyWarning = function(message) {
		toastr.warning(message, '', $Q.$getToastrOptions());
	};
	$Q.notifySuccess = function(message) {
		toastr.success(message, '', $Q.$getToastrOptions());
	};
	$Q.notifyInfo = function(message) {
		toastr.info(message, '', $Q.$getToastrOptions());
	};
	$Q.notifyError = function(message) {
		toastr.error(message, '', $Q.$getToastrOptions());
	};
	$Q.$getToastrOptions = function() {
		var dialog = $(window.document.body).children('.ui-dialog:visible').last();
		var toastrDiv = $('#toast-container');
		var options = { timeOut: 3000, showDuration: 250, hideDuration: 500, extendedTimeOut: 500 };
		if (dialog.length > 0) {
			if (!toastrDiv.hasClass('dialog-toast') && toastrDiv.length > 0) {
				toastrDiv.remove();
			}
			options.target = dialog;
			options.positionClass = 'toast-top-full-width dialog-toast';
		}
		else {
			toastrDiv.removeClass('dialog-toast');
			if (toastrDiv.hasClass('dialog-toast') && toastrDiv.length > 0) {
				toastrDiv.remove();
			}
			options.positionClass = 'toast-top-full-width';
		}
		return options;
	};
	$Q.formatNumber = function(number, format) {
		if (!ss.isValue(number)) {
			return '';
		}
		return Q$Externals.formatNumber(number, format, $Q$Culture.decimalSeparator, $Q$Culture.get_groupSeperator());
	};
	$Q.parseDecimal = function(value) {
		if (ss.isNullOrUndefined(value) || $Q.isTrimmedEmpty(value)) {
			return null;
		}
		return Q$Externals.parseDecimal(value);
	};
	$Q.getRemoteData = function(key) {
		return $Q$ScriptData.ensure('RemoteData.' + key);
	};
	$Q.getRemoteDataAsync = function(key) {
		return $Q$ScriptData.ensureAsync('RemoteData.' + key);
	};
	$Q.getLookup = function(key) {
		return $Q$ScriptData.ensure('Lookup.' + key);
	};
	$Q.getLookupAsync = function(key) {
		return $Q$ScriptData.ensureAsync('Lookup.' + key);
	};
	$Q.reloadLookup = function(key) {
		$Q$ScriptData.reload('Lookup.' + key);
	};
	$Q.reloadLookupAsync = function(key) {
		return $Q$ScriptData.reloadAsync('Lookup.' + key);
	};
	$Q.getColumns = function(key) {
		return $Q$ScriptData.ensure('Columns.' + key);
	};
	$Q.getColumnsAsync = function(key) {
		return $Q$ScriptData.ensureAsync('Columns.' + key);
	};
	$Q.getForm = function(key) {
		return $Q$ScriptData.ensure('Form.' + key);
	};
	$Q.getFormAsync = function(key) {
		return $Q$ScriptData.ensureAsync('Form.' + key);
	};
	$Q.getTemplate = function(key) {
		return $Q$ScriptData.ensure('Template.' + key);
	};
	$Q.getTemplateAsync = function(key) {
		return $Q$ScriptData.ensureAsync('Template.' + key);
	};
	$Q.canLoadScriptData = function(name) {
		return $Q$ScriptData.canLoad(name);
	};
	$Q.serviceCall = function(options) {
		var handleError = function(response) {
			if (!ss.staticEquals($Q$Config.notLoggedInHandler, null) && ss.isValue(response) && ss.isValue(response.Error) && response.Error.Code === 'NotLoggedIn' && $Q$Config.notLoggedInHandler(options, response)) {
				return;
			}
			if (!ss.staticEquals(options.onError, null)) {
				options.onError(response);
			}
			else {
				$Q$ErrorHandling.showServiceError(response.Error);
			}
		};
		options = $.extend({
			dataType: 'json',
			contentType: 'application/json',
			type: 'POST',
			cache: false,
			blockUI: true,
			url: ((ss.isValue(options.service) && !ss.startsWithString(options.service, String.fromCharCode(126)) && !ss.startsWithString(options.service, String.fromCharCode(47))) ? $Q.resolveUrl('~/services/' + options.service) : $Q.resolveUrl(options.service)),
			data: $.toJSON(options.request),
			success: function(data, textStatus, request) {
				var response1 = data;
				try {
					if (ss.isNullOrUndefined(response1.Error)) {
						if (!ss.staticEquals(options.onSuccess, null)) {
							options.onSuccess(response1);
						}
					}
					else {
					}
				}
				finally {
					if (options.blockUI) {
						$Q.blockUndo();
					}
					if (!ss.staticEquals(options.onCleanup, null)) {
						options.onCleanup();
					}
				}
			},
			error: function(xhr, status, ev) {
				try {
					if (xhr.status === 403) {
						var l = null;
						try {
							l = xhr.getResponseHeader('Location');
						}
						catch ($t1) {
							l = null;
						}
						if (ss.isValue(l)) {
							window.top.location.href = l;
							return;
						}
					}
					if (ss.coalesce(xhr.getResponseHeader('content-type'), '').toLowerCase().indexOf('application/json') >= 0) {
						var json = $.parseJSON(xhr.responseText);
						if (ss.isValue(json) && ss.isValue(json.Error)) {
							handleError(json);
							return;
						}
					}
					var html = xhr.responseText;
					Q$Externals.iframeDialog({ html: html });
				}
				finally {
					if (options.blockUI) {
						$Q.blockUndo();
					}
					if (!ss.staticEquals(options.onCleanup, null)) {
						options.onCleanup();
					}
				}
			}
		}, options);
		if (options.blockUI) {
			$Q.blockUI(null);
		}
		return $.ajax(options);
	};
	$Q.serviceRequest = function(service, request, onSuccess, options) {
		return $Q.serviceCall($.extend({ service: service, request: request, onSuccess: onSuccess }, options));
	};
	$Q.trim = function(text) {
		return ss.coalesce(text, '').replace(new RegExp('^\\s+|\\s+$', 'g'), '');
	};
	$Q.isEmptyOrNull = function(str) {
		return ss.isNullOrUndefined(str) || str.length === 0;
	};
	$Q.isTrimmedEmpty = function(str) {
		return ss.isNullOrUndefined($Q.trimToNull(str));
	};
	$Q.trimToNull = function(str) {
		if (ss.isNullOrUndefined(str) || str.length === 0) {
			return null;
		}
		else {
			str = str.trim();
			if (str.length === 0) {
				return null;
			}
			else {
				return str;
			}
		}
	};
	$Q.trimToEmpty = function(str) {
		if (ss.isNullOrUndefined(str) || str.length === 0) {
			return '';
		}
		else {
			return str.trim();
		}
	};
	$Q.toSingleLine = function(str) {
		return ss.replaceAllString(ss.replaceAllString($Q.trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
	};
	$Q.text = function(key) {
		var $t1 = $Q$LT.$table[key];
		if (ss.isNullOrUndefined($t1)) {
			$t1 = ss.coalesce(key, '');
		}
		return $t1;
	};
	$Q.tryGetText = function(key) {
		return $Q$LT.$table[key];
	};
	$Q.resolveUrl = function(url) {
		if (ss.isValue(url) && url.length > 0 && url.substr(0, 2) === '~/') {
			return $Q$Config.applicationPath + url.substr(2);
		}
		else {
			return url;
		}
	};
	$Q.autoOpenByQuery = function(key, autoOpen) {
		var query = Q$Externals.parseQueryString();
		var value = query[key];
		if (ss.isValue(value)) {
			autoOpen(value);
		}
	};
	$Q.autoOpenByQueryID = function(key, autoOpen) {
		$Q.autoOpenByQuery(key, function(value) {
			var id = $Serenity_IdExtensions.convertToId(value);
			if (ss.isNullOrUndefined(id) || isNaN(id)) {
				return;
			}
			autoOpen(ss.unbox(id));
		});
	};
	global.Q = $Q;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Q.Config
	var $Q$Config = function() {
	};
	$Q$Config.__typeName = 'Q$Config';
	global.Q$Config = $Q$Config;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Q.Culture
	var $Q$Culture = function() {
	};
	$Q$Culture.__typeName = 'Q$Culture';
	$Q$Culture.get_groupSeperator = function() {
		return (($Q$Culture.decimalSeparator === ',') ? '.' : ',');
	};
	global.Q$Culture = $Q$Culture;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Q.ErrorHandling
	var $Q$ErrorHandling = function() {
	};
	$Q$ErrorHandling.__typeName = 'Q$ErrorHandling';
	$Q$ErrorHandling.showServiceError = function(error) {
		if (ss.isNullOrUndefined(error)) {
			throw new ss.Exception('error is null!');
		}
		var $t2;
		if (ss.isNullOrUndefined(error)) {
			$t2 = '??ERROR??';
		}
		else {
			var $t1 = error.Message;
			if (ss.isNullOrUndefined($t1)) {
				$t1 = error.Code;
			}
			$t2 = $t1;
		}
		$Q.alert($t2);
	};
	global.Q$ErrorHandling = $Q$ErrorHandling;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Lookup
	var $Q$Lookup = function(options, items) {
		this.$items = null;
		this.$itemById = null;
		this.$options = null;
		this.$items = [];
		this.$itemById = {};
		this.$options = options || {};
		if (ss.isValue(items)) {
			this.update(items);
		}
	};
	$Q$Lookup.__typeName = 'Q$Lookup';
	global.Q$Lookup = $Q$Lookup;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LocalText
	var $Q$LT = function(key) {
		this.$key = null;
		this.$key = key;
	};
	$Q$LT.__typeName = 'Q$LT';
	$Q$LT.add = function(obj, prefix) {
		if (!ss.isValue(obj)) {
			return;
		}
		prefix = ss.coalesce(prefix, '');
		var $t1 = ss.getEnumerator(Object.keys(obj));
		try {
			while ($t1.moveNext()) {
				var k = $t1.current();
				var actual = prefix + k;
				var o = obj[k];
				if (typeof(o) === 'object') {
					$Q$LT.add(o, actual + '.');
				}
				else {
					$Q$LT.$table[actual] = o;
				}
			}
		}
		finally {
			$t1.dispose();
		}
	};
	$Q$LT.initializeTextClass = function(type, prefix) {
		var t = type;
		var $t1 = ss.arrayClone(Object.keys(type));
		for (var $t2 = 0; $t2 < $t1.length; $t2++) {
			var member = $t1[$t2];
			var value = t[member];
			if (ss.isInstanceOfType(value, $Q$LT)) {
				var lt = value;
				var key = prefix + member;
				$Q$LT.$table[key] = lt.$key;
				t[member] = new $Q$LT(key);
			}
		}
	};
	$Q$LT.getDefault = function(key, defaultText) {
		var $t2 = $Q$LT.$table[key];
		if (ss.isNullOrUndefined($t2)) {
			var $t1 = defaultText;
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.coalesce(key, '');
			}
			$t2 = $t1;
		}
		return $t2;
	};
	global.Q$LT = $Q$LT;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Q.ScriptData
	var $Q$ScriptData = function() {
	};
	$Q$ScriptData.__typeName = 'Q$ScriptData';
	$Q$ScriptData.bindToChange = function(name, regClass, onChange) {
		$(document.body).bind('scriptdatachange.' + regClass, function(e, s) {
			if (ss.referenceEquals(s, name)) {
				onChange();
			}
		});
	};
	$Q$ScriptData.triggerChange = function(name) {
		$(document.body).triggerHandler('scriptdatachange', [name]);
	};
	$Q$ScriptData.unbindFromChange = function(regClass) {
		$(document.body).unbind('scriptdatachange.' + regClass);
	};
	$Q$ScriptData.$syncLoadScript = function(url) {
		$.ajax({ async: false, cache: true, type: 'GET', url: url, data: null, dataType: 'script' });
	};
	$Q$ScriptData.$loadScriptAsync = function(url) {
		return RSVP.resolve().then(function() {
			$Q.blockUI(null);
			return RSVP.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always($Q.blockUndo));
		}, null);
	};
	$Q$ScriptData.$loadScriptData = function(name) {
		if (!ss.keyExists($Q$ScriptData.$registered, name)) {
			throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
		}
		name = name + '.js?' + $Q$ScriptData.$registered[name];
		$Q$ScriptData.$syncLoadScript($Q.resolveUrl('~/DynJS.axd/') + name);
	};
	$Q$ScriptData.$loadScriptDataAsync = function(name) {
		return RSVP.resolve().then(function() {
			if (!ss.keyExists($Q$ScriptData.$registered, name)) {
				throw new ss.Exception(ss.formatString('Script data {0} is not found in registered script list!', name));
			}
			name = name + '.js?' + $Q$ScriptData.$registered[name];
			return $Q$ScriptData.$loadScriptAsync($Q.resolveUrl('~/DynJS.axd/') + name);
		}, null);
	};
	$Q$ScriptData.ensure = function(name) {
		var data = $Q$ScriptData.$loadedData[name];
		if (!ss.isValue(data)) {
			$Q$ScriptData.$loadScriptData(name);
		}
		data = $Q$ScriptData.$loadedData[name];
		if (!ss.isValue(data)) {
			throw new ss.NotSupportedException(ss.formatString("Can't load script data: {0}!", name));
		}
		return data;
	};
	$Q$ScriptData.ensureAsync = function(name) {
		return RSVP.resolve().then(function() {
			var data = $Q$ScriptData.$loadedData[name];
			if (ss.isValue(data)) {
				return RSVP.resolve(data);
			}
			return $Q$ScriptData.$loadScriptDataAsync(name).then(function() {
				data = $Q$ScriptData.$loadedData[name];
				if (!ss.isValue(data)) {
					throw new ss.NotSupportedException(ss.formatString("Can't load script data: {0}!", name));
				}
				return data;
			}, null);
		}, null);
	};
	$Q$ScriptData.reload = function(name) {
		if (!ss.keyExists($Q$ScriptData.$registered, name)) {
			throw new ss.NotSupportedException(ss.formatString('Script data {0} is not found in registered script list!'));
		}
		$Q$ScriptData.$registered[name] = (new Date()).getTime().toString();
		$Q$ScriptData.$loadScriptData(name);
		var data = $Q$ScriptData.$loadedData[name];
		return data;
	};
	$Q$ScriptData.reloadAsync = function(name) {
		return RSVP.resolve().then(function() {
			if (!ss.keyExists($Q$ScriptData.$registered, name)) {
				throw new ss.NotSupportedException(ss.formatString('Script data {0} is not found in registered script list!'));
			}
			$Q$ScriptData.$registered[name] = (new Date()).getTime().toString();
			return $Q$ScriptData.$loadScriptDataAsync(name).then(function() {
				return $Q$ScriptData.$loadedData[name];
			}, null);
		}, null);
	};
	$Q$ScriptData.canLoad = function(name) {
		var data = $Q$ScriptData.$loadedData[name];
		if (ss.isValue(data) || ss.keyExists($Q$ScriptData.$registered, name)) {
			return true;
		}
		return false;
	};
	$Q$ScriptData.setRegisteredScripts = function(scripts) {
		ss.clearKeys($Q$ScriptData.$registered);
		var $t1 = new ss.ObjectEnumerator(scripts);
		try {
			while ($t1.moveNext()) {
				var k = $t1.current();
				$Q$ScriptData.$registered[k.key] = k.value.toString();
			}
		}
		finally {
			$t1.dispose();
		}
	};
	$Q$ScriptData.set = function(name, value) {
		$Q$ScriptData.$loadedData[name] = value;
		$Q$ScriptData.triggerChange(name);
	};
	global.Q$ScriptData = $Q$ScriptData;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts
	var $Texts = function() {
	};
	$Texts.__typeName = 'Texts';
	global.Texts = $Texts;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls
	var $Texts$Controls = function() {
	};
	$Texts$Controls.__typeName = 'Texts$Controls';
	global.Texts$Controls = $Texts$Controls;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.EntityDialog
	var $Texts$Controls$EntityDialog = function() {
	};
	$Texts$Controls$EntityDialog.__typeName = 'Texts$Controls$EntityDialog';
	global.Texts$Controls$EntityDialog = $Texts$Controls$EntityDialog;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.EntityGrid
	var $Texts$Controls$EntityGrid = function() {
	};
	$Texts$Controls$EntityGrid.__typeName = 'Texts$Controls$EntityGrid';
	global.Texts$Controls$EntityGrid = $Texts$Controls$EntityGrid;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.Pager
	var $Texts$Controls$Pager = function() {
	};
	$Texts$Controls$Pager.__typeName = 'Texts$Controls$Pager';
	global.Texts$Controls$Pager = $Texts$Controls$Pager;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.PropertyGrid
	var $Texts$Controls$PropertyGrid = function() {
	};
	$Texts$Controls$PropertyGrid.__typeName = 'Texts$Controls$PropertyGrid';
	global.Texts$Controls$PropertyGrid = $Texts$Controls$PropertyGrid;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.QuickSearch
	var $Texts$Controls$QuickSearch = function() {
	};
	$Texts$Controls$QuickSearch.__typeName = 'Texts$Controls$QuickSearch';
	global.Texts$Controls$QuickSearch = $Texts$Controls$QuickSearch;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Controls.SelectEditor
	var $Texts$Controls$SelectEditor = function() {
	};
	$Texts$Controls$SelectEditor.__typeName = 'Texts$Controls$SelectEditor';
	global.Texts$Controls$SelectEditor = $Texts$Controls$SelectEditor;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Texts.Dialogs
	var $Texts$Dialogs = function() {
	};
	$Texts$Dialogs.__typeName = 'Texts$Dialogs';
	global.Texts$Dialogs = $Texts$Dialogs;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.BooleanFormatter
	var $Serenity_BooleanFormatter = function() {
		this.$1$FalseTextField = null;
		this.$1$TrueTextField = null;
	};
	$Serenity_BooleanFormatter.__typeName = 'Serenity.BooleanFormatter';
	global.Serenity.BooleanFormatter = $Serenity_BooleanFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CheckboxFormatter
	var $Serenity_CheckboxFormatter = function() {
	};
	$Serenity_CheckboxFormatter.__typeName = 'Serenity.CheckboxFormatter';
	global.Serenity.CheckboxFormatter = $Serenity_CheckboxFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ColumnsKeyAttribute
	var $Serenity_ColumnsKeyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_ColumnsKeyAttribute.__typeName = 'Serenity.ColumnsKeyAttribute';
	global.Serenity.ColumnsKeyAttribute = $Serenity_ColumnsKeyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.CriteriaUtil
	var $Serenity_Criteria = function() {
	};
	$Serenity_Criteria.__typeName = 'Serenity.Criteria';
	$Serenity_Criteria.isEmpty = function(criteria) {
		var array = criteria;
		return array.length === 0 || array.length === 1 && ss.isInstanceOfType(array[0], String) && ss.cast(array[0], String).length === 0;
	};
	$Serenity_Criteria.join = function(criteria1, op, criteria2) {
		if (ss.referenceEquals(null, criteria1)) {
			throw new ss.ArgumentNullException('criteria1');
		}
		if (ss.referenceEquals(null, criteria2)) {
			throw new ss.ArgumentNullException('criteria2');
		}
		if (Serenity.Criteria.isEmpty(criteria1)) {
			return criteria2;
		}
		if (Serenity.Criteria.isEmpty(criteria2)) {
			return criteria1;
		}
		return [criteria1, op, criteria2];
	};
	$Serenity_Criteria.paren = function(criteria) {
		if (!Serenity.Criteria.isEmpty(criteria)) {
			return ['()', criteria];
		}
		return criteria;
	};
	global.Serenity.Criteria = $Serenity_Criteria;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateFormatter
	var $Serenity_DateFormatter = function() {
		this.$1$DisplayFormatField = null;
		this.set_displayFormat($Q$Culture.dateFormat);
	};
	$Serenity_DateFormatter.__typeName = 'Serenity.DateFormatter';
	$Serenity_DateFormatter.format = function(value, format) {
		if (!ss.isValue(value)) {
			return '';
		}
		var date;
		if (typeof(value) === 'date') {
			date = value;
		}
		else if (typeof(value) === 'string') {
			date = Q$Externals.parseISODateTime(value);
			if (ss.staticEquals(date, null)) {
				return $Q.htmlEncode(value);
			}
		}
		else {
			return value.toString();
		}
		return $Q.htmlEncode($Q.formatDate(date, format));
	};
	global.Serenity.DateFormatter = $Serenity_DateFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DateTimeFormatter
	var $Serenity_DateTimeFormatter = function() {
		$Serenity_DateFormatter.call(this);
		this.set_displayFormat($Q$Culture.dateTimeFormat);
	};
	$Serenity_DateTimeFormatter.__typeName = 'Serenity.DateTimeFormatter';
	global.Serenity.DateTimeFormatter = $Serenity_DateTimeFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.DialogTypeAttribute
	var $Serenity_DialogTypeAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_DialogTypeAttribute.__typeName = 'Serenity.DialogTypeAttribute';
	global.Serenity.DialogTypeAttribute = $Serenity_DialogTypeAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EditorAttribute
	var $Serenity_EditorAttribute = function() {
		this.$2$KeyField = null;
	};
	$Serenity_EditorAttribute.__typeName = 'Serenity.EditorAttribute';
	global.Serenity.EditorAttribute = $Serenity_EditorAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ElementAttribute
	var $Serenity_ElementAttribute = function(html) {
		this.$html = null;
		this.$html = html;
	};
	$Serenity_ElementAttribute.__typeName = 'Serenity.ElementAttribute';
	global.Serenity.ElementAttribute = $Serenity_ElementAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EntityTypeAttribute
	var $Serenity_EntityTypeAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_EntityTypeAttribute.__typeName = 'Serenity.EntityTypeAttribute';
	global.Serenity.EntityTypeAttribute = $Serenity_EntityTypeAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EnumFormatter
	var $Serenity_EnumFormatter = function() {
		this.$1$EnumKeyField = null;
	};
	$Serenity_EnumFormatter.__typeName = 'Serenity.EnumFormatter';
	$Serenity_EnumFormatter.format = function(enumType, value) {
		if (!ss.isValue(value)) {
			return '';
		}
		var name;
		try {
			name = ss.Enum.toString(enumType, value);
		}
		catch ($t1) {
			$t1 = ss.Exception.wrap($t1);
			if (ss.isInstanceOfType($t1, ss.ArgumentException)) {
				name = value.toString();
			}
			else {
				throw $t1;
			}
		}
		var enumKeyAttr = ss.getAttributes(enumType, $Serenity_EnumKeyAttribute, false);
		var enumKey = ((enumKeyAttr.length > 0) ? ss.cast(enumKeyAttr[0], $Serenity_EnumKeyAttribute).get_value() : ss.getTypeFullName(enumType));
		return $Serenity_EnumFormatter.getText$1(enumKey, name);
	};
	$Serenity_EnumFormatter.getText$1 = function(enumKey, name) {
		return $Q.htmlEncode(ss.coalesce($Q.tryGetText('Enums.' + enumKey + '.' + name), name));
	};
	$Serenity_EnumFormatter.getText = function(TEnum) {
		return function(value) {
			if (!ss.isValue(value)) {
				return '';
			}
			return $Serenity_EnumFormatter.format(TEnum, ss.unbox(value));
		};
	};
	$Serenity_EnumFormatter.getName = function(TEnum) {
		return function(value) {
			if (!ss.isValue(value)) {
				return '';
			}
			return ss.Enum.toString(TEnum, ss.unbox(value));
		};
	};
	global.Serenity.EnumFormatter = $Serenity_EnumFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EnumKeyAttribute
	var $Serenity_EnumKeyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_EnumKeyAttribute.__typeName = 'Serenity.EnumKeyAttribute';
	global.Serenity.EnumKeyAttribute = $Serenity_EnumKeyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.EnumTypeRegistry
	var $Serenity_EnumTypeRegistry = function() {
	};
	$Serenity_EnumTypeRegistry.__typeName = 'Serenity.EnumTypeRegistry';
	$Serenity_EnumTypeRegistry.get = function(key) {
		if (ss.isNullOrUndefined($Serenity_EnumTypeRegistry.$knownTypes)) {
			$Serenity_EnumTypeRegistry.$knownTypes = {};
			var $t1 = ss.getAssemblies();
			for (var $t2 = 0; $t2 < $t1.length; $t2++) {
				var assembly = $t1[$t2];
				var $t3 = ss.getAssemblyTypes(assembly);
				for (var $t4 = 0; $t4 < $t3.length; $t4++) {
					var type = $t3[$t4];
					if (ss.isEnum(type)) {
						var fullName = ss.getTypeFullName(type);
						$Serenity_EnumTypeRegistry.$knownTypes[fullName] = type;
						var enumKeyAttr = ss.getAttributes(type, $Serenity_EnumKeyAttribute, false);
						if (ss.isValue(enumKeyAttr) && enumKeyAttr.length > 0) {
							$Serenity_EnumTypeRegistry.$knownTypes[ss.cast(enumKeyAttr[0], $Serenity_EnumKeyAttribute).get_value()] = type;
						}
						for (var $t5 = 0; $t5 < $Q$Config.rootNamespaces.length; $t5++) {
							var k = $Q$Config.rootNamespaces[$t5];
							if (ss.startsWithString(fullName, k + '.')) {
								$Serenity_EnumTypeRegistry.$knownTypes[fullName.substr(k.length + 1)] = type;
							}
						}
					}
				}
			}
		}
		if (!ss.keyExists($Serenity_EnumTypeRegistry.$knownTypes, key)) {
			throw new ss.Exception(ss.formatString("Can't find {0} enum type!", key));
		}
		return $Serenity_EnumTypeRegistry.$knownTypes[key];
	};
	global.Serenity.EnumTypeRegistry = $Serenity_EnumTypeRegistry;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FlexifyAttribute
	var $Serenity_FlexifyAttribute = function() {
	};
	$Serenity_FlexifyAttribute.__typeName = 'Serenity.FlexifyAttribute';
	global.Serenity.FlexifyAttribute = $Serenity_FlexifyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.FormKeyAttribute
	var $Serenity_FormKeyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_FormKeyAttribute.__typeName = 'Serenity.FormKeyAttribute';
	global.Serenity.FormKeyAttribute = $Serenity_FormKeyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IdExtensions
	var $Serenity_IdExtensions = function() {
	};
	$Serenity_IdExtensions.__typeName = 'Serenity.IdExtensions';
	$Serenity_IdExtensions.convertToId = function(value) {
		return Q$Externals.toId(value);
	};
	$Serenity_IdExtensions.toInt32 = function(value) {
		return Q$Externals.toId(value);
	};
	$Serenity_IdExtensions.isPositiveId = function(id) {
		if (!ss.isValue(id)) {
			return false;
		}
		else if (typeof(id) === 'string') {
			var idStr = id;
			if (ss.startsWithString(idStr, '-')) {
				return false;
			}
			return idStr.length > 0;
		}
		else if (typeof(id) === 'number') {
			return id > 0;
		}
		else {
			return true;
		}
	};
	$Serenity_IdExtensions.isNegativeId = function(id) {
		if (!ss.isValue(id)) {
			return false;
		}
		else if (typeof(id) === 'string') {
			var idStr = id;
			if (ss.startsWithString(idStr, '-')) {
				return true;
			}
			return false;
		}
		else if (typeof(id) === 'number') {
			return id < 0;
		}
		else {
			return false;
		}
	};
	global.Serenity.IdExtensions = $Serenity_IdExtensions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IdPropertyAttribute
	var $Serenity_IdPropertyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_IdPropertyAttribute.__typeName = 'Serenity.IdPropertyAttribute';
	global.Serenity.IdPropertyAttribute = $Serenity_IdPropertyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IInitializeColumn
	var $Serenity_IInitializeColumn = function() {
	};
	$Serenity_IInitializeColumn.__typeName = 'Serenity.IInitializeColumn';
	global.Serenity.IInitializeColumn = $Serenity_IInitializeColumn;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.IsActivePropertyAttribute
	var $Serenity_IsActivePropertyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_IsActivePropertyAttribute.__typeName = 'Serenity.IsActivePropertyAttribute';
	global.Serenity.IsActivePropertyAttribute = $Serenity_IsActivePropertyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ISlickFormatter
	var $Serenity_ISlickFormatter = function() {
	};
	$Serenity_ISlickFormatter.__typeName = 'Serenity.ISlickFormatter';
	global.Serenity.ISlickFormatter = $Serenity_ISlickFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ItemNameAttribute
	var $Serenity_ItemNameAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_ItemNameAttribute.__typeName = 'Serenity.ItemNameAttribute';
	global.Serenity.ItemNameAttribute = $Serenity_ItemNameAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LazyLoadHelper
	var $Serenity_LazyLoadHelper = function() {
	};
	$Serenity_LazyLoadHelper.__typeName = 'Serenity.LazyLoadHelper';
	$Serenity_LazyLoadHelper.executeOnceWhenShown = function(element, callback) {
		$Serenity_LazyLoadHelper.$autoIncrement++;
		var eventClass = 'ExecuteOnceWhenShown' + $Serenity_LazyLoadHelper.$autoIncrement;
		var executed = false;
		if (element.is(':visible')) {
			callback();
		}
		else {
			var uiTabs = element.closest('.ui-tabs');
			if (uiTabs.length > 0) {
				uiTabs.bind('tabsshow.' + eventClass, function(e) {
					if (element.is(':visible')) {
						uiTabs.unbind('tabsshow.' + eventClass);
						if (!executed) {
							executed = true;
							element.unbind('shown.' + eventClass);
							callback();
						}
					}
				});
			}
			var dialog;
			if (element.hasClass('ui-dialog')) {
				dialog = element.children('.ui-dialog-content');
			}
			else {
				dialog = element.closest('.ui-dialog-content');
			}
			if (dialog.length > 0) {
				dialog.bind('dialogopen.' + eventClass, function() {
					dialog.unbind('dialogopen.' + eventClass);
					if (element.is(':visible') && !executed) {
						executed = true;
						element.unbind('shown.' + eventClass);
						callback();
					}
				});
			}
			element.bind('shown.' + eventClass, function() {
				if (element.is(':visible')) {
					element.unbind('shown.' + eventClass);
					if (!executed) {
						executed = true;
						callback();
					}
				}
			});
		}
	};
	$Serenity_LazyLoadHelper.executeEverytimeWhenShown = function(element, callback, callNowIfVisible) {
		$Serenity_LazyLoadHelper.$autoIncrement++;
		var eventClass = 'ExecuteEverytimeWhenShown' + $Serenity_LazyLoadHelper.$autoIncrement;
		var wasVisible = element.is(':visible');
		if (wasVisible && callNowIfVisible) {
			callback();
		}
		var check = function(e) {
			if (element.is(':visible')) {
				if (!wasVisible) {
					wasVisible = true;
					callback();
				}
			}
			else {
				wasVisible = false;
			}
		};
		var uiTabs = element.closest('.ui-tabs');
		if (uiTabs.length > 0) {
			uiTabs.bind('tabsactivate.' + eventClass, check);
		}
		var dialog = element.closest('.ui-dialog-content');
		if (dialog.length > 0) {
			dialog.bind('dialogopen.' + eventClass, check);
		}
		element.bind('shown.' + eventClass, check);
	};
	global.Serenity.LazyLoadHelper = $Serenity_LazyLoadHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.LocalTextPrefixAttribute
	var $Serenity_LocalTextPrefixAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_LocalTextPrefixAttribute.__typeName = 'Serenity.LocalTextPrefixAttribute';
	global.Serenity.LocalTextPrefixAttribute = $Serenity_LocalTextPrefixAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.MaximizableAttribute
	var $Serenity_MaximizableAttribute = function() {
	};
	$Serenity_MaximizableAttribute.__typeName = 'Serenity.MaximizableAttribute';
	global.Serenity.MaximizableAttribute = $Serenity_MaximizableAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.MinuteFormatter
	var $Serenity_MinuteFormatter = function() {
	};
	$Serenity_MinuteFormatter.__typeName = 'Serenity.MinuteFormatter';
	$Serenity_MinuteFormatter.format = function(value) {
		var hour = ss.Int32.trunc(Math.floor(value / 60));
		var minute = value - hour * 60;
		var hourStr, minuteStr;
		if (!ss.isValue(value) || isNaN(value)) {
			return '';
		}
		if (hour < 10) {
			hourStr = '0' + hour;
		}
		else {
			hourStr = hour.toString();
		}
		if (minute < 10) {
			minuteStr = '0' + minute;
		}
		else {
			minuteStr = minute.toString();
		}
		return ss.formatString('{0}:{1}', hourStr, minuteStr);
	};
	global.Serenity.MinuteFormatter = $Serenity_MinuteFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.NamePropertyAttribute
	var $Serenity_NamePropertyAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_NamePropertyAttribute.__typeName = 'Serenity.NamePropertyAttribute';
	global.Serenity.NamePropertyAttribute = $Serenity_NamePropertyAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.NumberFormatter
	var $Serenity_NumberFormatter = function() {
		this.$1$DisplayFormatField = null;
	};
	$Serenity_NumberFormatter.__typeName = 'Serenity.NumberFormatter';
	$Serenity_NumberFormatter.format = function(value, format) {
		format = ss.coalesce(format, '0.##');
		if (!ss.isValue(value)) {
			return '';
		}
		if (typeof(value) === 'number') {
			if (isNaN(value)) {
				return '';
			}
			return $Q.htmlEncode($Q.formatNumber(value, format));
		}
		var dbl = $Q.parseDecimal(value.toString());
		if (ss.isNullOrUndefined(dbl)) {
			return '';
		}
		return $Q.htmlEncode(value.toString());
	};
	global.Serenity.NumberFormatter = $Serenity_NumberFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.OptionsTypeAttribute
	var $Serenity_OptionsTypeAttribute = function(optionsType) {
		this.$2$OptionsTypeField = null;
		this.set_optionsType(optionsType);
	};
	$Serenity_OptionsTypeAttribute.__typeName = 'Serenity.OptionsTypeAttribute';
	global.Serenity.OptionsTypeAttribute = $Serenity_OptionsTypeAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.PanelAttribute
	var $Serenity_PanelAttribute = function() {
	};
	$Serenity_PanelAttribute.__typeName = 'Serenity.PanelAttribute';
	global.Serenity.PanelAttribute = $Serenity_PanelAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ResizableAttribute
	var $Serenity_ResizableAttribute = function() {
	};
	$Serenity_ResizableAttribute.__typeName = 'Serenity.ResizableAttribute';
	global.Serenity.ResizableAttribute = $Serenity_ResizableAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ScriptContext
	var $Serenity_ScriptContext = function() {
	};
	$Serenity_ScriptContext.__typeName = 'Serenity.ScriptContext';
	global.Serenity.ScriptContext = $Serenity_ScriptContext;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ServiceAttribute
	var $Serenity_ServiceAttribute = function(value) {
		this.$2$ValueField = null;
		this.set_value(value);
	};
	$Serenity_ServiceAttribute.__typeName = 'Serenity.ServiceAttribute';
	global.Serenity.ServiceAttribute = $Serenity_ServiceAttribute;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SlickFormatting
	var $Serenity_SlickFormatting = function() {
	};
	$Serenity_SlickFormatting.__typeName = 'Serenity.SlickFormatting';
	$Serenity_SlickFormatting.getEnumText = function(TEnum) {
		return function(value) {
			return $Serenity_EnumFormatter.getText(TEnum).call(null, value);
		};
	};
	$Serenity_SlickFormatting.getEnumText$1 = function(enumKey, name) {
		return $Serenity_EnumFormatter.getText$1(enumKey, name);
	};
	$Serenity_SlickFormatting.enum$1 = function(enumKey) {
		return function(ctx) {
			if (ss.isValue(ctx.value)) {
				return $Q.htmlEncode($Q.text('Enums.' + enumKey + '.' + ctx.value));
			}
			else {
				return '';
			}
		};
	};
	$Serenity_SlickFormatting.treeToggle = function(TEntity) {
		return function(getView, getId, formatter) {
			return function(ctx) {
				var text = formatter(ctx);
				var view = getView();
				var indent = ss.coalesce(ctx.item._indent, 0);
				var spacer = '<span class="s-TreeIndent" style="width:' + 15 * indent + 'px"></span>';
				var id = getId(ss.cast(ctx.item, TEntity));
				var idx = view.getIdxById(id);
				var next = view.getItemByIdx(idx + 1);
				if (!!ss.isValue(next)) {
					var nextIndent = ss.coalesce(next._indent, 0);
					if (nextIndent > indent) {
						if (!!!!ctx.item._collapsed) {
							return spacer + '<span class="s-TreeToggle s-TreeExpand"></span>' + text;
						}
						else {
							return spacer + '<span class="s-TreeToggle s-TreeCollapse"></span>' + text;
						}
					}
				}
				return spacer + '<span class="s-TreeToggle"></span>' + text;
			};
		};
	};
	$Serenity_SlickFormatting.date = function(format) {
		var $t1 = format;
		if (ss.isNullOrUndefined($t1)) {
			$t1 = $Q$Culture.dateFormat;
		}
		format = $t1;
		return function(ctx) {
			return $Q.htmlEncode($Serenity_DateFormatter.format(ctx.value, format));
		};
	};
	$Serenity_SlickFormatting.dateTime = function(format) {
		var $t1 = format;
		if (ss.isNullOrUndefined($t1)) {
			$t1 = $Q$Culture.dateTimeFormat;
		}
		format = $t1;
		return function(ctx) {
			return $Q.htmlEncode($Serenity_DateFormatter.format(ctx.value, format));
		};
	};
	$Serenity_SlickFormatting.checkBox = function() {
		return function(ctx) {
			return '<span class="check-box no-float ' + (!!ctx.value ? ' checked' : '') + '"></span>';
		};
	};
	$Serenity_SlickFormatting.number = function(format) {
		return function(ctx) {
			return $Serenity_NumberFormatter.format(ctx.value, format);
		};
	};
	$Serenity_SlickFormatting.getItemType$1 = function(link) {
		return ss.safeCast(link.data('item-type'), String);
	};
	$Serenity_SlickFormatting.getItemType = function(href) {
		if ($Q.isEmptyOrNull(href)) {
			return null;
		}
		if (ss.startsWithString(href, '#')) {
			href = href.substr(1);
		}
		var idx = href.lastIndexOf(String.fromCharCode(47));
		if (idx >= 0) {
			href = href.substr(0, idx);
		}
		return href;
	};
	$Serenity_SlickFormatting.getItemId$1 = function(link) {
		var value = link.data('item-id');
		return (ss.isNullOrUndefined(value) ? null : value.toString());
	};
	$Serenity_SlickFormatting.getItemId = function(href) {
		if ($Q.isEmptyOrNull(href)) {
			return null;
		}
		if (ss.startsWithString(href, '#')) {
			href = href.substr(1);
		}
		var idx = href.lastIndexOf(String.fromCharCode(47));
		if (idx >= 0) {
			href = href.substr(idx + 1);
		}
		return href;
	};
	$Serenity_SlickFormatting.itemLinkText = function(itemType, id, text, extraClass, encode) {
		return '<a' + (ss.isValue(id) ? (' href="#' + ss.replaceAllString(itemType, '.', '-') + '/' + id + '"') : '') + ' data-item-type="' + itemType + '"' + ' data-item-id="' + id + '"' + ' class="s-EditLink s-' + ss.replaceAllString(itemType, '.', '-') + 'Link' + ($Q.isEmptyOrNull(extraClass) ? '' : (' ' + extraClass)) + '">' + (encode ? $Q.htmlEncode(ss.coalesce(text, '')) : ss.coalesce(text, '')) + '</a>';
	};
	$Serenity_SlickFormatting.itemLink = function(itemType, idField, getText, cssClass, encode) {
		return function(ctx) {
			return ss.cast($Serenity_SlickFormatting.itemLinkText(itemType, ctx.item[idField], (ss.staticEquals(getText, null) ? ctx.value : getText(ctx)), (ss.staticEquals(cssClass, null) ? '' : cssClass(ctx)), encode), String);
		};
	};
	global.Serenity.SlickFormatting = $Serenity_SlickFormatting;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SlickHelper
	var $Serenity_SlickHelper = function() {
	};
	$Serenity_SlickHelper.__typeName = 'Serenity.SlickHelper';
	$Serenity_SlickHelper.setDefaults = function(columns, localTextPrefix) {
		var $t1 = ss.getEnumerator(columns);
		try {
			while ($t1.moveNext()) {
				var col = $t1.current();
				col.sortable = (ss.isValue(col.sortable) ? col.sortable : true);
				var $t2 = col.id;
				if (ss.isNullOrUndefined($t2)) {
					$t2 = col.field;
				}
				col.id = $t2;
				if (ss.isValue(localTextPrefix) && ss.isValue(col.id) && (ss.isNullOrUndefined(col.name) || ss.startsWithString(col.name, '~'))) {
					var key = (ss.isValue(col.name) ? col.name.substr(1) : col.id);
					col.name = $Q.text(localTextPrefix + key);
				}
				if (ss.staticEquals(col.formatter, null) && !ss.staticEquals(col.format, null)) {
					col.formatter = $Serenity_SlickHelper.convertToFormatter(col.format);
				}
				else if (ss.staticEquals(col.formatter, null)) {
					col.formatter = function(row, cell, value, column, item) {
						return $Q.htmlEncode(value);
					};
				}
			}
		}
		finally {
			$t1.dispose();
		}
		return columns;
	};
	$Serenity_SlickHelper.convertToFormatter = function(format) {
		if (ss.staticEquals(format, null)) {
			return null;
		}
		else {
			return function(row, cell, value, column, item) {
				return format({ row: row, cell: cell, value: value, column: column, item: item });
			};
		}
	};
	global.Serenity.SlickHelper = $Serenity_SlickHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.SlickTreeHelper
	var $Serenity_SlickTreeHelper = function() {
	};
	$Serenity_SlickTreeHelper.__typeName = 'Serenity.SlickTreeHelper';
	$Serenity_SlickTreeHelper.filterCustom = function(item, getParent) {
		var parent = getParent(item);
		var loop = 0;
		while (ss.isValue(parent)) {
			if (!!parent._collapsed) {
				return false;
			}
			parent = getParent(parent);
			if (loop++ > 1000) {
				throw new ss.InvalidOperationException('Possible infinite loop, check parents has no circular reference!');
			}
		}
		return true;
	};
	$Serenity_SlickTreeHelper.filterById = function(item, view, getParentId) {
		return $Serenity_SlickTreeHelper.filterCustom(item, function(x) {
			var parentId = getParentId(x);
			if (ss.isNullOrUndefined(parentId)) {
				return null;
			}
			return view.getItemById(parentId);
		});
	};
	$Serenity_SlickTreeHelper.setCollapsed = function(items, collapsed) {
		if (ss.isValue(items)) {
			var $t1 = ss.getEnumerator(items);
			try {
				while ($t1.moveNext()) {
					var item = $t1.current();
					item._collapsed = collapsed;
				}
			}
			finally {
				$t1.dispose();
			}
		}
	};
	$Serenity_SlickTreeHelper.setCollapsedFlag = function(item, collapsed) {
		item._collapsed = collapsed;
	};
	$Serenity_SlickTreeHelper.setIndents = function(items, getId, getParentId, setCollapsed) {
		var depth = 0;
		var depths = {};
		for (var line = 0; line < ss.count(items); line++) {
			var item = ss.getItem(items, line);
			if (line > 0) {
				var parentId = getParentId(item);
				if (ss.isValue(parentId) && ss.referenceEquals(parentId, getId(ss.getItem(items, line - 1)))) {
					depth += 1;
				}
				else if (ss.isNullOrUndefined(parentId)) {
					depth = 0;
				}
				else if (!ss.referenceEquals(parentId, getParentId(ss.getItem(items, line - 1)))) {
					if (ss.keyExists(depths, parentId)) {
						depth = depths[parentId] + 1;
					}
					else {
						depth = 0;
					}
				}
			}
			depths[getId(item)] = depth;
			item._indent = depth;
			if (ss.isValue(setCollapsed)) {
				item._collapsed = ss.unbox(setCollapsed);
			}
		}
	};
	$Serenity_SlickTreeHelper.toggleClick = function(TEntity) {
		return function(e, row, cell, view, getId) {
			var target = $(e.target);
			if (!target.hasClass('s-TreeToggle')) {
				return;
			}
			if (target.hasClass('s-TreeCollapse') || target.hasClass('s-TreeExpand')) {
				var item = view.rows[row];
				if (!!ss.isValue(item)) {
					if (!!!item._collapsed) {
						item._collapsed = true;
					}
					else {
						item._collapsed = false;
					}
					view.updateItem(getId(ss.cast(item, TEntity)), item);
				}
				if (e.shiftKey) {
					view.beginUpdate();
					try {
						$Serenity_SlickTreeHelper.setCollapsed(view.getItems(), !!item._collapsed);
						view.setItems(view.getItems(), true);
					}
					finally {
						view.endUpdate();
					}
				}
			}
		};
	};
	global.Serenity.SlickTreeHelper = $Serenity_SlickTreeHelper;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.TabsExtensions
	var $Serenity_TabsExtensions = function() {
	};
	$Serenity_TabsExtensions.__typeName = 'Serenity.TabsExtensions';
	$Serenity_TabsExtensions.setDisabled = function(tabs, tabKey, isDisabled) {
		if (ss.isNullOrUndefined(tabs)) {
			return;
		}
		var indexByKey = $Serenity_TabsExtensions.indexByKey(tabs);
		if (ss.isNullOrUndefined(indexByKey)) {
			return;
		}
		var index = indexByKey[tabKey];
		if (ss.isNullOrUndefined(index)) {
			return;
		}
		if (ss.unbox(index) === tabs.tabs('option', 'active')) {
			tabs.tabs('option', 'active', 0);
		}
		if (isDisabled) {
			tabs.tabs('disable', ss.unbox(index));
		}
		else {
			tabs.tabs('enable', ss.unbox(index));
		}
	};
	$Serenity_TabsExtensions.activeTabKey = function(tabs) {
		var href = tabs.children('ul').children('li').eq(tabs.tabs('option', 'active')).children('a').attr('href').toString();
		var prefix = '_Tab';
		var lastIndex = href.lastIndexOf(prefix);
		if (lastIndex >= 0) {
			href = href.substr(lastIndex + prefix.length);
		}
		return href;
	};
	$Serenity_TabsExtensions.indexByKey = function(tabs) {
		var indexByKey = tabs.data('indexByKey');
		if (ss.isNullOrUndefined(indexByKey)) {
			indexByKey = {};
			tabs.children('ul').children('li').children('a').each(function(index, el) {
				var href = el.getAttribute('href').toString();
				var prefix = '_Tab';
				var lastIndex = href.lastIndexOf(prefix);
				if (lastIndex >= 0) {
					href = href.substr(lastIndex + prefix.length);
				}
				indexByKey[href] = index;
			});
			tabs.data('indexByKey', indexByKey);
		}
		return indexByKey;
	};
	global.Serenity.TabsExtensions = $Serenity_TabsExtensions;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.ComponentModel.OptionAttribute
	var $Serenity_ComponentModel_OptionAttribute = function() {
	};
	$Serenity_ComponentModel_OptionAttribute.__typeName = 'Serenity.ComponentModel.OptionAttribute';
	global.Serenity.ComponentModel.OptionAttribute = $Serenity_ComponentModel_OptionAttribute;
	ss.initClass($Q, $asm, {});
	ss.initClass($Q$Config, $asm, {});
	ss.initClass($Q$Culture, $asm, {});
	ss.initClass($Q$ErrorHandling, $asm, {});
	ss.initClass($Q$Lookup, $asm, {
		update: function(newItems) {
			this.$items = [];
			this.$itemById = {};
			if (ss.isValue(newItems)) {
				ss.arrayAddRange(this.$items, newItems);
			}
			var idField = this.$options.idField;
			if (!$Q.isEmptyOrNull(idField)) {
				for (var i = 0; i < this.$items.length; i++) {
					var r = this.$items[i];
					var v = r[idField];
					// ?? Type.GetProperty(r, idField);
					if (ss.isValue(v)) {
						this.$itemById[v] = r;
					}
				}
			}
		},
		get_idField: function() {
			return this.$options.idField;
		},
		get_parentIdField: function() {
			return this.$options.parentIdField;
		},
		get_textField: function() {
			return this.$options.textField;
		},
		get_textFormatter: function() {
			return this.$options.textFormatter;
		},
		get_itemById: function() {
			return this.$itemById;
		},
		get_items: function() {
			return this.$items;
		}
	});
	ss.initClass($Q$LT, $asm, {
		get: function() {
			var $t1 = $Q$LT.$table[this.$key];
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.coalesce(this.$key, '');
			}
			return $t1;
		},
		toString: function() {
			var $t1 = $Q$LT.$table[this.$key];
			if (ss.isNullOrUndefined($t1)) {
				$t1 = ss.coalesce(this.$key, '');
			}
			return $t1;
		}
	});
	ss.initClass($Q$ScriptData, $asm, {});
	ss.initClass($Texts, $asm, {});
	ss.initClass($Texts$Controls, $asm, {});
	ss.initClass($Texts$Controls$EntityDialog, $asm, {});
	ss.initClass($Texts$Controls$EntityGrid, $asm, {});
	ss.initClass($Texts$Controls$Pager, $asm, {});
	ss.initClass($Texts$Controls$PropertyGrid, $asm, {});
	ss.initClass($Texts$Controls$QuickSearch, $asm, {});
	ss.initClass($Texts$Controls$SelectEditor, $asm, {});
	ss.initClass($Texts$Dialogs, $asm, {});
	ss.initInterface($Serenity_ISlickFormatter, $asm, { format: null });
	ss.initClass($Serenity_BooleanFormatter, $asm, {
		format: function(ctx) {
			if (!ss.isValue(ctx.value)) {
				return '';
			}
			if (!!ctx.value) {
				var $t2 = $Q.tryGetText(this.get_trueText());
				if (ss.isNullOrUndefined($t2)) {
					var $t1 = this.get_trueText();
					if (ss.isNullOrUndefined($t1)) {
						$t1 = ss.coalesce($Q.tryGetText('Dialogs.YesButton'), 'Yes');
					}
					$t2 = $t1;
				}
				return $Q.htmlEncode($t2);
			}
			var $t4 = $Q.tryGetText(this.get_falseText());
			if (ss.isNullOrUndefined($t4)) {
				var $t3 = this.get_falseText();
				if (ss.isNullOrUndefined($t3)) {
					$t3 = ss.coalesce($Q.tryGetText('Dialogs.NoButton'), 'No');
				}
				$t4 = $t3;
			}
			return $Q.htmlEncode($t4);
		},
		get_falseText: function() {
			return this.$1$FalseTextField;
		},
		set_falseText: function(value) {
			this.$1$FalseTextField = value;
		},
		get_trueText: function() {
			return this.$1$TrueTextField;
		},
		set_trueText: function(value) {
			this.$1$TrueTextField = value;
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_CheckboxFormatter, $asm, {
		format: function(ctx) {
			return '<span class="check-box no-float readonly ' + (!!ctx.value ? ' checked' : '') + '"></span>';
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_ColumnsKeyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_Criteria, $asm, {});
	ss.initClass($Serenity_DateFormatter, $asm, {
		get_displayFormat: function() {
			return this.$1$DisplayFormatField;
		},
		set_displayFormat: function(value) {
			this.$1$DisplayFormatField = value;
		},
		format: function(ctx) {
			return $Serenity_DateFormatter.format(ctx.value, this.get_displayFormat());
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_DateTimeFormatter, $asm, {}, $Serenity_DateFormatter, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_DialogTypeAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_EditorAttribute, $asm, {
		get_key: function() {
			return this.$2$KeyField;
		},
		set_key: function(value) {
			this.$2$KeyField = value;
		}
	});
	ss.initClass($Serenity_ElementAttribute, $asm, {
		get_html: function() {
			return this.$html;
		}
	});
	ss.initClass($Serenity_EntityTypeAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_EnumFormatter, $asm, {
		get_enumKey: function() {
			return this.$1$EnumKeyField;
		},
		set_enumKey: function(value) {
			this.$1$EnumKeyField = value;
		},
		format: function(ctx) {
			return $Serenity_EnumFormatter.format($Serenity_EnumTypeRegistry.get(this.get_enumKey()), ctx.value);
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_EnumKeyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_EnumTypeRegistry, $asm, {});
	ss.initClass($Serenity_FlexifyAttribute, $asm, {});
	ss.initClass($Serenity_FormKeyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_IdExtensions, $asm, {});
	ss.initClass($Serenity_IdPropertyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initInterface($Serenity_IInitializeColumn, $asm, { initializeColumn: null });
	ss.initClass($Serenity_IsActivePropertyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_ItemNameAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_LazyLoadHelper, $asm, {});
	ss.initClass($Serenity_LocalTextPrefixAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_MaximizableAttribute, $asm, {});
	ss.initClass($Serenity_MinuteFormatter, $asm, {
		format: function(ctx) {
			return $Serenity_MinuteFormatter.format(ctx.value);
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_NamePropertyAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_NumberFormatter, $asm, {
		get_displayFormat: function() {
			return this.$1$DisplayFormatField;
		},
		set_displayFormat: function(value) {
			this.$1$DisplayFormatField = value;
		},
		format: function(ctx) {
			return $Serenity_NumberFormatter.format(ctx.value, this.get_displayFormat());
		}
	}, null, [$Serenity_ISlickFormatter]);
	ss.initClass($Serenity_OptionsTypeAttribute, $asm, {
		get_optionsType: function() {
			return this.$2$OptionsTypeField;
		},
		set_optionsType: function(value) {
			this.$2$OptionsTypeField = value;
		}
	});
	ss.initClass($Serenity_PanelAttribute, $asm, {});
	ss.initClass($Serenity_ResizableAttribute, $asm, {});
	ss.initClass($Serenity_ScriptContext, $asm, {});
	ss.initClass($Serenity_ServiceAttribute, $asm, {
		get_value: function() {
			return this.$2$ValueField;
		},
		set_value: function(value) {
			this.$2$ValueField = value;
		}
	});
	ss.initClass($Serenity_SlickFormatting, $asm, {});
	ss.initClass($Serenity_SlickHelper, $asm, {});
	ss.initClass($Serenity_SlickTreeHelper, $asm, {});
	ss.initClass($Serenity_TabsExtensions, $asm, {});
	ss.initClass($Serenity_ComponentModel_OptionAttribute, $asm, {});
	ss.setMetadata($Serenity_BooleanFormatter, { members: [{ attr: [new $Serenity_ComponentModel_OptionAttribute()], name: 'FalseText', type: 16, returnType: String, getter: { name: 'get_FalseText', type: 8, sname: 'get_falseText', returnType: String, params: [] }, setter: { name: 'set_FalseText', type: 8, sname: 'set_falseText', returnType: Object, params: [String] } }, { attr: [new $Serenity_ComponentModel_OptionAttribute()], name: 'TrueText', type: 16, returnType: String, getter: { name: 'get_TrueText', type: 8, sname: 'get_trueText', returnType: String, params: [] }, setter: { name: 'set_TrueText', type: 8, sname: 'set_trueText', returnType: Object, params: [String] } }] });
	ss.setMetadata($Serenity_DateFormatter, { members: [{ attr: [new $Serenity_ComponentModel_OptionAttribute()], name: 'DisplayFormat', type: 16, returnType: String, getter: { name: 'get_DisplayFormat', type: 8, sname: 'get_displayFormat', returnType: String, params: [] }, setter: { name: 'set_DisplayFormat', type: 8, sname: 'set_displayFormat', returnType: Object, params: [String] } }] });
	ss.setMetadata($Serenity_EnumFormatter, { members: [{ attr: [new $Serenity_ComponentModel_OptionAttribute()], name: 'EnumKey', type: 16, returnType: String, getter: { name: 'get_EnumKey', type: 8, sname: 'get_enumKey', returnType: String, params: [] }, setter: { name: 'set_EnumKey', type: 8, sname: 'set_enumKey', returnType: Object, params: [String] } }] });
	ss.setMetadata($Serenity_NumberFormatter, { members: [{ attr: [new $Serenity_ComponentModel_OptionAttribute()], name: 'DisplayFormat', type: 16, returnType: String, getter: { name: 'get_DisplayFormat', type: 8, sname: 'get_displayFormat', returnType: String, params: [] }, setter: { name: 'set_DisplayFormat', type: 8, sname: 'set_displayFormat', returnType: Object, params: [String] } }] });
	(function() {
		$Q$Culture.decimalSeparator = '.';
		$Q$Culture.dateSeparator = '/';
		$Q$Culture.dateOrder = 'dmy';
		$Q$Culture.dateFormat = 'dd/MM/yyyy';
		$Q$Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
	})();
	(function() {
		$Q$LT.$table = {};
		$Q$LT.empty = new $Q$LT('');
	})();
	(function() {
		$Texts$Dialogs.OkButton = new Q$LT('OK');
		$Texts$Dialogs.YesButton = new Q$LT('Yes');
		$Texts$Dialogs.NoButton = new Q$LT('No');
		$Texts$Dialogs.CancelButton = new Q$LT('Cancel');
		$Texts$Dialogs.AlertTitle = new Q$LT('Alert');
		$Texts$Dialogs.ConfirmationTitle = new Q$LT('Confirm');
		$Texts$Dialogs.InformationTitle = new Q$LT('Information');
		$Texts$Dialogs.WarningTitle = new Q$LT('Warning');
		$Q$LT.initializeTextClass($Texts$Dialogs, 'Dialogs.');
	})();
	(function() {
		$Texts$Controls$SelectEditor.EmptyItemText = new Q$LT('--select--');
		$Texts$Controls$SelectEditor.InplaceAdd = new Q$LT('Define New');
		$Texts$Controls$SelectEditor.InplaceEdit = new Q$LT('Edit');
		$Texts$Controls$SelectEditor.ClickToDefine = new Q$LT('*** Click to define a new one ***');
		$Texts$Controls$SelectEditor.NoResultsClickToDefine = new Q$LT('*** No results. Click to define a new one ***');
		$Q$LT.initializeTextClass($Texts$Controls$SelectEditor, 'Controls.SelectEditor.');
	})();
	(function() {
		$Serenity_LazyLoadHelper.$autoIncrement = 0;
	})();
	(function() {
		$Q$Config.applicationPath = '/';
		$Q$Config.emailAllowOnlyAscii = false;
		$Q$Config.rootNamespaces = null;
		$Q$Config.notLoggedInHandler = null;
		var pathLink = $('link#ApplicationPath');
		if (pathLink.length > 0) {
			$Q$Config.applicationPath = pathLink.attr('href');
		}
		$Q$Config.rootNamespaces = [];
		$Q$Config.rootNamespaces.push('Serenity');
		$Q$Config.emailAllowOnlyAscii = true;
	})();
	(function() {
		$Q$ScriptData.$registered = {};
		$Q$ScriptData.$loadedData = {};
	})();
	(function() {
		$Q.$blockUICount = 0;
		var window1 = window.window;
		var rsvp = window1.RSVP;
		if (!!(ss.isValue(rsvp) && ss.isValue(rsvp.on))) {
			rsvp.on('error', function(e) {
				window1.console.log(e);
				window1.console.log((!!ss.isValue(e.get_stack) ? e.get_stack() : e.stack));
			});
		}
	})();
	(function() {
		$Texts$Controls$EntityDialog.DeleteConfirmation = new Q$LT('Delete record?');
		$Texts$Controls$EntityDialog.UndeleteButton = new Q$LT('Undelete');
		$Texts$Controls$EntityDialog.UndeleteConfirmation = new Q$LT('Undelete record?');
		$Texts$Controls$EntityDialog.CloneButton = new Q$LT('Clone');
		$Texts$Controls$EntityDialog.SaveSuccessMessage = new Q$LT('Save success');
		$Texts$Controls$EntityDialog.SaveButton = new Q$LT('Save');
		$Texts$Controls$EntityDialog.UpdateButton = new Q$LT('Update');
		$Texts$Controls$EntityDialog.ApplyChangesButton = new Q$LT('Apply Changes');
		$Texts$Controls$EntityDialog.DeleteButton = new Q$LT('Delete');
		$Texts$Controls$EntityDialog.LocalizationButton = new Q$LT('Localization');
		$Texts$Controls$EntityDialog.LocalizationBack = new Q$LT('Back to Form');
		$Texts$Controls$EntityDialog.LocalizationConfirmation = new Q$LT('Save changes to translations?');
		$Texts$Controls$EntityDialog.NewRecordTitle = new Q$LT('New {0}');
		$Texts$Controls$EntityDialog.EditRecordTitle = new Q$LT('Edit {0}{1}');
		$Q$LT.initializeTextClass($Texts$Controls$EntityDialog, 'Controls.EntityDialog.');
	})();
	(function() {
		$Texts$Controls$EntityGrid.NewButton = new Q$LT('New {0}');
		$Texts$Controls$EntityGrid.RefreshButton = new Q$LT('Refresh');
		$Texts$Controls$EntityGrid.IncludeDeletedToggle = new Q$LT('display inactive records');
		$Q$LT.initializeTextClass($Texts$Controls$EntityGrid, 'Controls.EntityGrid.');
	})();
	(function() {
		$Texts$Controls$Pager.Page = new Q$LT('Page');
		$Texts$Controls$Pager.PageStatus = new Q$LT('Showing {from} to {to} of {total} total records');
		$Texts$Controls$Pager.NoRowStatus = new Q$LT('No records');
		$Texts$Controls$Pager.LoadingStatus = new Q$LT('Please wait, loading data...');
		$Texts$Controls$Pager.DefaultLoadError = new Q$LT('An error occured while loading data!');
		$Q$LT.initializeTextClass($Texts$Controls$Pager, 'Controls.Pager.');
	})();
	(function() {
		$Texts$Controls$PropertyGrid.DefaultCategory = new Q$LT('Properties');
		$Texts$Controls$PropertyGrid.RequiredHint = new Q$LT('this field is required');
		$Q$LT.initializeTextClass($Texts$Controls$PropertyGrid, 'Controls.PropertyGrid.');
	})();
	(function() {
		$Texts$Controls$QuickSearch.Placeholder = new Q$LT('search...');
		$Texts$Controls$QuickSearch.Hint = new Q$LT('enter the text to search for...');
		$Texts$Controls$QuickSearch.FieldSelection = new Q$LT('select the field to search on');
		$Q$LT.initializeTextClass($Texts$Controls$QuickSearch, 'Controls.QuickSearch.');
	})();
	(function() {
		$Serenity_EnumTypeRegistry.$knownTypes = null;
	})();
})();

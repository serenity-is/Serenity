(function() {
	'use strict';
	var $asm = {};
	global.BasicApplication = global.BasicApplication || {};
	ss.initAssembly($asm, 'BasicApplication.Script');
	////////////////////////////////////////////////////////////////////////////////
	// BasicApplication.ScriptInitialization
	var $BasicApplication_ScriptInitialization = function() {
	};
	$BasicApplication_ScriptInitialization.__typeName = 'BasicApplication.ScriptInitialization';
	global.BasicApplication.ScriptInitialization = $BasicApplication_ScriptInitialization;
	ss.initClass($BasicApplication_ScriptInitialization, $asm, {});
	ss.add(Q$Config.rootNamespaces, 'BasicApplication');
})();

//! Script# Core Runtime
//! More information at http://projects.nikhilk.net/ScriptSharp
//!
if (typeof(global) === "undefined")
	global = window;

var ss = {};

ss.isUndefined = function ss$isUndefined(o) {
	return (o === undefined);
};

ss.isNull = function ss$isNull(o) {
	return (o === null);
};

ss.isNullOrUndefined = function ss$isNullOrUndefined(o) {
	return (o === null) || (o === undefined);
};

ss.isValue = function ss$isValue(o) {
	return (o !== null) && (o !== undefined);
};

ss.referenceEquals = function ss$referenceEquals(a, b) {
	return ss.isValue(a) ? a === b : !ss.isValue(b);
};

ss.mkdict = function ss$mkdict() {
	var a = (arguments.length != 1 ? arguments : arguments[0]);
	var r = {};
	for (var i = 0; i < a.length; i += 2) {
		r[a[i]] = a[i + 1];
	}
	return r;
};

ss.coalesce = function ss$coalesce(a, b) {
	return ss.isValue(a) ? a : b;
};

ss.isDate = function ss$isDate(obj) {
	return Object.prototype.toString.call(obj) === '[object Date]';
};

ss.isArray = function ss$isArray(obj) {
	return Object.prototype.toString.call(obj) === '[object Array]';
};

ss.isTypedArrayType = function ss$isTypedArrayType(type) {
	return ['Float32Array', 'Float64Array', 'Int8Array', 'Int16Array', 'Int32Array', 'Uint8Array', 'Uint16Array', 'Uint32Array', 'Uint8ClampedArray'].indexOf(ss.getTypeFullName(type)) >= 0;
};

ss.isArrayOrTypedArray = function ss$isArray(obj) {
	return ss.isArray(obj) || ss.isTypedArrayType(ss.getInstanceType(obj));
};

ss.getHashCode = function ss$getHashCode(obj) {
	if (!ss.isValue(obj))
		throw 'Cannot get hash code of null';
	else if (typeof(obj.getHashCode) === 'function')
		return obj.getHashCode();
	else if (typeof(obj) === 'boolean') {
		return obj ? 1 : 0;
	}
	else if (typeof(obj) === 'number') {
		var s = obj.toExponential();
		s = s.substr(0, s.indexOf('e'));
		return parseInt(s.replace('.', ''), 10) & 0xffffffff;
	}
	else if (typeof(obj) === 'string') {
		var res = 0;
		for (var i = 0; i < obj.length; i++)
			res = (res * 31 + obj.charCodeAt(i)) & 0xffffffff;
		return res;
	}
	else if (ss.isDate(obj)) {
		return obj.valueOf() & 0xffffffff;
	}
	else {
		return ss.defaultHashCode(obj);
	}
};

ss.defaultHashCode = function ss$defaultHashCode(obj) {
	return obj.$__hashCode__ || (obj.$__hashCode__ = (Math.random() * 0x100000000) | 0);
};

ss.equals = function ss$equals(a, b) {
	if (!ss.isValue(a))
		throw 'Object is null';
	else if (typeof(a.equals) === 'function')
		return a.equals(b);
	if (ss.isDate(a) && ss.isDate(b))
		return a.valueOf() === b.valueOf();
	else if (typeof(a) === 'function' && typeof(b) === 'function')
		return ss.delegateEquals(a, b);
	else if (ss.isNullOrUndefined(a) && ss.isNullOrUndefined(b))
		return true;
	else
		return a === b;
};

ss.compare = function ss$compare(a, b) {
	if (!ss.isValue(a))
		throw 'Object is null';
	else if (typeof(a) === 'number' || typeof(a) === 'string' || typeof(a) === 'boolean')
		return a < b ? -1 : (a > b ? 1 : 0);
	else if (ss.isDate(a))
		return ss.compare(a.valueOf(), b.valueOf());
	else
		return a.compareTo(b);
};

ss.equalsT = function ss$equalsT(a, b) {
	if (!ss.isValue(a))
		throw 'Object is null';
	else if (typeof(a) === 'number' || typeof(a) === 'string' || typeof(a) === 'boolean')
		return a === b;
	else if (ss.isDate(a))
		return a.valueOf() === b.valueOf();
	else
		return a.equalsT(b);
};

ss.staticEquals = function ss$staticEquals(a, b) {
	if (!ss.isValue(a))
		return !ss.isValue(b);
	else
		return ss.isValue(b) ? ss.equals(a, b) : false;
};

ss.shallowCopy = function ss$shallowCopy(source, target) {
	for (var p in source) {
		if (source.hasOwnProperty(p))
			target[p] = source[p];
	}
};

if (typeof(window) == 'object') {
	// Browser-specific stuff that could go into the Web assembly, but that assembly does not have an associated JS file.
	if (!window.Element) {
		// IE does not have an Element constructor. This implementation should make casting to elements work.
		window.Element = function() {};
		window.Element.isInstanceOfType = function(instance) { return instance && typeof instance.constructor === 'undefined' && typeof instance.tagName === 'string'; };
	}
	window.Element.__typeName = 'Element';
	window.Element.__baseType = Object;
	
	if (!window.XMLHttpRequest) {
		window.XMLHttpRequest = function() {
			var progIDs = [ 'Msxml2.XMLHTTP', 'Microsoft.XMLHTTP' ];
	
			for (var i = 0; i < progIDs.length; i++) {
				try {
					var xmlHttp = new ActiveXObject(progIDs[i]);
					return xmlHttp;
				}
				catch (ex) {
				}
			}
	
			return null;
		};
	}

	ss.parseXml = function(markup) {
		try {
			if (DOMParser) {
				var domParser = new DOMParser();
				return domParser.parseFromString(markup, 'text/xml');
			}
			else {
				var progIDs = [ 'Msxml2.DOMDocument.3.0', 'Msxml2.DOMDocument' ];

				for (var i = 0; i < progIDs.length; i++) {
					var xmlDOM = new ActiveXObject(progIDs[i]);
					xmlDOM.async = false;
					xmlDOM.loadXML(markup);
					xmlDOM.setProperty('SelectionLanguage', 'XPath');
					return xmlDOM;
				}
			}
		}
		catch (ex) {
		}

		return null;
	};
}

///////////////////////////////////////////////////////////////////////////////
// Type System Implementation

ss.registerType = function Type$registerType(root, typeName, type) {
	var ns = root;
	var nameParts = typeName.split('.');

	for (var i = 0; i < nameParts.length - 1; i++) {
		var part = nameParts[i];
		var nso = ns[part];
		if (!nso) {
			ns[part] = nso = {};
		}
		ns = nso;
	}
	ns[nameParts[nameParts.length - 1]] = type;
};

ss.__genericCache = {};

ss._makeGenericTypeName = function ss$_makeGenericTypeName(genericType, typeArguments) {
	var result = genericType.__typeName;
	for (var i = 0; i < typeArguments.length; i++)
		result += (i === 0 ? '[' : ',') + ss.getTypeFullName(typeArguments[i]);
	result += ']';
	return result;
};

ss.makeGenericType = function ss$makeGenericType(genericType, typeArguments) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	return ss.__genericCache[name] || genericType.apply(null, typeArguments);
};

ss.registerGenericClassInstance = function ss$registerGenericClassInstance(instance, genericType, typeArguments, baseType, interfaceTypes, metadata) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	ss.__genericCache[name] = instance;
	instance.__genericTypeDefinition = genericType;
	instance.__typeArguments = typeArguments;
	ss.registerClass(null, name, instance, baseType(), interfaceTypes(), metadata);
};

ss.registerGenericInterfaceInstance = function ss$registerGenericInterfaceInstance(instance, genericType, typeArguments, baseInterfaces, metadata) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	ss.__genericCache[name] = instance;
	instance.__genericTypeDefinition = genericType;
	instance.__typeArguments = typeArguments;
	ss.registerInterface(null, name, instance, baseInterfaces(), metadata);
};

ss.isGenericTypeDefinition = function ss$isGenericTypeDefinition(type) {
	return type.__isGenericTypeDefinition || false;
};

ss.getGenericTypeDefinition = function ss$getGenericTypeDefinition(type) {
	return type.__genericTypeDefinition || null;
};

ss.getGenericParameterCount = function ss$getGenericParameterCount(type) {
	return type.__typeArgumentCount || 0;
};

ss.getGenericArguments = function ss$getGenericArguments(type) {
	return type.__typeArguments || null;
};

ss._setMetadata = function ss$_setMetadata(type, metadata) {
	if (metadata.members) {
		for (var i = 0; i < metadata.members.length; i++) {
			var m = metadata.members[i];
			m.typeDef = type;
			if (m.adder) m.adder.typeDef = type;
			if (m.remover) m.remover.typeDef = type;
			if (m.getter) m.getter.typeDef = type;
			if (m.setter) m.setter.typeDef = type;
		}
	}
	type.__metadata = metadata;
}

ss.registerClass = function ss$registerClass(root, name, ctor, baseType, interfaces, metadata) {
	if (root)
		ss.registerType(root, name, ctor);

	ctor.prototype.constructor = ctor;
	ctor.__typeName = name;
	ctor.__class = true;
	ctor.__baseType = baseType || Object;
	if (interfaces)
		ctor.__interfaces = interfaces;
	if (metadata)
		ss._setMetadata(ctor, metadata);

	if (baseType) {
		ss.setupBase(ctor);
	}
};

ss.registerGenericClass = function ss$registerGenericClass(root, name, ctor, typeArgumentCount, metadata) {
	if (root)
		ss.registerType(root, name, ctor);

	ctor.prototype.constructor = ctor;
	ctor.__typeName = name;
	ctor.__class = true;
	ctor.__typeArgumentCount = typeArgumentCount;
	ctor.__isGenericTypeDefinition = true;
	ctor.__baseType = Object;
	if (metadata)
		ss._setMetadata(ctor, metadata);
};

ss.registerInterface = function ss$createInterface(root, name, ctor, baseInterfaces, metadata) {
	if (root)
		ss.registerType(root, name, ctor);

	ctor.__typeName = name;
	ctor.__interface = true;
	if (baseInterfaces)
		ctor.__interfaces = baseInterfaces;
	if (metadata)
		ss._setMetadata(ctor, metadata);
};

ss.registerGenericInterface = function ss$registerGenericClass(root, name, ctor, typeArgumentCount, metadata) {
	if (root)
		ss.registerType(root, name, ctor);

	ctor.prototype.constructor = ctor;
	ctor.__typeName = name;
	ctor.__interface = true;;
	ctor.__typeArgumentCount = typeArgumentCount;
	ctor.__isGenericTypeDefinition = true;
	if (metadata)
		ss._setMetadata(ctor, metadata);
};

ss.registerEnum = function ss$registerEnum(root, name, ctor, metadata) {
	if (root)
		ss.registerType(root, name, ctor);

	for (var field in ctor.prototype)
		ctor[field] = ctor.prototype[field];

	ctor.__typeName = name;
	ctor.__enum = true;
	if (metadata)
		ss._setMetadata(ctor, metadata);
	ctor.getDefaultValue = ctor.createInstance = function() { return 0; };
	ctor.isInstanceOfType = function(instance) { return typeof(instance) == 'number'; };
};

ss.setupBase = function Type$setupBase(type) {
	var baseType = type.__baseType;

	for (var memberName in baseType.prototype) {
		var memberValue = baseType.prototype[memberName];
		if (!type.prototype[memberName]) {
			type.prototype[memberName] = memberValue;
		}
	}
};

ss.getBaseType = function ss$getBaseType(type) {
	return type.__baseType || (type === Object ? null : Object);
};

ss.getTypeFullName = function ss$getTypeFullName(type) {
	return type.__typeName || type.name || (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

ss.getTypeName = function ss$getTypeName(type) {
	var fullName = ss.getTypeFullName(type);
	var bIndex = fullName.indexOf('[');
	var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
	return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
};

ss.getInterfaces = function ss$getInterfaces(type) {
	if (type.__interfaces)
		return type.__interfaces;
	else if (type === Date || type === Number)
		return [ ss_IEquatable, ss_IComparable, ss_IFormattable ];
	else if (type === Boolean || type === String)
		return [ ss_IEquatable, ss_IComparable ];
	else if (type === Array || ss.isTypedArrayType(type))
		return [ ss_IEnumerable, ss_ICollection, ss_IList ];
	else
		return [];
};

ss.isInstanceOfType = function ss$isInstanceOfType(instance, type) {
	if (ss.isNullOrUndefined(instance))
		return false;

	if (typeof(type.isInstanceOfType) === 'function')
		return type.isInstanceOfType(instance);

	if ((type == Object) || (instance instanceof type)) {
		return true;
	}

	return ss.isAssignableFrom(type, ss.getInstanceType(instance));
};

ss.isAssignableFrom = function ss$isAssignableFrom(target, type) {
	if ((target == Object) || (target == type)) {
		return true;
	}
	if (target.__class) {
		var baseType = type.__baseType;
		while (baseType) {
			if (target == baseType) {
				return true;
			}
			baseType = baseType.__baseType;
		}
	}
	else if (target.__interface) {
		var interfaces = ss.getInterfaces(type);
		if (interfaces && ss.contains(interfaces, target)) {
			return true;
		}

		var baseType = ss.getBaseType(type);
		while (baseType) {
			interfaces = ss.getInterfaces(baseType);
			if (interfaces && ss.contains(interfaces, target)) {
				return true;
			}
			baseType = ss.getBaseType(baseType);
		}
	}
	return false;
};

ss.hasProperty = function ss$hasProperty(instance, name) {
	return typeof(instance['get_' + name]) === 'function' || typeof(instance['set_' + name]) === 'function';
};

ss.isClass = function Type$isClass(type) {
	return (type.__class == true || type === Array || type === Function || type === RegExp || type === String || type === Error || type === Object);
};

ss.isEnum = function Type$isEnum(type) {
	return (type.__enum == true);
};

ss.isFlags = function Type$isFlags(type) {
	return type.__metadata && type.__metadata.enumFlags;
};

ss.isInterface = function Type$isInterface(type) {
	return (type.__interface == true);
};

ss.safeCast = function ss$safeCast(instance, type) {
	if (type === true)
		return instance;
	else if (type === false)
		return null;
	else
		return ss.isInstanceOfType(instance, type) ? instance : null;
};

ss.cast = function ss$cast(instance, type) {
	if (instance === null || type === false)
		return null;
	else if (typeof(instance) === "undefined" || type === true || ss.isInstanceOfType(instance, type)) {
		return instance;
	}
	throw 'Cannot cast object to type ' + ss.getTypeFullName(type);
};

ss.getInstanceType = function ss$getInstanceType(instance) {
	if (instance === null)
		throw 'Cannot get type of null';
	if (typeof(instance) === "undefined")
		throw 'Cannot get type of undefined';

	var ctor = null;

	// NOTE: We have to catch exceptions because the constructor
	//       cannot be looked up on native COM objects
	try {
		ctor = instance.constructor;
	}
	catch (ex) {
	}
	return ctor || Object;
};

ss.getType = function ss$getType(typeName) {
	if (!typeName) {
		return null;
	}

	if (!ss.__typeCache) {
		ss.__typeCache = {};
	}

	var type = ss.__typeCache[typeName];
	if (!type) {
		var arr = typeName.split(',');
		var type = (arr.length > 1 ? require(arr[1].trim) : global);

		var parts = arr[0].trim().split('.');
		for (var i = 0; i < parts.length; i++) {
			type = type[parts[i]];
			if (!type)
				break;
		}

		ss.__typeCache[typeName] = type || null;
	}
	return type;
};

ss.getDefaultValue = function ss$getDefaultValue(type) {
	if (typeof(type.getDefaultValue) === 'function')
		return type.getDefaultValue();
	else if (type === Boolean)
		return false;
	else if (type === Date)
		return new Date(0);
	else if (type === Number)
		return 0;
	return null;
};

ss.createInstance = function ss$createInstance(type) {
	if (typeof(type.createInstance) === 'function')
		return type.createInstance();
	else if (type === Boolean)
		return false;
	else if (type === Date)
		return new Date(0);
	else if (type === Number)
		return 0;
	else if (type === String)
		return '';
	else
		return new type();
};

ss.applyConstructor = function ss$applyConstructor(constructor, args) {
	var f = function() {
		constructor.apply(this, args);
	};
	f.prototype = constructor.prototype;
	return new f();
};

ss.getAttributes = function ss$getAttributes(type, attrType, inherit) {
	var result = inherit && type.__baseType ? ss.getAttributes(type.__baseType, attrType, true).filter(function(a) { var t = ss.getInstanceType(a); return !t.__metadata || !t.__metadata.attrNoInherit; }) : [];
	if (type.__metadata && type.__metadata.attr) {
		for (var i = 0; i < type.__metadata.attr.length; i++) {
			var a = type.__metadata.attr[i];
			if (attrType == null || ss.isInstanceOfType(a, attrType)) {
				var t = ss.getInstanceType(a);
				if (!t.__metadata || !t.__metadata.attrAllowMultiple)
					result = result.filter(function (a) { return !ss.isInstanceOfType(a, t); });
				result.push(a);
			}
		}
	}
	return result;
};

ss.getMembers = function ss$getAttributes(type, memberTypes, bindingAttr, name, params) {
	var result = type.__baseType && ((bindingAttr & 72) == 72 || (bindingAttr & 6) == 4) ? ss.getMembers(type.__baseType, memberTypes & ~1, bindingAttr & (bindingAttr & 64 ? 255 : 247) & (bindingAttr & 2 ? 251 : 255), name, params) : [];

	var f = function(m) {
		if ((memberTypes & m.type) && (((bindingAttr & 4) && !m.isStatic) || ((bindingAttr & 8) && m.isStatic)) && (!name || m.name === name)) {
			if (params) {
				if ((m.params || []).length !== params.length)
					return;
				for (var i = 0; i < params.length; i++) {
					if (params[i] !== m.params[i])
						return;
				}
			}
			result.push(m);
		}
	};

	if (type.__metadata && type.__metadata.members) {
		for (var i = 0; i < type.__metadata.members.length; i++) {
			var m = type.__metadata.members[i];
			f(m);
			['getter','setter','adder','remover'].forEach(function(e) { if (m[e]) f(m[e]); });
		}
	}

	if (bindingAttr & 256) {
		while (type) {
			var r = result.filter(function(m) { return m.typeDef === type; });
			if (r.length > 1)
				throw 'Ambiguous match';
			else if (r.length === 1)
				return r[0];
			type = type.__baseType;
		}
		return null;
	}

	return result;
};

ss.midel = function ss$midel(mi, target, typeArguments) {
	if (mi.isStatic && !!target)
		throw 'Cannot specify target for static method';
	else if (!mi.isStatic && !target)
		throw 'Must specify target for instance method';

	var method;
	if (mi.fget) {
		method = function() { return (mi.isStatic ? mi.typeDef : this)[mi.fget]; };
	}
	else if (mi.fset) {
		method = function(v) { (mi.isStatic ? mi.typeDef : this)[mi.fset] = v; };
	}
	else {
		method = mi.def || (mi.isStatic || mi.sm ? mi.typeDef[mi.sname] : target[mi.sname]);

		if (mi.tpcount) {
			if (!typeArguments || typeArguments.length !== mi.tpcount)
				throw 'Wrong number of type arguments';
			method = method.apply(null, typeArguments);
		}
		else {
			if (typeArguments && typeArguments.length)
				throw 'Cannot specify type arguments for non-generic method';
		}
		if (mi.exp) {
			var _m1 = method;
			method = function () { return _m1.apply(this, Array.prototype.slice.call(arguments, 0, arguments.length - 1).concat(arguments[arguments.length - 1])); };
		}
		if (mi.sm) {
			var _m2 = method;
			method = function() { return _m2.apply(null, [this].concat(Array.prototype.slice.call(arguments))); };
		}
	}
	return ss.mkdel(target, method);
};

ss.invokeCI = function ss$invokeCI(ci, args) {
	if (ci.exp)
		args = args.slice(0, args.length - 1).concat(args[args.length - 1]);

	if (ci.def)
		return ci.def.apply(null, args);
	else if (ci.sm)
		return ci.typeDef[ci.sname].apply(null, args);
	else
		return ss.applyConstructor(ci.sname ? ci.typeDef[ci.sname] : ci.typeDef, args);
};

ss.fieldAccess = function ss$fieldAccess(fi, obj) {
	if (fi.isStatic && !!obj)
		throw 'Cannot specify target for static field';
	else if (!fi.isStatic && !obj)
		throw 'Must specify target for instance field';
	obj = fi.isStatic ? fi.typeDef : obj;
	if (arguments.length === 3)
		obj[fi.sname] = arguments[2];
	else
		return obj[fi.sname];
};

///////////////////////////////////////////////////////////////////////////////
// IFormattable

var ss_IFormattable = function IFormattable$() { };
ss_IFormattable.prototype = {
	format: null
};

ss.registerInterface(global, 'ss.IFormattable', ss_IFormattable);

ss.format = function ss$format(obj, fmt) {
	if (typeof(obj) === 'number')
		return ss.formatNumber(obj, fmt);
	else if (ss.isDate(obj))
		return ss.formatDate(obj, fmt);
	else
		return obj.format(fmt);
};

///////////////////////////////////////////////////////////////////////////////
// IComparable

var ss_IComparable = function IComparable$() { };
ss_IComparable.prototype = {
	compareTo: null
};

ss.registerInterface(global, 'ss.IComparable', ss_IComparable);

///////////////////////////////////////////////////////////////////////////////
// IEquatable

var ss_IEquatable = function IEquatable$() { };
ss_IEquatable.prototype = {
	equalsT: null
};

ss.registerInterface(global, 'ss.IEquatable', ss_IEquatable);

///////////////////////////////////////////////////////////////////////////////
// Object Extensions

ss.clearKeys = function ss$clearKeys(d) {
	for (var n in d) {
		if (d.hasOwnProperty(n))
			delete d[n];
	}
};

ss.keyExists = function ss$keyExists(d, key) {
	return d[key] !== undefined;
};

if (!Object.keys) {
	Object.keys = function Object$keys(d) {
		var keys = [];
		for (var n in d) {
			if (d.hasOwnProperty(n))
				keys.push(n);
		}
		return keys;
	};

	ss.getKeyCount = function ss$getKeyCount(d) {
		var count = 0;
		for (var n in d) {
			if (d.hasOwnProperty(n))
				count++;
		}
		return count;
	};
}
else {
	ss.getKeyCount = function ss$getKeyCount2(d) {
		return Object.keys(d).length;
	};
}

///////////////////////////////////////////////////////////////////////////////
// Number Extensions

ss.formatNumber = function ss$formatNumber(num, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return num.toString();
	}
	return ss._netFormatNumber(num, format, false);
};

ss.localeFormatNumber = function ss$localeFormatNumber(num, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return num.toLocaleString();
	}
	return ss._netFormatNumber(num, format, true);
};

ss._commaFormatNumber = function ss$_commaFormat(number, groups, decimal, comma) {
	var decimalPart = null;
	var decimalIndex = number.indexOf(decimal);
	if (decimalIndex > 0) {
		decimalPart = number.substr(decimalIndex);
		number = number.substr(0, decimalIndex);
	}

	var negative = ss.startsWithString(number, '-');
	if (negative) {
		number = number.substr(1);
	}

	var groupIndex = 0;
	var groupSize = groups[groupIndex];
	if (number.length < groupSize) {
		return (negative ? '-' : '') + (decimalPart ? number + decimalPart : number);
	}

	var index = number.length;
	var s = '';
	var done = false;
	while (!done) {
		var length = groupSize;
		var startIndex = index - length;
		if (startIndex < 0) {
			groupSize += startIndex;
			length += startIndex;
			startIndex = 0;
			done = true;
		}
		if (!length) {
			break;
		}
		
		var part = number.substr(startIndex, length);
		if (s.length) {
			s = part + comma + s;
		}
		else {
			s = part;
		}
		index -= length;

		if (groupIndex < groups.length - 1) {
			groupIndex++;
			groupSize = groups[groupIndex];
		}
	}

	if (negative) {
		s = '-' + s;
	}    
	return decimalPart ? s + decimalPart : s;
};

ss._netFormatNumber = function ss$_netFormatNumber(num, format, useLocale) {
	var nf = useLocale ? ss_CultureInfo.CurrentCulture.numberFormat : ss_CultureInfo.InvariantCulture.numberFormat;

	var s = '';    
	var precision = -1;
	
	if (format.length > 1) {
		precision = parseInt(format.substr(1));
	}

	var fs = format.charAt(0);
	switch (fs) {
		case 'd': case 'D':
			s = parseInt(Math.abs(num)).toString();
			if (precision != -1) {
				s = ss.padLeftString(s, precision, 0x30);
			}
			if (num < 0) {
				s = '-' + s;
			}
			break;
		case 'x': case 'X':
			s = parseInt(Math.abs(num)).toString(16);
			if (fs == 'X') {
				s = s.toUpperCase();
			}
			if (precision != -1) {
				s = ss.padLeftString(s, precision, 0x30);
			}
			break;
		case 'e': case 'E':
			if (precision == -1) {
				s = num.toExponential();
			}
			else {
				s = num.toExponential(precision);
			}
			if (fs == 'E') {
				s = s.toUpperCase();
			}
			break;
		case 'f': case 'F':
		case 'n': case 'N':
			if (precision == -1) {
				precision = nf.numberDecimalDigits;
			}
			s = num.toFixed(precision).toString();
			if (precision && (nf.numberDecimalSeparator != '.')) {
				var index = s.indexOf('.');
				s = s.substr(0, index) + nf.numberDecimalSeparator + s.substr(index + 1);
			}
			if ((fs == 'n') || (fs == 'N')) {
				s = ss._commaFormatNumber(s, nf.numberGroupSizes, nf.numberDecimalSeparator, nf.numberGroupSeparator);
			}
			break;
		case 'c': case 'C':
			if (precision == -1) {
				precision = nf.currencyDecimalDigits;
			}
			s = Math.abs(num).toFixed(precision).toString();
			if (precision && (nf.currencyDecimalSeparator != '.')) {
				var index = s.indexOf('.');
				s = s.substr(0, index) + nf.currencyDecimalSeparator + s.substr(index + 1);
			}
			s = ss._commaFormatNumber(s, nf.currencyGroupSizes, nf.currencyDecimalSeparator, nf.currencyGroupSeparator);
			if (num < 0) {
				s = ss.formatString(nf.currencyNegativePattern, s);
			}
			else {
				s = ss.formatString(nf.currencyPositivePattern, s);
			}
			break;
		case 'p': case 'P':
			if (precision == -1) {
				precision = nf.percentDecimalDigits;
			}
			s = (Math.abs(num) * 100.0).toFixed(precision).toString();
			if (precision && (nf.percentDecimalSeparator != '.')) {
				var index = s.indexOf('.');
				s = s.substr(0, index) + nf.percentDecimalSeparator + s.substr(index + 1);
			}
			s = ss._commaFormatNumber(s, nf.percentGroupSizes, nf.percentDecimalSeparator, nf.percentGroupSeparator);
			if (num < 0) {
				s = ss.formatString(nf.percentNegativePattern, s);
			}
			else {
				s = ss.formatString(nf.percentPositivePattern, s);
			}
			break;
	}

	return s;
};

///////////////////////////////////////////////////////////////////////////////
// String Extensions

ss.compareStrings = function ss$compareStrings(s1, s2, ignoreCase) {
	if (ignoreCase) {
		if (s1) {
			s1 = s1.toUpperCase();
		}
		if (s2) {
			s2 = s2.toUpperCase();
		}
	}
	s1 = s1 || '';
	s2 = s2 || '';

	if (s1 == s2) {
		return 0;
	}
	if (s1 < s2) {
		return -1;
	}
	return 1;
};

ss.endsWithString = function ss$endsWithString(s, suffix) {
	if (!suffix.length) {
		return true;
	}
	if (suffix.length > s.length) {
		return false;
	}
	return (s.substr(s.length - suffix.length) == suffix);
};

ss._formatString = function ss$_formatString(format, values, useLocale) {
	if (!ss._formatRE) {
		ss._formatRE = /(\{[^\}^\{]+\})/g;
	}

	return format.replace(ss._formatRE,
						  function(str, m) {
							  var index = parseInt(m.substr(1));
							  var value = values[index + 1];
							  if (ss.isNullOrUndefined(value)) {
								  return '';
							  }
							  if (ss.isInstanceOfType(value, ss_IFormattable)) {
								  var formatSpec = null;
								  var formatIndex = m.indexOf(':');
								  if (formatIndex > 0) {
									  formatSpec = m.substring(formatIndex + 1, m.length - 1);
								  }
								  return ss.format(value, formatSpec);
							  }
							  else {
								  return useLocale ? value.toLocaleString() : value.toString();
							  }
						  });
};

ss.formatString = function String$format(format) {
	return ss._formatString(format, arguments, /* useLocale */ false);
};

ss.stringFromChar = function ss$stringFromChar(ch, count) {
	var s = ch;
	for (var i = 1; i < count; i++) {
		s += ch;
	}
	return s;
};

ss.htmlDecode = function ss$htmlDecode(s) {
	return s.replace(/&([^;]+);/g, function(_, e) {
		if (e[0] === '#')
			return String.fromCharCode(parseInt(e.substr(1)));
		switch (e) {
			case 'quot': return '"';
			case 'apos': return "'";
			case 'amp': return '&';
			case 'lt': return '<';
			case 'gt': return '>';
			default : return '&' + e + ';';
		}
	});
};

ss.htmlEncode = function ss$htmlEncode(s) {
	return s.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
};

ss.jsEncode = function ss$jsEncode(s, q) {
	s = s.replace(/\\/g, '\\\\').replace(/'/g, "\\'").replace(/"/g, '\\"');
	return q ? '"' + s + '"' : s;
};

ss.indexOfAnyString = function ss$indexOfAnyString(s, chars, startIndex, count) {
	var length = s.length;
	if (!length) {
		return -1;
	}

	chars = String.fromCharCode.apply(null, chars);
	startIndex = startIndex || 0;
	count = count || length;

	var endIndex = startIndex + count - 1;
	if (endIndex >= length) {
		endIndex = length - 1;
	}

	for (var i = startIndex; i <= endIndex; i++) {
		if (chars.indexOf(s.charAt(i)) >= 0) {
			return i;
		}
	}
	return -1;
};

ss.insertString = function ss$insertString(s, index, value) {
	if (!value) {
		return s;
	}
	if (!index) {
		return value + s;
	}
	var s1 = s.substr(0, index);
	var s2 = s.substr(index);
	return s1 + value + s2;
};

ss.isNullOrEmptyString = function ss$isNullOrEmptyString(s) {
	return !s || !s.length;
};

ss.lastIndexOfAnyString = function ss$lastIndexOfAnyString(s, chars, startIndex, count) {
	var length = s.length;
	if (!length) {
		return -1;
	}

	chars = String.fromCharCode.apply(null, chars);
	startIndex = startIndex || length - 1;
	count = count || length;

	var endIndex = startIndex - count + 1;
	if (endIndex < 0) {
		endIndex = 0;
	}

	for (var i = startIndex; i >= endIndex; i--) {
		if (chars.indexOf(s.charAt(i)) >= 0) {
			return i;
		}
	}
	return -1;
};

ss.localeFormatString = function ss$localeFormatString(format) {
	return ss._formatString(format, arguments, /* useLocale */ true);
};

ss.padLeftString = function ss$padLeftString(s, totalWidth, ch) {
	if (s.length < totalWidth) {
		ch = String.fromCharCode(ch || 0x20);
		return ss.stringFromChar(ch, totalWidth - s.length) + s;
	}
	return s;
};

ss.padRightString = function ss$padRightString(s, totalWidth, ch) {
	if (s.length < totalWidth) {
		ch = String.fromCharCode(ch || 0x20);
		return s + ss.stringFromChar(ch, totalWidth - s.length);
	}
	return s;
};

ss.removeString = function ss$removeString(s, index, count) {
	if (!count || ((index + count) > this.length)) {
		return s.substr(0, index);
	}
	return s.substr(0, index) + s.substr(index + count);
};

ss.replaceAllString = function ss$replaceAllString(s, oldValue, newValue) {
	newValue = newValue || '';
	return s.split(oldValue).join(newValue);
};

ss.startsWithString = function ss$startsWithString(s, prefix) {
	if (!prefix.length) {
		return true;
	}
	if (prefix.length > s.length) {
		return false;
	}
	return (s.substr(0, prefix.length) == prefix);
};

if (!String.prototype.trim) {
	String.prototype.trim = function String$trim() {
		return ss.trimStartString(ss.trimEndString(this));
	};
}

ss.trimEndString = function ss$trimEndString(s, chars) {
	return s.replace(chars ? new RegExp('[' + String.fromCharCode.apply(null, chars) + ']+$') : /\s*$/, '');
};

ss.trimStartString = function ss$trimStartString(s, chars) {
	return s.replace(chars ? new RegExp('^[' + String.fromCharCode.apply(null, chars) + ']+') : /^\s*/, '');
};

ss.trimString = function ss$trimString(s, chars) {
	return ss.trimStartString(ss.trimEndString(s, chars), chars);
};

ss.lastIndexOfString = function ss$lastIndexOfString(s, search, startIndex, count) {
	var index = s.lastIndexOf(search, startIndex);
	return (index < (startIndex - count + 1)) ? -1 : index;
};

ss.indexOfString = function ss$indexOfString(s, search, startIndex, count) {
	var index = s.indexOf(search, startIndex);
	return ((index + search.length) <= (startIndex + count)) ? index : -1;
};

///////////////////////////////////////////////////////////////////////////////
// Math Extensions

ss.divRem = function ss$divRem(a, b, result) {
	var remainder = a % b;
	result.$ = remainder;
	return (a - remainder) / b;
};

ss.round = function ss$round(n, d, rounding) {
	var m = Math.pow(10, d || 0);
	n *= m;
	var sign = (n > 0) | -(n < 0);
	if (n % 1 === 0.5 * sign) {
		var f = Math.floor(n);
		return (f + (rounding ? (sign > 0) : (f % 2 * sign))) / m;
	}

	return Math.round(n) / m;
};

///////////////////////////////////////////////////////////////////////////////
// Array Extensions

ss.arrayGet2 = function ss$arrayGet2(arr, indices) {
	if (indices.length != (arr._sizes ? arr._sizes.length : 1))
		throw 'Invalid number of indices';

	if (indices[0] < 0 || indices[0] >= (arr._sizes ? arr._sizes[0] : arr.length))
		throw 'Index 0 out of range';

	var idx = indices[0];
	if (arr._sizes) {
		for (var i = 1; i < arr._sizes.length; i++) {
			if (indices[i] < 0 || indices[i] >= arr._sizes[i])
				throw 'Index ' + i + ' out of range';
			idx = idx * arr._sizes[i] + indices[i];
		}
	}
	var r = arr[idx];
	return typeof r !== 'undefined' ? r : arr._defvalue;
};

ss.arrayGet = function ss$arrayGet(arr) {
	return ss.arrayGet2(arr, Array.prototype.slice.call(arguments, 1));
}

ss.arraySet2 = function ss$arraySet2(arr, value, indices) {
	if (indices.length != (arr._sizes ? arr._sizes.length : 1))
		throw 'Invalid number of indices';

	if (indices[0] < 0 || indices[0] >= (arr._sizes ? arr._sizes[0] : arr.length))
		throw 'Index 0 out of range';

	var idx = indices[0];
	if (arr._sizes) {
		for (var i = 1; i < arr._sizes.length; i++) {
			if (indices[i] < 0 || indices[i] >= arr._sizes[i])
				throw 'Index ' + i + ' out of range';
			idx = idx * arr._sizes[i] + indices[i];
		}
	}
	arr[idx] = value;
};

ss.arraySet = function ss$arraySet() {
	return ss.arraySet2(arguments[0], arguments[arguments.length - 1], Array.prototype.slice.call(arguments, 1, arguments.length - 1));
};

ss.arrayRank = function ss$arrayRank(arr) {
	return arr._sizes ? arr._sizes.length : 1;
};

ss.arrayLength = function ss$arrayLength(arr, dimension) {
	if (dimension >= (arr._sizes ? arr._sizes.length : 1))
		throw 'Invalid dimension';
	return arr._sizes ? arr._sizes[dimension] : arr.length;
};

ss.arrayExtract = function ss$arrayExtract(arr, start, count) {
	if (!ss.isValue(count)) {
		return arr.slice(start);
	}
	return arr.slice(start, start + count);
};

ss.arrayAddRange = function ss$arrayAddRange(arr, items) {
	if (items instanceof Array) {
		arr.push.apply(arr, items);
	}
	else {
		var e = ss.getEnumerator(items);
		try {
			while (e.moveNext()) {
				ss.add(arr, e.current());
			}
		}
		finally {
			if (ss.isInstanceOfType(e, ss_IDisposable)) {
				ss.cast(e, ss_IDisposable).dispose();
			}
		}
	}
};

ss.arrayClone = function ss$arrayClone(arr) {
	if (arr.length === 1) {
		return [arr[0]];
	}
	else {
		return Array.apply(null, arr);
	}
};

ss.arrayPeekFront = function ss$arrayPeekFront(arr) {
	if (arr.length)
		return arr[0];
	throw 'Array is empty';
};

ss.arrayPeekBack = function ss$arrayPeekBack(arr) {
	if (arr.length)
		return arr[arr.length - 1];
	throw 'Array is empty';
};

if (!Array.prototype.every) {
	Array.prototype.every = function Array$every(callback, instance) {
		var length = this.length;
		for (var i = 0; i < length; i++) {
			if (i in this && !callback.call(instance, this[i], i, this)) {
				return false;
			}
		}
		return true;
	};
}

if (!Array.prototype.filter) {
	Array.prototype.filter = function Array$filter(callback, instance) {
		var length = this.length;    
		var filtered = [];
		for (var i = 0; i < length; i++) {
			if (i in this) {
				var val = this[i];
				if (callback.call(instance, val, i, this)) {
					filtered.push(val);
				}
			}
		}
		return filtered;
	};
}

if (!Array.prototype.forEach) {
	Array.prototype.forEach = function Array$forEach(callback, instance) {
		var length = this.length;
		for (var i = 0; i < length; i++) {
			if (i in this) {
				callback.call(instance, this[i], i, this);
			}
		}
	};
}

ss.indexOfArray = function ss$indexOfArray(arr, item, startIndex) {
	startIndex = startIndex || 0;
	for (var i = startIndex; i < arr.length; i++) {
		if (ss.staticEquals(arr[i], item)) {
			return i;
		}
	}
	return -1;
}

ss.arrayInsertRange = function ss$arrayInsertRange(arr, index, items) {
	if (items instanceof Array) {
		if (index === 0) {
			arr.unshift.apply(arr, items);
		}
		else {
			for (var i = 0; i < items.length; i++) {
				arr.splice(index + i, 0, items[i]);
			}
		}
	}
	else {
		var e = ss.getEnumerator(items);
		try {
			while (e.moveNext()) {
				arr.insert(index, e.current());
				index++;
			}
		}
		finally {
			if (ss.isInstanceOfType(e, ss_IDisposable)) {
				ss.cast(e, ss_IDisposable).dispose();
			}
		}
	}
};

if (!Array.prototype.map) {
	Array.prototype.map = function Array$map(callback, instance) {
		var length = this.length;
		var mapped = new Array(length);
		for (var i = 0; i < length; i++) {
			if (i in this) {
				mapped[i] = callback.call(instance, this[i], i, this);
			}
		}
		return mapped;
	};
}

ss.arrayRemoveRange = function ss$arrayRemoveRange(arr, index, count) {
	arr.splice(index, count);
};

if (!Array.prototype.some) {
	Array.prototype.some = function Array$some(callback, instance) {
		var length = this.length;
		for (var i = 0; i < length; i++) {
			if (i in this && callback.call(instance, this[i], i, this)) {
				return true;
			}
		}
		return false;
	};
}

ss.arrayFromEnumerable = function ss$arrayFromEnumerable(enm) {
	var e = ss.getEnumerator(enm), r = [];
	try {
		while (e.moveNext())
			r.push(e.current());
	}
	finally {
		e.dispose();
	}
	return r;
};

ss.multidimArray = function ss$multidimArray(defvalue, sizes) {
	var arr = [];
	arr._defvalue = defvalue;
	arr._sizes = [arguments[1]];
	var length = arguments[1];
	for (var i = 2; i < arguments.length; i++) {
		length *= arguments[i];
		arr._sizes[i - 1] = arguments[i];
	}
	arr.length = length;
	return arr;
};

ss.repeat = function ss$repeat(value, count) {
	var result = [];
	for (var i = 0; i < count; i++)
		result.push(value);
	return result;
};

///////////////////////////////////////////////////////////////////////////////
// Date Extensions

ss.utcNow = function ss$utcNow() {
	var d = new Date();
	return new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds(), d.getMilliseconds()));
};

ss.today = function ss$today() {
	var d = new Date();
	return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

ss.formatDate = function ss$formatDate(date, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return date.toString();
	}
	if (format == 'id') {
		return date.toDateString();
	}
	if (format == 'it') {
		return date.toTimeString();
	}

	return ss._netFormatDate(date, format, false);
};

ss.localeFormatDate = function ss$localeFormatDate(date, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return date.toLocaleString();
	}
	if (format == 'id') {
		return date.toLocaleDateString();
	}
	if (format == 'it') {
		return date.toLocaleTimeString();
	}

	return ss._netFormatDate(date, format, true);
};

ss._netFormatDate = function ss$_netFormatDate(dt, format, useLocale) {
	var dtf = useLocale ? ss_CultureInfo.CurrentCulture.dateFormat : ss_CultureInfo.InvariantCulture.dateFormat;

	if (format.length == 1) {
		switch (format) {
			case 'f': format = dtf.longDatePattern + ' ' + dtf.shortTimePattern; break;
			case 'F': format = dtf.dateTimePattern; break;

			case 'd': format = dtf.shortDatePattern; break;
			case 'D': format = dtf.longDatePattern; break;

			case 't': format = dtf.shortTimePattern; break;
			case 'T': format = dtf.longTimePattern; break;

			case 'g': format = dtf.shortDatePattern + ' ' + dtf.shortTimePattern; break;
			case 'G': format = dtf.shortDatePattern + ' ' + dtf.longTimePattern; break;

			case 'R': case 'r':
				dtf = ss_CultureInfo.InvariantCulture.dateFormat;
				format = dtf.gmtDateTimePattern;
				break;
			case 'u': format = dtf.universalDateTimePattern; break;
			case 'U':
				format = dtf.dateTimePattern;
				dt = new Date(dt.getUTCFullYear(), dt.getUTCMonth(), dt.getUTCDate(),
							  dt.getUTCHours(), dt.getUTCMinutes(), dt.getUTCSeconds(), dt.getUTCMilliseconds());
				break;

			case 's': format = dtf.sortableDateTimePattern; break;
		}
	}

	if (format.charAt(0) == '%') {
		format = format.substr(1);
	}

	if (!Date._formatRE) {
		Date._formatRE = /'.*?[^\\]'|dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z/g;
	}

	var re = Date._formatRE;
	var sb = new ss_StringBuilder();

	re.lastIndex = 0;
	while (true) {
		var index = re.lastIndex;
		var match = re.exec(format);

		sb.append(format.slice(index, match ? match.index : format.length));
		if (!match) {
			break;
		}

		var fs = match[0];
		var part = fs;
		switch (fs) {
			case 'dddd':
				part = dtf.dayNames[dt.getDay()];
				break;
			case 'ddd':
				part = dtf.shortDayNames[dt.getDay()];
				break;
			case 'dd':
				part = ss.padLeftString(dt.getDate().toString(), 2, 0x30);
				break;
			case 'd':
				part = dt.getDate();
				break;
			case 'MMMM':
				part = dtf.monthNames[dt.getMonth()];
				break;
			case 'MMM':
				part = dtf.shortMonthNames[dt.getMonth()];
				break;
			case 'MM':
				part = ss.padLeftString((dt.getMonth() + 1).toString(), 2, 0x30);
				break;
			case 'M':
				part = (dt.getMonth() + 1);
				break;
			case 'yyyy':
				part = dt.getFullYear();
				break;
			case 'yy':
				part = ss.padLeftString((dt.getFullYear() % 100).toString(), 2, 0x30);
				break;
			case 'y':
				part = (dt.getFullYear() % 100);
				break;
			case 'h': case 'hh':
				part = dt.getHours() % 12;
				if (!part) {
					part = '12';
				}
				else if (fs == 'hh') {
					part = ss.padLeftString(part.toString(), 2, 0x30);
				}
				break;
			case 'HH':
				part = ss.padLeftString(dt.getHours().toString(), 2, 0x30);
				break;
			case 'H':
				part = dt.getHours();
				break;
			case 'mm':
				part = ss.padLeftString(dt.getMinutes().toString(), 2, 0x30);
				break;
			case 'm':
				part = dt.getMinutes();
				break;
			case 'ss':
				part = ss.padLeftString(dt.getSeconds().toString(), 2, 0x30);
				break;
			case 's':
				part = dt.getSeconds();
				break;
			case 't': case 'tt':
				part = (dt.getHours() < 12) ? dtf.amDesignator : dtf.pmDesignator;
				if (fs == 't') {
					part = part.charAt(0);
				}
				break;
			case 'fff':
				part = ss.padLeftString(dt.getMilliseconds().toString(), 3, 0x30);
				break;
			case 'ff':
				part = ss.padLeftString(dt.getMilliseconds().toString(), 3).substr(0, 2);
				break;
			case 'f':
				part = ss.padLeftString(dt.getMilliseconds().toString(), 3).charAt(0);
				break;
			case 'z':
				part = dt.getTimezoneOffset() / 60;
				part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part));
				break;
			case 'zz': case 'zzz':
				part = dt.getTimezoneOffset() / 60;
				part = ((part >= 0) ? '-' : '+') + Math.floor(ss.padLeftString(Math.abs(part)).toString(), 2, 0x30);
				if (fs == 'zzz') {
					part += dtf.timeSeparator + Math.abs(ss.padLeftString(dt.getTimezoneOffset() % 60).toString(), 2, 0x30);
				}
				break;
			default:
				if (part.charAt(0) == '\'') {
					part = part.substr(1, part.length - 2).replace(/\\'/g, '\'');
				}
				break;
		}
		sb.append(part);
	}

	return sb.toString();
};

ss._parseExactDate = function ss$_parseExactDate(val, format, culture, utc) {
	culture = culture || ss_CultureInfo.CurrentCulture;
	var AM = culture.amDesignator, PM = culture.pmDesignator;

	var _isInteger = function(val) {
		var digits="1234567890";
		for (var i=0; i < val.length; i++) {
			if (digits.indexOf(val.charAt(i))==-1) {
				return false;
			}
		}
		return true;
	};

	var _getInt = function(str,i,minlength,maxlength) {
		for (var x=maxlength; x>=minlength; x--) {
			var token=str.substring(i,i+x);
			if (token.length < minlength) {
				return null;
			}
			if (_isInteger(token)) {
				return token;
			}
		}
		return null;
	};

	val = val + "";
	format = format + "";
	var i_val = 0;
	var i_format = 0;
	var c = "";
	var token = "";

	var year = 0, month = 1, date = 1, hh = 0, mm = 0, _ss = 0, ampm = "";
		
	while (i_format < format.length) {
		// Get next token from format string
		c = format.charAt(i_format);
		token = "";
		while ((format.charAt(i_format) == c) && (i_format < format.length)) {
			token += format.charAt(i_format++);
		}
		// Extract contents of value based on format token
		if (token=="yyyy" || token=="yy" || token=="y") {
			if (token == "yyyy")
				year = _getInt(val, i_val, 4, 4);
			if (token == "yy")
				year = _getInt(val, i_val, 2, 2);
			if (token == "y")
				year = _getInt(val, i_val, 2, 4);

			if (year == null)
				return null;

			i_val += year.length;
			if (year.length == 2) {
				if (year > 30) {
					year = 1900 + (year-0);
				}
				else {
					year = 2000 + (year-0);
				}
			}
		}
		else if (token == "MM" || token == "M") {
			month = _getInt(val, i_val, token.length, 2);
			if (month == null || (month < 1) || (month > 12))
				return null;
			i_val += month.length;
		}
		else if (token=="dd"||token=="d") {
			date = _getInt(val, i_val, token.length, 2);
			if (date == null || (date < 1) || (date > 31))
				return null;
			i_val += date.length;
		}
		else if (token=="hh"||token=="h") {
			hh = _getInt(val, i_val, token.length, 2);
			if (hh == null || (hh < 1) || (hh > 12))
				return null;
			i_val += hh.length;
		}
		else if (token=="HH"||token=="H") {
			hh = _getInt(val, i_val, token.length, 2);
			if (hh == null || (hh < 0) || (hh > 23))
				return null;
			i_val += hh.length;
		}
		else if (token == "mm" || token == "m") {
			mm = _getInt(val, i_val, token.length, 2);
			if (mm == null || (mm < 0) || (mm > 59))
				return null;
			i_val += mm.length;
		}
		else if (token == "ss" || token == "s") {
			_ss = _getInt(val, i_val, token.length, 2);
			if (_ss == null || (_ss < 0) || (_ss > 59))
				return null;
			i_val += _ss.length;
		}
		else if (token == "t") {
			if (val.substring(i_val, i_val + 1).toLowerCase() == AM.charAt(0).toLowerCase())
				ampm = AM;
			else if (val.substring(i_val, i_val + 1).toLowerCase() == PM.charAt(0).toLowerCase())
				ampm = PM;
			else
				return null;
			i_val += 1;
		}
		else if (token == "tt") {
			if (val.substring(i_val, i_val + 2).toLowerCase() == AM.toLowerCase())
				ampm = AM;
			else if (val.substring(i_val,i_val+2).toLowerCase() == PM.toLowerCase())
				ampm = PM;
			else
				return null;
			i_val += 2;
		}
		else {
			if (val.substring(i_val, i_val + token.length) != token)
				return null;
			else
				i_val += token.length;
		}
	}
	// If there are any trailing characters left in the value, it doesn't match
	if (i_val != val.length)
		return null;

	// Is date valid for month?
	if (month == 2) {
		// Check for leap year
		if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) { // leap year
			if (date > 29)
				return null;
		}
		else if (date > 28)
			return null;
	}
	if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) {
		if (date > 30) {
			return null;
		}
	}
	// Correct hours value
	if (hh < 12 && ampm == PM) {
		hh = hh - 0 + 12;
	}
	else if (hh > 11 && ampm == AM) {
		hh -= 12;
	}

	if (utc)
		return new Date(Date.UTC(year, month - 1, date, hh, mm, _ss));
	else
		return new Date(year, month - 1, date, hh, mm, _ss);
};

ss.parseExactDate = function ss$parseExactDate(val, format, culture) {
	return ss._parseExactDate(val, format, culture, false);
};

ss.parseExactDateUTC = function ss$parseExactDateUTC(val, format, culture) {
	return ss._parseExactDate(val, format, culture, true);
};

///////////////////////////////////////////////////////////////////////////////
// Function Extensions

ss._delegateContains = function ss$_delegateContains(targets, object, method) {
	for (var i = 0; i < targets.length; i += 2) {
		if (targets[i] === object && targets[i + 1] === method) {
			return true;
		}
	}
	return false;
};

ss._mkdel = function ss$_mkdel(targets) {
	var delegate = function() {
		if (targets.length == 2) {
			return targets[1].apply(targets[0], arguments);
		}
		else {
			var clone = ss.arrayClone(targets);
			for (var i = 0; i < clone.length; i += 2) {
				if (ss._delegateContains(targets, clone[i], clone[i + 1])) {
					clone[i + 1].apply(clone[i], arguments);
				}
			}
			return null;
		}
	};
	delegate._targets = targets;

	return delegate;
};

ss.mkdel = function ss$mkdel(object, method) {
	if (!object) {
		return method;
	}
	return ss._mkdel([object, method]);
};

ss.delegateCombine = function ss$delegateCombine(delegate1, delegate2) {
	if (!delegate1) {
		if (!delegate2._targets) {
			return ss.mkdel(null, delegate2);
		}
		return delegate2;
	}
	if (!delegate2) {
		if (!delegate1._targets) {
			return ss.mkdel(null, delegate1);
		}
		return delegate1;
	}

	var targets1 = delegate1._targets ? delegate1._targets : [null, delegate1];
	var targets2 = delegate2._targets ? delegate2._targets : [null, delegate2];

	return ss._mkdel(targets1.concat(targets2));
};

ss.delegateRemove = function ss$delegateRemove(delegate1, delegate2) {
	if (!delegate1 || (delegate1 === delegate2)) {
		return null;
	}
	if (!delegate2) {
		return delegate1;
	}

	var targets = delegate1._targets;
	var object = null;
	var method;
	if (delegate2._targets) {
		object = delegate2._targets[0];
		method = delegate2._targets[1];
	}
	else {
		method = delegate2;
	}

	for (var i = 0; i < targets.length; i += 2) {
		if ((targets[i] === object) && (targets[i + 1] === method)) {
			if (targets.length == 2) {
				return null;
			}
			var t = ss.arrayClone(targets);
			t.splice(i, 2);
			return ss._mkdel(t);
		}
	}

	return delegate1;
};

ss.delegateEquals = function ss$delegateEquals(a, b) {
	if (a === b)
		return true;
	if (!a._targets && !b._targets)
		return false;
	var ta = a._targets || [null, a], tb = b._targets || [null, b];
	if (ta.length != tb.length)
		return false;
	for (var i = 0; i < ta.length; i++) {
		if (ta[i] !== tb[i])
			return false;
	}
	return true;
};

ss.delegateClone = function ss$delegateClone(source) {
	return source._targets ? ss._mkdel(source._targets) : function() { return source.apply(this, arguments); };
};

ss.thisFix = function ss$thisFix(source) {
	return function() {
		var x = [this];
		for(var i = 0; i < arguments.length; i++)
			x.push(arguments[i]);
		return source.apply(source, x);
	};
};

ss.getInvocationList = function ss$getInvocationList(delegate) {
	if (!delegate._targets)
		return [delegate];
	var result = [];
	for (var i = 0; i < delegate._targets.length; i += 2)
		result.push(ss.mkdel(delegate._targets[i], delegate._targets[i + 1]));
	return result;
};

///////////////////////////////////////////////////////////////////////////////
// Debug Extensions

ss.Debug = global.Debug || function() {};
ss.Debug.__typeName = 'Debug';

if (!ss.Debug.writeln) {
	ss.Debug.writeln = function Debug$writeln(text) {
		if (global.console) {
			if (global.console.debug) {
				global.console.debug(text);
				return;
			}
			else if (global.console.log) {
				global.console.log(text);
				return;
			}
		}
		else if (global.opera &&
			global.opera.postError) {
			global.opera.postError(text);
			return;
		}
	}
};

ss.Debug._fail = function Debug$_fail(message) {
	ss.Debug.writeln(message);
	eval('debugger;');
};

ss.Debug.assert = function Debug$assert(condition, message) {
	if (!condition) {
		message = 'Assert failed: ' + message;
		if (confirm(message + '\r\n\r\nBreak into debugger?')) {
			ss.Debug._fail(message);
		}
	}
};

ss.Debug.fail = function Debug$fail(message) {
	ss.Debug._fail(message);
};

///////////////////////////////////////////////////////////////////////////////
// Enum

var ss_Enum = function Enum$() {
};
ss.registerClass(global, 'ss.Enum', ss_Enum);

ss_Enum.parse = function Enum$parse(enumType, s) {
	var values = enumType.prototype;
	if (!ss.isFlags(enumType)) {
		for (var f in values) {
			if (f === s) {
				return values[f];
			}
		}
	}
	else {
		var parts = s.split('|');
		var value = 0;
		var parsed = true;

		for (var i = parts.length - 1; i >= 0; i--) {
			var part = parts[i].trim();
			var found = false;

			for (var f in values) {
				if (f === part) {
					value |= values[f];
					found = true;
					break;
				}
			}
			if (!found) {
				parsed = false;
				break;
			}
		}

		if (parsed) {
			return value;
		}
	}
	throw 'Invalid Enumeration Value';
};

ss_Enum.toString = function  Enum$toString(enumType, value) {
	var values = enumType.prototype;
	if (!ss.isFlags(enumType) || (value === 0)) {
		for (var i in values) {
			if (values[i] === value) {
				return i;
			}
		}
		throw 'Invalid Enumeration Value';
	}
	else {
		var parts = [];
		for (var i in values) {
			if (values[i] & value) {
				ss.add(parts, i);
			}
		}
		if (!parts.length) {
			throw 'Invalid Enumeration Value';
		}
		return parts.join(' | ');
	}
};

///////////////////////////////////////////////////////////////////////////////
// CultureInfo

var ss_CultureInfo = function CultureInfo$(name, numberFormat, dateFormat) {
	this.name = name;
	this.numberFormat = numberFormat;
	this.dateFormat = dateFormat;
};
ss.registerClass(global, 'ss.CultureInfo', ss_CultureInfo);

ss_CultureInfo.InvariantCulture = new ss_CultureInfo('en-US',
	{
		naNSymbol: 'NaN',
		negativeSign: '-',
		positiveSign: '+',
		negativeInfinityText: '-Infinity',
		positiveInfinityText: 'Infinity',
		
		percentSymbol: '%',
		percentGroupSizes: [3],
		percentDecimalDigits: 2,
		percentDecimalSeparator: '.',
		percentGroupSeparator: ',',
		percentPositivePattern: '{0} %',
		percentNegativePattern: '-{0} %',

		currencySymbol:'$',
		currencyGroupSizes: [3],
		currencyDecimalDigits: 2,
		currencyDecimalSeparator: '.',
		currencyGroupSeparator: ',',
		currencyNegativePattern: '(${0})',
		currencyPositivePattern: '${0}',

		numberGroupSizes: [3],
		numberDecimalDigits: 2,
		numberDecimalSeparator: '.',
		numberGroupSeparator: ','
	},
	{
		amDesignator: 'AM',
		pmDesignator: 'PM',

		dateSeparator: '/',
		timeSeparator: ':',

		gmtDateTimePattern: 'ddd, dd MMM yyyy HH:mm:ss \'GMT\'',
		universalDateTimePattern: 'yyyy-MM-dd HH:mm:ssZ',
		sortableDateTimePattern: 'yyyy-MM-ddTHH:mm:ss',
		dateTimePattern: 'dddd, MMMM dd, yyyy h:mm:ss tt',

		longDatePattern: 'dddd, MMMM dd, yyyy',
		shortDatePattern: 'M/d/yyyy',

		longTimePattern: 'h:mm:ss tt',
		shortTimePattern: 'h:mm tt',

		firstDayOfWeek: 0,
		dayNames: ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'],
		shortDayNames: ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'],
		minimizedDayNames: ['Su','Mo','Tu','We','Th','Fr','Sa'],

		monthNames: ['January','February','March','April','May','June','July','August','September','October','November','December',''],
		shortMonthNames: ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec','']
	});
ss_CultureInfo.CurrentCulture = ss_CultureInfo.InvariantCulture;

///////////////////////////////////////////////////////////////////////////////
// IEnumerator

var ss_IEnumerator = function IEnumerator$() { };
ss_IEnumerator.prototype = {
	current: null,
	moveNext: null,
	reset: null
};

ss.registerInterface(global, 'ss.IEnumerator', ss_IEnumerator, [ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// IEnumerable

var ss_IEnumerable = function IEnumerable$() { };
ss_IEnumerable.prototype = {
	getEnumerator: null
};

ss.registerInterface(global, 'ss.IEnumerable', ss_IEnumerable);

ss.getEnumerator = function ss$getEnumerator(obj) {
	return obj.getEnumerator ? obj.getEnumerator() : new ss_ArrayEnumerator(obj);
};

///////////////////////////////////////////////////////////////////////////////
// ICollection

var ss_ICollection = function ICollection$() { };
ss_ICollection.prototype = {
	get_count: null,
	add: null,
	clear: null,
	contains: null,
	remove: null
};

ss.registerInterface(global, 'ss.ICollection', ss_ICollection);

ss.count = function ss$count(obj) {
	return obj.get_count ? obj.get_count() : obj.length;
};

ss.add = function ss$add(obj, item) {
	if (obj.add)
		obj.add(item);
	else if (ss.isArray(obj))
		obj.push(item);
	else
		throw new ss_NotSupportedException();
};

ss.clear = function ss$clear(obj) {
	if (obj.clear)
		obj.clear();
	else if (ss.isArray(obj))
		obj.length = 0;
	else
		throw new ss_NotSupportedException();
};

ss.remove = function ss$remove(obj, item) {
	if (obj.remove)
		return obj.remove(item);
	else if (ss.isArray(obj)) {
		var index = ss.indexOf(obj, item);
		if (index >= 0) {
			obj.splice(index, 1);
			return true;
		}
		return false;
	}
	else
		throw new ss_NotSupportedException();
};

ss.contains = function ss$contains(obj, item) {
	if (obj.contains)
		return obj.contains(item);
	else
		return ss.indexOf(obj, item) >= 0;
};

///////////////////////////////////////////////////////////////////////////////
// TimeSpan

var ss_TimeSpan = function TimeSpan$(ticks) {
	this.ticks = ticks || 0;
};

ss_TimeSpan.fromValues = function TimeSpan$fromValues(days, hours, minutes, seconds, milliseconds) {
	return new ss_TimeSpan((days * 86400000 + hours * 3600000 + minutes * 60000 + seconds * 1000 + milliseconds) * 10000);
};

ss_TimeSpan.getDefaultValue = ss_TimeSpan.createInstance = function TimeSpan$default() {
	return new ss_TimeSpan(0);
};

ss_TimeSpan.prototype = {
	compareTo: function TimeSpan$compareTo(other) {
		return this.ticks < other.ticks ? -1 : (this.ticks > other.ticks ? 1 : 0);
	},
	equals: function TimeSpan$equals(other) {
		return ss.isInstanceOfType(other, ss_TimeSpan) && other.ticks === this.ticks;
	},
	equalsT: function TimeSpan$equalsT(other) {
		return this.equals(other);
	},
	toString: function TimeSpan$toString() {
		var d = function(s, n) { return ss.padLeftString(s + '', n || 2, 48); };

		var ticks = this.ticks;
		var result = '';
		if (Math.abs(ticks) >= 864000000000) {
			result += d((ticks / 864000000000) | 0) + '.';
			ticks %= 864000000000;
		}
		result += d(ticks / 36000000000 | 0) + ':';
		ticks %= 36000000000;
		result += d(ticks / 600000000 | 0) + ':';
		ticks %= 600000000;
		result += d(ticks / 10000000 | 0);
		ticks %= 10000000;
		if (ticks > 0)
			result += '.' + d(ticks, 7);
		return result;
	}
};

ss.registerClass(global, 'ss.TimeSpan', ss_TimeSpan, null, [ss_IComparable, ss_IEquatable]);
ss_TimeSpan.__class = false;

///////////////////////////////////////////////////////////////////////////////
// IEqualityComparer

var ss_IEqualityComparer = function IEqualityComparer$() { };
ss_IEqualityComparer.prototype = {
	areEqual: null,
	getObjectHashCode: null
};

ss.registerInterface(global, 'ss.IEqualityComparer', ss_IEqualityComparer);

///////////////////////////////////////////////////////////////////////////////
// IComparer

var ss_IComparer = function IComparer$() { };
ss_IComparer.prototype = {
	compare: null
};

ss.registerInterface(global, 'ss.IComparer', ss_IComparer);

///////////////////////////////////////////////////////////////////////////////
// Nullable

var ss_Nullable = function Nullable$() {
};

ss.registerClass(global, 'ss.Nullable', ss_Nullable);

ss_Nullable.unbox = function Nullable$unbox(instance) {
	if (!ss.isValue(instance))
		throw 'Instance is null';
	return instance;
};

ss_Nullable.eq = function Nullable$eq(a, b) {
	return !ss.isValue(a) ? !ss.isValue(b) : (a === b);
};

ss_Nullable.ne = function Nullable$eq(a, b) {
	return !ss.isValue(a) ? ss.isValue(b) : (a !== b);
};

ss_Nullable.le = function Nullable$le(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a <= b;
};

ss_Nullable.ge = function Nullable$ge(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a >= b;
};

ss_Nullable.lt = function Nullable$lt(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a < b;
};

ss_Nullable.gt = function Nullable$gt(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a > b;
};

ss_Nullable.sub = function Nullable$sub(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a - b : null;
};

ss_Nullable.add = function Nullable$add(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a + b : null;
};

ss_Nullable.mod = function Nullable$mod(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a % b : null;
};

ss_Nullable.div = function Nullable$divf(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a / b : null;
};

ss_Nullable.mul = function Nullable$mul(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a * b : null;
};

ss_Nullable.band = function Nullable$band(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a & b : null;
};

ss_Nullable.bor = function Nullable$bor(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a | b : null;
};

ss_Nullable.xor = function Nullable$xor(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a ^ b : null;
};

ss_Nullable.shl = function Nullable$shl(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a << b : null;
};

ss_Nullable.srs = function Nullable$srs(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a >> b : null;
};

ss_Nullable.sru = function Nullable$sru(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a >>> b : null;
};

ss_Nullable.and = function Nullable$and(a, b) {
	if (a === true && b === true)
		return true;
	else if (a === false || b === false)
		return false;
	else
		return null;
};

ss_Nullable.or = function Nullable$or(a, b) {
	if (a === true || b === true)
		return true;
	else if (a === false && b === false)
		return false;
	else
		return null;
};

ss_Nullable.not = function Nullable$not(a) {
	return ss.isValue(a) ? !a : null;
};

ss_Nullable.neg = function Nullable$neg(a) {
	return ss.isValue(a) ? -a : null;
};

ss_Nullable.pos = function Nullable$pos(a) {
	return ss.isValue(a) ? +a : null;
};

ss_Nullable.cpl = function Nullable$cpl(a) {
	return ss.isValue(a) ? ~a : null;
};

///////////////////////////////////////////////////////////////////////////////
// IList

var ss_IList = function IList$() { };
ss_IList.prototype = {
	get_item: null,
	set_item: null,
	indexOf: null,
	insert: null,
	removeAt: null
};

ss.registerInterface(global, 'ss.IList', ss_IList, [ss_ICollection, ss_IEnumerable]);

ss.getItem = function ss$getItem(obj, index) {
	return obj.get_item ? obj.get_item(index) : obj[index];
}

ss.setItem = function ss$setItem(obj, index, value) {
	obj.set_item ? obj.set_item(index, value) : (obj[index] = value);
}

ss.indexOf = function ss$indexOf(obj, item) {
	if (ss.isArrayOrTypedArray(obj)) {
		for (var i = 0; i < obj.length; i++) {
			if (ss.staticEquals(obj[i], item)) {
				return i;
			}
		}
		return -1;
	}
	else
		return obj.indexOf(item);
};

ss.insert = function ss$insert(obj, index, item) {
	if (obj.insert)
		obj.insert(index, item);
	else if (ss.isArray(obj))
		obj.splice(index, 0, item);
	else
		throw new ss_NotSupportedException();
};

ss.removeAt = function ss$removeAt(obj, index) {
	if (obj.removeAt)
		obj.removeAt(index);
	else if (ss.isArray(obj))
		obj.splice(index, 1);
	else
		throw new ss_NotSupportedException();
};

///////////////////////////////////////////////////////////////////////////////
// IDictionary

var ss_IDictionary = function IDictionary$() { };
ss_IDictionary.prototype = {
	get_item: null,
	set_item: null,
	get_keys: null,
	get_values: null,
	containsKey: null,
	add: null,
	remove: null,
	tryGetValue: null
};

ss.registerInterface(global, 'ss.IDictionary', ss_IDictionary, [ss_IEnumerable]);

///////////////////////////////////////////////////////////////////////////////
// Int32

var ss_Int32 = function Int32$() { };

ss.registerClass(global, 'ss.Int32', ss_Int32, Object, [ ss_IEquatable, ss_IComparable, ss_IFormattable ]);
ss_Int32.__class = false;

ss_Int32.isInstanceOfType = function Int32$isInstanceOfType(instance) {
	return typeof(instance) === 'number' && isFinite(instance) && Math.round(instance, 0) == instance;
};

ss_Int32.getDefaultValue = ss_Int32.createInstance = function Int32$getDefaultValue() {
	return 0;
};

ss_Int32.div = function Int32$div(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? (a / b) | 0 : null;
};

ss_Int32.trunc = function Int32$trunc(n) {
	return ss.isValue(n) ? n | 0 : null;
};

ss_Int32.tryParse = function Int32$tryParse(s, result, min, max) {
	result.$ = 0;
	if (!/^[+-]?[0-9]+$/.test(s))
		return 0;
	var n = parseInt(s);
	if (n < min || n > max)
		return false;
	result.$ = n;
	return true;
};

///////////////////////////////////////////////////////////////////////////////
// MutableDateTime

var ss_JsDate = function JsDate$() { };

ss.registerClass(global, 'ss.JsDate', ss_JsDate, Object, [ ss_IEquatable, ss_IComparable ]);

ss_JsDate.createInstance = function JsDate$createInstance() {
	return new Date();
};

ss_JsDate.isInstanceOfType = function JsDate$isInstanceOfType(instance) {
	return instance instanceof Date;
};

///////////////////////////////////////////////////////////////////////////////
// ArrayEnumerator

var ss_ArrayEnumerator = function ArrayEnumerator$(array) {
	this._array = array;
	this._index = -1;
};
ss_ArrayEnumerator.prototype = {
	moveNext: function ArrayEnumerator$moveNext() {
		this._index++;
		return (this._index < this._array.length);
	},
	reset: function ArrayEnumerator$reset() {
		this._index = -1;
	},
	current: function ArrayEnumerator$current() {
		if (this._index < 0 || this._index >= this._array.length)
			throw 'Invalid operation';
		return this._array[this._index];
	},
	dispose: function ArrayEnumerator$dispose() {
	}
};

ss.registerClass(global, 'ss.ArrayEnumerator', ss_ArrayEnumerator, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// ObjectEnumerator

var ss_ObjectEnumerator = function ObjectEnumerator$(o) {
	this._keys = Object.keys(o);
	this._index = -1;
	this._object = o;
};
ss_ObjectEnumerator.prototype = {
	moveNext: function ObjectEnumerator$moveNext() {
		this._index++;
		return (this._index < this._keys.length);
	},
	reset: function ObjectEnumerator$reset() {
		this._index = -1;
	},
	current: function ObjectEnumerator$current() {
		if (this._index < 0 || this._index >= this._keys.length)
			throw 'Invalid operation';
		var k = this._keys[this._index];
		return { key: k, value: this._object[k] };
	},
	dispose: function ObjectEnumerator$dispose() {
	}
};

ss.registerClass(global, 'ss.ObjectEnumerator', ss_ObjectEnumerator, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// EqualityComparer

var ss_EqualityComparer = function EqualityComparer$() {
};
ss_EqualityComparer.prototype.areEqual = function EqualityComparer$areEqual(x, y) {
	return ss.staticEquals(x, y);
};
ss_EqualityComparer.prototype.getObjectHashCode = function EqualityComparer$getObjectHashCode(obj) {
	return ss.isValue(obj) ? ss.getHashCode(obj) : 0;
};
ss.registerClass(global, 'ss.EqualityComparer', ss_EqualityComparer, null, [ss_IEqualityComparer]);
ss_EqualityComparer.def = new ss_EqualityComparer();

///////////////////////////////////////////////////////////////////////////////
// Comparer

var ss_Comparer = function Comparer$(f) {
	this.f = f;
};
ss_Comparer.prototype.compare = function Comparer$compare(x, y) {
	return this.f(x, y);
};
ss_Comparer.create = function Comparer$create(f) {
	return new ss_Comparer(f);
};
ss.registerClass(global, 'ss.Comparer', ss_Comparer, null, [ss_IComparer]);
ss_Comparer.def = new ss_Comparer(ss.compare);

///////////////////////////////////////////////////////////////////////////////
// Dictionary
var ss_$DictionaryCollection = function $DictionaryCollection$(dict, isKeys) {
	this._dict = dict;
	this._isKeys = isKeys;
};
ss_$DictionaryCollection.prototype = {
	get_count: function $DictionaryCollection$get_count() {
		return this._dict.get_count();
	},
	contains: function $DictionaryCollection$contains(v) {
		if (this._isKeys) {
			return this._dict.containsKey(v);
		}
		else {
			for (var e in this._dict.buckets) {
				if (this._dict.buckets.hasOwnProperty(e)) {
					var bucket = this._dict.buckets[e];
					for (var i = 0; i < bucket.length; i++) {
						if (this._dict.comparer.areEqual(bucket[i].value, v))
							return true;
					}
				}
			}
			return false;
		}
	},
	getEnumerator: function $DictionaryCollection$getEnumerator(v) {
		return this._dict._getEnumerator(this._isKeys ? function(e) { return e.key; } : function(e) { return e.value; });
	},
	add: function $DictionaryCollection$add(v) {
		throw 'Collection is read-only';
	},
	clear: function $DictionaryCollection$clear() {
		throw 'Collection is read-only';
	},
	remove: function $DictionaryCollection$remove() {
		throw 'Collection is read-only';
	}
};

var ss_Dictionary$2 = function Dictionary$2$(TKey, TValue) {
	var $type = function(o, cmp) {
		this.countField = 0;
		this.buckets = {};

		this.comparer = cmp || ss_EqualityComparer.def;

		if (ss.isInstanceOfType(o, ss_IDictionary)) {
			var e = ss.getEnumerator(o);
			try {
				while (e.moveNext()) {
					var c = e.current();
					this.add(c.key, c.value);
				}
			}
			finally {
				if (ss.isInstanceOfType(e, ss_IDisposable)) {
					ss.cast(e, ss_IDisposable).dispose();
				}
			}
		}
		else if (o) {
			var keys = Object.keys(o);
			for (var i = 0; i < keys.length; i++) {
				this.add(keys[i], o[keys[i]]);
			}
		}
	};

	$type.prototype = {
		_setOrAdd: function(key, value, add) {
			var hash = this.comparer.getObjectHashCode(key);
			var entry = { key: key, value: value };
			if (this.buckets.hasOwnProperty(hash)) {
				var array = this.buckets[hash];
				for (var i = 0; i < array.length; i++) {
					if (this.comparer.areEqual(array[i].key, key)) {
						if (add)
							throw 'Key ' + key + ' already exists.';
						array[i] = entry;
						return;
					}
				}
				array.push(entry);
			} else {
				this.buckets[hash] = [entry];
			}
			this.countField++;
		},

		add: function(key, value) {
			this._setOrAdd(key, value, true);
		},

		set_item: function(key, value) {
			this._setOrAdd(key, value, false);
		},

		_get: function(key) {
			var hash = this.comparer.getObjectHashCode(key);
			if (this.buckets.hasOwnProperty(hash)) {
				var array = this.buckets[hash];
				for (var i = 0; i < array.length; i++) {
					var entry = array[i];
					if (this.comparer.areEqual(entry.key, key))
						return entry.value !== undefined ? entry.value : null;
				}
			}
			return undefined;
		},

		get_item: function(key) {
			var v = this._get(key);
			if (v === undefined)
				throw 'Key ' + key + ' does not exist.';
			return v;
		},

		tryGetValue: function(key, value) {
			var v = this._get(key);
			if (v !== undefined) {
				value.$ = v;
				return true;
			}
			else {
				value.$ = ss.getDefaultValue(TValue);
				return false;
			}
		},

		containsKey: function(key) {
			var hash = this.comparer.getObjectHashCode(key);
			if (!this.buckets.hasOwnProperty(hash))
				return false;

			var array = this.buckets[hash];
			for (var i = 0; i < array.length; i++) {
				if (this.comparer.areEqual(array[i].key, key))
					return true;
			}
			return false;
		},

		clear: function() {
			this.countField = 0;
			this.buckets = {};
		},

		remove: function(key) {
			var hash = this.comparer.getObjectHashCode(key);
			if (!this.buckets.hasOwnProperty(hash))
				return false;

			var array = this.buckets[hash];
			for (var i = 0; i < array.length; i++) {
				if (this.comparer.areEqual(array[i].key, key)) {
					array.splice(i, 1);
					if (array.length == 0) delete this.buckets[hash];
					this.countField--;
					return true;
				}
			}
			return false;
		},

		get_count: function() {
			return this.countField;
		},

		_getEnumerator: function(projector) {
			var bucketKeys = Object.keys(this.buckets), bucketIndex = -1, arrayIndex;
			return new ss_IteratorBlockEnumerator(function() {
				if (bucketIndex < 0 || arrayIndex >= (this.buckets[bucketKeys[bucketIndex]].length - 1)) {
					arrayIndex = -1;
					bucketIndex++;
				}
				if (bucketIndex >= bucketKeys.length)
					return false;
				arrayIndex++;
				return true;
			}, function() { return projector(this.buckets[bucketKeys[bucketIndex]][arrayIndex]); }, null, this);
		},

		get_keys: function() {
			return new ss_$DictionaryCollection(this, true);
		},

		get_values: function() {
			return new ss_$DictionaryCollection(this, false);
		},

		getEnumerator: function() {
			return this._getEnumerator(function(e) { return e; });
		}
	};

	ss.registerGenericClassInstance($type, ss_Dictionary$2, [TKey, TValue], function() { return null; }, function() { return [ ss_IDictionary, ss_IEnumerable ]; });
	return $type;
};

ss.registerGenericClass(global, 'ss.Dictionary$2', ss_Dictionary$2, 2);
ss.registerClass(global, 'ss.$DictionaryCollection', ss_$DictionaryCollection, null, [ss_IEnumerable, ss_ICollection]);

///////////////////////////////////////////////////////////////////////////////
// IDisposable

var ss_IDisposable = function IDisposable$() { };
ss_IDisposable.prototype = {
	dispose: null
};
ss.registerInterface(global, 'ss.IDisposable', ss_IDisposable);

///////////////////////////////////////////////////////////////////////////////
// StringBuilder

var ss_StringBuilder = function StringBuilder$(s) {
	this._parts = ss.isNullOrUndefined(s) || s === '' ? [] : [s];
	this.isEmpty = this._parts.length == 0;
}
ss_StringBuilder.prototype = {
	append: function StringBuilder$append(s) {
		if (!ss.isNullOrUndefined(s) && s !== '') {
			ss.add(this._parts, s);
			this.isEmpty = false;
		}
		return this;
	},

	appendChar: function StringBuilder$appendChar(c) {
		return this.append(String.fromCharCode(c));
	},

	appendLine: function StringBuilder$appendLine(s) {
		this.append(s);
		this.append('\r\n');
		this.isEmpty = false;
		return this;
	},

	appendLineChar: function StringBuilder$appendLineChar(c) {
		return this.appendLine(String.fromCharCode(c));
	},

	clear: function StringBuilder$clear() {
		this._parts = [];
		this.isEmpty = true;
	},

	toString: function StringBuilder$toString() {
		return this._parts.join('');
	}
};

ss.registerClass(global, 'ss.StringBuilder', ss_StringBuilder);

///////////////////////////////////////////////////////////////////////////////
// Random

var ss_Random = function Random$(seed) {
	var _seed = (seed === undefined) ? parseInt(Date.now() % 2147483648) : parseInt(Math.abs(seed));
	this.inext = 0;
	this.inextp = 21;
	this.seedArray = new Array(56);
	for(var i = 0; i < 56; i++)
		this.seedArray[i] = 0;

	_seed = 161803398 - _seed;
	if (_seed < 0)
		_seed += 2147483648;
	this.seedArray[55] = _seed;
	var mk = 1;
	for (var i = 1; i < 55; i++) {
		var ii = (21 * i) % 55;
		this.seedArray[ii] = mk;
		mk = _seed - mk;
		if (mk < 0)
			mk += 2147483648;

		_seed = this.seedArray[ii];
	}
	for (var j = 1; j < 5; j++) {
		for (var k = 1; k < 56; k++) {
			this.seedArray[k] -= this.seedArray[1 + (k + 30) % 55];
			if (this.seedArray[k] < 0)
				this.seedArray[k] += 2147483648;
		}
	}
};

ss_Random.prototype = {
	next: function Random$next() {
		return this.sample() * 2147483648 | 0;
	},
	nextMax: function Random$nextMax(max) {
		return this.sample() * max | 0;
	},
	nextMinMax: function Random$nextMinMax(min, max) {
		return (this.sample() * (max - min) + min) | 0;
	},
	nextBytes: function Random$nextBytes(bytes) {
		for (var i = 0; i < bytes.length; i++)
			bytes[i] = (this.sample() * 256) | 0;
	},
	nextDouble: function Random$nextDouble() {
		return this.sample();
	},
	sample: function Random$sample() {
		if (++this.inext >= 56)
			this.inext = 1;
		if (++this.inextp >= 56)
			this.inextp = 1;

		var retVal =  this.seedArray[this.inext] - this.seedArray[this.inextp];

		if (retVal < 0)
			retVal += 2147483648;

		this.seedArray[this.inext] = retVal;

		return retVal * (1.0 / 2147483648);
	}
};

ss.registerClass(global, 'ss.Random', ss_Random);

///////////////////////////////////////////////////////////////////////////////
// EventArgs

var ss_EventArgs = function EventArgs$() {
}
ss.registerClass(global, 'ss.EventArgs', ss_EventArgs);

ss_EventArgs.Empty = new ss_EventArgs();

///////////////////////////////////////////////////////////////////////////////
// Exception

var ss_Exception = function Exception$(message, innerException) {
	this._message = message || null;
	this._innerException = innerException || null;
}
ss.registerClass(global, 'ss.Exception', ss_Exception);

ss_Exception.prototype = {
	get_message: function Exception$get_message() {
		return this._message;
	},
	get_innerException: function Exception$get_innerException() {
		return this._innerException;
	}
};

ss_Exception.wrap = function Exception$wrap(o) {
	if (ss.isInstanceOfType(o, ss_Exception)) {
		return o;
	}
	else if (o instanceof Error) {
		return new ss_JsErrorException(o);
	}
	else {
		return new ss_Exception(o.toString());
	}
};

////////////////////////////////////////////////////////////////////////////////
// NotSupportedException

var ss_NotSupportedException = function NotSupportedException$(message, innerException) {
	ss_Exception.call(this, message, innerException);
};
ss.registerClass(global, 'ss.NotSupportedException', ss_NotSupportedException, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// AggregateException

var ss_AggregateException = function AggregateException$(message, innerExceptions) {
	if (typeof(message) !== 'string') {
		innerExceptions = message;
		message = 'One or more errors occurred.';
	}
	innerExceptions = ss.isValue(innerExceptions) ? ss.arrayFromEnumerable(innerExceptions) : null;

	ss_Exception.call(this, message, innerExceptions && innerExceptions.length ? innerExceptions[0] : null);
	this._innerExceptions = innerExceptions;
};
ss_AggregateException.prototype = {
	get_innerExceptions: function AggregateException$get_innerExceptions() {
		return this._innerExceptions;
	}
};
ss.registerClass(global, 'ss.AggregateException', ss_AggregateException, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// PromiseException

var ss_PromiseException = function PromiseException(args) {
	ss_Exception.call(this, args[0] ? args[0].toString() : 'An error occurred');
	this._arguments = ss.arrayClone(args);
};
ss_PromiseException.prototype = {
	get_arguments: function PromiseException$get_arguments() {
		return this._arguments;
	}
};
ss.registerClass(global, 'ss.PromiseException', ss_PromiseException, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// JsErrorException

var ss_JsErrorException = function JsErrorException$(error) {
	ss_Exception.call(this, error.message);
	this._error = error;
};
ss_JsErrorException.prototype = {
	get_error: function JsErrorException$get_error() {
		return this._error;
	}
};
ss.registerClass(global, 'ss.JsErrorException', ss_JsErrorException, ss_Exception);

///////////////////////////////////////////////////////////////////////////////
// IteratorBlockEnumerable

var ss_IteratorBlockEnumerable = function IteratorBlockEnumerable$(getEnumerator, $this) {
	this._getEnumerator = getEnumerator;
	this._this = $this;
};

ss_IteratorBlockEnumerable.prototype = {
	getEnumerator: function IteratorBlockEnumerable$getEnumerator() {
		return this._getEnumerator.call(this._this);
	}
};

ss.registerClass(global, 'ss.IteratorBlockEnumerable', ss_IteratorBlockEnumerable, null, [ss_IEnumerable]);

///////////////////////////////////////////////////////////////////////////////
// IteratorBlockEnumerator

var ss_IteratorBlockEnumerator = function IteratorBlockEnumerator$(moveNext, getCurrent, dispose, $this) {
	this._moveNext = moveNext;
	this._getCurrent = getCurrent;
	this._dispose = dispose;
	this._this = $this;
};

ss_IteratorBlockEnumerator.prototype = {
	moveNext: function IteratorBlockEnumerator$moveNext() {
		try {
			return this._moveNext.call(this._this);
		}
		catch (ex) {
			if (this._dispose)
				this._dispose.call(this._this);
			throw ex;
		}
	},
	current: function IteratorBlockEnumerator$current() {
		return this._getCurrent.call(this._this);
	},
	reset: function IteratorBlockEnumerator$reset() {
		throw new ss_NotSupportedException('Reset is not supported.');
	},
	dispose: function IteratorBlockEnumerator$dispose() {
		if (this._dispose)
			this._dispose.call(this._this);
	}
};

ss.registerClass(global, 'ss.IteratorBlockEnumerator', ss_IteratorBlockEnumerator, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// Lazy

var ss_Lazy = function Lazy$(valueFactory) {
	this._valueFactory = valueFactory;
	this.isValueCreated = false;
};
ss_Lazy.prototype.value = function Lazy$value() {
	if (!this.isValueCreated) {
		this._value = this._valueFactory();
		delete this._valueFactory;
		this.isValueCreated = true;
	}
	return this._value;
};
ss.registerClass(global, 'ss.Lazy', ss_Lazy);


///////////////////////////////////////////////////////////////////////////////
// Task

var ss_Task = function Task$(action, state) {
	this._action = action;
	this._state = state;
	this.exception = null;
	this.status = 0;
	this._thens = [];
	this._result = null;
};

ss_Task.prototype = {
	continueWith: function Task$continueWith(continuation) {
		var tcs = new ss_TaskCompletionSource();
		var _this = this;
		var fn = function() {
			try {
				tcs.setResult(continuation(_this));
			}
			catch (e) {
				tcs.setException(ss_Exception.wrap(e));
			}
		};
		if (this.isCompleted()) {
			setTimeout(fn, 0);
		}
		else {
			this._thens.push(fn);
		}
		return tcs.task;
	},
	start: function Task$start() {
		if (this.status !== 0)
			throw 'Task was already started.';
		var _this = this;
		this.status = 3;
		setTimeout(function() {
			try {
				var result = _this._action(_this._state);
				delete _this._action;
				delete _this._state;
				_this._complete(result);
			}
			catch (e) {
				_this._fail(new ss_AggregateException([ss_Exception.wrap(e)]));
			}
		}, 0);
	},
	_runCallbacks: function Task$_runCallbacks() {
		for (var i = 0; i < this._thens.length; i++)
			this._thens[i](this);
		delete this._thens;
	},
	_complete: function Task$_complete(result) {
		if (this.isCompleted())
			return false;
		this._result = result;
		this.status = 5;
		this._runCallbacks();
		return true;
	},
	_fail: function Task$_fail(exception) {
		if (this.isCompleted())
			return false;
		this.exception = exception;
		this.status = 7;
		this._runCallbacks();
		return true;
	},
	_cancel: function Task$_cancel() {
		if (this.isCompleted())
			return false;
		this.status = 6;
		this._runCallbacks();
		return true;
	},
	isCanceled: function Task$isCanceled() {
		return this.status === 6;
	},
	isCompleted: function Task$isCompleted() {
		return this.status >= 5;
	},
	isFaulted: function Task$isFaulted() {
		return this.status === 7;
	},
	getResult: function Task$getResult() {
		switch (this.status) {
			case 5:
				return this._result;
			case 6:
				throw 'Task was cancelled.';
			case 7:
				throw this.exception;
			default:
				throw 'Task is not yet completed.';
		}
	},
	dispose: function Task$dispose() {
	}
};

ss_Task.delay = function Task$delay(delay) {
	var tcs = new ss_TaskCompletionSource();
	setTimeout(function() {
		tcs.setResult(0);
	}, delay);
	return tcs.task;
};

ss_Task.fromResult = function Task$fromResult(result) {
	var t = new ss_Task();
	t.status = 5;
	t._result = result;
	return t;
};

ss_Task.run = function Task$fromResult(f) {
	var tcs = new ss_TaskCompletionSource();
	setTimeout(function() {
		try {
			tcs.setResult(f());
		}
		catch (e) {
			tcs.setException(ss_Exception.wrap(e));
		}
	}, 0);
	return tcs.task;
};

ss_Task.whenAll = function Task$whenAll(tasks) {
	var tcs = new ss_TaskCompletionSource();
	if (tasks.length === 0) {
		tcs.setResult([]);
	}
	else {
		var result = new Array(tasks.length), remaining = tasks.length, cancelled = false, exceptions = [];
		for (var i = 0; i < tasks.length; i++) {
			(function(i) {
				tasks[i].continueWith(function(t) {
					switch (t.status) {
						case 5:
							result[i] = t.getResult();
							break;
						case 6:
							cancelled = true;
							break;
						case 7:
							ss.arrayAddRange(exceptions, t.exception.get_innerExceptions());
							break;
						default:
							throw 'Invalid task status ' + t.status;
					}
					if (--remaining === 0) {
						if (exceptions.length > 0)
							tcs.setException(exceptions);
						else if (cancelled)
							tcs.setCanceled();
						else
							tcs.setResult(result);
					}
				});
			})(i);
		}
	}
	return tcs.task;
};

ss_Task.whenAny = function Task$whenAny(tasks) {
	if (!tasks.length)
		throw 'Must wait for at least one task';

	var tcs = new ss_TaskCompletionSource();
	for (var i = 0; i < tasks.length; i++) {
		tasks[i].continueWith(function(t) {
			switch (t.status) {
				case 5:
					tcs.trySetResult(t);
					break;
				case 6:
					tcs.trySetCanceled();
					break;
				case 7:
					tcs.trySetException(t.exception.get_innerExceptions());
					break;
				default:
					throw 'Invalid task status ' + t.status;
			}
		});
	}
	return tcs.task;
};

ss_Task.fromDoneCallback = function Task$fromDoneCallback(t, i, m) {
	var tcs = new ss_TaskCompletionSource(), args;
    if (typeof(i) === 'number') {
        args = Array.prototype.slice.call(arguments, 3);
        if (i < 0)
            i += args.length + 1;
    }
    else {
        args = Array.prototype.slice.call(arguments, 2);
        m = i;
        i = args.length;
    }

	var cb = function(v) {
		tcs.setResult(v);
	};
	
    args = args.slice(0, i).concat(cb, args.slice(i));

	t[m].apply(t, args);
	return tcs.task;
};

ss_Task.fromPromise = function Task$fromPromise(p, f) {
	var tcs = new ss_TaskCompletionSource();
	if (typeof(f) === 'number')
		f = (function(i) { return function() { return arguments[i >= 0 ? i : (arguments.length + i)]; }; })(f);
    else if (typeof(f) !== 'function')
        f = function() { return Array.prototype.slice.call(arguments, 0); };

	p.then(function() {
		tcs.setResult(typeof(f) === 'function' ? f.apply(null, arguments) : null);
	}, function() {
		tcs.setException(new ss_PromiseException(Array.prototype.slice.call(arguments, 0)));
	});
	return tcs.task;
};

ss_Task.fromNode = function  Task$fromNode(t, f, m) {
	var tcs = new ss_TaskCompletionSource(), args;
    if (typeof(f) === 'function') {
        args = Array.prototype.slice.call(arguments, 3);
    }
    else {
        args = Array.prototype.slice.call(arguments, 2);
        m = f;
		f = function() { return arguments[0]; };
    }

	var cb = function(e) {
		if (e)
			tcs.setException(ss_Exception.wrap(e));
		else
			tcs.setResult(f.apply(null, Array.prototype.slice.call(arguments, 1)));
	};
	
	args.push(cb);

	t[m].apply(t, args);
	return tcs.task;
};

ss.registerClass(global, 'ss.Task', ss_Task, null, [ss_IDisposable]);

////////////////////////////////////////////////////////////////////////////////
// TaskStatus
var ss_TaskStatus = function() {
};
ss_TaskStatus.prototype = { created: 0, running: 3, ranToCompletion: 5, canceled: 6, faulted: 7 };
ss.registerEnum(global, 'ss.TaskStatus', ss_TaskStatus, false);

///////////////////////////////////////////////////////////////////////////////
// TaskCompletionSource

var ss_TaskCompletionSource = function TaskCompletionSource$() {
	this.task = new ss_Task();
	this.task.status = 3;
};
ss_TaskCompletionSource.prototype = {
	setCanceled: function TaskCompletionSource$setCanceled() {
		if (!this.task._cancel())
			throw 'Task was already completed.';
	},
	setResult: function TaskCompletionSource$setResult(result) {
		if (!this.task._complete(result))
			throw 'Task was already completed.';
	},
	setException: function TaskCompletionSource$setException(exception) {
		if (!this.trySetException(exception))
			throw 'Task was already completed.';
	},
	trySetCanceled: function TaskCompletionSource$trySetCanceled() {
		return this.task._cancel();
	},
	trySetResult: function TaskCompletionSource$setResult(result) {
		return this.task._complete(result);
	},
	trySetException: function TaskCompletionSource$setException(exception) {
		if (!ss.isInstanceOfType(exception, ss_AggregateException)) {
			if (ss.isInstanceOfType(exception, ss_Exception))
				exception = [exception];
			exception = new ss_AggregateException(exception);
		}
		return this.task._fail(exception);
	}
};

ss.registerClass(global, 'ss.TaskCompletionSource', ss_TaskCompletionSource);

///////////////////////////////////////////////////////////////////////////////
// CancelEventArgs

var ss_CancelEventArgs = function CancelEventArgs$() {
	ss_CancelEventArgs.call(this);
	this.cancel = false;
}
ss.registerClass(global, 'ss.CancelEventArgs', ss_CancelEventArgs, ss_EventArgs);

if (global.ss) {
	for (var n in ss) {
		if (ss.hasOwnProperty(n))
			global.ss[n] = ss[n];
	}
}
else {
	global.ss = ss;
}

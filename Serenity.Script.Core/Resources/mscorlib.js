// SaltarelleCompiler Runtime (http://www.saltarelle-compiler.com)
// Modified version of Script# Core Runtime (http://projects.nikhilk.net/ScriptSharp)

if (typeof(global) === "undefined") {
	if (typeof(window) !== "undefined")
		global = window;
	else if (typeof(self) !== "undefined")
		global = self;
}
(function(global) {
"use strict";

var ss = { __assemblies: {} };

ss.initAssembly = function assembly(obj, name, res) {
	res = res || {};
	obj.name = name;
	obj.toString = function() { return this.name; };
	obj.__types = {};
	obj.getResourceNames = function() { return Object.keys(res); };
	obj.getResourceDataBase64 = function(name) { return res[name] || null; };
	obj.getResourceData = function(name) { var r = res[name]; return r ? ss.dec64(r) : null; };
	ss.__assemblies[name] = obj;
};
ss.initAssembly(ss, 'mscorlib');

ss.load = function ss$load(name) {
	return ss.__assemblies[name] || require(name);
};

var enc = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/', dec;
ss.enc64 = function(a, b) {
	var s = '', i;
	for (i = 0; i < a.length; i += 3) {
		var c1 = a[i], c2 = a[i+1], c3 = a[i+2];
		s += (b && i && !(i%57) ? '\n' : '') + enc[c1 >> 2] + enc[((c1 & 3) << 4) | (c2 >> 4)] + (i < a.length - 1 ? enc[((c2 & 15) << 2) | (c3 >> 6)] : '=') + (i < a.length - 2 ? enc[c3 & 63] : '=');
	}
	return s;
};

ss.dec64 = function(s) {
	s = s.replace(/\s/g, '');
	dec = dec || (function() { var o = {'=':-1}; for (var i = 0; i < 64; i++) o[enc[i]] = i; return o; })();
	var a = Array(Math.max(s.length * 3 / 4 - 2, 0)), i;
	for (i = 0; i < s.length; i += 4) {
		var j = i * 3 / 4, c1 = dec[s[i]], c2 = dec[s[i+1]], c3 = dec[s[i+2]], c4 = dec[s[i+3]];
		a[j] = (c1 << 2) | (c2 >> 4);
		if (c3 >= 0) a[j+1] = ((c2 & 15) << 4) | (c3 >> 2);
		if (c4 >= 0) a[j+2] = ((c3 & 3) << 6) | c4;
	}
	return a;
};

ss.getAssemblies = function ss$getAssemblies() {
	return Object.keys(ss.__assemblies).map(function(n) { return ss.__assemblies[n]; });
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

ss.clone = function ss$clone(t, o) {
	return o ? t.$clone(o) : o;
}

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
		throw new ss_NullReferenceException('Cannot get hash code of null');
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
		throw new ss_NullReferenceException('Object is null');
	else if (a !== ss && typeof(a.equals) === 'function')
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
		throw new ss_NullReferenceException('Object is null');
	else if (typeof(a) === 'number' || typeof(a) === 'string' || typeof(a) === 'boolean')
		return a < b ? -1 : (a > b ? 1 : 0);
	else if (ss.isDate(a))
		return ss.compare(a.valueOf(), b.valueOf());
	else
		return a.compareTo(b);
};

ss.equalsT = function ss$equalsT(a, b) {
	if (!ss.isValue(a))
		throw new ss_NullReferenceException('Object is null');
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
	var keys = Object.keys(source);
	for (var i = 0, l = keys.length; i < l; i++) {
		var k = keys[i];
		target[k] = source[k];
	}
};

ss.isLower = function ss$isLower(c) {
	var s = String.fromCharCode(c);
	return s === s.toLowerCase() && s !== s.toUpperCase();
};

ss.isUpper = function ss$isUpper(c) {
	var s = String.fromCharCode(c);
	return s !== s.toLowerCase() && s === s.toUpperCase();
};

if (typeof(window) == 'object') {
	// Browser-specific stuff that could go into the Web assembly, but that assembly does not have an associated JS file.
	if (!window.Element) {
		// IE does not have an Element constructor. This implementation should make casting to elements work.
		window.Element = function() {};
		window.Element.isInstanceOfType = function(instance) { return instance && typeof instance.constructor === 'undefined' && typeof instance.tagName === 'string'; };
	}
	window.Element.__typeName = 'Element';
	
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
	Object.keys = (function() {
		'use strict';
		var hasOwnProperty = Object.prototype.hasOwnProperty,
			hasDontEnumBug = !({toString: null}).propertyIsEnumerable('toString'),
			dontEnums = ['toString','toLocaleString','valueOf','hasOwnProperty','isPrototypeOf','propertyIsEnumerable','constructor'],
			dontEnumsLength = dontEnums.length;

		return function (obj) {
			if (typeof obj !== 'object' && (typeof obj !== 'function' || obj === null)) {
				throw new TypeError('Object.keys called on non-object');
			}

			var result = [], prop, i;

			for (prop in obj) {
				if (hasOwnProperty.call(obj, prop)) {
					result.push(prop);
				}
			}

			if (hasDontEnumBug) {
				for (i = 0; i < dontEnumsLength; i++) {
					if (hasOwnProperty.call(obj, dontEnums[i])) {
						result.push(dontEnums[i]);
					}
				}
			}
			return result;
		};
	}());
}

ss.getKeyCount = function ss$getKeyCount(d) {
	return Object.keys(d).length;
};

///////////////////////////////////////////////////////////////////////////////
// Type System Implementation

ss.__genericCache = {};

ss._makeGenericTypeName = function ss$_makeGenericTypeName(genericType, typeArguments) {
	var result = genericType.__typeName;
	for (var i = 0; i < typeArguments.length; i++)
		result += (i === 0 ? '[' : ',') + '[' + ss.getTypeQName(typeArguments[i]) + ']';
	result += ']';
	return result;
};

ss.makeGenericType = function ss$makeGenericType(genericType, typeArguments) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	return ss.__genericCache[name] || genericType.apply(null, typeArguments);
};

ss.registerGenericClassInstance = function ss$registerGenericClassInstance(instance, genericType, typeArguments, members, baseType, interfaceTypes) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	ss.__genericCache[name] = instance;
	instance.__typeName = name;
	instance.__genericTypeDefinition = genericType;
	instance.__typeArguments = typeArguments;
	ss.initClass(instance, genericType.__assembly, members, baseType(), interfaceTypes());
};

ss.registerGenericInterfaceInstance = function ss$registerGenericInterfaceInstance(instance, genericType, typeArguments, members, baseInterfaces) {
	var name = ss._makeGenericTypeName(genericType, typeArguments);
	ss.__genericCache[name] = instance;
	instance.__typeName = name;
	instance.__genericTypeDefinition = genericType;
	instance.__typeArguments = typeArguments;
	ss.initInterface(instance, genericType.__assembly, members, baseInterfaces());
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

ss.setMetadata = function ss$_setMetadata(type, metadata) {
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
	if (metadata.variance) {
		type.isAssignableFrom = function(source) {
			var check = function(target, type) {
				if (type.__genericTypeDefinition === target.__genericTypeDefinition && type.__typeArguments.length == target.__typeArguments.length) {
					for (var i = 0; i < target.__typeArguments.length; i++) {
						var v = target.__metadata.variance[i], t = target.__typeArguments[i], s = type.__typeArguments[i];
						switch (v) {
							case 1: if (!ss.isAssignableFrom(t, s)) return false; break;
							case 2: if (!ss.isAssignableFrom(s, t)) return false; break;
							default: if (s !== t) return false;
						}
					}
					return true;
				}
				return false;
			};

			if (source.__interface && check(this, source))
				return true;
			var ifs = ss.getInterfaces(source);
			for (var i = 0; i < ifs.length; i++) {
				if (ifs[i] === this || check(this, ifs[i]))
					return true;
			}
			return false;
		};
	}
}

ss.initClass = function ss$initClass(ctor, asm, members, baseType, interfaces) {
	ctor.__class = true;
	ctor.__assembly = asm;
	if (!ctor.__typeArguments)
		asm.__types[ctor.__typeName] = ctor;
	if (baseType && baseType !== Object) {
		var f = function(){};
		f.prototype = baseType.prototype;
		ctor.prototype = new f();
		ctor.prototype.constructor = ctor;
	}
	ss.shallowCopy(members, ctor.prototype);
	if (interfaces)
		ctor.__interfaces = interfaces;
};

ss.initGenericClass = function ss$initGenericClass(ctor, asm, typeArgumentCount) {
	ctor.__class = true;
	ctor.__assembly = asm;
	asm.__types[ctor.__typeName] = ctor;
	ctor.__typeArgumentCount = typeArgumentCount;
	ctor.__isGenericTypeDefinition = true;
};

ss.initInterface = function ss$initInterface(ctor, asm, members, baseInterfaces) {
	ctor.__interface = true;
	ctor.__assembly = asm;
	if (!ctor.__typeArguments)
		asm.__types[ctor.__typeName] = ctor;
	if (baseInterfaces)
		ctor.__interfaces = baseInterfaces;
	ss.shallowCopy(members, ctor.prototype);
	ctor.isAssignableFrom = function(type) { return ss.contains(ss.getInterfaces(type), this); };
};

ss.initGenericInterface = function ss$initGenericClass(ctor, asm, typeArgumentCount) {
	ctor.__interface = true;
	ctor.__assembly = asm;
	asm.__types[ctor.__typeName] = ctor;
	ctor.__typeArgumentCount = typeArgumentCount;
	ctor.__isGenericTypeDefinition = true;
};

ss.initEnum = function ss$initEnum(ctor, asm, members, namedValues) {
	ctor.__enum = true;
	ctor.__assembly = asm;
	asm.__types[ctor.__typeName] = ctor;
	ss.shallowCopy(members, ctor.prototype);
	ctor.getDefaultValue = ctor.createInstance = function() { return namedValues ? null : 0; };
	ctor.isInstanceOfType = function(instance) { return typeof(instance) == (namedValues ? 'string' : 'number'); };
};

ss.getBaseType = function ss$getBaseType(type) {
	if (type === Object || type.__interface) {
		return null;
	}
	else if (Object.getPrototypeOf) {
		return Object.getPrototypeOf(type.prototype).constructor;
	}
	else {
		var p = type.prototype;
		if (Object.prototype.hasOwnProperty.call(p, 'constructor')) {
			try {
				var ownValue = p.constructor;
				delete p.constructor;
				return p.constructor;
			}
			finally {
				p.constructor = ownValue;
			}
		}
		return p.constructor;
	}
};

ss.getTypeFullName = function ss$getTypeFullName(type) {
	return type.__typeName || type.name || (type.toString().match(/^\s*function\s*([^\s(]+)/) || [])[1] || 'Object';
};

ss.getTypeQName = function ss$getTypeFullName(type) {
	return ss.getTypeFullName(type) + (type.__assembly ? ', ' + type.__assembly.name : '');
};

ss.getTypeName = function ss$getTypeName(type) {
	var fullName = ss.getTypeFullName(type);
	var bIndex = fullName.indexOf('[');
	var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
	return nsIndex > 0 ? fullName.substr(nsIndex + 1) : fullName;
};

ss.getTypeNamespace = function ss$getTypeNamespace(type) {
	var fullName = ss.getTypeFullName(type);
	var bIndex = fullName.indexOf('[');
	var nsIndex = fullName.lastIndexOf('.', bIndex >= 0 ? bIndex : fullName.length);
	return nsIndex > 0 ? fullName.substr(0, nsIndex) : '';
};

ss.getTypeAssembly = function ss$getTypeAssembly(type) {
	if (ss.contains([Date, Number, Boolean, String, Function, Array], type))
		return ss;
	else
		return type.__assembly || null;
};

ss._getAssemblyType = function ss$_getAssemblyType(asm, name) {
	var result = [];
	if (asm.__types) {
		return asm.__types[name] || null;
	}
	else {
		var a = name.split('.');
		for (var i = 0; i < a.length; i++) {
			asm = asm[a[i]];
			if (!ss.isValue(asm))
				return null;
		}
		if (typeof asm !== 'function')
			return null;
		return asm;
	}
};

ss.getAssemblyTypes = function ss$getAssemblyTypes(asm) {
	var result = [];
	if (asm.__types) {
		for (var t in asm.__types) {
			if (asm.__types.hasOwnProperty(t))
				result.push(asm.__types[t]);
		}
	}
	else {
		var traverse = function(s, n) {
			for (var c in s) {
				if (s.hasOwnProperty(c))
					traverse(s[c], c);
			}
			if (typeof(s) === 'function' && ss.isUpper(n.charCodeAt(0)))
				result.push(s);
		};
		traverse(asm, '');
	}
	return result;
};

ss.createAssemblyInstance = function ss$createAssemblyInstance(asm, typeName) {
	var t = ss.getType(typeName, asm);
	return t ? ss.createInstance(t) : null;
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

	return ss.isAssignableFrom(type, ss.getInstanceType(instance));
};

ss.isAssignableFrom = function ss$isAssignableFrom(target, type) {
	return target === type || (typeof(target.isAssignableFrom) === 'function' && target.isAssignableFrom(type)) || type.prototype instanceof target;
};

ss.isClass = function Type$isClass(type) {
	return (type.__class == true || type === Array || type === Function || type === RegExp || type === String || type === Error || type === Object);
};

ss.isEnum = function Type$isEnum(type) {
	return !!type.__enum;
};

ss.isFlags = function Type$isFlags(type) {
	return type.__metadata && type.__metadata.enumFlags || false;
};

ss.isInterface = function Type$isInterface(type) {
	return !!type.__interface;
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
	if (instance === null || typeof(instance) === 'undefined')
		return instance;
	else if (type === true || (type !== false && ss.isInstanceOfType(instance, type)))
		return instance;
	throw new ss_InvalidCastException('Cannot cast object to type ' + ss.getTypeFullName(type));
};

ss.getInstanceType = function ss$getInstanceType(instance) {
	if (!ss.isValue(instance))
		throw new ss_NullReferenceException('Cannot get type of null');

	// NOTE: We have to catch exceptions because the constructor
	//       cannot be looked up on native COM objects
	try {
		return instance.constructor;
	}
	catch (ex) {
		return Object;
	}
};

ss._getType = function (typeName, asm, re) {
	var outer = !re;
	re = re || /[[,\]]/g;
	var last = re.lastIndex, m = re.exec(typeName), tname, targs = [];
	if (m) {
		tname = typeName.substring(last, m.index);
		switch (m[0]) {
			case '[':
				if (typeName[m.index + 1] != '[')
					return null;
				for (;;) {
					re.exec(typeName);
					var t = ss._getType(typeName, global, re);
					if (!t)
						return null;
					targs.push(t);
					m = re.exec(typeName);
					if (m[0] === ']')
						break;
					else if (m[0] !== ',')
						return null;
				}
				m = re.exec(typeName);
				if (m && m[0] === ',') {
					re.exec(typeName);
					if (!(asm = ss.__assemblies[(re.lastIndex > 0 ? typeName.substring(m.index + 1, re.lastIndex - 1) : typeName.substring(m.index + 1)).trim()]))
						return null;
				}
				break;

			case ']':
				break;

			case ',':
				re.exec(typeName);
				if (!(asm = ss.__assemblies[(re.lastIndex > 0 ? typeName.substring(m.index + 1, re.lastIndex - 1) : typeName.substring(m.index + 1)).trim()]))
					return null;
				break;
		}
	}
	else {
		tname = typeName.substring(last);
	}

	if (outer && re.lastIndex)
		return null;

	var t = ss._getAssemblyType(asm, tname.trim());
	return targs.length ? ss.makeGenericType(t, targs) : t;
}

ss.getType = function ss$getType(typeName, asm) {
	return typeName ? ss._getType(typeName, asm || global) : null;
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
	var result = [];
	if (inherit) {
		var b = ss.getBaseType(type);
		if (b) {
			var a = ss.getAttributes(b, attrType, true);
			for (var i = 0; i < a.length; i++) {
				var t = ss.getInstanceType(a[i]);
				if (!t.__metadata || !t.__metadata.attrNoInherit)
					result.push(a[i]);
			}
		}
	}
	if (type.__metadata && type.__metadata.attr) {
		for (var i = 0; i < type.__metadata.attr.length; i++) {
			var a = type.__metadata.attr[i];
			if (attrType == null || ss.isInstanceOfType(a, attrType)) {
				var t = ss.getInstanceType(a);
				if (!t.__metadata || !t.__metadata.attrAllowMultiple) {
					for (var j = result.length - 1; j >= 0; j--) {
						if (ss.isInstanceOfType(result[j], t))
							result.splice(j, 1);
					}
				}
				result.push(a);
			}
		}
	}
	return result;
};

ss.getMembers = function ss$getMembers(type, memberTypes, bindingAttr, name, params) {
	var result = [];
	if ((bindingAttr & 72) == 72 || (bindingAttr & 6) == 4) {
		var b = ss.getBaseType(type);
		if (b)
			result = ss.getMembers(b, memberTypes & ~1, bindingAttr & (bindingAttr & 64 ? 255 : 247) & (bindingAttr & 2 ? 251 : 255), name, params);
	}

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
			for (var j = 0; j < 4; j++) {
				var a = ['getter','setter','adder','remover'][j];
				if (m[a])
					f(m[a]);
			}
		}
	}

	if (bindingAttr & 256) {
		while (type) {
			var r = [];
			for (var i = 0; i < result.length; i++) {
				if (result[i].typeDef === type)
					r.push(result[i]);
			}
			if (r.length > 1)
				throw new ss_AmbiguousMatchException('Ambiguous match');
			else if (r.length === 1)
				return r[0];
			type = ss.getBaseType(type);
		}
		return null;
	}

	return result;
};

ss.midel = function ss$midel(mi, target, typeArguments) {
	if (mi.isStatic && !!target)
		throw new ss_ArgumentException('Cannot specify target for static method');
	else if (!mi.isStatic && !target)
		throw new ss_ArgumentException('Must specify target for instance method');

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
				throw new ss_ArgumentException('Wrong number of type arguments');
			method = method.apply(null, typeArguments);
		}
		else {
			if (typeArguments && typeArguments.length)
				throw new ss_ArgumentException('Cannot specify type arguments for non-generic method');
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
		throw new ss_ArgumentException('Cannot specify target for static field');
	else if (!fi.isStatic && !obj)
		throw new ss_ArgumentException('Must specify target for instance field');
	obj = fi.isStatic ? fi.typeDef : obj;
	if (arguments.length === 3)
		obj[fi.sname] = arguments[2];
	else
		return obj[fi.sname];
};

///////////////////////////////////////////////////////////////////////////////
// IFormattable

var ss_IFormattable = function IFormattable$() { };

ss_IFormattable.__typeName = 'ss.IFormattable';
ss.IFormattable = ss_IFormattable;
ss.initInterface(ss_IFormattable, ss, { format: null });

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

ss_IComparable.__typeName = 'ss.IComparable';
ss.IComparable = ss_IComparable;
ss.initInterface(ss_IComparable, ss, { compareTo: null });

///////////////////////////////////////////////////////////////////////////////
// IEquatable

var ss_IEquatable = function IEquatable$() { };

ss_IEquatable.__typeName = 'ss.IEquatable';
ss.IEquatable = ss_IEquatable;
ss.initInterface(ss_IEquatable, ss, { equalsT: null });

///////////////////////////////////////////////////////////////////////////////
// Number Extensions

ss.formatNumber = function ss$formatNumber(num, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return num.toString();
	}
	return ss.netFormatNumber(num, format, ss_CultureInfo.invariantCulture.numberFormat);
};

ss.localeFormatNumber = function ss$localeFormatNumber(num, format) {
	if (ss.isNullOrUndefined(format) || (format.length == 0) || (format == 'i')) {
		return num.toLocaleString();
	}
	return ss.netFormatNumber(num, format, ss_CultureInfo.currentCulture.numberFormat);
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

ss.netFormatNumber = function ss$netFormatNumber(num, format, numberFormat) {
	var nf = (numberFormat && numberFormat.getFormat(ss_NumberFormatInfo)) || ss_CultureInfo.currentCulture.numberFormat;

	var s = '';    
	var precision = -1;
	
	if (format.length > 1) {
		precision = parseInt(format.substr(1), 10);
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
ss.netSplit = function ss$netSplit(s, strings, limit, options) {
	var re = new RegExp(strings.map(ss.regexpEscape).join('|'), 'g'), res = [], m, i;
	for (i = 0;; i = re.lastIndex) {
		if (m = re.exec(s)) {
			if (options !== 1 || m.index > i) {
				if (res.length === limit - 1) {
					res.push(s.substr(i));
					return res;
				}
				else
					res.push(s.substring(i, m.index));
			}
		}
		else {
			if (options !== 1 || i !== s.length)
				res.push(s.substr(i));
			return res;
		}
	}
};

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
		ss._formatRE = /\{\{|\}\}|\{[^\}\{]+\}/g;
	}

	return format.replace(ss._formatRE,
		function(m) {
			if (m === '{{' || m === '}}')
				return m.charAt(0);
			var index = parseInt(m.substr(1), 10);
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
			return String.fromCharCode(parseInt(e.substr(1), 10));
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
// IFormatProvider

var ss_IFormatProvider = function IFormatProvider$() { };

ss_IFormatProvider.__typeName = 'ss.IFormatProvider';
ss.IFormatProvider = ss_IFormatProvider;
ss.initInterface(ss_IFormatProvider, ss, { getFormat: null });

///////////////////////////////////////////////////////////////////////////////
// NumberFormatInfo

var ss_NumberFormatInfo = function NumberFormatInfo$() {
};

ss_NumberFormatInfo.__typeName = 'ss.NumberFormatInfo';
ss.NumberFormatInfo = ss_NumberFormatInfo;
ss.initClass(ss_NumberFormatInfo, ss, {
	getFormat:  function NumberFormatInfo$getFormat(type) {
		return (type === ss_NumberFormatInfo) ? this : null;
	}
}, null, [ss_IFormatProvider]);

ss_NumberFormatInfo.invariantInfo = new ss_NumberFormatInfo();
ss.shallowCopy({
	naNSymbol: 'NaN',
	negativeSign: '-',
	positiveSign: '+',
	negativeInfinitySymbol: '-Infinity',
	positiveInfinitySymbol: 'Infinity',

	percentSymbol: '%',
	percentGroupSizes: [3],
	percentDecimalDigits: 2,
	percentDecimalSeparator: '.',
	percentGroupSeparator: ',',
	percentPositivePattern: 0,
	percentNegativePattern: 0,

	currencySymbol: '$',
	currencyGroupSizes: [3],
	currencyDecimalDigits: 2,
	currencyDecimalSeparator: '.',
	currencyGroupSeparator: ',',
	currencyNegativePattern: 0,
	currencyPositivePattern: 0,

	numberGroupSizes: [3],
	numberDecimalDigits: 2,
	numberDecimalSeparator: '.',
	numberGroupSeparator: ','
}, ss_NumberFormatInfo.invariantInfo);

///////////////////////////////////////////////////////////////////////////////
// DateTimeFormatInfo

var ss_DateTimeFormatInfo = function DateTimeFormatInfo$() {
};

ss_DateTimeFormatInfo.__typeName = 'ss.DateTimeFormatInfo';
ss.DateTimeFormatInfo = ss_DateTimeFormatInfo;
ss.initClass(ss_DateTimeFormatInfo, ss, {
	getFormat: function DateTimeFormatInfo$getFormat(type) {
		return type === ss_DateTimeFormatInfo ? this : null;
	}
}, null, [ss_IFormatProvider]);

ss_DateTimeFormatInfo.invariantInfo = new ss_DateTimeFormatInfo();
ss.shallowCopy({
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
}, ss_DateTimeFormatInfo.invariantInfo);

///////////////////////////////////////////////////////////////////////////////
// Stopwatch

var ss_Stopwatch = function Stopwatch$() {
	this._stopTime = 0;
	this._startTime = 0;
	this.isRunning = false;
};

ss_Stopwatch.startNew = function  Stopwatch$startNew() {
	var s = new ss_Stopwatch();
	s.start();
	return s;
};

if (typeof(window) !== 'undefined' && window.performance && window.performance.now) {
	ss_Stopwatch.frequency = 1e6;
	ss_Stopwatch.isHighResolution = true;
	ss_Stopwatch.getTimestamp = function() { return Math.round(window.performance.now() * 1000); };
}
else if (typeof(process) !== 'undefined' && process.hrtime) {
	ss_Stopwatch.frequency = 1e9;
	ss_Stopwatch.isHighResolution = true;
	ss_Stopwatch.getTimestamp = function() { var hr = process.hrtime(); return hr[0] * 1e9 + hr[1]; };
}
else {
	ss_Stopwatch.frequency = 1e3;
	ss_Stopwatch.isHighResolution = false;
	ss_Stopwatch.getTimestamp = function() { return new Date().valueOf(); };
}

ss_Stopwatch.__typeName = 'ss.Stopwatch';
ss.Stopwatch = ss_Stopwatch;
ss.initClass(ss_Stopwatch, ss, {
	reset: function Stopwatch$reset() {
		this._stopTime = this._startTime = ss_Stopwatch.getTimestamp();
		this.isRunning = false;
	},

	ticks: function Stopwatch$ticks() {
		return (this.isRunning ? ss_Stopwatch.getTimestamp() : this._stopTime) - this._startTime;
	},

	milliseconds: function Stopwatch$milliseconds() {
		return Math.round(this.ticks() / ss_Stopwatch.frequency * 1000);
	},

	timeSpan: function Stopwatch$timeSpan() {
		return new ss_TimeSpan(this.milliseconds() * 10000);
	},

	start: function Stopwatch$start() {
		if (this.isRunning)
			return;
		this._startTime = ss_Stopwatch.getTimestamp();
		this.isRunning = true;
	},

	stop: function Stopwatch$stop() {
		if (!this.isRunning)
			return;
		this._stopTime = ss_Stopwatch.getTimestamp();
		this.isRunning = false;
	},

	restart: function Stopwatch$restart() {
		this.isRunning = false;
		this.start();
	}
});

///////////////////////////////////////////////////////////////////////////////
// Array Extensions

ss._flatIndex = function ss$_flatIndex(arr, indices) {
	if (indices.length != (arr._sizes ? arr._sizes.length : 1))
		throw new ss_ArgumentException('Invalid number of indices');

	if (indices[0] < 0 || indices[0] >= (arr._sizes ? arr._sizes[0] : arr.length))
		throw new ss_ArgumentException('Index 0 out of range');

	var idx = indices[0];
	if (arr._sizes) {
		for (var i = 1; i < arr._sizes.length; i++) {
			if (indices[i] < 0 || indices[i] >= arr._sizes[i])
				throw new ss_ArgumentException('Index ' + i + ' out of range');
			idx = idx * arr._sizes[i] + indices[i];
		}
	}
	return idx;
};

ss.arrayGet2 = function ss$arrayGet2(arr, indices) {
	var idx = ss._flatIndex(arr, indices);
	var r = arr[idx];
	return typeof r !== 'undefined' ? r : arr._defvalue;
};

ss.arrayGet = function ss$arrayGet(arr) {
	return ss.arrayGet2(arr, Array.prototype.slice.call(arguments, 1));
}

ss.arraySet2 = function ss$arraySet2(arr, value, indices) {
	var idx = ss._flatIndex(arr, indices);
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
		throw new ss_ArgumentException('Invalid dimension');
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
	throw new ss_InvalidOperationException('Array is empty');
};

ss.arrayPeekBack = function ss$arrayPeekBack(arr) {
	if (arr.length)
		return arr[arr.length - 1];
	throw new ss_InvalidOperationException('Array is empty');
};

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
	if (!ss.isValue(enm))
		return null;

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
	return new Date(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds(), d.getUTCMilliseconds());
};

ss.toUTC = function ss$toUniversalTime(d) {
	return new Date(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds(), d.getUTCMilliseconds());
};

ss.fromUTC = function ss$toLocalTime(d) {
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
	var dtf = useLocale ? ss_CultureInfo.currentCulture.dateTimeFormat : ss_CultureInfo.invariantCulture.dateTimeFormat;

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
				dtf = ss_CultureInfo.InvariantCulture.dateTimeFormat;
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

ss._parseExactDate = function ss$_parseExactDate(val, format, provider, utc) {
	provider = (provider && provider.getFormat(ss_DateTimeFormatInfo)) || ss_CultureInfo.currentCulture.dateTimeFormat;
	var AM = provider.amDesignator, PM = provider.pmDesignator;

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

ss.parseExactDate = function ss$parseExactDate(val, format, provider) {
	return ss._parseExactDate(val, format, provider, false);
};

ss.parseExactDateUTC = function ss$parseExactDateUTC(val, format, provider) {
	return ss._parseExactDate(val, format, provider, true);
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
// RegExp Extensions
ss.regexpEscape = function ss$regexpEscape(s) {
	return s.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
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
	debugger;
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
ss_Enum.__typeName = 'ss.Enum';
ss.Enum = ss_Enum;
ss.initClass(ss_Enum, ss, {});

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
	throw new ss_ArgumentException('Invalid Enumeration Value');
};

ss_Enum.toString = function  Enum$toString(enumType, value) {
	var values = enumType.prototype;
	if (!ss.isFlags(enumType) || (value === 0)) {
		for (var i in values) {
			if (values[i] === value) {
				return i;
			}
		}
		throw new ss_ArgumentException('Invalid Enumeration Value');
	}
	else {
		var parts = [];
		for (var i in values) {
			if (values[i] & value) {
				ss.add(parts, i);
			}
		}
		if (!parts.length) {
			throw new ss_ArgumentException('Invalid Enumeration Value');
		}
		return parts.join(' | ');
	}
};

ss_Enum.getValues = function Enum$getValues(enumType) {
	var parts = [];
	var values = enumType.prototype;
	for (var i in values) {
		if (values.hasOwnProperty(i))
			parts.push(values[i]);
	}
	return parts;
};

///////////////////////////////////////////////////////////////////////////////
// CultureInfo

var ss_CultureInfo = function CultureInfo$(name, numberFormat, dateTimeFormat) {
	this.name = name;
	this.numberFormat = numberFormat;
	this.dateTimeFormat = dateTimeFormat;
};

ss_CultureInfo.__typeName = 'ss.CultureInfo';
ss.CultureInfo = ss_CultureInfo;
ss.initClass(ss_CultureInfo, ss, {
	getFormat:  function CultureInfo$getFormat(type) {
		switch (type) {
			case ss_NumberFormatInfo: return this.numberFormat;
			case ss_DateTimeFormatInfo: return this.dateTimeFormat;
			default: return null;
		}
	}
}, null, [ss_IFormatProvider]);

ss_CultureInfo.invariantCulture = new ss_CultureInfo('en-US', ss_NumberFormatInfo.invariantInfo, ss_DateTimeFormatInfo.invariantInfo);
ss_CultureInfo.currentCulture = ss_CultureInfo.invariantCulture;

///////////////////////////////////////////////////////////////////////////////
// IEnumerator

var ss_IEnumerator = function IEnumerator$() { };

ss_IEnumerator.__typeName = 'ss.IEnumerator';
ss.IEnumerator = ss_IEnumerator;
ss.initInterface(ss_IEnumerator, ss, { current: null, moveNext: null, reset: null }, [ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// IEnumerable

var ss_IEnumerable = function IEnumerable$() { };

ss_IEnumerable.__typeName = 'ss.IEnumerable';
ss.IEnumerable = ss_IEnumerable;
ss.initInterface(ss_IEnumerable, ss, { getEnumerator: null });
ss.getEnumerator = function ss$getEnumerator(obj) {
	return obj.getEnumerator ? obj.getEnumerator() : new ss_ArrayEnumerator(obj);
};

///////////////////////////////////////////////////////////////////////////////
// ICollection

var ss_ICollection = function ICollection$() { };

ss_ICollection.__typeName = 'ss.ICollection';
ss.ICollection = ss_ICollection;
ss.initInterface(ss_ICollection, ss, { get_count: null, add: null, clear: null, contains: null, remove: null });

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

ss_TimeSpan.getDefaultValue = ss_TimeSpan.createInstance = function TimeSpan$default() {
	return new ss_TimeSpan(0);
};

ss_TimeSpan.__typeName = 'ss.TimeSpan';
ss.TimeSpan = ss_TimeSpan;
ss.initClass(ss_TimeSpan, ss, {
	compareTo: function TimeSpan$compareTo(other) {
		return this.ticks < other.ticks ? -1 : (this.ticks > other.ticks ? 1 : 0);
	},
	equals: function TimeSpan$equals(other) {
		return ss.isInstanceOfType(other, ss_TimeSpan) && other.ticks === this.ticks;
	},
	equalsT: function TimeSpan$equalsT(other) {
		return other.ticks === this.ticks;
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
}, null, [ss_IComparable, ss_IEquatable]);
ss_TimeSpan.__class = false;

///////////////////////////////////////////////////////////////////////////////
// IEqualityComparer

var ss_IEqualityComparer = function IEqualityComparer$() { };

ss_IEqualityComparer.__typeName = 'ss.IEqualityComparer';
ss.IEqualityComparer = ss_IEqualityComparer;
ss.initInterface(ss_IEqualityComparer, ss, { areEqual: null, getObjectHashCode: null });

///////////////////////////////////////////////////////////////////////////////
// IComparer

var ss_IComparer = function IComparer$() { };

ss_IComparer.__typeName = 'ss.IComparer';
ss.IComparer = ss_IComparer;
ss.initInterface(ss_IComparer, ss, { compare: null });

///////////////////////////////////////////////////////////////////////////////
// Nullable

ss.unbox = function ss$unbox(instance) {
	if (!ss.isValue(instance))
		throw new ss_InvalidOperationException('Nullable object must have a value.');
	return instance;
};

var ss_Nullable$1 = function Nullable$1$(T) {
	var $type = function() {
	};
	$type.isInstanceOfType = function(instance) {
		return ss.isInstanceOfType(instance, T);
	};
	ss.registerGenericClassInstance($type, ss_Nullable$1, [T], {}, function() { return null; }, function() { return []; });
	return $type;
};

ss_Nullable$1.__typeName = 'ss.Nullable$1';
ss.Nullable$1 = ss_Nullable$1;
ss.initGenericClass(ss_Nullable$1, ss, 1);

ss_Nullable$1.eq = function Nullable$eq(a, b) {
	return !ss.isValue(a) ? !ss.isValue(b) : (a === b);
};

ss_Nullable$1.ne = function Nullable$eq(a, b) {
	return !ss.isValue(a) ? ss.isValue(b) : (a !== b);
};

ss_Nullable$1.le = function Nullable$le(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a <= b;
};

ss_Nullable$1.ge = function Nullable$ge(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a >= b;
};

ss_Nullable$1.lt = function Nullable$lt(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a < b;
};

ss_Nullable$1.gt = function Nullable$gt(a, b) {
	return ss.isValue(a) && ss.isValue(b) && a > b;
};

ss_Nullable$1.sub = function Nullable$sub(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a - b : null;
};

ss_Nullable$1.add = function Nullable$add(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a + b : null;
};

ss_Nullable$1.mod = function Nullable$mod(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a % b : null;
};

ss_Nullable$1.div = function Nullable$divf(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a / b : null;
};

ss_Nullable$1.mul = function Nullable$mul(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a * b : null;
};

ss_Nullable$1.band = function Nullable$band(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a & b : null;
};

ss_Nullable$1.bor = function Nullable$bor(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a | b : null;
};

ss_Nullable$1.xor = function Nullable$xor(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a ^ b : null;
};

ss_Nullable$1.shl = function Nullable$shl(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a << b : null;
};

ss_Nullable$1.srs = function Nullable$srs(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a >> b : null;
};

ss_Nullable$1.sru = function Nullable$sru(a, b) {
	return ss.isValue(a) && ss.isValue(b) ? a >>> b : null;
};

ss_Nullable$1.and = function Nullable$and(a, b) {
	if (a === true && b === true)
		return true;
	else if (a === false || b === false)
		return false;
	else
		return null;
};

ss_Nullable$1.or = function Nullable$or(a, b) {
	if (a === true || b === true)
		return true;
	else if (a === false && b === false)
		return false;
	else
		return null;
};

ss_Nullable$1.not = function Nullable$not(a) {
	return ss.isValue(a) ? !a : null;
};

ss_Nullable$1.neg = function Nullable$neg(a) {
	return ss.isValue(a) ? -a : null;
};

ss_Nullable$1.pos = function Nullable$pos(a) {
	return ss.isValue(a) ? +a : null;
};

ss_Nullable$1.cpl = function Nullable$cpl(a) {
	return ss.isValue(a) ? ~a : null;
};

ss_Nullable$1.lift = function Nullable$lift() {
	for (var i = 0; i < arguments.length; i++) {
		if (!ss.isValue(arguments[i]))
			return null;
	}
	return arguments[0].apply(null, Array.prototype.slice.call(arguments, 1));
};

///////////////////////////////////////////////////////////////////////////////
// IList

var ss_IList = function IList$() { };

ss_IList.__typeName = 'ss.IList';
ss.IList = ss_IList;
ss.initInterface(ss_IList, ss, { get_item: null, set_item: null, indexOf: null, insert: null, removeAt: null }, [ss_ICollection, ss_IEnumerable]);

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

ss_IDictionary.__typeName = 'ss.IDictionary';
ss.IDictionary = ss_IDictionary;
ss.initInterface(ss_IDictionary, ss, { get_item: null, set_item: null, get_keys: null, get_values: null, containsKey: null, add: null, remove: null, tryGetValue: null }, [ss_IEnumerable]);

///////////////////////////////////////////////////////////////////////////////
// Int32

var ss_Int32 = function Int32$() { };

ss_Int32.__typeName = 'ss.Int32';
ss.Int32 = ss_Int32;
ss.initClass(ss_Int32, ss, {}, Object, [ ss_IEquatable, ss_IComparable, ss_IFormattable ]);
ss_Int32.__class = false;

ss_Int32.isInstanceOfType = function Int32$isInstanceOfType(instance) {
	return typeof(instance) === 'number' && isFinite(instance) && Math.round(instance, 0) == instance;
};

ss_Int32.getDefaultValue = ss_Int32.createInstance = function Int32$getDefaultValue() {
	return 0;
};

ss_Int32.div = function Int32$div(a, b) {
	if (!ss.isValue(a) || !ss.isValue(b)) return null;
	if (b === 0) throw new ss_DivideByZeroException();
	return ss_Int32.trunc(a / b);
};

ss_Int32.trunc = function Int32$trunc(n) {
	return ss.isValue(n) ? (n > 0 ? Math.floor(n) : Math.ceil(n)) : null;
};

ss_Int32.tryParse = function Int32$tryParse(s, result, min, max) {
	result.$ = 0;
	if (!/^[+-]?[0-9]+$/.test(s))
		return 0;
	var n = parseInt(s, 10);
	if (n < min || n > max)
		return false;
	result.$ = n;
	return true;
};

///////////////////////////////////////////////////////////////////////////////
// MutableDateTime

var ss_JsDate = function JsDate$() { };

ss_JsDate.__typeName = 'ss.JsDate';
ss.JsDate = ss_JsDate;
ss.initClass(ss_JsDate, ss, {}, Object, [ ss_IEquatable, ss_IComparable ]);

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
ss_ArrayEnumerator.__typeName = 'ss.ArrayEnumerator';
ss.ArrayEnumerator = ss_ArrayEnumerator;
ss.initClass(ss_ArrayEnumerator, ss, {
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
}, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// ObjectEnumerator

var ss_ObjectEnumerator = function ObjectEnumerator$(o) {
	this._keys = Object.keys(o);
	this._index = -1;
	this._object = o;
};

ss_ObjectEnumerator.__typeName = 'ss.ObjectEnumerator';
ss.ObjectEnumerator = ss_ObjectEnumerator;
ss.initClass(ss_ObjectEnumerator, ss, {
	moveNext: function ObjectEnumerator$moveNext() {
		this._index++;
		return (this._index < this._keys.length);
	},
	reset: function ObjectEnumerator$reset() {
		this._index = -1;
	},
	current: function ObjectEnumerator$current() {
		if (this._index < 0 || this._index >= this._keys.length)
			throw new ss_InvalidOperationException('Invalid operation');
		var k = this._keys[this._index];
		return { key: k, value: this._object[k] };
	},
	dispose: function ObjectEnumerator$dispose() {
	}
}, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// EqualityComparer

var ss_EqualityComparer = function EqualityComparer$() {
};
ss_EqualityComparer.__typeName = 'ss.EqualityComparer';
ss.EqualityComparer = ss_EqualityComparer;
ss.initClass(ss_EqualityComparer, ss, {
	areEqual: function EqualityComparer$areEqual(x, y) {
		return ss.staticEquals(x, y);
	},
	getObjectHashCode: function EqualityComparer$getObjectHashCode(obj) {
		return ss.isValue(obj) ? ss.getHashCode(obj) : 0;
	}
}, null, [ss_IEqualityComparer]);
ss_EqualityComparer.def = new ss_EqualityComparer();

///////////////////////////////////////////////////////////////////////////////
// Comparer

var ss_Comparer = function Comparer$(f) {
	this.f = f;
};

ss_Comparer.__typeName = 'ss.Comparer';
ss.Comparer = ss_Comparer;
ss.initClass(ss_Comparer, ss, {
	compare: function Comparer$compare(x, y) {
		return this.f(x, y);
	}
}, null, [ss_IComparer]);
ss_Comparer.def = new ss_Comparer(function Comparer$defaultCompare(a, b) {
	if (!ss.isValue(a))
		return !ss.isValue(b)? 0 : -1;
	else if (!ss.isValue(b))
		return 1;
	else
		return ss.compare(a, b);
});

///////////////////////////////////////////////////////////////////////////////
// Dictionary
var ss_$DictionaryCollection = function $DictionaryCollection$(dict, isKeys) {
	this._dict = dict;
	this._isKeys = isKeys;
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

	ss.registerGenericClassInstance($type, ss_Dictionary$2, [TKey, TValue], {
		_setOrAdd: function(key, value, add) {
			var hash = this.comparer.getObjectHashCode(key);
			var entry = { key: key, value: value };
			if (this.buckets.hasOwnProperty(hash)) {
				var array = this.buckets[hash];
				for (var i = 0; i < array.length; i++) {
					if (this.comparer.areEqual(array[i].key, key)) {
						if (add)
							throw new ss_ArgumentException('Key ' + key + ' already exists.');
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
				throw new ss_KeyNotFoundException('Key ' + key + ' does not exist.');
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
	}, function() { return null; }, function() { return [ ss_IDictionary, ss_IEnumerable ]; });
	return $type;
};

ss_Dictionary$2.__typeName = 'ss.Dictionary$2';
ss.Dictionary$2 = ss_Dictionary$2;
ss.initGenericClass(ss_Dictionary$2, ss, 2);
ss_$DictionaryCollection.__typeName = 'ss.$DictionaryCollection';
ss.$DictionaryCollection = ss_$DictionaryCollection;
ss.initClass(ss_$DictionaryCollection, ss, {
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
		throw new ss_InvalidOperationException('Collection is read-only');
	},
	clear: function $DictionaryCollection$clear() {
		throw new ss_InvalidOperationException('Collection is read-only');
	},
	remove: function $DictionaryCollection$remove() {
		throw new ss_InvalidOperationException('Collection is read-only');
	}
}, null, [ss_IEnumerable, ss_ICollection]);

///////////////////////////////////////////////////////////////////////////////
// IDisposable

var ss_IDisposable = function IDisposable$() { };
ss_IDisposable.__typeName = 'ss.IDisposable';
ss.IDisposable = ss_IDisposable;
ss.initInterface(ss_IDisposable, ss, { dispose: null });

///////////////////////////////////////////////////////////////////////////////
// StringBuilder

var ss_StringBuilder = function StringBuilder$(s) {
	this._parts = (ss.isValue(s) && s != '') ? [s] : [];
	this.length = ss.isValue(s) ? s.length : 0;
}

ss_StringBuilder.__typeName = 'ss.StringBuilder';
ss.StringBuilder = ss_StringBuilder;
ss.initClass(ss_StringBuilder, ss, {
	append: function StringBuilder$append(o) {
		if (ss.isValue(o)) {
			var s = o.toString();
			ss.add(this._parts, s);
			this.length += s.length;
		}
		return this;
	},

	appendChar: function StringBuilder$appendChar(c) {
		return this.append(String.fromCharCode(c));
	},

	appendLine: function StringBuilder$appendLine(s) {
		this.append(s);
		this.append('\r\n');
		return this;
	},

	appendLineChar: function StringBuilder$appendLineChar(c) {
		return this.appendLine(String.fromCharCode(c));
	},

	clear: function StringBuilder$clear() {
		this._parts = [];
		this.length = 0;
	},

	toString: function StringBuilder$toString() {
		return this._parts.join('');
	}
});

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

ss_Random.__typeName = 'ss.Random';
ss.Random = ss_Random;
ss.initClass(ss_Random, ss, {
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
});

///////////////////////////////////////////////////////////////////////////////
// EventArgs

var ss_EventArgs = function EventArgs$() {
}
ss_EventArgs.__typeName = 'ss.EventArgs';
ss.EventArgs = ss_EventArgs;
ss.initClass(ss_EventArgs, ss, {});

ss_EventArgs.Empty = new ss_EventArgs();

///////////////////////////////////////////////////////////////////////////////
// Exception

var ss_Exception = function Exception$(message, innerException) {
	this._message = message || 'An error occurred.';
	this._innerException = innerException || null;
	this._error = new Error();
}

ss_Exception.__typeName = 'ss.Exception';
ss.Exception = ss_Exception;
ss.initClass(ss_Exception, ss, {
	get_message: function Exception$get_message() {
		return this._message;
	},
	get_innerException: function Exception$get_innerException() {
		return this._innerException;
	},
	get_stack: function Exception$get_stack() {
		return this._error.stack;
	}
});

ss_Exception.wrap = function Exception$wrap(o) {
	if (ss.isInstanceOfType(o, ss_Exception)) {
		return o;
	}
	else if (o instanceof TypeError) {
		// TypeError can either be 'cannot read property blah of null/undefined' (proper NullReferenceException), or it can be eg. accessing a non-existent method of an object.
		// As long as all code is compiled, they should with a very high probability indicate the use of a null reference.
		return new ss_NullReferenceException(o.message, new ss_JsErrorException(o));
	}
	else if (o instanceof RangeError) {
		return new ss_ArgumentOutOfRangeException(null, o.message, new ss_JsErrorException(o));
	}
	else if (o instanceof Error) {
		return new ss_JsErrorException(o);
	}
	else {
		return new ss_Exception(o.toString());
	}
};

////////////////////////////////////////////////////////////////////////////////
// NotImplementedException

var ss_NotImplementedException = function NotImplementedException$(message, innerException) {
	ss_Exception.call(this, message || 'The method or operation is not implemented.', innerException);
};
ss_NotImplementedException.__typeName = 'ss.NotImplementedException';
ss.NotImplementedException = ss_NotImplementedException;
ss.initClass(ss_NotImplementedException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// NotSupportedException

var ss_NotSupportedException = function NotSupportedException$(message, innerException) {
	ss_Exception.call(this, message || 'Specified method is not supported.', innerException);
};
ss_NotSupportedException.__typeName = 'ss.NotSupportedException';
ss.NotSupportedException = ss_NotSupportedException;
ss.initClass(ss_NotSupportedException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// AggregateException

var ss_AggregateException = function AggregateException$(message, innerExceptions) {
	this.innerExceptions = ss.isValue(innerExceptions) ? ss.arrayFromEnumerable(innerExceptions) : [];
	ss_Exception.call(this, message || 'One or more errors occurred.', this.innerExceptions.length ? this.innerExceptions[0] : null);
};

ss_AggregateException.__typeName = 'ss.AggregateException';
ss.AggregateException = ss_AggregateException;
ss.initClass(ss_AggregateException, ss, {
	flatten: function  AggregateException$flatten() {
		var inner = [];
		for (var i = 0; i < this.innerExceptions.length; i++) {
			var e = this.innerExceptions[i];
			if (ss.isInstanceOfType(e, ss_AggregateException)) {
				inner.push.apply(inner, e.flatten().innerExceptions);
			}
			else {
				inner.push(e);
			}
		}
		return new ss_AggregateException(this._message, inner);
	}
}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// PromiseException

var ss_PromiseException = function PromiseException(args, message, innerException) {
	ss_Exception.call(this, message || (args.length && args[0] ? args[0].toString() : 'An error occurred'), innerException);
	this.arguments = ss.arrayClone(args);
};

ss_PromiseException.__typeName = 'ss.PromiseException';
ss.PromiseException = ss_PromiseException;
ss.initClass(ss_PromiseException, ss, {
	get_arguments: function PromiseException$get_arguments() {
		return this._arguments;
	}
}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// JsErrorException

var ss_JsErrorException = function JsErrorException$(error, message, innerException) {
	ss_Exception.call(this, message || error.message, innerException);
	this.error = error;
};
ss_JsErrorException.__typeName = 'ss.JsErrorException';
ss.JsErrorException = ss_JsErrorException;
ss.initClass(ss_JsErrorException, ss, {
	get_stack: function Exception$get_stack() {
		return this.error.stack;
	}
}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// ArgumentException

var ss_ArgumentException = function ArgumentException$(message, paramName, innerException) {
	ss_Exception.call(this, message || 'Value does not fall within the expected range.', innerException);
	this.paramName = paramName || null;
};

ss_ArgumentException.__typeName = 'ss.ArgumentException';
ss.ArgumentException = ss_ArgumentException;
ss.initClass(ss_ArgumentException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// ArgumentNullException

var ss_ArgumentNullException = function ArgumentNullException$(paramName, message, innerException) {
	if (!message) {
		message = 'Value cannot be null.';
		if (paramName)
			message += '\nParameter name: ' + paramName;
	}

	ss_ArgumentException.call(this, message, paramName, innerException);
};

ss_ArgumentNullException.__typeName = 'ss.ArgumentNullException';
ss.ArgumentNullException = ss_ArgumentNullException;
ss.initClass(ss_ArgumentNullException, ss, {}, ss_ArgumentException);

////////////////////////////////////////////////////////////////////////////////
// ArgumentNullException

var ss_ArgumentOutOfRangeException = function ArgumentOutOfRangeException$(paramName, message, innerException, actualValue) {
	if (!message) {
		message = 'Value is out of range.';
		if (paramName)
			message += '\nParameter name: ' + paramName;
	}

	ss_ArgumentException.call(this, message, paramName, innerException);
	this.actualValue = actualValue || null;
};

ss_ArgumentOutOfRangeException.__typeName = 'ss.ArgumentOutOfRangeException';
ss.ArgumentOutOfRangeException = ss_ArgumentOutOfRangeException;
ss.initClass(ss_ArgumentOutOfRangeException, ss, {}, ss_ArgumentException);

////////////////////////////////////////////////////////////////////////////////
// FormatException

var ss_FormatException = function FormatException$(message, innerException) {
	ss_Exception.call(this, message || 'Invalid format.', innerException);
};
ss_FormatException.__typeName = 'ss.FormatException';
ss.FormatException = ss_FormatException;
ss.initClass(ss_FormatException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// DivideByZeroException

var ss_DivideByZeroException = function DivideByZeroException$(message, innerException) {
	ss_Exception.call(this, message || 'Division by 0.', innerException);
};
ss_DivideByZeroException.__typeName = 'ss.DivideByZeroException';
ss.DivideByZeroException = ss_DivideByZeroException;
ss.initClass(ss_DivideByZeroException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// InvalidCastException

var ss_InvalidCastException = function InvalidCastException$(message, innerException) {
	ss_Exception.call(this, message || 'The cast is not valid.', innerException);
};
ss_InvalidCastException.__typeName = 'ss.InvalidCastException';
ss.InvalidCastException = ss_InvalidCastException;
ss.initClass(ss_InvalidCastException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// InvalidOperationException

var ss_InvalidOperationException = function InvalidOperationException$(message, innerException) {
	ss_Exception.call(this, message || 'Operation is not valid due to the current state of the object.', innerException);
};
ss_InvalidOperationException.__typeName = 'ss.InvalidOperationException';
ss.InvalidOperationException = ss_InvalidOperationException;
ss.initClass(ss_InvalidOperationException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// NullReferenceException

var ss_NullReferenceException = function NullReferenceException$(message, innerException) {
	ss_Exception.call(this, message || 'Object is null.', innerException);
};
ss_NullReferenceException.__typeName = 'ss.NullReferenceException';
ss.NullReferenceException = ss_NullReferenceException;
ss.initClass(ss_NullReferenceException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// KeyNotFoundException

var ss_KeyNotFoundException = function KeyNotFoundException$(message, innerException) {
	ss_Exception.call(this, message || 'Key not found.', innerException);
};
ss_KeyNotFoundException.__typeName = 'ss.KeyNotFoundException';
ss.KeyNotFoundException = ss_KeyNotFoundException;
ss.initClass(ss_KeyNotFoundException, ss, {}, ss_Exception);

////////////////////////////////////////////////////////////////////////////////
// InvalidOperationException

var ss_AmbiguousMatchException = function AmbiguousMatchException$(message, innerException) {
	ss_Exception.call(this, message || 'Ambiguous match.', innerException);
};
ss_AmbiguousMatchException.__typeName = 'ss.AmbiguousMatchException';
ss.AmbiguousMatchException = ss_AmbiguousMatchException;
ss.initClass(ss_AmbiguousMatchException, ss, {}, ss_Exception);

///////////////////////////////////////////////////////////////////////////////
// IteratorBlockEnumerable

var ss_IteratorBlockEnumerable = function IteratorBlockEnumerable$(getEnumerator, $this) {
	this._getEnumerator = getEnumerator;
	this._this = $this;
};

ss_IteratorBlockEnumerable.__typeName = 'ss.IteratorBlockEnumerable';
ss.IteratorBlockEnumerable = ss_IteratorBlockEnumerable;
ss.initClass(ss_IteratorBlockEnumerable, ss, {
	getEnumerator: function IteratorBlockEnumerable$getEnumerator() {
		return this._getEnumerator.call(this._this);
	}
}, null, [ss_IEnumerable]);

///////////////////////////////////////////////////////////////////////////////
// IteratorBlockEnumerator

var ss_IteratorBlockEnumerator = function IteratorBlockEnumerator$(moveNext, getCurrent, dispose, $this) {
	this._moveNext = moveNext;
	this._getCurrent = getCurrent;
	this._dispose = dispose;
	this._this = $this;
};

ss_IteratorBlockEnumerator.__typeName = 'ss.IteratorBlockEnumerator';
ss.IteratorBlockEnumerator = ss_IteratorBlockEnumerator;
ss.initClass(ss_IteratorBlockEnumerator, ss, {
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
}, null, [ss_IEnumerator, ss_IDisposable]);

///////////////////////////////////////////////////////////////////////////////
// Lazy

var ss_Lazy = function Lazy$(valueFactory) {
	this._valueFactory = valueFactory;
	this.isValueCreated = false;
};
ss_Lazy.__typeName = 'ss.Lazy';
ss.Lazy = ss_Lazy;
ss.initClass(ss_Lazy, ss, {
	value: function Lazy$value() {
		if (!this.isValueCreated) {
			this._value = this._valueFactory();
			delete this._valueFactory;
			this.isValueCreated = true;
		}
		return this._value;
	}
});

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

ss_Task.run = function Task$run(f) {
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
							ss.arrayAddRange(exceptions, t.exception.innerExceptions);
							break;
						default:
							throw new ss_InvalidOperationException('Invalid task status ' + t.status);
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
		throw new ss_ArgumentException('Must wait for at least one task', 'tasks');

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
					tcs.trySetException(t.exception.innerExceptions);
					break;
				default:
					throw new ss_InvalidOperationException('Invalid task status ' + t.status);
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

ss_Task.__typeName = 'ss.Task';
ss.Task = ss_Task;
ss.initClass(ss_Task, ss, {
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
			throw new ss_InvalidOperationException('Task was already started.');
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
				_this._fail(new ss_AggregateException(null, [ss_Exception.wrap(e)]));
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
	_getResult: function Task$_getResult(await) {
		switch (this.status) {
			case 5:
				return this._result;
			case 6:
				throw new ss_InvalidOperationException('Task was cancelled.');
			case 7:
				throw await ? this.exception.innerExceptions[0] : this.exception;
			default:
				throw new ss_InvalidOperationException('Task is not yet completed.');
		}
	},
	getResult: function Task$getResult() {
		return this._getResult(false);
	},
	getAwaitedResult: function Task$getAwaitedResult() {
		return this._getResult(true);
	},
	dispose: function Task$dispose() {
	}
}, null, [ss_IDisposable]);

////////////////////////////////////////////////////////////////////////////////
// TaskStatus
var ss_TaskStatus = function() {
};
ss_TaskStatus.__typeName = 'ss.TaskStatus';
ss.TaskStatus = ss_TaskStatus;
ss.initEnum(ss_TaskStatus, ss, { created: 0, running: 3, ranToCompletion: 5, canceled: 6, faulted: 7 });

///////////////////////////////////////////////////////////////////////////////
// TaskCompletionSource

var ss_TaskCompletionSource = function TaskCompletionSource$() {
	this.task = new ss_Task();
	this.task.status = 3;
};

ss_TaskCompletionSource.__typeName = 'ss.TaskCompletionSource';
ss.TaskCompletionSource = ss_TaskCompletionSource;
ss.initClass(ss_TaskCompletionSource, ss, {
	setCanceled: function TaskCompletionSource$setCanceled() {
		if (!this.task._cancel())
			throw new ss_InvalidOperationException('Task was already completed.');
	},
	setResult: function TaskCompletionSource$setResult(result) {
		if (!this.task._complete(result))
			throw new ss_InvalidOperationException('Task was already completed.');
	},
	setException: function TaskCompletionSource$setException(exception) {
		if (!this.trySetException(exception))
			throw new ss_InvalidOperationException('Task was already completed.');
	},
	trySetCanceled: function TaskCompletionSource$trySetCanceled() {
		return this.task._cancel();
	},
	trySetResult: function TaskCompletionSource$setResult(result) {
		return this.task._complete(result);
	},
	trySetException: function TaskCompletionSource$setException(exception) {
		if (ss.isInstanceOfType(exception, ss_Exception))
			exception = [exception];
		return this.task._fail(new ss_AggregateException(null, exception));
	}
});

///////////////////////////////////////////////////////////////////////////////
// CancelEventArgs

var ss_CancelEventArgs = function CancelEventArgs$() {
	ss_EventArgs.call(this);
	this.cancel = false;
}

ss_CancelEventArgs.__typeName = 'ss.CancelEventArgs';
ss.CancelEventArgs = ss_CancelEventArgs;
ss.initClass(ss_CancelEventArgs, ss, {}, ss_EventArgs);

///////////////////////////////////////////////////////////////////////////////
// Guid

var ss_Guid = function Guid$() {
};
ss_Guid.$valid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/ig;
ss_Guid.$split = /^(.{8})(.{4})(.{4})(.{4})(.{12})$/;
ss_Guid.empty = '00000000-0000-0000-0000-000000000000';
ss_Guid.$rng = new ss_Random();

ss_Guid.__typeName = 'ss.Guid';
ss.Guid = ss_Guid;
ss.initClass(ss_Guid, ss, {}, Object, [ ss_IEquatable, ss_IComparable ]);
ss_Guid.__class = false;

ss_Guid.isInstanceOfType = function Guid$isInstanceOfType(instance) {
	return typeof(instance) === 'string' && instance.match(ss_Guid.$valid);
};

ss_Guid.getDefaultValue = ss_Guid.createInstance = function Guid$default() {
	return ss_Guid.empty;
};

ss_Guid.parse = function Guid$parse(uuid, format) {
	var r = {};
	if (ss_Guid.tryParse(uuid, format, r))
		return r.$;
	throw new ss_FormatException('Unable to parse UUID');
};

ss_Guid.tryParse = function Guid$tryParse(uuid, format, r) {
	r.$ = ss_Guid.empty;
	if (!ss.isValue(uuid)) throw new ss_ArgumentNullException('uuid');
	if (!format) {
		var m = /^[{(]?([0-9a-f]{8})-?([0-9a-f]{4})-?([0-9a-f]{4})-?([0-9a-f]{4})-?([0-9a-f]{12})[)}]?$/ig.exec(uuid);
		if (m) {
			r.$ = m.slice(1).join('-').toLowerCase();
			return true;
		}
	}
	else {
		if (format === 'N') {
			var m = ss_Guid.$split.exec(uuid);
			if (!m)
				return false;
			uuid = m.slice(1).join('-');
		}
		else if (format === 'B' || format === 'P') {
			var b = format === 'B';
			if (uuid[0] !== (b ? '{' : '(') || uuid[uuid.length - 1] !== (b ? '}' : ')'))
				return false;
			uuid = uuid.substr(1, uuid.length - 2);
		}
		if (uuid.match(ss_Guid.$valid)) {
			r.$ = uuid.toLowerCase();
			return true;
		}
	}
	return false;
};

ss_Guid.format = function Guid$format(uuid, format) {
	switch (format) {
		case 'N': return uuid.replace(/-/g, '');
		case 'B': return '{' + uuid + '}';
		case 'P': return '(' + uuid + ')';
		default : return uuid;
	}
}

ss_Guid.fromBytes = function Guid$fromBytes(b) {
	if (!b || b.length !== 16)
		throw new ss_ArgumentException('b', 'Must be 16 bytes');
	var s = b.map(function(x) { return ss.formatNumber(x & 0xff, 'x2'); }).join('');
	return ss_Guid.$split.exec(s).slice(1).join('-');
}

ss_Guid.newGuid = function Guid$newGuid() {
	var a = Array(16);
	ss_Guid.$rng.nextBytes(a);
	a[6] = a[6] & 0x0f | 0x40;
	a[8] = a[8] & 0xbf | 0x80;
	return ss_Guid.fromBytes(a);
};

ss_Guid.getBytes = function Guid$getBytes(uuid) {
	var a = Array(16);
	var s = uuid.replace(/-/g, '');
	for (var i = 0; i < 16; i++) {
		a[i] = parseInt(s.substr(i * 2, 2), 16);
	}
	return a;
};

if (global.ss) {
	for (var n in ss) {
		if (ss.hasOwnProperty(n))
			global.ss[n] = ss[n];
	}
}
else {
	global.ss = ss;
}
})(global);

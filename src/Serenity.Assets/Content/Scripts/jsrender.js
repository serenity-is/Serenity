/*! JsRender v1.0.0-beta: http://github.com/BorisMoore/jsrender and http://jsviews.com/jsviews
informal pre V1.0 commit counter: 56 */
/*
 * Optimized version of jQuery Templates, for rendering to string.
 * Does not require jQuery, or HTML DOM
 * Integrates with JsViews (http://jsviews.com/jsviews)
 *
 * Copyright 2014, Boris Moore
 * Released under the MIT License.
 */

(function(global, jQuery, undefined) {
	// global is the this object, which is window when running in the usual browser environment.
	"use strict";

	if (jQuery && jQuery.render || global.jsviews) { return; } // JsRender is already loaded

	//========================== Top-level vars ==========================

	var versionNumber = "v1.0.0-beta",

		$, jsvStoreName, rTag, rTmplString, indexStr, // nodeJsModule,

//TODO	tmplFnsCache = {},
		delimOpenChar0 = "{", delimOpenChar1 = "{", delimCloseChar0 = "}", delimCloseChar1 = "}", linkChar = "^",

		rPath = /^(!*?)(?:null|true|false|\d[\d.]*|([\w$]+|\.|~([\w$]+)|#(view|([\w$]+))?)([\w$.^]*?)(?:[.[^]([\w$]+)\]?)?)$/g,
		//                                     none   object     helper    view  viewProperty pathTokens      leafToken

		rParams = /(\()(?=\s*\()|(?:([([])\s*)?(?:(\^?)(!*?[#~]?[\w$.^]+)?\s*((\+\+|--)|\+|-|&&|\|\||===|!==|==|!=|<=|>=|[<>%*:?\/]|(=))\s*|(!*?[#~]?[\w$.^]+)([([])?)|(,\s*)|(\(?)\\?(?:(')|("))|(?:\s*(([)\]])(?=\s*\.|\s*\^|\s*$)|[)\]])([([]?))|(\s+)/g,
		//          lftPrn0        lftPrn        bound            path    operator err                                                eq             path2       prn    comma   lftPrn2   apos quot      rtPrn rtPrnDot                        prn2  space
		// (left paren? followed by (path? followed by operator) or (path followed by left paren?)) or comma or apos or quot or right paren or space

		rNewLine = /[ \t]*(\r\n|\n|\r)/g,
		rUnescapeQuotes = /\\(['"])/g,
		rEscapeQuotes = /['"\\]/g, // Escape quotes and \ character
		rBuildHash = /(?:\x08|^)(onerror:)?(?:(~?)(([\w$]+):)?([^\x08]+))\x08(,)?([^\x08]+)/gi,
		rTestElseIf = /^if\s/,
		rFirstElem = /<(\w+)[>\s]/,
		rAttrEncode = /[\x00`><"'&]/g, // Includes > encoding since rConvertMarkers in JsViews does not skip > characters in attribute strings
		rIsHtml = /[\x00`><\"'&]/,
		rHasHandlers = /^on[A-Z]|^convert(Back)?$/,
		rHtmlEncode = rAttrEncode,
		autoTmplName = 0,
		viewId = 0,
		charEntities = {
			"&": "&amp;",
			"<": "&lt;",
			">": "&gt;",
			"\x00": "&#0;",
			"'": "&#39;",
			'"': "&#34;",
			"`": "&#96;"
		},
		htmlStr = "html",
		tmplAttr = "data-jsv-tmpl",
		$render = {},
		jsvStores = {
			template: {
				compile: compileTmpl
			},
			tag: {
				compile: compileTag
			},
			helper: {},
			converter: {}
		},

		// jsviews object ($.views if jQuery is loaded)
		$views = {
			jsviews: versionNumber,
			settings: function(settings) {
				$extend($viewsSettings, settings);
				dbgMode($viewsSettings._dbgMode);
				if ($viewsSettings.jsv) {
					$viewsSettings.jsv();
				}
			},
			sub: {
				// subscription, e.g. JsViews integration
				View: View,
				Err: JsViewsError,
				tmplFn: tmplFn,
				cvt: convertArgs,
				parse: parseParams,
				extend: $extend,
				syntaxErr: syntaxError,
				onStore: {},
				_lnk: retVal
			},
//			map: $views.dataMap || dataMap, // If jsObservable loaded first, use that definition of dataMap
			map: dataMap, // If jsObservable loaded first, use that definition of dataMap
			_cnvt: convertVal,
			_tag: renderTag,
			_err: error
		};

	function retVal(val) {
		return val;
	}

	function dbgBreak(val) {
		debugger;
		return val;
	}

	function dbgMode(debugMode) {
		$viewsSettings._dbgMode = debugMode;
		indexStr = debugMode ? "Unavailable (nested view): use #getIndex()" : ""; // If in debug mode set #index to a warning when in nested contexts
		$tags("dbg", $helpers.dbg = $converters.dbg = debugMode ? dbgBreak : retVal); // If in debug mode, register {{dbg/}}, {{dbg:...}} and ~dbg() to insert break points for debugging.
	}

	function JsViewsError(message) {
		// Error exception type for JsViews/JsRender
		// Override of $.views.sub.Error is possible
		this.name = ($.link ? "JsViews" : "JsRender") + " Error";
		this.message = message || this.name;
	}

	function $extend(target, source) {
		var name;
		for (name in source) {
			target[name] = source[name];
		}
		return target;
	}

	function $isFunction(ob) {
		return typeof ob === "function";
	}

	(JsViewsError.prototype = new Error()).constructor = JsViewsError;

	//========================== Top-level functions ==========================

	//===================
	// jsviews.delimiters
	//===================
	function $viewsDelimiters(openChars, closeChars, link) {
		// Set the tag opening and closing delimiters and 'link' character. Default is "{{", "}}" and "^"
		// openChars, closeChars: opening and closing strings, each with two characters

		if (!$sub.rTag || openChars) {
			delimOpenChar0 = openChars ? openChars.charAt(0) : delimOpenChar0; // Escape the characters - since they could be regex special characters
			delimOpenChar1 = openChars ? openChars.charAt(1) : delimOpenChar1;
			delimCloseChar0 = closeChars ? closeChars.charAt(0) : delimCloseChar0;
			delimCloseChar1 = closeChars ? closeChars.charAt(1) : delimCloseChar1;
			linkChar = link || linkChar;
			openChars = "\\" + delimOpenChar0 + "(\\" + linkChar + ")?\\" + delimOpenChar1;  // Default is "{^{"
			closeChars = "\\" + delimCloseChar0 + "\\" + delimCloseChar1;                   // Default is "}}"
			// Build regex with new delimiters
			//          tag    (followed by / space or })   or cvtr+colon or html or code
			rTag = "(?:(?:(\\w+(?=[\\/\\s\\" + delimCloseChar0 + "]))|(?:(\\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\\*)))"
				+ "\\s*((?:[^\\" + delimCloseChar0 + "]|\\" + delimCloseChar0 + "(?!\\" + delimCloseChar1 + "))*?)";

			// make rTag available to JsViews (or other components) for parsing binding expressions
			$sub.rTag = rTag + ")";

			rTag = new RegExp(openChars + rTag + "(\\/)?|(?:\\/(\\w+)))" + closeChars, "g");

			// Default:    bind           tag       converter colon html     comment            code      params            slash   closeBlock
			//           /{(\^)?{(?:(?:(\w+(?=[\/\s}]))|(?:(\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\*)))\s*((?:[^}]|}(?!}))*?)(\/)?|(?:\/(\w+)))}}/g

			rTmplString = new RegExp("<.*>|([^\\\\]|^)[{}]|" + openChars + ".*" + closeChars);
			// rTmplString looks for html tags or { or } char not preceded by \\, or JsRender tags {{xxx}}. Each of these strings are considered
			// NOT to be jQuery selectors
		}
		return [delimOpenChar0, delimOpenChar1, delimCloseChar0, delimCloseChar1, linkChar];
	}

	//=========
	// View.get
	//=========

	function getView(inner, type) { //view.get(inner, type)
		if (!type) {
			// view.get(type)
			type = inner;
			inner = undefined;
		}

		var views, i, l, found,
			view = this,
			root = !type || type === "root";
			// If type is undefined, returns root view (view under top view).

		if (inner) {
			// Go through views - this one, and all nested ones, depth-first - and return first one with given type.
			found = view.type === type ? view : undefined;
			if (!found) {
				views = view.views;
				if (view._.useKey) {
					for (i in views) {
						if (found = views[i].get(inner, type)) {
							break;
						}
					}
				} else {
					for (i = 0, l = views.length; !found && i < l; i++) {
						found = views[i].get(inner, type);
					}
				}
			}
		} else if (root) {
			// Find root view. (view whose parent is top view)
			while (view.parent.parent) {
				found = view = view.parent;
			}
		} else {
			while (view && !found) {
				// Go through views - this one, and all parent ones - and return first one with given type.
				found = view.type === type ? view : undefined;
				view = view.parent;
			}
		}
		return found;
	}

	function getNestedIndex() {
		var view = this.get("item");
		return view ? view.index : undefined;
	}

	getNestedIndex.depends = function() {
		return [this.get("item"), "index"];
	};

	function getIndex() {
		return this.index;
	}

	getIndex.depends = function() {
		return ["index"];
	};

	//==========
	// View.hlp
	//==========

	function getHelper(helper) {
		// Helper method called as view.hlp(key) from compiled template, for helper functions or template parameters ~foo
		var wrapped,
			view = this,
			ctx = view.linkCtx,
			res = (view.ctx || {})[helper];

		if (res === undefined && ctx && ctx.ctx) {
			res = ctx.ctx[helper];
		}
		if (res === undefined) {
			res = $helpers[helper];
		}

		if (res) {
			if ($isFunction(res) && !res._wrp) {
				wrapped = function() {
					// If it is of type function, and not already wrapped, we will wrap it, so if called with no this pointer it will be called with the
					// view as 'this' context. If the helper ~foo() was in a data-link expression, the view will have a 'temporary' linkCtx property too.
					// Note that helper functions on deeper paths will have specific this pointers, from the preceding path.
					// For example, ~util.foo() will have the ~util object as 'this' pointer
					return res.apply((!this || this === global) ? view : this, arguments);
				};
				wrapped._wrp = 1;
				$extend(wrapped, res); // Attach same expandos (if any) to the wrapped function
			}
		}
		return wrapped || res;
	}

	//==============
	// jsviews._cnvt
	//==============

	function convertVal(converter, view, tagCtx) {
		// self is template object or linkCtx object
		var tag, value, prop,
			boundTagCtx = +tagCtx === tagCtx && tagCtx, // if tagCtx is an integer, then it is the key for the boundTagCtx (compiled function to return the tagCtx)
			linkCtx = view.linkCtx; // For data-link="{cvt:...}"...

		if (boundTagCtx) {
			// This is a bound tag: {^{xx:yyy}}. Call compiled function which returns the tagCtxs for current data
			tagCtx = (boundTagCtx = view.tmpl.bnds[boundTagCtx-1])(view.data, view, $views);
		}

		value = tagCtx.args[0];
		if (converter || boundTagCtx) {
			tag = linkCtx && linkCtx.tag;
			if (!tag) {
				tag = {
					_: {
						inline: !linkCtx,
						bnd: boundTagCtx
					},
					tagName: ":",
					cvt: converter,
					flow: true,
					tagCtx: tagCtx,
					_is: "tag"
				};
				if (linkCtx) {
					linkCtx.tag = tag;
					tag.linkCtx = linkCtx;
					tagCtx.ctx = extendCtx(tagCtx.ctx, linkCtx.view.ctx);
				}
				$sub._lnk(tag);
			}
			for (prop in tagCtx.props) {
				if (rHasHandlers.test(prop)) {
					tag[prop] = tagCtx.props[prop]; // Copy over the onFoo props from tagCtx.props to tag (overrides values in tagDef).
					// Note: unsupported scenario: if handlers are dynamically added ^onFoo=expression this will work, but dynamically removing will not work.
				}
			}

			tagCtx.view = view;

			tag.ctx = tagCtx.ctx || {};
			delete tagCtx.ctx;
			// Provide this tag on view, for addBindingMarkers on bound tags to add the tag to view._.bnds, associated with the tag id,
			view._.tag = tag;

			value = convertArgs(tag, tag.convert || converter !== "true" && converter)[0]; // If there is a convertBack but no convert, converter will be "true"

			// Call onRender (used by JsViews if present, to add binding annotations around rendered content)
			value = boundTagCtx && view._.onRender
				? view._.onRender(value, view, boundTagCtx)
				: value;
			view._.tag = undefined;
		}
		return value != undefined ? value : "";
	}

	function convertArgs(tag, converter) {
		var tagCtx = tag.tagCtx,
			view = tagCtx.view,
			args = tagCtx.args;

		converter = converter && ("" + converter === converter
			? (view.getRsc("converters", converter) || error("Unknown converter: '"+ converter + "'"))
			: converter);

		args = !args.length && !tagCtx.index // On the opening tag with no args, bind the the current data context
			? [view.data]
			: converter
				? args.slice() // If there is a converter, use a copy of the tagCtx.args array for rendering, and replace the args[0] in
				// the copied array with the converted value. But we do not modify the value of tag.tagCtx.args[0] (the original args array)
				: args; // If no converter, render with the original tagCtx.args

		if (converter) {
			if (converter.depends) {
				tag.depends = $sub.getDeps(tag.depends, tag, converter.depends, converter);
			}
			args[0] = converter.apply(tag, args);
		}
		return args;
	}

	//=============
	// jsviews._tag
	//=============

	function getResource(resourceType, itemName) {
		var res, store,
			view = this;
		while ((res === undefined) && view) {
			store = view.tmpl[resourceType];
			res = store && store[itemName];
			view = view.parent;
		}
		return res || $views[resourceType][itemName];
	}

	function renderTag(tagName, parentView, tmpl, tagCtxs, isUpdate) {
		// Called from within compiled template function, to render a template tag
		// Returns the rendered tag

		var render, tag, tags, attr, parentTag, i, l, itemRet, tagCtx, tagCtxCtx, content, boundTagFn, tagDef,
			callInit, mapDef, thisMap, args, prop, props, initialTmpl,
			ret = "",
			boundTagKey = +tagCtxs === tagCtxs && tagCtxs, // if tagCtxs is an integer, then it is the boundTagKey
			linkCtx = parentView.linkCtx || 0,
			ctx = parentView.ctx,
			parentTmpl = tmpl || parentView.tmpl;

		if (tagName._is === "tag") {
			tag = tagName;
			tagName = tag.tagName;
			tagCtxs = tag.tagCtxs;
		}
		tag = tag || linkCtx.tag;

		// Provide tagCtx, linkCtx and ctx access from tag
		if (boundTagKey) {
			// if tagCtxs is an integer, we are data binding
			// Call compiled function which returns the tagCtxs for current data
			tagCtxs = (boundTagFn = parentTmpl.bnds[boundTagKey-1])(parentView.data, parentView, $views);
		}

		l = tagCtxs.length;
		for (i = 0; i < l; i++) {
			if (!i && (!tmpl || !tag)) {
				tagDef = parentView.getRsc("tags", tagName) || error("Unknown tag: {{"+ tagName + "}}");
			}
			tagCtx = tagCtxs[i];
			if (!linkCtx.tag) {
				// We are initializing tag, so for block tags, tagCtx.tmpl is an integer > 0
				content = tagCtx.tmpl;
				content = tagCtx.content = content && parentTmpl.tmpls[content - 1];

				$extend(tagCtx, {
					tmpl: (tag ? tag : tagDef).template || content, // Set the tmpl property to the content of the block tag
					render: renderContent,
					// Possible future feature:
					//var updatedValueOfArg0 = this.tagCtx.get(0);
					//var test3 = this.tagCtx.get(0);
					//var updatedValueOfPropFoo = this.tagCtx.get("foo");
					//var updatedValueOfCtxPropFoo = this.tagCtx.get("~foo");
					//_fns: {},
 					//get: function(key) {
					//	return (this._fns[key] = this._fns[key] || new Function("data,view,j,u",
					//		"return " + $.views.sub.parse(this.params[+key === key ? "args" : (key.charAt(0) === "~" ? (key = key.slice(1), "ctx") : "props")][key]) + ";")
					//	)(this.view.data, this.view, $views);
					//},
					index: i,
					view: parentView,
					ctx: extendCtx(tagCtx.ctx, ctx) // Extend parentView.ctx
				}); // Extend parentView.ctx
			}
			if (tmpl = tagCtx.props.tmpl) {
				// If the tmpl property is overridden, set the value (when initializing, or, in case of binding: ^tmpl=..., when updating)
				tmpl = "" + tmpl === tmpl // if a string
					? parentView.getRsc("templates", tmpl) || $templates(tmpl)
					: tmpl;

				tagCtx.tmpl = tmpl;
			}

			if (!tag) {
				// This will only be hit for initial tagCtx (not for {{else}}) - if the tag instance does not exist yet
				// Instantiate tag if it does not yet exist
				if (tagDef._ctr) {
					// If the tag has not already been instantiated, we will create a new instance.
					// ~tag will access the tag, even within the rendering of the template content of this tag.
					// From child/descendant tags, can access using ~tag.parent, or ~parentTags.tagName
					tag = new tagDef._ctr();
					callInit = !!tag.init;

					// Set attr on linkCtx to ensure outputting to the correct target attribute.
					tag.attr = tag.attr || tagDef.attr || undefined;
					// Setting either linkCtx.attr or this.attr in the init() allows per-instance choice of target attrib.
				} else {
					// This is a simple tag declared as a function, or with init set to false. We won't instantiate a specific tag constructor - just a standard instance object.
					$sub._lnk(tag = {
						// tag instance object if no init constructor
						render: tagDef.render
					});
				}
				tag._ = {
					inline: !linkCtx
				};
				if (linkCtx) {
					// Set attr on linkCtx to ensure outputting to the correct target attribute.
					linkCtx.attr = tag.attr = linkCtx.attr || tag.attr;
					linkCtx.tag = tag;
					tag.linkCtx = linkCtx;
				}
				if (tag._.bnd = boundTagFn || linkCtx.fn) {
					// Bound if {^{tag...}} or data-link="{tag...}"
					tag._.arrVws = {};
				} else if (tag.dataBoundOnly) {
					error("{^{" + tagName + "}} tag must be data-bound");
				}
				tag.tagName = tagName;
				tag.parent = parentTag = ctx && ctx.tag;
				tag._is = "tag";
				tag._def = tagDef;
				tag.tagCtxs = tagCtxs;

				//TODO better perf for childTags() - keep child tag.tags array, (and remove child, when disposed)
				// tag.tags = [];
				// Provide this tag on view, for addBindingMarkers on bound tags to add the tag to view._.bnds, associated with the tag id,
			}
			if (!i) {
				for (prop in props = tagCtx.props) {
					if (rHasHandlers.test(prop)) {
						tag[prop] = props[prop]; // Copy over the onFoo or convert or convertBack props from tagCtx.props to tag (overrides values in tagDef).
					}
				}
			}
			tagCtx.tag = tag;
			if (tag.dataMap && tag.tagCtxs) {
				tagCtx.map = tag.tagCtxs[i].map; // Copy over the compiled map instance from the previous tagCtxs to the refreshed ones
			}
			if (!tag.flow) {
				tagCtxCtx = tagCtx.ctx = tagCtx.ctx || {};

				// tags hash: tag.ctx.tags, merged with parentView.ctx.tags,
				tags = tag.parents = tagCtxCtx.parentTags = ctx && extendCtx(tagCtxCtx.parentTags, ctx.parentTags) || {};
				if (parentTag) {
					tags[parentTag.tagName] = parentTag;
					//TODO better perf for childTags: parentTag.tags.push(tag);
				}
				tagCtxCtx.tag = tag;
			}
		}
		parentView._.tag = tag;
		tag.rendering = {}; // Provide object for state during render calls to tag and elses. (Used by {{if}} and {{for}}...)
		for (i = 0; i < l; i++) {
			tagCtx = tag.tagCtx = tag.tagCtxs[i];
			props = tagCtx.props;
			args = convertArgs(tag, tag.convert);

			if (mapDef = props.dataMap || tag.dataMap) {
				if (args.length || props.dataMap) {
					thisMap = tagCtx.map;
					if (!thisMap || thisMap.src !== args[0] || isUpdate) {
						if (thisMap && thisMap.src) {
							thisMap.unmap(); // only called if observable map - not when only used in JsRender, e.g. by {{props}}
						}
						thisMap = tagCtx.map = mapDef.map(args[0], props);
					}
					args = [thisMap.tgt];
				}
			}
			tag.ctx = tagCtx.ctx;

			if (!i && callInit) {
				initialTmpl = tag.template;
				tag.init(tagCtx, linkCtx, tag.ctx);
				callInit = undefined;
				if (tag.template !== initialTmpl) {
					tag._.tmpl = tag.template; // This will override the tag.template and also tagCtx.props.tmpl for all tagCtxs
				}
			}

			itemRet = undefined;
			render = tag.render;
			if (render = tag.render) {
				itemRet = render.apply(tag, args);
			}
			args = args.length ? args : [parentView]; // no arguments - get data context from view.
			itemRet = itemRet !== undefined
				? itemRet // Return result of render function unless it is undefined, in which case return rendered template
				: tagCtx.render(args[0], true) || (isUpdate ? undefined : "");
				// No return value from render, and no template/content tagCtx.render(...), so return undefined
			ret = ret ? ret + (itemRet || "") : itemRet; // If no rendered content, this will be undefined
		}

		delete tag.rendering;

		tag.tagCtx = tag.tagCtxs[0];
		tag.ctx= tag.tagCtx.ctx;

		if (tag._.inline && (attr = tag.attr) && attr !== htmlStr) {
			// inline tag with attr set to "text" will insert HTML-encoded content - as if it was element-based innerText
			ret = attr === "text"
				? $converters.html(ret)
				: "";
		}
		return boundTagKey && parentView._.onRender
			// Call onRender (used by JsViews if present, to add binding annotations around rendered content)
			? parentView._.onRender(ret, parentView, boundTagKey)
			: ret;
	}

	//=================
	// View constructor
	//=================

	function View(context, type, parentView, data, template, key, contentTmpl, onRender) {
		// Constructor for view object in view hierarchy. (Augmented by JsViews if JsViews is loaded)
		var views, parentView_, tag,
			self = this,
			isArray = type === "array",
			self_ = {
				key: 0,
				useKey: isArray ? 0 : 1,
				id: "" + viewId++,
				onRender: onRender,
				bnds: {}
			};

		self.data = data;
		self.tmpl = template,
		self.content = contentTmpl;
		self.views = isArray ? [] : {};
		self.parent = parentView;
		self.type = type;
		// If the data is an array, this is an 'array view' with a views array for each child 'item view'
		// If the data is not an array, this is an 'item view' with a views 'hash' object for any child nested views
		// ._.useKey is non zero if is not an 'array view' (owning a data array). Use this as next key for adding to child views hash
		self._ = self_;
		self.linked = !!onRender;
		if (parentView) {
			views = parentView.views;
			parentView_ = parentView._;
			if (parentView_.useKey) {
				// Parent is an 'item view'. Add this view to its views object
				// self._key = is the key in the parent view hash
				views[self_.key = "_" + parentView_.useKey++] = self;
				self.index = indexStr;
				self.getIndex = getNestedIndex;
				tag = parentView_.tag;
				self_.bnd = isArray && (!tag || !!tag._.bnd && tag); // For array views that are data bound for collection change events, set the
				// view._.bnd property to true for top-level link() or data-link="{for}", or to the tag instance for a data-bound tag, e.g. {^{for ...}}
			} else {
				// Parent is an 'array view'. Add this view to its views array
				views.splice(
					// self._.key = self.index - the index in the parent view array
					self_.key = self.index = key,
				0, self);
			}
			// If no context was passed in, use parent context
			// If context was passed in, it should have been merged already with parent context
			self.ctx = context || parentView.ctx;
		} else {
			self.ctx = context;
		}
	}

	View.prototype = {
		get: getView,
		getIndex: getIndex,
		getRsc: getResource,
		hlp: getHelper,
		_is: "view"
	};

	//=============
	// Registration
	//=============

	function compileChildResources(parentTmpl) {
		var storeName, resources, resourceName, resource, settings, compile, onStore;
		for (storeName in jsvStores) {
			settings = jsvStores[storeName];
			if ((compile = settings.compile) && (resources = parentTmpl[storeName + "s"])) {
				for (resourceName in resources) {
					// compile child resource declarations (templates, tags, tags["for"] or helpers)
					resource = resources[resourceName] = compile(resourceName, resources[resourceName], parentTmpl);
					if (resource && (onStore = $sub.onStore[storeName])) {
						// e.g. JsViews integration
						onStore(resourceName, resource, compile);
					}
				}
			}
		}
	}

	function compileTag(name, tagDef, parentTmpl) {
		var init, tmpl;
		if ($isFunction(tagDef)) {
			// Simple tag declared as function. No presenter instantation.
			tagDef = {
				depends: tagDef.depends,
				render: tagDef
			};
		} else {
			if (tagDef.baseTag) {
				tagDef.flow = !!tagDef.flow; // default to false even if baseTag has flow=true
				tagDef = $extend($extend({}, tagDef.baseTag), tagDef);
			}
			// Tag declared as object, used as the prototype for tag instantiation (control/presenter)
			if ((tmpl = tagDef.template) !== undefined) {
				tagDef.template = "" + tmpl === tmpl ? ($templates[tmpl] || $templates(tmpl)) : tmpl;
			}
			if (tagDef.init !== false) {
				// Set int: false on tagDef if you want to provide just a render method, or render and template, but no constuctor or prototype.
				// so equivalent to setting tag to render function, except you can also provide a template.
				init = tagDef._ctr = function() {};
				(init.prototype = tagDef).constructor = init;
			}
		}
		if (parentTmpl) {
			tagDef._parentTmpl = parentTmpl;
		}
		return tagDef;
	}

	function compileTmpl(name, tmpl, parentTmpl, options) {
		// tmpl is either a template object, a selector for a template script block, the name of a compiled template, or a template object

		//==== nested functions ====
		function tmplOrMarkupFromStr(value) {
			// If value is of type string - treat as selector, or name of compiled template
			// Return the template object, if already compiled, or the markup string

			if (("" + value === value) || value.nodeType > 0) {
				try {
					elem = value.nodeType > 0
					? value
					: !rTmplString.test(value)
					// If value is a string and does not contain HTML or tag content, then test as selector
						&& jQuery && jQuery(global.document).find(value)[0]; // TODO address case where DOM is not available
					// If selector is valid and returns at least one element, get first element
					// If invalid, jQuery will throw. We will stay with the original string.
				} catch (e) {}

				if (elem) {
					// Generally this is a script element.
					// However we allow it to be any element, so you can for example take the content of a div,
					// use it as a template, and replace it by the same content rendered against data.
					// e.g. for linking the content of a div to a container, and using the initial content as template:
					// $.link("#content", model, {tmpl: "#content"});

					value = $templates[name = name || elem.getAttribute(tmplAttr)];
					if (!value) {
						// Not already compiled and cached, so compile and cache the name
						// Create a name for compiled template if none provided
						name = name || "_" + autoTmplName++;
						elem.setAttribute(tmplAttr, name);
						// Use tmpl as options
						value = $templates[name] = compileTmpl(name, elem.innerHTML, parentTmpl, options);
					}
					elem = undefined;
				}
				return value;
			}
			// If value is not a string, return undefined
		}

		var tmplOrMarkup, elem;

		//==== Compile the template ====
		tmpl = tmpl || "";
		tmplOrMarkup = tmplOrMarkupFromStr(tmpl);

		// If options, then this was already compiled from a (script) element template declaration.
		// If not, then if tmpl is a template object, use it for options
		options = options || (tmpl.markup ? tmpl : {});
		options.tmplName = name;
		if (parentTmpl) {
			options._parentTmpl = parentTmpl;
		}
		// If tmpl is not a markup string or a selector string, then it must be a template object
		// In that case, get it from the markup property of the object
		if (!tmplOrMarkup && tmpl.markup && (tmplOrMarkup = tmplOrMarkupFromStr(tmpl.markup))) {
			if (tmplOrMarkup.fn && (tmplOrMarkup.debug !== tmpl.debug || tmplOrMarkup.allowCode !== tmpl.allowCode)) {
				// if the string references a compiled template object, but the debug or allowCode props are different, need to recompile
				tmplOrMarkup = tmplOrMarkup.markup;
			}
		}
		if (tmplOrMarkup !== undefined) {
			if (name && !parentTmpl) {
				$render[name] = function() {
					return tmpl.render.apply(tmpl, arguments);
				};
			}
			if (tmplOrMarkup.fn || tmpl.fn) {
				// tmpl is already compiled, so use it, or if different name is provided, clone it
				if (tmplOrMarkup.fn) {
					if (name && name !== tmplOrMarkup.tmplName) {
						tmpl = extendCtx(options, tmplOrMarkup);
					} else {
						tmpl = tmplOrMarkup;
					}
				}
			} else {
				// tmplOrMarkup is a markup string, not a compiled template
				// Create template object
				tmpl = TmplObject(tmplOrMarkup, options);
				// Compile to AST and then to compiled function
				tmplFn(tmplOrMarkup.replace(rEscapeQuotes, "\\$&"), tmpl);
			}
			compileChildResources(options);
			return tmpl;
		}
	}

	function dataMap(mapDef) {
		function newMap(source, options) {
			this.tgt = mapDef.getTgt(source, options);
		}

		if ($isFunction(mapDef)) {
			// Simple map declared as function
			mapDef = {
				getTgt: mapDef,
			};
		}

		if (mapDef.baseMap) {
			mapDef = $extend($extend({}, mapDef.baseMap), mapDef);
		}

		mapDef.map = function(source, options) {
			return new newMap(source, options);
		};
		return mapDef;
	}

	//==== /end of function compile ====

	function TmplObject(markup, options) {
		// Template object constructor
		var htmlTag,
			wrapMap = $viewsSettings.wrapMap || {},
			tmpl = $extend(
				{
					markup: markup,
					tmpls: [],
					links: {}, // Compiled functions for link expressions
					tags: {}, // Compiled functions for bound tag expressions
					bnds: [],
					_is: "template",
					render: fastRender
				},
				options
			);

		if (!options.htmlTag) {
			// Set tmpl.tag to the top-level HTML tag used in the template, if any...
			htmlTag = rFirstElem.exec(markup);
			tmpl.htmlTag = htmlTag ? htmlTag[1].toLowerCase() : "";
		}
		htmlTag = wrapMap[tmpl.htmlTag];
		if (htmlTag && htmlTag !== wrapMap.div) {
			// When using JsViews, we trim templates which are inserted into HTML contexts where text nodes are not rendered (i.e. not 'Phrasing Content').
			// Currently not trimmed for <li> tag. (Not worth adding perf cost)
			tmpl.markup = $.trim(tmpl.markup);
		}

		return tmpl;
	}

	function registerStore(storeName, storeSettings) {

		function theStore(name, item, parentTmpl) {
			// The store is also the function used to add items to the store. e.g. $.templates, or $.views.tags

			// For store of name 'thing', Call as:
			//    $.views.things(items[, parentTmpl]),
			// or $.views.things(name, item[, parentTmpl])

			var onStore, compile, itemName, thisStore;

			if (name && typeof name === "object" && !name.nodeType && !name.markup && !name.getTgt) {
				// Call to $.views.things(items[, parentTmpl]),

				// Adding items to the store
				// If name is a hash, then item is parentTmpl. Iterate over hash and call store for key.
				for (itemName in name) {
					theStore(itemName, name[itemName], item);
				}
				return $views;
			}
			// Adding a single unnamed item to the store
			if (item === undefined) {
				item = name;
				name = undefined;
			}
			if (name && "" + name !== name) { // name must be a string
				parentTmpl = item;
				item = name;
				name = undefined;
			}
			thisStore = parentTmpl ? parentTmpl[storeNames] = parentTmpl[storeNames] || {} : theStore;
			compile = storeSettings.compile;
			if (item === null) {
				// If item is null, delete this entry
				name && delete thisStore[name];
			} else {
				item = compile ? (item = compile(name, item, parentTmpl)) : item;
				name && (thisStore[name] = item);
			}
			if (compile && item) {
				item._is = storeName; // Only do this for compiled objects (tags, templates...)
			}
			if (item && (onStore = $sub.onStore[storeName])) {
				// e.g. JsViews integration
				onStore(name, item, compile);
			}
			return item;
		}

		var storeNames = storeName + "s";

		$views[storeNames] = theStore;
		jsvStores[storeName] = storeSettings;
	}

	//==============
	// renderContent
	//==============

	function $fastRender(data, context) {
		var tmplElem = this.jquery && (this[0] || error('Unknown template: "' + this.selector + '"')),
			tmpl = tmplElem.getAttribute(tmplAttr);

		return fastRender.call(tmpl ? $templates[tmpl] : $templates(tmplElem), data, context);
	}

	function tryFn(tmpl, data, view) {
		if ($viewsSettings._dbgMode) {
			try {
				return tmpl.fn(data, view, $views);
			}
			catch (e) {
				return error(e, view);
			}
		}
		return tmpl.fn(data, view, $views);
	}

	function fastRender(data, context, noIteration, parentView, key, onRender) {
		var self = this;
		if (!parentView && self.fn._nvw && !$.isArray(data)) {
			return tryFn(self, data, {tmpl: self});
		}
		return renderContent.call(self, data, context, noIteration, parentView, key, onRender);
	}

	function renderContent(data, context, noIteration, parentView, key, onRender) {
		// Render template against data as a tree of subviews (nested rendered template instances), or as a string (top-level template).
		// If the data is the parent view, treat as noIteration, re-render with the same data context.
		var i, l, dataItem, newView, childView, itemResult, swapContent, tagCtx, contentTmpl, tag_, outerOnRender, tmplName, tmpl, noViews,
			self = this,
			result = "";

		if (!!context === context) {
			noIteration = context; // passing boolean as second param - noIteration
			context = undefined;
		}

		if (key === true) {
			swapContent = true;
			key = 0;
		}

		if (self.tag) {
			// This is a call from renderTag or tagCtx.render(...)
			tagCtx = self;
			self = self.tag;
			tag_ = self._;
			tmplName = self.tagName;
			tmpl = tag_.tmpl || tagCtx.tmpl;
			noViews = self.attr && self.attr !== htmlStr,
			context = extendCtx(context, self.ctx);
			contentTmpl = tagCtx.content; // The wrapped content - to be added to views, below
			if (tagCtx.props.link === false) {
				// link=false setting on block tag
				// We will override inherited value of link by the explicit setting link=false taken from props
				// The child views of an unlinked view are also unlinked. So setting child back to true will not have any effect.
				context = context || {};
				context.link = false;
			}
			parentView = parentView || tagCtx.view;
			data = arguments.length ? data : parentView;
		} else {
			tmpl = self;
		}

		if (tmpl) {
			if (!parentView && data && data._is === "view") {
				parentView = data; // When passing in a view to render or link (and not passing in a parent view) use the passed in view as parentView
			}
			if (parentView) {
				contentTmpl = contentTmpl || parentView.content; // The wrapped content - to be added as #content property on views, below
				onRender = onRender || parentView._.onRender;
				if (data === parentView) {
					// Inherit the data from the parent view.
					// This may be the contents of an {{if}} block
					data = parentView.data;
				}
				context = extendCtx(context, parentView.ctx);
			}
			if (!parentView || parentView.data === undefined) {
				(context = context || {}).root = data; // Provide ~root as shortcut to top-level data.
			}

			// Set additional context on views created here, (as modified context inherited from the parent, and to be inherited by child views)
			// Note: If no jQuery, $extend does not support chained copies - so limit extend() to two parameters

			if (!tmpl.fn) {
				tmpl = $templates[tmpl] || $templates(tmpl);
			}

			if (tmpl) {
				onRender = (context && context.link) !== false && !noViews && onRender;
				// If link===false, do not call onRender, so no data-linking marker nodes
				outerOnRender = onRender;
				if (onRender === true) {
					// Used by view.refresh(). Don't create a new wrapper view.
					outerOnRender = undefined;
					onRender = parentView._.onRender;
				}
				context = tmpl.helpers
					? extendCtx(tmpl.helpers, context)
					: context;
				if ($.isArray(data) && !noIteration) {
					// Create a view for the array, whose child views correspond to each data item. (Note: if key and parentView are passed in
					// along with parent view, treat as insert -e.g. from view.addViews - so parentView is already the view item for array)
					newView = swapContent
						? parentView :
						(key !== undefined && parentView) || new View(context, "array", parentView, data, tmpl, key, contentTmpl, onRender);
					for (i = 0, l = data.length; i < l; i++) {
						// Create a view for each data item.
						dataItem = data[i];
						childView = new View(context, "item", newView, dataItem, tmpl, (key || 0) + i, contentTmpl, onRender);
						itemResult = tryFn(tmpl, dataItem, childView);
						result += newView._.onRender ? newView._.onRender(itemResult, childView) : itemResult;
					}
				} else {
					// Create a view for singleton data object. The type of the view will be the tag name, e.g. "if" or "myTag" except for
					// "item", "array" and "data" views. A "data" view is from programmatic render(object) against a 'singleton'.
					if (parentView || !tmpl.fn._nvw) {
						newView = swapContent ? parentView : new View(context, tmplName || "data", parentView, data, tmpl, key, contentTmpl, onRender);
						if (tag_ && !self.flow) {
							newView.tag = self;
						}
					}
					result += tryFn(tmpl, data, newView);
				}
				return outerOnRender ? outerOnRender(result, newView) : result;
			}
		}
		return "";
	}

	//===========================
	// Build and compile template
	//===========================

	// Generate a reusable function that will serve to render a template against data
	// (Compile AST then build template function)

	function error(e, view, fallback) {
		var message = $viewsSettings.onError(e, view, fallback);
		if ("" + e === e) { // if e is a string, not an Exception, then throw new Exception
			throw new $sub.Err(message);
		}
		return !view.linkCtx && view.linked ? $converters.html(message) : message;
	}

	function syntaxError(message) {
		error("Syntax error\n" + message);
	}

	function tmplFn(markup, tmpl, isLinkExpr, convertBack) {
		// Compile markup to AST (abtract syntax tree) then build the template function code from the AST nodes
		// Used for compiling templates, and also by JsViews to build functions for data link expressions

		//==== nested functions ====
		function pushprecedingContent(shift) {
			shift -= loc;
			if (shift) {
				content.push(markup.substr(loc, shift).replace(rNewLine, "\\n"));
			}
		}

		function blockTagCheck(tagName) {
			tagName && syntaxError('Unmatched or missing tag: "{{/' + tagName + '}}" in template:\n' + markup);
		}

		function parseTag(all, bind, tagName, converter, colon, html, comment, codeTag, params, slash, closeBlock, index) {

			//    bind         tag        converter colon html     comment            code      params            slash   closeBlock
			// /{(\^)?{(?:(?:(\w+(?=[\/\s}]))|(?:(\w+)?(:)|(>)|!--((?:[^-]|-(?!-))*)--|(\*)))\s*((?:[^}]|}(?!}))*?)(\/)?|(?:\/(\w+)))}}/g
			// Build abstract syntax tree (AST): [tagName, converter, params, content, hash, bindings, contentMarkup]
			if (html) {
				colon = ":";
				converter = htmlStr;
			}
			slash = slash || isLinkExpr;
			var pathBindings = (bind || isLinkExpr) && [],
				props = "",
				args = "",
				ctxProps = "",
				paramsArgs = "",
				paramsProps = "",
				paramsCtxProps = "",
				onError = "",
				useTrigger = "",
				// Block tag if not self-closing and not {{:}} or {{>}} (special case) and not a data-link expression
				block = !slash && !colon && !comment;

			//==== nested helper function ====
			tagName = tagName || (params = params || "#data", colon); // {{:}} is equivalent to {{:#data}}
			pushprecedingContent(index);
			loc = index + all.length; // location marker - parsed up to here
			if (codeTag) {
				if (allowCode) {
					content.push(["*", "\n" + params.replace(rUnescapeQuotes, "$1") + "\n"]);
				}
			} else if (tagName) {
				if (tagName === "else") {
					if (rTestElseIf.test(params)) {
						syntaxError('for "{{else if expr}}" use "{{else expr}}"');
					}
					pathBindings = current[7];
					current[8] = markup.substring(current[8], index); // contentMarkup for block tag
					current = stack.pop();
					content = current[2];
					block = true;
				}
				if (params) {
					// remove newlines from the params string, to avoid compiled code errors for unterminated strings
					parseParams(params.replace(rNewLine, " "), pathBindings, tmpl)
						.replace(rBuildHash, function(all, onerror, isCtx, key, keyToken, keyValue, arg, param) {
							if (arg) {
								args += keyValue + ",";
								paramsArgs += "'" + param + "',";
							} else if (isCtx) {
								ctxProps += key + keyValue + ",";
								paramsCtxProps += key + "'" + param + "',";
							} else if (onerror) {
								onError += keyValue;
							} else {
								if (keyToken === "trigger") {
									useTrigger += keyValue;
								}
								props += key + keyValue + ",";
								paramsProps += key + "'" + param + "',";
								hasHandlers = hasHandlers || rHasHandlers.test(keyToken);
							}
							return "";
						}).slice(0, -1);
				}

				newNode = [
						tagName,
						converter || !!convertBack || hasHandlers || "",
						block && [],
						parsedParam(paramsArgs, paramsProps, paramsCtxProps),
						parsedParam(args, props, ctxProps),
						onError,
						useTrigger,
						pathBindings || 0
					];
				content.push(newNode);
				if (block) {
					stack.push(current);
					current = newNode;
					current[8] = loc; // Store current location of open tag, to be able to add contentMarkup when we reach closing tag
				}
			} else if (closeBlock) {
				blockTagCheck(closeBlock !== current[0] && current[0] !== "else" && closeBlock);
				current[8] = markup.substring(current[8], index); // contentMarkup for block tag
				current = stack.pop();
			}
			blockTagCheck(!current && closeBlock);
			content = current[2];
		}
		//==== /end of nested functions ====

		var result, newNode, hasHandlers,
			allowCode = tmpl && tmpl.allowCode,
			astTop = [],
			loc = 0,
			stack = [],
			content = astTop,
			current = [,,astTop];

//TODO	result = tmplFnsCache[markup]; // Only cache if template is not named and markup length < ...,
//and there are no bindings or subtemplates?? Consider standard optimization for data-link="a.b.c"
//		if (result) {
//			tmpl.fn = result;
//		} else {

//		result = markup;
		if (isLinkExpr) {
			markup = delimOpenChar0 + markup + delimCloseChar1;
		}

		blockTagCheck(stack[0] && stack[0][2].pop()[0]);
		// Build the AST (abstract syntax tree) under astTop
		markup.replace(rTag, parseTag);

		pushprecedingContent(markup.length);

		if (loc = astTop[astTop.length - 1]) {
			blockTagCheck("" + loc !== loc && (+loc[8] === loc[8]) && loc[0]);
		}
//			result = tmplFnsCache[markup] = buildCode(astTop, tmpl);
//		}

		if (isLinkExpr) {
			result = buildCode(astTop, markup, isLinkExpr);
			result.paths = astTop[0][7]; // With data-link expressions, pathBindings array is astTop[0][7]
		} else {
			result = buildCode(astTop, tmpl);
		}
		if (result._nvw) {
			result._nvw = !/[~#]/.test(markup);
		}
		return result;
	}

	function parsedParam(args, props, ctx) {
		 return [args.slice(0, -1), props.slice(0, -1), ctx.slice(0, -1)];
	}

	function paramStructure(parts, type) {
		return '\n\t' + (type ? type + ':{' : '') + 'args:[' + parts[0] + ']' + (parts[1] || !type ? ',\n\tprops:{' + parts[1] + '}' : "") + (parts[2] ? ',\n\tctx:{' + parts[2] + '}' : "");
	}

	function parseParams(params, bindings, tmpl) {

		function parseTokens(all, lftPrn0, lftPrn, bound, path, operator, err, eq, path2, prn, comma, lftPrn2, apos, quot, rtPrn, rtPrnDot, prn2, space, index, full) {
			//rParams = /(\()(?=\s*\()|(?:([([])\s*)?(?:(\^?)(!*?[#~]?[\w$.^]+)?\s*((\+\+|--)|\+|-|&&|\|\||===|!==|==|!=|<=|>=|[<>%*:?\/]|(=))\s*|(!*?[#~]?[\w$.^]+)([([])?)|(,\s*)|(\(?)\\?(?:(')|("))|(?:\s*(([)\]])(?=\s*\.|\s*\^)|[)\]])([([]?))|(\s+)/g,
			//          lftPrn0        lftPrn        bound            path    operator err                                                eq             path2       prn    comma   lftPrn2   apos quot      rtPrn rtPrnDot                    prn2   space
			// (left paren? followed by (path? followed by operator) or (path followed by paren?)) or comma or apos or quot or right paren or space
			operator = operator || "";
			lftPrn = lftPrn || lftPrn0 || lftPrn2;
			path = path || path2;
			prn = prn || prn2 || "";

			var expr, isFn, exprFn;

			function parsePath(allPath, not, object, helper, view, viewProperty, pathTokens, leafToken) {
				// rPath = /^(?:null|true|false|\d[\d.]*|(!*?)([\w$]+|\.|~([\w$]+)|#(view|([\w$]+))?)([\w$.^]*?)(?:[.[^]([\w$]+)\]?)?)$/g,
				//                                        none   object     helper    view  viewProperty pathTokens      leafToken
				if (object) {
					if (bindings) {
						if (named === "linkTo") {
							bindto = bindings._jsvto = bindings._jsvto || [];
							bindto.push(path);
						}
						if (!named || boundName) {
							bindings.push(path.slice(not.length)); // Add path binding for paths on props and args,
						}
					}
					if (object !== ".") {
						var ret = (helper
								? 'view.hlp("' + helper + '")'
								: view
									? "view"
									: "data")
							+ (leafToken
								? (viewProperty
									? "." + viewProperty
									: helper
										? ""
										: (view ? "" : "." + object)
									) + (pathTokens || "")
								: (leafToken = helper ? "" : view ? viewProperty || "" : object, ""));

						ret = ret + (leafToken ? "." + leafToken : "");

						return not + (ret.slice(0, 9) === "view.data"
							? ret.slice(5) // convert #view.data... to data...
							: ret);
					}
				}
				return allPath;
			}

			if (err && !aposed && !quoted) {
				syntaxError(params);
			} else {
				if (bindings && rtPrnDot && !aposed && !quoted) {
					// This is a binding to a path in which an object is returned by a helper/data function/expression, e.g. foo()^x.y or (a?b:c)^x.y
					// We create a compiled function to get the object instance (which will be called when the dependent data of the subexpression changes, to return the new object, and trigger re-binding of the subsequent path)
					if (!named || boundName || bindto) {
						expr = pathStart[parenDepth];
						if (full.length - 1 > index - expr) { // We need to compile a subexpression
							expr = full.slice(expr, index + 1);
							rtPrnDot = delimOpenChar1 + ":" + expr + delimCloseChar0; // The parameter or function subexpression
							exprFn = tmplLinks[rtPrnDot];
							if (!exprFn) {
								tmplLinks[rtPrnDot] = 1; // Flag that this exprFn (for rtPrnDot) is being compiled
								tmplLinks[rtPrnDot] = exprFn = tmplFn(rtPrnDot, tmpl || bindings, true); // Compile the expression (or use cached copy already in tmpl.links)
								exprFn.paths.push({_jsvOb: exprFn}); //list.push({_jsvOb: rtPrnDot});
							}
							if (exprFn !== 1) { // If not reentrant call during compilation
								(bindto || bindings).push({_jsvOb: exprFn}); // Insert special object for in path bindings, to be used for binding the compiled sub expression ()
							}
						}
					}
				}
				return (aposed
					// within single-quoted string
					? (aposed = !apos, (aposed ? all : '"'))
					: quoted
					// within double-quoted string
						? (quoted = !quot, (quoted ? all : '"'))
						:
					(
						(lftPrn
								? (parenDepth++, pathStart[parenDepth] = index++, lftPrn)
								: "")
						+ (space
							? (parenDepth
								? ""
								: (paramIndex = full.slice(paramIndex, index), named
									? (named = boundName = bindto = false, "\b")
									: "\b,") + paramIndex + (paramIndex = index + all.length, "\b")
							)
							: eq
					// named param
					// Insert backspace \b (\x08) as separator for named params, used subsequently by rBuildHash
								? (parenDepth && syntaxError(params), named = path, boundName = bound, paramIndex = index + all.length, /*pushBindings(),*/ path + ':')
								: path
					// path
									? (path.split("^").join(".").replace(rPath, parsePath)
										+ (prn
											? (fnCall[++parenDepth] = true, path.charAt(0) !== "." && (pathStart[parenDepth] = index), isFn ? "" : prn)
											: operator)
									)
									: operator
										? operator
										: rtPrn
					// function
											? ((fnCall[parenDepth--] = false, rtPrn)
												+ (prn
													? (fnCall[++parenDepth] = true, prn)
													: "")
											)
											: comma
												? (fnCall[parenDepth] || syntaxError(params), ",") // We don't allow top-level literal arrays or objects
												: lftPrn0
													? ""
													: (aposed = apos, quoted = quot, '"')
					))
				);
			}
		}
		var named, bindto, boundName,
			quoted, // boolean for string content in double quotes
			aposed, // or in single quotes
			paramIndex = 0, // list,
			tmplLinks = tmpl ? tmpl.links : bindings && (bindings.links = bindings.links || {}),
			fnCall = {},
			pathStart = {0:-1},
			parenDepth = 0;

		//pushBindings();
		return (params + (tmpl ? " " : ""))
			.replace(/\)\^/g, ").") // Treat "...foo()^bar..." as equivalent to "...foo().bar..."
								//since preceding computed observables in the path will always be updated if their dependencies change
			.replace(rParams, parseTokens);
	}

	function buildCode(ast, tmpl, isLinkExpr) {
		// Build the template function code from the AST nodes, and set as property on the passed-in template object
		// Used for compiling templates, and also by JsViews to build functions for data link expressions
		var i, node, tagName, converter, tagCtx, hasTag, hasEncoder, getsVal, hasCnvt, needView, useCnvt, tmplBindings, pathBindings, params,
			nestedTmpls, tmplName, nestedTmpl, tagAndElses, content, markup, nextIsElse, oldCode, isElse, isGetVal, tagCtxFn, onError, tagStart, trigger,
			tmplBindingKey = 0,
			code = "",
			tmplOptions = {},
			l = ast.length;

		if ("" + tmpl === tmpl) {
			tmplName = isLinkExpr ? 'data-link="' + tmpl.replace(rNewLine, " ").slice(1, -1) + '"' : tmpl;
			tmpl = 0;
		} else {
			tmplName = tmpl.tmplName || "unnamed";
			if (tmpl.allowCode) {
				tmplOptions.allowCode = true;
			}
			if (tmpl.debug) {
				tmplOptions.debug = true;
			}
			tmplBindings = tmpl.bnds;
			nestedTmpls = tmpl.tmpls;
		}
		for (i = 0; i < l; i++) {
			// AST nodes: [tagName, converter, content, params, code, onError, pathBindings, contentMarkup, link]
			node = ast[i];

			// Add newline for each callout to t() c() etc. and each markup string
			if ("" + node === node) {
				// a markup string to be inserted
				code += '\n+"' + node + '"';
			} else {
				// a compiled tag expression to be inserted
				tagName = node[0];
				if (tagName === "*") {
					// Code tag: {{* }}
					code += ";\n" + node[1] + "\nret=ret";
				} else {
					converter = node[1];
					content = node[2];
					tagCtx = paramStructure(node[3], 'params') + '},' + paramStructure(params = node[4]);
					onError = node[5];
					trigger = node[6];
					markup = node[8];
					if (!(isElse = tagName === "else")) {
						tmplBindingKey = 0;
						if (tmplBindings && (pathBindings = node[7])) { // Array of paths, or false if not data-bound
							tmplBindingKey = tmplBindings.push(pathBindings);
						}
					}
					if (isGetVal = tagName === ":") {
						if (converter) {
							tagName = converter === htmlStr ? ">" : converter + tagName;
						}
					} else {
						if (content) {
							// Create template object for nested template
							nestedTmpl = TmplObject(markup, tmplOptions);
							nestedTmpl.tmplName = tmplName + "/" + tagName;
							// Compile to AST and then to compiled function
							buildCode(content, nestedTmpl);
							nestedTmpls.push(nestedTmpl);
						}

						if (!isElse) {
							// This is not an else tag.
							tagAndElses = tagName;
							// Switch to a new code string for this bound tag (and its elses, if it has any) - for returning the tagCtxs array
							oldCode = code;
							code = "";
						}
						nextIsElse = ast[i + 1];
						nextIsElse = nextIsElse && nextIsElse[0] === "else";
					}
					tagStart = (onError ? ";\ntry{\nret+=" : "\n+");

					if (isGetVal && (pathBindings || trigger || converter && converter !== htmlStr)) {
						// For convertVal we need a compiled function to return the new tagCtx(s)
						tagCtxFn = "return {" + tagCtx + "};";
						if (onError) {
							tagCtxFn = "try {\n" + tagCtxFn + '\n}catch(e){return {error: j._err(e,view,' + onError + ')}}\n';
						}
						tagCtxFn = new Function("data,view,j,u", " // " + tmplName + " " + tmplBindingKey + " " + tagName
											+ "\n" + tagCtxFn);

						tagCtxFn.paths = pathBindings;
						tagCtxFn._tag = tagName;
						if (isLinkExpr) {
							return tagCtxFn;
						}
						useCnvt = 1;
					}
					code += (isGetVal
						? (isLinkExpr ? (onError ? "\ntry{\n" : "") + "return " : tagStart) + (useCnvt // Call _cnvt if there is a converter: {{cnvt: ... }} or {^{cnvt: ... }}
							? (useCnvt = 0, needView = hasCnvt = true, 'c("' + converter + '",view,' + (pathBindings
								? ((tmplBindings[tmplBindingKey - 1] = tagCtxFn), tmplBindingKey) // Store the compiled tagCtxFn in tmpl.bnds, and pass the key to convertVal()
								: "{" + tagCtx + "}") + ")")
							: tagName === ">"
								? (hasEncoder = true, "h(" + params[0] + ')')
								: (getsVal = true, "((v=" + params[0] + ')!=null?v:"")') // Strict equality just for data-link="title{:expr}" so expr=null will remove title attribute
						)
						: (needView = hasTag = true, "\n{view:view,tmpl:" // Add this tagCtx to the compiled code for the tagCtxs to be passed to renderTag()
							+ (content ? nestedTmpls.length : "0") + "," // For block tags, pass in the key (nestedTmpls.length) to the nested content template
							+ tagCtx + "},"));

					if (tagAndElses && !nextIsElse) {
						code = "[" + code.slice(0, -1) + "]"; // This is a data-link expression or the last {{else}} of an inline bound tag. We complete the code for returning the tagCtxs array
						if (isLinkExpr || pathBindings) {
							// This is a bound tag (data-link expression or inline bound tag {^{tag ...}}) so we store a compiled tagCtxs function in tmp.bnds
							code = new Function("data,view,j,u", " // " + tmplName + " " + tmplBindingKey + " " + tagAndElses + "\nreturn " + code + ";");
							if (pathBindings) {
								(tmplBindings[tmplBindingKey - 1] = code).paths = pathBindings;
							}
							code._tag = tagName;
							if (isLinkExpr) {
								return code; // For a data-link expression we return the compiled tagCtxs function
							}
						}

						// This is the last {{else}} for an inline tag.
						// For a bound tag, pass the tagCtxs fn lookup key to renderTag.
						// For an unbound tag, include the code directly for evaluating tagCtxs array
						code = oldCode + tagStart + 't("' + tagAndElses + '",view,this,' + (tmplBindingKey || code) + ")";
						pathBindings = 0;
						tagAndElses = 0;
					}
					if (onError) {
						needView = true;
						code += ';\n}catch(e){ret' + (isLinkExpr ? "urn " : "+=") + 'j._err(e,view,' + onError + ');}\n' + (isLinkExpr ? "" : 'ret=ret');
					}
				}
			}
		}
		// Include only the var references that are needed in the code
		code = "// " + tmplName

			+ "\nj=j||" + (jQuery ? "jQuery." : "jsviews.") + "views;var v"
			+ (hasTag ? ",t=j._tag" : "")                // has tag
			+ (hasCnvt ? ",c=j._cnvt" : "")              // converter
			+ (hasEncoder ? ",h=j.converters.html":"") // html converter
			+ (isLinkExpr ? ";\n" : ',ret=""\n')
			+ (tmplOptions.debug ? "debugger;" : "")
			+ code
			+ (isLinkExpr ? "\n" : ";\nreturn ret;");
		try {
			code = new Function("data,view,j,u", code);
		} catch (e) {
			syntaxError("Compiled template code:\n\n" + code + '\n: "' + e.message + '"');
		}
		if (tmpl) {
			tmpl.fn = code;
		}
		if (!needView) {
			code._nvw = true;
		}
		return code;
	}

	//==========
	// Utilities
	//==========

	// Merge objects, in particular contexts which inherit from parent contexts
	function extendCtx(context, parentContext) {
		// Return copy of parentContext, unless context is defined and is different, in which case return a new merged context
		// If neither context nor parentContext are defined, return undefined
		return context && context !== parentContext
			? (parentContext
				? $extend($extend({}, parentContext), context)
				: context)
			: parentContext && $extend({}, parentContext);
	}

	// Get character entity for HTML and Attribute encoding
	function getCharEntity(ch) {
		return charEntities[ch] || (charEntities[ch] = "&#" + ch.charCodeAt(0) + ";");
	}

	//========================== Initialize ==========================

	for (jsvStoreName in jsvStores) {
		registerStore(jsvStoreName, jsvStores[jsvStoreName]);
	}

	var $templates = $views.templates,
		$converters = $views.converters,
		$helpers = $views.helpers,
		$tags = $views.tags,
		$sub = $views.sub,
		$viewsSettings = $views.settings;

	if (jQuery) {
		////////////////////////////////////////////////////////////////////////////////////////////////
		// jQuery is loaded, so make $ the jQuery object
		$ = jQuery;
		$.fn.render = $fastRender;
		if ($.observable) {
			$extend($sub, $.views.sub); // jquery.observable.js was loaded before jsrender.js
			$views.map = $.views.map;
		}
	} else {
		////////////////////////////////////////////////////////////////////////////////////////////////
		// jQuery is not loaded.

		$ = global.jsviews = {};

		$.isArray = Array && Array.isArray || function(obj) {
			return Object.prototype.toString.call(obj) === "[object Array]";
		};

	//	//========================== Future Node.js support ==========================
	//	if ((nodeJsModule = global.module) && nodeJsModule.exports) {
	//		nodeJsModule.exports = $;
	//	}
	}

	$.render = $render;
	$.views = $views;
	$.templates = $templates = $views.templates;

	$viewsSettings({
		debugMode: dbgMode,
		delimiters: $viewsDelimiters,
		onError: function(e, view, fallback) {
			// Can override using $.views.settings({onError: function(...) {...}});
			if (view) {
				// For render errors, e is an exception thrown in compiled template, and view is the current view. For other errors, e is an error string.
				e = fallback === undefined
					? "{Error: " + e + "}"
					: $isFunction(fallback)
						? fallback(e, view) : fallback;
			}
			return e == undefined ? "" : e;
		},
		_dbgMode: true
	});

	//========================== Register tags ==========================

	$tags({
		"else": function() {}, // Does nothing but ensures {{else}} tags are recognized as valid
		"if": {
			render: function(val) {
				// This function is called once for {{if}} and once for each {{else}}.
				// We will use the tag.rendering object for carrying rendering state across the calls.
				// If not done (a previous block has not been rendered), look at expression for this block and render the block if expression is truthy
				// Otherwise return ""
				var self = this,
					ret = (self.rendering.done || !val && (arguments.length || !self.tagCtx.index))
						? ""
						: (self.rendering.done = true, self.selected = self.tagCtx.index,
							// Test is satisfied, so render content on current context. We call tagCtx.render() rather than return undefined
							// (which would also render the tmpl/content on the current context but would iterate if it is an array)
							self.tagCtx.render(self.tagCtx.view, true)); // no arg, so renders against parentView.data
				return ret;
			},
			onUpdate: function(ev, eventArgs, tagCtxs) {
				var tci, prevArg, different;
				for (tci = 0; (prevArg = this.tagCtxs[tci]) && prevArg.args.length; tci++) {
					prevArg = prevArg.args[0];
					different = !prevArg !== !tagCtxs[tci].args[0];
					if ((!this.convert && !!prevArg) || different) {
						return different;
						// If there is no converter, and newArg and prevArg are both truthy, return false to cancel update. (Even if values on later elses are different, we still don't want to update, since rendered output would be unchanged)
						// If newArg and prevArg are different, return true, to update
						// If newArg and prevArg are both falsey, move to the next {{else ...}}
					}
				}
				// Boolean value of all args are unchanged (falsey), so return false to cancel update
				return false;
			},
			flow: true
		},
		"for": {
			render: function(val) {
				// This function is called once for {{for}} and once for each {{else}}.
				// We will use the tag.rendering object for carrying rendering state across the calls.
				var finalElse,
					self = this,
					tagCtx = self.tagCtx,
					result = "",
					done = 0;

				if (!self.rendering.done) {
					if (finalElse = !arguments.length) {
						val = tagCtx.view.data; // For the final else, defaults to current data without iteration.
					}
					if (val !== undefined) {
						result += tagCtx.render(val, finalElse); // Iterates except on final else, if data is an array. (Use {{include}} to compose templates without array iteration)
						done += $.isArray(val) ? val.length : 1;
					}
					if (self.rendering.done = done) {
						self.selected = tagCtx.index;
					}
					// If nothing was rendered we will look at the next {{else}}. Otherwise, we are done.
				}
				return result;
			},
			flow: true
		},
		include: {
			flow: true
		},
		"*": {
			// {{* code... }} - Ignored if template.allowCode is false. Otherwise include code in compiled template
			render: retVal,
			flow: true
		}
	});

	function getTargetProps(source) {
		// this pointer is theMap - which has tagCtx.props too
		// arguments: tagCtx.args.
		var key, prop,
			props = [];

		if (typeof source === "object") {
			for (key in source) {
				prop = source[key];
				if (!prop || !prop.toJSON || prop.toJSON()) {
					if (!$isFunction(prop)) {
						props.push({ key: key, prop: prop });
					}
				}
			}
		}
		return props;
	}

	$tags("props", {
		baseTag: $tags["for"],
		dataMap: dataMap(getTargetProps)
	});

	//========================== Register converters ==========================

	function htmlEncode(text) {
		// HTML encode: Replace < > & ' and " by corresponding entities.
		return text != null ? rIsHtml.test(text) && ("" + text).replace(rHtmlEncode, getCharEntity) || text : "";
	}

	$converters({
		html: htmlEncode,
		attr: htmlEncode, // Includes > encoding since rConvertMarkers in JsViews does not skip > characters in attribute strings
		url: function(text) {
			// URL encoding helper.
			return text != undefined ? encodeURI("" + text) : text === null ? text : ""; // null returns null, e.g. to remove attribute. undefined returns ""
		}
	});

	//========================== Define default delimiters ==========================
	$viewsDelimiters();

})(this, this.jQuery);

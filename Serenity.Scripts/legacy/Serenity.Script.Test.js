(function() {
	'use strict';
	var $asm = {};
	global.EditorTypeRegistryTestNamespace = global.EditorTypeRegistryTestNamespace || {};
	global.FormatterTypeRegistryTestNamespace = global.FormatterTypeRegistryTestNamespace || {};
	global.Serenity = global.Serenity || {};
	global.Serenity.Test = global.Serenity.Test || {};
	ss.initAssembly($asm, 'Serenity.Script.Test');
	////////////////////////////////////////////////////////////////////////////////
	// EditorTypeRegistryTestNamespace.DummyEditor
	var $EditorTypeRegistryTestNamespace_DummyEditor = function(element) {
		Serenity.Widget.call(this, element, null);
	};
	$EditorTypeRegistryTestNamespace_DummyEditor.__typeName = 'EditorTypeRegistryTestNamespace.DummyEditor';
	global.EditorTypeRegistryTestNamespace.DummyEditor = $EditorTypeRegistryTestNamespace_DummyEditor;
	////////////////////////////////////////////////////////////////////////////////
	// FormatterTypeRegistryTestNamespace.DummyFormatter
	var $FormatterTypeRegistryTestNamespace_DummyFormatter = function() {
	};
	$FormatterTypeRegistryTestNamespace_DummyFormatter.__typeName = 'FormatterTypeRegistryTestNamespace.DummyFormatter';
	global.FormatterTypeRegistryTestNamespace.DummyFormatter = $FormatterTypeRegistryTestNamespace_DummyFormatter;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.ScriptContextTests.SubClass
	var $Serenity_$Test_ScriptContextTests$SubClass = function() {
	};
	$Serenity_$Test_ScriptContextTests$SubClass.__typeName = 'Serenity.$Test.ScriptContextTests$SubClass';
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.EditorTypeRegistryTests
	var $Serenity_Test_EditorTypeRegistryTests = function() {
	};
	$Serenity_Test_EditorTypeRegistryTests.__typeName = 'Serenity.Test.EditorTypeRegistryTests';
	global.Serenity.Test.EditorTypeRegistryTests = $Serenity_Test_EditorTypeRegistryTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.EmailEditorTests
	var $Serenity_Test_EmailEditorTests = function() {
	};
	$Serenity_Test_EmailEditorTests.__typeName = 'Serenity.Test.EmailEditorTests';
	$Serenity_Test_EmailEditorTests.$createValidatedForm = function() {
		var div = Q.newBodyDiv();
		var form = $('<form/>').appendTo(div);
		form.validate(Q.validateOptions({}));
		return form;
	};
	$Serenity_Test_EmailEditorTests.$createDummyValidator = function() {
		return {
			optional: function() {
				return null;
			}
		};
	};
	global.Serenity.Test.EmailEditorTests = $Serenity_Test_EmailEditorTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.FormatterTypeRegistryTests
	var $Serenity_Test_FormatterTypeRegistryTests = function() {
	};
	$Serenity_Test_FormatterTypeRegistryTests.__typeName = 'Serenity.Test.FormatterTypeRegistryTests';
	global.Serenity.Test.FormatterTypeRegistryTests = $Serenity_Test_FormatterTypeRegistryTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.JsonCriteriaConverterSerializationTests
	var $Serenity_Test_JsonCriteriaConverterSerializationTests = function() {
	};
	$Serenity_Test_JsonCriteriaConverterSerializationTests.__typeName = 'Serenity.Test.JsonCriteriaConverterSerializationTests';
	global.Serenity.Test.JsonCriteriaConverterSerializationTests = $Serenity_Test_JsonCriteriaConverterSerializationTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QBlockUITests
	var $Serenity_Test_QBlockUITests = function() {
	};
	$Serenity_Test_QBlockUITests.__typeName = 'Serenity.Test.QBlockUITests';
	global.Serenity.Test.QBlockUITests = $Serenity_Test_QBlockUITests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QConfigTests
	var $Serenity_Test_QConfigTests = function() {
	};
	$Serenity_Test_QConfigTests.__typeName = 'Serenity.Test.QConfigTests';
	global.Serenity.Test.QConfigTests = $Serenity_Test_QConfigTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QCultureTests
	var $Serenity_Test_QCultureTests = function() {
	};
	$Serenity_Test_QCultureTests.__typeName = 'Serenity.Test.QCultureTests';
	global.Serenity.Test.QCultureTests = $Serenity_Test_QCultureTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QDateTests
	var $Serenity_Test_QDateTests = function() {
	};
	$Serenity_Test_QDateTests.__typeName = 'Serenity.Test.QDateTests';
	global.Serenity.Test.QDateTests = $Serenity_Test_QDateTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QDialogTests
	var $Serenity_Test_QDialogTests = function() {
	};
	$Serenity_Test_QDialogTests.__typeName = 'Serenity.Test.QDialogTests';
	global.Serenity.Test.QDialogTests = $Serenity_Test_QDialogTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QExternalsTests
	var $Serenity_Test_QExternalsTests = function() {
	};
	$Serenity_Test_QExternalsTests.__typeName = 'Serenity.Test.QExternalsTests';
	global.Serenity.Test.QExternalsTests = $Serenity_Test_QExternalsTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QHtmlTests
	var $Serenity_Test_QHtmlTests = function() {
	};
	$Serenity_Test_QHtmlTests.__typeName = 'Serenity.Test.QHtmlTests';
	global.Serenity.Test.QHtmlTests = $Serenity_Test_QHtmlTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QMethodTests
	var $Serenity_Test_QMethodTests = function() {
	};
	$Serenity_Test_QMethodTests.__typeName = 'Serenity.Test.QMethodTests';
	global.Serenity.Test.QMethodTests = $Serenity_Test_QMethodTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.QNumberTests
	var $Serenity_Test_QNumberTests = function() {
	};
	$Serenity_Test_QNumberTests.__typeName = 'Serenity.Test.QNumberTests';
	global.Serenity.Test.QNumberTests = $Serenity_Test_QNumberTests;
	////////////////////////////////////////////////////////////////////////////////
	// Serenity.Test.ScriptContextTests
	var $Serenity_Test_ScriptContextTests = function() {
	};
	$Serenity_Test_ScriptContextTests.__typeName = 'Serenity.Test.ScriptContextTests';
	global.Serenity.Test.ScriptContextTests = $Serenity_Test_ScriptContextTests;
	ss.initClass($EditorTypeRegistryTestNamespace_DummyEditor, $asm, {}, Serenity.Widget);
	ss.initClass($FormatterTypeRegistryTestNamespace_DummyFormatter, $asm, {
		format: function(ctx) {
			throw new ss.NotImplementedException();
		}
	}, null, [Serenity.ISlickFormatter]);
	ss.initClass($Serenity_$Test_ScriptContextTests$SubClass, $asm, {
		$select: function(x) {
			return $(x);
		},
		$select$1: function(x, y) {
			return $(x, y);
		}
	});
	ss.initClass($Serenity_Test_EditorTypeRegistryTests, $asm, {
		runTests: function() {
			test('EditorTypeRegistry_CanLocateSerenityEditors', ss.mkdel(this, function() {
				strictEqual(Serenity.StringEditor, Serenity.EditorTypeRegistry.get('String'), 'shortest');
				strictEqual(Serenity.StringEditor, Serenity.EditorTypeRegistry.get('StringEditor'), 'with editor suffix');
				strictEqual(Serenity.StringEditor, Serenity.EditorTypeRegistry.get('Serenity.String'), 'with namespace no suffix');
				strictEqual(Serenity.StringEditor, Serenity.EditorTypeRegistry.get('Serenity.StringEditor'), 'with namespace and suffix');
			}));
			test('EditorTypeRegistry_CanLocateDummyEditor', ss.mkdel(this, function() {
				strictEqual($EditorTypeRegistryTestNamespace_DummyEditor, Serenity.EditorTypeRegistry.get('SomeOtherKeyForDummyEditor'), 'with editor key');
				strictEqual($EditorTypeRegistryTestNamespace_DummyEditor, Serenity.EditorTypeRegistry.get('EditorTypeRegistryTestNamespace.Dummy'), 'with namespace and no suffix');
				strictEqual($EditorTypeRegistryTestNamespace_DummyEditor, Serenity.EditorTypeRegistry.get('EditorTypeRegistryTestNamespace.DummyEditor'), 'with namespace and suffix');
				throws(function() {
					Serenity.EditorTypeRegistry.get('DummyEditor');
				}, function() {
					return ss.isInstanceOfType(arguments[0], ss.Exception);
				}, "can't find if no root namespace");
				Q.Config.rootNamespaces.push('EditorTypeRegistryTestNamespace');
				try {
					Serenity.EditorTypeRegistry.reset();
					strictEqual($EditorTypeRegistryTestNamespace_DummyEditor, Serenity.EditorTypeRegistry.get('DummyEditor'), 'can find if root namespace and suffix');
					strictEqual($EditorTypeRegistryTestNamespace_DummyEditor, Serenity.EditorTypeRegistry.get('Dummy'), 'can find if root namespace and no suffix');
				}
				finally {
					ss.remove(Q.Config.rootNamespaces, 'EditorTypeRegistryTestNamespace');
					Serenity.EditorTypeRegistry.reset();
				}
			}));
		}
	});
	ss.initClass($Serenity_Test_EmailEditorTests, $asm, {
		runTests: function() {
			test('EmailValidationClassWorks', ss.mkdel(this, function() {
				var form = $Serenity_Test_EmailEditorTests.$createValidatedForm();
				var validator = form.validate();
				var input = $("<input type='text'/>").attr('name', 'dummy').addClass('email').appendTo(form);
				input.val('valid.email@address.com');
				ok(validator.element(input[0]));
				input.val('invalid.email@');
				ok(!validator.element(input[0]));
				form.parent().remove();
			}));
			test('EmailValidationMethodWorks', ss.mkdel(this, function() {
				var oldAscii = Q.Config.emailAllowOnlyAscii;
				try {
					var form = $Serenity_Test_EmailEditorTests.$createValidatedForm();
					var validator = form.validate();
					var input = $("<input type='text'/>").attr('name', 'dummy').appendTo(form);
					var jq = $;
					var $t1 = [false, true];
					for (var $t2 = 0; $t2 < $t1.length; $t2++) {
						var onlyAscii = $t1[$t2];
						Q.Config.emailAllowOnlyAscii = onlyAscii;
						ok(!!jq.validator.methods.email.call(validator, 'x@y.com', input.val('x@y.com')[0]));
						ok(!!jq.validator.methods.email.call(validator, 'some.user@somedomain.com', input.val('some.user@somedomain.com')[0]));
						ok(!!jq.validator.methods.email.call(validator, 'some_user@some.domain.com.tr', input.val('some_user@some.domain.com.tr')[0]));
						ok(!!!jq.validator.methods.email.call(validator, 'abcdef', input.val('abcdef')[0]));
						ok(!!!jq.validator.methods.email.call(validator, 'xyz@', input.val('xyz@')[0]));
						ok(!!!jq.validator.methods.email.call(validator, '@xyz', input.val('@xyz')[0]));
						ok(!!!jq.validator.methods.email.call(validator, '@xyz.com', input.val('@xyz.com')[0]));
						if (onlyAscii) {
							deepEqual(false, jq.validator.methods.email.call(validator, 'êığş@ädomaın.com', input.val('êığş@ädomaın.com')[0]));
						}
					}
					form.parent().remove();
				}
				finally {
					Q.Config.emailAllowOnlyAscii = oldAscii;
				}
			}));
			test('EmailUserValidationMethodWorks', ss.mkdel(this, function() {
				var oldAscii = Q.Config.emailAllowOnlyAscii;
				try {
					var form = $Serenity_Test_EmailEditorTests.$createValidatedForm();
					var validator = form.validate();
					var user = $("<input type='text' class='emailuser'/>").appendTo(form);
					var domain = $("<input type='text' class='emaildomain'/>").appendTo(form);
					var jq = $;
					Serenity.EmailEditor.registerValidationMethods();
					var $t1 = [false, true];
					for (var $t2 = 0; $t2 < $t1.length; $t2++) {
						var domainReadOnly = $t1[$t2];
						var $t3 = [false, true];
						for (var $t4 = 0; $t4 < $t3.length; $t4++) {
							var onlyAscii = $t3[$t4];
							var title = (domainReadOnly ? 'domainreadonly_' : '');
							title += (onlyAscii ? 'ascii_' : 'unicode_');
							if (domainReadOnly) {
								domain.attr('readonly', 'readonly');
							}
							else {
								domain.removeAttr('readonly');
							}
							domain.val((onlyAscii ? 'ascii.domain.com' : 'unıcõde.domaîn.com'));
							Q.Config.emailAllowOnlyAscii = onlyAscii;
							ok(!!jq.validator.methods.emailuser.call(validator, 'x', user.val('x')[0]), title + '1');
							ok(!!jq.validator.methods.emailuser.call(validator, 'some.user', user.val('some.user')[0]), title + '2');
							domain.val('');
							ok(!!!jq.validator.methods.emailuser.call(validator, ' ', user.val(' ')[0]), title + '3');
							domain.val((onlyAscii ? 'ascii.domain.co.uk' : 'unıcõde.domaîn.co.uk'));
							ok(!!jq.validator.methods.emailuser.call(validator, 'some_user', user.val('some_user')[0]), title + '4');
							ok(!!!jq.validator.methods.emailuser.call(validator, 'xyz@', user.val('xyz@')[0]), title + '5');
							ok(!!!jq.validator.methods.emailuser.call(validator, '@xyz', user.val('@xyz')[0]), title + '6');
							ok(!!!jq.validator.methods.emailuser.call(validator, '@@', user.val('@@')[0]), title + '7');
							deepEqual(!onlyAscii, jq.validator.methods.emailuser.call(validator, 'êığş', user.val('êığş')[0]), title + '8');
						}
					}
					form.parent().remove();
				}
				finally {
					Q.Config.emailAllowOnlyAscii = oldAscii;
				}
			}));
			test('EmailEditorUserOnlyValidationWorks', ss.mkdel(this, function() {
				var form = $Serenity_Test_EmailEditorTests.$createValidatedForm();
				var validator = form.validate();
				var input = $("<input type='text'/>").attr('name', 'dummy').addClass('email').appendTo(form);
				input.val('valid.email@address.com');
				ok(validator.element(input[0]));
				input.val('invalid.email@');
				ok(!validator.element(input[0]));
				form.parent().remove();
			}));
		}
	});
	ss.initClass($Serenity_Test_FormatterTypeRegistryTests, $asm, {
		runTests: function() {
			test('FormatterTypeRegistry_CanLocateSerenityFormatters', ss.mkdel(this, function() {
				strictEqual(Serenity.EnumFormatter, Serenity.FormatterTypeRegistry.get('Enum'), 'shortest');
				strictEqual(Serenity.EnumFormatter, Serenity.FormatterTypeRegistry.get('EnumFormatter'), 'with suffix');
				strictEqual(Serenity.EnumFormatter, Serenity.FormatterTypeRegistry.get('Serenity.Enum'), 'with namespace no suffix');
				strictEqual(Serenity.EnumFormatter, Serenity.FormatterTypeRegistry.get('Serenity.EnumFormatter'), 'with namespace and suffix');
			}));
			test('FormatterTypeRegistry_CanLocateDummyFormatter', ss.mkdel(this, function() {
				strictEqual($FormatterTypeRegistryTestNamespace_DummyFormatter, Serenity.FormatterTypeRegistry.get('FormatterTypeRegistryTestNamespace.Dummy'), 'with namespace and no suffix');
				strictEqual($FormatterTypeRegistryTestNamespace_DummyFormatter, Serenity.FormatterTypeRegistry.get('FormatterTypeRegistryTestNamespace.DummyFormatter'), 'with namespace and suffix');
				throws(function() {
					Serenity.FormatterTypeRegistry.get('DummyFormatter');
				}, function() {
					return ss.isInstanceOfType(arguments[0], ss.Exception);
				}, "can't find if no root namespace");
				Q.Config.rootNamespaces.push('FormatterTypeRegistryTestNamespace');
				try {
					Serenity.FormatterTypeRegistry.reset();
					strictEqual($FormatterTypeRegistryTestNamespace_DummyFormatter, Serenity.FormatterTypeRegistry.get('DummyFormatter'), 'can find if root namespace and suffix');
					strictEqual($FormatterTypeRegistryTestNamespace_DummyFormatter, Serenity.FormatterTypeRegistry.get('Dummy'), 'can find if root namespace and no suffix');
				}
				finally {
					ss.remove(Q.Config.rootNamespaces, 'FormatterTypeRegistryTestNamespace');
					Serenity.FormatterTypeRegistry.reset();
				}
			}));
		}
	});
	ss.initClass($Serenity_Test_JsonCriteriaConverterSerializationTests, $asm, {
		runTests: function() {
			test('JsonCriteriaConverter_SerializesStringCriteriaAsIs', ss.mkdel(this, function() {
				var actual = $.toJSON(['a']);
				deepEqual(actual, '["a"]');
			}));
			test('JsonCriteriaConverter_SerializesParenCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.paren(['a']));
				deepEqual(actual, '["()",["a"]]');
			}));
			test('JsonCriteriaConverter_SerializesIsNullCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(['is null', ['x']]);
				deepEqual(actual, '["is null",["x"]]');
			}));
			test('JsonCriteriaConverter_SerializesIsNotNullCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(['is not null', ['b']]);
				deepEqual(actual, '["is not null",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesExistsCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(['exists', ['some expression']]);
				deepEqual(actual, '["exists",["some expression"]]');
			}));
			test('JsonCriteriaConverter_SerializesAndCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(['a'], 'and', ['b']));
				deepEqual(actual, '[["a"],"and",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesMultipleAndCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(Serenity.Criteria.join(['a'], 'and', ['b']), 'and', ['c']));
				deepEqual(actual, '[[["a"],"and",["b"]],"and",["c"]]');
			}));
			test('JsonCriteriaConverter_SerializesOrCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(['c'], 'or', ['d']));
				deepEqual(actual, '[["c"],"or",["d"]]');
			}));
			test('JsonCriteriaConverter_SerializesMultipleOrCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(Serenity.Criteria.join(['a'], 'or', ['b']), 'or', ['c']));
				deepEqual(actual, '[[["a"],"or",["b"]],"or",["c"]]');
			}));
			test('JsonCriteriaConverter_SerializesXorCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(['c'], 'or', ['d']));
				deepEqual(actual, '[["c"],"or",["d"]]');
			}));
			test('JsonCriteriaConverter_SerializesMultipleXorCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(Serenity.Criteria.join(['a'], 'xor', ['b']), 'xor', ['c']));
				deepEqual(actual, '[[["a"],"xor",["b"]],"xor",["c"]]');
			}));
			test('JsonCriteriaConverter_SerializesEQCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '=', ['b']]);
				deepEqual(actual, '[["a"],"=",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesNECriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '!=', ['b']]);
				deepEqual(actual, '[["a"],"!=",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesGTCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '>', ['b']]);
				deepEqual(actual, '[["a"],">",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesGECriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '>=', ['b']]);
				deepEqual(actual, '[["a"],">=",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesLTCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '<', ['b']]);
				deepEqual(actual, '[["a"],"<",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesLECriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '<=', ['b']]);
				deepEqual(actual, '[["a"],"<=",["b"]]');
			}));
			test('JsonCriteriaConverter_SerializesINCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], 'in', [['b', 'c', 'd']]]);
				deepEqual(actual, '[["a"],"in",[["b","c","d"]]]');
			}));
			test('JsonCriteriaConverter_SerializesNotINCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], 'not in', [['b']]]);
				deepEqual(actual, '[["a"],"not in",[["b"]]]');
			}));
			test('JsonCriteriaConverter_SerializesLikeCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], 'like', 'b' + '%']);
				deepEqual(actual, '[["a"],"like","b%"]');
			}));
			test('JsonCriteriaConverter_SerializesNotLikeCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], 'not like', '%b']);
				deepEqual(actual, '[["a"],"not like","%b"]');
			}));
			test('JsonCriteriaConverter_SerializesValueCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON([['a'], '>=', 5]);
				deepEqual(actual, '[["a"],">=",5]');
			}));
			test('JsonCriteriaConverter_SerializesComplexCriteriaProperly', ss.mkdel(this, function() {
				var actual = $.toJSON(Serenity.Criteria.join(Serenity.Criteria.paren(Serenity.Criteria.join([['a'], '>=', 5], 'and', ['is not null', ['c']])), 'or', [[['x'], 'in', [[4, 5, 6]]], '!=', 7]));
				deepEqual(actual, '[["()",[[["a"],">=",5],"and",["is not null",["c"]]]],"or",[[["x"],"in",[[4,5,6]]],"!=",7]]');
			}));
		}
	});
	ss.initClass($Serenity_Test_QBlockUITests, $asm, {
		runTests: function() {
			test('BlockUIWorksWithDefaults', ss.mkdel(this, function() {
				Q.blockUI(null);
				ok($('div.blockUI.blockOverlay').length > 0, 'Check BlockUI overlay exists.');
				Q.blockUndo();
				ok($('div.blockUI.blockOverlay').length === 0, 'Check that BlockUI overlay is removed after BlockUndo.');
			}));
			test('BlockUICascadedCallWorks', ss.mkdel(this, function() {
				Q.blockUI(null);
				Q.blockUI(null);
				ok($('div.blockUI.blockOverlay').length > 0, 'Check BlockUI overlay exists after double calls.');
				Q.blockUndo();
				ok($('div.blockUI.blockOverlay').length > 0, 'Check that BlockUI overlay IS NOT removed after first BlockUndo.');
				Q.blockUndo();
				ok($('div.blockUI.blockOverlay').length === 0, 'Check that BlockUI overlay IS removed after second BlockUndo.');
			}));
		}
	});
	ss.initClass($Serenity_Test_QConfigTests, $asm, {
		runTests: function() {
			test('ApplicationPathCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qConfig = window.window.Q.Config;
				var backup = Q.Config.applicationPath;
				try {
					Q.Config.applicationPath = '/Dummy1';
					deepEqual(qConfig.applicationPath, '/Dummy1', 'Set by C#, read directly');
					qConfig.applicationPath = '/Dummy2';
					deepEqual(Q.Config.applicationPath, '/Dummy2', 'Set directly, read by C#');
				}
				finally {
					Q.Config.applicationPath = backup;
				}
			}));
			test('RootNamespacesListWorks', ss.mkdel(this, function() {
				ok(ss.isValue(Q.Config.rootNamespaces), 'The list is initialized');
				var count = Q.Config.rootNamespaces.length;
				ok(count > 0 && ss.contains(Q.Config.rootNamespaces, 'Serenity'), "The list should contain 'Serenity'");
				Q.Config.rootNamespaces.push('SomeDummyNamespace');
				ok(ss.contains(Q.Config.rootNamespaces, 'SomeDummyNamespace'), 'Can add a new root namespace to list');
				ss.remove(Q.Config.rootNamespaces, 'SomeDummyNamespace');
				ok(!ss.contains(Q.Config.rootNamespaces, 'SomeDummyNamespace'), 'Can remove namespace from the list');
			}));
		}
	});
	ss.initClass($Serenity_Test_QCultureTests, $asm, {
		runTests: function() {
			test('CultureDateSeparatorCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qCulture = window.window.Q.Culture;
				var backup = Q.Culture.dateSeparator;
				try {
					Q.Culture.dateSeparator = '$';
					deepEqual(qCulture.dateSeparator, '$', 'Set by C#, read directly');
					qCulture.dateSeparator = '/';
					deepEqual(Q.Culture.dateSeparator, '/', 'Set by C#, read directly');
				}
				finally {
					Q.Culture.dateSeparator = backup;
				}
			}));
			test('CultureDateOrderCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qCulture = window.window.Q.Culture;
				var backup = Q.Culture.dateOrder;
				try {
					Q.Culture.dateOrder = 'myd';
					deepEqual(qCulture.dateOrder, 'myd', 'Set by C#, read directly');
					qCulture.dateOrder = 'dmy';
					deepEqual(Q.Culture.dateOrder, 'dmy', 'Set by C#, read directly');
				}
				finally {
					Q.Culture.dateOrder = backup;
				}
			}));
			test('CultureDateFormatCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qCulture = window.window.Q.Culture;
				var backup = Q.Culture.dateFormat;
				try {
					Q.Culture.dateFormat = '%';
					deepEqual(qCulture.dateFormat, '%', 'Set by C#, read directly');
					qCulture.dateFormat = '.';
					deepEqual(Q.Culture.dateFormat, '.', 'Set by C#, read directly');
				}
				finally {
					Q.Culture.dateFormat = backup;
				}
			}));
			test('CultureDateTimeFormatCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qCulture = window.window.Q.Culture;
				var backup = Q.Culture.dateTimeFormat;
				try {
					Q.Culture.dateTimeFormat = '%';
					deepEqual(qCulture.dateTimeFormat, '%', 'Set by C#, read directly');
					qCulture.dateTimeFormat = '.';
					deepEqual(Q.Culture.dateTimeFormat, '.', 'Set by C#, read directly');
				}
				finally {
					Q.Culture.dateTimeFormat = backup;
				}
			}));
			test('CultureDecimalSeparatorCanBeSet', ss.mkdel(this, function() {
				// to check script name safety
				var qCulture = window.window.Q.Culture;
				var backup = Q.Culture.decimalSeparator;
				try {
					Q.Culture.decimalSeparator = '%';
					deepEqual(qCulture.decimalSeparator, '%', 'Set by C#, read directly');
					qCulture.decimalSeparator = '.';
					deepEqual(Q.Culture.decimalSeparator, '.', 'Set by C#, read directly');
				}
				finally {
					Q.Culture.decimalSeparator = backup;
				}
			}));
		}
	});
	ss.initClass($Serenity_Test_QDateTests, $asm, {
		runTests: function() {
			test('FormatDateWorks', ss.mkdel(this, function() {
				var backupDec = Q.Culture.dateSeparator;
				var backupDateFormat = Q.Culture.dateFormat;
				var backupDateTimeFormat = Q.Culture.dateTimeFormat;
				try {
					Q.Culture.dateSeparator = '/';
					Q.Culture.dateFormat = 'dd/MM/yyyy';
					Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
					var date = new Date(2029, 0, 2, 3, 4, 5, 6);
					// 02.01.2029 03:04:05.006
					deepEqual(Q.formatDate(date, 'dd/MM/yyyy'), '02/01/2029', "'/': dd/MM/yyy");
					deepEqual(Q.formatDate(date, 'd/M/yy'), '2/1/29', "'/': d/M/yy");
					deepEqual(Q.formatDate(date, 'd.M.yyyy'), '2.1.2029', "'/': d.M.yyy");
					deepEqual(Q.formatDate(date, 'yyyyMMdd'), '20290102', "'/': yyyyMMdd");
					deepEqual(Q.formatDate(date, 'hh:mm tt'), '03:04 AM', "'/': hh:mm tt");
					deepEqual(Q.formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff'), '2029-01-02T03:04:05.006', "'/': yyyy-MM-ddTHH:mm:ss.fff");
					deepEqual(Q.formatDate(date, 'd'), '02/01/2029', "'/': d");
					deepEqual(Q.formatDate(date, 'g'), '02/01/2029 03:04', "'/': g");
					deepEqual(Q.formatDate(date, 'G'), '02/01/2029 03:04:05', "'/': G");
					deepEqual(Q.formatDate(date, 's'), '2029-01-02T03:04:05', "'/': s");
					deepEqual(Q.formatDate(date, 'u'), Q.formatISODateTimeUTC(date), "'/': u");
					Q.Culture.dateSeparator = '.';
					deepEqual(Q.formatDate(date, 'dd/MM/yyyy'), '02.01.2029', "'.': dd/MM/yyy");
					deepEqual(Q.formatDate(date, 'd/M/yy'), '2.1.29', "'.': d/M/yy");
					deepEqual(Q.formatDate(date, 'd-M-yyyy'), '2-1-2029', "'.': d-M-yyy");
					deepEqual(Q.formatDate(date, 'yyyy-MM-ddTHH:mm:ss.fff'), '2029-01-02T03:04:05.006', "'.': yyyy-MM-ddTHH:mm:ss.fff");
					deepEqual(Q.formatDate(date, 'g'), '02.01.2029 03:04', "'.': g");
					deepEqual(Q.formatDate(date, 'G'), '02.01.2029 03:04:05', "'.': G");
					deepEqual(Q.formatDate(date, 's'), '2029-01-02T03:04:05', "'.': s");
					deepEqual(Q.formatDate(date, 'u'), Q.formatISODateTimeUTC(date), "'.': u");
				}
				finally {
					Q.Culture.decimalSeparator = backupDec;
					Q.Culture.dateFormat = backupDateFormat;
					Q.Culture.dateTimeFormat = backupDateTimeFormat;
				}
			}));
			test('FormatDateWorksWithISOString', ss.mkdel(this, function() {
				var backupDec = Q.Culture.dateSeparator;
				var backupDateFormat = Q.Culture.dateFormat;
				var backupDateTimeFormat = Q.Culture.dateTimeFormat;
				try {
					Q.Culture.dateSeparator = '/';
					Q.Culture.dateFormat = 'dd/MM/yyyy';
					Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
					deepEqual(Q.formatDate('2029-01-02', null), '02/01/2029', "'/': date only, empty format");
					deepEqual(Q.formatDate('2029-01-02T16:35:24', null), '02/01/2029', "'/': date with time, empty format");
					deepEqual(Q.formatDate('2029-01-02T16:35:24', 'g'), '02/01/2029 16:35', "'/': date with time, g format");
					Q.Culture.dateSeparator = '.';
					deepEqual(Q.formatDate('2029-01-02', null), '02.01.2029', "'.': date only, empty format");
					deepEqual(Q.formatDate('2029-01-02T16:35:24', null), '02.01.2029', "'.': date with time, empty format");
					deepEqual(Q.formatDate('2029-01-02T16:35:24', 'g'), '02.01.2029 16:35', "'.': date with time, g format");
				}
				finally {
					Q.Culture.decimalSeparator = backupDec;
					Q.Culture.dateFormat = backupDateFormat;
					Q.Culture.dateTimeFormat = backupDateTimeFormat;
				}
			}));
			test('FormatDateWorksWithDateString', ss.mkdel(this, function() {
				var backupDec = Q.Culture.dateSeparator;
				var backupDateFormat = Q.Culture.dateFormat;
				var backupDateTimeFormat = Q.Culture.dateTimeFormat;
				try {
					Q.Culture.dateSeparator = '/';
					Q.Culture.dateFormat = 'dd/MM/yyyy';
					Q.Culture.dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';
					deepEqual(Q.formatDate('2/1/2029', null), '02/01/2029', "'/': date only, empty format");
					Q.Culture.dateSeparator = '.';
					deepEqual(Q.formatDate('2/1/2029', null), '02.01.2029', "'.': date only, empty format");
				}
				finally {
					Q.Culture.decimalSeparator = backupDec;
					Q.Culture.dateFormat = backupDateFormat;
					Q.Culture.dateTimeFormat = backupDateTimeFormat;
				}
			}));
		}
	});
	ss.initClass($Serenity_Test_QDialogTests, $asm, {
		runTests: function() {
			test('AlertDialogCanOpenClose', ss.mkdel(this, function() {
				Q.alert('Test');
				ok($('div.s-AlertDialog.ui-dialog:visible').length > 0, 'Check alert dialog exists and visible.');
				$('.ui-dialog-titlebar-close:visible').click();
				ok($('div.s-AlertDialog.ui-dialog:visible').length === 0, 'Check alert dialog is closed.');
			}));
			test('ConfirmDialogCanOpenClose', ss.mkdel(this, function() {
				var confirmed = false;
				Q.confirm('Test ABCDEFGHJKL', function() {
					confirmed = true;
				});
				var dlg = $('div.s-ConfirmDialog.ui-dialog:visible');
				ok(dlg.length > 0, 'Check confirmation dialog exists and visible.');
				ok(dlg.find(":contains('Test ABCDEFGHJKL'):visible").length > 0, 'Check that message is displayed');
				var yesButton = $(".ui-dialog-buttonset button:contains('" + Q.text('Dialogs.YesButton') + "')");
				ok(yesButton.length === 1, 'Check that dialog has Yes button');
				var noButton = $(".ui-dialog-buttonset button:contains('" + Q.text('Dialogs.NoButton') + "')");
				ok(noButton.length === 1, 'Check that dialog has No button');
				$('.ui-dialog-titlebar-close:visible').click();
				ok($('div.s-ConfirmDialog.ui-dialog:visible').length === 0, 'Check confirmation dialog is closed.');
				ok(!confirmed, 'Check that confirmed is not set when dialog is closed directly');
			}));
			test('ConfirmDialogYesClickCallsSuccess', ss.mkdel(this, function() {
				var confirmed = false;
				Q.confirm('Test', function() {
					confirmed = true;
				});
				$(".ui-dialog-buttonset button:contains('" + Q.text('Dialogs.YesButton') + "')").click();
				ok(confirmed, 'Ensure yes button click called success delegate');
			}));
			test('ConfirmDialogNoClickDoesntCallSuccess', ss.mkdel(this, function() {
				var confirmed = false;
				Q.confirm('Test', function() {
					confirmed = true;
				});
				$(".ui-dialog-buttonset button:contains('" + Q.text('Dialogs.NoButton') + "')").click();
				ok(!confirmed, "Ensure no button click didn't call success delegate");
			}));
			test('InformationDialogCanOpenClose', ss.mkdel(this, function() {
				Q.information('Test');
				ok($('div.s-InformationDialog.ui-dialog:visible').length > 0, 'Check information dialog exists and visible.');
				$('.ui-dialog-titlebar-close:visible').click();
				ok($('div.s-InformationDialog.ui-dialog:visible').length === 0, 'Check information dialog is closed.');
			}));
		}
	});
	ss.initClass($Serenity_Test_QExternalsTests, $asm, {
		runTests: function() {
			test('TurkishLocaleCompareWorksProperly', ss.mkdel(this, function() {
				deepEqual(Q.turkishLocaleCompare('a', 'b'), -1);
				deepEqual(Q.turkishLocaleCompare('b', 'a'), 1);
				deepEqual(Q.turkishLocaleCompare('b', 'b'), 0);
				deepEqual(Q.turkishLocaleCompare('i', 'ı'), 1);
				deepEqual(Q.turkishLocaleCompare('ı', 'i'), -1);
				deepEqual(Q.turkishLocaleCompare('İSTANBUL', 'ıSpaRTA'), 1);
				deepEqual(Q.turkishLocaleCompare('ıSpaRTA', 'İSTANBUL'), -1);
				deepEqual(Q.turkishLocaleCompare('0', 'TEXT'), -1);
				deepEqual(Q.turkishLocaleCompare('TEXT', '5'), 1);
				deepEqual(Q.turkishLocaleCompare('123ABC', '123AB'), 1);
				deepEqual(Q.turkishLocaleCompare('', null), 0);
				deepEqual(Q.turkishLocaleCompare('_', null), 1);
				deepEqual(Q.turkishLocaleCompare(null, 'X'), -1);
			}));
		}
	});
	ss.initClass($Serenity_Test_QHtmlTests, $asm, {
		runTests: function() {
			test('HtmlEncodeWorks', ss.mkdel(this, function() {
				deepEqual(Q.htmlEncode(null), '');
				deepEqual(Q.htmlEncode('a'), 'a');
				deepEqual(Q.htmlEncode('cdef'), 'cdef');
				deepEqual(Q.htmlEncode('<'), '&lt;');
				deepEqual(Q.htmlEncode('>'), '&gt;');
				deepEqual(Q.htmlEncode('&'), '&amp;');
				deepEqual(Q.htmlEncode('if (a < b && c > d) x = 5 & 3 else y = (u <> w)'), 'if (a &lt; b &amp;&amp; c &gt; d) x = 5 &amp; 3 else y = (u &lt;&gt; w)');
				var q = window.window['Q'];
				deepEqual(q.htmlEncode('<script&>'), '&lt;script&amp;&gt;', 'check direct script access');
			}));
		}
	});
	ss.initClass($Serenity_Test_QMethodTests, $asm, {
		runTests: function() {
			test('ToJSONWorks', ss.mkdel(this, function() {
				deepEqual($.toJSON(12345), '12345', 'Number');
				deepEqual($.toJSON('abcd"\'e'), '"abcd\\"\'e"', 'String');
				var date = new Date(2013, 12, 27, 16, 19, 35, 345);
				deepEqual($.toJSON(date), '"' + Q.formatISODateTimeUTC(date) + '"', 'Date/Time');
				deepEqual($.toJSON(12345.678), '12345.678', 'Double');
				var o = { num: 5, str: 'abc', date: date };
				var json = $.toJSON(o);
				ok(typeof(json) === 'string', 'Ensure serialized object is string');
				var deserialized = $.parseJSON(json);
				o.date = Q.formatISODateTimeUTC(date);
				deepEqual(deserialized, o, 'Compare original object and deserialization');
			}));
			test('IsTrueWorks', ss.mkdel(this, function() {
				deepEqual(!!1, true, '1 is true');
				deepEqual(!!0, false, '0 is false');
				deepEqual(!!null, false, 'null is false');
				deepEqual(!!undefined, false, 'undefined is false');
				deepEqual(!!'0', true, "'0' is true");
				deepEqual(!!'1', true, "'1' is true");
				deepEqual(!!'-1', true, "'-1' is true");
				deepEqual(!!'xysa', true, 'any other value is true');
			}));
			test('IsFalseWorks', ss.mkdel(this, function() {
				deepEqual(!1, false, '1 is false');
				deepEqual(!0, true, '0 is true');
				deepEqual(!null, true, 'null is true');
				deepEqual(!undefined, true, 'undefined is true');
				deepEqual(!'0', false, "'0' is false");
				deepEqual(!'-1', false, '-1 is false');
				deepEqual(!'xysa', false, 'any other value is false');
			}));
		}
	});
	ss.initClass($Serenity_Test_QNumberTests, $asm, {
		runTests: function() {
			test('FormatNumberWorks', ss.mkdel(this, function() {
				deepEqual(Q.formatNumber(1, '1'), '1');
				var backupDec = Q.Culture.decimalSeparator;
				try {
					Q.Culture.decimalSeparator = ',';
					deepEqual(Q.formatNumber(1, '0.00'), '1,00');
					deepEqual(Q.formatNumber(1, '0.0000'), '1,0000');
					deepEqual(Q.formatNumber(1234, '#,##0'), '1.234');
					deepEqual(Q.formatNumber(1234.5, '#,##0.##'), '1.234,5');
					deepEqual(Q.formatNumber(1234.5678, '#,##0.##'), '1.234,57');
					deepEqual(Q.formatNumber(1234.5, '#,##0.00'), '1.234,50');
					Q.Culture.decimalSeparator = '.';
					deepEqual(Q.formatNumber(1234, '#,##0'), '1,234');
					deepEqual(Q.formatNumber(1234.5, '#,##0.##'), '1,234.5');
					deepEqual(Q.formatNumber(1234.5678, '#,##0.##'), '1,234.57');
					deepEqual(Q.formatNumber(1234.5, '#,##0.00'), '1,234.50');
				}
				finally {
					Q.Culture.decimalSeparator = backupDec;
				}
			}));
		}
	});
	ss.initClass($Serenity_Test_ScriptContextTests, $asm, {
		runTests: function() {
			test('JWorksWithOneParam', ss.mkdel(this, function() {
				deepEqual(1, (new $Serenity_$Test_ScriptContextTests$SubClass()).$select('body').length);
			}));
			test('JWorksWithTwoParams', ss.mkdel(this, function() {
				deepEqual(1, (new $Serenity_$Test_ScriptContextTests$SubClass()).$select$1('body', window.document).length);
				deepEqual(0, (new $Serenity_$Test_ScriptContextTests$SubClass()).$select$1('document', window.document.body).length);
			}));
		}
	});
	ss.setMetadata($EditorTypeRegistryTestNamespace_DummyEditor, { attr: [(function() {
		var $t1 = new Serenity.EditorAttribute();
		$t1.key = 'SomeOtherKeyForDummyEditor';
		return $t1;
	})()] });
})();

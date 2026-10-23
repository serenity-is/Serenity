import {
	SymbolFlags,
	type Program,
	type Symbol,
} from 'typescript/sync';
import * as ast from 'typescript/ast';

import type { BundleStageOptions } from './types';

const NODE_MODULES_RE = /[\\/]node_modules[\\/]/;

const VALUABLE_KINDS = new Set<ast.SyntaxKind>([
	ast.SyntaxKind.ClassDeclaration,
	ast.SyntaxKind.FunctionDeclaration,
	ast.SyntaxKind.VariableStatement,
	ast.SyntaxKind.EnumDeclaration,
	ast.SyntaxKind.ModuleDeclaration,
]);

export interface BundleInput {
	program: Program;
	entryPath: string;
	options: BundleStageOptions;
}

export function bundle(input: BundleInput): string {
	// Per-phase timings, for when the bundle stage looks slow:
	//   DTSBUNDLER_TIMINGS=1 dtsbundler --verbose
	const timings = process.env.DTSBUNDLER_TIMINGS !== undefined;
	let phaseStarted = performance.now();
	let checkerCalls = 0;
	const lap = (label: string): void => {
		if (!timings) {
			return;
		}
		const now = performance.now();
		console.error(`[timing] ${label.padEnd(22)} ${(now - phaseStarted).toFixed(1).padStart(7)} ms  (checker calls so far: ${checkerCalls})`);
		phaseStarted = now;
	};

	const { program, entryPath, options } = input;
	const project = program.getProject();
	const checker = project.checker;

	const entrySourceFile = program.getSourceFile(entryPath);
	if (!entrySourceFile) {
		throw new Error(`Cannot find entry source file ${entryPath}`);
	}

	const classify = (file: { fileName: string }): 'global' | 'imported' | 'inlined' => {
		const sf = file as ast.SourceFile;
		if (program.isSourceFileDefaultLibrary(sf)) {
			return 'global';
		}
		if (NODE_MODULES_RE.test(file.fileName)) {
			// node_modules files may also be inlined via `externalInlines`
			const specifier = moduleSpecifierByPath.get(file.fileName);
			if (specifier !== undefined && options.externalInlines.includes(specifier)) {
				return 'inlined';
			}
			return 'imported';
		}
		return 'inlined';
	};

	// ---- helpers -------------------------------------------------------------
	const isPlumbingStatement = (stmt: ast.Node): boolean => {
		if (ast.isImportDeclaration(stmt) || ast.isImportEqualsDeclaration(stmt) || ast.isExportAssignment(stmt)) {
			return true;
		}
		if (ast.isExportDeclaration(stmt) || ast.isNamespaceExportDeclaration(stmt)) {
			return true;
		}
		return false;
	};

	const resolveAlias = (sym: Symbol): Symbol => {
		if (sym.flags & SymbolFlags.Alias) {
			const target = checker.getAliasedSymbol(sym);
			if (target && !checker.isUnknownSymbol(target)) {
				return target;
			}
		}
		return sym;
	};

	const resolveSpecifierTarget = (specifier: ast.ExportSpecifier | ast.ImportSpecifier): Symbol | undefined => {
		const ref = (specifier as { propertyName?: ast.Identifier; name: ast.Identifier }).propertyName
			?? (specifier as { name: ast.Identifier }).name;
		const target = checker.getSymbolAtLocation(ref);
		if (!target) {
			return undefined;
		}
		const resolved = resolveAlias(target);
		return checker.isUnknownSymbol(resolved) ? undefined : resolved;
	};

	// `declare module "./x" { ... }` augments a sibling file. Such a block has no meaning
	// after bundling, so its statements are promoted to top-level (see includeStatement).
	const isRelativeDeclareModule = (node: ast.Node): boolean => {
		return ast.isModuleDeclaration(node)
			&& ast.isStringLiteral(node.name)
			&& (node.name.text.startsWith('./') || node.name.text.startsWith('../'));
	};

	/** The statement that gets emitted for a declaration (used as its identity everywhere). */
	const getTopLevelStatement = (node: ast.Node): ast.Node | null => {
		let n: ast.Node = node;
		while (n.parent && !ast.isSourceFile(n.parent)) {
			// a statement inside a promoted relative `declare module` is emitted on its own,
			// so it - not the module declaration - is the top-level statement
			if (ast.isModuleBlock(n.parent) && isRelativeDeclareModule(n.parent.parent)) {
				return isNodeStatement(n) && !isPlumbingStatement(n) ? n : null;
			}
			n = n.parent;
			// other namespaces/declare module blocks are not inlined yet
			if (ast.isModuleDeclaration(n) && !ast.isSourceFile(n.parent)) {
				return null;
			}
		}
		if (!ast.isSourceFile(n.parent) || !isNodeStatement(n) || isPlumbingStatement(n)) {
			return null;
		}
		return n;
	};

	const isNodeStatement = (node: ast.Node): boolean => {
		return ast.isVariableStatement(node)
			|| ast.isFunctionDeclaration(node)
			|| ast.isClassDeclaration(node)
			|| ast.isInterfaceDeclaration(node)
			|| ast.isTypeAliasDeclaration(node)
			|| ast.isEnumDeclaration(node)
			|| ast.isModuleDeclaration(node)
			|| ast.isImportDeclaration(node)
			|| ast.isImportEqualsDeclaration(node)
			|| ast.isExportDeclaration(node)
			|| ast.isExportAssignment(node)
			|| ast.isNamespaceExportDeclaration(node);
	};

	const getNodeNameText = (statement: ast.Node): string[] => {
		if (ast.isVariableStatement(statement)) {
			const names: string[] = [];
			for (const decl of statement.declarationList.declarations) {
				if (ast.isIdentifier(decl.name)) {
					names.push(decl.name.text);
				}
			}
			return names;
		}
		const named = statement as { name?: ast.Node };
		if (named.name && ast.isIdentifier(named.name)) {
			return [named.name.text];
		}
		return [];
	};

	/** Symbols a top-level statement declares, used to detect declaration merging. */
	const getStatementSymbols = (statement: ast.Node): Set<Symbol> => {
		const result = new Set<Symbol>();
		const nameNodes: ast.Node[] = [];
		if (ast.isVariableStatement(statement)) {
			for (const decl of statement.declarationList.declarations) {
				if (ast.isIdentifier(decl.name)) {
					nameNodes.push(decl.name);
				}
			}
		} else {
			const named = statement as { name?: ast.Node };
			if (named.name && ast.isIdentifier(named.name)) {
				nameNodes.push(named.name);
			}
		}
		for (const nameNode of nameNodes) {
			const symbol = checker.getSymbolAtLocation(nameNode);
			if (symbol) {
				result.add(symbol);
			}
		}
		return result;
	};

	const symbolSourceFile = (symbol: Symbol): ast.SourceFile | undefined => {
		if (symbol.declarations.length === 0) {
			return undefined;
		}
		const node = symbol.declarations[0].resolve(project);
		return node ? node.getSourceFile() : undefined;
	};

	// ---- module specifier lookup per inlined file -----------------------------
	const moduleSpecifierByPath = new Map<string, string>();
	for (const file of program.getSourceFileNames()) {
		const sf = program.getSourceFile(file);
		if (!sf) {
			continue;
		}
		if (program.isSourceFileDefaultLibrary(sf) || NODE_MODULES_RE.test(sf.fileName)) {
			continue;
		}
		for (const st of sf.statements) {
			if (!ast.isImportDeclaration(st) && !ast.isExportDeclaration(st)) {
				continue;
			}
			if (!st.moduleSpecifier) {
				continue;
			}
			const sym = checker.getSymbolAtLocation(st.moduleSpecifier);
			if (!sym || sym.declarations.length === 0) {
				continue;
			}
			const target = sym.declarations[0].resolve(project);
			if (!target) {
				continue;
			}
			const targetFile = target.getSourceFile();
			if (targetFile !== sf) {
				moduleSpecifierByPath.set(targetFile.fileName, (st.moduleSpecifier as { text?: string }).text ?? st.moduleSpecifier.getText());
			}
		}
	}

	// ---- collection ------------------------------------------------------------
	const entrySymbol = checker.getSymbolAtLocation(entrySourceFile);
	if (!entrySymbol) {
		throw new Error(`No module symbol for ${entryPath}`);
	}
	const entryExports = checker.getExportsOfModule(entrySymbol);

	const includedSymbols = new Set<Symbol>();
	const includedStatements = new Set<ast.Node>();
	const statements: ast.Node[] = [];
	const importedRefs = new Map<string, Set<Symbol>>(); // moduleSpecifier -> symbols

	const queue: Symbol[] = [...entryExports];

	const includeSymbol = (symbol: Symbol): void => {
		if (includedSymbols.has(symbol)) {
			return;
		}
		includedSymbols.add(symbol);

		const sourceFile = symbolSourceFile(symbol);
		if (sourceFile && classify(sourceFile) === 'imported') {
			addImportedSymbol(sourceFile.fileName, symbol);
		}

		for (const declHandle of symbol.declarations) {
			const node = declHandle.resolve(project);
			if (!node) {
				continue;
			}
			if (node.kind === ast.SyntaxKind.SourceFile) {
				for (const e of checker.getExportsOfModule(symbol)) {
					queue.push(e);
				}
				continue;
			}
			// import/export plumbing: resolve the referenced targets
			if (ast.isImportDeclaration(node) || ast.isExportDeclaration(node)) {
				if (ast.isExportDeclaration(node) && node.exportClause && ast.isNamedExports(node.exportClause)) {
					for (const specifier of node.exportClause.elements) {
						const target = resolveSpecifierTarget(specifier);
						if (target) {
							queue.push(target);
						}
					}
				} else if (ast.isImportDeclaration(node) && node.importClause && node.importClause.namedBindings && ast.isNamedImports(node.importClause.namedBindings)) {
					for (const specifier of node.importClause.namedBindings.elements) {
						const target = resolveSpecifierTarget(specifier);
						if (target) {
							queue.push(target);
						}
					}
				}
				continue;
			}
			// `export { x } from './mod'` / `import { x } from './mod'`: the exported symbol's own
			// declaration is the specifier itself, so follow it to the real declaration
			if (ast.isExportSpecifier(node) || ast.isImportSpecifier(node)) {
				const target = resolveSpecifierTarget(node);
				if (target) {
					queue.push(target);
				}
				continue;
			}
			// `export * as ns from './mod'` / `import * as ns from './mod'`
			if (ast.isNamespaceExport(node) || ast.isNamespaceImport(node)) {
				const target = checker.getSymbolAtLocation(node.parent && ast.isExportDeclaration(node.parent) ? node.parent.moduleSpecifier! : (node.parent as ast.ImportDeclaration).moduleSpecifier);
				if (target) {
					const resolved = resolveAlias(target);
					if (!checker.isUnknownSymbol(resolved)) {
						for (const exp of checker.getExportsOfModule(resolved)) {
							queue.push(exp);
						}
					}
				}
				continue;
			}
			const stmt = getTopLevelStatement(node);
			if (!stmt) {
				continue;
			}
			includeStatement(stmt);
		}
	};

	const addImportedSymbol = (sourceFileName: string, symbol: Symbol): void => {
		const specifier = moduleSpecifierByPath.get(sourceFileName);
		if (specifier) {
			let set = importedRefs.get(specifier);
			if (!set) {
				set = new Set<Symbol>();
				importedRefs.set(specifier, set);
			}
			set.add(symbol);
		}
	};

	/** `isDeclarationName` narrows to `never` for an `Identifier`, so ask through a boolean. */
	const isDeclarationName = (node: ast.Node): boolean => ast.isDeclarationName(node);

	const isReferenceIdentifier = (node: ast.Node): boolean => {
		if (!ast.isIdentifier(node)) {
			return false;
		}
		const p = node.parent;
		if (!p) {
			return true;
		}
		// the node is the name slot of its parent declaration (interface, member, parameter,
		// type parameter, enum member, ...); asking the checker would only return the symbol
		// being declared. Note `ast.isDeclarationName` alone is not enough: it is also true for
		// identifiers inside computed names (`[sym]: T`) and type queries (`typeof ns`), which
		// are real references.
		if ((p as { name?: ast.Node }).name === node && isDeclarationName(node)) {
			return false;
		}
		// the right hand side of `a.b` / `A.B` is a member, never a declaration to inline
		if (ast.isPropertyAccessExpression(p) && p.name === node) return false;
		if (ast.isQualifiedName(p) && p.right === node) return false;
		if (ast.isNamedTupleMember(p) && p.name === node) return false;
		if (ast.isBindingElement(p) && p.propertyName === node) return false;
		if (ast.isImportSpecifier(p) && p.propertyName === node) return false;
		if (ast.isExportSpecifier(p) && p.propertyName === node) return false;
		return true;
	};

	const scanReferences = (statement: ast.Node): void => {
		// references inside a `declare module "pkg" { ... }` augmentation resolve in that
		// module's scope, so they must not be turned into top-level imports
		if (ast.isModuleDeclaration(statement) && ast.isStringLiteral(statement.name)) {
			return;
		}

		const visit = (node: ast.Node): void => {
			if (isReferenceIdentifier(node)) {
				checkerCalls++;
				const rawSym = checker.getSymbolAtLocation(node);
				if (rawSym) {
					const sym = resolveAlias(rawSym);
					if (!includedSymbols.has(sym)) {
						const sourceFile = symbolSourceFile(sym);
						const cls = sourceFile ? classify(sourceFile) : null;
						if (cls === 'inlined') {
							queue.push(sym);
						} else if (cls === 'imported') {
							addImportedSymbol(sourceFile!.fileName, sym);
						}
					}
				}
			}
			node.forEachChild(visit);
		};
		visit(statement);
	};

	const includeStatement = (statement: ast.Node): void => {
		if (includedStatements.has(statement)) {
			return;
		}

		// A relative `declare module "./x" { ... }` is a module augmentation of a
		// sibling file. Once bundled there is no "./x" module anymore, so its
		// declarations have to be promoted to top-level (that is also what
		// dts-bundle-generator does). Non-relative ones are kept as-is.
		if (isRelativeDeclareModule(statement) && ast.isModuleDeclaration(statement)
			&& statement.body !== undefined && ast.isModuleBlock(statement.body)) {
			includedStatements.add(statement);
			for (const inner of statement.body.statements) {
				includeStatement(inner);
			}
			return;
		}

		includedStatements.add(statement);
		statements.push(statement);
		scanReferences(statement);
	};

	lap('collect entry exports');
	while (queue.length > 0) {
		includeSymbol(queue.pop()!);
	}

	// `--inline-declare-global` / `--inline-declare-externals`: bring over the `declare
	// global` and `declare module "pkg"` blocks of the inlined files, since they would
	// otherwise be lost together with the files they came from.
	lap('crawl references');
	if (options.inlineDeclareGlobals || options.inlineDeclareExternals) {
		for (const fileName of program.getSourceFileNames()) {
			const sourceFile = program.getSourceFile(fileName);
			if (!sourceFile || classify(sourceFile) !== 'inlined') {
				continue;
			}
			for (const statement of sourceFile.statements) {
				if (!ast.isModuleDeclaration(statement)) {
					continue;
				}
				const isGlobalAugmentation = ast.isIdentifier(statement.name) && statement.name.text === 'global';
				if (isGlobalAugmentation) {
					if (options.inlineDeclareGlobals) {
						includeStatement(statement);
					}
					continue;
				}
				if (options.inlineDeclareExternals && ast.isStringLiteral(statement.name) && !isRelativeDeclareModule(statement)) {
					includeStatement(statement);
				}
			}
		}
	}

	lap('augmentations');
	// ---- entry exports: exported name + local (in-scope) name ------------------
	// Keyed by the *resolved* target symbol because `export { x } from './m'` makes the
	// exported symbol an alias whose declarations are the specifiers in the entry file.
	interface ExportedNameInfo {
		exportedName: string;
		localName: string;
	}
	const exportedByTarget = new Map<Symbol, ExportedNameInfo[]>();	const exportedStatements = new Set<ast.Node>();

	const addExportedStatement = (symbol: Symbol): void => {
		for (const declaration of symbol.declarations) {
			const node = declaration.resolve(project);
			if (!node) {
				continue;
			}
			const statement = getTopLevelStatement(node);
			if (statement) {
				exportedStatements.add(statement);
			}
		}
	};

	for (const exp of entryExports) {
		if (exp.name === 'default' || exp.name.startsWith('*')) {
			continue;
		}

		let localName = exp.name;
		let target: Symbol = exp;
		for (const declaration of exp.declarations) {
			const node = declaration.resolve(project);
			if (!node) {
				continue;
			}
			if (ast.isExportSpecifier(node) || ast.isImportSpecifier(node)) {
				const spec = node as { propertyName?: ast.Identifier; name: ast.Identifier };
				localName = (spec.propertyName ?? spec.name).text;
				const resolvedTarget = resolveSpecifierTarget(node);
				if (resolvedTarget) {
					target = resolvedTarget;
				}
			}
		}

		const infos = exportedByTarget.get(target) ?? [];
		infos.push({ exportedName: exp.name, localName });
		exportedByTarget.set(target, infos);

		addExportedStatement(exp);
		if (target !== exp) {
			addExportedStatement(target);
		}
	}

	lap('export mapping');
	// ---- name collision resolution ----------------------------------------------
	const takenNames = new Map<string, { statement: ast.Node; exported: boolean }>();
	const renamedSymbols = new Map<Symbol, string>();

	const isStatementExported = (st: ast.Node): boolean => exportedStatements.has(st);

	for (const st of statements) {
		for (const name of getNodeNameText(st)) {
			const existing = takenNames.get(name);
			if (!existing) {
				takenNames.set(name, { statement: st, exported: isStatementExported(st) });
				continue;
			}

			// Two declarations of the *same* symbol (interface/namespace/function merging):
			// they must keep the name so TypeScript merges them in the bundle.
			const statementSymbols = getStatementSymbols(st);
			const existingSymbols = getStatementSymbols(existing.statement);
			if ([...statementSymbols].some((symbol) => existingSymbols.has(symbol))) {
				continue;
			}

			const thisNameExported = isStatementExported(st);
			if (thisNameExported) {
				// Exported declarations must keep their public name. If an exported
				// declaration already holds it, keep both (that is what
				// dts-bundle-generator does too).
				if (!existing.exported) {
					renameSymbolOf(existing.statement, name);
				}
			} else {
				renameSymbolOf(st, name);
			}
			takenNames.set(name, { statement: st, exported: thisNameExported || existing.exported });
		}
	}

	function renameSymbolOf(statement: ast.Node, fromName: string): void {
		let newName = `${fromName}2`;
		while (takenNames.has(newName)) {
			newName += '2';
		}
		takenNames.set(newName, { statement, exported: false });
		const visit = (node: ast.Node): void => {
			if (ast.isIdentifier(node) && node.text === fromName) {
				const sym = checker.getSymbolAtLocation(node);
				if (sym) {
					renamedSymbols.set(sym, newName);
				}
			}
			node.forEachChild(visit);
		};
		visit(statement);
	}

	lap('collision resolution');
	// ---- external re-exports: `export { X as Y } from 'ext'` ---------------------
	for (const target of exportedByTarget.keys()) {
		const sourceFile = symbolSourceFile(target);
		if (!sourceFile || classify(sourceFile) !== 'imported') {
			continue;
		}
		addImportedSymbol(sourceFile.fileName, target);
	}

	lap('external re-exports');
	// ---- emit ---------------------------------------------------------------------
	const parts: string[] = [];

	const ourNameForSymbol = (sym: Symbol): string => {
		const renamed = renamedSymbols.get(sym);
		if (renamed !== undefined) {
			return renamed;
		}
		// an imported symbol may only be known by the name it was imported with
		const infos = exportedByTarget.get(sym);
		return infos && infos.length !== 0 ? infos[0].localName : sym.name;
	};

	// imports
	const importLines: string[] = [];
	for (const [specifier, symbols] of [...importedRefs.entries()].sort((a, b) => a[0].localeCompare(b[0]))) {
		const names = [...symbols].map((s) => ourNameForSymbol(s)).sort((a, b) => a.localeCompare(b));
		importLines.push(`import { ${[...new Set(names)].join(', ')} } from '${specifier}';`);
	}
	if (importLines.length) {
		parts.push(importLines.join('\n'));
	}

	// statements: entry-file declarations first, then the rest (stable order)
	// Emit in the compiler's own file order (dependency-first, like
	// `program.getSourceFiles()`) and source order within each file. This keeps the
	// output comparable with dts-bundle-generator's, which walks the program too.
	const sourceFileOrder = new Map<string, number>();
	program.getSourceFileNames().forEach((fileName, index) => {
		sourceFileOrder.set(fileName, index);
	});
	const statementOrder = new Map<ast.Node, number>();
	statements.forEach((statement, index) => {
		statementOrder.set(statement, index);
	});

	const orderedStatements = [...new Set(statements)].sort((a, b) => {
		const aFile = a.getSourceFile().fileName;
		const bFile = b.getSourceFile().fileName;
		if (aFile !== bFile) {
			return (sourceFileOrder.get(aFile) ?? Number.MAX_SAFE_INTEGER)
				- (sourceFileOrder.get(bFile) ?? Number.MAX_SAFE_INTEGER);
		}
		if (a.pos !== b.pos) {
			return a.pos - b.pos;
		}
		return (statementOrder.get(a) ?? 0) - (statementOrder.get(b) ?? 0);
	});

	// Emit the *original source text* of each statement and apply edits (renames and
	// modifier changes) as text ranges. Printing the AST would drop every JSDoc
	// comment and reformat the code, which makes the bundle impossible to diff
	// against dts-bundle-generator's output.
	interface TextEdit {
		start: number;
		end: number;
		text: string;
	}

	const modifierKeywords = new Set<ast.SyntaxKind>([
		ast.SyntaxKind.ExportKeyword,
		ast.SyntaxKind.DefaultKeyword,
		ast.SyntaxKind.DeclareKeyword,
	]);

	/** Whether an `import("x").Type` refers to a module that gets inlined into the bundle. */
	/**
	 * The name an `import("mod").Name` should be spelled with in the bundle, or
	 * `undefined` when it has to be left as is (no qualifier or `default`, a name that
	 * does not resolve, or a package we are not importing from).
	 */
	const importTypeTargetName = (node: ast.ImportTypeNode): string | undefined => {
		const qualifier = node.qualifier;
		if (qualifier === undefined || !ast.isIdentifier(qualifier) || qualifier.text === 'default') {
			return undefined;
		}
		checkerCalls++;
		const symbol = checker.getSymbolAtLocation(qualifier);
		if (symbol === undefined) {
			return undefined;
		}
		const target = resolveAlias(symbol);
		const sourceFile = symbolSourceFile(target);
		if (sourceFile === undefined) {
			return undefined;
		}

		// the module is inlined into the bundle, so its declarations are already in scope
		if (classify(sourceFile) === 'inlined') {
			return ourNameForSymbol(target);
		}

		// an external module: only names that end up in an `import` statement can be used
		const specifier = moduleSpecifierByPath.get(sourceFile.fileName);
		const imported = specifier !== undefined && importedRefs.get(specifier)?.has(target) === true;
		return imported ? ourNameForSymbol(target) : undefined;
	};

	// Only identifiers spelled like a renamed declaration can possibly need a rename,
	// so the (comparatively expensive) checker round trip is skipped for all others.
	const renamedOriginalNames = new Set<string>();
	for (const symbol of renamedSymbols.keys()) {
		renamedOriginalNames.add(symbol.name);
	}

	const emitStatementText = (statement: ast.Node, keepExport: boolean): string => {
		const sourceFile = statement.getSourceFile();
		const edits: TextEdit[] = [];

		// renames
		const visit = (node: ast.Node): void => {
			if (ast.isIdentifier(node) && renamedOriginalNames.has(node.text)
				&& !(ast.isImportTypeNode(node.parent) && node.parent.qualifier === node)) {
				checkerCalls++;
				const symbol = checker.getSymbolAtLocation(node);
				if (symbol) {
					const newName = renamedSymbols.get(symbol);
					if (newName !== undefined) {
						edits.push({ start: node.getStart(sourceFile), end: node.getEnd(), text: newName });
					}
				}
			}

			// `import("./mod").Type` -> `Type`: the module is inlined, or imported anyway,
			// so the specifier carries no information and would only duplicate the import
			if (ast.isImportTypeNode(node) && node.qualifier !== undefined) {
				const qualifier = node.qualifier;
				const targetName = importTypeTargetName(node);
				if (targetName !== undefined && ast.isIdentifier(qualifier)) {
					edits.push({
						start: node.getStart(sourceFile),
						end: qualifier.getStart(sourceFile),
						text: '',
					});
					if (targetName !== qualifier.text) {
						edits.push({
							start: qualifier.getStart(sourceFile),
							end: qualifier.getEnd(),
							text: targetName,
						});
					}
				}
			}

			node.forEachChild(visit);
		};
		visit(statement);

		// modifiers: `export`/`declare` are added or removed to match what the bundle needs.
		const modifiers = ((statement as { modifiers?: readonly ast.Node[] }).modifiers ?? [])
			.filter((modifier) => modifierKeywords.has(modifier.kind));

		const hasExport = modifiers.some((modifier) => modifier.kind === ast.SyntaxKind.ExportKeyword);
		const hasDeclare = modifiers.some((modifier) => modifier.kind === ast.SyntaxKind.DeclareKeyword);

		for (const modifier of modifiers) {
			// `declare` is never removed: `export declare class` is valid, and dropping it
			// from a non-exported value declaration would require re-adding it right away
			const shouldRemove = modifier.kind === ast.SyntaxKind.DefaultKeyword
				|| (!keepExport && modifier.kind === ast.SyntaxKind.ExportKeyword);
			if (!shouldRemove) {
				continue;
			}
			let end = modifier.getEnd();
			// swallow the whitespace that follows the keyword
			while (end < sourceFile.text.length && (sourceFile.text[end] === ' ' || sourceFile.text[end] === '\t')) {
				end += 1;
			}
			edits.push({ start: modifier.getStart(sourceFile), end, text: '' });
		}

		// keyword(s) to add in front of the statement, in this order
		const prefix: string[] = [];
		if (keepExport && !hasExport) {
			// the declaration is exported by the entry but written without `export` in its file
			prefix.push('export');
		}
		// a value declaration without `export` has to be `declare`d in a .d.ts
		if (VALUABLE_KINDS.has(statement.kind) && !keepExport && !hasDeclare) {
			prefix.push('declare');
		}
		if (prefix.length !== 0) {
			const insertAt = statement.getStart(sourceFile);
			edits.push({ start: insertAt, end: insertAt, text: `${prefix.join(' ')} ` });
		}

		let text = sourceFile.text.slice(statement.getFullStart(), statement.getEnd());
		const baseOffset = statement.getFullStart();
		const sortedEdits = [...edits].sort((a, b) => b.start - a.start);
		for (const edit of sortedEdits) {
			text = text.slice(0, edit.start - baseOffset) + edit.text + text.slice(edit.end - baseOffset);
		}

		// drop the blank lines that belonged to the previous statement
		text = text.replace(/^(?:[ \t]*\r?\n)+/, '');

		// dedent: a statement promoted out of an indented block (e.g. the body of an inlined
		// relative `declare module "./x"`) carries its original indentation
		const statementStart = statement.getStart(sourceFile);
		const lineStart = sourceFile.text.lastIndexOf('\n', statementStart - 1) + 1;
		const indent = sourceFile.text.slice(lineStart, statementStart);
		if (indent !== '') {
			const lines = text.split('\n');
			lines[0] = lines[0].trimStart();
			for (let index = 1; index < lines.length; index += 1) {
				if (lines[index].startsWith(indent)) {
					lines[index] = lines[index].slice(indent.length);
				}
			}
			text = lines.join('\n');
		}

		return text.trimEnd();
	};

	// dts-bundle-generator joins top-level statements with a single newline (only separate
	// sections - banner, imports, statements, exports - are separated by a blank line)
	lap('imports + ordering');
	const statementTexts: string[] = [];
	for (const st of orderedStatements) {
		const isValuable = VALUABLE_KINDS.has(st.kind);
		const isTypeStatement = ast.isInterfaceDeclaration(st) || ast.isTypeAliasDeclaration(st);
		// "valuable" statements (classes/functions/vars/enums/modules) are exported only when
		// the entry exports them; types are exported unless `exportReferencedTypes` is disabled
		const keepExport = isStatementExported(st) || (!isValuable && !isTypeStatement) || (isTypeStatement && options.exportReferencedTypes);
		if (process.env.DTSBUNDLER_DEBUG !== undefined) {
			const name = (st as { name?: ast.Identifier }).name?.text ?? '?';
			console.error(`[dtsbundler] ${name}: kind=${ast.SyntaxKind[st.kind]} exported=${isStatementExported(st)} keepExport=${Boolean(keepExport)}`);
		}
		statementTexts.push(emitStatementText(st, keepExport));
	}
	lap('emit statements');
	if (statementTexts.length !== 0) {
		parts.push(statementTexts.join('\n'));
	}

	// export block: renames + external re-exports
	const exportEntries: string[] = [];
	for (const [target, infos] of exportedByTarget) {
		const sourceFile = symbolSourceFile(target);
		const localName = ourNameForSymbol(target);
		const isImported = Boolean(sourceFile && classify(sourceFile) === 'imported');
		for (const info of infos) {
			if (localName === info.exportedName) {
				// only worth restating for imported symbols (their local name is the import to re-export)
				if (isImported) {
					exportEntries.push(localName);
				}
			} else {
				exportEntries.push(`${localName} as ${info.exportedName}`);
			}
		}
	}
	lap('export block');
	if (exportEntries.length) {
		parts.push(`export {\n\t${[...new Set(exportEntries)].sort().join(',\n\t')},\n};`);
	}

	parts.push('export {};');

	return parts.join('\n\n');
}
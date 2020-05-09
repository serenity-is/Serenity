var Serenity;
(function (Serenity) {
    var FilterOperators;
    (function (FilterOperators) {
        FilterOperators.isTrue = 'true';
        FilterOperators.isFalse = 'false';
        FilterOperators.contains = 'contains';
        FilterOperators.startsWith = 'startswith';
        FilterOperators.EQ = 'eq';
        FilterOperators.NE = 'ne';
        FilterOperators.GT = 'gt';
        FilterOperators.GE = 'ge';
        FilterOperators.LT = 'lt';
        FilterOperators.LE = 'le';
        FilterOperators.BW = 'bw';
        FilterOperators.IN = 'in';
        FilterOperators.isNull = 'isnull';
        FilterOperators.isNotNull = 'isnotnull';
        FilterOperators.toCriteriaOperator = {
            eq: '=',
            ne: '!=',
            gt: '>',
            ge: '>=',
            lt: '<',
            le: '<='
        };
    })(FilterOperators = Serenity.FilterOperators || (Serenity.FilterOperators = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FilterStore = /** @class */ (function () {
        function FilterStore(fields) {
            this.items = [];
            if (fields == null) {
                throw new Q.ArgumentNullException('source');
            }
            this.fields = fields.slice();
            this.get_fields().sort(function (x, y) {
                var titleX = Q.tryGetText(x.title);
                if (titleX == null) {
                    titleX = x.title;
                    if (titleX == null)
                        titleX = x.name;
                }
                var titleY = Q.tryGetText(y.title);
                if (titleY == null) {
                    titleY = y.title;
                    if (titleY == null)
                        titleY = y.name;
                }
                return Q.Culture.stringCompare(titleX, titleY);
            });
            this.fieldByName = {};
            for (var _i = 0, fields_1 = fields; _i < fields_1.length; _i++) {
                var field = fields_1[_i];
                this.get_fieldByName()[field.name] = field;
            }
        }
        FilterStore_1 = FilterStore;
        FilterStore.getCriteriaFor = function (items) {
            if (items == null)
                return [''];
            var inParens = false;
            var currentBlock = [''];
            var isBlockOr = false;
            var criteria = [''];
            for (var i = 0; i < items.length; i++) {
                var line = items[i];
                if (line.leftParen || inParens && line.rightParen) {
                    if (!Serenity.Criteria.isEmpty(currentBlock)) {
                        if (inParens)
                            currentBlock = Serenity.Criteria.paren(currentBlock);
                        if (isBlockOr)
                            criteria = Serenity.Criteria.join(criteria, 'or', currentBlock);
                        else
                            criteria = Serenity.Criteria.join(criteria, 'and', currentBlock);
                        currentBlock = [''];
                    }
                    inParens = false;
                }
                if (line.leftParen) {
                    isBlockOr = line.isOr;
                    inParens = true;
                }
                if (line.isOr)
                    currentBlock = Serenity.Criteria.join(currentBlock, 'or', line.criteria);
                else
                    currentBlock = Serenity.Criteria.join(currentBlock, 'and', line.criteria);
            }
            if (!Serenity.Criteria.isEmpty(currentBlock)) {
                if (isBlockOr)
                    criteria = Serenity.Criteria.join(criteria, 'or', Serenity.Criteria.paren(currentBlock));
                else
                    criteria = Serenity.Criteria.join(criteria, 'and', Serenity.Criteria.paren(currentBlock));
            }
            return criteria;
        };
        FilterStore.getDisplayTextFor = function (items) {
            if (items == null)
                return '';
            var inParens = false;
            var displayText = '';
            for (var i = 0; i < items.length; i++) {
                var line = items[i];
                if (inParens && (line.rightParen || line.leftParen)) {
                    displayText += ')';
                    inParens = false;
                }
                if (displayText.length > 0) {
                    displayText += ' ' + Q.text('Controls.FilterPanel.' +
                        (line.isOr ? 'Or' : 'And')) + ' ';
                }
                if (line.leftParen) {
                    displayText += '(';
                    inParens = true;
                }
                displayText += line.displayText;
            }
            if (inParens) {
                displayText += ')';
            }
            return displayText;
        };
        FilterStore.prototype.get_fields = function () {
            return this.fields;
        };
        FilterStore.prototype.get_fieldByName = function () {
            return this.fieldByName;
        };
        FilterStore.prototype.get_items = function () {
            return this.items;
        };
        FilterStore.prototype.raiseChanged = function () {
            this.displayText = null;
            this.changed && this.changed(this, {});
        };
        FilterStore.prototype.add_changed = function (value) {
            this.changed = Q.delegateCombine(this.changed, value);
        };
        FilterStore.prototype.remove_changed = function (value) {
            this.changed = Q.delegateRemove(this.changed, value);
        };
        FilterStore.prototype.get_activeCriteria = function () {
            return FilterStore_1.getCriteriaFor(this.items);
        };
        FilterStore.prototype.get_displayText = function () {
            if (this.displayText == null)
                this.displayText = FilterStore_1.getDisplayTextFor(this.items);
            return this.displayText;
        };
        var FilterStore_1;
        FilterStore = FilterStore_1 = __decorate([
            Serenity.Decorators.registerClass('FilterStore')
        ], FilterStore);
        return FilterStore;
    }());
    Serenity.FilterStore = FilterStore;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var IFiltering = /** @class */ (function () {
        function IFiltering() {
        }
        IFiltering = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IFiltering')
        ], IFiltering);
        return IFiltering;
    }());
    Serenity.IFiltering = IFiltering;
    var IQuickFiltering = /** @class */ (function () {
        function IQuickFiltering() {
        }
        IQuickFiltering = __decorate([
            Serenity.Decorators.registerInterface('Serenity.IQuickFiltering')
        ], IQuickFiltering);
        return IQuickFiltering;
    }());
    Serenity.IQuickFiltering = IQuickFiltering;
    var Operators = Serenity.FilterOperators;
    var Option = Serenity.Decorators.option;
    var BaseFiltering = /** @class */ (function () {
        function BaseFiltering() {
        }
        BaseFiltering.prototype.get_field = function () {
            return this.field;
        };
        BaseFiltering.prototype.set_field = function (value) {
            this.field = value;
        };
        BaseFiltering.prototype.get_container = function () {
            return this.container;
        };
        BaseFiltering.prototype.set_container = function (value) {
            this.container = value;
        };
        BaseFiltering.prototype.get_operator = function () {
            return this.operator;
        };
        BaseFiltering.prototype.set_operator = function (value) {
            this.operator = value;
        };
        BaseFiltering.prototype.appendNullableOperators = function (list) {
            if (!this.isNullable()) {
                return list;
            }
            list.push({ key: Serenity.FilterOperators.isNotNull });
            list.push({ key: Serenity.FilterOperators.isNull });
            return list;
        };
        BaseFiltering.prototype.appendComparisonOperators = function (list) {
            list.push({ key: Serenity.FilterOperators.EQ });
            list.push({ key: Serenity.FilterOperators.NE });
            list.push({ key: Serenity.FilterOperators.LT });
            list.push({ key: Serenity.FilterOperators.LE });
            list.push({ key: Serenity.FilterOperators.GT });
            list.push({ key: Serenity.FilterOperators.GE });
            return list;
        };
        BaseFiltering.prototype.isNullable = function () {
            return this.get_field().required !== true;
        };
        BaseFiltering.prototype.createEditor = function () {
            switch (this.get_operator().key) {
                case 'true':
                case 'false':
                case 'isnull':
                case 'isnotnull': {
                    return;
                }
                case 'contains':
                case 'startswith':
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    this.get_container().html('<input type="text"/>');
                    return;
                }
            }
            throw new Q.Exception(Q.format("Filtering '{0}' has no editor for '{1}' operator", Q.getTypeName(Q.getInstanceType(this)), this.get_operator().key));
        };
        BaseFiltering.prototype.operatorFormat = function (op) {
            return Q.coalesce(op.format, Q.coalesce(Q.tryGetText('Controls.FilterPanel.OperatorFormats.' + op.key), op.key));
        };
        BaseFiltering.prototype.getTitle = function (field) {
            return Q.coalesce(Q.tryGetText(field.title), Q.coalesce(field.title, field.name));
        };
        BaseFiltering.prototype.displayText = function (op, values) {
            if (!values || values.length === 0) {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field));
            }
            else if (values.length === 1) {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field), values[0]);
            }
            else {
                return Q.format(this.operatorFormat(op), this.getTitle(this.field), values[0], values[1]);
            }
        };
        BaseFiltering.prototype.getCriteriaField = function () {
            return this.field.name;
        };
        BaseFiltering.prototype.getCriteria = function () {
            var result = {};
            var text;
            switch (this.get_operator().key) {
                case 'true': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = [[this.getCriteriaField()], '=', true];
                    return result;
                }
                case 'false': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = [[this.getCriteriaField()], '=', false];
                    return result;
                }
                case 'isnull': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = ['is null', [this.getCriteriaField()]];
                    return result;
                }
                case 'isnotnull': {
                    result.displayText = this.displayText(this.get_operator(), []);
                    result.criteria = ['is not null', [this.getCriteriaField()]];
                    return result;
                }
                case 'contains': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], 'like', '%' + text + '%'];
                    return result;
                }
                case 'startswith': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], 'like', text + '%'];
                    return result;
                }
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    text = this.getEditorText();
                    result.displayText = this.displayText(this.get_operator(), [text]);
                    result.criteria = [[this.getCriteriaField()], Serenity.FilterOperators.toCriteriaOperator[this.get_operator().key], this.getEditorValue()];
                    return result;
                }
            }
            throw new Q.Exception(Q.format("Filtering '{0}' has no handler for '{1}' operator", Q.getTypeName(Q.getInstanceType(this)), this.get_operator().key));
        };
        BaseFiltering.prototype.loadState = function (state) {
            var input = this.get_container().find(':input').first();
            input.val(state);
        };
        BaseFiltering.prototype.saveState = function () {
            switch (this.get_operator().key) {
                case 'contains':
                case 'startswith':
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    var input = this.get_container().find(':input').first();
                    return input.val();
                }
            }
            return null;
        };
        BaseFiltering.prototype.argumentNull = function () {
            return new Q.ArgumentNullException('value', Q.text('Controls.FilterPanel.ValueRequired'));
        };
        BaseFiltering.prototype.validateEditorValue = function (value) {
            if (value.length === 0) {
                throw this.argumentNull();
            }
            return value;
        };
        BaseFiltering.prototype.getEditorValue = function () {
            var input = this.get_container().find(':input').not('.select2-focusser').first();
            if (input.length !== 1) {
                throw new Q.Exception(Q.format("Couldn't find input in filter container for {0}", Q.coalesce(this.field.title, this.field.name)));
            }
            var value;
            if (input.data('select2') != null) {
                value = input.select2('val');
            }
            else {
                value = input.val();
            }
            value = Q.coalesce(value, '').trim();
            return this.validateEditorValue(value);
        };
        BaseFiltering.prototype.getEditorText = function () {
            var input = this.get_container().find(':input').not('.select2-focusser').not('.select2-input').first();
            if (input.length === 0) {
                return this.get_container().text().trim();
            }
            var value;
            if (input.data('select2') != null) {
                value = Q.coalesce(input.select2('data'), {}).text;
            }
            else {
                value = input.val();
            }
            return value;
        };
        BaseFiltering.prototype.initQuickFilter = function (filter) {
            filter.field = this.getCriteriaField();
            filter.type = Serenity.StringEditor;
            filter.title = this.getTitle(this.field);
            filter.options = Q.deepClone(this.get_field().quickFilterParams);
        };
        BaseFiltering = __decorate([
            Serenity.Decorators.registerClass('Serenity.BaseFiltering', [IFiltering, IQuickFiltering])
        ], BaseFiltering);
        return BaseFiltering;
    }());
    Serenity.BaseFiltering = BaseFiltering;
    function Filtering(name) {
        return Serenity.Decorators.registerClass('Serenity.' + name + 'Filtering');
    }
    var BaseEditorFiltering = /** @class */ (function (_super) {
        __extends(BaseEditorFiltering, _super);
        function BaseEditorFiltering(editorType) {
            var _this = _super.call(this) || this;
            _this.editorType = editorType;
            return _this;
        }
        BaseEditorFiltering.prototype.useEditor = function () {
            switch (this.get_operator().key) {
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge':
                    return true;
            }
            return false;
        };
        BaseEditorFiltering.prototype.createEditor = function () {
            if (this.useEditor()) {
                this.editor = Serenity.Widget.create({
                    type: this.editorType,
                    container: this.get_container(),
                    options: this.getEditorOptions(),
                    init: null
                });
                return;
            }
            this.editor = null;
            _super.prototype.createEditor.call(this);
        };
        BaseEditorFiltering.prototype.useIdField = function () {
            return false;
        };
        BaseEditorFiltering.prototype.getCriteriaField = function () {
            if (this.useEditor() &&
                this.useIdField() &&
                !Q.isEmptyOrNull(this.get_field().filteringIdField)) {
                return this.get_field().filteringIdField;
            }
            return _super.prototype.getCriteriaField.call(this);
        };
        BaseEditorFiltering.prototype.getEditorOptions = function () {
            var opt = Q.deepClone(this.get_field().editorParams || {});
            delete opt['cascadeFrom'];
            // currently can't support cascadeFrom in filtering
            return Q.extend(opt, this.get_field().filteringParams);
        };
        BaseEditorFiltering.prototype.loadState = function (state) {
            if (this.useEditor()) {
                if (state == null) {
                    return;
                }
                Serenity.EditorUtils.setValue(this.editor, state);
                return;
            }
            _super.prototype.loadState.call(this, state);
        };
        BaseEditorFiltering.prototype.saveState = function () {
            if (this.useEditor()) {
                return Serenity.EditorUtils.getValue(this.editor);
            }
            return _super.prototype.saveState.call(this);
        };
        BaseEditorFiltering.prototype.getEditorValue = function () {
            if (this.useEditor()) {
                var value = Serenity.EditorUtils.getValue(this.editor);
                if (value == null || (typeof value == "string" && value.trim().length === 0))
                    throw this.argumentNull();
                return value;
            }
            return _super.prototype.getEditorValue.call(this);
        };
        BaseEditorFiltering.prototype.initQuickFilter = function (filter) {
            _super.prototype.initQuickFilter.call(this, filter);
            filter.type = this.editorType;
            filter.options = Q.extend(Q.extend({}, Q.deepClone(this.getEditorOptions())), Q.deepClone(this.get_field().quickFilterParams));
        };
        BaseEditorFiltering = __decorate([
            Filtering('BaseEditor')
        ], BaseEditorFiltering);
        return BaseEditorFiltering;
    }(BaseFiltering));
    Serenity.BaseEditorFiltering = BaseEditorFiltering;
    var DateFiltering = /** @class */ (function (_super) {
        __extends(DateFiltering, _super);
        function DateFiltering() {
            return _super.call(this, Serenity.DateEditor) || this;
        }
        DateFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators(this.appendComparisonOperators([]));
        };
        DateFiltering = __decorate([
            Filtering('Date')
        ], DateFiltering);
        return DateFiltering;
    }(BaseEditorFiltering));
    Serenity.DateFiltering = DateFiltering;
    var BooleanFiltering = /** @class */ (function (_super) {
        __extends(BooleanFiltering, _super);
        function BooleanFiltering() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        BooleanFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators([
                { key: Serenity.FilterOperators.isTrue },
                { key: Serenity.FilterOperators.isFalse }
            ]);
        };
        BooleanFiltering = __decorate([
            Filtering('Boolean')
        ], BooleanFiltering);
        return BooleanFiltering;
    }(BaseFiltering));
    Serenity.BooleanFiltering = BooleanFiltering;
    var DateTimeFiltering = /** @class */ (function (_super) {
        __extends(DateTimeFiltering, _super);
        function DateTimeFiltering() {
            return _super.call(this, Serenity.DateTimeEditor) || this;
        }
        DateTimeFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators(this.appendComparisonOperators([]));
        };
        DateTimeFiltering.prototype.getCriteria = function () {
            var result = {};
            switch (this.get_operator().key) {
                case 'eq':
                case 'ne':
                case 'lt':
                case 'le':
                case 'gt':
                case 'ge': {
                    {
                        var text = this.getEditorText();
                        result.displayText = this.displayText(this.get_operator(), [text]);
                        var date = Q.parseISODateTime(this.getEditorValue());
                        date = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                        var next = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);
                        var criteria = [this.getCriteriaField()];
                        var dateValue = Q.formatDate(date, 'yyyy-MM-dd');
                        var nextValue = Q.formatDate(next, 'yyyy-MM-dd');
                        switch (this.get_operator().key) {
                            case 'eq': {
                                result.criteria = Serenity.Criteria.join([criteria, '>=', dateValue], 'and', [criteria, '<', nextValue]);
                                return result;
                            }
                            case 'ne': {
                                result.criteria = Serenity.Criteria.paren(Serenity.Criteria.join([criteria, '<', dateValue], 'or', [criteria, '>', nextValue]));
                                return result;
                            }
                            case 'lt': {
                                result.criteria = [criteria, '<', dateValue];
                                return result;
                            }
                            case 'le': {
                                result.criteria = [criteria, '<', nextValue];
                                return result;
                            }
                            case 'gt': {
                                result.criteria = [criteria, '>=', nextValue];
                                return result;
                            }
                            case 'ge': {
                                result.criteria = [criteria, '>=', dateValue];
                                return result;
                            }
                        }
                    }
                    break;
                }
            }
            return _super.prototype.getCriteria.call(this);
        };
        DateTimeFiltering = __decorate([
            Filtering('DateTime')
        ], DateTimeFiltering);
        return DateTimeFiltering;
    }(BaseEditorFiltering));
    Serenity.DateTimeFiltering = DateTimeFiltering;
    var DecimalFiltering = /** @class */ (function (_super) {
        __extends(DecimalFiltering, _super);
        function DecimalFiltering() {
            return _super.call(this, Serenity.DecimalEditor) || this;
        }
        DecimalFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators(this.appendComparisonOperators([]));
        };
        DecimalFiltering = __decorate([
            Filtering('Decimal')
        ], DecimalFiltering);
        return DecimalFiltering;
    }(BaseEditorFiltering));
    Serenity.DecimalFiltering = DecimalFiltering;
    var EditorFiltering = /** @class */ (function (_super) {
        __extends(EditorFiltering, _super);
        function EditorFiltering() {
            return _super.call(this, Serenity.Widget) || this;
        }
        EditorFiltering.prototype.getOperators = function () {
            var list = [];
            list.push({ key: Operators.EQ });
            list.push({ key: Operators.NE });
            if (this.useRelative) {
                list.push({ key: Operators.LT });
                list.push({ key: Operators.LE });
                list.push({ key: Operators.GT });
                list.push({ key: Operators.GE });
            }
            if (this.useLike) {
                list.push({ key: Operators.contains });
                list.push({ key: Operators.startsWith });
            }
            this.appendNullableOperators(list);
            return list;
        };
        EditorFiltering.prototype.useEditor = function () {
            var op = this.get_operator().key;
            return op === Operators.EQ ||
                op === Operators.NE ||
                (this.useRelative && (op === Operators.LT ||
                    op === Operators.LE ||
                    op === Operators.GT ||
                    op === Operators.GE));
        };
        EditorFiltering.prototype.getEditorOptions = function () {
            var opt = _super.prototype.getEditorOptions.call(this);
            if (this.useEditor() && this.editorType === Q.coalesce(this.get_field().editorType, 'String')) {
                opt = Q.extend(opt, this.get_field().editorParams);
            }
            return opt;
        };
        EditorFiltering.prototype.createEditor = function () {
            var _this = this;
            if (this.useEditor()) {
                var editorType = Serenity.EditorTypeRegistry.get(this.editorType);
                this.editor = Serenity.Widget.create({
                    type: editorType,
                    element: function (e) { return e.appendTo(_this.get_container()); },
                    options: this.getEditorOptions()
                });
                return;
            }
            _super.prototype.createEditor.call(this);
        };
        EditorFiltering.prototype.useIdField = function () {
            return this.useEditor();
        };
        EditorFiltering.prototype.initQuickFilter = function (filter) {
            _super.prototype.initQuickFilter.call(this, filter);
            filter.type = Serenity.EditorTypeRegistry.get(this.editorType);
        };
        __decorate([
            Option()
        ], EditorFiltering.prototype, "editorType", void 0);
        __decorate([
            Option()
        ], EditorFiltering.prototype, "useRelative", void 0);
        __decorate([
            Option()
        ], EditorFiltering.prototype, "useLike", void 0);
        EditorFiltering = __decorate([
            Filtering('Editor')
        ], EditorFiltering);
        return EditorFiltering;
    }(BaseEditorFiltering));
    Serenity.EditorFiltering = EditorFiltering;
    var EnumFiltering = /** @class */ (function (_super) {
        __extends(EnumFiltering, _super);
        function EnumFiltering() {
            return _super.call(this, Serenity.EnumEditor) || this;
        }
        EnumFiltering.prototype.getOperators = function () {
            var op = [{ key: Operators.EQ }, { key: Operators.NE }];
            return this.appendNullableOperators(op);
        };
        EnumFiltering = __decorate([
            Filtering('Enum')
        ], EnumFiltering);
        return EnumFiltering;
    }(BaseEditorFiltering));
    Serenity.EnumFiltering = EnumFiltering;
    var IntegerFiltering = /** @class */ (function (_super) {
        __extends(IntegerFiltering, _super);
        function IntegerFiltering() {
            return _super.call(this, Serenity.IntegerEditor) || this;
        }
        IntegerFiltering.prototype.getOperators = function () {
            return this.appendNullableOperators(this.appendComparisonOperators([]));
        };
        IntegerFiltering = __decorate([
            Filtering('Integer')
        ], IntegerFiltering);
        return IntegerFiltering;
    }(BaseEditorFiltering));
    Serenity.IntegerFiltering = IntegerFiltering;
    var LookupFiltering = /** @class */ (function (_super) {
        __extends(LookupFiltering, _super);
        function LookupFiltering() {
            return _super.call(this, Serenity.LookupEditor) || this;
        }
        LookupFiltering.prototype.getOperators = function () {
            var ops = [{ key: Operators.EQ }, { key: Operators.NE }, { key: Operators.contains }, { key: Operators.startsWith }];
            return this.appendNullableOperators(ops);
        };
        LookupFiltering.prototype.useEditor = function () {
            var op = this.get_operator().key;
            return op == Operators.EQ || op == Operators.NE;
        };
        LookupFiltering.prototype.useIdField = function () {
            return this.useEditor();
        };
        LookupFiltering.prototype.getEditorText = function () {
            if (this.useEditor()) {
                return this.editor.text;
            }
            return _super.prototype.getEditorText.call(this);
        };
        LookupFiltering = __decorate([
            Filtering('Lookup')
        ], LookupFiltering);
        return LookupFiltering;
    }(BaseEditorFiltering));
    Serenity.LookupFiltering = LookupFiltering;
    var ServiceLookupFiltering = /** @class */ (function (_super) {
        __extends(ServiceLookupFiltering, _super);
        function ServiceLookupFiltering() {
            return _super.call(this, Serenity.ServiceLookupEditor) || this;
        }
        ServiceLookupFiltering.prototype.getOperators = function () {
            var ops = [{ key: Operators.EQ }, { key: Operators.NE }, { key: Operators.contains }, { key: Operators.startsWith }];
            return this.appendNullableOperators(ops);
        };
        ServiceLookupFiltering.prototype.useEditor = function () {
            var op = this.get_operator().key;
            return op == Operators.EQ || op == Operators.NE;
        };
        ServiceLookupFiltering.prototype.useIdField = function () {
            return this.useEditor();
        };
        ServiceLookupFiltering.prototype.getEditorText = function () {
            if (this.useEditor()) {
                return this.editor.text;
            }
            return _super.prototype.getEditorText.call(this);
        };
        ServiceLookupFiltering = __decorate([
            Filtering('ServiceLookup')
        ], ServiceLookupFiltering);
        return ServiceLookupFiltering;
    }(BaseEditorFiltering));
    Serenity.ServiceLookupFiltering = ServiceLookupFiltering;
    var StringFiltering = /** @class */ (function (_super) {
        __extends(StringFiltering, _super);
        function StringFiltering() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        StringFiltering.prototype.getOperators = function () {
            var ops = [
                { key: Operators.contains },
                { key: Operators.startsWith },
                { key: Operators.EQ },
                { key: Operators.NE }
            ];
            return this.appendNullableOperators(ops);
        };
        StringFiltering.prototype.validateEditorValue = function (value) {
            if (value.length === 0) {
                return value;
            }
            return _super.prototype.validateEditorValue.call(this, value);
        };
        StringFiltering = __decorate([
            Filtering('String')
        ], StringFiltering);
        return StringFiltering;
    }(BaseFiltering));
    Serenity.StringFiltering = StringFiltering;
    var FilteringTypeRegistry;
    (function (FilteringTypeRegistry) {
        var knownTypes;
        function initialize() {
            if (knownTypes != null)
                return;
            knownTypes = {};
            for (var _i = 0, _a = Q.getTypes(); _i < _a.length; _i++) {
                var type = _a[_i];
                if (!Q.isAssignableFrom(Serenity.IFiltering, type))
                    continue;
                var fullName = Q.getTypeFullName(type).toLowerCase();
                knownTypes[fullName] = type;
                for (var _b = 0, _c = Q.Config.rootNamespaces; _b < _c.length; _b++) {
                    var k = _c[_b];
                    if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                        var kx = fullName.substr(k.length + 1).toLowerCase();
                        if (knownTypes[kx] == null) {
                            knownTypes[kx] = type;
                        }
                    }
                }
            }
            setTypeKeysWithoutFilterHandlerSuffix();
        }
        function setTypeKeysWithoutFilterHandlerSuffix() {
            var suffix = 'filtering';
            for (var _i = 0, _a = Object.keys(knownTypes); _i < _a.length; _i++) {
                var k = _a[_i];
                if (!Q.endsWith(k, suffix))
                    continue;
                var p = k.substr(0, k.length - suffix.length);
                if (Q.isEmptyOrNull(p))
                    continue;
                if (knownTypes[p] != null)
                    continue;
                knownTypes[p] = knownTypes[k];
            }
        }
        function reset() {
            knownTypes = null;
        }
        function get(key) {
            if (Q.isEmptyOrNull(key))
                throw new Q.ArgumentNullException('key');
            initialize();
            var formatterType = knownTypes[key.toLowerCase()];
            if (formatterType == null)
                throw new Q.Exception(Q.format("Can't find {0} filter handler type!", key));
            return formatterType;
        }
        FilteringTypeRegistry.get = get;
    })(FilteringTypeRegistry = Serenity.FilteringTypeRegistry || (Serenity.FilteringTypeRegistry = {}));
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FilterWidgetBase = /** @class */ (function (_super) {
        __extends(FilterWidgetBase, _super);
        function FilterWidgetBase(div, opt) {
            var _this = _super.call(this, div, opt) || this;
            _this.store = new Serenity.FilterStore([]);
            _this.onFilterStoreChanged = function () { return _this.filterStoreChanged(); };
            _this.store.add_changed(_this.onFilterStoreChanged);
            return _this;
        }
        FilterWidgetBase.prototype.destroy = function () {
            if (this.store) {
                this.store.remove_changed(this.onFilterStoreChanged);
                this.onFilterStoreChanged = null;
                this.store = null;
            }
            _super.prototype.destroy.call(this);
        };
        FilterWidgetBase.prototype.filterStoreChanged = function () {
        };
        FilterWidgetBase.prototype.get_store = function () {
            return this.store;
        };
        FilterWidgetBase.prototype.set_store = function (value) {
            if (this.store !== value) {
                if (this.store != null)
                    this.store.remove_changed(this.onFilterStoreChanged);
                this.store = value || new Serenity.FilterStore([]);
                this.store.add_changed(this.onFilterStoreChanged);
                this.filterStoreChanged();
            }
        };
        FilterWidgetBase = __decorate([
            Serenity.Decorators.registerClass('Serenity.FilterWidgetBase')
        ], FilterWidgetBase);
        return FilterWidgetBase;
    }(Serenity.TemplatedWidget));
    Serenity.FilterWidgetBase = FilterWidgetBase;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FilterFieldSelect = /** @class */ (function (_super) {
        __extends(FilterFieldSelect, _super);
        function FilterFieldSelect(hidden, fields) {
            var _this = _super.call(this, hidden) || this;
            for (var _i = 0, fields_2 = fields; _i < fields_2.length; _i++) {
                var field = fields_2[_i];
                _this.addOption(field.name, Q.coalesce(Q.tryGetText(field.title), Q.coalesce(field.title, field.name)), field);
            }
            return _this;
        }
        FilterFieldSelect.prototype.emptyItemText = function () {
            if (Q.isEmptyOrNull(this.value)) {
                return Q.text('Controls.FilterPanel.SelectField');
            }
            return null;
        };
        FilterFieldSelect.prototype.getSelect2Options = function () {
            var opt = _super.prototype.getSelect2Options.call(this);
            opt.allowClear = false;
            return opt;
        };
        FilterFieldSelect = __decorate([
            Serenity.Decorators.registerClass('Serenity.FilterFieldSelect')
        ], FilterFieldSelect);
        return FilterFieldSelect;
    }(Serenity.Select2Editor));
    var FilterOperatorSelect = /** @class */ (function (_super) {
        __extends(FilterOperatorSelect, _super);
        function FilterOperatorSelect(hidden, source) {
            var _this = _super.call(this, hidden) || this;
            for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                var op = source_1[_i];
                var title = Q.coalesce(op.title, Q.coalesce(Q.tryGetText("Controls.FilterPanel.OperatorNames." + op.key), op.key));
                _this.addOption(op.key, title, op);
            }
            if (source.length && source[0])
                _this.value = source[0].key;
            return _this;
        }
        FilterOperatorSelect.prototype.emptyItemText = function () {
            return null;
        };
        FilterOperatorSelect.prototype.getSelect2Options = function () {
            var opt = _super.prototype.getSelect2Options.call(this);
            opt.allowClear = false;
            return opt;
        };
        FilterOperatorSelect = __decorate([
            Serenity.Decorators.registerClass('Serenity.FilterOperatorSelect')
        ], FilterOperatorSelect);
        return FilterOperatorSelect;
    }(Serenity.Select2Editor));
    var FilterPanel = /** @class */ (function (_super) {
        __extends(FilterPanel, _super);
        function FilterPanel(div) {
            var _this = _super.call(this, div) || this;
            _this.element.addClass('s-FilterPanel');
            _this.rowsDiv = _this.byId('Rows');
            _this.initButtons();
            _this.updateButtons();
            return _this;
        }
        FilterPanel.prototype.get_showInitialLine = function () {
            return this.showInitialLine;
        };
        FilterPanel.prototype.set_showInitialLine = function (value) {
            if (this.showInitialLine !== value) {
                this.showInitialLine = value;
                if (this.showInitialLine && this.rowsDiv.children().length === 0) {
                    this.addEmptyRow(false);
                }
            }
        };
        FilterPanel.prototype.filterStoreChanged = function () {
            _super.prototype.filterStoreChanged.call(this);
            this.updateRowsFromStore();
        };
        FilterPanel.prototype.updateRowsFromStore = function () {
            this.rowsDiv.empty();
            var items = this.get_store().get_items();
            for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                var item = items_1[_i];
                this.addEmptyRow(false);
                var row = this.rowsDiv.children().last();
                var divl = row.children('div.l');
                divl.children('.leftparen').toggleClass('active', !!item.leftParen);
                divl.children('.rightparen').toggleClass('active', !!item.rightParen);
                divl.children('.andor').toggleClass('or', !!item.isOr)
                    .text(Q.text((!!item.isOr ? 'Controls.FilterPanel.Or' :
                    'Controls.FilterPanel.And')));
                var fieldSelect = row.children('div.f')
                    .find('input.field-select').getWidget(FilterFieldSelect);
                fieldSelect.value = item.field;
                this.rowFieldChange(row);
                var operatorSelect = row.children('div.o')
                    .find('input.op-select').getWidget(FilterOperatorSelect);
                operatorSelect.set_value(item.operator);
                this.rowOperatorChange(row);
                var filtering = this.getFilteringFor(row);
                if (filtering != null) {
                    filtering.set_operator({ key: item.operator });
                    filtering.loadState(item.state);
                }
            }
            if (this.get_showInitialLine() && this.rowsDiv.children().length === 0) {
                this.addEmptyRow(false);
            }
            this.updateParens();
        };
        FilterPanel.prototype.get_showSearchButton = function () {
            return this.showSearchButton;
        };
        FilterPanel.prototype.set_showSearchButton = function (value) {
            if (this.showSearchButton !== value) {
                this.showSearchButton = value;
                this.updateButtons();
            }
        };
        FilterPanel.prototype.get_updateStoreOnReset = function () {
            return this.updateStoreOnReset;
        };
        FilterPanel.prototype.set_updateStoreOnReset = function (value) {
            if (this.updateStoreOnReset !== value) {
                this.updateStoreOnReset = value;
            }
        };
        FilterPanel.prototype.getTemplate = function () {
            return "<div id='~_Rows' class='filter-lines'>" +
                "</div>" +
                "<div id='~_Buttons' class='buttons'>" +
                "<button id='~_AddButton' class='btn btn-primary add'></button>" +
                "<button id='~_SearchButton' class='btn btn-success search'></button>" +
                "<button id='~_ResetButton' class='btn btn-danger reset'></button>" +
                "</div>" +
                "<div style='clear: both'>" +
                "</div>";
        };
        FilterPanel.prototype.initButtons = function () {
            var _this = this;
            this.byId('AddButton').text(Q.text('Controls.FilterPanel.AddFilter'))
                .click(function (e) { return _this.addButtonClick(e); });
            this.byId('SearchButton').text(Q.text('Controls.FilterPanel.SearchButton'))
                .click(function (e) { return _this.searchButtonClick(e); });
            this.byId('ResetButton').text(Q.text('Controls.FilterPanel.ResetButton'))
                .click(function (e) { return _this.resetButtonClick(e); });
        };
        FilterPanel.prototype.searchButtonClick = function (e) {
            e.preventDefault();
            this.search();
        };
        FilterPanel.prototype.get_hasErrors = function () {
            return this.rowsDiv.children().children('div.v')
                .children('span.error').length > 0;
        };
        FilterPanel.prototype.search = function () {
            this.rowsDiv.children().children('div.v')
                .children('span.error').remove();
            var filterLines = [];
            var errorText = null;
            var row = null;
            for (var i = 0; i < this.rowsDiv.children().length; i++) {
                row = this.rowsDiv.children().eq(i);
                var filtering = this.getFilteringFor(row);
                if (filtering == null) {
                    continue;
                }
                var field = this.getFieldFor(row);
                var op = row.children('div.o').find('input.op-select')
                    .getWidget(FilterOperatorSelect).value;
                if (op == null || op.length === 0) {
                    errorText = Q.text('Controls.FilterPanel.InvalidOperator');
                    break;
                }
                var line = {};
                line.field = field.name;
                line.operator = op;
                line.isOr = row.children('div.l')
                    .children('a.andor').hasClass('or');
                line.leftParen = row.children('div.l')
                    .children('a.leftparen').hasClass('active');
                line.rightParen = row.children('div.l')
                    .children('a.rightparen').hasClass('active');
                filtering.set_operator({ key: op });
                var criteria = filtering.getCriteria();
                line.criteria = criteria.criteria;
                line.state = filtering.saveState();
                line.displayText = criteria.displayText;
                filterLines.push(line);
            }
            // if an error occurred, display it, otherwise set current filters
            if (errorText != null) {
                $('<span/>').addClass('error')
                    .attr('title', errorText).appendTo(row.children('div.v'));
                row.children('div.v').find('input:first').focus();
                return;
            }
            var items = this.get_store().get_items();
            items.length = 0;
            items.push.apply(items, filterLines);
            this.get_store().raiseChanged();
        };
        FilterPanel.prototype.addButtonClick = function (e) {
            this.addEmptyRow(true);
            e.preventDefault();
        };
        FilterPanel.prototype.resetButtonClick = function (e) {
            e.preventDefault();
            if (this.get_updateStoreOnReset()) {
                if (this.get_store().get_items().length > 0) {
                    this.get_store().get_items().length = 0;
                    this.get_store().raiseChanged();
                }
            }
            this.rowsDiv.empty();
            this.updateButtons();
            if (this.get_showInitialLine()) {
                this.addEmptyRow(false);
            }
        };
        FilterPanel.prototype.findEmptyRow = function () {
            var result = null;
            this.rowsDiv.children().each(function (index, row) {
                var fieldInput = $(row).children('div.f')
                    .children('input.field-select').first();
                if (fieldInput.length === 0) {
                    return true;
                }
                var val = fieldInput.val();
                if (val == null || val.length === 0) {
                    result = $(row);
                    return false;
                }
                return true;
            });
            return result;
        };
        FilterPanel.prototype.addEmptyRow = function (popupField) {
            var _this = this;
            var emptyRow = this.findEmptyRow();
            if (emptyRow != null) {
                emptyRow.find('input.field-select').select2('focus');
                if (popupField) {
                    emptyRow.find('input.field-select').select2('open');
                }
                return emptyRow;
            }
            var isLastRowOr = this.rowsDiv.children().last()
                .children('a.andor').hasClass('or');
            var row = $("<div class='filter-line'>" +
                "<a class='delete'><span></span></a>" +
                "<div class='l'>" +
                "<a class='rightparen' href='#'>)</a>" +
                "<a class='andor' href='#'></a>" +
                "<a class='leftparen' href='#'>(</a>" +
                "</div>" +
                "<div class='f'>" +
                "<input type='hidden' class='field-select'>" +
                "</div>" +
                "<div class='o'></div>" +
                "<div class='v'></div>" +
                "<div style='clear: both'></div>" +
                "</div>").appendTo(this.rowsDiv);
            var parenDiv = row.children('div.l').hide();
            parenDiv.children('a.leftparen, a.rightparen')
                .click(function (e) { return _this.leftRightParenClick(e); });
            var andor = parenDiv.children('a.andor').attr('title', Q.text('Controls.FilterPanel.ChangeAndOr'));
            if (isLastRowOr) {
                andor.addClass('or').text(Q.text('Controls.FilterPanel.Or'));
            }
            else {
                andor.text(Q.text('Controls.FilterPanel.And'));
            }
            andor.click(function (e) { return _this.andOrClick(e); });
            row.children('a.delete')
                .attr('title', Q.text('Controls.FilterPanel.RemoveField'))
                .click(function (e) { return _this.deleteRowClick(e); });
            var fieldSel = new FilterFieldSelect(row.children('div.f')
                .children('input'), this.get_store().get_fields())
                .changeSelect2(function (e) { return _this.onRowFieldChange(e); });
            this.updateParens();
            this.updateButtons();
            row.find('input.field-select').select2('focus');
            if (popupField) {
                row.find('input.field-select').select2('open');
            }
            return row;
        };
        FilterPanel.prototype.onRowFieldChange = function (e) {
            var row = $(e.target).closest('div.filter-line');
            this.rowFieldChange(row);
            var opSelect = row.children('div.o').find('input.op-select');
            opSelect.select2('focus');
        };
        FilterPanel.prototype.rowFieldChange = function (row) {
            row.removeData('Filtering');
            var select = row.children('div.f').find('input.field-select')
                .getWidget(FilterFieldSelect);
            var fieldName = select.get_value();
            var isEmpty = fieldName == null || fieldName === '';
            this.removeFiltering(row);
            this.populateOperatorList(row);
            this.rowOperatorChange(row);
            this.updateParens();
            this.updateButtons();
        };
        FilterPanel.prototype.removeFiltering = function (row) {
            row.data('Filtering', null);
            row.data('FilteringField', null);
        };
        FilterPanel.prototype.populateOperatorList = function (row) {
            var _this = this;
            row.children('div.o').html('');
            var filtering = this.getFilteringFor(row);
            if (filtering == null)
                return;
            var hidden = row.children('div.o').html('<input/>')
                .children().attr('type', 'hidden').addClass('op-select');
            var operators = filtering.getOperators();
            var opSelect = new FilterOperatorSelect(hidden, operators);
            opSelect.changeSelect2(function (e) { return _this.onRowOperatorChange(e); });
        };
        FilterPanel.prototype.getFieldFor = function (row) {
            if (row.length === 0) {
                return null;
            }
            var select = row.children('div.f').find('input.field-select')
                .getWidget(FilterFieldSelect);
            if (Q.isEmptyOrNull(select.value)) {
                return null;
            }
            return this.get_store().get_fieldByName()[select.get_value()];
        };
        FilterPanel.prototype.getFilteringFor = function (row) {
            var field = this.getFieldFor(row);
            if (field == null)
                return null;
            var filtering = Q.cast(row.data('Filtering'), Serenity.IFiltering);
            if (filtering != null)
                return filtering;
            var filteringType = Serenity.FilteringTypeRegistry.get(Q.coalesce(field.filteringType, 'String'));
            var editorDiv = row.children('div.v');
            filtering = new filteringType();
            Serenity.ReflectionOptionsSetter.set(filtering, field.filteringParams);
            filtering.set_container(editorDiv);
            filtering.set_field(field);
            row.data('Filtering', filtering);
            return filtering;
        };
        FilterPanel.prototype.onRowOperatorChange = function (e) {
            var row = $(e.target).closest('div.filter-line');
            this.rowOperatorChange(row);
            var firstInput = row.children('div.v').find(':input:visible').first();
            try {
                firstInput.focus();
            }
            catch ($t1) {
            }
        };
        FilterPanel.prototype.rowOperatorChange = function (row) {
            if (row.length === 0) {
                return;
            }
            var editorDiv = row.children('div.v');
            editorDiv.html('');
            var filtering = this.getFilteringFor(row);
            if (filtering == null)
                return;
            var operatorSelect = row.children('div.o').find('input.op-select')
                .getWidget(FilterOperatorSelect);
            if (Q.isEmptyOrNull(operatorSelect.get_value()))
                return;
            var ops = filtering.getOperators().filter(function (x) {
                return x.key === operatorSelect.value;
            });
            var op = ((ops.length > 0) ? ops[0] : null);
            if (op == null)
                return;
            filtering.set_operator(op);
            filtering.createEditor();
        };
        FilterPanel.prototype.deleteRowClick = function (e) {
            e.preventDefault();
            var row = $(e.target).closest('div.filter-line');
            row.remove();
            if (this.rowsDiv.children().length === 0) {
                this.search();
            }
            this.updateParens();
            this.updateButtons();
        };
        FilterPanel.prototype.updateButtons = function () {
            this.byId('SearchButton').toggle(this.rowsDiv.children().length >= 1 && this.showSearchButton);
            this.byId('ResetButton').toggle(this.rowsDiv.children().length >= 1);
        };
        FilterPanel.prototype.andOrClick = function (e) {
            e.preventDefault();
            var andor = $(e.target).toggleClass('or');
            andor.text(Q.text('Controls.FilterPanel.' +
                (andor.hasClass('or') ? 'Or' : 'And')));
        };
        FilterPanel.prototype.leftRightParenClick = function (e) {
            e.preventDefault();
            $(e.target).toggleClass('active');
            this.updateParens();
        };
        FilterPanel.prototype.updateParens = function () {
            var rows = this.rowsDiv.children();
            if (rows.length === 0) {
                return;
            }
            rows.removeClass('paren-start');
            rows.removeClass('paren-end');
            rows.children('div.l').css('display', ((rows.length === 1) ? 'none' : 'block'));
            rows.first().children('div.l').children('a.rightparen, a.andor')
                .css('visibility', 'hidden');
            for (var i = 1; i < rows.length; i++) {
                var row = rows.eq(i);
                row.children('div.l').css('display', 'block')
                    .children('a.lefparen, a.andor').css('visibility', 'visible');
            }
            var inParen = false;
            for (var i1 = 0; i1 < rows.length; i1++) {
                var row1 = rows.eq(i1);
                var divParen = row1.children('div.l');
                var lp = divParen.children('a.leftparen');
                var rp = divParen.children('a.rightparen');
                if (rp.hasClass('active') && inParen) {
                    inParen = false;
                    if (i1 > 0) {
                        rows.eq(i1 - 1).addClass('paren-end');
                    }
                }
                if (lp.hasClass('active')) {
                    inParen = true;
                    if (i1 > 0) {
                        row1.addClass('paren-start');
                    }
                }
            }
        };
        return FilterPanel;
    }(Serenity.FilterWidgetBase));
    Serenity.FilterPanel = FilterPanel;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FilterDialog = /** @class */ (function (_super) {
        __extends(FilterDialog, _super);
        function FilterDialog() {
            var _this = _super.call(this) || this;
            _this.filterPanel = new Serenity.FilterPanel(_this.byId('FilterPanel'));
            _this.filterPanel.set_showInitialLine(true);
            _this.filterPanel.set_showSearchButton(false);
            _this.filterPanel.set_updateStoreOnReset(false);
            _this.dialogTitle = Q.text('Controls.FilterPanel.DialogTitle');
            return _this;
        }
        FilterDialog.prototype.get_filterPanel = function () {
            return this.filterPanel;
        };
        FilterDialog.prototype.getTemplate = function () {
            return '<div id="~_FilterPanel"/>';
        };
        FilterDialog.prototype.getDialogButtons = function () {
            var _this = this;
            return [
                {
                    text: Q.text('Dialogs.OkButton'),
                    click: function () {
                        _this.filterPanel.search();
                        if (_this.filterPanel.get_hasErrors()) {
                            Q.notifyError(Q.text('Controls.FilterPanel.FixErrorsMessage'), '', null);
                            return;
                        }
                        _this.dialogClose();
                    }
                },
                {
                    text: Q.text('Dialogs.CancelButton'),
                    click: function () { return _this.dialogClose(); }
                }
            ];
        };
        FilterDialog = __decorate([
            Serenity.Decorators.registerClass('Serenity.FilterDialog')
        ], FilterDialog);
        return FilterDialog;
    }(Serenity.TemplatedDialog));
    Serenity.FilterDialog = FilterDialog;
})(Serenity || (Serenity = {}));
var Serenity;
(function (Serenity) {
    var FilterDisplayBar = /** @class */ (function (_super) {
        __extends(FilterDisplayBar, _super);
        function FilterDisplayBar(div) {
            var _this = _super.call(this, div) || this;
            _this.element.find('.cap').text(Q.text('Controls.FilterPanel.EffectiveFilter'));
            _this.element.find('.edit').text(Q.text('Controls.FilterPanel.EditFilter'));
            _this.element.find('.reset').attr('title', Q.text('Controls.FilterPanel.ResetFilterHint'));
            var openFilterDialog = function (e) {
                e.preventDefault();
                var dialog = new Serenity.FilterDialog();
                dialog.get_filterPanel().set_store(_this.get_store());
                dialog.dialogOpen(null);
            };
            _this.element.find('.edit').click(openFilterDialog);
            _this.element.find('.txt').click(openFilterDialog);
            _this.element.find('.reset').click(function (e1) {
                e1.preventDefault();
                _this.get_store().get_items().length = 0;
                _this.get_store().raiseChanged();
            });
            return _this;
        }
        FilterDisplayBar.prototype.filterStoreChanged = function () {
            _super.prototype.filterStoreChanged.call(this);
            var displayText = Q.trimToNull(this.get_store().get_displayText());
            this.element.find('.current').toggle(displayText != null);
            this.element.find('.reset').toggle(displayText != null);
            if (displayText == null)
                displayText = Q.text('Controls.FilterPanel.EffectiveEmpty');
            this.element.find('.txt').text('[' + displayText + ']');
        };
        FilterDisplayBar.prototype.getTemplate = function () {
            return "<div><a class='reset'></a><a class='edit'></a>" +
                "<div class='current'><span class='cap'></span>" +
                "<a class='txt'></a></div></div>";
        };
        FilterDisplayBar = __decorate([
            Serenity.Decorators.registerClass('Serenity.FilterDisplayBar')
        ], FilterDisplayBar);
        return FilterDisplayBar;
    }(Serenity.FilterWidgetBase));
    Serenity.FilterDisplayBar = FilterDisplayBar;
})(Serenity || (Serenity = {}));
//# sourceMappingURL=serenity-filterpanel.js.map
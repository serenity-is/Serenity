namespace Serenity.TypeScript;

internal static class Diagnostics
{
    private static DiagnosticMessage Diag(int code, DiagnosticCategory category, string key, string message)
    {
        return new()
        {
            Code = code,
            Category = category,
            Key = key,
            Message = message
        };
    }

    // scanner diagnostics
    internal static readonly DiagnosticMessage A_bigint_literal_cannot_use_exponential_notation = Diag(1352, DiagnosticCategory.Error, 
        "A_bigint_literal_cannot_use_exponential_notation_1352", "A bigint literal cannot use exponential notation.");
    internal static readonly DiagnosticMessage A_bigint_literal_must_be_an_integer = Diag(1353, DiagnosticCategory.Error, 
        "A_bigint_literal_must_be_an_integer_1353", "A bigint literal must be an integer.");
    internal static readonly DiagnosticMessage An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive = Diag(1198, DiagnosticCategory.Error, 
        "An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive_1198", "An extended Unicode escape value must be between 0x0 and 0x10FFFF inclusive.");
    internal static readonly DiagnosticMessage An_identifier_or_keyword_cannot_immediately_follow_a_numeric_literal = Diag(1351, DiagnosticCategory.Error, 
        "An_identifier_or_keyword_cannot_immediately_follow_a_numeric_literal_1351", "An identifier or keyword cannot immediately follow a numeric literal.");
    internal static readonly DiagnosticMessage Asterisk_Slash_expected = Diag(1010, DiagnosticCategory.Error, 
        "Asterisk_Slash_expected_1010", "'*/' expected.");
    internal static readonly DiagnosticMessage Binary_digit_expected = Diag(1177, DiagnosticCategory.Error, 
        "Binary_digit_expected_1177", "Binary digit expected.");
    internal static readonly DiagnosticMessage can_only_be_used_at_the_start_of_a_file = Diag(18026, DiagnosticCategory.Error, 
        "can_only_be_used_at_the_start_of_a_file_18026", "'#!' can only be used at the start of a file.");
    internal static readonly DiagnosticMessage Decimals_with_leading_zeros_are_not_allowed = Diag(1489, DiagnosticCategory.Error, 
        "Decimals_with_leading_zeros_are_not_allowed_1489", "Decimals with leading zeros are not allowed.");
    internal static readonly DiagnosticMessage Digit_expected = Diag(1124, DiagnosticCategory.Error, 
        "Digit_expected_1124", "Digit expected.");
    internal static readonly DiagnosticMessage Escape_sequence_0_is_not_allowed = Diag(1488, DiagnosticCategory.Error, 
        "Escape_sequence_0_is_not_allowed_1488", "Escape sequence '{0}' is not allowed.");
    internal static readonly DiagnosticMessage File_appears_to_be_binary = Diag(1490, DiagnosticCategory.Error, 
        "File_appears_to_be_binary_1490", "File appears to be binary.");
    internal static readonly DiagnosticMessage Hexadecimal_digit_expected = Diag(1125, DiagnosticCategory.Error, 
        "Hexadecimal_digit_expected_1125", "Hexadecimal digit expected.");
    internal static readonly DiagnosticMessage Invalid_character = Diag(1127, DiagnosticCategory.Error, 
        "Invalid_character_1127", "Invalid character.");
    internal static readonly DiagnosticMessage Merge_conflict_marker_encountered = Diag(1185, DiagnosticCategory.Error, 
        "Merge_conflict_marker_encountered_1185", "Merge conflict marker encountered.");
    internal static readonly DiagnosticMessage Multiple_consecutive_numeric_separators_are_not_permitted = Diag(6189, DiagnosticCategory.Error, 
        "Multiple_consecutive_numeric_separators_are_not_permitted_6189", "Multiple consecutive numeric separators are not permitted.");
    internal static readonly DiagnosticMessage Numeric_separators_are_not_allowed_here = Diag(6188, DiagnosticCategory.Error, 
        "Numeric_separators_are_not_allowed_here_6188", "Numeric separators are not allowed here.");
    internal static readonly DiagnosticMessage Octal_digit_expected = Diag(1178, DiagnosticCategory.Error, 
        "Octal_digit_expected_1178", "Octal digit expected.");
    internal static readonly DiagnosticMessage Octal_escape_sequences_are_not_allowed_Use_the_syntax_0 = Diag(1487, DiagnosticCategory.Error, 
        "Octal_escape_sequences_are_not_allowed_Use_the_syntax_0_1487", "Octal escape sequences are not allowed. Use the syntax '{0}'.");
    internal static readonly DiagnosticMessage Octal_literals_are_not_allowed_Use_the_syntax_0 = Diag(1121, DiagnosticCategory.Error, 
        "Octal_literals_are_not_allowed_Use_the_syntax_0_1121", "Octal literals are not allowed. Use the syntax '{0}'.");
    internal static readonly DiagnosticMessage Unexpected_end_of_text = Diag(1126, DiagnosticCategory.Error, 
        "Unexpected_end_of_text_1126", "Unexpected end of text.");
    internal static readonly DiagnosticMessage Unexpected_token_Did_you_mean_or_gt = Diag(1382, DiagnosticCategory.Error, 
        "Unexpected_token_Did_you_mean_or_gt_1382", "Unexpected token. Did you mean `{'>'}` or `&gt;`?");
    internal static readonly DiagnosticMessage Unexpected_token_Did_you_mean_or_rbrace = Diag(1381, DiagnosticCategory.Error, 
        "Unexpected_token_Did_you_mean_or_rbrace_1381", "Unexpected token. Did you mean `{'}'}` or `&rbrace;`?");
    internal static readonly DiagnosticMessage Unterminated_regular_expression_literal = Diag(1161, DiagnosticCategory.Error, 
        "Unterminated_regular_expression_literal_1161", "Unterminated regular expression literal.");
    internal static readonly DiagnosticMessage Unterminated_string_literal = Diag(1002, DiagnosticCategory.Error, 
        "Unterminated_string_literal_1002", "Unterminated string literal.");
    internal static readonly DiagnosticMessage Unterminated_template_literal = Diag(1160, DiagnosticCategory.Error, 
        "Unterminated_template_literal_1160", "Unterminated template literal.");
    internal static readonly DiagnosticMessage Unterminated_Unicode_escape_sequence = Diag(1199, DiagnosticCategory.Error, 
        "Unterminated_Unicode_escape_sequence_1199", "Unterminated Unicode escape sequence.");

    // parser diagnostics
    internal static readonly DiagnosticMessage _0_expected = Diag(1005, DiagnosticCategory.Error, 
        "_0_expected_1005", "'{0}' expected.");
    internal static readonly DiagnosticMessage _0_is_not_allowed_as_a_parameter_name = Diag(1390, DiagnosticCategory.Error, 
        "_0_is_not_allowed_as_a_parameter_name_1390", "'{0}' is not allowed as a parameter name.");
    internal static readonly DiagnosticMessage _0_is_not_allowed_as_a_variable_declaration_name = Diag(1389, DiagnosticCategory.Error, 
        "_0_is_not_allowed_as_a_variable_declaration_name_1389", "'{0}' is not allowed as a variable declaration name.");
    internal static readonly DiagnosticMessage A_type_assertion_expression_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_expression_Consider_enclosing_the_expression_in_parentheses = Diag(17007, DiagnosticCategory.Error, 
        "A_type_assertion_expression_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_expression_Con_17007", "A type assertion expression is not allowed in the left-hand side of an exponentiation expression. Consider enclosing the expression in parentheses.");
    internal static readonly DiagnosticMessage A_type_predicate_is_only_allowed_in_return_type_position_for_functions_and_methods = Diag(1228, DiagnosticCategory.Error, 
        "A_type_predicate_is_only_allowed_in_return_type_position_for_functions_and_methods_1228", "A type predicate is only allowed in return type position for functions and methods.");
    internal static readonly DiagnosticMessage An_AMD_module_cannot_have_multiple_name_assignments = Diag(2458, DiagnosticCategory.Error, 
        "An_AMD_module_cannot_have_multiple_name_assignments_2458", "An AMD module cannot have multiple name assignments.");
    internal static readonly DiagnosticMessage An_element_access_expression_should_take_an_argument = Diag(1011, DiagnosticCategory.Error, 
        "An_element_access_expression_should_take_an_argument_1011", "An element access expression should take an argument.");
    internal static readonly DiagnosticMessage An_enum_member_name_must_be_followed_by_a_or = Diag(1357, DiagnosticCategory.Error, 
        "An_enum_member_name_must_be_followed_by_a_or_1357", "An enum member name must be followed by a ',', '=', or '}'.");
    internal static readonly DiagnosticMessage An_instantiation_expression_cannot_be_followed_by_a_property_access = Diag(1477, DiagnosticCategory.Error, 
        "An_instantiation_expression_cannot_be_followed_by_a_property_access_1477", "An instantiation expression cannot be followed by a property access.");
    internal static readonly DiagnosticMessage An_optional_chain_cannot_contain_private_identifiers = Diag(18030, DiagnosticCategory.Error, 
        "An_optional_chain_cannot_contain_private_identifiers_18030", "An optional chain cannot contain private identifiers.");
    internal static readonly DiagnosticMessage An_unary_expression_with_the_0_operator_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_expression_Consider_enclosing_the_expression_in_parentheses = Diag(17006, DiagnosticCategory.Error, 
        "An_unary_expression_with_the_0_operator_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_ex_17006", "An unary expression with the '{0}' operator is not allowed in the left-hand side of an exponentiation expression. Consider enclosing the expression in parentheses.");
    internal static readonly DiagnosticMessage Argument_expression_expected = Diag(1135, DiagnosticCategory.Error, 
        "Argument_expression_expected_1135", "Argument expression expected.");
    internal static readonly DiagnosticMessage Array_element_destructuring_pattern_expected = Diag(1181, DiagnosticCategory.Error, 
        "Array_element_destructuring_pattern_expected_1181", "Array element destructuring pattern expected.");
    internal static readonly DiagnosticMessage Cannot_start_a_function_call_in_a_type_annotation = Diag(1441, DiagnosticCategory.Error, 
        "Cannot_start_a_function_call_in_a_type_annotation_1441", "Cannot start a function call in a type annotation.");
    internal static readonly DiagnosticMessage case_or_default_expected = Diag(1130, DiagnosticCategory.Error, 
        "case_or_default_expected_1130", "'case' or 'default' expected.");
    internal static readonly DiagnosticMessage catch_or_finally_expected = Diag(1472, DiagnosticCategory.Error, 
        "catch_or_finally_expected_1472", "'catch' or 'finally' expected.");
    internal static readonly DiagnosticMessage Constructor_type_notation_must_be_parenthesized_when_used_in_a_union_type = Diag(1386, DiagnosticCategory.Error, 
        "Constructor_type_notation_must_be_parenthesized_when_used_in_a_union_type_1386", "Constructor type notation must be parenthesized when used in a union type.");
    internal static readonly DiagnosticMessage Constructor_type_notation_must_be_parenthesized_when_used_in_an_intersection_type = Diag(1388, DiagnosticCategory.Error, 
        "Constructor_type_notation_must_be_parenthesized_when_used_in_an_intersection_type_1388", "Constructor type notation must be parenthesized when used in an intersection type.");
    internal static readonly DiagnosticMessage Declaration_expected = Diag(1146, DiagnosticCategory.Error, 
        "Declaration_expected_1146", "Declaration expected.");
    internal static readonly DiagnosticMessage Declaration_or_statement_expected = Diag(1128, DiagnosticCategory.Error, 
        "Declaration_or_statement_expected_1128", "Declaration or statement expected.");
    internal static readonly DiagnosticMessage Declaration_or_statement_expected_This_follows_a_block_of_statements_so_if_you_intended_to_write_a_destructuring_assignment_you_might_need_to_wrap_the_whole_assignment_in_parentheses = Diag(2809, DiagnosticCategory.Error, 
        "Declaration_or_statement_expected_This_follows_a_block_of_statements_so_if_you_intended_to_write_a_d_2809", "Declaration or statement expected. This '=' follows a block of statements, so if you intended to write a destructuring assignment, you might need to wrap the whole assignment in parentheses.");
    internal static readonly DiagnosticMessage Decorators_must_precede_the_name_and_all_keywords_of_property_declarations = Diag(1436, DiagnosticCategory.Error, 
        "Decorators_must_precede_the_name_and_all_keywords_of_property_declarations_1436", "Decorators must precede the name and all keywords of property declarations.");
    internal static readonly DiagnosticMessage Enum_member_expected = Diag(1132, DiagnosticCategory.Error, 
        "Enum_member_expected_1132", "Enum member expected.");
    internal static readonly DiagnosticMessage Expected_corresponding_closing_tag_for_JSX_fragment = Diag(17015, DiagnosticCategory.Error, 
        "Expected_corresponding_closing_tag_for_JSX_fragment_17015", "Expected corresponding closing tag for JSX fragment.");
    internal static readonly DiagnosticMessage Expected_corresponding_JSX_closing_tag_for_0 = Diag(17002, DiagnosticCategory.Error, 
        "Expected_corresponding_JSX_closing_tag_for_0_17002", "Expected corresponding JSX closing tag for '{0}'.");
    internal static readonly DiagnosticMessage Expected_for_property_initializer = Diag(1442, DiagnosticCategory.Error, 
        "Expected_for_property_initializer_1442", "Expected '=' for property initializer.");
    internal static readonly DiagnosticMessage Expression_expected = Diag(1109, DiagnosticCategory.Error, 
        "Expression_expected_1109", "Expression expected.");
    internal static readonly DiagnosticMessage Expression_or_comma_expected = Diag(1137, DiagnosticCategory.Error, 
        "Expression_or_comma_expected_1137", "Expression or comma expected.");
    internal static readonly DiagnosticMessage Function_type_notation_must_be_parenthesized_when_used_in_a_union_type = Diag(1385, DiagnosticCategory.Error, 
        "Function_type_notation_must_be_parenthesized_when_used_in_a_union_type_1385", "Function type notation must be parenthesized when used in a union type.");
    internal static readonly DiagnosticMessage Function_type_notation_must_be_parenthesized_when_used_in_an_intersection_type = Diag(1387, DiagnosticCategory.Error, 
        "Function_type_notation_must_be_parenthesized_when_used_in_an_intersection_type_1387", "Function type notation must be parenthesized when used in an intersection type.");
    internal static readonly DiagnosticMessage Identifier_expected = Diag(1003, DiagnosticCategory.Error, 
        "Identifier_expected_1003", "Identifier expected.");
    internal static readonly DiagnosticMessage Identifier_expected_0_is_a_reserved_word_that_cannot_be_used_here = Diag(1359, DiagnosticCategory.Error, 
        "Identifier_expected_0_is_a_reserved_word_that_cannot_be_used_here_1359", "Identifier expected. '{0}' is a reserved word that cannot be used here.");
    internal static readonly DiagnosticMessage Identifier_or_string_literal_expected = Diag(1478, DiagnosticCategory.Error, 
        "Identifier_or_string_literal_expected_1478", "Identifier or string literal expected.");
    internal static readonly DiagnosticMessage Interface_must_be_given_a_name = Diag(1438, DiagnosticCategory.Error, 
        "Interface_must_be_given_a_name_1438", "Interface must be given a name.");
    internal static readonly DiagnosticMessage Interface_name_cannot_be_0 = Diag(2427, DiagnosticCategory.Error, 
        "Interface_name_cannot_be_0_2427", "Interface name cannot be '{0}'.");
    internal static readonly DiagnosticMessage Invalid_optional_chain_from_new_expression_Did_you_mean_to_call_0 = Diag(1209, DiagnosticCategory.Error, 
        "Invalid_optional_chain_from_new_expression_Did_you_mean_to_call_0_1209", "Invalid optional chain from new expression. Did you mean to call '{0}()'?");
    internal static readonly DiagnosticMessage Invalid_reference_directive_syntax = Diag(1084, DiagnosticCategory.Error, 
        "Invalid_reference_directive_syntax_1084", "Invalid 'reference' directive syntax.");
    internal static readonly DiagnosticMessage JSX_element_0_has_no_corresponding_closing_tag = Diag(17008, DiagnosticCategory.Error, 
        "JSX_element_0_has_no_corresponding_closing_tag_17008", "JSX element '{0}' has no corresponding closing tag.");
    internal static readonly DiagnosticMessage JSX_expressions_must_have_one_parent_element = Diag(2657, DiagnosticCategory.Error, 
        "JSX_expressions_must_have_one_parent_element_2657", "JSX expressions must have one parent element.");
    internal static readonly DiagnosticMessage JSX_fragment_has_no_corresponding_closing_tag = Diag(17014, DiagnosticCategory.Error, 
        "JSX_fragment_has_no_corresponding_closing_tag_17014", "JSX fragment has no corresponding closing tag.");
    internal static readonly DiagnosticMessage Keywords_cannot_contain_escape_characters = Diag(1260, DiagnosticCategory.Error, 
        "Keywords_cannot_contain_escape_characters_1260", "Keywords cannot contain escape characters.");
    internal static readonly DiagnosticMessage Line_break_not_permitted_here = Diag(1142, DiagnosticCategory.Error, 
        "Line_break_not_permitted_here_1142", "Line break not permitted here.");
    internal static readonly DiagnosticMessage Module_declaration_names_may_only_use_or_quoted_strings = Diag(1443, DiagnosticCategory.Error, 
        "Module_declaration_names_may_only_use_or_quoted_strings_1443", "Module declaration names may only use ' or \" quoted strings.");
    internal static readonly DiagnosticMessage Namespace_must_be_given_a_name = Diag(1437, DiagnosticCategory.Error, 
        "Namespace_must_be_given_a_name_1437", "Namespace must be given a name.");
    internal static readonly DiagnosticMessage Namespace_name_cannot_be_0 = Diag(2819, DiagnosticCategory.Error, 
        "Namespace_name_cannot_be_0_2819", "Namespace name cannot be '{0}'.");
    internal static readonly DiagnosticMessage Neither_decorators_nor_modifiers_may_be_applied_to_this_parameters = Diag(1433, DiagnosticCategory.Error, 
        "Neither_decorators_nor_modifiers_may_be_applied_to_this_parameters_1433", "Neither decorators nor modifiers may be applied to 'this' parameters.");
    internal static readonly DiagnosticMessage or_expected = Diag(1144, DiagnosticCategory.Error, 
        "or_expected_1144", "'{' or ';' expected.");
    internal static readonly DiagnosticMessage or_JSX_element_expected = Diag(1145, DiagnosticCategory.Error, 
        "or_JSX_element_expected_1145", "'{' or JSX element expected.");
    internal static readonly DiagnosticMessage Parameter_declaration_expected = Diag(1138, DiagnosticCategory.Error, 
        "Parameter_declaration_expected_1138", "Parameter declaration expected.");
    internal static readonly DiagnosticMessage Private_identifiers_are_not_allowed_in_variable_declarations = Diag(18029, DiagnosticCategory.Error, 
        "Private_identifiers_are_not_allowed_in_variable_declarations_18029", "Private identifiers are not allowed in variable declarations.");
    internal static readonly DiagnosticMessage Private_identifiers_are_not_allowed_outside_class_bodies = Diag(18016, DiagnosticCategory.Error, 
        "Private_identifiers_are_not_allowed_outside_class_bodies_18016", "Private identifiers are not allowed outside class bodies.");
    internal static readonly DiagnosticMessage Private_identifiers_cannot_be_used_as_parameters = Diag(18009, DiagnosticCategory.Error, 
        "Private_identifiers_cannot_be_used_as_parameters_18009", "Private identifiers cannot be used as parameters.");
    internal static readonly DiagnosticMessage Property_assignment_expected = Diag(1136, DiagnosticCategory.Error, 
        "Property_assignment_expected_1136", "Property assignment expected.");
    internal static readonly DiagnosticMessage Property_destructuring_pattern_expected = Diag(1180, DiagnosticCategory.Error, 
        "Property_destructuring_pattern_expected_1180", "Property destructuring pattern expected.");
    internal static readonly DiagnosticMessage Property_or_signature_expected = Diag(1131, DiagnosticCategory.Error, 
        "Property_or_signature_expected_1131", "Property or signature expected.");
    internal static readonly DiagnosticMessage resolution_mode_should_be_either_require_or_import = Diag(1453, DiagnosticCategory.Error, 
        "resolution_mode_should_be_either_require_or_import_1453", "`resolution-mode` should be either `require` or `import`.");
    internal static readonly DiagnosticMessage Statement_expected = Diag(1129, DiagnosticCategory.Error, 
        "Statement_expected_1129", "Statement expected.");
    internal static readonly DiagnosticMessage super_may_not_use_type_arguments = Diag(2754, DiagnosticCategory.Error, 
        "super_may_not_use_type_arguments_2754", "'super' may not use type arguments.");
    internal static readonly DiagnosticMessage super_must_be_followed_by_an_argument_list_or_member_access = Diag(1034, DiagnosticCategory.Error, 
        "super_must_be_followed_by_an_argument_list_or_member_access_1034", "'super' must be followed by an argument list or member access.");
    internal static readonly DiagnosticMessage The_parser_expected_to_find_a_1_to_match_the_0_token_here = Diag(1007, DiagnosticCategory.Error, 
        "The_parser_expected_to_find_a_1_to_match_the_0_token_here_1007", "The parser expected to find a '{1}' to match the '{0}' token here.");
    internal static readonly DiagnosticMessage Type_alias_must_be_given_a_name = Diag(1439, DiagnosticCategory.Error, 
        "Type_alias_must_be_given_a_name_1439", "Type alias must be given a name.");
    internal static readonly DiagnosticMessage Type_alias_name_cannot_be_0 = Diag(2457, DiagnosticCategory.Error, 
        "Type_alias_name_cannot_be_0_2457", "Type alias name cannot be '{0}'.");
    internal static readonly DiagnosticMessage Type_argument_expected = Diag(1140, DiagnosticCategory.Error, 
        "Type_argument_expected_1140", "Type argument expected.");
    internal static readonly DiagnosticMessage Type_expected = Diag(1110, DiagnosticCategory.Error, 
        "Type_expected_1110", "Type expected.");
    internal static readonly DiagnosticMessage Type_parameter_declaration_expected = Diag(1139, DiagnosticCategory.Error, 
        "Type_parameter_declaration_expected_1139", "Type parameter declaration expected.");
    internal static readonly DiagnosticMessage Unexpected_keyword_or_identifier = Diag(1434, DiagnosticCategory.Error, 
        "Unexpected_keyword_or_identifier_1434", "Unexpected keyword or identifier.");
    internal static readonly DiagnosticMessage Unexpected_token = Diag(1012, DiagnosticCategory.Error, 
        "Unexpected_token_1012", "Unexpected token.");
    internal static readonly DiagnosticMessage Unexpected_token_A_constructor_method_accessor_or_property_was_expected = Diag(1068, DiagnosticCategory.Error, 
        "Unexpected_token_A_constructor_method_accessor_or_property_was_expected_1068", "Unexpected token. A constructor, method, accessor, or property was expected.");
    internal static readonly DiagnosticMessage Unexpected_token_A_type_parameter_name_was_expected_without_curly_braces = Diag(1069, DiagnosticCategory.Error, 
        "Unexpected_token_A_type_parameter_name_was_expected_without_curly_braces_1069", "Unexpected token. A type parameter name was expected without curly braces.");
    internal static readonly DiagnosticMessage Unexpected_token_expected = Diag(1179, DiagnosticCategory.Error, 
        "Unexpected_token_expected_1179", "Unexpected token. '{' expected.");
    internal static readonly DiagnosticMessage Unicode_escape_sequence_cannot_appear_here = Diag(17021, DiagnosticCategory.Error, 
        "Unicode_escape_sequence_cannot_appear_here_17021", "Unicode escape sequence cannot appear here.");
    internal static readonly DiagnosticMessage Unknown_keyword_or_identifier_Did_you_mean_0 = Diag(1435, DiagnosticCategory.Error, 
        "Unknown_keyword_or_identifier_Did_you_mean_0_1435", "Unknown keyword or identifier. Did you mean '{0}'?");
    internal static readonly DiagnosticMessage Variable_declaration_expected = Diag(1134, DiagnosticCategory.Error, 
        "Variable_declaration_expected_1134", "Variable declaration expected.");
    internal static readonly DiagnosticMessage Variable_declaration_not_allowed_at_this_location = Diag(1440, DiagnosticCategory.Error, 
        "Variable_declaration_not_allowed_at_this_location_1440", "Variable declaration not allowed at this location.");
}

grammar formula_grammar;

options
{
    language=CSharp3;
}

condition
	:
		formula EOF
	;

formula
	:
		disjunction
	;

disjunction:
	conjunction (OR conjunction)*
	;

conjunction:
	negation (AND negation)*
	;

negation:
	NOT? (predicate | LPAREN formula RPAREN)
	;

predicate:
	functor=identifier LPAREN predicateTuple RPAREN
	;

predicateTuple
	:
		term (',' term)*
	;

term
	:
		literal_term
	|	variable_term
	;

literal_term:	name=identifier;

variable_term
	:
		name=identifier ASSIGNMENT_OP type=varType LPAREN value=varValue RPAREN
	;

varValue
	:
		INT
	|	FLOAT
	|	String
	;

identifier returns [String expr]
	:
		ID {$expr = $ID.text;}
	;

ID  :	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*
    ;

INT :	'0'..'9'+
    ;

FLOAT
    :   ('0'..'9')+ '.' ('0'..'9')* EXPONENT?
    |   '.' ('0'..'9')+ EXPONENT?
    |   ('0'..'9')+ EXPONENT
    ;

WS  :   ( ' '
        | '\t'
        | '\r'
        | '\n'
        )//{$channel=HIDDEN;}
    ;

String : ('a'..'z'|'A'..'Z'|'0'..'9'|'_')+ | ('"' (~'"')* '"');



CHAR:  '\'' ( ESC_SEQ | ~('\''|'\\') ) '\''
    ;

fragment
EXPONENT : ('e'|'E') ('+'|'-')? ('0'..'9')+ ;

fragment
HEX_DIGIT : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment
ESC_SEQ
    :   '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
    |   UNICODE_ESC
    |   OCTAL_ESC
    ;

fragment
OCTAL_ESC
    :   '\\' ('0'..'3') ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7') ('0'..'7')
    |   '\\' ('0'..'7')
    ;

fragment
UNICODE_ESC
    :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
    ;

LPAREN
    : '('
    ;

RPAREN
    : ')'
    ;

ASSIGNMENT_OP
	:
	'<-'
	;

AND
    : '&'
    ;

OR
    : '|'
    ;

NOT
    : '!'
    ;

/** primitive types */
varType
  : simple_type
  | nullable_type
  ;

simple_type returns [String expr]
	: numeric_type			{$expr = $numeric_type.expr; }
	| 'bool'			{$expr = "System.Boolean";}
	;
numeric_type returns [String expr]
	: integral_type 		{$expr = $integral_type.expr; }
	| floating_point_type	 	{$expr = $floating_point_type.expr; }
	| 'decimal'			{$expr = "System.Decimal";}
	;

integral_type returns [String expr]
	: 'sbyte'	{$expr = "System.SByte";}
	| 'byte'	{$expr = "System.Byte";}
	| 'short'	{$expr = "System.Int16";}
	| 'ushort'	{$expr = "System.UInt16";}
	| 'int'		{$expr = "System.Int32";}
	| 'uint' 	{$expr = "System.UInt32";}
	| 'long'	{$expr = "System.Int64";}
	| 'ulong'	{$expr = "System.UInt64";}
	| 'char' 	{$expr = "System.Char";}
	;
floating_point_type returns [String expr]
	: 'float'	 {$expr = "System.Single";}
	| 'double'	 {$expr = "System.Double";}
	;

nullable_type returns  [String expr]
	:
		string_type {$expr = $string_type.expr; }
	;
string_type returns [String expr]
	:
		'string' {$expr = "System.String";}
	;

grammar formula_grammar;


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
//		name=identifier ASSIGNMENT_OP type=varType LPAREN value=varValue RPAREN
		the_int_type
	|	the_boolean_type
	| 	the_string_type
	|	the_float_type
	| 	the_char_type
	;
	
the_int_type returns [int value]:
	INT	 {$value = Int32.Parse($INT.text);}
	
	;

the_boolean_type returns [bool value]:
		'false' {$value = false;}
	|	'true' {$value = true;}
	;

the_string_type returns [string value]:
	String {$value = $String.text.Replace("\"","");}
	;

the_char_type returns [char value]:
	CHAR {$value = $CHAR.text.Replace("'","")[0];}
	;
	
the_float_type returns [float value]:
	FLOAT {$value = float.Parse($FLOAT.text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);}
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
		) -> channel(HIDDEN)
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

AND
	: '&'
	;

OR
	: '|'
	;

NOT
	: '!'
	;

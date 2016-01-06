/**
__  __                                     _   
|  \/  |                                   | |  
| \  / | _   _  ___   __ _     _ __    ___ | |_ 
| |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
| |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
|_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|

*/
namespace FormulaLibrary
{
    static class TypesMapping
    {
        /// <summary>
        /// Given a short type name, return its full qualified name
        /// </summary>
        public static string getCompleteTypeName(string shortName)
        {
            if (shortName.Equals("bool")) 		return "System.Boolean";
            if (shortName.Equals("decimal")) 	return "System.Decimal";
            if (shortName.Equals("sbyte")) 		return "System.SByte";
            if (shortName.Equals("byte")) 		return "System.Byte";
            if (shortName.Equals("short")) 		return "System.Int16";
            if (shortName.Equals("ushort")) 	return "System.UInt16";
            if (shortName.Equals("int")) 		return "System.Int32";
            if (shortName.Equals("uint")) 		return "System.UInt32";
            if (shortName.Equals("long")) 		return "System.Int64";
            if (shortName.Equals("ulong")) 		return "System.UInt64";
            if (shortName.Equals("char")) 		return "System.Char";
            if (shortName.Equals("float")) 		return "System.Single";
            if (shortName.Equals("double")) 	return "System.Double";
            if (shortName.Equals("string")) 	return "System.String";
            	return "";
        }

        /// <summary>
        /// Given a full qualified type name, return its short name
        /// </summary>
        /// <param name="completeName"></param>
        /// <returns></returns>
        public static string getShortTypeName(string completeName)
        {
            if (completeName.Equals("System.Boolean")) 		return "bool";
            if (completeName.Equals("System.Decimal")) 		return "decimal";
            if (completeName.Equals("System.SByte")) 		return "sbyte";
            if (completeName.Equals("System.Byte")) 		return "byte";
            if (completeName.Equals("System.Int16")) 		return "short";
            if (completeName.Equals("System.UInt16")) 		return "ushort";
            if (completeName.Equals("System.Int32")) 		return "int";
            if (completeName.Equals("System.UInt32")) 		return "uint";
            if (completeName.Equals("System.Int64")) 		return "long";
            if (completeName.Equals("System.UInt64")) 		return "ulong";
            if (completeName.Equals("System.Char")) 		return "char";
            if (completeName.Equals("System.Single")) 		return "float";
            if (completeName.Equals("System.Double")) 		return "double";
            if (completeName.Equals("System.String")) 		return "string";
            return "";
        }
    }
}

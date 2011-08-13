using System;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Clavis
{
    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    public class SQLiteRegex : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Regex.IsMatch(
                Convert.ToString(args[1], CultureInfo.InvariantCulture),
                Convert.ToString(args[0], CultureInfo.InvariantCulture));
        }
    }
}
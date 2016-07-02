using System.Collections.Generic;
using System.Linq;

public static class StringExtensions
{
	public static string Join(this IEnumerable<string> stringList, string seperator)
	{
		return string.Join(seperator, stringList.ToArray());
	}
}
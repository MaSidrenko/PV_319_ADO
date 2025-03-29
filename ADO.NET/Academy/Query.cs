using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
	internal class Query
	{
		public string Columns { get; }
		public string Tables { get; }
		public string Condition { get; }
		public string Group_by { get; }
		public Query(string columns, string tables, string condtion="", string group_by="")
		{
			Columns = columns;
			Tables = tables;
			Condition = condtion;
			Group_by = group_by;
		}
	}
}

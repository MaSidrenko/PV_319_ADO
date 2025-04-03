using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
	internal class Query
	{
		public string Columns { get; set; }
		public string Tables { get; set; }
		public string Condition { get; set; }
		public string Group_by { get; set; }
		public Query(string columns, string tables, string condtion="", string group_by="")
		{
			Columns = columns;
			Tables = tables;
			Condition = condtion;
			Group_by = group_by;
		}
		public Query(Query other)
		{
			this.Columns = other.Columns;
			this.Tables = other.Tables;
			this.Condition = other.Condition;
			this.Group_by = other.Group_by;
		}
	}
}

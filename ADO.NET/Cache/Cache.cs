using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace Cache
{
	public class Cache
	{
		readonly string CONN_STR = "";
		public DataSet Set { get; set; }
		SqlConnection conn = null;
		List<string> tables;
		public Cache()
		{
			CONN_STR = ConfigurationManager.ConnectionStrings["PV_319_IMPORT"].ConnectionString;
			conn = new SqlConnection(CONN_STR);

			tables = new List<string>();

			Set = new DataSet(nameof(Set));
		}
		public void AddTable(string table_name, string columns)
		{
			string[] separated_columns = columns.Split(',');
			Set.Tables.Add(table_name);
			for (int i = 0; i < separated_columns.Length; i++)
			{
				Set.Tables[table_name]?.Columns.Add(separated_columns[i]);
			}

			Set.Tables[table_name].PrimaryKey = new DataColumn[]
				{ Set.Tables[table_name].Columns[separated_columns[0]] };
			tables.Add($"{table_name},{columns}");
		}
		public void AddRelation(string name, string child, string parent)
		{
			Set.Relations.Add
				(
				name,
				Set.Tables[parent.Split(',')[0]].Columns[parent.Split(',')[1]],
				Set.Tables[child.Split(',')[0]].Columns[child.Split(',')[1]]
				);
		}
		public void Load()
		{
			try
			{
				string[] tables = this.tables.ToArray();
				for (int i = 0; i < tables.Length; i++)
				{
					string columns = "";
					DataColumnCollection column_collection = Set.Tables[tables[i].Split(',')[0]].Columns;
					foreach (DataColumn column in column_collection)
					{
						columns += $"[{column.ColumnName}],";
					}
					columns = columns.Remove(columns.LastIndexOf(','));
					Console.WriteLine(columns);
					string cmd = $"SELECT {columns} FROM {tables[i].Split(',')[0]}";
					SqlDataAdapter adapter = new SqlDataAdapter(cmd, conn);
					adapter.Fill(Set.Tables[tables[i].Split(',')[0]]);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		public void Print(string table_name)
		{
			Console.WriteLine("\n|==============================================================================|\n");
			string relation_name = "No relation";
			string parent_table_name = "";
			string parent_column_name = "";
			Console.WriteLine(table_name);
			int parent_index = -1;
			if (hasParents(table_name))
			{
				relation_name = Set.Tables[table_name].ParentRelations[0].RelationName;
				parent_table_name = Set.Tables[table_name].ParentRelations[0].ParentTable.TableName;
				parent_column_name = parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1) + "_name";
				parent_index = Set.Tables[table_name].Columns.IndexOf(parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1));
			}
			foreach (DataRow row in Set.Tables[table_name].Rows)
			{
				for (int i = 0; i < row.ItemArray.Length; i++)
				{
					if (i == parent_index)
						Console.Write(row.GetParentRow(relation_name)[parent_column_name].ToString().PadRight(35));
					else
						Console.Write((row[i].ToString()).PadRight(35));
				}
				Console.WriteLine();
			}
			Console.WriteLine("\n|==============================================================================|\n");
		}
		bool hasParents(string table_name)
		{
			return Set.Tables[table_name]?.ParentRelations.Count > 0;
		}
		public void CleareCahce()
		{
			Set.Clear();
		}
		public void Check()
		{
			AddTable("Directions", "direction_id,direction_name");
			AddTable("Groups", "group_id,group_name,direction");
			AddTable("Students", "stud_id,last_name,first_name,middle_name,birth_date,group");

			AddRelation("GroupsDirections", "Groups,direction", "Directions,direction_id");
			AddRelation("StudentsGroups", "Students,group", "Groups,group_id");

			Load();

			Print("Directions");
			Print("Groups");
			Print("Students");
		}
	}
}

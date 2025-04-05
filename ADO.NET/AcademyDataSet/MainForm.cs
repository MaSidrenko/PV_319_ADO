using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Runtime.InteropServices;

namespace AcademyDataSet
{
	public partial class MainForm : Form
	{
		readonly string CONN_STR = "";
		DataSet GroupsRelatedData = null;
		SqlConnection conn = null;
		List<string> tables;
		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			CONN_STR = ConfigurationManager.ConnectionStrings["PV_319_IMPORT"].ConnectionString;
			conn = new SqlConnection(CONN_STR);

			tables = new List<string>();

			GroupsRelatedData = new DataSet(nameof(GroupsRelatedData));
			//LoadGroupsRelatedData();
			Check();
		}
		public void AddTable(string table_name, string columns)
		{
			string[] separated_columns = columns.Split(',');
			GroupsRelatedData.Tables.Add(table_name);
			for(int i = 0; i < separated_columns.Length; i++)
			{
				GroupsRelatedData.Tables[table_name].Columns.Add(separated_columns[i]);
			}
		
			GroupsRelatedData.Tables[table_name].PrimaryKey = new DataColumn[]
				{ GroupsRelatedData.Tables[table_name].Columns[separated_columns[0]] };
			tables.Add($"{table_name},{columns}");

		}
		public void AddRelation(string name,string child, string parent)
		{
			GroupsRelatedData.Relations.Add
				(
				name,
				GroupsRelatedData.Tables[parent.Split(',')[0]].Columns[parent.Split(',')[1]],
				GroupsRelatedData.Tables[child.Split(',')[0]].Columns[child.Split(',')[1]]
				);
		}
		public void Load()
		{
			string[] tables = this.tables.ToArray();
			for (int i = 0; i < tables.Length; i++)
			{
				string cmd = $"SELECT * FROM {tables[i].Split(',')[0]}";
				SqlDataAdapter adapter = new SqlDataAdapter(cmd, conn);
				adapter.Fill(GroupsRelatedData.Tables[tables[i].Split(',')[0]]);
			}
		}
		public void Print(string table_name)
		{
			Console.WriteLine("\n|==============================================================================|\n");
			Console.WriteLine(hasParents(table_name));
			foreach (DataRow row in GroupsRelatedData.Tables[table_name].Rows)
			{
				for (int i = 0; i < row.ItemArray.Length; i++)
				{
					Console.Write(row[i].ToString() + "\t");
				}
				Console.WriteLine();
			}
			Console.WriteLine("\n|==============================================================================|\n");
		}
		bool hasParents(string table_name)
		{
			bool yes = false;
			for(int i = 0; i < GroupsRelatedData.Relations.Count;i++)
			{
				if (GroupsRelatedData.Relations[i].ChildTable.TableName == table_name) return true;
			}
			return yes;
		}
		void LoadGroupsRelatedData()
		{
			Console.WriteLine(nameof(GroupsRelatedData));
			
			const string dsTable_Directions = "Directions";
			const string dst_col_direction_id = "direction_id";
			const string dst_col_direction_name = "direction_name"
				;
			GroupsRelatedData.Tables.Add(dsTable_Directions);
			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_id, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_name, typeof(String));


			GroupsRelatedData.Tables[dsTable_Directions].PrimaryKey = new DataColumn[]
				{ GroupsRelatedData.Tables[dsTable_Directions].Columns[dst_col_direction_id] };

			const string dsTable_Groups = "Groups";
			const string dst_col_group_id = "group_id";
			const string dst_col_group_name = "group_name";
			const string dst_col_direction = "direction";
			GroupsRelatedData.Tables.Add(dsTable_Groups);
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_col_group_id, typeof(int));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_col_group_name, typeof(string));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_col_direction, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Groups].PrimaryKey = new DataColumn[]
			{
				GroupsRelatedData.Tables[dsTable_Groups].Columns[dst_col_group_id]
			};
			string dsRealtion_Groups_Directions = "GroupsDirections";
			GroupsRelatedData.Relations.Add
				(
					dsRealtion_Groups_Directions,
					GroupsRelatedData.Tables["Directions"].Columns["direction_id"],
					GroupsRelatedData.Tables["Groups"].Columns["direction"]
				);

			string directions_cmd = "SELECT * FROM Directions";
			string groups_cmd = "SELECT * FROM Groups";
			SqlDataAdapter directionsAdapter = new SqlDataAdapter(directions_cmd, conn);
			SqlDataAdapter groupsAdapter = new SqlDataAdapter(groups_cmd, conn);

			conn.Open();
			directionsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Directions]);
			groupsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Groups]);
			conn.Close();


			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Directions].Rows)
			{
				Console.WriteLine($"{row[dst_col_direction_id]}\t{row[dst_col_direction_name]}");
			}
			Console.WriteLine("\n------------------------------------------------------------\n");
			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Groups].Rows)
			{
				Console.WriteLine($"{row[dst_col_group_id]}\t{row[dst_col_group_name]}\t{row.GetParentRow(dsRealtion_Groups_Directions)[dst_col_direction_name]}");
			}
		}
		public void Check()
		{
			AddTable("Directions","direction_id,direction_name");
			AddTable("Groups", "group_id,group_name,direction");

			AddRelation("GroupsDirections", "Groups,direction","Directions,direction_id");
			Load();
			Print("Directions");
			Print("Groups");
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}

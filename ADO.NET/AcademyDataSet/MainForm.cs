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
		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			CONN_STR = ConfigurationManager.ConnectionStrings["PV_319_IMPORT"].ConnectionString;
			conn = new SqlConnection(CONN_STR);
			LoadGroupsRelatedData();
		}
		
		void LoadGroupsRelatedData()
		{
			Console.WriteLine(nameof(GroupsRelatedData));
			GroupsRelatedData = new DataSet(nameof(GroupsRelatedData));

			const string dsTabe_Directions = "Directions";
			const string dst_col_direction_id = "direction_id";
			const string dst_col_direction_name = "direction_name"
				;
			GroupsRelatedData.Tables.Add(dsTabe_Directions);
			GroupsRelatedData.Tables[dsTabe_Directions].Columns.Add(dst_col_direction_id, typeof(byte));
			GroupsRelatedData.Tables[dsTabe_Directions].Columns.Add(dst_col_direction_name, typeof(string));
			GroupsRelatedData.Tables[dsTabe_Directions].PrimaryKey = new DataColumn[]
				{ GroupsRelatedData.Tables[dsTabe_Directions].Columns[dst_col_direction_id] };

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
			directionsAdapter.Fill(GroupsRelatedData.Tables[dsTabe_Directions]);
			groupsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Groups]);
			conn.Close();


			foreach (DataRow row in GroupsRelatedData.Tables[dsTabe_Directions].Rows)
			{
				Console.WriteLine($"{row[dst_col_direction_id]}\t{row[dst_col_direction_name]}");
			}
			Console.WriteLine("\n------------------------------------------------------------\n");
			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Groups].Rows)
			{
				Console.WriteLine($"{row[dst_col_group_id]}\t{row[dst_col_group_name]}\t{row.GetParentRow(dsRealtion_Groups_Directions)[dst_col_direction_name]}");
			}
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}

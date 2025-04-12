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
using System.Diagnostics.Eventing.Reader;

using Cache;

namespace AcademyDataSet
{
	public partial class MainForm : Form
	{
		Cache.Cache GroupsRelatedData;
		DateTime timer;
		bool isNeedRefresh = false;
		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			GroupsRelatedData = new Cache.Cache();
			GroupsRelatedData.AddTable("Directions", "direction_id,direction_name");
			GroupsRelatedData.AddTable("Groups", "group_id,group_name,direction");
			GroupsRelatedData.AddRelation("GroupsDirections", "Groups,direction","Directions,direction_id");
			GroupsRelatedData.Load();


			//LoadGroupsRelatedData();
			cbDirections.DataSource = GroupsRelatedData.Set.Tables["Directions"];
			cbDirections.ValueMember = "direction_id";
			cbDirections.DisplayMember = "direction_name";

			cbGroups.DataSource = GroupsRelatedData.Set.Tables["Groups"];
			cbGroups.ValueMember = "group_id";
			cbGroups.DisplayMember = "group_name";
		}
		private void cbDirections_SelectedIndexChanged(object sender, EventArgs e)
		{
			Console.WriteLine(GroupsRelatedData.Set.Tables["Directions"].ChildRelations);
			DataRow row = GroupsRelatedData.Set.Tables["Directions"].Rows.Find(cbDirections.SelectedValue);

			/*cbGroups.DataSource = row.GetChildRows("GroupsDirections");
			cbGroups.DisplayMember = "group_name";
			cbGroups.ValueMember = "group_id";*/
			GroupsRelatedData.Set.Tables["Groups"].DefaultView.RowFilter = $"direction={cbDirections.SelectedValue}";
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		private void timer1_Tick(object sender, EventArgs e)
		{
			DateTime RefreshTimerCache = new DateTime();
			RefreshTimerCache = RefreshTimerCache.AddTicks(DateTime.Now.Ticks - timer.Ticks);
			if(RefreshTimerCache.Second == 1 || isNeedRefresh)
			{
				isNeedRefresh = false;
				GroupsRelatedData.CleareCahce();
				GroupsRelatedData.Load();
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			isNeedRefresh = true;
		}
	}
}

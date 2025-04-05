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
		Dictionary<int, string> d_groups = new Dictionary<int, string>
	{
		//{1, "group_id"},
		{1, "Группа"},					//group_name
		{2, "Дисциплина"},				//discipline_name
		{3, "Учебные дни" },			//weekdats
		{4, "Время начала занятий" }	//start_time
	};
		Dictionary<int, string> d_directions = new Dictionary<int, string>
	{
		{1, "direction_id" },
		{2, "direction_name" }
	};
		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			CONN_STR = ConfigurationManager.ConnectionStrings["PV_319_IMPORT"].ConnectionString;
			Connector connector = new Connector(CONN_STR);
			connector.LoadTable("SELECT group_name AS N'Группа', direction_name AS N'Дисциплина', dbo.GetLearningDays(group_name) AS N'Учебные дни', start_time AS N'Время начала занятий' FROM Groups, Directions WHERE direction=direction_id", "Groups", d_groups);
			connector.LoadTable("SELECT * FROM Directions", "Directions", d_directions);
		}
		
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}

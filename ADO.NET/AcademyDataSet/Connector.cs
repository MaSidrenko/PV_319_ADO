using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AcademyDataSet
{
	internal class Connector
	{
		readonly string CONNECTION_STRING;
		SqlConnection conn;
		public Connector(string connectionString)
		{
			CONNECTION_STRING = connectionString;
			conn = new SqlConnection(CONNECTION_STRING);
			AllocConsole();
			Console.WriteLine(CONNECTION_STRING);
		}
		~Connector()
		{
			FreeConsole();
		}

		public void LoadTable(string cmd, string table_name, Dictionary<int, string> table_columns)
		{
			try
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = new DataTable();

				dataTable.TableName = table_name;
				dataSet.Tables.Add(dataTable);

				DataTableReader reader = GetReader(cmd, table_name);

				dataSet.Load(reader, LoadOption.OverwriteChanges, dataTable);

				Console.WriteLine("========================================================================================================================");
				foreach (DataColumn columns in dataTable.Columns)
				{
					Console.Write((columns).ToString().PadRight(40));
				}
				Console.WriteLine();
				Console.WriteLine("========================================================================================================================");
				foreach (DataRow row in dataTable.Rows)
				{
					foreach (KeyValuePair<int, string> rows in table_columns)
					{
						Console.Write($"{row[rows.Value]}".PadRight(40));
					}
					Console.WriteLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		private DataTableReader GetReader(string cmd, string name)
		{
			DataTable table = null;
			try
			{
				table = new DataTable(name);
				SqlDataAdapter adapter = new SqlDataAdapter(cmd, conn);
				adapter.Fill(table);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return table.CreateDataReader();
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}


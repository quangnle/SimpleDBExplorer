using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBExplorer
{
    public partial class FrmMain : Form
    {
        private SqlConnection _connection;
        private DBAccess _db;
        public FrmMain()
        {
            InitializeComponent();
            lstTables.SelectedIndexChanged += LstTables_SelectedIndexChanged;
        }

        void LstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tableName = ((string)lstTables.SelectedItem).Split(new char[] {' '})[0];
            var gridSource = _db.GetTableInfo(tableName);

            dataGridView1.DataSource = gridSource;
            dataGridView1.Refresh();
        }

        private void tbtnLogin_Click(object sender, EventArgs e)
        {
            _connection = new SqlConnection();

            var connectionStr = "data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework";
            _connection.ConnectionString = String.Format(connectionStr, txtServer.Text, txtDB.Text, txtUid.Text, txtPassword.Text);

            try
            {
                _db = new DBAccess(_connection);
                var tables = _db.GetAllTables();

                foreach (var item in tables)
                {
                    lstTables.Items.Add(string.Format("{0} ({1})", item.Name, item.NumberOfColumns));
                }

                toolStripStatusLabel1.Text = "Connected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

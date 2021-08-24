using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechDevs_Member_Maintenance
{
    static class Lol
    {

    }
    public partial class MainUI : Form
    {
        // Data source for DataGridView of MainUI
        private DataTable memberDataTable;

        public MainUI()
        {
            InitializeComponent();
            memberDataTable = new DataTable();
        }
       
        private void MainUI_Load(object sender, EventArgs e)
        {
            // Fetch data from SQL db and set as source for DataGridView
            RefreshMemberData();
            BindingSource SBind = new BindingSource();
            SBind.DataSource = memberDataTable;
            MemberDataGridView.DataSource = SBind;

            MemberDataGridView.Columns[0].Visible = false; // Hide ID from user (primary key)
        }

        private void RefreshMemberData()
        {
            // Refreshes the data table
            string connString = ConnectionHelper.ConnVal("TD_MembersDb");
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand loadAllCmd = new SqlCommand("SELECT * FROM Members", conn))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(loadAllCmd))
            {
                memberDataTable.Rows.Clear();
                dataAdapter.Fill(memberDataTable);
            }
        }

        private void Insert_Click(object sender, EventArgs e)
        {
            // Runs when "Add New Member" button is clicked
            InsertUI insertUI = new InsertUI();
            insertUI.ShowDialog();

            // Refresh data table
            RefreshMemberData();
        }

        private void Update_Click(object sender, EventArgs e)
        {
            // Runs when "Edit" button is clicked
            Member memberToUpdate;

            try
            {
                // Get member data of 1st row selected
                var rowToUpdate = MemberDataGridView.SelectedRows[0].Cells;
                memberToUpdate.MemberId = (int)rowToUpdate[0].Value;
                memberToUpdate.Name = (string)rowToUpdate[1].Value;
                memberToUpdate.ContactNum = (string)rowToUpdate[2].Value;
                memberToUpdate.Email = (string)rowToUpdate[3].Value;

                // Open insert form in 'updating' mode
                InsertUI insertUI = new InsertUI(updating: true, memberToUpdate: memberToUpdate);
                insertUI.ShowDialog();

                // Refresh data table
                RefreshMemberData();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DialogResult errorMessage = MessageBox.Show("No entry selected; please ensure entire row is selected via the leftmost column.", "Error");
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            // Runs when "Delete" button is clicked

            // If no row selected, return
            if (MemberDataGridView.SelectedRows.Count == 0)
            {
                DialogResult errorMessage = MessageBox.Show("No entry selected; please ensure entire row is selected via the leftmost column", "Error");
                return;
            }

            // Confirm deletion from user
            DialogResult confirmDel = MessageBox.Show("Are you sure you wish to delete the selected Members?", "Delete Member", MessageBoxButtons.YesNo);

            if (confirmDel == DialogResult.Yes)
            {
                string connString = ConnectionHelper.ConnVal("TD_MembersDb");
                foreach (DataGridViewRow row in MemberDataGridView.SelectedRows)
                {
                    // Get MemberId of selected member for deletion
                    int delId = (int)row.Cells[0].Value;

                    // Execute DELETE SQL query
                    using (SqlConnection conn = new SqlConnection(connString))
                    using (SqlCommand delCmd = new SqlCommand($"DELETE FROM Members WHERE MemberId = {delId}", conn))
                    {
                        conn.Open();
                        delCmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                // Refresh data table
                RefreshMemberData();
            }


        }
    }
}

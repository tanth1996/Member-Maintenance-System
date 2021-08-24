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

namespace TechDevs_Member_Maintenance
{
    public partial class InsertUI : Form
    {
        // Flag to track if this form should be in a mode to update members instead of to add new members
        bool UpdateMode = false;
        Member memberToUpdate;

        public InsertUI(bool updating=false)
        {
            // Constructor for initialisation in Insert/Add New Member mode
            UpdateMode = updating;
            InitializeComponent();
        }
        public InsertUI(bool updating, Member memberToUpdate)
        {
            // Overload of constructor for Updating/Edit Member mode
            if (!updating)
                throw new ArgumentException("updating cannot be false if memberToUpdate is passed");
            UpdateMode = updating;
            this.memberToUpdate = memberToUpdate;

            InitializeComponent();

            // Populate text boxes with memberToUpdate info
            nameTextBox.Text = memberToUpdate.Name;
            contactNumTextBox.Text = memberToUpdate.ContactNum;
            emailTextBox.Text = memberToUpdate.Email;

            // Change other window properties for updating mode
            this.Text = "Edit Member";
        }

        private void insertButton2_Click(object sender, EventArgs e)
        {
            if (!UpdateMode)
            {
                // Execute INSERT SQL Query
                string insertQuery = "INSERT INTO Members (Name, ContactNum, Email) VALUES(" +
                    $"'{(string)nameTextBox.Text}', '{(string)contactNumTextBox.Text}', '{(string)emailTextBox.Text}')";
                string connString = ConnectionHelper.ConnVal("TD_MembersDb");
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    conn.Open();
                    insertCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else
            {
                // Execute UPDATE SQL Query
                string updateQuery = "UPDATE Members SET " +
                    $"Name='{(string)nameTextBox.Text}', ContactNum='{(string)contactNumTextBox.Text}', Email='{(string)emailTextBox.Text}' " +
                    $"WHERE MemberId={memberToUpdate.MemberId}";
                string connString = ConnectionHelper.ConnVal("TD_MembersDb");
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    conn.Open();
                    updateCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            // Close this window
            this.Close();
        }

    }
}

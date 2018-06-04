using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace ProjectFifaV2
{
    class DatabaseHandler
    {
        private SqlConnection con;

        public DatabaseHandler()
        {
            //SqlCeEngine engine = new SqlCeEngine(@"Data Source=.\DB.sdf");
            //engine.Upgrade(@"Data Source=.\DB2.sdf");


            string Path = Environment.CurrentDirectory;
            string[] appPath = Path.Split(new string[] { "bin" }, StringSplitOptions.None);
            AppDomain.CurrentDomain.SetData("DataDirectory", appPath[0]);

            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\db.mdf';Integrated Security=True;Connect Timeout=30");
        }

        public void TestConnection()
        {
            bool open = false;
            
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    open = true;
                }
                con.Close();
            }

            if (!open)
            {
                Application.Exit();
            }
        }

        public void OpenConnectionToDB()
        {
            con.Open();
        }

        public void CloseConnectionToDB()
        {
            con.Close();
        }

        public System.Data.DataTable FillDT(string query)
        {
            TestConnection();
            OpenConnectionToDB();

            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, GetCon());
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            
            CloseConnectionToDB();

            return dt;
        }

        public SqlConnection GetCon()
        {
            return con;
        }

        //Saves data into database INSERT Query
        public void SaveImportDataToDatabase(DataTable importData)
        {
            {
                foreach (DataRow importRow in importData.Rows)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO TblGames (HomeTeam, AwayTeam, HomeTeamScore, AwayTeamScore)" +
                                        "Values (@Home, @Away, @HomeScore, @AwayScore)", con);

                    cmd.Parameters.AddWithValue("@Home", importRow["team_a"]);
                    cmd.Parameters.AddWithValue("@Away", importRow["team_b"]);
                    cmd.Parameters.AddWithValue("@HomeScore", importRow["score_team_a"]);
                    cmd.Parameters.AddWithValue("@AwayScore", importRow["score_team_b"]);

                    cmd.ExecuteNonQuery();
                }

            }
        }
    }
}

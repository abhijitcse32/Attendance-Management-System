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
using Oracle.DataAccess.Client;
using CopyServerToLocal.DALL.Gateway;
using System.Threading;


namespace CopyServerToLocal
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        SqlConnection connSQL;
        OracleConnection connORA;

        private int GetTotalRecords()
        {
            int TotalRecords = 0, TB_TA_Result = 0, emp = 0, dept = 0;
            try
            {
                using (connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON))
                {
                    SqlCommand cmdEMP = new SqlCommand("SELECT COUNT(nUserIdn) FROM TB_USER", connSQL);
                    connSQL.Open();
                    emp = int.Parse(cmdEMP.ExecuteScalar().ToString());
                    connSQL.Close();

                    SqlCommand cmdDept = new SqlCommand("SELECT COUNT(nDepartmentIdn) FROM TB_USER_DEPT", connSQL);
                    connSQL.Open();
                    dept = int.Parse(cmdDept.ExecuteScalar().ToString());
                    connSQL.Close();

                    SqlCommand cmdResult = new SqlCommand("SELECT COUNT(nUserIdn) FROM TB_TA_RESULT", connSQL);
                    connSQL.Open();
                    TB_TA_Result = int.Parse(cmdResult.ExecuteScalar().ToString());
                    connSQL.Close();

                    TotalRecords = emp + dept + TB_TA_Result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return TotalRecords;
        }

        private int GetEmpRecords()
        {
            int emp = 0;
            try
            {
                using (connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON))
                {
                    SqlCommand cmdEMP = new SqlCommand("SELECT COUNT(nUserIdn) FROM TB_USER", connSQL);
                    connSQL.Open();
                    emp = int.Parse(cmdEMP.ExecuteScalar().ToString());
                    connSQL.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return emp;
        }


        private int GetDeptRecords()
        {
            int dept = 0;
            try
            {
                using (connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON))
                {
                    SqlCommand cmdEMP = new SqlCommand("SELECT COUNT(nDepartmentIdn) FROM TB_USER_DEPT", connSQL);
                    connSQL.Open();
                    dept = int.Parse(cmdEMP.ExecuteScalar().ToString());
                    connSQL.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dept;
        }

        private void btDownload_Click(object sender, EventArgs e)
        {

            DownLoadEMP();
            //DownLoadDept();
          DownLoadResult();
            label1.ForeColor = Color.Blue;
            label1.Font = new Font("Arial", 16, FontStyle.Bold);
            label1.Text = "Data Saved Successfully";
            btDownload.Font = new Font("Arial", 16, FontStyle.Bold);
            int i;

            progressBar1.Minimum = 0;
            //progressBar1.Maximum = table.Items.Count;
            int TotalRecords = GetTotalRecords();
            progressBar1.Maximum = TotalRecords;

            for (i = 0; i <= TotalRecords; i++)
            {
                progressBar1.Value = i;
            }
        }

    

        public void DownLoadEMP()
        {

            connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON);
            String CommandText = "SELECT nUserIdn,sUserName, nDepartmentIdn,sUserID FROM  TB_USER";
            SqlCommand cmdEMP = new SqlCommand(CommandText, connSQL);
            connSQL.Open();
            DataTable tblEMP = new DataTable();
            using (SqlDataAdapter daEMP = new SqlDataAdapter(cmdEMP))
            {
                daEMP.Fill(tblEMP);
            }
                       
            try
            {

                string qu_del = @"delete from KPL.ATTN_EMP";

                connORA = new OracleConnection(DataConnectionString.OraKPL);
                OracleCommand cmdResultdel = new OracleCommand(qu_del, connORA);
                connORA.Open();
                cmdResultdel.ExecuteNonQuery();
                connORA.Close();

                string query = @"insert into KPL.ATTN_EMP ( 
                                    EMPID, EMP_NAME, DEPT_CODE, MANUALID
                                    ) values 
                                    (
                                    :EMPID, :EMP_NAME, :DEPT_CODE, :MANUALID
                                    )";
                connORA = new OracleConnection(DataConnectionString.OraKPL);
                connORA.Open();
                using (var command = connORA.CreateCommand())
               {
                   command.CommandText = query;
                   command.CommandType = CommandType.Text;
                   command.Parameters.Add(":EMPID", OracleDbType.Int32);
                   command.Parameters.Add(":EMP_NAME", OracleDbType.Varchar2);
                   command.Parameters.Add(":DEPT_CODE", OracleDbType.Int16);
                   command.Parameters.Add(":MANUALID", OracleDbType.Int32);
                   foreach (DataRow r in tblEMP.Rows)
                   {
                       for (int i = 0; i < 4; i++)
                       {
                           command.Parameters[i].Value = r[i];
                       }
                       command.ExecuteNonQuery();
                   }
               }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Not insert");
            }
            finally
            {
                connORA.Close();
                connSQL.Close();
            }
        }

        /////////////////////////////////////////////////

        public void DownLoadDept()
        {

          connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON);
            String StrDept = "SELECT nDepartmentIdn,sName FROM  TB_USER_DEPT";
            SqlCommand cmdDept = new SqlCommand(StrDept, connSQL);
            connSQL.Open();
            DataTable tblDept = new DataTable();
            using (SqlDataAdapter daDept = new SqlDataAdapter(cmdDept))
            {
                daDept.Fill(tblDept);
            }

            try
            {

                string qu_del = @"delete from KPL.ATTN_DEPT";

                connORA = new OracleConnection(DataConnectionString.OraKPL);
                OracleCommand cmdResultdel = new OracleCommand(qu_del, connORA);
                connORA.Open();
                cmdResultdel.ExecuteNonQuery();
                connORA.Close();

                string query = @"insert into KPL.ATTN_DEPT ( 
                                    DEPT_CODE, DEPT_NAME
                                    ) values 
                                    (
                                    :DEPT_CODE, :DEPT_NAME
                                    )";
                connORA = new OracleConnection(DataConnectionString.OraKPL);
                connORA.Open();
                using (var command = connORA.CreateCommand())
                {
                    
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(":DEPT_CODE", OracleDbType.Int16);
                    command.Parameters.Add(":DEPT_NAME", OracleDbType.Varchar2);
                    foreach (DataRow r in tblDept.Rows)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            command.Parameters[i].Value = r[i];
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Not insert");
            }
            finally
            {
                connORA.Close();
                connSQL.Close();
            }
        }

        //////////////////////////////////////////////////////




       


        /////////////////////////////////////////////////////


        public void DownLoadResult()
        {

            connSQL = new SqlConnection(DataConnectionString.BIOSTAR_CON);
            String StrResult = "SELECT nDateTime,nUserIdn,nStartTime,nEndTime,nTAResult,nWorkTime FROM TB_TA_RESULT";
            //String StrResult = "SELECT nDateTime FROM  TB_TA_RESULT";
            SqlCommand cmdResult = new SqlCommand(StrResult, connSQL);
            connSQL.Open();
            DataTable tblResult = new DataTable();
            using (SqlDataAdapter daResult = new SqlDataAdapter(cmdResult))
            {
                daResult.Fill(tblResult);

            }
            //MessageBox.Show("AAAAAAAAA" + tblResult.Rows.Count);

            try
            {
                string qu_del = @"delete from KPL.ATTN_EMP_RESULT";

                connORA = new OracleConnection(DataConnectionString.OraKPL);
                OracleCommand cmdResultdel = new OracleCommand(qu_del, connORA);
                connORA.Open();
                cmdResultdel.ExecuteNonQuery();
                connORA.Close();

                string query = @"insert into KPL.ATTN_EMP_RESULT ( 
                                                    DATE_TIME, EMPID, START_TIME, END_TIME, RESULT, WORK_TIME, MNYR
                                                    ) values 
                                                    (
                                                    :DATE_TIME, :EMPID, :START_TIME, :END_TIME, :RESULT, :WORK_TIME,:MNYR
                                                    )";

//                string query = @"insert into KPL.ATTN_RESULT ( 
//                                    EMPID
//                                    ) values 
//                                    (
//                                    :EMPID
//                                    )";

                connORA = new OracleConnection(DataConnectionString.OraKPL);
                connORA.Open();


                foreach (DataRow r in tblResult.Rows)
                {

                    OracleCommand result = new OracleCommand(query, connORA);
                    result.Parameters.Add(":DATE_TIME", OracleDbType.Int64).Value = Convert.ToInt64(r["nDateTime"]);
                    result.Parameters.Add(":EMPID", OracleDbType.Int32).Value = Convert.ToInt32(r["nUserIdn"]);
                    result.Parameters.Add(":START_TIME", OracleDbType.Int64).Value = Convert.ToInt64(r["nStartTime"]);
                    result.Parameters.Add(":END_TIME", OracleDbType.Int64).Value = Convert.ToInt64(r["nEndTime"]);
                    result.Parameters.Add(":RESULT", OracleDbType.Int64).Value = Convert.ToInt64(r["nTAResult"]);
                    result.Parameters.Add(":WORK_TIME", OracleDbType.Int64).Value = Convert.ToInt64(r["nWorkTime"]);
                    DateTime dtmnyr = new DateTime(1970, 1, 1).AddTicks(Convert.ToInt64(Convert.ToInt64(r["nDateTime"])) * 10000 * 1000);
                    result.Parameters.Add(":MNYR", OracleDbType.Date).Value = dtmnyr.ToShortDateString();
                    int intres = result.ExecuteNonQuery();
                    //result.ExecuteNonQuery();
                }

                //foreach (DataRow r in tblResult.Rows)
                //{
                //    for (int i = 0; i < 6; i++)
                //    {
                //        command.Parameters[i].Value = r[i];
                //        // DateTime converted = new DateTime(1970, 1, 1).AddTicks(Convert.ToInt64(abc) * 10000);
                //    }
                //    command.ExecuteNonQuery();
                //}
            }

            catch (OracleException ex)
            {
                MessageBox.Show("Not insert");
            }
            finally
            {
                connORA.Close();
                connSQL.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DateTime converted = new DateTime(1970, 1, 1).AddTicks(Convert.ToInt64(1457827200) * 10000 * 1000);
            //MessageBox.Show(converted.ToShortDateString());

            label1.Text = "Data Saved Successfully";
            progressBar1.Minimum = 0;
            //progressBar1.Maximum = table.Items.Count;
            int EmpRecords = GetEmpRecords(), i;
            progressBar1.Maximum = EmpRecords;

            for (i = 0; i <= EmpRecords; i++)
            {
                DownLoadEMP();
                DownLoadDept();
                DownLoadResult();
                progressBar1.Value = i;
            }

           
            //DownLoadDept();
            
            //progressBar1.Minimum = TotalRecords;
            //int DeptRecords = GetDeptRecords(), j;

            //for (j = TotalRecords; j <= DeptRecords; j++)
            //{
            //    progressBar1.Value = j;
            //}

            //int i;

            //progressBar1.Minimum = 0;
            //progressBar1.Maximum = 200;

            //for (i = 0; i <= 200; i++)
            //{
            //    progressBar1.Value = i;
            //}
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                // Wait 100 milliseconds.
                Thread.Sleep(100);
                // Report progress.
                backgroundWorker1.ReportProgress(i);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage;
            // Set the text.
            this.Text = e.ProgressPercentage.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        

    

    }
}

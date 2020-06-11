using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    class SQLData : Helpers
    {
        // Insert list of IPTPSH from STRATIX into IMPORT
        public int Write_IPTPSH_IMPORT(List<IPTPSH> lstIPTPSH)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from ST_IMPORT_tbl_IPTPSH";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = "INSERT INTO ST_IMPORT_tbl_IPTPSH (psh_job_no,psh_pwc,psh_pwg,psh_sch_seq_no,psh_sch_ltts,job_desc30) " +
                                        "VALUES (@job,@pwc,@pwg,@seq,@schltts,@desc)";

                    cmd.Parameters.Add("@job", SqlDbType.Int);
                    cmd.Parameters.Add("@pwc", SqlDbType.VarChar);
                    cmd.Parameters.Add("@pwg", SqlDbType.VarChar);
                    cmd.Parameters.Add("@seq", SqlDbType.Int);
                    cmd.Parameters.Add("@schltts", SqlDbType.DateTime);
                    cmd.Parameters.Add("@desc", SqlDbType.VarChar);

                    foreach (IPTPSH s in lstIPTPSH)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(s.job_no);
                        cmd.Parameters[1].Value = s.pwc;
                        cmd.Parameters[2].Value = s.pwg;
                        cmd.Parameters[3].Value = Convert.ToInt32(s.seq_no);
                        cmd.Parameters[4].Value = Convert.ToDateTime(s.sch_ltts);
                        cmd.Parameters[5].Value = s.desc30;                    

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't lave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(psh_job_no) from ST_IMPORT_tbl_IPTPSH";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        // Insert values from IMPORT into WIP IPTPSH
        public int Write_IMPORT_to_IPTPSH()
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to IPTPSH table.  Return rows inserted.
                cmd.CommandText = "ST_proc_IMPORT_to_IPTPSH";
              
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        } 
    }
}

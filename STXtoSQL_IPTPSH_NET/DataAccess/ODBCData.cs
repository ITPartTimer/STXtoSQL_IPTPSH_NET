using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<IPTPSH> Get_IPTPSH()
        {

            List<IPTPSH> lstIPTPSH = new List<IPTPSH>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select psh_job_no,psh_pwc,psh_pwg,psh_sch_seq_no,psh_sch_ltts,job_desc30
                                    from iptpsh_rec s inner join iptjob_rec j on j.job_job_no = s.psh_job_no
                                    where s.psh_whs = 'SW' and psh_sch_seq_no <> 99999999
                                    and (job_job_sts = 0 or job_job_sts = 1)
                                    and (job_prs_cl = 'SL' or job_prs_cl = 'CL' or job_prs_cl = 'MB')";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        IPTPSH b = new IPTPSH();

                        b.job_no = Convert.ToInt32(rdr["psh_job_no"]);
                        b.pwc = rdr["psh_pwc"].ToString();
                        b.pwg = rdr["psh_pwg"].ToString();
                        b.seq_no = Convert.ToInt32(rdr["psh_sch_seq_no"]);                     
                        b.sch_ltts = rdr["psh_sch_ltts"].ToString();
                        b.desc30 = rdr["job_desc30"].ToString();                     

                        lstIPTPSH.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstIPTPSH;
        }
    }
}

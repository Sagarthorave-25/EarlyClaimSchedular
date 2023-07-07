using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
namespace SchedulerEarlyClaim
{
    class sql
    {
         string strConn = ConfigurationSettings.AppSettings["Trnx"].ToString().Trim();
        string strConn1 = ConfigurationSettings.AppSettings["Trnx1"].ToString().Trim();
        string strConn2 = ConfigurationSettings.AppSettings["Trnx2"].ToString().Trim();
        string strConn3 = ConfigurationSettings.AppSettings["Trnx3"].ToString().Trim();
        public void featchSendEmailVal(String spName, ref DataSet _dt)
        {
            _dt = SqlHelper.ExecuteDataset(strConn, CommandType.StoredProcedure, spName);
        }
        public void featchSendEmailCC(String spName, ref DataSet _dt)
        {
            _dt = SqlHelper.ExecuteDataset(strConn3, CommandType.StoredProcedure, spName);
        }
        public void featchSendEmailVal1(String spName, ref DataSet _dt)
        {
            _dt = SqlHelper.ExecuteDataset(strConn1, CommandType.StoredProcedure, spName);
        }
        public void featchSendEmailVal2(String spName, ref DataSet _dt,String AgentNo, string Appno)
        {
            SqlConnection connection = new SqlConnection(strConn2);
            
            
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SqlCommandTimeOut"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AGENTID", AgentNo);
            cmd.Parameters.AddWithValue("@APPNO", Appno);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(_dt);
            connection.Close();
          
        
            //_dt = SqlHelper.ExecuteDataset(strConn2, CommandType.StoredProcedure, spName,_sqlparam);
        }
        public void featchSendEmailVal3(String spName, ref DataSet _dt, string AgentNo)
        {

            SqlConnection connection = new SqlConnection(strConn1);
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AGENTID", AgentNo);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(_dt);
            connection.Close();

            //_dt = SqlHelper.ExecuteDataset(strConn1, CommandType.StoredProcedure, spName);
        }
        public void InsertCommunicationLog(String strErrorLogDesc, String strClassName, String strMethodName, String strDataKey,
         String strDataValue, String strSubDataKey, String strSubDataValue, String strComments)
        {

            try
            {
                SqlConnection con = new SqlConnection(strConn3);
                SqlCommand cmd = new SqlCommand("USP_Log_EarlyClaimAutomation_Insert_Logs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ErrorLogDesc", strErrorLogDesc);
                cmd.Parameters.AddWithValue("@ClassName", strClassName);
                cmd.Parameters.AddWithValue("@MethodName", strMethodName);
                cmd.Parameters.AddWithValue("@DataKey", strDataKey);
                cmd.Parameters.AddWithValue("@DataValue", strDataValue);
                cmd.Parameters.AddWithValue("@SubDataKey ", strSubDataKey);
                cmd.Parameters.AddWithValue("@SubDataValue", strSubDataValue);
                cmd.Parameters.AddWithValue("@ApplicationSource", "Interim");
                cmd.Parameters.AddWithValue("@Comments", strComments);
                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) {
                string msg = ex.Message;
            }
        }
    }
}

//using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace WindowServiceMonitorDLL
{
    public class Process
    {
        public static Response ServiceLastRunDate(string ServiceName, string sqlConnStr)
        {
            Response Resp = new Response();
            try
            {
                DataAccess.DataAccessSql<int> DtSql = new DataAccess.DataAccessSql<int>(sqlConnStr);
                DtSql.comm.Parameters.AddWithValue("@serviceName", SqlDbType.VarChar).Value = ServiceName;
                int count = DtSql.ExecuteScalar("select count(*) as count from zib_service_monitor where service_name = @serviceName");
                if (count > 0)
                {
                    DtSql = new DataAccess.DataAccessSql<int>(sqlConnStr);
                    DtSql.comm.Parameters.AddWithValue("@serviceName", SqlDbType.VarChar).Value = ServiceName;
                    int AffectedRow = DtSql.ExecuteNonQuery("update zib_service_monitor set last_run_date = getdate() where service_name = @serviceName");
                    if (AffectedRow > 0)
                    {
                        Resp.ResponseCode = "00";
                        Resp.ResponseMessage = "Successful";
                    }
                }
                else
                {
                    DtSql = new DataAccess.DataAccessSql<int>(sqlConnStr);
                    DtSql.comm.Parameters.AddWithValue("@serviceName", SqlDbType.VarChar).Value = ServiceName;
                    int AffectedRow = DtSql.ExecuteNonQuery("insert into zib_service_monitor (service_name, last_run_date) values (@serviceName, getdate())");
                    if (AffectedRow > 0)
                    {
                        Resp.ResponseCode = "00";
                        Resp.ResponseMessage = "Successful";
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Resp;
        }
        
        public static Response FunctionCall(FunctionDetails functionDetails, string sqlConnStr)
        {
            Response Resp = new Response();
            try
            {
                DataAccess.DataAccessSql<zib_service_monitor> DtSql = new DataAccess.DataAccessSql<zib_service_monitor>(sqlConnStr);
                DtSql.comm.Parameters.AddWithValue("@serviceName", SqlDbType.VarChar).Value = functionDetails.ServiceName;
                List<zib_service_monitor> serviceParam = DtSql.ExecuteReader("select * from zib_service_monitor where service_name = @serviceName");

                if (serviceParam.Count <= 0)
                {
                    Resp.ResponseCode = "77";
                    Resp.ResponseMessage = functionDetails.ServiceName + " need to first call ServiceLastRunDate";
                    return Resp;
                }

                string functioDetailsString = serviceParam.FirstOrDefault().function_details;
                List<FunctionDetail> functioDetailsObject = string.IsNullOrEmpty(functioDetailsString) ? new List<FunctionDetail>() : JsonConvert.DeserializeObject<List<FunctionDetail>>(functioDetailsString);
                var singleParam = functioDetailsObject.Where(param => param.Key == functionDetails.MethodName).Select(o => o).SingleOrDefault();

                if (singleParam != null)
                {
                    functioDetailsObject.Remove(singleParam);
                    singleParam.Value = DateTime.Now.ToString();
                }
                else
                {
                    singleParam = new FunctionDetail()
                    {
                        Key = functionDetails.MethodName,
                        Value = DateTime.Now.ToString()
                    };
                }

                functioDetailsObject.Add(singleParam);
                functioDetailsString = JsonConvert.SerializeObject(functioDetailsObject);

                DtSql = new DataAccess.DataAccessSql<zib_service_monitor>(sqlConnStr);
                DtSql.comm.Parameters.AddWithValue("@serviceName", SqlDbType.VarChar).Value = functionDetails.ServiceName;
                DtSql.comm.Parameters.AddWithValue("@functionDetails", SqlDbType.VarChar).Value = functioDetailsString;
                int AffectedRow = DtSql.ExecuteNonQuery("update zib_service_monitor set function_details = @functionDetails where service_name = @serviceName");
                if (AffectedRow > 0)
                {
                    Resp.ResponseCode = "00";
                    Resp.ResponseMessage = "Successful";
                }                
            }
            catch (Exception ex)
            {
                throw;
            }
            return Resp;
        }
    }
}

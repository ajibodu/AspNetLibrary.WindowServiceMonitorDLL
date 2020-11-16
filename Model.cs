using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowServiceMonitorDLL
{
    public class FunctionDetails
    {
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public string MethodName { get; set; }
    }

    public class FunctionDetail
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class zib_service_monitor
    {
        public string service_name { get; set; }
        public DateTime last_run_date { get; set; }
        public string function_details { get; set; }
    }

    public class Response
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}

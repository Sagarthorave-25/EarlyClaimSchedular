using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerEarlyClaim
{
    class BlobClass
    {
    }
    public class RequestGenerateToken
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string source { get; set; }
    }

    public class ResponseGenerateToken
    {
        public string tokenErrorCode { get; set; }
        public string tokenErrorDesc { get; set; }
        public string token { get; set; }
    }

    public class RequestPutBlobStorage
    {
        public string applicationNumber { get; set; }
        public string containName { get; set; }
        public string subFolder { get; set; }
        public string fileName { get; set; }
        public string fileBytes { get; set; }
        public string token { get; set; }
    }
    public class ResponsePutBlobStorage
    {
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public string filePath { get; set; }
    }
}

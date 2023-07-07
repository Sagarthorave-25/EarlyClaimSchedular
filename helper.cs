using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NReco.PdfGenerator;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net.Mail;
using RestSharp;
using System.Web.Script.Serialization;

namespace SchedulerEarlyClaim
{
   public class helper
    {
        SqlParameter[] _sqlparam = new SqlParameter[0];
        sql sqlClass = new sql();
        //  static string Trnxconnection = ConfigurationSettings.AppSettings["Trnx"].ToString().Trim();
        string TempleteFilepath = ConfigurationSettings.AppSettings["HTMLTemplatePath"].ToString().Trim();
        string SavePdfPath = ConfigurationSettings.AppSettings["SavePdfPath"].ToString().Trim();
        static string BlobTokenAPI = ConfigurationSettings.AppSettings["GenerateToken"].ToString().Trim();
        //Fetch data from databse to send Email

        public string BlobTokn()
        {
            string BlobTokenAPI = ConfigurationSettings.AppSettings["GenerateToken"].ToString().Trim();

            BlobClass blobClass = new BlobClass();
            RequestGenerateToken generateToken = new RequestGenerateToken();
            generateToken.userName = ConfigurationSettings.AppSettings["BlobUserName"].ToString().Trim();
            generateToken.password = ConfigurationSettings.AppSettings["BlobPassword"].ToString().Trim();
            generateToken.source = ConfigurationSettings.AppSettings["BlobSource"].ToString().Trim();

            var json2 = SimpleJson.SerializeObject(generateToken);
            var webResponse = GenerateWebRequest(BlobTokenAPI, json2);
            var vOthr = new JavaScriptSerializer().Deserialize<ResponseGenerateToken>(webResponse);
            if (vOthr.tokenErrorCode == "0")
            {
                //new BussLayer().InsertMerVidLogs("Blob Token Generated", "BussLayer", "GetBlobTokn", "TokenID", vOthr.token, "Blob Token Generated");
                return vOthr.token;
            }
            else
            {
              //  new BussLayer().InsertMerVidLogs("Exception", "helper", "GetBlobTokn", "TokenErrorCode", vOthr.tokenErrorCode, "Exception: " + vOthr.tokenErrorDesc);
                return "";
            }

        }
        public String GenerateWebRequest(string loginURL, string objJsonString)
        {
            string webResponse = String.Empty;
            try
            {
                //RestClient for GenerateToken
                var client = new RestClient(loginURL);
                var request = new RestRequest(Method.POST);

                // easily add HTTP Headers
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", objJsonString, ParameterType.RequestBody);

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content; // raw content as string

                if (response != null && !String.IsNullOrEmpty(content))
                {
                    webResponse = content;
                }
            }
            catch (Exception ex)
            {

            }
            return webResponse;
        }

        public DataSet GetPdfData()
        {
            int ii = 0;
            DataSet ds = new DataSet();
            sqlClass.featchSendEmailVal("USP_EARLY_CLAIMS_PDF_GENERATE", ref ds);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    ii = ds.Tables[0].Rows.Count;

                }
                DataSet ds1= GetPdfData1();
               

                //copy first table
                DataTable dk = ds1.Tables[0].AsEnumerable().CopyToDataTable();
                ds.Tables.Add(dk);
            }
        return ds;
        }
        public DataSet GetPdfData1()
        {
            int ii = 0;
            DataSet dt = new DataSet();
            sqlClass.featchSendEmailVal1("USP_EARLY_CLAIMS_PDF_GENERATE1", ref dt);
            if (dt != null)
            {
                if (dt.Tables.Count > 0)
                {
                    ii = dt.Tables[0].Rows.Count;
                }
            }
            return dt;
        }
        public DataSet GetPdfData2(string appno,string agentno) {
            int ii = 0;
            DataSet dt = new DataSet();
            sqlClass.featchSendEmailVal2("USP_Eary_Cliam_GenertePdfData", ref dt,agentno,appno);
            if (dt != null)
            {
                if (dt.Tables.Count > 0)
                {
                    ii = dt.Tables[0].Rows.Count;
                }
            }
            return dt;
        }
        public DataSet GetPdfData3(string AgentNo)
        {
            int ii = 0;
            DataSet dt = new DataSet();
            sqlClass.featchSendEmailVal3("USP_EARLY_CLAIMS_OTHERPOLICYDETAILS", ref dt,AgentNo);
            if (dt != null)
            {
                if (dt.Tables.Count > 0)
                {
                    ii = dt.Tables[0].Rows.Count;
                }
            }
            return dt;
        }
        public void GeneratePdf(DataSet dt,string token) {
            string SaveEmlpdf = ConfigurationSettings.AppSettings["SaveEmlpdf"].ToString();
            Common com = new Common();
            string falcoSts = string.Empty;
            string body2 = string.Empty;

            string AgentCode = string.Empty;
            string Appno = string.Empty;
            try {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (dt.Tables[2].Rows.Count > 0)
                    {
                        AgentCode = dt.Tables[2].Rows[i]["AgentCode"].ToString().Trim();
                        com.Er_policy_number = dt.Tables[2].Rows[i]["PolicyNo"].ToString().Trim();
                        //Claim Info
                        String Date_Er_date_of_loss = dt.Tables[2].Rows[i]["Date of Loss"].ToString().Trim();
                        DateTime Er_date_of_loss = DateTime.Parse(Date_Er_date_of_loss);
                        com.Er_date_of_loss = Er_date_of_loss.ToString("dd-MMM-yyyy");

                        String DateOFIntemation = dt.Tables[2].Rows[i]["Date of Intimation"].ToString().Trim();
                        DateTime DateOFIntemations = DateTime.Parse(DateOFIntemation);
                        com.Er_date_of_intimation = DateOFIntemations.ToString("dd-MMM-yyyy");

                        com.Er_no_of_days = dt.Tables[2].Rows[i]["No of days elapsed from RCD to Death"].ToString().Trim();
                        com.Er_death_sum_assured = dt.Tables[2].Rows[i]["Death Sum Assured"].ToString().Trim();
                        com.Er_causes_of_death = dt.Tables[2].Rows[i]["Cause of death reported"].ToString().Trim();
                        if (!string.IsNullOrEmpty(AgentCode))
                        {
                            DataSet ds3 = GetPdfData3(AgentCode);
                            if (ds3.Tables[0].Rows.Count > 0)
                            {
                                com.Er_other_claim = ds3.Tables[0].Rows[i]["OTHERPOLICYDATA"].ToString().Trim();
                            }
                        }
                        }
                    if (dt.Tables[1].Rows.Count > 0)
                    {
                        string MHR_Value = dt.Tables[1].Rows[0]["Value"].ToString().Trim();
                        if (MHR_Value == "MHR - Due Diligence")
                        {
                            com.Er_mhr_provided_by_bm = "Yes";
                        }
                        else
                        {
                            com.Er_mhr_provided_by_bm = "No";
                        }
                    }
                    else
                    {
                        com.Er_mhr_provided_by_bm = "No";
                    }
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        com.Er_sum_assured = dt.Tables[0].Rows[i]["SumAssured"].ToString().Trim();
                        Appno = dt.Tables[0].Rows[i]["POL_applicationNoStr"].ToString().Trim();
                        com.Er_mode = dt.Tables[0].Rows[i]["Mode"].ToString().Trim();
                        com.Er_product_type = dt.Tables[0].Rows[i]["ProductType"].ToString().Trim();
                        com.Er_product_name = dt.Tables[0].Rows[i]["ProductName"].ToString().Trim();
                        com.Er_ape = dt.Tables[0].Rows[i]["APE"].ToString().Trim();
                        com.Er_insured_name = dt.Tables[0].Rows[i]["InsuredName"].ToString().Trim();
                        com.Er_age_of_life_assured = dt.Tables[0].Rows[i]["AgeOfLifeAss" +
                            "ured"].ToString().Trim();
                        com.Er_cusomer_location = dt.Tables[0].Rows[i]["CustomerLocation"].ToString().Trim();
                        com.Er_life_assured_profession = dt.Tables[0].Rows[i]["LifeAssuredProfession"].ToString().Trim();
                        string DateEr_risk_commencement_date = dt.Tables[0].Rows[i]["RiskCommencementdate"].ToString().Trim();
                        DateTime Er_risk_commencement_date = DateTime.Parse(DateEr_risk_commencement_date);
                        com.Er_risk_commencement_date = Er_risk_commencement_date.ToString("dd-MMM-yyyy");
                        if (!string.IsNullOrEmpty(Appno))
                        {
                            if (!string.IsNullOrEmpty(AgentCode))
                            {
                                DataSet ds2 = GetPdfData2(Appno, AgentCode);
                                if (ds2.Tables[0].Rows.Count > 0)
                                {
                                    com.Er_channel = ds2.Tables[0].Rows[i]["Channel"].ToString().Trim();
                                }
                                if (ds2.Tables[1].Rows.Count > 0)
                                {
                                    com.Er_partner_name = ds2.Tables[1].Rows[i]["Partner_Name"].ToString().Trim();
                                }
                                if (ds2.Tables[2].Rows.Count > 0)
                                {
                                    com.Er_zone = ds2.Tables[2].Rows[i]["Zone"].ToString().Trim();
                                }
                                if (ds2.Tables[3].Rows.Count > 0)
                                {
                                    com.Er_region = ds2.Tables[3].Rows[i]["Region"].ToString().Trim();

                                }
                                if (ds2.Tables[4].Rows.Count > 0)
                                {
                                    com.Er_regional_head = ds2.Tables[4].Rows[i]["Regional_Head"].ToString().Trim();
                                }
                                if (ds2.Tables[5].Rows.Count > 0)
                                {
                                    com.ER_Unit_Id = ds2.Tables[5].Rows[i]["UNIT_ID"].ToString().Trim();
                                }
                                if (ds2.Tables[6].Rows.Count > 0)
                                {
                                    com.Er_DIRECT_REMPORTING_MANAGER = ds2.Tables[6].Rows[i]["DIRECT_REMPORTING_MANAGER"].ToString().Trim();
                                }
                                if (ds2.Tables[7].Rows.Count > 0)
                                {
                                    com.Er_sourcing_agent = ds2.Tables[7].Rows[i]["SourcingAgentName"].ToString().Trim();
                                }
                                if (ds2.Tables[8].Rows.Count > 0)
                                {
                                    com.Er_sourcing_manager_status = ds2.Tables[8].Rows[i]["SourcingAgentStatus"].ToString().Trim();
                                }
                                if (ds2.Tables[9].Rows.Count > 0)
                                {
                                    com.Er_total_cases = ds2.Tables[9].Rows[i]["TotalCasesSourced"].ToString().Trim();
                                }
                                if (ds2.Tables[11].Rows.Count > 0)
                                {
                                    com.Er_death_before_proposal = ds2.Tables[11].Rows[i]["Death Before Proposal"].ToString().Trim();
                                }
                                if (ds2.Tables[12].Rows.Count > 0)
                                {
                                    com.Er_medical_nonmedical = ds2.Tables[12].Rows[i]["Category"].ToString().Trim();
                                }
                            }
                        }
                    }

                    //Early Claim Information - Urgent Attention Required
                    //Sales Information

                    //Pre-Issuance Risk verification

                    //Policy details




                    //set Templete Information
                    string path = "PolicyPdf.html";
                        StreamReader reader = new StreamReader(TempleteFilepath + path);
                        StringBuilder body = new StringBuilder("");
                        body.Append(reader.ReadToEnd());
                        //SalesBody
                        body = body.Replace("[Er_channel]", com.Er_channel);
                        body = body.Replace("[Er_partner_name]", com.Er_partner_name);
                        body = body.Replace("[Er_zone]", com.Er_zone);
                        body = body.Replace("[Er_region]", com.Er_region);
                        body = body.Replace("[Er_regional_head]", com.Er_regional_head);
                        body = body.Replace("[Unit_Id]", com.ER_Unit_Id);
                        body = body.Replace("[Er_sourcing_manager]", com.Er_DIRECT_REMPORTING_MANAGER);
                        body = body.Replace("[Er_sourcing_agent]", com.Er_sourcing_agent);
                        body = body.Replace("[Er_sourcing_manager_status]", com.Er_sourcing_manager_status);
                        body = body.Replace("[Er_total_cases]", com.Er_total_cases);
                        body = body.Replace("[Er_other_claim]", com.Er_other_claim);
                        body = body.Replace("[Er_death_before_proposal]", com.Er_death_before_proposal);
                        body = body.Replace("[Er_medical_nonmedical]", com.Er_medical_nonmedical);
                        //Policy details
                        body = body.Replace("[Er_sum_assured]", com.Er_sum_assured);
                        body = body.Replace("[Er_mode]", com.Er_mode);
                        body = body.Replace("[Er_product_type]", com.Er_product_type);
                        body = body.Replace("[Er_product_name]", com.Er_product_name);
                        body = body.Replace("[Er_ape]", com.Er_ape);
                        body = body.Replace("[Er_insured_name]", com.Er_insured_name);
                        body = body.Replace("[Er_age_of_life_assured]", com.Er_age_of_life_assured);
                        body = body.Replace("[Er_cusomer_location]", com.Er_cusomer_location);
                        body = body.Replace("[Er_risk_commencement_date]", com.Er_risk_commencement_date);
                        body = body.Replace("[Er_life_assured_profession]", com.Er_life_assured_profession);
                        body = body.Replace("[Er_policy_number]", com.Er_policy_number);
                        body = body.Replace("[Er_date_of_loss]", com.Er_date_of_loss);
                        body = body.Replace("[Er_date_of_intimation]", com.Er_date_of_intimation);
                        body = body.Replace("[Er_no_of_days]", com.Er_no_of_days);
                        body = body.Replace("[Er_death_sum_assured]", com.Er_death_sum_assured);
                        body = body.Replace("[Er_causes_of_death]", com.Er_causes_of_death);
                        body = body.Replace("[Er_mhr_provided_by_bm]", com.Er_mhr_provided_by_bm);
                        body2 = body.ToString();
                    if (!string.IsNullOrEmpty(body2))
                    {
                        byte[] filebytes = pdfcon(body2);
                        String localfilePathName = SavePdfPath;

                        //Create new directory
                        if (!System.IO.Directory.Exists(localfilePathName))
                        {
                            System.IO.Directory.CreateDirectory(localfilePathName);
                        }
                        if (!System.IO.Directory.Exists(SaveEmlpdf))
                        {
                            System.IO.Directory.CreateDirectory(SaveEmlpdf);
                        }
                        String FalcoTempName = "EarlyClaimMailerUAT";
                        int FalcoTempId = 33803;
                        string pdfFileName = localfilePathName + "PolicyNumber_" + com.Er_policy_number + ".pdf";
                        string EmlpdfFileName = SaveEmlpdf + "PolicyNumber_" + com.Er_policy_number + ".pdf";
                        File.WriteAllBytes(pdfFileName, filebytes);
                        File.WriteAllBytes(EmlpdfFileName, filebytes);

                        byte[] eml = SendMailAndCreateEML_Attachment(Appno, com.Er_policy_number, com.Er_insured_name, com.Er_risk_commencement_date, com.Er_date_of_intimation, com.Er_date_of_loss, com.Er_death_sum_assured, com.Er_causes_of_death, token, pdfFileName, FalcoTempName);

                        if (eml != null)
                        {
                            new sql().InsertCommunicationLog("Success", "SendSMS", "GeneratePdf", "PolicyNo", com.Er_policy_number, "ApplicationNo", Appno, "Eml file Generated");

                            falcoSts = SendByFalco(Appno, com.Er_policy_number, com.Er_insured_name, com.Er_risk_commencement_date, com.Er_date_of_intimation, com.Er_date_of_loss, com.Er_death_sum_assured, com.Er_causes_of_death, token, EmlpdfFileName, FalcoTempName, FalcoTempId);

                            if (falcoSts == "success")
                            {
                                new sql().InsertCommunicationLog("Success", "SendSMS", "GeneratePdf", "PolicyNo", com.Er_policy_number, "ApplicationNo", Appno, "falco status ok");
                            }
                            else
                            {
                                new sql().InsertCommunicationLog("error", "SendSMS", "GeneratePdf", "PolicyNo", com.Er_policy_number, "ApplicationNo", Appno, "falco status error");
                            }
                        }
                        else
                        {
                            new sql().InsertCommunicationLog("error", "SendSMS", "GeneratePdf", "PolicyNo", com.Er_policy_number, "ApplicationNo", Appno, "Eml file not generated error");

                        }


                        //string Mailbody= EmailTemplete(com.Er_policy_number, com.Er_insured_name,com.Er_risk_commencement_date, com.Er_date_of_intimation, com.Er_date_of_loss, com.Er_death_sum_assured, com.Er_causes_of_death);
                        //SmtpSendEmail(Mailbody, pdfFileName);
                    }
                }

            }
            catch (Exception ex) {
                string msg = ex.Message;
            }

           
        }
        public string Serialize(string fileName)
        {
            FileStream reader = new FileStream(fileName, FileMode.Open);
            byte[] buffer = new byte[reader.Length];
            reader.Read(buffer, 0, (int)reader.Length);
            string a = Convert.ToBase64String(buffer);
            return a;
        }
        public string SendByFalco(string Appno,string Policy_Number, string Insured_Name, string Risk_Commencement_Date, string Date_of_Intimation, string Date_of_Loss, string Death_Sum_Assured, string Cause_of_death_reported, string token, string PolicyFilePathName,String FalcoTempName, int FalcoTempId) {
            try
            {
                string s = "";
                string mailfromname = ConfigurationSettings.AppSettings["mailfromname"].ToString();
                string FromEmailID = ConfigurationSettings.AppSettings["FromEmailID"].ToString();
                string ToEmailID = ConfigurationSettings.AppSettings["ToEmailID"].ToString();
                string ToSubject = ConfigurationSettings.AppSettings["ToSubject"].ToString();

                string mailrespo = "";
                string[] recipients = { ToEmailID.Trim() };
                DataSet ds = getEmailCC();
                List<string> _data = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    _data.Add(ds.Tables[0].Rows[i]["Email"].ToString().Trim());
                }

                string[] recipientsCC = _data.ToArray();

                string content = "";
                string[] filename = null;
                string[] filecontent = null;
                filecontent = new string[1];
                filecontent[0] = Serialize(PolicyFilePathName);
                filename = new string[1];
                filename[0] = "PolicyNumber " + Policy_Number + ".pdf";

                string apiheaderNum = Policy_Number + "_" + DateTime.Now.ToString("ddmmyyyyHHMMss");
                string[] apiheader = { apiheaderNum };
                string[] attr = { "" };
                    string otherAttributes = "\"Policy_Number\":\"" + Policy_Number + "\""
                         + ",\"Insured_Name\":\"" + Insured_Name + "\""
                         + ",\"Risk_Commencement_Date\":\"" + Risk_Commencement_Date + "\""
                         + ",\"Date_of_Intimation\":\"" + Date_of_Intimation + "\""
                         + ",\"Date_of_Loss\":\"" + Date_of_Loss + "\""
                         + ",\"Death_Sum_Assured\":\"" + Death_Sum_Assured + "\""
                         + ",\"Cause_of_death_reported\":\"" + Cause_of_death_reported + "\"";
                // svcFalcoMail.ServiceClient FalcoobjFalconideMailClient = new svcFalcoMail.ServiceClient();

                multiMailService.ServiceClient objFalconideMailClient = new multiMailService.ServiceClient();

                    var result_Service = objFalconideMailClient.SendMail(mailfromname, ToSubject, FromEmailID, "", recipients, recipientsCC, "", content, filename, filecontent, attr, apiheader, "", FalcoTempId, otherAttributes);
                  string response = result_Service.ToString().Replace("\r\n", " ").Replace("\"", " ").Replace("{", "").Replace("}", "");
                string[] split = response.Split(new Char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                mailrespo = split[6];
                mailrespo = mailrespo.Replace("\n", " ");
                mailrespo.Trim();
                new sql().InsertCommunicationLog("Success", "SendSMS", "sendByFalco", "PolicyNo", Policy_Number, "ApplicationNo", Appno, "falco status ok");
                return mailrespo;
            }
            catch (Exception ex){
                string mailrespo = "error";
                string msg = ex.Message;
                new sql().InsertCommunicationLog("error", "SendSMS", "sendByFalco", "PolicyNo", Policy_Number, "ApplicationNo", Appno, msg);
                return mailrespo;
            }
        }
        public byte[] SendMailAndCreateEML_Attachment(string Appno,string Policy_Number, string Insured_Name, string Risk_Commencement_Date, string Date_of_Intimation, string Date_of_Loss, string Death_Sum_Assured, string Cause_of_death_reported,string token,string PolicyFilePathName,string FalcoTempName) {
            byte[] data = null;

            try
            {
                string ToSubject = ConfigurationSettings.AppSettings["ToSubject"].ToString();
                string FromEmailID = ConfigurationSettings.AppSettings["FromEmailID"].ToString();
                string ToEmailID = ConfigurationSettings.AppSettings["ToEmailID"].ToString();
                DataSet ds = getEmailCC();
                string EMLTempFile = ConfigurationSettings.AppSettings["EmlHTMLTemplatePath"].ToString();
                string EMLFilePath = ConfigurationSettings.AppSettings["EMLFilePath"].ToString();
                // string CCEmailID = ConfigurationSettings.AppSettings["CCEmailID"].ToString();
                string Port = ConfigurationSettings.AppSettings["SMTPPort"].ToString();
                string Host = ConfigurationSettings.AppSettings["SMTPHost"].ToString();
                string FileName = "EARLYCLAIMTEMPLATE.html";

                string Body1 = string.Empty;

                StreamReader reader = new StreamReader(EMLTempFile + FileName);
                StringBuilder body = new StringBuilder("");
                body.Append(reader.ReadToEnd());
                body = body.Replace("[Policy_Number]", Policy_Number);
                body = body.Replace("[Insured_Name]", Insured_Name);
                body = body.Replace("[Risk_Commencement_Date]", Risk_Commencement_Date);
                body = body.Replace("[Date_of_Intimation]", Date_of_Intimation);
                body = body.Replace("[Date_of_Loss]", Date_of_Loss);
                body = body.Replace("[Death_Sum_Assured]", Death_Sum_Assured);
                body = body.Replace("[Cause_of_death_reported]", Cause_of_death_reported);
                Body1 = body.ToString();
                if (ToEmailID != "")
                {
                    MailMessage ObjMailmsg = new MailMessage();
                    MailAddress madFrom = new MailAddress(FromEmailID);
                    //string[] CCId = CCEmailID.Split(',');
                    //foreach (string CCEmail in CCId)
                    //{
                    //    ObjMailmsg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id  
                    //}
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string CCEmail = ds.Tables[0].Rows[i]["Email"].ToString();
                        ObjMailmsg.CC.Add(new MailAddress(CCEmail));
                    }

                    ObjMailmsg.From = madFrom;
                    ObjMailmsg.Subject = ToSubject;
                    ObjMailmsg.Body = Body1.ToString();
                    ObjMailmsg.IsBodyHtml = true;
                    ObjMailmsg.To.Add(ToEmailID);

                    SmtpClient smtp = new SmtpClient();
                    smtp.Port = Convert.ToInt32(Port);
                    smtp.Host = Host;
                    smtp.UseDefaultCredentials = true;

                    if (File.Exists(Path.Combine(PolicyFilePathName)))
                    {
                        Attachment attachment = new Attachment(PolicyFilePathName);
                        ObjMailmsg.Attachments.Add(attachment);
                    }
                    string timstame = DateTime.Now.ToString("ddMMyyyyhhmmss");
                    var id = FalcoTempName + "_" + timstame + "_" + Policy_Number;
                    var tempFolder = Path.GetFullPath(EMLFilePath);
                    tempFolder = Path.Combine(tempFolder, "MailMessageToEMLTemp");
                    tempFolder = Path.Combine(tempFolder, id.ToString());
                    if (!Directory.Exists(tempFolder))
                    {
                        Directory.CreateDirectory(tempFolder);
                    }
                    smtp.UseDefaultCredentials = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtp.PickupDirectoryLocation = tempFolder;
                    smtp.Send(ObjMailmsg);
                    var filePath = Directory.GetFiles(tempFolder).Single();
                    FileInfo fInfo = new FileInfo(filePath);
                    long numBytes = fInfo.Length;
                    var fStream = new FileStream(filePath, FileMode.Open);
                    BinaryReader br = new BinaryReader(fStream);

                    data = br.ReadBytes((int)numBytes);
                    string BlobFileName = Policy_Number + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".eml";
                    //helper ss = new helper();
                    //SaveEmlBlob(token, Policy_Number, DateTime.Now.ToString("yyyy"), Convert.ToBase64String(data), BlobFileName, "comm", id);

                    ObjMailmsg.Attachments.Clear();
                    br.Close();
                    fStream.Dispose();
                    reader.Close();
                    ObjMailmsg.Dispose();
                    smtp.Dispose();
                    ObjMailmsg.Dispose();
                    smtp.Dispose();
                    new sql().InsertCommunicationLog("Success", "Email save", "SendMailAndCreateEML_Attachment", "PolicyNo", Policy_Number, "ApplicationNo", Appno, "Email attchment save");
                    return data;
                }
                
            }
            catch (Exception ex){
              
                string msg = ex.Message;
                new sql().InsertCommunicationLog("error", "Email save error", "SendMailAndCreateEML_Attachment", "PolicyNo", Policy_Number, "ApplicationNo", Appno, msg);
              
            }

            return data;
        }
        public string SaveEmlBlob(string token, string PolicyNum, string containname, string filByte, string BlobFileName,
          string subFldr, string ExistingFileName)
        {
            string BlobPutAPI = ConfigurationSettings.AppSettings["PutBlobStorage"].ToString().Trim();
            if (token != "")
            {
                RequestPutBlobStorage putBlobStorage = new RequestPutBlobStorage();
                putBlobStorage.applicationNumber = PolicyNum.ToLower();
                putBlobStorage.containName = containname;
                putBlobStorage.subFolder = subFldr;
                putBlobStorage.fileName = BlobFileName;
                putBlobStorage.fileBytes = filByte;
                putBlobStorage.token = token;

                var json2 = SimpleJson.SerializeObject(putBlobStorage);
                var webResponse2 = GenerateWebRequest(BlobPutAPI, json2);
                var vOthr = new JavaScriptSerializer().Deserialize<ResponsePutBlobStorage>(webResponse2);
                if (vOthr.errorCode == "0")
                {

                    //new BussLayer().InsertMerVidDetails(applno, ExistingFileName, BlobFileName, vOthr.errorCode, userId);
                    //new BussLayer().InsertMerVidLogs("Mer Video Saved in Blob", "CommnFun", "SaveMerVideoBlob", "Applno", applno, "Video Blob file save");

                }
                else
                {
                    //new BussLayer().InsertMerVidDetails(applno, ExistingFileName, BlobFileName, vOthr.errorCode, userId);
                    //new BussLayer().InsertMerVidLogs("Exception", "CommnFun", "SaveMerVideoBlob", "TokenErrorCode", string.IsNullOrEmpty(vOthr.errorCode).ToString(), "Exception: " + vOthr.errorDesc);

                }
                return vOthr.errorCode;
            }
            else
            {
                return "";
            }
        }
        public DataSet getEmailCC()
        {
            int ii = 0;
            DataSet dt = new DataSet();
            sqlClass.featchSendEmailCC("Sp_FeatchT_CLAIMS_Active_EmailCC", ref dt);
            if (dt != null)
            {
                if (dt.Tables.Count > 0)
                {
                    ii = dt.Tables[0].Rows.Count;
                }
            }
            return dt;

        }
        //Generate Pdf Attachment File
        public byte[] pdfcon(string s) {
            HtmlToPdfConverter nRecohtmltoPdfObj = new HtmlToPdfConverter();
            nRecohtmltoPdfObj.Orientation = PageOrientation.Portrait;

            using (MemoryStream output = new MemoryStream())
            {
                nRecohtmltoPdfObj.GeneratePdf(s, null, output);
                GC.Collect();
                return output.ToArray();
            }
        }


        //Generate Email Body 
        //public string EmailTemplete(string Policy_Number, string Insured_Name, string Risk_Commencement_Date, string Date_of_Intimation, string Date_of_Loss, string Death_Sum_Assured, string Cause_of_death_reported)
        //{
        //    StringBuilder htmlContent = new StringBuilder();
        //    htmlContent.Append("<!DOCTYPE html>");
        //    htmlContent.Append("<html>");
        //    htmlContent.Append("<head>");
        //    htmlContent.Append("<title> </title>");
        //    htmlContent.Append("</head>");
        //    htmlContent.Append("<body>");
        //    htmlContent.Append("<div>");
        //    htmlContent.Append("<p>");
        //    htmlContent.Append("Dear Sir, </p>");
        //    htmlContent.Append("<p>");
        //    htmlContent.Append("We are awaiting the response on early claim alert sent to you with claim details.");
        //    htmlContent.Append("We had sent  the details of early claim case for highlighting to the concerned sourcing team about a pre-mature claim.");
        //    htmlContent.Append("<br>");
        //    htmlContent.Append("A response is expected detailing out the corrective actions that shall be taken with respect to the said claim and also the actions that will be taken to ensure that such adverse life selection is prevented.");
        //    htmlContent.Append("</p><p>");
        //    htmlContent.Append("<table style=\"border: 1.5px solid black; border-collapse:collapse;\">");
        //    htmlContent.Append("<tbody>");
        //    htmlContent.Append("<tr>");
        //    htmlContent.Append("<th style=\"border: 1.5px solid black; border-collapse: collapse;\">Policy Number &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</th>");
        //    htmlContent.Append("<th style=\"border: 1.5px solid black; border-collapse: collapse;\">" + Policy_Number + " &nbsp &nbsp&nbsp</th></tr>");
        //    htmlContent.Append("<tr>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Insured Name </td>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> " + Insured_Name + " </td></tr>");
        //    htmlContent.Append("<tr><td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Risk Commencement Date </td>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> " + Risk_Commencement_Date + "</td></tr>");
        //    htmlContent.Append("<tr><td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Date of Intimation </td>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\">" + Date_of_Intimation+" </td></tr>");
        //    htmlContent.Append("<tr><td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Date of Loss </td >");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> " + Date_of_Loss + " </td ></tr>");
        //    htmlContent.Append("<tr><td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Death Sum Assured </td>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> " + Death_Sum_Assured + " </td></tr>");
        //    htmlContent.Append("<tr><td style=\"border: 1.5px solid black; border-collapse: collapse;\"> Cause of death reported </td>");
        //    htmlContent.Append("<td style=\"border: 1.5px solid black; border-collapse: collapse;\"> " + Cause_of_death_reported+" </td></tr>");
        //    htmlContent.Append("</tbody>");
        //    htmlContent.Append("</table>");
        //    htmlContent.Append("</p>");
        //    htmlContent.Append("<p>");
        //    htmlContent.Append("Regards,<br>");
        //    htmlContent.Append("Claims Team");
        //    htmlContent.Append("</p>");
        //    htmlContent.Append("</div>");
        //    htmlContent.Append("</body>");
        //    htmlContent.Append("</html>");
         
        //    return htmlContent.ToString(); 
        //}
        
        //Send Email to Claim team
        //public void SmtpSendEmail(string body, string PolicyFilePathName) {            
        //    string FromEmailID = ConfigurationSettings.AppSettings["FromEmailID"].ToString();
        //    string ToEmailID = ConfigurationSettings.AppSettings["ToEmailID"].ToString();
        //    DataSet ds = getEmailCC();
        //   // string CCEmailID = ConfigurationSettings.AppSettings["CCEmailID"].ToString();
        //    string Port = ConfigurationSettings.AppSettings["SMTPPort"].ToString();
        //    string Host = ConfigurationSettings.AppSettings["SMTPHost"].ToString();
        //    if (ds.Tables[0].Rows.Count != 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        if (ToEmailID != "")
        //        {
        //            MailMessage ObjMailmsg = new MailMessage();
        //            MailAddress madFrom = new MailAddress(FromEmailID);
        //            //string[] CCId = CCEmailID.Split(',');
        //            //foreach (string CCEmail in CCId)
        //            //{
        //            //    ObjMailmsg.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id  
        //            //}
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            { 
        //                string CCEmail = ds.Tables[0].Rows[i]["Email"].ToString();
        //                ObjMailmsg.CC.Add(new MailAddress(CCEmail));
        //            }

        //                ObjMailmsg.From = madFrom;
        //            ObjMailmsg.Subject = "Please Ignore Test Mail For Early Claim Scheduler";
        //            ObjMailmsg.Body = body.ToString();
        //            ObjMailmsg.IsBodyHtml = true;
        //            ObjMailmsg.To.Add(ToEmailID);

        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Port = Convert.ToInt32(Port);
        //            smtp.Host = Host;
        //            smtp.UseDefaultCredentials = true;

        //            if (File.Exists(Path.Combine(PolicyFilePathName)))
        //            {
        //                Attachment attachment = new Attachment(PolicyFilePathName);
        //                ObjMailmsg.Attachments.Add(attachment);
        //            }
        //            try
        //            {
        //                //smtp.Send(ObjMailmsg);
        //                ObjMailmsg.Attachments.Clear();
        //            }
        //            catch (Exception ex)
        //            {
        //                string ff = ex.Message;
        //            }
        //            ObjMailmsg.Dispose();
        //            smtp.Dispose();
        //        }
        //        else
        //        {
        //        }
        //    }
        //}
    }
}

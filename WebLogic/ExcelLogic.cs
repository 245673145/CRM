using CRM.Web.DataAccess;
using CRM.Web.DataAccess.Excel;
using CRM.Web.Models;
using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRM.Web.Logic
{
    public class ExcelLogic
    {
        private ExcelContext econtext { get; set; }
        private CustomerContext ccontext { get; set; }
        public ExcelLogic()
        {
            econtext = new ExcelContext();
            ccontext = new CustomerContext();
        }
        public Message<MemoryStream> ImportLogic(HttpPostedFileBase filebase)
        {
            if (filebase.ContentType != "application/vnd.ms-excel" || !filebase.FileName.EndsWith(".xls"))
                return new Message<MemoryStream> { IsSuccess = false, Messages = "文件类型错误，只支持xls类型文件",Data=null };
            if (filebase.ContentLength / 1024 / 1024 > 20)
                return new Message<MemoryStream> { IsSuccess = false, Messages = "文件太大，最多支持20MB文件",Data=null };
            Stream stream = filebase.InputStream;
            if (!econtext.Open(stream, 1, 14))
                return new Message<MemoryStream> { IsSuccess = false, Messages = "读取文件失败",Data=null };
            string[] rows;
            int successcount = 0;
            int totalcount = 0;
            List<string[]> errrows = new List<string[]>();
            List<Customer> datacus = new List<Customer>();
            while ((rows = econtext.Next()) != null)
            {
                Customer customer = new Customer();
                customer.EnterpriseName = rows[0];
                customer.Province = rows[1];
                customer.City = rows[2];
                customer.CreditCode = rows[3];
                customer.Representative = rows[4];
                customer.EnterpriseType = rows[5];
                customer.CreateTime = Convert.ToDateTime(rows[6]).Date;
                customer.Capital = rows[7];
                customer.Address = rows[8];
                customer.Email = rows[9];
                customer.ScopeOperation = rows[10];
                customer.Website = rows[11];
                customer.TelNumber = rows[12];
                customer.MoreTelNumber = rows[13];
                if (string.IsNullOrEmpty(customer.EnterpriseName))
                {
                    rows[14] = "企业名称为空";
                    errrows.Add(rows);
                    continue;
                }
                if (string.IsNullOrEmpty(customer.CreditCode))
                {
                    rows[14] = "信用编码为空";
                    errrows.Add(rows);
                    continue;
                }
                datacus.Add(customer);
                //合并重复
                //添加数据             
                totalcount++;
            }
            successcount += ccontext.AddCustomer(datacus);
            ImportRecord record = new ImportRecord();

            //保存文件到服务器;
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") +"_"+ DateTime.Now.Ticks + "_" + successcount + "_" + totalcount + ".xls";
            record.FileName = filename;
            record.SuccessCount = successcount;
            record.TotalCount = totalcount;
            ccontext.AddReCord(record);
            //错误数据返回
           byte [] data=  File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/FileModel/" + "excelmodel.xls");           
            MemoryStream ms= econtext.WriteSheet(new MemoryStream(data),"错误数据", 1,0, errrows);
            filebase.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "/ExcelFiles/" + filename);
            stream.Close();
            stream.Flush();
            if(errrows.Count==0)
                return new Message<MemoryStream> { IsSuccess = true, Data = null, Messages = "数据导入成功" };
            return new Message<MemoryStream> { IsSuccess = true, Data = ms, Messages = "数据导入成功" };
            


        }
    }
}

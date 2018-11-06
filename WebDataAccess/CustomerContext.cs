using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.DataAccess
{
  public  class CustomerContext:DbContext
    {
        public CustomerContext():base()
        {

        }
        public int AddCustomer(Customer cus)
        {
            string sql = @"insert into Customer(EnterpriseName,Province,City,CreditCode,Representative,EnterpriseType,CreateTime,Capital,Address,
Email,ScopeOperation,Website,TelNumber,MoreTelNumber) values(@enterprise,@pr,@ciry,@cread,@rep,@en,@c,@ca,@add,@email,@scope,@web,@tel,@more)";
            return this.Save(sql, cus.EnterpriseName, cus.Province, cus.City, cus.CreditCode, cus.Representative, cus.EnterpriseType, cus.CreateTime,
                cus.Capital, cus.Address, cus.Email, cus.ScopeOperation, cus.Website, cus.TelNumber, cus.MoreTelNumber);
        }
        public int AddCustomer(List<Customer> customer)
        {
            string sql = @"insert into Customer(EnterpriseName,Province,City,CreditCode,Representative,EnterpriseType,CreateTime,Capital,Address,
Email,ScopeOperation,Website,TelNumber,MoreTelNumber) values {0} ";
            string values = "";
            foreach (Customer item in customer)
            {
                values += $@"('{item.EnterpriseName}','{item.Province}','{item.City}','{item.CreditCode}','{item.Representative}',
'{item.EnterpriseType}','{item.CreateTime}','{item.Capital}','{item.Address}','{item.Email}','{item.ScopeOperation}','{item.Website}','{item.TelNumber}','{item.MoreTelNumber}'),";
            }
            values = values.TrimEnd(',');
            sql = string.Format(sql, values);
            return Save(sql,false);
        }
        public int AddReCord(ImportRecord record)
        {
            string sql = "insert into ImportRecord(FileName,SuccessCount,TotalCount) values (@file,@succes,@total)";
            return Save(sql, record.FileName, record.SuccessCount, record.TotalCount);
        }
    }
}

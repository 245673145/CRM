using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
    public class Customer
    {
      	/// <summary>
		/// 编号
        /// </summary>
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName
        {
            get;
            set;
        }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province
        {
            get;
            set;
        }
        /// <summary>
        /// 城市
        /// </summary>
        public string City
        {
            get;
            set;
        }
        /// <summary>
        /// 信用编码
        /// </summary>
        public string CreditCode
        {
            get;
            set;
        }
        /// <summary>
        /// 法定代表
        /// </summary>
        public string Representative
        {
            get;
            set;
        }
        /// <summary>
        /// 企业类型
        /// </summary>
        public string EnterpriseType
        {
            get;
            set;
        }
        /// <summary>
        /// 成立日期
        /// </summary>
        public DateTime? CreateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 最后操作时间
        /// </summary>
        public DateTime? OptionTime
        {
            get;
            set;
        }
        /// <summary>
        /// 注册资本
        /// </summary>
        public string Capital
        {
            get;
            set;
        }
        /// <summary>
        /// 所在地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// 经营范围
        /// </summary>
        public string ScopeOperation
        {
            get;
            set;
        }
        /// <summary>
        /// 网址
        /// </summary>
        public string Website
        {
            get;
            set;
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 更多电话
        /// </summary>
        public string MoreTelNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 所属销售
        /// </summary>
        public int? SaleOwner
        {
            get;
            set;
        }
        /// <summary>
        /// 当前销售
        /// </summary>
        public int? SaleCurrent
        {
            get;
            set;
        }
        /// <summary>
        /// 最终销售
        /// </summary>
        public int? SaleDefine
        {
            get;
            set;
        }
        /// <summary>
        /// 所属实施
        /// </summary>
        public int? ImEngineerOwner
        {
            get;
            set;
        }
        /// <summary>
        /// 当前实施
        /// </summary>
        public int? ImEngineerDefine
        {
            get;
            set;
        }
        /// <summary>
        /// 最终实施
        /// </summary>
        public int? ImEngineerCurrent
        {
            get;
            set;
        }
        /// <summary>
        /// 是否转实施
        /// </summary>
        public bool IsImplementation
        {
            get;
            set;
        }

    }
}

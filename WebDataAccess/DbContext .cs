
using CRM.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CRM.Web.DataAccess
{
  public  abstract class DbContext
    {
        public string Connection { get; set; }


        public DbContext(string connection= "ConnectionStr")
        {
            this.Connection = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
        }

        private SqlParameter[] BuildParameters(string sql, params Object[] objects)
        {
            SqlParameter[] result = new SqlParameter[objects.Length];
            //所有的参数名称
            Regex regParam = new Regex("@\\w+");
            //找到所有的参数
            MatchCollection mc = regParam.Matches(sql);
            //遍历所有找到的参数名称，比如：@name，@password
            for (int i = 0; i < mc.Count; i++)
            {
                //创建一个参数。
                result[i] = new SqlParameter(mc[i].Value, objects[i] ?? DBNull.Value);
                result[i].IsNullable = true;
            }
            return result;
        }
        protected int Save(string sql, params Object[] values)
        {
            int result = -1;
            //这里没有异常处理
            using (SqlConnection con = new SqlConnection(Connection))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));

                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        protected int Save(string sql,bool parameters)
        {
            int result = -1;
            //这里没有异常处理
            using (SqlConnection con = new SqlConnection(Connection))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
               // cmd.Parameters.AddRange(BuildParameters(sql, values));

                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        protected int Save(SqlCommand cmd, string sql, params Object[] values)
        {
            int result = -1;
            //这里没有异常处理

            try
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));

                result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception Ex)
            {
                cmd.Transaction.Rollback();
                throw Ex;
            }
        }
        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected object FindOnly(string sql, params Object[] values)
        {
            Object result = null;
            //这里没有异常处理
            using (SqlConnection con = new SqlConnection(Connection))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));
                result = cmd.ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// 返回第一行第一列(手动事务)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected object FindOnly(SqlCommand Cmd, string sql, params Object[] values)
        {
            Object result = null;
            //这里没有异常处理

            try
            {
                Cmd.CommandText = sql;
                Cmd.Parameters.AddRange(BuildParameters(sql, values));
                result = Cmd.ExecuteScalar();
                return result;
            }
            catch (Exception Ex)
            {
                Cmd.Transaction.Rollback();
                throw Ex;
            }
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected List<Dictionary<string, Object>> Find(string sql, params Object[] values)
        {
            List<Dictionary<string, Object>> result = new List<Dictionary<string, object>>();
            using (SqlConnection con = new SqlConnection(Connection))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));
                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, Object> item = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object value = reader.GetValue(i);
                        item.Add(reader.GetName(i), value == DBNull.Value ? null : value);
                    }
                    result.Add(item);
                }
                reader.Close();
            }
            return result;
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected List<Dictionary<string, Object>> Find(SqlCommand Cmd, string sql, params Object[] values)
        {
            List<Dictionary<string, Object>> result = new List<Dictionary<string, object>>();

            try
            {
                Cmd.CommandText = sql;
                Cmd.Parameters.AddRange(BuildParameters(sql, values));
                DbDataReader reader = Cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, Object> item = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object value = reader.GetValue(i);
                        item.Add(reader.GetName(i), value == DBNull.Value ? null : value);
                    }
                    result.Add(item);
                }
                reader.Close();

                return result;
            }
            catch (Exception Ex)
            {
                //出现异常事务回滚
                Cmd.Transaction.Rollback();
                throw Ex;
            }

        }
        /// <summary>
        /// 查询泛型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected List<T> Find<T>(string sql, params Object[] values)
            where T : new()
        {
            List<T> result = new List<T>();
            using (SqlConnection con = new SqlConnection(Connection))
            {
                SqlCommand cmd = con.CreateCommand();

                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    cmd.Dispose();
                    return result;
                }

                Type type = typeof(T);
                while (reader.Read())
                {
                    T entity = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo pi = type.GetProperty(reader.GetName(i));
                        if (pi != null)
                        {
                            object value = reader.GetValue(i);
                            pi.SetValue(entity, value == DBNull.Value ? null : value, null);
                        }
                    }
                    result.Add(entity);
                }
                reader.Close();
            }
            return result;
        }
        protected List<T> Find<T>(SqlCommand cmd, string sql, params Object[] values)
          where T : new()
        {
            List<T> result = new List<T>();

            try
            {


                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    cmd.Dispose();
                    return result;
                }

                Type type = typeof(T);
                while (reader.Read())
                {
                    T entity = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo pi = type.GetProperty(reader.GetName(i));
                        if (pi != null)
                        {
                            object value = reader.GetValue(i);
                            pi.SetValue(entity, value == DBNull.Value ? null : value, null);
                        }
                    }
                    result.Add(entity);
                }
                reader.Close();

                return result;
            }
            catch (Exception Em)
            {
                throw Em;
            }

        }

        protected DataTable FindTable(string sql, params Object[] values)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Connection))
            {
                SqlCommand cmd = con.CreateCommand();

                cmd.CommandText = sql;
                cmd.Parameters.AddRange(BuildParameters(sql, values));
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }
        protected DataTable FindTable(SqlCommand Cmd, string sql, params Object[] values)
        {
            DataTable dt = new DataTable();

            try
            {

                Cmd.CommandText = sql;
                Cmd.Parameters.AddRange(BuildParameters(sql, values));

                SqlDataReader reader = Cmd.ExecuteReader();
                dt.Load(reader);

                return dt;

            }
            catch (Exception Ex)
            {
                Cmd.Transaction.Rollback();
                Cmd.Dispose();
                throw Ex;
            }
        }
        /// <summary>
        /// 根据SQL分页查询，并返回分页对象，分页对象只写一条。
        /// SQL格式：
        ///     select field_name from table_name where condition
        /// </summary>
        /// <typeparam name="T">被泛型的类型</typeparam>
        /// <param name="sql">查询的条件</param>
        /// <param name="index">当前页码</param>
        /// <param name="count">当前每页显示数据总数</param>
        /// <param name="field">排序字段</param>
        /// <param name="order"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected Pagination<T> Find<T>(string sql, int index, int count, string field, bool desc, params Object[] values)
            where T : new()
        {
            //让数据库自己去优化
            string findSql = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} {1}) as RowNumber  from ({2}) as idata) as odata where odata.RowNumber>@min and odata.RowNumber<@max", field, desc ? "desc" : "asc", sql);
            string summarySql = string.Format("select count(1) from ({0}) as odata", sql);
            //分页对象
            Pagination<T> result = new Pagination<T>();
            //页码和每页显示的数据总数
            result.PageIndex = index;
            result.PageCount = count;
            //添加自定义参数
            List<Object> lvalues = new List<object>(values);
            //添加最小值参数
            lvalues.Add(index * count - count);
            //添加最大值参数
            lvalues.Add(index * count + 1);
            //当前页数据
            result.Data = this.Find<T>(findSql, lvalues.ToArray());
            //当前数据库中的数据总数
            result.DataCount = Convert.ToInt32(this.FindOnly(summarySql, values));
            return result;
        }
        /// <summary>
        /// 根据SQL分页查询，并返回分页对象，分页对象只写一条。
        /// SQL格式：
        ///     select field_name from table_name where condition
        /// 默认为倒序
        /// </summary>
        /// <typeparam name="T">被泛型的类型</typeparam>
        /// <param name="sql">查询的条件</param>
        /// <param name="index">当前页码</param>
        /// <param name="count">当前每页显示数据总数</param>
        /// <param name="field">排序字段</param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected Pagination<T> Find<T>(string sql, int index, int count, string field, params Object[] values)
            where T : new()
        {
            //让数据库自己去优化
            string findSql = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} {1}) as RowNumber  from ({2}) as idata) as odata where odata.RowNumber>@min and odata.RowNumber<@max", field, "desc", sql);
            string summarySql = string.Format("select count(1) from ({0}) as odata", sql);
            //分页对象
            Pagination<T> result = new Pagination<T>();
            //页码和每页显示的数据总数
            result.PageIndex = index;
            result.PageCount = count;
            //添加自定义参数
            List<Object> lvalues = new List<object>(values);
            //添加最小值参数
            lvalues.Add(index * count - count);
            //添加最大值参数
            lvalues.Add(index * count + 1);
            //当前页数据
            result.Data = this.Find<T>(findSql, this.BuildParameters(findSql, lvalues.ToArray()));
            //当前数据库中的数据总数
            result.DataCount = Convert.ToInt32(this.FindOnly(summarySql, values));
            return result;
        }
        /// <summary>
        /// 根据SQL分页查询，并返回分页对象，分页对象只写一条。
        /// SQL格式：
        ///     select field_name from table_name where condition
        /// 默认为ID倒序
        /// </summary>
        /// <typeparam name="T">被泛型的类型</typeparam>
        /// <param name="sql">查询的条件</param>
        /// <param name="index">当前页码</param>
        /// <param name="count">当前每页显示数据总数</param>
        /// <param name="field">排序字段</param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected Pagination<T> Find<T>(string sql, int index, int count, params Object[] values)
            where T : new()
        {
            //让数据库自己去优化
            string findSql = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} {1}) as RowNumber  from ({2}) as idata) as odata where odata.RowNumber>@min and odata.RowNumber<@max", "ID", "desc", sql);
            string summarySql = string.Format("select count(1) from ({0}) as odata", sql);
            //分页对象
            Pagination<T> result = new Pagination<T>();
            //页码和每页显示的数据总数
            result.PageIndex = index;
            result.PageCount = count;
            //添加自定义参数
            List<Object> lvalues = new List<object>(values);
            //添加最小值参数
            lvalues.Add(index * count - count);
            //添加最大值参数
            lvalues.Add(index * count + 1);
            //当前页数据
            result.Data = this.Find<T>(findSql, lvalues.ToArray());
            //当前数据库中的数据总数
            result.DataCount = Convert.ToInt32(this.FindOnly(summarySql, values));
            return result;
        }
        /// <summary>
        /// 根据SQL分页查询，并返回分页对象，分页对象只写一条。
        /// SQL格式：
        ///     select field_name from table_name where condition
        /// </summary>
        /// <typeparam name="T">被泛型的类型</typeparam>
        /// <param name="sql">查询的条件</param>
        /// <param name="index">当前页码</param>
        /// <param name="count">当前每页显示数据总数</param>
        /// <param name="field">排序字段</param>
        /// <param name="desc">排序方式</param>
        /// <param name="values">参数的值</param>
        /// <returns></returns>
        protected Pagination<Dictionary<string, Object>> Find(string sql, int index, int count, string field, bool desc, params Object[] values)
        {
            //让数据库自己去优化
            string findSql = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} {1}) as RowNumber  from ({2}) as idata) as odata where odata.RowNumber>@min and odata.RowNumber<@max", field, desc ? "desc" : "asc", sql);
            string summarySql = string.Format("select count(1) from ({0}) as odata", sql);
            //分页对象
            Pagination<Dictionary<string, Object>> result = new Pagination<Dictionary<string, Object>>();
            //页码和每页显示的数据总数
            result.PageIndex = index;
            result.PageCount = count;
            //添加自定义参数
            List<Object> lvalues = new List<object>(values);
            //添加最小值参数
            lvalues.Add(index * count - count);
            //添加最大值参数
            lvalues.Add(index * count + 1);
            //当前页数据
            result.Data = this.Find(findSql, lvalues.ToArray());
            //当前数据库中的数据总数
            result.DataCount = Convert.ToInt32(this.FindOnly(summarySql, values));
            return result;
        }
        //执行事务


    }
}

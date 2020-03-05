using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Retail.DAL.Repository
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class BaseDbContext
    {

        public SqlSugarClient Db;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDbContext(string connectionString)
        {
            try
            {
                InitDataBase(new List<string>() { connectionString });
            }
            catch (Exception ex)
            {
                //DALHandler.WriteErrorLog("未配置数据库连接字符串", ex);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="listConnSettings">
        /// 连接字符串配置Key集合,配置多个连接则是读写分离 
        /// </param>
        public BaseDbContext(List<string> listConnSettings)
        {
            try
            {
                var listConn = new List<string>();
                foreach (var t in listConnSettings)
                {
                    listConn.Add(ConfigurationManager.ConnectionStrings[t].ToString());
                }
                InitDataBase(listConn);
            }
            catch (Exception ex)
            {
                //DALHandler.WriteErrorLog("未配置数据库连接字符串", ex);
            }

        }
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        /// <param name="listConn">连接字符串</param>
        private void InitDataBase(List<string> listConn)
        {
            var connStr = "";//主库
            var slaveConnectionConfigs = new List<SlaveConnectionConfig>();//从库集合
            for (var i = 0; i < listConn.Count; i++)
            {
                if (i == 0)
                {
                    connStr = listConn[i];//主数据库连接
                }
                else
                {
                    slaveConnectionConfigs.Add(new SlaveConnectionConfig()
                    {
                        HitRate = i * 2,
                        ConnectionString = listConn[i]
                    });
                }
            }
            //如果配置了 SlaveConnectionConfigs那就是主从模式,所有的写入删除更新都走主库，查询走从库，
            //事务内都走主库，HitRate表示权重 值越大执行的次数越高，如果想停掉哪个连接可以把HitRate设为0
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,//可以自动关闭连接，不需要close和using
                IsShardSameThread = false, //设为true相同线程是同一个SqlConnection
                                           //SlaveConnectionConfigs = slaveConnectionConfigs
            });

            Db.Ado.CommandTimeOut = 200;//设置超时时间  //由30000改为200，查询资料发现单位是秒 modify by jjx 20190430
            Db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
//#if DEBUG
//                DALHandler.WriteLog($"{sql}{Environment.NewLine} {GetString(pars)}{Environment.NewLine}[耗时 {Db.Ado.SqlExecutionTime.TotalSeconds}s ]");
//#endif
//                //sql执行超过100ms  则记录
//                //单位是秒  sql执行超过1s则记录 modify by jjx 20190430
//                if (Db.Ado.SqlExecutionTime.TotalSeconds > 1)
//                {
//                    DALHandler.WriteLog($"{sql}{Environment.NewLine} {GetString(pars)}{Environment.NewLine}[耗时 {Db.Ado.SqlExecutionTime.TotalSeconds}s ]");
//                }
//                if (!DALHandler.DbConnectState)
//                {
//                    DALHandler.DbConnectStateChange(true);
//                }
            };
            Db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
            {
                //var p = sql + "\r\n" + Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
            };
            Db.Aop.OnError = (ex) =>//执行SQL 错误事件
            {
                //if (Db.Ado.Connection.State == ConnectionState.Closed && DALHandler.DbConnectState)
                //{
                //    DALHandler.DbConnectStateChange(false);
                //}
                //var pars = (ex.Parametres as SugarParameter[]) ?? new SugarParameter[] { };
                //DALHandler.WriteErrorLog($"执行sql出错:{ex.Sql}{Environment.NewLine}{GetString(pars)}", ex);
            };
            Db.Aop.OnDiffLogEvent = it =>
            {
                var editBeforeData = it.BeforeData;
                var editAfterData = it.AfterData;
                var sql = it.Sql;
                var parameter = it.Parameters;
                var data = it.BusinessData;
                var time = it.Time;
                var diffType = it.DiffType;//枚举值 insert 、update 和 delete 用来作业务区分
            };
            Db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL执行前 可以修改SQL
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };
        }

        private string GetString(SugarParameter[] pars)
        {
            StringBuilder sbr = new StringBuilder();
            foreach (var i in pars)
            {
                sbr.Append($"{i.ParameterName}:{i.Value.ObjToString()},");
            }
            return sbr.ToString();
        }

    }
}

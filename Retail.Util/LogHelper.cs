using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Retail.Util
{

    /// <summary>
    /// 日志操作类
    /// </summary>
    public class LogHelper
    {

        /// <summary>
        /// 取得当前源码的哪一行
        /// </summary>
        /// <returns></returns>
        public static int GetLineNum()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileLineNumber();
        }

        /// <summary>
        /// 取当前源码的源文件名
        /// </summary>
        /// <returns></returns>
        public static string GetCurSourceFileName()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);

            return st.GetFrame(0).GetFileName();

        }

        /// <summary>
        /// 获取当前方法名
        /// </summary>
        /// <returns></returns>
        public static string GetCurMethodName()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);

            return st.GetFrame(0).GetMethod().Name;

        }

        /// <summary>
        /// 日志格式
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="moduleName"></param>
        /// <param name="stackInfo"></param>
        /// <param name="keyWord"></param>
        /// <param name="data"></param>
        public static void WriteLogFormat(LogType logType, string moduleName, string stackInfo, string keyWord, string data)
        {
            try
            {
                //***功能模块***方法+行数***关键参数(主要是订单号),data:数据
                WriteLog(logType, $"***{moduleName}***{stackInfo}***{keyWord},data:{data}");
            }
            catch
            {


            }

        }

        /// <summary>
        /// Info:记录一般信息
        /// </summary>
        /// <param name="type">LogType要写入的日志文件类型</param>
        /// <param name="msg">信息</param>
        public static void WriteLog(LogType type, string msg)
        {
            try
            {
                var logCur = GetLog(type);
                if (logCur != null && logCur.IsInfoEnabled)
                {
                    logCur.Info(msg);
                }
            }
            catch (Exception)
            {
                //这里不处理
            }

        }

        /// <summary>
        /// Error:记录错误日志并上传
        /// </summary>
        /// <param name="type">LogType要写入的日志文件类型</param>
        /// <param name="ex">异常信息</param>
        /// <param name="dataSourceInfo">数据源信息</param>
        public static void WriteErrorLog(LogType type, Exception ex, object dataSourceInfo = null)
        {
            var methodName = new StackTrace().GetFrame(1).GetMethod();
            //var model = new box_system_exceptioninfo();
            //model.ErrorCode = methodName.ReflectedType.FullName + "." + methodName.Name + "[" + ex.GetType() + "]";//唯一标识码
            //model.ErrorInfo = "\r\n[堆栈信息]：" + ex.StackTrace + "\r\n[错误信息]：" + ex.Message;
            //model.DataSourceInfo = dataSourceInfo?.ToJsonStr();
            //model.MachineId = string.IsNullOrEmpty(AppSetting.machineId) ? "" : AppSetting.machineId;
            //model.ShopId = string.IsNullOrEmpty(AppSetting.shopperId) ? "" : AppSetting.shopperId;
            //model.UploadState = 0;
            WriteErrorLog(type, "\r\n[堆栈信息]：" + ex.StackTrace + "\r\n[错误信息]：" + ex.Message);
            //try
            //{
            //    var dal = new box_system_exceptioninfo();
            //    dal.Add(model);
            //}
            //catch (Exception)
            //{
            //    //异常存数据库错误 跳过
            //}
        }

        /// <summary>
        /// 本地异常数据上传
        /// </summary>
        public static void UploadExceptionData()
        {
            //Thread t = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            //查询本地所有异常信息
            //            APIManage.ExceptionAPI api = new APIManage.ExceptionAPI();
            //            var dal = new Box_System_ExceptionInfoDAL();
            //            var list = dal.GetList("");
            //            if (list.Any())
            //            {
            //                foreach (var model in list)
            //                {
            //                    try
            //                    {
            //                        api.UploadLocalException(model);
            //                    }
            //                    catch (Exception)
            //                    {
            //                    }
            //                }
            //                //上传后删除
            //                dal.DeleteList(string.Join(",", list.Select(c => c.Id)));
            //            }
            //        }
            //        catch
            //        {
            //        }
            //        Thread.Sleep(60 * 60 * 1000);//1小时轮询一次
            //    }
            //});
            //t.IsBackground = true;
            //t.Start();
        }

        /// <summary>
        /// 本地日志清理
        /// </summary>
        public static void ClearLogs()
        {

        }

        /// <summary>
        ///  Error:记录错误日志
        /// </summary>
        /// <param name="type">LogType要写入的日志文件类型</param>
        /// <param name="msg">异常信息</param>
        /// <param name="ex">异常对象</param>
        private static void WriteErrorLog(LogType type, string msg, Exception ex = null)
        {
            try
            {
                var logCur = GetLog(type);
                if (logCur != null && logCur.IsErrorEnabled)
                {
                    if (ex == null)
                    {
                        logCur.Error(msg);
                    }
                    else
                    {
                        logCur.Error(msg, ex);
                    }
                }
            }
            catch (Exception)
            {
                //这里不处理
            }

        }

        /// <summary>
        /// 加载对应日志类型配置
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <returns></returns>
        private static ILog GetLog(LogType type)
        {
            ////这里要清空配置 防止追加
            var repository = LogManager.GetRepository("RetailLog");
            repository.ResetConfiguration();
            var desc = GetModuleDescription(type);
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository("RetailLog");
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = "logs/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + desc + ".log";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 20;
            roller.MaximumFileSize = "10MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Date;
            roller.Encoding = Encoding.UTF8;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
            return LogManager.GetLogger("RetailLog", desc);

        }

        /// <summary>
        /// 获取模块描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetModuleDescription(object value)
        {
            var description = "";
            if (value != null)
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var attributes = fieldInfo.GetCustomAttributes(typeof(ModuleDescriptionAttribute), false);
                if (attributes.OfType<ModuleDescriptionAttribute>().Any())
                {
                    description = ((ModuleDescriptionAttribute)attributes[0]).Description;
                }
            }
            return description;
        }
    }

    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        ///  基础日志
        /// </summary>
        [ModuleDescription("Basic")]
        BASE,
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ModuleDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public ModuleDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        ///     模块描述
        /// </summary>
        public string Description { get; set; }
    }
}

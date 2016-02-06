using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
using System.Data.SQLite.Linq;
using System.Data.SQLite;
using System.Data.Linq;
using System.Data.Linq.Mapping;
 * */
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;

namespace MyControl.Helper
{
    /*
    public class LogDatabaseInfo
    {
        public const string Logger_Connection_Str = "Data Source=";
        public const string Logger_Table_Name = "TrackingLogger";

        public const string Logger_Column_ID = "id";
        public const string Logger_Column_StartTime = "start_time";
        public const string Logger_Column_IndexTime = "index_time";
        public const string Logger_Column_PrintIndex = "print_index";
        public const string Logger_Column_RAPTime = "rap_time";
        public const string Logger_Column_IsRAPFailed = "is_rap_failed";
        public const string Logger_Column_RAPContent = "rap_content";
        public const string Logger_Column_VerifyTime = "verify_time";
        public const string Logger_Column_IsVerifyFailed = "is_verify_failed";
        public const string Logger_Column_VerifyContent = "verify_content";
        public const string Logger_Column_BatchingTime = "batching_time";
        public const string Logger_Column_IsBatchingFailed = "is_batching_failed";

        public const string Logger_Table_CreateStr = "CREATE TABLE " + Logger_Table_Name
                    + "(" + Logger_Column_ID + " INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
                    + Logger_Column_StartTime + " DATETIME NOT NULL,"
                    + Logger_Column_IndexTime + " DATETIME NOT NULL,"
                    + Logger_Column_PrintIndex + " INTEGER NOT NULL,"
                    + Logger_Column_RAPTime + " DATETIME,"
                    + Logger_Column_IsRAPFailed + " INTEGER DEFAULT (0) NOT NULL,"
                    + Logger_Column_RAPContent + " VARCHAR (100),"
                    + Logger_Column_VerifyTime + " DATETIME,"
                    + Logger_Column_IsVerifyFailed + " INTEGER DEFAULT (0) NOT NULL,"
                    + Logger_Column_VerifyContent + " VARCHAR (100),"
                    + Logger_Column_BatchingTime + " DATETIME,"
                    + Logger_Column_IsBatchingFailed + " INTEGER DEFAULT (0) NOT NULL);";
    }

    public class SQLiteLinqHelper : DataContext
    {
        public SQLiteLinqHelper()
            : base(new SQLiteConnection(LogDatabaseInfo.Logger_Connection_Str + ResourceMap.TrackingPathHashtable[TrackingPath.LoggerFileFullPath]))
        {
            Log = Console.Out;
        }

        public Table<LogInfo> LogTable
        {
            get { return this.GetTable<LogInfo>(); }
        }

        public bool IsExistsLogInfo(LogInfo _LogInfo, ref bool IsExists)
        {
            try
            {
                int InfoCount = LogTable.Count<LogInfo>(info => info.PrintIndex == _LogInfo.PrintIndex && info.StartTime == _LogInfo.StartTime);
                IsExists = InfoCount == 0 ? false : true;

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        Func<LogInfo, LogSetting, bool> FuncLogDateTime = (_LogInfo, _LogParameter) =>
        {
            var MAXDateTimeArray = new DateTime[] { _LogInfo.IndexTime, _LogInfo.RAPTime == null ? DateTime.MinValue : _LogInfo.RAPTime.Value, _LogInfo.VerifyTime == null ? DateTime.MinValue : _LogInfo.VerifyTime.Value };
            var MINDateTimeArray = new DateTime[] { _LogInfo.IndexTime, _LogInfo.RAPTime == null ? DateTime.MaxValue : _LogInfo.RAPTime.Value, _LogInfo.VerifyTime == null ? DateTime.MaxValue : _LogInfo.VerifyTime.Value };
            var OnlyFailedResult = _LogParameter.IsOnlyFailed ? _LogInfo.IsRAPFailed || _LogInfo.IsVerifyFailed || _LogInfo.IsBatchingFailed : true;

            return MAXDateTimeArray.Max() >= _LogParameter.StartDateTime && MINDateTimeArray.Min() <= _LogParameter.EndDateTime && OnlyFailedResult;
        };

        public bool GetLogInfos(LogSetting _LogParameter, ref List<LogInfo> _LogInfoList)
        {
            try
            {
                _LogInfoList = LogTable.ToList().Where<LogInfo>(_LogInfo => FuncLogDateTime(_LogInfo, _LogParameter)).ToList();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool DeleteLogInfo(LogInfo _LogInfo)
        {
            try
            {
                var deleteInfo = LogTable.SingleOrDefault<LogInfo>(info => info.PrintIndex == _LogInfo.PrintIndex && info.StartTime == _LogInfo.StartTime);
                if (deleteInfo != null)
                {
                    LogTable.DeleteOnSubmit(deleteInfo);
                    SubmitChanges();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        //根据Page Index进行判断是否存在，存在则进行更新操作，否则，进行插入操作
        public bool InsertOrUpdateSingleLogInfo(LogInfo _LogInfo)
        {
            try
            {
                bool Result = false;

                bool IsExists = false;
                if (IsExistsLogInfo(_LogInfo, ref IsExists))
                {
                    if (IsExists)
                        Result = UpdateInfo(_LogInfo);
                    else
                        Result = InsertInfo(_LogInfo);
                }

                if (Result)
                    SubmitChanges();

                return Result;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        private bool InsertInfo(LogInfo _LogInfo)
        {
            try
            {
                LogTable.InsertOnSubmit(_LogInfo);

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        private bool UpdateInfo(LogInfo _LogInfo)
        {
            try
            {
                var updateInfo = this.LogTable.SingleOrDefault<LogInfo>(info => info.PrintIndex == _LogInfo.PrintIndex && info.StartTime == _LogInfo.StartTime);
                if (updateInfo == null)
                    return false;
                else
                {
                    updateInfo.RAPTime = string.IsNullOrEmpty(updateInfo.RAPContent) ? _LogInfo.RAPTime : updateInfo.RAPTime;
                    updateInfo.RAPContent = string.IsNullOrEmpty(updateInfo.RAPContent) ? _LogInfo.RAPContent : updateInfo.RAPContent;
                    updateInfo.IsRAPFailed = _LogInfo.IsRAPFailed;

                    updateInfo.VerifyTime = string.IsNullOrEmpty(updateInfo.VerifyContent) ? _LogInfo.VerifyTime : updateInfo.VerifyTime;
                    updateInfo.VerifyContent = string.IsNullOrEmpty(updateInfo.VerifyContent) ? _LogInfo.VerifyContent : updateInfo.VerifyContent;
                    updateInfo.IsVerifyFailed = _LogInfo.IsVerifyFailed;

                    updateInfo.IsBatchingFailed = _LogInfo.IsBatchingFailed;

                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }

    public class SQLiteCSharpHelper
    {
        SQLiteConnection SQLiteConn = null;
        string ConnectionString = null;

        public static readonly SQLiteCSharpHelper _SQLiteHelper = new SQLiteCSharpHelper();

        ~SQLiteCSharpHelper()
        {
            if (SQLiteConn != null)
            {
                SQLiteConn.Close();
                SQLiteConn.Dispose();
                SQLiteConn = null;
            }
        }

        public bool ConnectToSQLiteFile(string _ConnectionString)
        { 
            try
            {
                ConnectionString = _ConnectionString;
                SQLiteConnection conn = new SQLiteConnection(ConnectionString);
                conn.Open();    //若该文件不存在的话，Open()会自动创建该文件并打开连接
                conn.Close();
                SQLiteConn = conn;

                return true;
            }
            catch (Exception ex)
            { 
                return false;
            }
        }

        public bool IsExistsTable(string _TableName, ref bool _IsExists)
        {
            if (SQLiteConn == null || string.IsNullOrEmpty(_TableName))
                return false;

            try
            {
                SQLiteConn.Open();

                SQLiteCommand SQLiteCmd = SQLiteConn.CreateCommand();
                SQLiteCmd.Connection = SQLiteConn;
                SQLiteCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master where type='table' and name='" + _TableName + "';";
                if (0 == Convert.ToInt32(SQLiteCmd.ExecuteScalar()))
                    _IsExists = false;
                else
                    _IsExists = true;

                SQLiteConn.Close();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool CreateTable(string _CreateTableCommand)
        {
            if (SQLiteConn == null || string.IsNullOrEmpty(_CreateTableCommand))
                return false;

            try
            {
                SQLiteConn.Open();

                SQLiteCommand SQLiteCmd = SQLiteConn.CreateCommand();
                SQLiteCmd.Connection = SQLiteConn;
                SQLiteCmd.CommandText = _CreateTableCommand;
                SQLiteCmd.ExecuteNonQuery();
                SQLiteConn.Close();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        //舍弃
        private bool GetAllLogInfoByTimeSpan(DateTime _StartTime, DateTime _EndTime, ref List<LogInfo> _LogInfoList)
        {
            if (SQLiteConn == null)
                return false;

            try
            {
                SQLiteConn.Open();

                SQLiteCommand SQLiteCmd = SQLiteConn.CreateCommand();
                SQLiteCmd.CommandText = "Select * from TrackingLogger where max(index_time,rap_time,verify_time) >= @StartTime AND ( min(index_time,rap_time) <= @EndTime OR verify_time !=@Empty AND verify_time<=@EndTime )";
                SQLiteCmd.Parameters.Add(new SQLiteParameter("@StartTime", _StartTime));
                SQLiteCmd.Parameters.Add(new SQLiteParameter("@EndTime", _EndTime));
                SQLiteCmd.Parameters.Add(new SQLiteParameter("@Empty", null));
                SQLiteCmd.Parameters.Add(new SQLiteParameter("@EndTime", _EndTime));

                DataTable dt = new DataTable();
                SQLiteDataReader dr = SQLiteCmd.ExecuteReader();
                while (dr.Read())
                {
                    _LogInfoList.Add(new LogInfo { ID = dr.GetInt32(0), StartTime = dr.GetDateTime(1), IndexTime = dr.GetDateTime(2), PrintIndex = dr.GetInt32(3), RAPTime = dr.GetDateTime(4), IsRAPFailed = dr.GetInt32(5) == 0 ? false : true, RAPContent = dr.GetString(6), VerifyTime = dr.GetDateTime(7), IsVerifyFailed = dr.GetInt32(8) == 0 ? false : true, VerifyContent = dr.GetString(9), BatchingTime = dr.GetDateTime(10), IsBatchingFailed = dr.GetInt32(11) == 0 ? false : true });
                }

                dr.Close();
                SQLiteConn.Close();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool InsertOrUpdateMultiLogInfoByTransaction(IEnumerable<LogInfo> _LogInfoCollection)
        {
            if (SQLiteConn == null || _LogInfoCollection == null)
                return false;

            if (_LogInfoCollection.Count() == 0)
                return true;

            SQLiteConn.Open(); //1. 创建并打开DbConnection连接
            using (SQLiteTransaction tran = SQLiteConn.BeginTransaction()) //2. 开启DbTransaction
            {
                try
                {
                    SQLiteCommand smd = new SQLiteCommand();
                    smd.Connection = SQLiteConn;
                    smd.Transaction = tran;

                    foreach (LogInfo _LogInfo in _LogInfoCollection)
                    {
                        smd.CommandText = "INSERT OR REPLACE INTO TrackingLogger(id,start_time, index_time, print_index, rap_time, rap_content, is_rap_failed, verify_time,verify_content, is_verify_failed, batching_time, is_batching_failed) VALUES ((SELECT id FROM TrackingLogger WHERE start_time = strftime('%Y-%m-%d %H:%M:%S',@StartTime) AND print_index=@PrintIndex),strftime('%Y-%m-%d %H:%M:%S',@StartTime),strftime('%Y-%m-%d %H:%M:%f',@IndexTime), @PrintIndex, strftime('%Y-%m-%d %H:%M:%f',@RAPTime), @RAPContent, @IsRAPFailed, strftime('%Y-%m-%d %H:%M:%f',@VerifyTime),@VerifyContent, @IsVerifyFailed, strftime('%Y-%m-%d %H:%M:%f',@BatchingTime), @IsBatchingFailed)";
                        smd.Parameters.Add(new SQLiteParameter("@StartTime", _LogInfo.StartTime));
                        smd.Parameters.Add(new SQLiteParameter("@IndexTime", _LogInfo.IndexTime));
                        smd.Parameters.Add(new SQLiteParameter("@PrintIndex", _LogInfo.PrintIndex));
                        smd.Parameters.Add(new SQLiteParameter("@RAPTime", _LogInfo.RAPTime));
                        smd.Parameters.Add(new SQLiteParameter("@RAPContent", _LogInfo.RAPContent));
                        smd.Parameters.Add(new SQLiteParameter("@IsRAPFailed", _LogInfo.IsRAPFailed));
                        smd.Parameters.Add(new SQLiteParameter("@VerifyTime", _LogInfo.VerifyTime));
                        smd.Parameters.Add(new SQLiteParameter("@VerifyContent", _LogInfo.VerifyContent));
                        smd.Parameters.Add(new SQLiteParameter("@IsVerifyFailed", _LogInfo.IsVerifyFailed));
                        smd.Parameters.Add(new SQLiteParameter("@BatchingTime", _LogInfo.BatchingTime));
                        smd.Parameters.Add(new SQLiteParameter("@IsBatchingFailed", _LogInfo.IsBatchingFailed));

                        smd.ExecuteNonQuery();
                    }

                    tran.Commit();

                    return true;
                }
                catch (System.Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
                finally
                {
                    SQLiteConn.Close();
                }
            }
        }

        public bool Dispose()
        {
            try
            {
                if (SQLiteConn != null)
                {
                    SQLiteConn.Close();
                    SQLiteConn.Dispose();
                    SQLiteConn = null;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }*/
}

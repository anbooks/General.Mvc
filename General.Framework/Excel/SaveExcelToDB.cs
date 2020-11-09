using System;
using System.Data;

namespace General.Framework.Excel
{
    public static class SaveExcelToDB
    {
        #region 将DataTable中数据写入数据库中
        /// <summary>
        /// 将DataTable中数据写入数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string InsertDataToDB(DataTable dt, string TableName)
        {
            int ret = 0;
            if (dt == null || dt.Rows.Count == 0)
            {
                return "Excel无内容";
            }
            //数据库表名
            string tname = TableName;
            //获取要插入列的名字（为动态，由Excel列数决定）
            string colNames = "";
            //循环获取列名
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colNames += dt.Columns[i].ColumnName + ",";
            }
            //去除最后一位‘，’防止SQL语句错误
            colNames = colNames.TrimEnd(',');
            //定义SQL语句
            string cmd = "";
            //定义获取对应列的内容变量
            string colValues;
            //初始SQL语句
            string cmdmode = string.Format("insert into {0}({1}) values({{0}});", tname, colNames);
            //第一个循环，遍历每一行
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                colValues = "";
                //第二个循环，遍历第每一列
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //如果为空，就跳出循环
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        colValues += "NULL,";
                        continue;
                    }
                    //接下来可调试寻找规律，如有不解，欢迎留言
                    if (dt.Columns[j].DataType == typeof(string))
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) || dt.Columns[j].DataType == typeof(double))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        colValues += string.Format("cast('{0}' as datetime),", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j].ToString());
                    }
                    else
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                }
                cmd = string.Format(cmdmode, colValues.TrimEnd(','));
                try
                {
                    ret = 1; //执行SQL插入语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...
                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                }
            }
            return "Excel导入成功";

        }
        #endregion

        #region 写入基础数据，并删除其中的重复的项目
        /// <summary>
        /// 写入基础数据，并删除其中的重复的项目
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="KeyName">主键</param>
        /// <param name="icol">主键所在的列,起始为1</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string InsAndDelDataToDB(DataTable dt, string KeyName, int icol, string TableName)
        {
            //删除数据库中的重复项目
            string mKeyStr = "";
            string tableName = TableName;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                mKeyStr += "'" + dt.Rows[i][icol - 1] + "',";
            }
            mKeyStr = mKeyStr.Trim(',');
            string sqlStr = "Delete from " + tableName + " where " + KeyName + " in (" + mKeyStr + ")";
            //执行SQL删除语句（即cmd），获取结果（次方法有各自系统或框架决定）；

            //向数据库中写入新的数据
            string tname = tableName;
            string colNames = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colNames += dt.Columns[i].ColumnName + ",";
            }
            colNames = colNames.TrimEnd(',');
            //colNames = colNames + "CreateDate ";
            int ret = 0;
            string cmd = "";
            string colValues;
            string cmdmode = string.Format("insert into {0}({1}) values({{0}});", tname, colNames);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                colValues = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        colValues += "NULL,";
                        continue;
                    }
                    if (dt.Columns[j].DataType == typeof(string))
                    {
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) || dt.Columns[j].DataType == typeof(double))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        colValues += string.Format("cast('{0}' as datetime),", dt.Rows[i][j]);
                    }
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        colValues += string.Format("{0},", dt.Rows[i][j].ToString());
                    }
                    else
                        colValues += string.Format("'{0}',", dt.Rows[i][j]);
                }
                //colValues += "getdate()";  记录更新时间
                cmd = string.Format(cmdmode, colValues.TrimEnd(','));
                try
                {
                    ret = 1;//执行SQL插入语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...
                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                    //将信息写入到日志输出文件
                    //DllComm.TP_WriteAppLogFileEx(DllComm.g_AppLogFileName, strOuput);

                }
            }
            return "Excel导入成功";
        }
        #endregion

        #region 将数据库相同唯一键的信息替换成DataTable对应唯一键外的信息（一个唯一键，一个修改值）
        /// <summary>
        /// 将数据库相同唯一键的信息替换成DataTable对应唯一键外的信息（一个唯一键，一个修改值）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public static string UpdateDataToDB(DataTable dt, string TableName)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "Excel无内容";
            }
            string tname = TableName;
            string keyOnly = dt.Columns[0].ColumnName;
            string modifyItem = dt.Columns[1].ColumnName;
            string cmd = "";
            int ret = 0;
            string keyOnlyValue, modifyItemValue;
            string cmdmode = string.Format("update {0} set {1}={{0}} where {2}={{1}};", tname, modifyItem, keyOnly);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                keyOnlyValue = "";
                modifyItemValue = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].GetType() == typeof(DBNull))
                    {
                        keyOnlyValue = "NULL";
                        modifyItemValue = "NULL";
                        continue;
                    }
                    if (dt.Columns[j].DataType == typeof(string))
                    {
                        keyOnlyValue = string.Format("'{0}'", dt.Rows[i][0]);
                        modifyItemValue = string.Format("'{0}'", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(int) || dt.Columns[j].DataType == typeof(float) ||
                             dt.Columns[j].DataType == typeof(double))
                    {
                        keyOnlyValue = string.Format("{0}", dt.Rows[i][0]);
                        modifyItemValue = string.Format("{0}", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(DateTime))
                    {
                        keyOnlyValue = string.Format("cast('{0}' as datetime),", dt.Rows[i][0]);
                        modifyItemValue = string.Format("cast('{0}' as datetime),", dt.Rows[i][1]);
                    }
                    else if (dt.Columns[j].DataType == typeof(bool))
                    {
                        keyOnlyValue = string.Format("{0}", dt.Rows[i][j].ToString());
                        modifyItemValue = string.Format("{0}", dt.Rows[i][j].ToString());
                    }
                    else
                    {
                        keyOnlyValue = string.Format("'{0}'", dt.Rows[i][0]);
                        modifyItemValue = string.Format("'{0}'", dt.Rows[i][1]);
                    }

                }
                cmd = string.Format(cmdmode, modifyItemValue, keyOnlyValue);
                try
                {
                    ret = 1;//执行SQL修改语句（即cmd），获取结果（次方法有各自系统或框架决定）；
                    if (ret <= 0)
                    {
                        return "Excel导入失败，请检查匹配";
                    }
                }
                catch (Exception e)
                {
                    //写错误日志...
                    string strOuput = string.Format("向数据库中写数据失败,错误信息:{0},异常{1}\n", e.Message, e.InnerException);
                    return strOuput;
                }
            }
            return "Excel导入成功";

        }
        #endregion
    }
}

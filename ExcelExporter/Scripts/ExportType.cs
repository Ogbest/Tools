using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    /// <summary>
    /// 导出类型
    /// </summary>
    public class ExportType
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public static string SERVER = "SERVER";

        /// <summary>
        /// 客户端
        /// </summary>
        public static string CLIENT = "CLIENT";

        /// <summary>
        /// 后台
        /// </summary>
        public static string BACKSTAGE = "BACKSTAGE";

        public static string IsIgnoreFlage(string type)
        {
            switch (type)
            {
                case "SERVER":
                    return Flag.SERVER;
                case "CLIENT":
                    return Flag.CLIENT;
                case "BACKSTAGE":
                    return Flag.BACKSTAGE;
            }

            return Flag.NOTE;
        }
    }
}

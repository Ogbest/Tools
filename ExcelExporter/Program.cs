using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ET
{
    struct HeadInfo
    {
        public string FieldAttribute;
        public string FieldDesc;
        public string FieldName;
        public string FieldType;

        public HeadInfo(string cs, string desc, string name, string type)
        {
            this.FieldAttribute = cs;
            this.FieldDesc = desc;
            this.FieldName = name;
            this.FieldType = type;
        }
    }

    class Program
    {
        /// <summary>
        /// C#类模板
        /// </summary>
        private static string m_ClassTemplate_Class;
        private static string m_ClassTemplate_SingleClass;
        private static string m_ClassTemplate_SingleClassManager;

        /// <summary>
        /// 导出Json路径
        /// </summary>
        private static string m_JsonFolder;

        /// <summary>
        /// 配置表路径
        /// </summary>
        private static string m_ExcelFolder;

        /// <summary>
        /// 导出类型
        /// </summary>
        private static string m_ExportType;

        /// <summary>
        /// 导出C#类路径
        /// </summary>
        private static string m_ExportClassPath;

        /// <summary>
        /// 导出Proto二进制文件路径
        /// </summary>
        private static string m_ExportBytesPath;

        private static Dictionary<string, string> m_Args = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            try
            {
                LoadArgs(args);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (string path in Directory.GetFiles(m_ExcelFolder, "*.xlsx"))
                {
                    if (Path.GetFileName(path).StartsWith("~"))
                    {
                        continue;
                    }

                    using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using ExcelPackage p = new ExcelPackage(stream);
                    string name = Path.GetFileNameWithoutExtension(path);

                    Console.WriteLine($"Start Export Excel Name:{name} Path:{path}");

                    Exporter_Class.ExportExcelClass(p, name, m_ExportClassPath, m_ClassTemplate_Class, m_ClassTemplate_SingleClass, m_ClassTemplate_SingleClassManager);
                    Exporter_Json.ExportExcelJson(p, name, m_JsonFolder);
                }

                Console.WriteLine($"Start Export Excel Protobuf");

                Exporter_Protobuf.ExportExcelProtobuf(m_ExportClassPath, m_ExportBytesPath, m_JsonFolder, m_ClassTemplate_Class);

                Console.WriteLine("导表成功!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void LoadArgs(string[] args)
        {
            m_Args.Clear();


            if (args.Length <= 0)
            {
                m_Args.Add("EXPORT_TYPE", "SERVER");
                m_Args.Add("EXCEL_FOLDER", @".\Excel");
                m_Args.Add("CLASS_TEMPLATE_PATH", @".\Template");
                m_Args.Add("EXPORT_JSON_FOLDER", @".\ExcelJson");
                m_Args.Add("EXPORT_BYTES_FOLDER", @".\ExcelBytes");
                m_Args.Add("EXPORT_CLASS_FOLDER", @".\ExcelClass");
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string key = args[i];
                    if (key.StartsWith("$"))
                    {
                        key = key.Replace("$", "");
                        i++;
                        m_Args.Add(key, args[i]);
                    }
                }
            }

            m_ClassTemplate_Class = File.ReadAllText(m_Args[ArgsType.CLASS_TEMPLATE_PATH] + "/Template_Class.txt");
            m_ClassTemplate_SingleClass = File.ReadAllText(m_Args[ArgsType.CLASS_TEMPLATE_PATH] + "/Template_SingleClass.txt");
            m_ClassTemplate_SingleClassManager = File.ReadAllText(m_Args[ArgsType.CLASS_TEMPLATE_PATH] + "/Template_SingleClassManager.txt");
            m_JsonFolder = m_Args[ArgsType.EXPORT_JSON_FOLDER];
            m_ExcelFolder = m_Args[ArgsType.EXCEL_FOLDER];
            m_ExportType = m_Args[ArgsType.EXPORT_TYPE];
            m_ExportClassPath = m_Args[ArgsType.EXPORT_CLASS_FOLDER];
            m_ExportBytesPath = m_Args[ArgsType.EXPORT_BYTES_FOLDER];

            FileHelper.DelectDir(m_JsonFolder);
            FileHelper.DelectDir(m_ExportClassPath);
            FileHelper.DelectDir(m_ExportBytesPath);
        }

        #region 导出class

        #endregion
        public static string Convert(string type, string value)
        {
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                    return $"[{value}]";
                case "string[]":
                    return $"[{value}]";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    return value;
                case "string":
                    return $"\"{value}\"";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        public static bool IsIgnoreField(string field)
        {
            if (field.StartsWith(Flag.NOTE)) return true;
            if (field.StartsWith(ExportType.IsIgnoreFlage(m_ExportType))) return true;

            return false;
        }

        public static bool IsObject(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            if (field == Flag.OBJECT) return true;
            return false;
        }

    }
}
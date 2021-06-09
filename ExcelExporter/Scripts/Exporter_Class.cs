using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{

    public class Exporter_Class_Replace
    {
        /// <summary>
        /// 添加单个配置类标识
        /// </summary>
        public const string Classes = "(Classes)";

        /// <summary>
        /// 类名标识
        /// </summary>
        public const string ConfigName = "(ConfigName)";

        /// <summary>
        /// 类域名添加选项
        /// </summary>
        public const string Fields = "(Fields)";

        /// <summary>
        /// 配置对象列表
        /// </summary>
        public const string ConfigList = "(ConfigList)";

        /// <summary>
        /// 配置序列化后应用到字典
        /// </summary>
        public const string ConfigDeserialization = "(ConfigDeserialization)";
    }

    public class ClassFlag
    {
        public string ClassName { get; set; }
        public bool IsObject { get; set; }
    }

    public class Exporter_Class
    {
        private static Dictionary<ClassFlag, List<HeadInfo>> headInfos = new Dictionary<ClassFlag, List<HeadInfo>>();
        private static StringBuilder classes = new StringBuilder();
        private static StringBuilder configList = new StringBuilder();
        private static StringBuilder configDeserialization = new StringBuilder();

        public static void ExportExcelClass(ExcelPackage p, string className, string exportPath, string template, string template_singleClass, string template_singleClassManager)
        {
            //导出单个配置类
            if (p.Workbook.Worksheets.Count <= 1)
            {
                foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
                {
                    List<HeadInfo> classField = new List<HeadInfo>();
                    HashSet<string> uniqeField = new HashSet<string>();

                    bool isObj = Program.IsObject(worksheet.Cells[1, 1].Text.Trim());

                    ExportClassHeadInfos(worksheet, classField, uniqeField);
                    ExportClass(className, classField, exportPath, template, isObj);
                }
            }
            //导出内部类
            else
            {
                headInfos.Clear();
                classes.Clear();
                configList.Clear();
                configDeserialization.Clear();

                HashSet<string> uniqeField = new HashSet<string>();

                foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
                {
                    ExportInternalClassHeadInfos(worksheet);
                }

                ExportInternalSingleClassInfo(template_singleClass);
                ExportInternalSingleClassManager(className, exportPath, template_singleClassManager);
            }
        }

        /// <summary>
        /// 导出类属性标识
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="classField"></param>
        /// <param name="uniqeField"></param>
        private static void ExportClassHeadInfos(ExcelWorksheet worksheet, List<HeadInfo> classField, HashSet<string> uniqeField)
        {
            const int row = 1;
            for (int col = 1; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }
                if (!uniqeField.Add(fieldName))
                {
                    continue;
                }
                string fieldCS = worksheet.Cells[row, col].Text.Trim();
                string fieldDesc = worksheet.Cells[row + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                classField.Add(new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType));
            }
        }

        /// <summary>
        /// 导出类数据
        /// </summary>
        /// <param name="protoName"></param>
        /// <param name="classField"></param>
        /// <param name="dir"></param>
        /// <param name="template"></param>
        private static void ExportInternalSingleClassInfo(string template_singleClass)
        {
            //导出内部单个类

            int index = 1;
            foreach (var item in headInfos)
            {
                string className = item.Key.ClassName;
                bool IsObject = item.Key.IsObject;
                StringBuilder fields = new StringBuilder();
                //导出单个类所有属性
                for (int i = 0; i < item.Value.Count; i++)
                {
                    HeadInfo headInfo = item.Value[i];
                    if (Program.IsIgnoreField(headInfo.FieldAttribute))
                    {
                        continue;
                    }

                    fields.Append($"\t\t\t[ProtoMember({i + 1}, IsRequired  = true)]\n");
                    fields.Append($"\t\t\tpublic {headInfo.FieldType} {headInfo.FieldName} {{ get; set; }}\n");
                }
                //添加内部类
                string singleClass = $"{template_singleClass.Replace("(ConfigName)", className).Replace(("(Fields)"), fields.ToString())}\n";
                classes.AppendLine(singleClass);

                if (IsObject == false)
                {
                    //添加数组字段
                    configList.AppendLine("\t\t[BsonElement]");
                    configList.AppendLine($"\t\t[ProtoMember({index})]");
                    configList.AppendLine($"\t\tprivate List<{className}> {className}List = new List<{className}>();\n");
                    //添加反序列化添加到字典
                    configDeserialization.AppendLine($"\t\t\tType type_{className} = typeof({className});");
                    configDeserialization.AppendLine($"\t\t\tdicts.Add(type_{className}, new Dictionary<int, IConfig>());");
                    configDeserialization.AppendLine($"\t\t\tforeach ({className} config in {className}List)");
                    configDeserialization.AppendLine("\t\t\t{");
                    configDeserialization.AppendLine($"\t\t\t\tthis.dicts[type_{className}].Add(config.Id, config);");
                    configDeserialization.AppendLine("\t\t\t}");
                    configDeserialization.AppendLine($"\t\t\t{className}List.Clear();\n");
                }
                else
                {
                    configList.AppendLine("\t\t[BsonElement]");
                    configList.AppendLine($"\t\t[ProtoMember({index})]");
                    configList.Append($"\t\tpublic {className} {className}Property");
                    configList.AppendLine("{ get;private set; }\n");
                }

                index++;
            }

            //string content = template_singleClassManager.Replace("(ConfigName)", protoName);
            //content = content.Replace("(ConfigList)", configList.ToString());
            //content = content.Replace("(ConfigDeserialization)", configDeserialization.ToString());
            //content = content.Replace("(Classes)", classes.ToString());
        }

        private static void ExportInternalSingleClassManager(string protoName, string dir, string template_singleClassManager)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string exportPath = Path.Combine(dir, $"{protoName}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            string content = template_singleClassManager.Replace("(ConfigName)", protoName);
            content = content.Replace("(ConfigList)", configList.ToString());
            content = content.Replace("(ConfigDeserialization)", configDeserialization.ToString());
            content = content.Replace("(Classes)", classes.ToString());

            sw.Write(content);
        }

        /// <summary>
        /// 导出类属性标识
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="classField"></param>
        /// <param name="uniqeField"></param>
        private static void ExportInternalClassHeadInfos(ExcelWorksheet worksheet)
        {
            List<HeadInfo> classField = new List<HeadInfo>();

            const int row = 1;
            for (int col = 1; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }

                string fieldCS = worksheet.Cells[row, col].Text.Trim();
                string fieldDesc = worksheet.Cells[row + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                classField.Add(new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType));
            }

            ClassFlag flag = new ClassFlag() { ClassName = worksheet.Name, IsObject = Program.IsObject(worksheet.Cells[1, 1].Text.Trim()) };

            headInfos.Add(flag, classField);
        }

        /// <summary>
        /// 导出类数据
        /// </summary>
        /// <param name="protoName"></param>
        /// <param name="classField"></param>
        /// <param name="dir"></param>
        /// <param name="template"></param>
        private static void ExportClass(string protoName, List<HeadInfo> classField, string dir, string template, bool isObj)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string exportPath = Path.Combine(dir, $"{protoName}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < classField.Count; i++)
            {
                HeadInfo headInfo = classField[i];
                if (Program.IsIgnoreField(headInfo.FieldAttribute))
                {
                    continue;
                }

                sb.Append($"\t\t[ProtoMember({i + 1}, IsRequired  = true)]\n");
                sb.Append($"\t\tpublic {headInfo.FieldType} {headInfo.FieldName} {{ get; set; }}\n");
            }

            string content = template.Replace("(ConfigName)", protoName).Replace(("(Fields)"), sb.ToString());

            StringBuilder property = new StringBuilder();
            StringBuilder configDeserialization = new StringBuilder();
            //添加字段
            if (isObj == false)
            {
                property.AppendLine("\t\t[BsonElement]");
                property.AppendLine($"\t\t[ProtoMember(1)]");
                property.AppendLine($"\t\tprivate List<{protoName}> {protoName}List = new List<{protoName}>();\n");

                configDeserialization.AppendLine($"\t\t\tforeach ({protoName} config in {protoName}List)");
                configDeserialization.AppendLine("\t\t\t{");
                configDeserialization.AppendLine($"\t\t\t\tthis.dict.Add(config.Id, config);");
                configDeserialization.AppendLine("\t\t\t}");
                configDeserialization.AppendLine($"\t\t\t{protoName}List.Clear();");

                content = content.Replace("(ConfigDeserialization)", configDeserialization.ToString());
            }
            else
            {
                property.AppendLine("\t\t[BsonElement]");
                property.AppendLine($"\t\t[ProtoMember(1)]");
                property.Append($"\t\tpublic {protoName} {protoName}Property");
                property.AppendLine("{ get;private set; }\n");

                content = content.Replace("(ConfigDeserialization)", "");
            }

            content = content.Replace("(Property)", property.ToString());

            sw.Write(content);
        }
    }
}

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class Exporter_Json
    {

        #region 导出json
        //public static void ExportExcelJson(ExcelPackage p, string dir)
        //{
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //    }

        //    StringBuilder sb = new StringBuilder();

        //    foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
        //    {
        //        sb.Clear();

        //        sb.AppendLine("{\"list\":[");
        //        ExportSheetJson(worksheet, sb);
        //        sb.AppendLine("]}");

        //        string jsonPath = Path.Combine(dir, $"{worksheet.Name}.txt");
        //        using FileStream txt = new FileStream(jsonPath, FileMode.Create);
        //        using StreamWriter sw = new StreamWriter(txt);
        //        sw.Write(sb.ToString());
        //    }
        //}

        public static void ExportExcelJson(ExcelPackage p, string jsonName, string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string jsonPath = Path.Combine(dir, $"{jsonName}.txt");
            using FileStream txt = new FileStream(jsonPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);
            StringBuilder sb = new StringBuilder();

            bool isSingle = p.Workbook.Worksheets.Count <= 1;

            sb.Append("{");

            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                bool isObj = Program.IsObject(worksheet.Cells[1, 1].Text.Trim());
                //sb.Append("{\"");
                if (isObj)
                {
                    //属性名
                    sb.Append("\"");
                    if (isSingle) sb.Append($"{jsonName}Property");
                    else sb.Append($"{worksheet.Name}Property");
                    sb.Append("\":");

                    ExportSheetJson(worksheet, sb);
                    //sb.AppendLine("},");
                }
                else
                {
                    sb.Append("\"");
                    if (isSingle) sb.Append($"{jsonName}List");
                    else sb.Append($"{worksheet.Name}List");
                    sb.Append("\":[");

                    ExportSheetJson(worksheet, sb);
                    sb.AppendLine("],");
                }
            }

            sb.AppendLine("},");

            sw.Write(sb.ToString());
        }

        private static void ExportSheetJson(ExcelWorksheet worksheet, StringBuilder sb)
        {
            int infoRow = 1;
            HeadInfo[] headInfos = new HeadInfo[100];

            for (int col = 1; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldCS = worksheet.Cells[infoRow, col].Text.Trim();

                if (Program.IsIgnoreField(fieldCS))
                {
                    continue;
                }

                string fieldName = worksheet.Cells[infoRow + 2, col].Text.Trim();
                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                string fieldDesc = worksheet.Cells[infoRow + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[infoRow + 3, col].Text.Trim();

                headInfos[col] = new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType);
            }
            #endregion

            #region 导出数据
            for (int row = 5; row <= worksheet.Dimension.End.Row; ++row)
            {
                sb.Append("{");
                for (int col = 1; col <= worksheet.Dimension.End.Column; ++col)
                {
                    HeadInfo headInfo = headInfos[col];
                    if (headInfo.FieldAttribute == null)
                    {
                        continue;
                    }

                    if (Program.IsIgnoreField(headInfo.FieldAttribute))
                    {
                        continue;
                    }

                    if (headInfo.FieldName == "Id")
                    {
                        headInfo.FieldName = "_id";
                    }
                    else
                    {
                        sb.Append(",");
                    }

                    sb.Append($"\"{headInfo.FieldName}\":{Program.Convert(headInfo.FieldType, worksheet.Cells[row, col].Text.Trim())}");
                }
                sb.Append("},\n");
            }
        }
        #endregion
    }
}

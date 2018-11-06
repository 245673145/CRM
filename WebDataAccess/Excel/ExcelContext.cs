using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace CRM.Web.DataAccess.Excel
{
    public  class ExcelContext
    {
        /// <summary>
        /// 当前进行流式读取的excel
        /// </summary>
        HSSFWorkbook workbookForStream;
        /// <summary>
        /// 当前每个表格跳过的行
        /// </summary>
        int skipRow;
        /// <summary>
        /// 从读取的表格开始索引
        /// </summary>
        int startIndex;
        /// <summary>
        /// 从从读取的表格结束索引
        /// </summary>
        int endIndex;
        /// <summary>
        /// 每行读取的总的单元格数
        /// </summary>
        int cellCount;
        /// <summary>
        /// 当前表中行的迭代
        /// </summary>
        IEnumerator currentRows;
        /// <summary>
        /// 打开一个表格流
        /// </summary>
        /// <param name="excel">excel流</param>
        /// <param name="skipRow">每个sheet跳过的行</param>
        /// <param name="cellCount">每行读取的单元格数</param>
        /// <returns></returns>
        public bool Open(Stream excel, int skipRow, int cellCount)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return this.OpenWorkbook(workbook, 0, workbook.NumberOfSheets - 1, skipRow, cellCount);

        }
        /// <summary>
        /// 打开一个表格流
        /// </summary>
        /// <param name="excel">excel流</param>
        /// <param name="endSheetIndex">读到第几个sheet开始读</param>
        /// <param name="excel">excel流</param>
        /// <param name="skipRow">每个sheet跳过的行</param>
        /// <param name="cellCount">每行读取的单元格数</param>
        /// <returns></returns>
        public bool Open(Stream excel, int endSheetIndex, int skipRow, int cellCount)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return this.OpenWorkbook(workbook, 0, endSheetIndex, skipRow, cellCount);
        }
        /// <summary>
        /// 打开一个表格流
        /// </summary>
        /// <param name="excel">excel流</param>
        /// <param name="startSheetIndex">从第几个sheet开始读</param>
        /// <param name="endSheetIndex">读到第几个sheet开始读</param>
        /// <param name="excel">excel流</param>
        /// <param name="skipRow">每个sheet跳过的行</param>
        /// <param name="cellCount">每行读取的单元格数</param>
        /// <returns></returns>
        public bool Open(Stream excel, int startSheetIndex, int endSheetIndex, int skipRow, int cellCount)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return this.OpenWorkbook(workbook, startSheetIndex, endSheetIndex, skipRow, cellCount);
        }
        /// <summary>
        /// 打开一个表格
        /// </summary>
        /// <param name="excel">excel流</param>
        /// <param name="startSheetIndex">从第几个sheet开始读</param>
        /// <param name="endSheetIndex">读到第几个sheet开始读</param>
        /// <param name="excel">excel流</param>
        /// <param name="skipRow">每个sheet跳过的行</param>
        /// <param name="cellCount">每行读取的单元格数</param>
        /// <returns></returns>
        private bool OpenWorkbook(HSSFWorkbook excel, int startSheetIndex, int endSheetIndex, int skipRow, int cellCount)
        {
            if (workbookForStream != null)
                return false;
            workbookForStream = excel;
            this.skipRow = skipRow;
            this.startIndex = startSheetIndex;
            this.endIndex = endSheetIndex;
            this.cellCount = cellCount;
            //初始化当前表格
            HSSFSheet currentSheet = workbookForStream.GetSheetAt(startIndex) as HSSFSheet;
            //初始化当臆表格的所有行
            currentRows = currentSheet.GetRowEnumerator();
            int skip = this.skipRow;
            //去掉表头
            while (skip-- > 0)
            {
                currentRows.MoveNext();
            }
            return true;
        }
        /// <summary>
        /// 读取下一行，如果存在数据则返回一个数组，否则返回为空
        /// </summary>
        /// <returns></returns>
        public string[] Next()
        {
            //一行数据的返回值
            string[] result = new string[this.cellCount];
            //移动到下一行，如果下一行为false则读取下一个表格
            while (!currentRows.MoveNext())
            {
                //当进入最后一张表时，返回为null,i不在进行读取
                if (++startIndex > endIndex)
                    return null;
                //初始化当前表格
                HSSFSheet currentSheet = workbookForStream.GetSheetAt(startIndex) as HSSFSheet;
                //初始化当臆表格的所有行
                currentRows = currentSheet.GetRowEnumerator();
                //循环跳过表头
                int skip = this.skipRow;
                //去掉表头
                while (skip-- > 0)
                {
                    currentRows.MoveNext();
                }
            }
            //一行中所有的单元格
            HSSFRow row = currentRows.Current as HSSFRow;
            //根据列的个数来读取单元格中的内容
            for (int j = 0; j < this.cellCount; j++)
            {
                HSSFCell cell = row.GetCell(j) as HSSFCell;
                result[j] = cell == null ? "" : ChangeValueType(cell).ToString();
            }
            return result;
        }
        /// <summary>
        /// 关闭excel
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (this.workbookForStream == null)
                return false;
            this.workbookForStream = null;
            this.cellCount = 0;
            this.skipRow = 0;
            this.startIndex = 0;
            this.endIndex = 0;
            return true;
        }
        private object ChangeValueType(HSSFCell hssf)
        {
            object result="";
            switch (hssf.CellType)
            {
                case CellType.Blank://空字符串
                    break;
                case CellType.Boolean://
                    result = hssf.BooleanCellValue;
                    break;
                case CellType.Error://出现了错误
                    result = hssf.ErrorCellValue;
                    break;
                case CellType.Formula://公式
                    result ="含有公式";
                    break;
                case CellType.Numeric://数字
                    result = hssf.NumericCellValue.ToString().Trim();
                    break;
                case CellType.String://文本
                    result = hssf.StringCellValue.ToString().Trim();
                    break;
                case CellType.Unknown://未知
                    result = "该单元格的值不能被识别";
                    break;
            }
            return result;

        }
        /// <summary>
        /// 读取所有sheet中的数据
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="skipRow"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public List<DataTable> ReadSheet(Stream excel, int skipRow, string[] cols)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return ReadSheet(workbook, 0, workbook.NumberOfSheets - 1, skipRow, cols);
        }
        /// <summary>
        /// 读取所有sheet中的数据
        /// </summary>
        /// <param name="excel">被读取的表格流</param>
        /// <param name="workbook">被读取的表格</param>
        /// <param name="readSheetCount">读取的sheet总数，从0开始</param>
        /// <param name="skipRow">每个表格跳过的行</param>
        /// <param name="cols">返回结果的列名称</param>
        /// <returns></returns>
        public List<DataTable> ReadSheet(Stream excel, int readSheetCount, int skipRow, string[] cols)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return ReadSheet(workbook, 0, readSheetCount, skipRow, cols);
        }
        /// <summary>
        /// 读取所有表格中的数据
        /// </summary>
        /// <param name="excel">被读取的表格流</param>
        /// <param name="workbook">被读取的表格</param>
        /// <param name="startSheetIndex">开始的sheet索引</param>
        /// <param name="endSheetIndex">结束的sheet索引</param>
        /// <param name="skipRow">每个表格跳过的行</param>
        /// <param name="cols">返回结果的列名称</param>
        /// <returns></returns>
        public List<DataTable> ReadSheet(Stream excel, int startSheetIndex, int readSheetCount, int skipRow, string[] cols)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excel);
            return ReadSheet(workbook, startSheetIndex, startSheetIndex + readSheetCount, skipRow, cols);
        }
        /// <summary>
        /// 读取所有表格中的数据
        /// </summary>
        /// <param name="workbook">被读取的表格</param>
        /// <param name="startSheetIndex">开始的sheet索引</param>
        /// <param name="endSheetIndex">结束的sheet索引</param>
        /// <param name="skipRow">每个表格跳过的行</param>
        /// <param name="cols">返回结果的列名称</param>
        /// <returns></returns>
        private List<DataTable> ReadSheet(HSSFWorkbook workbook, int startSheetIndex, int endSheetIndex, int skipRow, string[] cols)
        {
            //初始化返回值
            List<DataTable> result = new List<DataTable>();
            //返回值中的列名称初始化
            DataColumn[] columns = new DataColumn[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                columns[i] = new DataColumn(cols[i]);
            }
            //从startSheetIndex开始读取sheet到endSheetIndex结束
            for (int i = startSheetIndex; i <= endSheetIndex; i++)
            {
                //复制要跳过的行数
                int skip = skipRow;
                //创建table
                DataTable table = new DataTable();
                //添加到返回值中
                result.Add(table);
                //向table中添加列
                table.Columns.AddRange(columns);
                //读取Excel中的sheet
                HSSFSheet sheet = workbook.GetSheetAt(i) as HSSFSheet;
                //读取行（一个sheet中的所有行）
                IEnumerator rows = sheet.GetRowEnumerator();
                //跳过表头不读取
                while (skip-- > 0)
                {
                    rows.MoveNext();
                }
                //循环读取当前sheet中的数据
                while (rows.MoveNext())
                {
                    //创建一行
                    DataRow dRow = table.NewRow();
                    //添加到表格中
                    table.Rows.Add(dRow);
                    //获取当前行，一行中所有的单元格
                    HSSFRow row = rows.Current as HSSFRow;

                    //根据列的个数来读取单元格中的内容
                    for (int j = 0; j < cols.Length; j++)
                    {
                        HSSFCell cell = row.GetCell(j) as HSSFCell;
                        dRow[j] = cell == null ? "" : cell.StringCellValue;
                    }
                }
            }
            return result;
        }
        public MemoryStream WriteSheet(MemoryStream excel, string sheetName, int skipRow, short color, List<string[]> rows)
        {
            HSSFWorkbook book = new HSSFWorkbook(excel);
            HSSFSheet sheet = book.GetSheet(sheetName) as HSSFSheet;
            MemoryStream ms = new MemoryStream();
            this.WriteSheet(sheet, skipRow, color, rows);
            book.Write(ms);
            return new MemoryStream(ms.ToArray());
        }
        public MemoryStream WriteSheet(MemoryStream excel, int sheetIndex, int skipRow, short color, List<string[]> rows)
        {
            HSSFWorkbook book = new HSSFWorkbook(excel);
            HSSFSheet sheet = book.GetSheetAt(sheetIndex) as HSSFSheet;
            MemoryStream ms = new MemoryStream();
            this.WriteSheet(sheet, skipRow, color, rows);
            book.Write(ms);
            return new MemoryStream(ms.ToArray());
        }
        /// <summary>
        /// 将数组写入到表格中
        /// </summary>
        /// <param name="sheet">被写入的sheet</param>
        /// <param name="skipRow">表头跳过的行</param>
        /// <param name="rows">所有写入的数据</param>
        public void WriteSheet(HSSFSheet sheet, int skipRow, short color, List<string[]> rows)
        {
            //写入的行的结束索引
            int endRowIndex = rows.Count + skipRow;
            //从skipRow开始写入数据
            for (int i = skipRow; i < endRowIndex; i++)
            {
                //创建一个单元格
                HSSFRow newRow = sheet.CreateRow(i) as HSSFRow;
                //当前行中要写的值
                string[] rowValues = rows[i - skipRow];
                //循环所有的值写入到单元格中
                for (int j = 0; j < rowValues.Length; j++)
                {
                    //获取当前行中的单元格
                    HSSFCell cell = newRow.CreateCell(j) as HSSFCell;
                    //设置单元格的值
                    cell.SetCellValue(rowValues[j]);
                    //设置背景颜色
                    cell.CellStyle.FillBackgroundColor = color;
                }
            }
        }
    }
}

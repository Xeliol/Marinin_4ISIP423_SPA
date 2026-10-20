using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class MastersLoadPage : Page
    {
        private class MasterReportData
        {
            public string MasterName { get; set; }
            public int ServicesCount { get; set; }
        }

        private List<MasterReportData> _cachedData = new List<MasterReportData>();
        private Chart ChartMasters = new Chart();

        public MastersLoadPage()
        {
            InitializeComponent();

            ChartMasters.ChartAreas.Add(new ChartArea("Main"));
            ChartMasters.Legends.Add(new Legend());

            var currentSeries = new Series("Услуги мастеров")
            {
                IsValueShownAsLabel = true
            };
            ChartMasters.Series.Add(currentSeries);

            WfHost.Child = ChartMasters;

            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            CmbDiagram.SelectedItem = SeriesChartType.Column;

            LoadMastersData();
        }

        private void LoadMastersData()
        {
            _cachedData = Core.Context.Services
                .Where(s => s.Users != null)
                .GroupBy(s => s.Users.Name)
                .Select(g => new MasterReportData
                {
                    MasterName = g.Key,
                    ServicesCount = g.Count()
                })
                .OrderByDescending(x => x.ServicesCount)
                .ToList();

            DgMasters.ItemsSource = _cachedData;
            UpdateChart(null, null);
        }
        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (ChartMasters != null && ChartMasters.Series.Count > 0 && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartMasters.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                foreach (var item in _cachedData)
                {
                    currentSeries.Points.AddXY(item.MasterName, item.ServicesCount);
                }
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cachedData == null || _cachedData.Count == 0) return;

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                Word.Paragraph titleParagraph = document.Paragraphs.Add();
                Word.Range titleRange = titleParagraph.Range;
                titleRange.Text = "Анализ количества оказываемых услуг по мастерам";
                titleParagraph.set_Style("Заголовок 1");
                titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titleRange.InsertParagraphAfter();
                document.Paragraphs.Add();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table mastersTable = document.Tables.Add(tableRange, _cachedData.Count + 1, 2);

                mastersTable.Borders.InsideLineStyle = mastersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                mastersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = mastersTable.Cell(1, 1).Range; cellRange.Text = "Мастер (Сотрудник)";
                cellRange = mastersTable.Cell(1, 2).Range; cellRange.Text = "Количество услуг";

                mastersTable.Rows[1].Range.Font.Name = "Times New Roman";
                mastersTable.Rows[1].Range.Font.Size = 14;
                mastersTable.Rows[1].Range.Bold = 1;
                mastersTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < _cachedData.Count; i++)
                {
                    cellRange = mastersTable.Cell(i + 2, 1).Range;
                    cellRange.Text = _cachedData[i].MasterName;
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;

                    cellRange = mastersTable.Cell(i + 2, 2).Range;
                    cellRange.Text = _cachedData[i].ServicesCount.ToString() + " шт.";
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                    cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }
                document.Paragraphs.Add();

                var maxMaster = _cachedData.OrderByDescending(x => x.ServicesCount).FirstOrDefault();
                if (maxMaster != null)
                {
                    Word.Paragraph maxParagraph = document.Paragraphs.Add();
                    Word.Range maxRange = maxParagraph.Range;
                    maxRange.Text = $"Максимальная нагрузка — Мастер \"{maxMaster.MasterName}\" (Услуг: {maxMaster.ServicesCount})";
                    maxParagraph.set_Style("Подзаголовок");
                    maxRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxRange.InsertParagraphAfter();
                }
                document.Paragraphs.Add();

                var minMaster = _cachedData.OrderBy(x => x.ServicesCount).FirstOrDefault();
                if (minMaster != null)
                {
                    Word.Paragraph minParagraph = document.Paragraphs.Add();
                    Word.Range minRange = minParagraph.Range;
                    minRange.Text = $"Минимальная нагрузка — Мастер \"{minMaster.MasterName}\" (Услуг: {minMaster.ServicesCount})";
                    minParagraph.set_Style("Подзаголовок");
                    minRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                    minRange.InsertParagraphAfter();
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.Size = 10;
                    headerRange.Text = DateTime.Now.ToString("dd.MM.yyyy");
                }

                document.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekCurrentPageFooter;
                object oMissing = System.Reflection.Missing.Value;
                object TotalPages = Word.WdFieldType.wdFieldNumPages;
                object CurrentPage = Word.WdFieldType.wdFieldPage;

                document.ActiveWindow.Selection.Fields.Add(document.ActiveWindow.Selection.Range, ref CurrentPage, ref oMissing, ref oMissing);
                document.ActiveWindow.Selection.TypeText(" из ");
                document.ActiveWindow.Selection.Fields.Add(document.ActiveWindow.Selection.Range, ref TotalPages, ref oMissing, ref oMissing);

                document.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekMainDocument;
                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка работы с Word: {ex.Message}");
            }
        }
        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cachedData == null || _cachedData.Count == 0) return;

                var application = new Excel.Application();
                application.SheetsInNewWorkbook = _cachedData.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                int grandTotal = 0;

                for (int i = 0; i < _cachedData.Count; i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];

                    string masterName = _cachedData[i].MasterName;
                    worksheet.Name = masterName.Length > 30 ? masterName.Substring(0, 30) : masterName;

                    worksheet.Cells[1, startRowIndex].Value = "ID Услуги";
                    worksheet.Cells[2, startRowIndex].Value = "Название услуги";
                    worksheet.Cells[3, startRowIndex].Value = "Стоимость базового часа";
                    worksheet.Cells[4, startRowIndex].Value = "Статус привязки";

                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, startRowIndex], worksheet.Cells[4, startRowIndex]];
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Bold = true;
                    startRowIndex++;

                    string currentMasterName = _cachedData[i].MasterName;
                    var localServices = Core.Context.Services
                        .Where(s => s.Users.Name == currentMasterName)
                        .ToList();

                    foreach (var srv in localServices)
                    {
                        worksheet.Cells[1, startRowIndex].Value = srv.ServiceID;
                        worksheet.Cells[2, startRowIndex].Value = srv.Name;
                        worksheet.Cells[3, startRowIndex].Value = 1000;
                        (worksheet.Cells[3, startRowIndex] as Excel.Range).NumberFormat = "0,00";
                        worksheet.Cells[4, startRowIndex].Value = "Закреплена";
                        startRowIndex++;
                    }
                    Excel.Range sumRange = worksheet.Range[worksheet.Cells[1, startRowIndex], worksheet.Cells[3, startRowIndex]];
                    sumRange.Merge();
                    sumRange.Value = "ИТОГО ЗАКРЕПЛЕНО УСЛУГ:";
                    sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    worksheet.Cells[4, startRowIndex].Formula = $"=COUNTA(A2:A{startRowIndex - 1})";
                    sumRange.Font.Bold = worksheet.Cells[4, startRowIndex].Font.Bold = true;
                    grandTotal += _cachedData[i].ServicesCount;
                    Excel.Range borders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[4, startRowIndex]];
                    borders.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Columns.AutoFit();
                }
                Excel.Worksheet summarySheet = workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                summarySheet.Cells[1, 1].Value = "Общий итог:";
                summarySheet.Cells[1, 2].Value = grandTotal;
                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[1, 2]];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;
                summarySheet.Columns.AutoFit();
                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка работы с Excel: {ex.Message}");
            }
        }
    }
}
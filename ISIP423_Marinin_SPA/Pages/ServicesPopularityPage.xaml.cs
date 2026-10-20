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
    public partial class ServicesPopularityPage : Page
    {
        private class PopularityDataData
        {
            public string ServiceName { get; set; }
            public int AppointmentCount { get; set; }
        }

        private List<PopularityDataData> _cachedData = new List<PopularityDataData>();

        private Chart ChartPopularity = new Chart();

        public ServicesPopularityPage()
        {
            InitializeComponent();

            ChartPopularity.ChartAreas.Add(new ChartArea("Main"));
            ChartPopularity.Legends.Add(new Legend());

            var currentSeries = new Series("Востребованность услуг")
            {
                IsValueShownAsLabel = true
            };
            ChartPopularity.Series.Add(currentSeries);

            WfHost.Child = ChartPopularity;

            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            CmbDiagram.SelectedItem = SeriesChartType.Column;

            LoadPopularityData();
        }

        private void LoadPopularityData()
        {
            _cachedData = Core.Context.Appointments
                .Where(a => a.UserID != null)
                .GroupBy(a => a.Services.Name)
                .Select(g => new PopularityDataData
                {
                    ServiceName = g.Key,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentCount)
                .ToList();

            DgServices.ItemsSource = _cachedData;

            UpdateChart(null, null);
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (ChartPopularity != null && ChartPopularity.Series.Count > 0 && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPopularity.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                foreach (var item in _cachedData)
                {
                    currentSeries.Points.AddXY(item.ServiceName, item.AppointmentCount);
                }
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cachedData == null || _cachedData.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var application = new Excel.Application();

                application.SheetsInNewWorkbook = _cachedData.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                int grandTotal = 0;

                for (int i = 0; i < _cachedData.Count; i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];

                    string sheetName = _cachedData[i].ServiceName;
                    worksheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName;

                    worksheet.Cells[1][startRowIndex] = "ID Записи";
                    worksheet.Cells[2][startRowIndex] = "ID Клиента";
                    worksheet.Cells[3][startRowIndex] = "Дата бронирования";
                    worksheet.Cells[4][startRowIndex] = "Статус";

                    Excel.Range columnHeaderRange = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[4][1]];
                    columnHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    columnHeaderRange.Font.Bold = true;
                    startRowIndex++;

                    string currentServiceName = _cachedData[i].ServiceName;
                    var detailsList = Core.Context.Appointments
                        .Where(a => a.UserID != null && a.Services.Name == currentServiceName)
                        .ToList();

                    int rowsAddedCount = 0;
                    foreach (var app in detailsList)
                    {
                        worksheet.Cells[1][startRowIndex] = app.AppointmentID;
                        worksheet.Cells[2][startRowIndex] = app.UserID;
                        worksheet.Cells[3][startRowIndex] = app.Date.ToString("dd.MM.yyyy");
                        worksheet.Cells[4][startRowIndex] = "Активно";

                        startRowIndex++;
                        rowsAddedCount++;
                    }

                    Excel.Range sumRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[3][startRowIndex]];
                    sumRange.Merge();
                    sumRange.Value = "ИТОГО ЗАПИСЕЙ:";
                    sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    worksheet.Cells[4][startRowIndex].Formula = $"=COUNTA(A2:A{startRowIndex - 1})";
                    sumRange.Font.Bold = worksheet.Cells[4][startRowIndex].Font.Bold = true;

                    grandTotal += _cachedData[i].AppointmentCount;

                    Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[4][startRowIndex]];
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                    worksheet.Columns.AutoFit();
                }

                Excel.Worksheet summarySheet = workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";

                summarySheet.Cells[1, 1] = "Общий итог всех бронирований:";
                summarySheet.Cells[1, 2] = grandTotal;

                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[1, 2]];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;

                summarySheet.Columns.AutoFit();

                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cachedData == null || _cachedData.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                Word.Paragraph titleParagraph = document.Paragraphs.Add();
                Word.Range titleRange = titleParagraph.Range;
                titleRange.Text = "Рейтинг востребованности услуг";
                titleParagraph.set_Style("Заголовок 1");
                titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titleRange.InsertParagraphAfter();
                document.Paragraphs.Add();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table servicesTable = document.Tables.Add(tableRange, _cachedData.Count + 1, 2);

                servicesTable.Borders.InsideLineStyle = servicesTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                servicesTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = servicesTable.Cell(1, 1).Range;
                cellRange.Text = "Название услуги";
                cellRange = servicesTable.Cell(1, 2).Range;
                cellRange.Text = "Количество записей";

                servicesTable.Rows[1].Range.Font.Name = "Times New Roman";
                servicesTable.Rows[1].Range.Font.Size = 14;
                servicesTable.Rows[1].Range.Bold = 1;
                servicesTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < _cachedData.Count; i++)
                {
                    var currentService = _cachedData[i];

                    cellRange = servicesTable.Cell(i + 2, 1).Range;
                    cellRange.Text = currentService.ServiceName;
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;

                    cellRange = servicesTable.Cell(i + 2, 2).Range;
                    cellRange.Text = currentService.AppointmentCount.ToString() + " шт.";
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                    cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }
                document.Paragraphs.Add();

                var maxService = _cachedData.OrderByDescending(x => x.AppointmentCount).FirstOrDefault();
                if (maxService != null)
                {
                    Word.Paragraph maxParagraph = document.Paragraphs.Add();
                    Word.Range maxRange = maxParagraph.Range;
                    maxRange.Text = $"Самая востребованная услуга — \"{maxService.ServiceName}\" (Записей: {maxService.AppointmentCount})";
                    maxParagraph.set_Style("Подзаголовок");
                    maxRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxRange.InsertParagraphAfter();
                }
                document.Paragraphs.Add();

                var minService = _cachedData.OrderBy(x => x.AppointmentCount).FirstOrDefault();
                if (minService != null)
                {
                    Word.Paragraph minParagraph = document.Paragraphs.Add();
                    Word.Range minRange = minParagraph.Range;
                    minRange.Text = $"Наименее востребованная услуга — \"{minService.ServiceName}\" (Записей: {minService.AppointmentCount})";
                    minParagraph.set_Style("Подзаголовок");
                    minRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                    minRange.InsertParagraphAfter();
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Word.WdColorIndex.wdBlack;
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
                MessageBox.Show($"Ошибка при экспорте в Word: {ex.Message}", "Ошибка", MessageBoxButton.OK);
            }
        }

    }
}

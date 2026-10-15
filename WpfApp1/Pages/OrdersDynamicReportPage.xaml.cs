using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApp1.Pages
{
    public partial class OrdersDynamicReportPage : Page
    {
        private class OrdersReportData
        {
            public DateTime OrderDate { get; set; }
            public int TotalOrders { get; set; }
        }

        private List<OrdersReportData> _reportData = new List<OrdersReportData>();

        private Chart ChartOrders = new Chart();

        public OrdersDynamicReportPage()
        {
            InitializeComponent();

            ChartOrders.ChartAreas.Add(new ChartArea("Main"));
            ChartOrders.Legends.Add(new Legend());

            var currentSeries = new Series("Интенсивность заказов")
            {
                IsValueShownAsLabel = true
            };
            ChartOrders.Series.Add(currentSeries);

            WfHost.Child = ChartOrders;

            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            CmbDiagram.SelectedItem = SeriesChartType.Line;

            LoadOrdersData();
        }

        private void LoadOrdersData()
        {
            _reportData = Core.Context.Orders
                .ToList()
                .GroupBy(o => o.PreferedDate.Date)
                .Select(g => new OrdersReportData
                {
                    OrderDate = g.Key,
                    TotalOrders = g.Count()
                })
                .OrderBy(d => d.OrderDate)
                .ToList();

            DgOrdersReport.ItemsSource = _reportData;

            UpdateChart(null, null);
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (ChartOrders != null && ChartOrders.Series.Count > 0 && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartOrders.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                foreach (var item in _reportData)
                {
                    currentSeries.Points.AddXY(item.OrderDate.ToString("dd.MM.yyyy"), item.TotalOrders);
                }
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportData == null || _reportData.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                Word.Paragraph titleParagraph = document.Paragraphs.Add();
                Word.Range titleRange = titleParagraph.Range;
                titleRange.Text = "Отчет по интенсивности заказов по дням";
                titleParagraph.set_Style("Заголовок 1");
                titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titleRange.InsertParagraphAfter();
                document.Paragraphs.Add();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table ordersTable = document.Tables.Add(tableRange, _reportData.Count + 1, 2);

                ordersTable.Borders.InsideLineStyle = ordersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                ordersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = ordersTable.Cell(1, 1).Range; cellRange.Text = "Дата отчета";
                cellRange = ordersTable.Cell(1, 2).Range; cellRange.Text = "Количество заказов";

                ordersTable.Rows[1].Range.Font.Name = "Times New Roman";
                ordersTable.Rows[1].Range.Font.Size = 14;
                ordersTable.Rows[1].Range.Bold = 1;
                ordersTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < _reportData.Count; i++)
                {
                    var data = _reportData[i];

                    cellRange = ordersTable.Cell(i + 2, 1).Range;
                    cellRange.Text = data.OrderDate.ToString("dd.MM.yyyy");
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;

                    cellRange = ordersTable.Cell(i + 2, 2).Range;
                    cellRange.Text = data.TotalOrders.ToString() + " шт.";
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                    cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }
                document.Paragraphs.Add();

                var maxDay = _reportData.OrderByDescending(x => x.TotalOrders).FirstOrDefault();
                if (maxDay != null)
                {
                    Word.Paragraph maxParagraph = document.Paragraphs.Add();
                    Word.Range maxRange = maxParagraph.Range;
                    maxRange.Text = $"Пиковая интенсивность — {maxDay.OrderDate.ToString("dd.MM.yyyy")} ({maxDay.TotalOrders} зак.)";
                    maxParagraph.set_Style("Подзаголовок");
                    maxRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxRange.InsertParagraphAfter();
                }
                document.Paragraphs.Add();

                var minDay = _reportData.OrderBy(x => x.TotalOrders).FirstOrDefault();
                if (minDay != null)
                {
                    Word.Paragraph minParagraph = document.Paragraphs.Add();
                    Word.Range minRange = minParagraph.Range;
                    minRange.Text = $"Минимальная интенсивность — {minDay.OrderDate.ToString("dd.MM.yyyy")} ({minDay.TotalOrders} зак.)";
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
                MessageBox.Show($"Ошибка работы с Word: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportData == null || _reportData.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var application = new Excel.Application();

                application.SheetsInNewWorkbook = _reportData.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                int grandTotal = 0;

                for (int i = 0; i < _reportData.Count; i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];
                    worksheet.Name = _reportData[i].OrderDate.ToString("dd.MM.yyyy");
                    // 3. Добавляем заголовки колонок для подробных записей бронирований (ЯВНО указываем столбцы 1-4)
                    worksheet.Cells[1, startRowIndex].Value = "ID Заказа";
                    worksheet.Cells[2, startRowIndex].Value = "Клиент";
                    worksheet.Cells[3, startRowIndex].Value = "Время предпочтения";
                    worksheet.Cells[4, startRowIndex].Value = "Статус выполнения";

                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[4, 1]];
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Bold = true;
                    startRowIndex++;

                    DateTime targetDate = _reportData[i].OrderDate.Date;
                    var dayOrdersList = Core.Context.Orders
                        .ToList()
                        .Where(o => o.PreferedDate.Date == targetDate)
                        .ToList();

                    foreach (var order in dayOrdersList)
                    {
                        worksheet.Cells[1, startRowIndex].Value = order.OrderID;
                        worksheet.Cells[2, startRowIndex].Value = order.UserID;
                        worksheet.Cells[3, startRowIndex].Value = order.PreferedDate.ToString("HH:mm");
                        worksheet.Cells[4, startRowIndex].Value = "Оформлен";

                        startRowIndex++;
                    }

                    Excel.Range sumRange = worksheet.Range[worksheet.Cells[1, startRowIndex], worksheet.Cells[3, startRowIndex]];
                    sumRange.Merge();
                    sumRange.Value = "ИТОГО ЗА ДЕНЬ:";
                    sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    worksheet.Cells[4, startRowIndex].Formula = $"=COUNTA(A2:A{startRowIndex - 1})";
                    sumRange.Font.Bold = worksheet.Cells[4, startRowIndex].Font.Bold = true;

                    grandTotal += _reportData[i].TotalOrders;

                    Excel.Range borders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[4, startRowIndex]];
                    borders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                    borders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                    borders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                    borders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                    borders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                    borders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                    worksheet.Columns.AutoFit();

                }
                Excel.Worksheet summarySheet = workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                summarySheet.Cells[1][1].Value = "Общий итог:";
                summarySheet.Cells[2][1].Value = grandTotal;
                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells, summarySheet.Cells];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;
                summarySheet.Columns.AutoFit();
                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка работы с Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
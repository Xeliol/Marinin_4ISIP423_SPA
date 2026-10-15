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
    public partial class ProductTypesReportPage : Page
    {
        private class ProductReportData
        {
            public string TypeName { get; set; }
            public int ProductCount { get; set; }
        }

        private List<ProductReportData> _reportData = new List<ProductReportData>();

        private Chart ChartProducts = new Chart();

        public ProductTypesReportPage()
        {
            InitializeComponent();

            ChartProducts.ChartAreas.Add(new ChartArea("Main"));
            ChartProducts.Legends.Add(new Legend());

            var currentSeries = new Series("Категории товаров")
            {
                IsValueShownAsLabel = true
            };
            ChartProducts.Series.Add(currentSeries);

            WfHost.Child = ChartProducts;

            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            CmbDiagram.SelectedItem = SeriesChartType.Pie;

            LoadReportData();
        }

        private void LoadReportData()
        {
            _reportData = Core.Context.Products
                .GroupBy(p => p.ProductTypes.Name)
                .Select(g => new ProductReportData
                {
                    TypeName = g.Key,
                    ProductCount = g.Count()
                })
                .ToList();

            DgReport.ItemsSource = _reportData;

            UpdateChart(null, null);
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (ChartProducts != null && ChartProducts.Series.Count > 0 && CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartProducts.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                foreach (var item in _reportData)
                {
                    currentSeries.Points.AddXY(item.TypeName, item.ProductCount);
                }
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportData == null || _reportData.Count == 0) return;

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                Word.Paragraph titleParagraph = document.Paragraphs.Add();
                Word.Range titleRange = titleParagraph.Range;
                titleRange.Text = "Отчет: Анализ структуры ассортимента по типам товаров";
                titleParagraph.set_Style("Заголовок 1");
                titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titleRange.InsertParagraphAfter();
                document.Paragraphs.Add();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Range tableRange = tableParagraph.Range;
                Word.Table dataTable = document.Tables.Add(tableRange, _reportData.Count + 1, 2);

                dataTable.Borders.InsideLineStyle = dataTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                dataTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = dataTable.Cell(1, 1).Range; cellRange.Text = "Тип товара";
                cellRange = dataTable.Cell(1, 2).Range; cellRange.Text = "Кол-во наименований";

                dataTable.Rows[1].Range.Font.Name = "Times New Roman";
                dataTable.Rows[1].Range.Font.Size = 14;
                dataTable.Rows[1].Range.Bold = 1;
                dataTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < _reportData.Count; i++)
                {
                    cellRange = dataTable.Cell(i + 2, 1).Range;
                    cellRange.Text = _reportData[i].TypeName;
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;

                    cellRange = dataTable.Cell(i + 2, 2).Range;
                    cellRange.Text = _reportData[i].ProductCount.ToString() + " наим.";
                    cellRange.Font.Name = "Times New Roman";
                    cellRange.Font.Size = 12;
                    cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }
                document.Paragraphs.Add();

                var maxItem = _reportData.OrderByDescending(x => x.ProductCount).FirstOrDefault();
                if (maxItem != null)
                {
                    Word.Paragraph maxParagraph = document.Paragraphs.Add();
                    Word.Range maxRange = maxParagraph.Range;
                    maxRange.Text = $"Наиболее представленный тип — \"{maxItem.TypeName}\" ({maxItem.ProductCount} наим.)";
                    maxParagraph.set_Style("Подзаголовок");
                    maxRange.Font.Color = Word.WdColor.wdColorDarkRed;
                    maxRange.InsertParagraphAfter();
                }
                document.Paragraphs.Add();

                var minItem = _reportData.OrderBy(x => x.ProductCount).FirstOrDefault();
                if (minItem != null)
                {
                    Word.Paragraph minParagraph = document.Paragraphs.Add();
                    Word.Range minRange = minParagraph.Range;
                    minRange.Text = $"Наименее представленный тип — \"{minItem.TypeName}\" ({minItem.ProductCount} наим.)";
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
                if (_reportData == null || _reportData.Count == 0) return;

                var application = new Excel.Application();

                application.SheetsInNewWorkbook = _reportData.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                int grandTotal = 0;

                for (int i = 0; i < _reportData.Count; i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];

                    string typeName = _reportData[i].TypeName;
                    worksheet.Name = typeName.Length > 30 ? typeName.Substring(0, 30) : typeName;

                    worksheet.Cells[1, startRowIndex].Value = "ID Товара";
                    worksheet.Cells[2, startRowIndex].Value = "Наименование";
                    worksheet.Cells[3, startRowIndex].Value = "Базовая цена";
                    worksheet.Cells[4, startRowIndex].Value = "Текущий статус";
                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, startRowIndex], worksheet.Cells[4, startRowIndex]];
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Bold = true;
                    startRowIndex++;
                    string currentTypeName = _reportData[i].TypeName;
                    var localProducts = Core.Context.Products
                    .Where(p => p.ProductTypes.Name == currentTypeName)
                    .ToList();
                    foreach (var prod in localProducts)
                    {
                        worksheet.Cells[1, startRowIndex].Value = prod.ProductID;
                        worksheet.Cells[2, startRowIndex].Value = prod.Name;
                        worksheet.Cells[3, startRowIndex].Value = prod.Price;
                        (worksheet.Cells[3, startRowIndex] as Excel.Range).NumberFormat = "0.00";
                        worksheet.Cells[4, startRowIndex].Value = "В наличии";
                        startRowIndex++;
                    }
                    Excel.Range sumRange = worksheet.Range[worksheet.Cells[1, startRowIndex], worksheet.Cells[3, startRowIndex]];
                    sumRange.Merge();
                    sumRange.Value = "ИТОГО НАИМЕНОВАНИЙ:";
                    sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    worksheet.Cells[4, startRowIndex].Formula = $"=COUNTA(A2:A{startRowIndex - 1})";
                    sumRange.Font.Bold = worksheet.Cells[4, startRowIndex].Font.Bold = true;
                    grandTotal += _reportData[i].ProductCount;
                    Excel.Range borders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[4, startRowIndex]];
                    borders.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Columns.AutoFit();
                }
                Excel.Worksheet summarySheet = workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                summarySheet.Cells[1, 1].Value = "Общий итог ассортимента:";
                summarySheet.Cells[2, 1].Value = grandTotal;
                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[2, 1]];
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
    
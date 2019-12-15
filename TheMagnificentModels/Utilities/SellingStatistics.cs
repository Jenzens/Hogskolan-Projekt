using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;

namespace TheMagnificentModels.Utilities
{
    public static class SellingStatistics
    {

        #region Methods

        /// <summary>
        /// Get a list of all users statistics in format of UserMonthlySales depending on year
        /// </summary>
        /// <param name="year"></param>
        /// <returns>A list of UserMonthlySales</returns>
        public static List<UserMonthlySales> GetSellStatisticsOfYearAllUsers(int year)
        {
            var users = DataManager.GetUsersWithSale();

            List<UserMonthlySales> users_sales = new List<UserMonthlySales>();

            foreach (var seller in users)
            {
                var sales = seller.Sale.Where(x => x.InsuranceApplicationRows.First().Insurance.InsuranceName.Contains("Barn") || x.InsuranceApplicationRows.First().Insurance.InsuranceName.Contains("Vuxen"));
                if (sales.Count() > 0)
                    users_sales.Add(SortSalesPerMonth(seller, year));
            }

            return users_sales;
        }

        /// <summary>
        /// Get sell statistic of selected user & year
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="year"></param>
        /// <returns>A set of UserMonthlySales</returns>
        public static UserMonthlySales GetSellStatisticsOfYearOfUser(Users seller, int year)
        {
            seller = DataManager.GetUsersSale(seller);
            return SortSalesPerMonth(seller, year);
        }

        /// <summary>
        /// Sort sales per month
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="year"></param>
        /// <returns>return a "UserMonthlySales" struct with sorted sales per month</returns>
        public static UserMonthlySales SortSalesPerMonth(Users seller, int year)
        {
            List<SalePerMonth> sales = new List<SalePerMonth>();
            for (var i = 1; i <= 12; i++)
            {
                sales.Add(new SalePerMonth
                {
                    month = i,
                    sales = SellOfMonth(seller, i, year)
                });
            }
            return (new UserMonthlySales
            {
                user = seller,
                monthSales = sales
            });
        }

        /// <summary>
        /// Seperate all sales into a list per months instead
        /// </summary>
        /// <param name="user"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>A list of sales for the specific year and month of the users sales</returns>
        public static List<Sale> SellOfMonth(Users user, int month, int year)
        {
            return user.Sale.Where(x => x.StartDate?.Year == year && x.StartDate?.Month == month).ToList();
        }

        /// <summary>
        /// Get sale statistics of a single user for the specific year
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="year"></param>
        /// <returns>A list of monthly statistics in format of StatisticSalesPerMonth</returns>
        public static List<StatisticSalesPerMonth> GetSellStatisticsViewDataOfUser(Users seller, int year)
        {
            UserMonthlySales sales = GetSellStatisticsOfYearOfUser(seller, year);
            var acq = DataManager.GetInsuranceBaseValues();
            return GetSellStatisticsOfUserSales(sales, year, acq);
        }

        /// <summary>
        /// Calculate the selling statistics of a users sale and return
        /// </summary>
        /// <param name="sales"></param>
        /// <param name="year"></param>
        /// <param name="acq"></param>
        /// <returns>A list of StatisticSalesPerMonth calculations</returns>
        public static List<StatisticSalesPerMonth> GetSellStatisticsOfUserSales(UserMonthlySales sales, int year, List<Insurance> acq)
        {
            List<StatisticSalesPerMonth> sale_statistics = new List<StatisticSalesPerMonth>();

            var currentTime = DateTime.Now;
            string day = currentTime.Day.ToString();
            if (day.Length == 1)
                day = "0" + currentTime.Day.ToString();
            string month = currentTime.Month.ToString();
            if (month.Length == 1)
                month = "0" + currentTime.Month.ToString();
            long adult_time = long.Parse((currentTime.Year - 18) + month + day + "9999");

            foreach (var user_sale in sales.monthSales)
            {
                StatisticSalesPerMonth temp_earning = new StatisticSalesPerMonth
                {
                    Month = months[sale_statistics.Count]
                };
                foreach (var sale in user_sale.sales)
                {
                    if (!(sale.FirstAPItem.Insurance.InsuranceName.Contains("Barn") || sale.FirstAPItem.Insurance.InsuranceName.Contains("Vuxen"))) continue;
                    if (sale.InsuranceObject.PersonData.First().PersonNr > adult_time)
                    {
                        temp_earning.Child += (long)CommissionManager.GetAcqValueOfSale(sale, acq);
                    }
                    else
                    {
                        temp_earning.Adult += (long)CommissionManager.GetAcqValueOfSale(sale, acq);
                    }
                }
                sale_statistics.Add(temp_earning);
            }

            return sale_statistics;
        }

        /// <summary>
        /// Get sale statistics of a ALL "seller" users for the specific year
        /// </summary>
        /// <param name="year"></param>
        /// <returns>A list of sales for the specific year and month of the sellers</returns>
        public static List<StatisticSalesPerMonth> GetSellStatisticsViewDataAllUsers(int year)
        {
            List<UserMonthlySales> sales = GetSellStatisticsOfYearAllUsers(year);
            List<StatisticSalesPerMonth> sale_statistics = new List<StatisticSalesPerMonth>()
            {
                new StatisticSalesPerMonth{ Month = "Januari"   },
                new StatisticSalesPerMonth{ Month = "Februari"  },
                new StatisticSalesPerMonth{ Month = "Mars"      },
                new StatisticSalesPerMonth{ Month = "April"     },
                new StatisticSalesPerMonth{ Month = "Maj"       },
                new StatisticSalesPerMonth{ Month = "Juni"      },
                new StatisticSalesPerMonth{ Month = "Juli"      },
                new StatisticSalesPerMonth{ Month = "Augusti"   },
                new StatisticSalesPerMonth{ Month = "September" },
                new StatisticSalesPerMonth{ Month = "Oktober"   },
                new StatisticSalesPerMonth{ Month = "November"  },
                new StatisticSalesPerMonth{ Month = "December " }
            };
            var acq = DataManager.GetInsuranceBaseValues();

            foreach (var users_sales in sales)
            {
                var stats = GetSellStatisticsOfUserSales(users_sales, year, acq);
                for (var i = 0; i < stats.Count(); i++)
                {
                    sale_statistics[i].Child += (long)stats.ElementAt(i).Child;
                    sale_statistics[i].Adult += (long)stats.ElementAt(i).Adult;
                }
            }

            return sale_statistics;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public struct UserMonthlySales
        {
            public Users user;
            public List<SalePerMonth> monthSales;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public struct SalePerMonth
        {
            public int month;
            public List<Sale> sales;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public class StatisticSalesPerMonth
        {
            public long Total
            {
                get { return Child + Adult; }
            }
            public long Child { get; set; }
            public long Adult { get; set; }
            public string Month { get; set; }
        }

        #endregion

        #region Print / Export

        /// <summary>
        /// A list of all months to be able to convert them into "int" but also keeping the names for exporting
        /// </summary>
        private static List<string> months = new List<string>
        {
            "Januari",
            "Februari",
            "Mars",
            "April",
            "Maj",
            "Juni",
            "Juli",
            "Augusti",
            "September",
            "Oktober",
            "November",
            "December",
            "Året"
        };

        /// <summary>
        /// Export the statistics to an excel which the user can later print
        /// </summary>
        /// <param name="usersMonthlySales"></param>
        /// <param name="year"></param>
        public static void PrintAllUserSales(List<UserMonthlySales> usersMonthlySales, int year)
        {
            List<Earnings> earnings = new List<Earnings>();
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sell Statistics");
            var dataTable = AddTableColumns(usersMonthlySales);
            ws.Cell(1, 1).Value = year;
            ws.Cell(2, 1).Value = "Säljare";
            ws.Range(1, 1, 1, 2).Merge().AddToNamed("Titles");
            ws.Range(2, 1, 2, 2).Merge().AddToNamed("Säljare");
            ws.Range("C2", "AO2").AddToNamed("Säljposter");

            for (int i = 0; i < months.Count; i++)
            {
                ws.Cell(1, 3 + (i * 3)).Value = months[i];
                ws.Range(1, 3 + (i * 3), 1, 3 + (i * 3) + 2).Merge().AddToNamed("Titles");

                if (months[i] != "Året")
                {
                    ws.Cell(2, 3 + (i * 3)).Value = "Barn";
                    ws.Cell(2, 4 + (i * 3)).Value = "Vuxen";
                    ws.Cell(2, 5 + (i * 3)).Value = "Totalt";
                }
                else
                {
                    ws.Cell(2, 3 + (i * 3)).Value = "Totalt";
                    ws.Cell(2, 4 + (i * 3)).Value = "Medel";
                    ws.Cell(2, 5 + (i * 3)).Value = "BERÄKNING";
                }
            }

            var rangeWithData = ws.Cell(3, 1).InsertData(dataTable.AsEnumerable());

            var acq = DataManager.GetInsuranceBaseValues();
            foreach (var users_sales in usersMonthlySales)
            {
                var stats = GetSellStatisticsOfUserSales(users_sales, year, acq);
                Earnings temp_earning = new Earnings();
                for (var i = 0; i < stats.Count(); i++)
                {
                    temp_earning.child[i] += (long)stats.ElementAt(i).Child;
                    temp_earning.adult[i] += (long)stats.ElementAt(i).Adult;
                }
                earnings.Add(temp_earning);
            }

            long ackmedeltotal = 0;
            List<long> medels = new List<long>();
            foreach (var earning in earnings)
            {
                long sale_months = 0;
                long ack_medel = 0;
                for (var i = 0; i < earning.child.Length; i++)
                {
                    if (earning.child[i] + earning.adult[i] > 0)
                    {
                        sale_months += 1;
                        ack_medel += earning.child[i] + earning.adult[i];
                    }
                }
                if (sale_months == 0)
                {
                    medels.Add(0);
                    ackmedeltotal += 0;
                }
                else
                {
                    medels.Add(ack_medel / sale_months);
                    ackmedeltotal += ack_medel / sale_months;
                }
            }

            if (earnings.Count > 0)
            {

                ackmedeltotal = ackmedeltotal / earnings.Count; // total ack / säljare
                ws.Cell(2, 5 + (12 * 3)).Value = ackmedeltotal;

                for (var i = 2; i <= ws.RowsUsed().Count(); i++)
                {
                    foreach (var cell in ws.Row(i).Cells())
                    {
                        if (i >= 3)
                        {
                            ws.Range(i, 1, i, 2).Merge().AddToNamed("SäljarensNamn");
                            for (int j = 0; j < 12; j++)
                            {
                                ws.Cell(i, 3 + (j * 3)).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                ws.Cell(i, 4 + (j * 3)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell(i, 5 + (j * 3)).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                if (earnings[i - 3].child[j] + earnings[i - 3].adult[j] > 0)
                                {
                                    ws.Cell(i, 3 + (j * 3)).Value = earnings[i - 3].child[j];
                                    ws.Cell(i, 4 + (j * 3)).Value = earnings[i - 3].adult[j];
                                    ws.Cell(i, 5 + (j * 3)).Value = earnings[i - 3].child[j] + earnings[i - 3].adult[j];
                                }
                            }

                            long total = 0;
                            for (int k = 0; k < 12; k++)
                            {
                                total += earnings[i - 3].adult[k] + earnings[i - 3].child[k];
                            }
                            ws.Cell(i, 3 + (12 * 3)).Value = total;
                            ws.Cell(i, 4 + (12 * 3)).Value = medels[i - 3]; // total_medel;
                            ws.Cell(i, 5 + (12 * 3)).Value = medels[i - 3] - ackmedeltotal; // total_medel - ackmedeltotal;

                        }
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }
                }

                // Format all titles in one shot
                wb.NamedRanges.NamedRange("Titles").Ranges.Style.Font.Bold = true;
                wb.NamedRanges.NamedRange("Titles").Ranges.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.NamedRanges.NamedRange("Titles").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                wb.NamedRanges.NamedRange("SäljarensNamn").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                wb.NamedRanges.NamedRange("Säljare").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                wb.NamedRanges.NamedRange("Säljposter").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                ws.Columns().AdjustToContents();

                long date = DateTime.Now.Ticks;

                wb.SaveAs(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/SellStatistics" + date + ".xlsx");
                Process.Start(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/SellStatistics" + date + ".xlsx");
            }
        }

        /// <summary>
        /// Adding table columns to a Datatable
        /// </summary>
        /// <param name="userMonthlySales"></param>
        /// <returns>Returns the generated Datatable</returns>
        private static System.Data.DataTable AddTableColumns(List<UserMonthlySales> userMonthlySales)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("Name", typeof(string));
            foreach (var user_sale in userMonthlySales)
            {
                table.Rows.Add(user_sale.user.Firstname + " " + user_sale.user.Lastname);
            }
            return table;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        private class Earnings
        {
            public long[] adult = {
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0
            };
            public long[] child = {
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0
            };
        }

        #endregion

        #region Trend

        /// <summary>
        /// Generate a trend of sells of the user of the incoming year and start the program
        /// </summary>
        /// <param name="user"></param>
        /// <param name="year"></param>
        /// <returns>A message to popup in the UI</returns>
        public static string GenerateTrend(Users user, int year)
        {
            try
            {
                //Start Excel 
                Application app = new Application();
                Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet); // generate a new work book
                Worksheet ws = (Worksheet)app.ActiveSheet; // set activesheet

                //Generate diagram range
                Range chartRange;
                ChartObjects xlCharts = (ChartObjects)ws.ChartObjects(Type.Missing);
                ChartObject myChart = (ChartObject)xlCharts.Add(200, 200, 1000, 600);
                Chart chartPage = myChart.Chart;

                chartRange = ws.get_Range("A1", "P3");
                chartPage.SetSourceData(chartRange);
                chartPage.ChartType = XlChartType.xlColumnClustered;

                ws.Cells[1, 1] = year; // set year
                ws.Cells[2, 1] = "Säljare";
                ws.Cells[3, 1] = user.UserIdAndName;

                Trend trend = GetAcqValues(user, year);

                int row = 3;
                foreach (TrendLine line in trend.Lines.Values)
                {
                    for (int month = 1; month <= 12; month++)
                    {
                        ws.Cells[row, month + 2] = line.MonthAcqValue["Totalt"][month];
                    }
                    row++;
                }

                ////rad 1
                ws.Cells[1, 3] = "Januari";
                ws.Cells[1, 4] = "Februari";
                ws.Cells[1, 5] = "Mars";
                ws.Cells[1, 6] = "April";
                ws.Cells[1, 7] = "Maj";
                ws.Cells[1, 8] = "Juni";
                ws.Cells[1, 9] = "Juli";
                ws.Cells[1, 10] = "Augusti";
                ws.Cells[1, 11] = "September";
                ws.Cells[1, 12] = "Oktober";
                ws.Cells[1, 13] = "November";
                ws.Cells[1, 14] = "December";

                ////rad 2
                int k = 0;
                int j = 0;
                while (j++ <= 11)
                {
                    ws.Cells[2, 3 + k] = "Totalt";
                    k += 1;
                }

                Series series = chartPage.FullSeriesCollection(2);
                Trendlines trendlines = series.Trendlines();
                var x = trendlines.Add(XlTrendlineType.xlMovingAvg);
                series.Name = "Trend Linje";

                app.Visible = true;

                return "Trend genererad, excel filen kommer att visas";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Trenden kunde inte generas eller startas, är Excel kanske inte är installerat på din dator.";
            }
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public class AcqValues
        {
            public int Month { get; set; }
            public double AcqValueSum { get; set; }
        }

        /// <summary>
        /// Get acq values for the user of the year into a new trend
        /// </summary>
        /// <param name="user"></param>
        /// <param name="year"></param>
        /// <returns>Returns an object of "Trend"</returns>
        public static Trend GetAcqValues(Users user, int year)
        {
            Trend trend = new Trend();
            foreach (AcqValues a in GetAllAcqValues(user, year))
                trend.Add(user.AgentNumber, a.Month, (int)Math.Round(a.AcqValueSum));
            return trend;
        }

        /// <summary>
        /// Generate a list of acq values
        /// </summary>
        /// <param name="user"></param>
        /// <param name="year"></param>
        /// <returns>Returns a list of acq values</returns>
        public static List<AcqValues> GetAllAcqValues(Users user, int year)
        {
            List<AcqValues> AcqValues = new List<AcqValues>();
            var stats = GetSellStatisticsViewDataOfUser(user, year);
            for (var i = 0; i < stats.Count(); i++)
            {
                AcqValues acqValue = new AcqValues()
                {
                    Month = i+1,
                    AcqValueSum = stats.ElementAt(i).Total
                };
                AcqValues.Add(acqValue);
            }
            return AcqValues;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public class Trend
        {
            public Dictionary<int, TrendLine> Lines     { get; private set; }

            public Trend()
            {
                Lines = new Dictionary<int, TrendLine>();
            }

            public void Add(int agentNr, int month, double acqValue)
            {
                TrendLine line;
                if (!Lines.TryGetValue(agentNr, out line))
                {
                    line = new TrendLine(agentNr);
                    Lines.Add(agentNr, line);
                }

                line.Add(month, acqValue);
            }
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public class TrendLine
        {
            public int AgentNumber { get; private set; }

            public Dictionary<string, Dictionary<int, double>> MonthAcqValue { get; private set; }

            public TrendLine(int _agentNr)
            {
                AgentNumber = _agentNr;
                MonthAcqValue = new Dictionary<string, Dictionary<int, double>>();
                Dictionary<int, double> monthAcqValueTotal = new Dictionary<int, double>();
                for (int i = 1; i <= 12; i++)
                {
                    monthAcqValueTotal.Add(i, 0.0);
                }
                MonthAcqValue.Add("Totalt", monthAcqValueTotal);
            }

            public void Add(int month, double acqValue)
            {
                MonthAcqValue["Totalt"][month] += acqValue;
            }

        }

        #endregion
    }
}

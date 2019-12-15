using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentModels.Utilities
{
    public static class CommissionManager
    {

        /// <summary>
        /// Gets the collected value of Acqsum of the insurance type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetUserAcqSumOfInsuranceType(string type, Users user, int month, int year)
        {
            var sum = 0.0;
            var acq = DataManager.GetInsuranceBaseValues();
            if (user.Sale.Count > 0)
            {
                if (type == "other")
                {
                    var fixed_sales = user.Sale.Where(x => x.InsuranceFixedComision != null && x.StartDate?.Year == year && x.StartDate?.Month == month);
                    foreach (var sale in fixed_sales)
                    {
                        sum += (double)sale.InsuranceFixedComision.Commision;
                    }
                }
                else
                {
                    var sales = user.Sale.Where(x => x.FirstAPItem.Insurance.InsuranceName.Contains(type) && x.StartDate?.Year == year && x.StartDate?.Month == month);
                    foreach (var sale in sales)
                    {
                        sum += (double)GetAcqValueOfSale(sale, acq);
                    }
                }
            }
            return (int) Math.Round(sum);
        }

        /// <summary>
        /// Gets the collected value of LifeAcqValues
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int GetUserLifeBaseSum(Users user, int month, int year)
        {
            var sum = 0;
            var acq = DataManager.GetInsuranceBaseValues();
            if (user.Sale.Count > 0)
            {
                var queryFirst = user.Sale.Where(x => x.FirstAPItem.Insurance.InsuranceName.Contains("Liv") && x.StartDate?.Year == year && x.StartDate?.Month == month);
                foreach (var item in queryFirst)
                {
                    if (item.InsuranceFixedComision != null)
                    {
                        continue;
                    }

                    sum += item.FirstAPItem.BaseValue.Value;
                }

                var querySecond = user.Sale.Where(x => x.FirstAPItem.Insurance.InsuranceName.Contains("Liv") && x.StartDate?.Year == year && x.StartDate?.Month == month);
                foreach (var item in querySecond)
                {
                    if (item.InsuranceFixedComision != null || item.SecondAPItem == null)
                    {
                        continue;
                    }

                    sum += item.SecondAPItem.BaseValue.Value;
                }
            }
            return sum;
        }

        /// <summary>
        /// gets the acq values based on sale and acqvalues
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="acq"></param>
        /// <returns></returns>
        public static double GetAcqValueOfSale(Sale sale, List<Insurance> acq)
        {
            if (sale.InsuranceFixedComision != null)
            {
                return 0;
            }

            if (acq.Where(x => x.InsuranceId == sale.FirstAPItem.InsuranceId).Count() == 0) // Om det är en företagsförsäkring, DOCK borde den då ha en "fixedcomision"
                                                                                            // Kanske n fix för sale att göra att man måste välja en fixedcomision vid val av företagsförsäkring?
                return 0;

            double avk = 0.0;

            avk += GetAcqValueOfInsuranceAppRow(sale.FirstAPItem, acq, sale.StartDate.Value.Year);

            if (sale.SecondAPItem == null) { // no addon, return avk
                return avk;
            }

            avk += GetAcqValueOfInsuranceAppRow(sale.SecondAPItem, acq, sale.StartDate.Value.Year);

            return avk;
        }

        /// <summary>
        /// Calculate the acqvalue of an insurance app row
        /// </summary>
        /// <param name="ap_row"></param>
        /// <param name="acq"></param>
        /// <param name="year"></param>
        /// <returns>returns a value</returns>
        private static double GetAcqValueOfInsuranceAppRow(InsuranceApplicationRow ap_row, List<Insurance> acq, int year)
        {
            double avk = 0.0;
            var b = ap_row.BaseValue.Value;
            var a = acq.Where(x => x.InsuranceId == ap_row.InsuranceId).First();
            var bv = a.InsuranceBaseValues.Where(x => x.BaseValue.Value == b && x.StartDate == year);
            var av = a.InsuranceAcqVariables.Where(x => x.Startdate == year);

            if (av.Count() > 0)
            {
                avk += ((double)b * av.First().AcqVariable);
            }
            else if (bv.Count() > 0)
            {
                avk += (double)bv.First().AcqValue;
            }
            return avk;
        }

    }
}

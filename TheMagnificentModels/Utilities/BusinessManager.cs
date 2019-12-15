using System;
using System.Collections.Generic;
using static TheMagnificentModels.Utilities.SellingStatistics;

namespace TheMagnificentModels.Utilities
{
    public class BusinessManager
    {
        //Test
        #region User methods

        /// <summary>
        /// Try perform a login
        /// </summary>
        /// <param name="agentNumber"></param>
        /// <param name="password"></param>
        /// <returns>On success returns a user or else null</returns>
        public Users Login(int agentNumber, string password)
        {
            return Users.LoginUser(agentNumber, password);
        }

        /// <summary>
        /// Try create a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>List of errors or if operation succeeds; an empty list</returns>
        public List<string> CreateNewUser(Users user)
        {
            return user.RegisterNewUser();
        }

        /// <summary>
        /// Try update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>List of errors or if operation succeeds; an empty list</returns>
        public List<string> UpdateUser(Users user)
        {
            return user.UpdateUser();
        }

        /// <summary>
        /// Update a password if the user knows his old
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <param name="confirmpassword"></param>
        /// <returns>Result of update</returns>
        public string ChangeUserPassword(Users user, string oldpassword, string newpassword, string confirmpassword)
        {
            return user.ChangePassword(oldpassword, newpassword, confirmpassword);
        }

        /// <summary>
        /// Force change a password without old PW, used for admins
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newpassword"></param>
        /// <param name="confirmpassword"></param>
        /// <returns>Result of update</returns>
        public string ChangeUserPassword(Users user, string newpassword, string confirmpassword)
        {
            return user.ChangePassword(newpassword, confirmpassword);
        }

        /// <summary>
        /// Get a list of sellers
        /// </summary>
        /// <returns>Return a list of all users with the role "Säljare"</returns>
        public List<Users> GetSellers()
        {
            var sellers = DataManager.GetUsers();
            List<Users> users = new List<Users>();
            foreach(var item in sellers)
            {
                if (item.IsUserAuthorized("säljare"))
                    users.Add(item);
            }
            return users;
        }

        #endregion

        #region Sale 

        /// <summary>
        /// Try register a sale
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <param name="insuranceTaker"></param>
        /// <param name="insuranceObject"></param>
        /// <returns>List of errors or empty if success</returns>
        public List<string> CreateSale(Users user, Sale sale, List<InsuranceApplicationRow> appRows, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            return sale.RegisterSale(user, appRows, insuranceTaker, insuranceObject);
        }

        /// <summary>
        /// Try update the sale data
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns>List of errors or empty if success</returns>
        public List<string> UpdateSale(Users user, Sale sale, List<InsuranceApplicationRow> appRows, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            return sale.UpdateSale(user, appRows, insuranceTaker, insuranceObject);
        }

        #endregion

        #region Print Statistics 
        /// <summary>
        /// Method for printing the statistics of a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="year"></param>
        public void PrintStatisticOfUser(Users user, int year)
        {
            List<UserMonthlySales> print_data = new List<UserMonthlySales>
            {
                GetSellStatisticsOfYearOfUser(user, year)
            };
            PrintAllUserSales(print_data, year);
        }
        /// <summary>
        /// Method for printing the statistics of several users
        /// </summary>
        /// <param name="year"></param>
        public void PrintStatisticOfAllUsers(int year)
        {
            List<UserMonthlySales> print_data = GetSellStatisticsOfYearAllUsers(year);
            PrintAllUserSales(print_data, year);
        }

        #endregion

        #region Vaction Rate

        /// <summary>
        /// Try register or update a vacation rate
        /// </summary>
        /// <param name="rate"></param>
        /// <returns>List of errors or empty if success</returns>
        public List<string> TryRegisterVacationRate(VacationRate rate)
        {
            return rate.TryRegister();
        }

        #endregion 

        #region Get Statistics 

        /// <summary>
        /// Method for getting the statistics of a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="year"></param>
        /// <returns>Returns a list of StatisticSalesPerMonth</returns>
        public List<StatisticSalesPerMonth> GetStatisticsOfUser(Users user, int year)
        {
            return GetSellStatisticsViewDataOfUser(user, year);
        }

        /// <summary>
        /// Method for getting the statistics of all users
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Returns a list of StatisticSalesPerMonth</returns>
        public List<StatisticSalesPerMonth> GetStatisticsOfAllUsers(int year)
        {
            return GetSellStatisticsViewDataAllUsers(year);
        }

        #endregion

        #region Base values 

        /// <summary>
        /// Try register or update base values
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns>List of errors or if operation succeeds; an empty list</returns>
        public List<string> TryRegisterBaseValues(Insurance insurance, InsuranceBaseValues insuranceBaseValues, InsuranceAcqVariables insuranceAcqVariables)
        {
            List<string> errors = new List<string>();

            if (insurance == null)
                errors.Add("Du måste ha valt en försäkring.");

            if (insuranceBaseValues != null)
            {
                if (insuranceBaseValues.StartDate < 0)
                    errors.Add("Kalenderåret måste vara efter kristus");

                if (insuranceBaseValues.BaseValue.HasValue)
                    if (insuranceBaseValues.BaseValue < 0)
                        errors.Add("Grundbeloppet får inte vara lägre än 0");

                if (insuranceBaseValues.AcqValue.HasValue)
                    if (insuranceBaseValues.AcqValue < 0)
                        errors.Add("Ack. Värde får inte vara lägre än 0");
            }

            if (insuranceAcqVariables != null)
            {
                if (insuranceAcqVariables.Startdate < 0)
                    errors.Add("Kalenderåret måste vara efter kristus");

                if (insuranceAcqVariables.AcqVariable < 0)
                    errors.Add("Ack. Värde får inte vara lägre än 0");
            }

            if (insuranceAcqVariables != null || insuranceBaseValues != null)
            {
                if (insuranceAcqVariables != null)
                {
                    if (insuranceAcqVariables.Startdate == 0)
                        errors.Add("Ett kalenderår måste finnas.");
                }
                else
                {
                    if (insuranceBaseValues.StartDate == 0)
                        errors.Add("Ett kalenderår måste finnas.");
                }
            }

            if (insuranceAcqVariables != null && insuranceBaseValues != null)
            {
                if (insuranceBaseValues.AcqValue.HasValue && insuranceBaseValues.AcqValue.Value > 0 && insuranceAcqVariables.AcqVariable > 0)
                    errors.Add("Det kan inte finnas ett akv värde och en acq variabel");
            }

            if (insuranceBaseValues == null && insuranceAcqVariables == null)
                errors.Add("Error....");

            if (errors.Count > 0)
                return errors;

            if (insuranceBaseValues != null)
            {
                if (insuranceBaseValues.BaseValue.HasValue || insuranceBaseValues.AcqValue.HasValue)
                {
                    if (insuranceBaseValues.Id > 0)
                    {
                        DataManager.UpdateBaseValues(insurance, insuranceBaseValues);
                    }
                    else
                    {
                        DataManager.AddBaseValues(insurance, insuranceBaseValues);
                    }
                }
            }

            if (insuranceAcqVariables != null)
            {
                if (insuranceAcqVariables.AcqVariable >= 0 && (insuranceBaseValues.AcqValue.HasValue && insuranceBaseValues.AcqValue <= 0 || !insuranceBaseValues.AcqValue.HasValue))
                {
                    if (insuranceAcqVariables.Id > 0)
                    {
                        DataManager.UpdateAcqVariables(insurance, insuranceAcqVariables);
                    }
                    else if ((insuranceBaseValues.AcqValue.HasValue && insuranceBaseValues.AcqValue <= 0 || !insuranceBaseValues.AcqValue.HasValue))
                    {
                        DataManager.AddAcqVariables(insurance, insuranceAcqVariables);
                    }
                }
                else if (insuranceAcqVariables.Id > 0 && insuranceBaseValues.AcqValue.HasValue)
                {
                    DataManager.RemoveAcqVariable(insurance, insuranceAcqVariables);
                }
            }

            return errors;
        }

        #endregion

        #region Commission Values 
        /// <summary>
        /// Method for registering new commission values
        /// </summary>
        /// <param name="commissionValue"></param>
        /// <returns>List of errors or if operation succeeds; an empty list</returns>
        public List<string> TryRegisterCommissionValues(CommisionValues commissionValue)
        {
            return commissionValue.RegisterCommissionValues();
        }

        #endregion
    }
}

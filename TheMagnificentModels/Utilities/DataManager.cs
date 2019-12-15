using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMagnificentModels;

namespace TheMagnificentModels.Utilities
{
    public static class DataManager
    {
        #region User

        /// <summary>
        /// Get a user of agentnumber
        /// </summary>
        /// <param name="agentNumber"></param>
        /// <returns>Returns a user if found</returns>
        public static Users GetUser(int agentNumber)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Where(x => x.AgentNumber == agentNumber);

                if (query.Count() > 0)
                {
                    var user = new Users();
                    user = query.First();
                    user.Roles = query.First().Roles;
                    user.ZipCity = query.First().ZipCity;
                    return user;
                }
                else
                {
                    return null;
                }
                
            }
        }

        /// <summary>
        /// Get a list of all users
        /// </summary>
        /// <returns>A list of Users</returns>
        public static List<Users> GetUsers()
        {
            var userList = new List<Users>();
            using(var db = new DatabaseContext())
            {
                if (db.Users.Count() > 0)
                {
                    foreach (var item in db.Users)
                    {
                        var user = new Users();
                        user = item;
                        user.Roles = item.Roles;
                        user.ZipCity = item.ZipCity;
                        userList.Add(user);
                    }
                    return userList;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get all user with all of their sales
        /// </summary>
        /// <returns>A list of user with all their sale data</returns>
        public static List<Users> GetUsersWithSale()
        {
            var userList = new List<Users>();
            using (var db = new DatabaseContext())
            {
                if (db.Users.Count() > 0)
                {
                    foreach (var item in db.Users)
                    {
                        var user = new Users();
                        user = item;
                        user.Roles = item.Roles;
                        if (!user.IsUserAuthorized("säljare"))
                            continue;
                        user.Sale = item.Sale;
                        user.ZipCity = user.ZipCity;
                        foreach (var sale in item.Sale)
                        {
                            sale.InsuranceApplicationRows = sale.InsuranceApplicationRows;
                            foreach (var ap_row in sale.InsuranceApplicationRows)
                            {
                                ap_row.Insurance = ap_row.Insurance;
                                ap_row.Insurance.Insurance1 = ap_row.Insurance.Insurance1;
                                ap_row.Insurance.Insurance2 = ap_row.Insurance.Insurance2;
                                ap_row.Insurance.InsuranceBaseValues = ap_row.Insurance.InsuranceBaseValues;
                                ap_row.Insurance.InsuranceAcqVariables = ap_row.Insurance.InsuranceAcqVariables;
                            }
                            sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                            sale.InsuranceObject = sale.InsuranceObject;
                            sale.InsuranceObject.PersonData = sale.InsuranceObject.PersonData;

                            sale.InsuranceTaker = sale.InsuranceTaker;
                            if (sale.InsuranceTaker.PersonData.Count > 0)
                            {
                                sale.InsuranceTaker.PersonData.First().ZipCity = sale.InsuranceTaker.PersonData.First().ZipCity;
                                sale.InsuranceTaker.PersonData.First().InsuranceTaker = sale.InsuranceTaker.PersonData.First().InsuranceTaker;
                            }

                            if (sale.InsuranceTaker.CompanyData.Count > 0)
                            {
                                sale.InsuranceTaker.CompanyData.First().CompanyContactData = sale.InsuranceTaker.CompanyData.First().CompanyContactData;
                                sale.InsuranceTaker.CompanyData.First().ZipCity = sale.InsuranceTaker.CompanyData.First().ZipCity;
                                sale.InsuranceTaker.CompanyData.First().InsuranceTaker = sale.InsuranceTaker.CompanyData.First().InsuranceTaker;
                            }

                            sale.PaymentTypes = sale.PaymentTypes;
                            sale.Users = sale.Users;
                        }
                        userList.Add(user);
                    }
                    return userList;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get sales of a user
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>Returns a user with sale data in it</returns>
        public static Users GetUsersSale(Users agent)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Users.Where(x => x.AgentNumber == agent.AgentNumber);
                if (query.Count() == 0) return null;
                var user = query.First();
                user.Roles = user.Roles;
                if (!user.IsUserAuthorized("säljare")) return null;
                user.Sale = user.Sale;
                foreach (var sale in user.Sale)
                {
                    sale.InsuranceApplicationRows = sale.InsuranceApplicationRows;
                    foreach (var ap_row in sale.InsuranceApplicationRows)
                    {
                        ap_row.Insurance = ap_row.Insurance;
                        ap_row.Insurance.Insurance1 = ap_row.Insurance.Insurance1;
                        ap_row.Insurance.Insurance2 = ap_row.Insurance.Insurance2;
                        ap_row.Insurance.InsuranceBaseValues = ap_row.Insurance.InsuranceBaseValues;
                        ap_row.Insurance.InsuranceAcqVariables = ap_row.Insurance.InsuranceAcqVariables;
                    }
                    sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                    sale.InsuranceObject = sale.InsuranceObject;
                    sale.InsuranceObject.PersonData = sale.InsuranceObject.PersonData;

                    sale.InsuranceTaker = sale.InsuranceTaker;
                    if (sale.InsuranceTaker.PersonData.Count > 0)
                    {
                        sale.InsuranceTaker.PersonData.First().ZipCity = sale.InsuranceTaker.PersonData.First().ZipCity;
                        sale.InsuranceTaker.PersonData.First().InsuranceTaker = sale.InsuranceTaker.PersonData.First().InsuranceTaker;
                    }

                    if (sale.InsuranceTaker.CompanyData.Count > 0)
                    {
                        sale.InsuranceTaker.CompanyData.First().CompanyContactData = sale.InsuranceTaker.CompanyData.First().CompanyContactData;
                        sale.InsuranceTaker.CompanyData.First().ZipCity = sale.InsuranceTaker.CompanyData.First().ZipCity;
                        sale.InsuranceTaker.CompanyData.First().InsuranceTaker = sale.InsuranceTaker.CompanyData.First().InsuranceTaker;
                    }

                    sale.PaymentTypes = sale.PaymentTypes;
                    sale.Users = sale.Users;
                }
                return user;
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddUser(Users user)
        {
            using(var db = new DatabaseContext())
            {
                if (ZipCityExist(user.Zipcode))
                    user.ZipCity = null;
                else
                    user.ZipCity.Zipcode = user.Zipcode;

                var q = db.Roles.Find(user.Roles.First().PermissionId); // Hämta rollen för contexten..

                user.Roles.Clear(); // ta bort den gamla icke none db context rollen
                user.Roles.Add(q);  // lägg till db context rollen så vi
                                    // 1. inte får error
                                    // 2. inte skapar rollen på nytt för att vi ÅTERANVÄNDER en gammal (idiot EF system) 

                db.Users.Add(user);
                try
                {
                    return db.SaveChanges() > 0;
                }
                catch (DbUpdateException ex) //DbContext
                {
                    Console.WriteLine(ex.InnerException);
                    return false;
                } catch(Exception) {
                    return false;
                }
            }
        }

        /// <summary>
        /// Updating an existing user based on "Users.AgentNumber"
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateUser(Users user)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Find(user.AgentNumber);
                try
                {
                    if (!ZipCityExist(user.Zipcode))
                    {
                        query.ZipCity = new ZipCity
                        {
                            Zipcode = user.Zipcode,
                            City = user.ZipCity.City
                        };
                    }
                    query.Email = user.Email;
                    query.Firstname = user.Firstname;
                    query.Lastname = user.Lastname;
                    query.Phonenumber = user.Phonenumber;
                    query.StreetAdress = user.StreetAdress;
                    query.Taxrate = user.Taxrate;
                    query.Zipcode = user.Zipcode;
                    query.Password = user.Password;

                    // remove old roles
                    foreach (var role in query.Roles)
                    {
                        RemoveRoleFromUser(query, role);
                    }

                    // add the new roles 
                    foreach (var role in user.Roles)
                    {
                        AddRoleToUser(query, role);
                    }

                    db.SaveChanges();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion //User

        #region User Roles

        /// <summary>
        /// Returning a list of all existing roles
        /// </summary>
        /// <returns>A list of roles</returns>
        public static List<Roles> GetRoles()
        {
            using(var db = new DatabaseContext())
            {
                if(db.Roles.Count() > 0)
                {
                    return db.Roles.ToList();
                }
                return null;
            }
        }

        /// <summary>
        /// Adding a role to an existing user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddRoleToUser(Users user, Roles role)
        {
            using(var db = new DatabaseContext())
            {
                var userquery = db.Users.Where(x => x.AgentNumber == user.AgentNumber).First();
                var rolequery = db.Roles.Where(x => x.PermissionId == role.PermissionId).First();
                userquery.Roles.Add(rolequery);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("?1? " + ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Removing a role of an existing user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>bool of success or failure removal</returns>
        public static bool RemoveRoleFromUser(Users user, Roles role)
        {
            using(var db = new DatabaseContext())
            {
                var userquery = db.Users.Where(x => x.AgentNumber == user.AgentNumber).First();
                var rolequery = db.Roles.Where(x => x.PermissionId == role.PermissionId).First();
                if (userquery.Roles.Where(x => x.PermissionId == role.PermissionId).Count() > 0)
                    userquery.Roles.Remove(rolequery);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("?2? " + ex);
                    return false;
                }
            }
        }

        #endregion

        #region Sale

        /// <summary>
        /// Get a list of sales
        /// </summary>
        /// <returns>A list of sales</returns>
        public static List<Sale> GetSales()
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Sale;
                if(query.Count() > 0)
                {
                    foreach (var sale in query)
                    {
                        sale.InsuranceApplicationRows = sale.InsuranceApplicationRows;
                        foreach ( var ap_row in sale.InsuranceApplicationRows )
                        {
                            ap_row.Insurance = ap_row.Insurance;
                            ap_row.Insurance.Insurance1 = ap_row.Insurance.Insurance1;
                            ap_row.Insurance.Insurance2 = ap_row.Insurance.Insurance2;
                        }
                        sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                        sale.InsuranceObject = sale.InsuranceObject;
                        sale.InsuranceObject.PersonData = sale.InsuranceObject.PersonData;

                        sale.InsuranceTaker = sale.InsuranceTaker;
                        if (sale.InsuranceTaker.PersonData.Count > 0)
                        {
                            sale.InsuranceTaker.PersonData.First().ZipCity = sale.InsuranceTaker.PersonData.First().ZipCity;
                        }

                        sale.InsuranceTaker.CompanyData = sale.InsuranceTaker.CompanyData;
                        if (sale.InsuranceTaker.CompanyData.Count > 0)
                        {
                            sale.InsuranceTaker.CompanyData.First().CompanyContactData = sale.InsuranceTaker.CompanyData.First().CompanyContactData;
                            sale.InsuranceTaker.CompanyData.First().ZipCity = sale.InsuranceTaker.CompanyData.First().ZipCity;
                        }

                        sale.InsuranceTaker.PersonData = sale.InsuranceTaker.PersonData;
                        sale.PaymentTypes = sale.PaymentTypes;
                        sale.Users = sale.Users;
                    }
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get Sales for Customer ID
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>List of sales associated with Customer ID</returns>
        public static List<Sale> GetSalesWithData(int insurance_taker_id)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Sale.Where(x => x.InsuranceTakerId == insurance_taker_id);

                if (query.Count() > 0)
                {
                    foreach (var sale in query)
                    {
                        sale.InsuranceApplicationRows = sale.InsuranceApplicationRows;
                        foreach ( var ap_row in sale.InsuranceApplicationRows )
                        {
                            ap_row.Insurance = ap_row.Insurance;
                            ap_row.Insurance.Insurance1 = ap_row.Insurance.Insurance1;
                            ap_row.Insurance.Insurance2 = ap_row.Insurance.Insurance2;
                            ap_row.Insurance.InsuranceBaseValues = ap_row.Insurance.InsuranceBaseValues;
                            ap_row.Insurance.InsuranceAcqVariables = ap_row.Insurance.InsuranceAcqVariables;
                        }
                        sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                        sale.InsuranceObject = sale.InsuranceObject;
                        sale.InsuranceObject.PersonData = sale.InsuranceObject.PersonData;

                        sale.InsuranceTaker = sale.InsuranceTaker;
                        if (sale.InsuranceTaker.PersonData.Count > 0)
                        {
                            sale.InsuranceTaker.PersonData.First().ZipCity = sale.InsuranceTaker.PersonData.First().ZipCity;
                            sale.InsuranceTaker.PersonData.First().InsuranceTaker = sale.InsuranceTaker.PersonData.First().InsuranceTaker;
                        }
                        
                        if (sale.InsuranceTaker.CompanyData.Count > 0)
                        {
                            sale.InsuranceTaker.CompanyData.First().CompanyContactData = sale.InsuranceTaker.CompanyData.First().CompanyContactData;
                            sale.InsuranceTaker.CompanyData.First().ZipCity = sale.InsuranceTaker.CompanyData.First().ZipCity;
                            sale.InsuranceTaker.CompanyData.First().InsuranceTaker = sale.InsuranceTaker.CompanyData.First().InsuranceTaker;
                        }

                        sale.PaymentTypes = sale.PaymentTypes;
                        sale.Users = sale.Users;
                    }

                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a users sales with all needed data
        /// </summary>
        /// <param name="agent_id"></param>
        /// <returns>A list of the users sale</returns>
        public static List<Sale> GetAgentSalesWithData(int agent_id)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Sale.Where(x => x.AgentId == agent_id);

                if (query.Count() > 0)
                {
                    foreach (var sale in query)
                    {
                        sale.InsuranceApplicationRows = sale.InsuranceApplicationRows;
                        foreach (var ap_row in sale.InsuranceApplicationRows)
                        {
                            ap_row.Insurance = ap_row.Insurance;
                            ap_row.Insurance.Insurance1 = ap_row.Insurance.Insurance1;
                            ap_row.Insurance.Insurance2 = ap_row.Insurance.Insurance2;
                            ap_row.Insurance.InsuranceBaseValues = ap_row.Insurance.InsuranceBaseValues;
                            ap_row.Insurance.InsuranceAcqVariables = ap_row.Insurance.InsuranceAcqVariables;
                        }
                        sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                        sale.InsuranceObject = sale.InsuranceObject;
                        sale.InsuranceObject.PersonData = sale.InsuranceObject.PersonData;

                        sale.InsuranceTaker = sale.InsuranceTaker;
                        if (sale.InsuranceTaker.PersonData.Count > 0)
                        {
                            sale.InsuranceTaker.PersonData.First().ZipCity = sale.InsuranceTaker.PersonData.First().ZipCity;
                            sale.InsuranceTaker.PersonData.First().InsuranceTaker = sale.InsuranceTaker.PersonData.First().InsuranceTaker;
                        }

                        if (sale.InsuranceTaker.CompanyData.Count > 0)
                        {
                            sale.InsuranceTaker.CompanyData.First().CompanyContactData = sale.InsuranceTaker.CompanyData.First().CompanyContactData;
                            sale.InsuranceTaker.CompanyData.First().ZipCity = sale.InsuranceTaker.CompanyData.First().ZipCity;
                            sale.InsuranceTaker.CompanyData.First().InsuranceTaker = sale.InsuranceTaker.CompanyData.First().InsuranceTaker;
                        }

                        sale.PaymentTypes = sale.PaymentTypes;
                        sale.Users = sale.Users;
                    }

                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Register a new sale
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="user"></param>
        /// <param name="insuranceTaker"></param>
        /// <param name="insuranceObject"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool CreateSale(Sale sale, List<InsuranceApplicationRow> appRows, Users user, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            using (var db = new DatabaseContext())
            {
                sale.AgentId = user.AgentNumber;

                if (sale.InsuranceFixedComision != null)
                {
                    sale.InsuranceFixedComision.InsuranceApplicationId = sale.InsuranceApplicationId;
                    sale.InsuranceFixedComision.InsuranceApplicationCN = sale.InsuranceCompany;
                }

                if (insuranceTaker.PersonData.Count > 0)
                {
                    insuranceTaker.PersonData.First().ZipCity.Zipcode = (int)insuranceTaker.PersonData.First().Zipcode;

                    var person = GetPerson(insuranceTaker.PersonData.First().PersonNr);
                    if (person != null)
                    {
                        if (person.InsuranceTaker.Count != 0)
                            sale.InsuranceTakerId = person.InsuranceTaker.First().Id;
                        else
                        {
                            if (InsertInsuranceTakerIdToInsurancePerson(insuranceTaker.PersonData.First()))
                            {
                                person = GetPerson(insuranceObject.PersonData.First().PersonNr);
                                sale.InsuranceTakerId = person.InsuranceTaker.First().Id;
                            }
                        }
                        UpdatePerson(insuranceTaker.PersonData.First()); // Update to latest information
                    }
                    else
                    {
                        if (ZipCityExist((int)insuranceTaker.PersonData.First().Zipcode))
                            insuranceTaker.PersonData.First().ZipCity = null;
                        sale.InsuranceTaker = insuranceTaker;
                    }
                }
                else if (insuranceTaker.CompanyData.Count > 0)
                {
                    insuranceTaker.CompanyData.First().ZipCity.Zipcode = insuranceTaker.CompanyData.First().Zipcode;

                    var company = GetCompany(insuranceTaker.CompanyData.First().OrgNr);
                    if (company != null)
                    {
                        sale.InsuranceTakerId = company.InsuranceTaker.First().Id;
                        UpdateCompany(insuranceTaker.CompanyData.First()); // Update to latest information
                    }
                    else
                    {
                        if (ZipCityExist((int)insuranceTaker.CompanyData.First().Zipcode))
                            insuranceTaker.CompanyData.First().ZipCity = null;
                        sale.InsuranceTaker = insuranceTaker;
                    }
                }

                var insurancePerson = GetPerson(insuranceObject.PersonData.First().PersonNr);
                if (insurancePerson != null)
                {
                    insuranceObject.PersonData.First().ZipCity = null;
                    if (insurancePerson.InsuranceObject.Count != 0)
                        sale.InsuranceObjectId = insurancePerson.InsuranceObject.First().ObjectId;
                    else
                    {
                        if (InsertObjectIdToObjectPerson(insurancePerson))
                        {
                            insurancePerson = GetPerson(insuranceObject.PersonData.First().PersonNr);
                            sale.InsuranceObjectId = insurancePerson.InsuranceObject.First().ObjectId;
                        }
                    }
                    UpdateObjectPerson(insuranceObject.PersonData.First());
                }
                else
                {
                    sale.InsuranceObject = insuranceObject;
                }

                var insuranceAppRow = appRows[0];

                var insurance = db.Insurance.Find(insuranceAppRow.Insurance.InsuranceId);
                if (insurance != null)
                {
                    insuranceAppRow.InsuranceApplicationId = sale.InsuranceApplicationId;
                    insuranceAppRow.InsuranceApplicationCN = sale.InsuranceCompany;
                    insuranceAppRow.InsuranceId = insurance.InsuranceId;
                    insuranceAppRow.Sale = sale;
                    insuranceAppRow.Insurance = insurance;
                    sale.InsuranceApplicationRows.Add(insuranceAppRow);
                }
                else
                {
                    return false;
                }

                if (appRows.Count > 1)
                {
                    var insuranceAddonAppRow = appRows[1];
                    if (insuranceAddonAppRow.Insurance != null)
                    {
                        var insuranceAddon = db.Insurance.Find(insuranceAddonAppRow.Insurance.InsuranceId);
                        if (insuranceAddon != null)
                        {
                            insuranceAddonAppRow.InsuranceApplicationId = sale.InsuranceApplicationId;
                            insuranceAddonAppRow.InsuranceApplicationCN = sale.InsuranceCompany;
                            insuranceAddonAppRow.InsuranceId = insuranceAddon.InsuranceId;
                            insuranceAddonAppRow.Sale = sale;
                            insuranceAddonAppRow.Insurance = insuranceAddon;
                            sale.InsuranceApplicationRows.Add(insuranceAddonAppRow);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                db.Sale.Add(sale);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                    {
                        // Get entry

                        DbEntityEntry entry = item.Entry;
                        string entityTypeName = entry.Entity.GetType().Name;

                        // Display or log error messages

                        foreach (DbValidationError subItem in item.ValidationErrors)
                        {
                            string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                     subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                            Console.WriteLine(message);
                        }
                    }
                    return false;
                }
                catch (DbUpdateException ex) //DbContext
                {
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex); // to be removed for debug testing later
                    return false;
                }
            }
        }

        /// <summary>
        /// Update an existing sale based on "Sale.InsuranceCompany" && "Sale.InsuranceApplicationId" according to PDF requierments.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateSale(Sale sale, List<InsuranceApplicationRow> appRows, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            using (var db = new DatabaseContext())
            {
                var _sale = db.Sale.SingleOrDefault(x => x.InsuranceCompany == sale.InsuranceCompany && x.InsuranceApplicationId == sale.InsuranceApplicationId);

                if (_sale == null)
                    return false;

                if (sale.InsuranceFixedComision != null)
                {
                    var commission = db.InsuranceFixedComision.SingleOrDefault(x => x.InsuranceApplicationId == sale.InsuranceApplicationId && x.InsuranceApplicationCN == sale.InsuranceCompany);
                    if (commission == null)
                    {
                        sale.InsuranceFixedComision.InsuranceApplicationId = sale.InsuranceApplicationId;
                        sale.InsuranceFixedComision.InsuranceApplicationCN = sale.InsuranceCompany;
                        _sale.InsuranceFixedComision = sale.InsuranceFixedComision;
                    }
                    else
                    {
                        _sale.InsuranceFixedComision.Commision = sale.InsuranceFixedComision.Commision;
                    }
                }

                if (insuranceTaker.PersonData.Count > 0)
                {
                    insuranceTaker.PersonData.First().ZipCity.Zipcode = (int)insuranceTaker.PersonData.First().Zipcode;
                    UpdatePerson(insuranceTaker.PersonData.First()); // Update to latest information
                }
                else if (insuranceTaker.CompanyData.Count > 0)
                {
                    insuranceTaker.CompanyData.First().ZipCity.Zipcode = insuranceTaker.CompanyData.First().Zipcode;
                    UpdateCompany(insuranceTaker.CompanyData.First()); // Update to latest information
                }

                UpdateObjectPerson(insuranceObject.PersonData.First()); // Update to latest information

                // insurance
                var insuranceApp = appRows[0]; // otherwise we get error for using an array...
                var insurance = db.InsuranceApplicationRows.SingleOrDefault(x => x.InsuranceApplicationId == insuranceApp.InsuranceApplicationId && x.InsuranceApplicationCN == insuranceApp.InsuranceApplicationCN && x.InsuranceId == insuranceApp.InsuranceId);
                insurance.Premium = insuranceApp.Premium;

                if (appRows.Count > 1) // insurance addon
                {
                    var insuranceAppAddon = appRows[1]; // otherwise we get error for using an array...
                    var insuranceAddon = db.InsuranceApplicationRows.SingleOrDefault(x => x.InsuranceApplicationId == insuranceAppAddon.InsuranceApplicationId && x.InsuranceApplicationCN == insuranceAppAddon.InsuranceApplicationCN && x.InsuranceId == insuranceAppAddon.InsuranceId);

                    if (insuranceAppAddon.Premium >= 0)
                        insuranceAddon.Premium = insuranceAppAddon.Premium;
                }

                _sale.PaymentType   = sale.PaymentType;
                _sale.ExpiaryDate   = sale.ExpiaryDate;
                _sale.StartDate     = sale.StartDate;
                _sale.Comments      = sale.Comments;

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex); // to be removed for debug testing later
                    return false;
                }
            }
        }

        #endregion

        #region Insurance

        /// <summary>
        /// Get all insurances there is
        /// </summary>
        /// <returns></returns>
        public static List<Insurance> GetInsurances()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance;
                if (query.Count() > 0 )
                {
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get data about insurances
        /// </summary>
        /// <returns>A list of insurances and data</returns>
        public static List<Insurance> GetInsuranceWithData()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance;
                if (query.Count() > 0)
                {
                    foreach (var q in query)
                    {
                        q.InsuranceAcqVariables     = q.InsuranceAcqVariables;
                        q.InsuranceBaseValues       = q.InsuranceBaseValues;
                    }
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get "main" insurances
        /// </summary>
        /// <returns>Return insurances as a list</returns>
        public static List<Insurance> GetMainInsurances()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance;
                if (query.Count() > 0)
                {
                    List<Insurance> ins = new List<Insurance>();
                    foreach (var item in query)
                    {
                        if (item.Insurance1.Count == 0)
                        {

                            item.InsuranceBaseValues = item.InsuranceBaseValues;
                            ins.Add(item);
                        }
                    }

                    return ins;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get "addon" insurances
        /// </summary>
        /// <returns>Return insurance addons as a list</returns>
        public static List<Insurance> GetAddonInsurances()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance;
                if (query.Count() > 0)
                {
                    List<Insurance> ins = new List<Insurance>();
                    foreach(var item in query)
                    {
                        if (item.Insurance1.Count > 0)
                        {
                            item.Insurance1 = item.Insurance1;
                            item.InsuranceBaseValues = item.InsuranceBaseValues;
                            ins.Add(item);
                        }
                    }

                    return ins;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion // Insurance

        #region CompanyData 

        /// <summary>
        /// Get a company based on orgnr
        /// </summary>
        /// <param name="orgNr"></param>
        /// <returns>Returns a CompanyData object</returns>
        public static CompanyData GetCompany(long orgNr)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData.Where(x => x.OrgNr == orgNr);

                if (query.Count() > 0)
                {
                    foreach( var item in query )
                    {
                        item.ZipCity = item.ZipCity;
                        item.CompanyContactData = item.CompanyContactData;
                        item.InsuranceTaker = item.InsuranceTaker;
                    }
                    return query.First();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a list of all OrgNumbers in the system
        /// </summary>
        /// <returns>A list of orgs numbers</returns>
        public static List<long> GetCompaniesOrgNr()
        {
            List<long> orgNrs = new List<long>();
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData;

                if (query.Count() > 0)
                {
                    foreach (var company in query)
                    {
                        orgNrs.Add(company.OrgNr);
                    }
                    return orgNrs;
                }

                return orgNrs;
            }
        }

        /// <summary>
        /// Update an existing companies data
        /// </summary>
        /// <param name="company"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateCompany(CompanyData company)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData.Find(company.OrgNr);
                try
                {
                    query.OrgNr = company.OrgNr;
                    query.CompanyName = company.CompanyName;
                    query.Email = company.Email;
                    query.Faxnumber = company.Faxnumber;
                    query.Phonenumber = company.Phonenumber;
                    query.StreetAdress = company.StreetAdress;
                    query.Zipcode = company.Zipcode;
                    if (!ZipCityExist(company.Zipcode))
                    {
                        query.ZipCity = new ZipCity
                        {
                            Zipcode = company.Zipcode,
                            City = company.ZipCity.City
                        };
                    }
                    if (company.ContactId > 0) // should always be true...
                    {
                        var contact = db.CompanyContactData.Find(company.ContactId);
                        contact.Email = company.CompanyContactData.Email;
                        contact.Firstname = company.CompanyContactData.Firstname;
                        contact.Lastname = company.CompanyContactData.Lastname;
                        contact.Phonenumber = company.CompanyContactData.Phonenumber;
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Error");
                    return false;
                }
            }
        }

        #endregion

        #region PersonData

        /// <summary>
        /// Get a person based on Persondata.personnr
        /// </summary>
        /// <param name="personNr"></param>
        /// <returns>returns a PersonData object</returns>
        public static PersonData GetPerson(long personNr)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Where(x => x.PersonNr == personNr);

                if (query.Count() > 0)
                {
                    foreach(var item in query)
                    {
                        item.ZipCity = item.ZipCity; // KANSKE Gjord för printfunktion
                        item.InsuranceTaker = item.InsuranceTaker;
                        item.InsuranceObject = item.InsuranceObject; // KANSKE! Kan vara bra för prospect?
                    }
                    return query.First();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Update an existing person based on "PersonData.PersonNr"
        /// </summary>
        /// <param name="person"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdatePerson(PersonData person)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Find(person.PersonNr);
                try
                {
                    if (!ZipCityExist((int) person.Zipcode))
                    {
                        query.ZipCity = new ZipCity
                        {
                            Zipcode = (int)person.Zipcode,
                            City = person.ZipCity.City
                        };
                    }
                    query.Email         = person.Email;
                    query.Firstname     = person.Firstname;
                    query.Lastname      = person.Lastname;
                    query.Phonenumber   = person.Phonenumber;
                    query.Homenumber    = person.Homenumber;
                    query.StreetAdress  = person.StreetAdress;
                    query.Zipcode       = person.Zipcode;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error");
                    return false;
                }
            }
        }

        /// <summary>
        /// Adding a new insurancetaker to an existing person
        /// </summary>
        /// <param name="person"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool InsertInsuranceTakerIdToInsurancePerson(PersonData person)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Find(person.PersonNr);
                try
                {
                    query.InsuranceTaker.Add(new InsuranceTaker
                    {
                        PersonData = new List<PersonData>
                        {
                            query
                        }
                    });
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Uppdatera endast specifika posterna av en "objectperson"
        /// </summary>
        /// <param name="person"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateObjectPerson(PersonData person)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Find(person.PersonNr);
                try
                {
                    query.Firstname = person.Firstname;
                    query.Lastname = person.Lastname;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error");
                    return false;
                }
            }
        }

        /// <summary>
        /// Adding an existing person as an insurance object
        /// </summary>
        /// <param name="person"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool InsertObjectIdToObjectPerson(PersonData person)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Find(person.PersonNr);
                try
                {
                    query.InsuranceObject.Add(new InsuranceObject
                    {
                        PersonData = new List<PersonData>
                        {
                            query
                        }
                    });
                    db.SaveChanges();
                    return true;
                }
                catch ( Exception )
                {
                    return false;
                }
            }
        }

        #endregion //PersonData

        #region Insurance base values

        /// <summary>
        /// Add a new set of Acqvariables
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="acqvariables"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddAcqVariables(Insurance insurance, InsuranceAcqVariables acqvariables)
        {
            using (var db = new DatabaseContext())
            {
                var q = db.InsuranceAcqVariables.Where(x => x.Insurance.InsuranceId == insurance.InsuranceId && x.Startdate == acqvariables.Startdate);

                if (q.Count() > 0)
                {
                    var old = q.FirstOrDefault();
                    old.AcqVariable = acqvariables.AcqVariable;
                }
                else
                {
                    var query = db.Insurance.Where(x => x.InsuranceId == insurance.InsuranceId).First();

                    query.InsuranceAcqVariables.Add(acqvariables);
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Update acqvalues based on "InsuranceAcqVariables.id"
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="basevalues"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateAcqVariables(Insurance insurance, InsuranceAcqVariables acqvariables)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceAcqVariables.Find(acqvariables.Id);

                query.AcqVariable = acqvariables.AcqVariable;
                query.Startdate = acqvariables.Startdate;

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Remove acqvalues based on "InsuranceAcqVariables.id"
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="basevalues"></param>
        /// <returns>bool of success or failure removal</returns>
        public static bool RemoveAcqVariable(Insurance insurance, InsuranceAcqVariables acqvariables)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceAcqVariables.Find(acqvariables.Id);

                db.InsuranceAcqVariables.Remove(query);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Add a new set of base values
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="basevalues"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddBaseValues(Insurance insurance, InsuranceBaseValues basevalues)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance.Where(x => x.InsuranceId == insurance.InsuranceId).First();

                query.InsuranceBaseValues.Add(basevalues);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Update basevalues based on "InsuranceBaseValues.id"
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="basevalues"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateBaseValues(Insurance insurance, InsuranceBaseValues basevalues)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceBaseValues.Find(basevalues.Id);

                if (basevalues.BaseValue.HasValue)
                    query.BaseValue = basevalues.BaseValue;
                if (basevalues.AcqValue.HasValue)
                {
                    if (basevalues.AcqValue <= 0)
                    {
                        query.AcqValue = null;
                    }
                    else
                    {
                        query.AcqValue = basevalues.AcqValue;
                    }
                }
                query.StartDate = basevalues.StartDate;

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get a list of insurances with all their base values
        /// </summary>
        /// <returns>A list of insurances</returns>
        public static List<Insurance> GetInsuranceBaseValues()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance;

                foreach ( var insurance in query )
                {
                    insurance.InsuranceBaseValues = insurance.InsuranceBaseValues;
                    insurance.InsuranceAcqVariables = insurance.InsuranceAcqVariables;
                }
               
                return query.ToList();
            }
        }

        #endregion // Insurance base values

        #region Prospect Contanct 

        /// <summary>
        /// Register when the "prospect" was printed so we can have a grace time of when it shall be printed next time
        /// </summary>
        /// <param name="contactdata"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool RegisterContactData(PersonProspectContact contactdata)
        {
            using (var db = new DatabaseContext())
            {
                db.PersonProspectContact.Add(contactdata);
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get a list of prospectsdata to know when the person was printed
        /// </summary>
        /// <returns>A list of prospectdata</returns>
        public static List<ProspectData> GetProspectPersonData()
        {
            List<ProspectData> prospect_persons = new List<ProspectData>();
            using (var db = new DatabaseContext())
            {
                var currentTime = DateTime.Now;
                string day = currentTime.Day.ToString();
                if (day.Length == 1)
                    day = "0" + currentTime.Day.ToString();
                string month = currentTime.Month.ToString();
                if (month.Length == 1)
                    month = "0" + currentTime.Month.ToString();
                long adult_time = long.Parse((currentTime.Year-18) + month + day + "9999");

                try
                {
                    var query = db.PersonData.Where(x => x.PersonNr <= adult_time) // get all "adults"
                                    .Include(y => y.InsuranceTaker) // include the takers
                                    .Where(y => y.InsuranceTaker.Any(z => z.Sale.FirstOrDefault().InsuranceObject.PersonData.FirstOrDefault().PersonNr > adult_time)); // are they paying for a kid insurance?
                                    //TODO check grace time variable
                    foreach (var item in query)
                    {
                        if (item.InsuranceObject.Count == 0 && item.InsuranceTaker.Count > 0)
                        { // person have no insurance, but is paying for an insurance of a kid
                            item.ZipCity = item.ZipCity;
                            prospect_persons.Add(new ProspectData
                            {
                                Person = item,
                                InsuranceSale = item.InsuranceTaker.First().Sale.First()
                            });
                        }
                        else if (item.InsuranceObject.Count > 0 && item.InsuranceTaker.Count > 0)
                        { // person has insurance, but are they active? 
                            var q = item.InsuranceObject.Where(x => x.Sale.FirstOrDefault().StartDate != null && (x.Sale.FirstOrDefault().ExpiaryDate > currentTime || x.Sale.FirstOrDefault().ExpiaryDate == null));
                            if (q.Count() > 0) // Shall be > 0 IF the insurance has a startdate AND is not expired
                            {
                                prospect_persons.Add(new ProspectData
                                {
                                    Person = item,
                                    InsuranceSale = item.InsuranceTaker.First().Sale.First()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return prospect_persons;
            }
        }

        #endregion

        #region PaymentTypes 

        /// <summary>
        /// Get all the different paymenttypes 
        /// </summary>
        /// <returns>List of paymenttypes</returns>
        public static List<PaymentTypes> GetPaymentTypes()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PaymentTypes;

                if (query.Count() > 0)
                {
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region InsuranceTakers / Object

        /// <summary>
        /// Gets a list of all insurance takers
        /// </summary>
        /// <returns>The list of insurancetakers</returns>
        public static List<InsuranceTaker> GetInsuranceTakers()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceTaker;
                if (db.InsuranceTaker.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        item.CompanyData = item.CompanyData;
                        item.PersonData = item.PersonData;

                        foreach (CompanyData company in item.CompanyData)
                        {
                            company.ZipCity = company.ZipCity;
                            company.InsuranceTaker = company.InsuranceTaker;
                            company.CompanyContactData = company.CompanyContactData;
                        }
                        foreach (PersonData person in item.PersonData)
                        {
                            person.ZipCity = person.ZipCity;
                            person.InsuranceTaker = person.InsuranceTaker;
                        }
                    }
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a list of all insurance objects
        /// </summary>
        /// <returns>The list of insuranceobjects</returns>
        public static List<InsuranceObject> GetInsuranceObjects()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceObject;
                if (db.InsuranceObject.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        item.PersonData = item.PersonData;
                        foreach (PersonData person in item.PersonData)
                        {
                            person.ZipCity = person.ZipCity;
                        }
                    }
                    return query.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region ZipCity

        /// <summary>
        /// Check if a zipcity already exist with "zipcode"
        /// </summary>
        /// <param name="Zipcode"></param>
        /// <returns>true or false if a city exist with zipcode</returns>
        public static bool ZipCityExist(int Zipcode)
        {
            using (var db = new DatabaseContext())
            {
                try
                {
                    var query = db.ZipCity.Where(x => x.Zipcode == Zipcode);
                    return query.Count() > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the ZipCity object of a zipcode
        /// </summary>
        /// <param name="Zipcode"></param>
        /// <returns>A object if zipcode is found</returns>
        public static ZipCity GetZipCity(int Zipcode)
        {
            using (var db = new DatabaseContext())
            {
                try
                {
                    var query = db.ZipCity.Where(x => x.Zipcode == Zipcode);
                    if (query.Count() > 0)
                        return query.First();
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion

        #region Vacation

        /// <summary>
        /// Get vacation rate for a specific year
        /// </summary>
        /// <param name="year">Which year of data that shall be retrieved</param>
        /// <returns>A object of the vacation rate for the year specified if found</returns>
        public static VacationRate GetVactionRateOfYear(int year)
        {
            using (var db = new DatabaseContext())
            {
                try
                {
                    var query = db.VacationRate.Where(x => x.StartDate == year);
                    if (query.Count() > 0)
                        return query.First();
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get a list of all vacation rates 
        /// </summary>
        /// <returns>A list of vacationrate objects</returns>
        public static List<VacationRate> GetVacationRates()
        {
            using (var db = new DatabaseContext())
            {
                try
                {
                    var query = db.VacationRate;
                    if (query.Count() > 0)
                        return query.ToList();
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///  Add new vacation rates
        /// </summary>
        /// <param name="rate"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddVacationRate(VacationRate rate)
        {
            using (var db = new DatabaseContext())
            {

                if (db.VacationRate.Where(x => x.StartDate == rate.StartDate).Count() > 0)
                    return false;

                db.VacationRate.Add(rate);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Update existing vacation rate values based on "VacationRate.StartDate"
        /// </summary>
        /// <param name="rate"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateVactionRate(VacationRate rate)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.VacationRate.Where(x => x.StartDate == rate.StartDate);

                if (query.Count() == 0) return false;

                var vacRate = query.First();

                vacRate.VacationRate1 = rate.VacationRate1;
                vacRate.VacationVariable = rate.VacationVariable;

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        #endregion

        #region Commission values

        /// <summary>
        /// Get all commission values
        /// </summary>
        /// <returns></returns>
        public static List<CommisionValues> GetCommissionValues()
        {
            using (var db = new DatabaseContext())
                return db.CommisionValues.ToList();
        }

        /// <summary>
        /// Add new commission values
        /// </summary>
        /// <param name="values"></param>
        /// <returns>bool of success or failure registration</returns>
        public static bool AddCommissionValues(CommisionValues values)
        {
            using (var db = new DatabaseContext())
            {

                db.CommisionValues.Add(values);

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Update existing commission values based on "CommisionValues.id"
        /// </summary>
        /// <param name="values"></param>
        /// <returns>bool of success or failure update</returns>
        public static bool UpdateCommissionValues(CommisionValues values)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CommisionValues.Find(values.Id);

                query.AdultAcqVariable = values.AdultAcqVariable;
                query.ChildAcqVariable = values.ChildAcqVariable;
                query.HighestAcqValue = values.HighestAcqValue;
                query.LowestAcqValue = values.LowestAcqValue;
                query.CommisionYear = values.CommisionYear;

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion

    }
}

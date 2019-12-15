using System;
using System.Collections.Generic;
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
        /// Returnerar en User baserat på agentNumber
        /// </summary>
        /// <param name="agentNumber"></param>
        /// <returns></returns>
        public static Users GetUser(int agentNumber)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Where(x => x.AgentNumber == agentNumber);

                if(query.Count() > 0)
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
        /// Returnerar users beroende på attr
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static List<Users> SearchUser(string attr)
        {
            using(var db = new DatabaseContext())
            {
                var sname = db.Users.Where(x => x.Firstname.ToLower().Equals(attr.ToLower()) || x.Lastname.ToLower().Equals(attr.ToLower()));

                int y;
                if (int.TryParse(attr, out y))
                {
                    var pnumb = db.Users.Where(x => x.PersonNr == y);
                    var union = sname.Union(pnumb);
                    foreach (var item in union)
                    {
                        item.Roles = item.Roles;
                        item.ZipCity = item.ZipCity;
                    }
                    return union.ToList();
                }

                foreach (var item in sname)
                {
                    item.Roles = item.Roles;
                    item.ZipCity = item.ZipCity;
                }
                return sname.ToList();
                
            }
        }

        /// <summary>
        /// Returnerar en lista på alla users
        /// </summary>
        /// <returns></returns>
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
        /// Lägger till en användare i databasen
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool AddUser(Users user)
        {
            using(var db = new DatabaseContext())
            {
                var adduser = new Users();
                adduser = user;
                db.Users.Add(adduser);
                try
                {
                    db.SaveChanges();
                    return true;
                } catch(Exception) {
                    return false;
                }
            }
        }

        /// <summary>
        /// Uppdaterar en user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool UpdateUser(Users user)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Where(x => x.AgentNumber == user.AgentNumber).First();
                query = user;
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

        #endregion //User

        #region User Roles
        /// <summary>
        /// Returnerar en lista på alla roller
        /// </summary>
        /// <returns></returns>
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
        /// Lägger på en roll på en användare
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool AddRoleToUser(Users user, Roles role)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Where(x=> x.AgentNumber == user.AgentNumber).First();
                query.Roles.Add(role);
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
        /// Tar bort en roll från en användare
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool RemoveRoleFromUser(Users user, Roles role)
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Users.Where(x => x.AgentNumber == user.AgentNumber).First();
                //var remove = query.Roles.Where(x => x.PermissionId == role.PermissionId).First();
                query.Roles.Remove(role);
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

        #region Sale

        /// <summary>
        /// Returnerar en lista på försäljningar
        /// </summary>
        /// <returns></returns>
        public static List<Sale> GetSales()
        {
            using(var db = new DatabaseContext())
            {
                var query = db.Sale;
                if(query.Count() > 0)
                {
                    foreach(var item in query)
                    {
                        item.InsuranceTaker = item.InsuranceTaker;
                        item.PaymentTypes = item.PaymentTypes;
                        item.InsurancePaid = item.InsurancePaid;
                        item.InsuranceObject = item.InsuranceObject;
                        item.InsuranceFixedComision = item.InsuranceFixedComision;
                        item.Insurance = item.Insurance;
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
        /// Creates a sale
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="user"></param>
        /// <param name="insuranceTaker"></param>
        /// <param name="insuranceObject"></param>
        /// <returns></returns>
        public static bool CreateSale(Sale sale, Users user, InsuranceTaker insuranceTaker, InsuranceObject insuranceObject)
        {
            using(var db = new DatabaseContext())
            {
                sale.AgentId = user.AgentNumber;

                if (insuranceTaker.PersonData.Count > 0)
                {
                    var person = GetPerson(insuranceTaker.PersonData.First().PersonNr);
                    if (person != null)
                    {
                        sale.InsuranceTakerId = person.InsuranceTaker.First();
                        UpdatePerson(insuranceTaker.PersonData.First()); // Update to latest information
                    }
                    else
                    {
                        sale.InsuranceTaker = insuranceTaker;
                    }
                }
                else if (insuranceTaker.CompanyData.Count > 0)
                {
                    var company = GetCompany(insuranceTaker.CompanyData.First().OrgNr);
                    if (company != null)
                    {
                        sale.InsuranceTaker = company.InsuranceTaker.First();
                        UpdateCompany(insuranceTaker.CompanyData.First()); // Update to latest information
                    }
                    else
                    {
                        sale.InsuranceTaker = insuranceTaker;
                    }
                }

                var insurancePerson = GetPerson(insuranceObject.PersonData.First().PersonNr);
                if (insurancePerson != null)
                {
                    sale.InsuranceObject = insurancePerson.InsuranceObject.First();
                    UpdateObjectPerson(insuranceObject.PersonData.First());
                }
                else
                {
                    sale.InsuranceObject = insuranceObject;
                }

                db.Sale.Add(sale);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public static bool UpdateSale(Sale sale)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Sale.Where(x => x.InsuranceCompany == sale.InsuranceCompany && x.InsuranceApplicationId == sale.InsuranceApplicationId).First();
                query = sale;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="fixedcomision"></param>
        /// <returns></returns>
        public static bool RegisterFixedComision(Sale sale, InsuranceFixedComision fixedcomision)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Sale.Where(x => x.InsuranceCompany == sale.InsuranceCompany && x.InsuranceApplicationId == sale.InsuranceApplicationId).First();
                query.InsuranceFixedComision = fixedcomision;
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
        /// 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="paiddata"></param>
        /// <returns></returns>
        public static bool RegisterPayment(Sale sale, InsurancePaid paiddata)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Sale.Where(x => x.InsuranceCompany == sale.InsuranceCompany && x.InsuranceApplicationId == sale.InsuranceApplicationId).First();
                query.InsurancePaid = paiddata;
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

        #region Insurance

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static InsuranceTypes GetInsuranceTypes()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceTypes;

                if (query.Count() > 0)
                {
                    return query.First();
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
        /// 
        /// </summary>
        /// <param name="orgNr"></param>
        /// <returns></returns>
        public static CompanyData GetCompany(long orgNr)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData.Where(x => x.OrgNr == orgNr);

                if (query.Count() > 0)
                {
                    foreach( var item in query )
                    {
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

        public static bool UpdateCompany(CompanyData company)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData.Where(x => x.OrgNr == company.OrgNr).First();
                try
                {
                    db.Entry(query).CurrentValues.SetValues(company);
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
        /// 
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static List<CompanyData> SearchCompanies(string attr)
        {
            using (var db = new DatabaseContext())
            {
                var cname = db.CompanyData.Where(x => x.CompanyName.ToLower().Equals(attr.ToLower()));

                int y;
                if (int.TryParse(attr, out y))
                {
                    var orgnumb = db.CompanyData.Where(x => x.OrgNr == y); // TODO - hämta som ovan för getcompany 
                    return cname.Union(orgnumb).ToList();
                }
                // TODO - hämta som ovan för getcompany 
                return cname.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<CompanyData> GetCompanies()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.CompanyData;
                if (query.Count() > 0)
                {
                    return db.CompanyData.ToList(); // TODO - hämta som ovan för getcompany 
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region PersonData

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personNr"></param>
        /// <returns></returns>
        public static PersonData GetPerson(long personNr)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Where(x => x.PersonNr == personNr);

                if (query.Count() > 0)
                {
                    foreach(var item in query)
                    {
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
        /// 
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static List<PersonData> SearchPersons(string attr)
        {
            using (var db = new DatabaseContext())
            {
                var sname = db.PersonData.Where(x => x.Firstname.ToLower().Equals(attr.ToLower()) || x.Lastname.ToLower().Equals(attr.ToLower()));

                int y;
                if (int.TryParse(attr, out y))
                {
                    var pnumb = db.PersonData.Where(x => x.PersonNr == y);  // TODO - hämta som ovan för GetPerson 
                    return sname.Union(pnumb).ToList();
                }
                // TODO - hämta som ovan för GetPerson 
                return sname.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<PersonData> GetPersons()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData;
                if (query.Count() > 0)
                {
                    // TODO - hämta som ovan för GetPerson 
                    return db.PersonData.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static bool UpdatePerson(PersonData person)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PersonData.Where(x => x.PersonNr == person.PersonNr).First();
                try
                {
                    db.Entry(query).CurrentValues.SetValues(person);
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
        /// Uppdatera endast specifika posterna av en "objectperson"
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
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

        #endregion //UserData

        #region Insurance base values

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="acqvalues"></param>
        /// <returns></returns>
        public static bool AddAcqValues(Insurance insurance, InsuranceAcqValues acqvalues)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance.Where(x => x.InsuranceId == insurance.InsuranceId).First();

                query.InsuranceAcqValues.Add(acqvalues);

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
        /// 
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="acqvariables"></param>
        /// <returns></returns>
        public static bool AddAcqVariables(Insurance insurance, InsuranceAcqVariables acqvariables)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Insurance.Where(x => x.InsuranceId == insurance.InsuranceId).First();

                query.InsuranceAcqVariables.Add(acqvariables);

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
        /// 
        /// </summary>
        /// <param name="insurance"></param>
        /// <param name="basevalues"></param>
        /// <returns></returns>
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

        #endregion // Insurance base values

        #region Prospect Contanct 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactdata"></param>
        /// <returns></returns>
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

        #endregion

        #region PaymentTypes 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static PaymentTypes GetPaymentTypes()
        {
            using (var db = new DatabaseContext())
            {
                var query = db.PaymentTypes;

                if (query.Count() > 0)
                {
                    return query.First();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region InsuranceTakers

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<InsuranceTaker> GetInsuranceTakers()
        {
            List<InsuranceTaker> insuranceTakers = new List<InsuranceTaker>();

            using (var db = new DatabaseContext())
            {
                var query = db.InsuranceTaker;
                if (db.InsuranceTaker.Count() > 0)
                {
                    foreach( var item in query )
                    {
                        item.CompanyData = item.CompanyData;
                        item.PersonData = item.PersonData;
                    }
                    return query.ToList() ;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

    }
}

using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TheMagnificentModels.Utilities
{
    /// <summary>
    /// Static class that handles all document related tasks
    /// </summary>
    public static class DocumentManager
    {
        private static readonly string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly string PROSPECTFILENAME = "\\prospects.pdf";
        private static readonly string PROVISIONFILENAME = "\\provision";
 
        /// <summary>
        /// Exports prospects to PDF
        /// </summary>
        public static void ExportProspectsToPdf()
        {
            var prospects = DataManager.GetProspectPersonData();
            if (prospects.Count > 0)
            {
                using (Document document = new Document(PageSize.A4))
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(DOCUMENTS + PROSPECTFILENAME, FileMode.Create));
                    document.Open();
                    foreach (var prospect in prospects)
                    {
                        document.Add(new Paragraph("Prospekt, utskriftdatum " + DateTime.Today.ToLocalTime().ToShortDateString().ToString()));
                        document.Add(new Paragraph("\nPersonnummer:\n" + prospect.Person.PersonNr.ToString().Substring(0, 8) + "-" + prospect.Person.PersonNr.ToString().Substring(8)));
                        document.Add(new Paragraph("\nNamn:\n" + prospect.Person.Firstname + " " + prospect.Person.Lastname));
                        document.Add(new Paragraph("\nAdress:\n" + prospect.Person.StreetAdress));
                        document.Add(new Paragraph("\nPostnummer:\n" + prospect.Person.Zipcode.ToString()));
                        document.Add(new Paragraph("\nOrt:\n" + prospect.Person.ZipCity.City));
                        document.Add(new Paragraph("\nTelefonnummer:\n" + prospect.Person.Phonenumber));
                        document.Add(new Paragraph("\nEmail:\n" + prospect.Person.Email));
                        document.Add(new Paragraph("\nAgenturnummer för såld barnförsäkring:\n" + prospect.InsuranceSale.AgentId));
                        document.Add(new Paragraph("------------------------------------------------------------------------"));
                        document.Add(new Paragraph("\nKontakt datum:\n"));
                        document.Add(new Paragraph("\nUtfall:\n\n\n"));
                        document.Add(new Paragraph("\nSäljare:\n"));
                        document.Add(new Paragraph("\nAgentur:\n"));
                        document.NewPage();
                    }
                    document.Close();
                }
            }
        }

        /// <summary>
        /// Exports custormer info to a pdf and returns the filename of the pdf
        /// </summary>
        /// <param name="insuranceTaker"></param>
        /// <returns></returns>
        public static string ExportCustomerInfoToPdf(object insuranceTaker)
        {
            if (insuranceTaker is PersonData)
            {
                var search = (PersonData)insuranceTaker;
                var personData = DataManager.GetPerson(search.PersonNr);
                using (Document document = new Document(PageSize.A4))
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(DOCUMENTS + string.Format("{0}", personData.PersonNr + ".pdf"), FileMode.Create));
                    document.Open();

                    document.Add(new Paragraph("Kundinformation, utskriftdatum " + DateTime.Today.ToLocalTime().ToShortDateString().ToString()));
                    document.Add(new Paragraph("\nPersonnummer:\n" + personData.PersonNr.ToString().Substring(0, 8) + "-" + personData.PersonNr.ToString().Substring(8)));
                    document.Add(new Paragraph("\nNamn:\n" + personData.Firstname + " " + personData.Lastname));
                    document.Add(new Paragraph("\nAdress:\n" + personData.StreetAdress));
                    document.Add(new Paragraph("\nPostnummer:\n" + personData.Zipcode.ToString()));
                    document.Add(new Paragraph("\nOrt:\n" + personData.ZipCity.City));
                    document.Add(new Paragraph("\nTelefonnummer:\n" + personData.Phonenumber));
                    document.Add(new Paragraph("\nEmail:\n" + personData.Email));

                    document.Close();

                    return string.Format("{0}", personData.PersonNr + ".pdf");
                }
            }
            else if (insuranceTaker is CompanyData)
            {
                var search = (CompanyData)insuranceTaker;
                var companyData = DataManager.GetCompany(search.OrgNr);
                using (Document document = new Document(PageSize.A4))
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(DOCUMENTS + string.Format("{0}", companyData.OrgNr + ".pdf"), FileMode.Create));
                    document.Open();
                    
                    document.Add(new Paragraph("Kundinformation, utskriftdatum " + DateTime.Today.ToLocalTime().ToShortDateString().ToString()));
                    document.Add(new Paragraph("\nOrganisationsnr:\n" + companyData.OrgNr.ToString()));
                    document.Add(new Paragraph("\nFöretagsnamn:\n" + companyData.CompanyName));
                    document.Add(new Paragraph("\nKontakt:\n" + companyData.CompanyContactData.Firstname + " " + companyData.CompanyContactData.Lastname));
                    document.Add(new Paragraph("\nAdress:\n" + companyData.StreetAdress));
                    document.Add(new Paragraph("\nPostnummer:\n" + companyData.Zipcode.ToString()));
                    document.Add(new Paragraph("\nOrt:\n" + companyData.ZipCity.City));
                    document.Add(new Paragraph("\nTelefonnummer:\n" + companyData.Phonenumber));
                    document.Add(new Paragraph("\nEmail:\n" + companyData.Email));

                    document.Close();

                    return string.Format("{0}", companyData.OrgNr + ".pdf");
                }
            }
            else
            {
                Console.WriteLine("Needs to be customer data or company data");
            }
            return "ERROR";
        }

        /// <summary>
        /// Exports provision statement to pdf
        /// </summary>
        /// <param name="childSumAcq"></param>
        /// <param name="adultSumAcq"></param>
        /// <param name="lifeSumBase"></param>
        /// <param name="otherSum"></param>
        /// <param name="provSo"></param>
        /// <param name="provLife"></param>
        /// <param name="provOther"></param>
        /// <param name="vacation"></param>
        /// <param name="vacationPercent"></param>
        /// <param name="taxPercent"></param>
        /// <param name="tax"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="sumProv"></param>
        /// <param name="toPay"></param>
        /// <returns></returns>
        public static string ExportProvisionToPdf(Users user, int childSumAcq, int adultSumAcq, int lifeSumBase, int otherSum, int provSo, int provLife, int provOther, int vacation, int vacationPercent, int taxPercent, int tax, int month, int year, int sumProv, int toPay)
        {
            using (Document document = new Document(PageSize.A4))
            {
    
                var writer = PdfWriter.GetInstance(document, new FileStream(DOCUMENTS + PROVISIONFILENAME + string.Format("{0}", user.PersonNr + ".pdf"), FileMode.Create));
                var sumAdultChild = childSumAcq + adultSumAcq;

                document.Open();
                document.Add(new Paragraph("Kundinformation"));
                document.Add(new Paragraph("Namn: " + user.Firstname + " " + user.Lastname + "                                            Provisioner"));
                document.Add(new Paragraph("Adress: " + user.StreetAdress));
                document.Add(new Paragraph("Postnummer: " + user.Zipcode.ToString()));
                document.Add(new Paragraph("Ort: " + user.ZipCity.City));
                document.Add(new Paragraph("\nPeriod                Månad:  " + month + "       År:  " + year));
                document.Add(new Paragraph("\n\n\nBu summa ackvärde:            " + childSumAcq));
                document.Add(new Paragraph("Vu summa ackvärde:            " + adultSumAcq)); 
                document.Add(new Paragraph("Summa ackvärde:                 " + sumAdultChild + "                                  Prov so:         " + provSo));
                document.Add(new Paragraph("\nLiv summa ackvärde:            " + lifeSumBase + "                                  Prov liv:         " + provLife));
                document.Add(new Paragraph("\nÖvrigt provision:                    " + otherSum + "                                  Prov övrigt:    " + provOther));
                document.Add(new Paragraph("\nSemesterersättning:             " + vacationPercent + "%" + "                             Semesterers:  " + vacation));
                document.Add(new Paragraph("\n                                                                                 Summa prov:  " + sumProv));
                document.Add(new Paragraph("\nPreliminär skatt:                   " + taxPercent + " %" + "                              Avgår skatt:    " + tax));
                document.Add(new Paragraph("Insättning på bankkonto den: " + year + "-" + month + "-" + "25" + "                 Att utbetala:    " + toPay ));

                document.Close();

                return string.Format(PROVISIONFILENAME + "{0}", user.PersonNr + ".pdf");
            }
        }

    }
}

namespace TheMagnificentModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }
        public virtual DbSet<CommisionValues> CommisionValues { get; set; }
        public virtual DbSet<CompanyContactData> CompanyContactData { get; set; }
        public virtual DbSet<CompanyData> CompanyData { get; set; }
        public virtual DbSet<Insurance> Insurance { get; set; }
        public virtual DbSet<InsuranceApplicationRow> InsuranceApplicationRows { get; set; }
        public virtual DbSet<InsuranceFixedComision> InsuranceFixedComision { get; set; }
        public virtual DbSet<InsuranceObject> InsuranceObject { get; set; }
        public virtual DbSet<InsuranceTaker> InsuranceTaker { get; set; }
        public virtual DbSet<PaymentTypes> PaymentTypes { get; set; }
        public virtual DbSet<PersonData> PersonData { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<VacationRate> VacationRate { get; set; }
        public virtual DbSet<ZipCity> ZipCity { get; set; }
        public virtual DbSet<InsuranceAcqVariables> InsuranceAcqVariables { get; set; }
        public virtual DbSet<InsuranceBaseValues> InsuranceBaseValues { get; set; }
        public virtual DbSet<PersonProspectContact> PersonProspectContact { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyContactData>()
                .Property(e => e.Firstname)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyContactData>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyContactData>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyContactData>()
                .Property(e => e.Phonenumber)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyContactData>()
                .HasMany(e => e.CompanyData)
                .WithRequired(e => e.CompanyContactData)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyData>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyData>()
                .Property(e => e.Phonenumber)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyData>()
                .Property(e => e.Faxnumber)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyData>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyData>()
                .Property(e => e.StreetAdress)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyData>()
                .HasMany(e => e.InsuranceTaker)
                .WithMany(e => e.CompanyData)
                .Map(m => m.ToTable("InsuranceCompanyTaker").MapLeftKey("CompanyOrgNr").MapRightKey("InsuranceTakerId"));

            modelBuilder.Entity<Insurance>()
                .Property(e => e.InsuranceName)
                .IsUnicode(false);

            modelBuilder.Entity<Insurance>()
                .HasMany(e => e.InsuranceAcqVariables)
                .WithRequired(e => e.Insurance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Insurance>()
                .HasMany(e => e.InsuranceApplicationRows)
                .WithRequired(e => e.Insurance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Insurance>()
                .HasMany(e => e.InsuranceBaseValues)
                .WithRequired(e => e.Insurance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Insurance>()
                .HasMany(e => e.Insurance1)
                .WithMany(e => e.Insurance2)
                .Map(m => m.ToTable("InsuranceAddons").MapLeftKey("InsuranceId").MapRightKey("AddonToInsurance"));

            modelBuilder.Entity<InsuranceApplicationRow>()
                .Property(e => e.InsuranceApplicationCN)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceFixedComision>()
                .Property(e => e.InsuranceApplicationCN)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceObject>()
                .HasMany(e => e.Sale)
                .WithRequired(e => e.InsuranceObject)
                .HasForeignKey(e => e.InsuranceObjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceTaker>()
                .HasMany(e => e.Sale)
                .WithRequired(e => e.InsuranceTaker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceTaker>()
                .HasMany(e => e.PersonProspectContact)
                .WithRequired(e => e.InsuranceTaker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceTaker>()
                .HasMany(e => e.PersonData)
                .WithMany(e => e.InsuranceTaker)
                .Map(m => m.ToTable("InsuranceTakerPerson").MapLeftKey("InsuranceTakerId").MapRightKey("PersonNr"));

            modelBuilder.Entity<InsuranceTaker>()
                .HasMany(e => e.CompanyData)
                .WithMany(e => e.InsuranceTaker)
                .Map(m => m.ToTable("InsuranceCompanyTaker").MapLeftKey("InsuranceTakerId").MapRightKey("CompanyOrgNr"));

            modelBuilder.Entity<PaymentTypes>()
                .Property(e => e.PaymentTypeName)
                .IsUnicode(false);

            modelBuilder.Entity<PaymentTypes>()
                .HasMany(e => e.Sale)
                .WithRequired(e => e.PaymentTypes)
                .HasForeignKey(e => e.PaymentType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.Firstname)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.Phonenumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.Homenumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .Property(e => e.StreetAdress)
                .IsUnicode(false);

            modelBuilder.Entity<PersonData>()
                .HasMany(e => e.PersonProspectContact)
                .WithRequired(e => e.PersonData)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonData>()
                .HasMany(e => e.InsuranceObject)
                .WithMany(e => e.PersonData)
                .Map(m => m.ToTable("InsuranceObjectRow").MapLeftKey("PersonNr").MapRightKey("InsuranceObjectId"));

            modelBuilder.Entity<Roles>()
                .Property(e => e.Permission)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("UserAuthRow").MapLeftKey("PermissionId").MapRightKey("AgentNumber"));

            modelBuilder.Entity<Sale>()
                .Property(e => e.InsuranceCompany)
                .IsUnicode(false);

            modelBuilder.Entity<Sale>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<Sale>()
                .HasMany(e => e.InsuranceApplicationRows)
                .WithRequired(e => e.Sale)
                .HasForeignKey(e => new { e.InsuranceApplicationId, e.InsuranceApplicationCN })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sale>()
                .HasOptional(e => e.InsuranceFixedComision)
                .WithRequired(e => e.Sale);

            modelBuilder.Entity<Users>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Firstname)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Phonenumber)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.StreetAdress)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Sale)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.AgentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ZipCity>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<ZipCity>()
                .HasMany(e => e.CompanyData)
                .WithRequired(e => e.ZipCity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ZipCity>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.ZipCity)
                .WillCascadeOnDelete(false);
        }
    }
}

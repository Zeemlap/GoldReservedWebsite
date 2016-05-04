using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class GoldReservesDbContext : DbContext
    {

        public DbSet<Language> Languages { get; set; }
        public DbSet<PoliticalEntity> PoliticalEntities { get; set; }
        public DbSet<PoliticalEntityName> PoliticalEntityNames { get; set; }
        public DbSet<WorldOfficialGoldHoldingReport> WorldOfficialGoldHoldingReports { get; set; }
        public DbSet<WorldOfficialGoldHoldingReportRow> WorldOfficialGoldHoldingReportRows { get; set; }
        public DbSet<GeoRegion> GeoRegions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReport>()
                .HasKey(r => r.DataTimePointInternal);
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReport>()
                .Property(r => r.PublishTimePointInternal);
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReportRow>()
                .HasRequired(r => r.Report)
                .WithMany(r => r.Rows)
                .HasForeignKey(r => r.ReportDataTimePointInternal)
                .WillCascadeOnDelete(true);
            modelBuilder
                .Entity<GeoRegion>()
                .HasKey(g => g.Id_Alpha3Internal);
            modelBuilder
                .Entity<PoliticalEntity>()
                .HasOptional(p => p.GeoRegion)
                .WithMany(g => g.PoliticalEntities)
                .HasForeignKey(p => p.GeoRegionId)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.LanguageId)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute(DbUtilities.IndexName_PoliticalEntityName_LanguageId_NamePrefix, 0)));
            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.NamePrefix)
                .IsUnicode()
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnAnnotation(
                    "Index", 
                    new IndexAnnotation(new IndexAttribute(DbUtilities.IndexName_PoliticalEntityName_LanguageId_NamePrefix, 1)));
            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.NameSuffix)
                .IsUnicode();
        }

        public void InitializeAndSeed()
        {
            DbCommand[] commands = null;
            var connection = Database.Connection;
            bool shouldClose = false;
            try
            {
                commands = new DbCommand[0];
                // TODO initialize seed commands
                if (0 < commands.Length && connection.State == ConnectionState.Closed)
                {
                    shouldClose = true;
                    connection.Open();
                }
                foreach(var command in commands)
                {
                    command.ExecuteNonQuery();
                }


                Languages.Add(new Language()
                {
                    Name_English = "english",
                });
                SaveChanges();
            }
            finally
            {
                if (shouldClose)
                {
                    connection.Close();
                }
                if (commands != null)
                {
                    foreach(var command in commands)
                    {
                        command.Dispose();
                    }
                }
            }
        }
    }
}

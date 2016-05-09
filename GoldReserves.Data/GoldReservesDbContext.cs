using Com.Jab.LibCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class GoldReservesDbContext : DbContext
    {
        public const short LanguageId_English = 1;

        public GoldReservesDbContext(string connectionString)
            : base(connectionString)
        {

        }


        public DbSet<Language> Languages { get; set; }
        public DbSet<PoliticalEntity> PoliticalEntities { get; set; }
        public DbSet<PoliticalEntityName> PoliticalEntityNames { get; set; }

        public List<TonsPerPoliticalEntity> GetWorldOfficialGoldHoldings(DateTime dt_tp1)
        {
            var dt_tp1YearQuarter = new DateTime(dt_tp1.Year, (dt_tp1.Month - 1) / 3 * 3 + 1, 1, 0, 0, 0, 0, dt_tp1.Kind);
            var tpq_tp1YearQuarter = new TimePointInQuarters(dt_tp1YearQuarter).Value;
            var q1 =
                from rr1 in (
                    from rr in WorldOfficialGoldHoldingReportRows
                    where rr.ReportDataTimePointInternal < tpq_tp1YearQuarter
                    group rr by rr.PoliticalEntityId into rrGroup
                    select new { PoliticalEntityId = rrGroup.Key, DataTimePointInternal = rrGroup.Max(rr_ => rr_.ReportDataTimePointInternal), })
                join rr2 in WorldOfficialGoldHoldingReportRows on rr1 equals new { rr2.PoliticalEntityId, DataTimePointInternal = rr2.ReportDataTimePointInternal, }
                select new { rr2.PoliticalEntityId, rr2.Tons };
            var l1 = q1.ToList();
            var l2 = l1.Select(rr => new TonsPerPoliticalEntity()
            {
                PoliticalEntityId = rr.PoliticalEntityId,
                Tons = rr.Tons,
            }).ToList();
            return l2;
        }

        public DbSet<WorldOfficialGoldHoldingReport> WorldOfficialGoldHoldingReports { get; set; }
        public DbSet<WorldOfficialGoldHoldingReportRow> WorldOfficialGoldHoldingReportRows { get; set; }
        public DbSet<GeoRegion> GeoRegions { get; set; }

        private static void AppendColumnName(StringBuilder sb, string columnName)
        {
            sb.EnsureCapacity(checked(sb.Length + columnName.Length + 2));
            sb.Append('[');
            sb.Append(columnName);
            sb.Replace("[", "]]", sb.Length - columnName.Length, columnName.Length);
            sb.Append(']');
        }

        private System.Data.Entity.Infrastructure.DbRawSqlQuery<PoliticalEntityName> GetPoliticalEntityNameQuery(string name)
        {
            string namePrefix, nameSuffix;
            DbUtil.IndexedVarChar_Split(name, DbUtil.CharLength_PoliticalEntityName_NamePrefix, out namePrefix, out nameSuffix);
            object[] paramArray = namePrefix == null
                ? Array.Empty<object>()
                : new object[1 + (nameSuffix != null ? 1 : 0)];
            int i = 0;
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT *");
            sqlBuilder.Append("FROM dbo.PoliticalEntityNames ");
            sqlBuilder.Append("WHERE ");

            AppendColumnName(sqlBuilder, DbUtil.ColumnName_PoliticalEntityName_NamePrefix);
            if (namePrefix != null)
            {
                sqlBuilder.Append(" = @p").Append(i);
                paramArray[i] = namePrefix;
                i += 1;

                sqlBuilder.Append(" AND ");
                AppendColumnName(sqlBuilder, DbUtil.ColumnName_PoliticalEntityName_NameSuffix);
                if (nameSuffix == null)
                {
                    sqlBuilder.Append(" IS NULL");
                }
                else
                {
                    sqlBuilder.Append(" = @p").Append(i);
                    paramArray[i] = nameSuffix;
                    i += 1;
                }
            }
            else
            {
                Debug.Assert(nameSuffix == null);
                sqlBuilder.Append(" IS NULL");
            }
            return Database.SqlQuery<PoliticalEntityName>(sqlBuilder.ToString(), paramArray);
        }

        public PoliticalEntity GetPoliticalEntity(string name)
        {
            var p1 = (from pen in GetPoliticalEntityNameQuery(name)
                          join l in Languages on pen.LanguageId equals l.Id
                          join p2 in PoliticalEntities on pen.PoliticalEntityId equals p2.Id
                          where l.Name_English == DbUtil.LanguageName_English
                          select p2).SingleOrDefault();
            if (p1 != null) Entry(p1).State = EntityState.Detached;
            return p1;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<WorldOfficialGoldHoldingReport>()
                .HasKey(r => r.DataTimePointInternal)
                .Property(r => r.DataTimePointInternal)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReport>()
                .Property(r => r.PublishTimePointInternal);
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReportRow>()
                .Property(r => r.ReportDataTimePointInternal)
                .HasColumnAnnotation(
                    "Index", new IndexAnnotation(
                        new IndexAttribute(DbUtil.IndexName_WorldOfficialGoldHoldingReportRow_PoliticalEntityId_ReportDataTimePointInternal) { IsUnique = true, Order = 2, }));
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReportRow>()
                .Property(r => r.PoliticalEntityId)
                .HasColumnAnnotation(
                    "Index", new IndexAnnotation(
                        new IndexAttribute(DbUtil.IndexName_WorldOfficialGoldHoldingReportRow_PoliticalEntityId_ReportDataTimePointInternal) { IsUnique = true, Order = 1, }));
            modelBuilder
                .Entity<WorldOfficialGoldHoldingReportRow>()
                .HasRequired(r => r.Report)
                .WithMany(r => r.Rows)
                .HasForeignKey(r => r.ReportDataTimePointInternal)
                .WillCascadeOnDelete(true);
            modelBuilder
                .Entity<Language>()
                .Property(l => l.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder
                .Entity<GeoRegion>()
                .HasKey(g => g.Id_Alpha3Internal);
            modelBuilder
                .Entity<GeoRegion>()
                .Property(g => g.Id_Alpha3Internal)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder
                .Entity<PoliticalEntity>()
                .Property(p => p.GeoRegionId);
            modelBuilder
                .Entity<PoliticalEntity>()
                .HasOptional(p => p.GeoRegion)
                .WithMany(g => g.PoliticalEntities)
                .HasForeignKey(p => p.GeoRegionId)
                .WillCascadeOnDelete(false);
            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.LanguageId)
                .HasColumnName(DbUtil.ColumnName_PoliticalEntityName_LanguageId)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute(DbUtil.IndexName_PoliticalEntityName_LanguageId_NamePrefix, 1) { IsUnique = false, }));
            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.NamePrefix)
                .IsUnicode()
                .HasMaxLength(16)
                .HasColumnName(DbUtil.ColumnName_PoliticalEntityName_NamePrefix)
                .IsFixedLength()
                .HasColumnAnnotation(
                    "Index", 
                    new IndexAnnotation(new IndexAttribute(DbUtil.IndexName_PoliticalEntityName_LanguageId_NamePrefix, 0) { IsUnique = false, }));
            modelBuilder
                .Entity<PoliticalEntityName>()
                .Property(p => p.NameSuffix)
                .HasColumnName(DbUtil.ColumnName_PoliticalEntityName_NameSuffix)
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
                    Id = LanguageId_English,
                    Name_English = DbUtil.LanguageName_English,
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

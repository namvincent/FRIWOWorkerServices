#nullable disable

using FRIWOServerApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FRIWOServerApi.Data.TRACE
{
    public class TraceDbContext : DbContext
    {
        /// <summary>
        /// Magic string.
        /// </summary>
        public static readonly string RowVersion = nameof(RowVersion);

        /// <summary>
        /// Magic strings.
        /// </summary>
        public static readonly string TraceDB = nameof(TraceDB).ToLower();


        public TraceDbContext(DbContextOptions<TraceDbContext> options)
            : base(options)
        {
            Debug.WriteLine($"Instance: {ContextId.InstanceId}; using pooling: {ContextId.Lease}; context is created.");

        }

        public virtual DbSet<LinkInfo> GetLinkInfos { get; set; }

        //public virtual DbSet<GET_PASS_FAIL_BY_TIME> GetPassFail { get; set; }
        public virtual DbSet<V_ROUTING_BY_PART_NO> GetTableName { get; set; }
        //public virtual DbSet<ProductionStaff> ProductionStaff { get; set; }
        public virtual DbSet<BusinessReport> BusinessReport { get; set; }

        public virtual DbSet<ProcessLock> ProcessLock { get; set; }
        public virtual DbSet<RoutingResults> RoutingResults { get; set; }
        public virtual DbSet<Husqvarna_Json> Husqvarna_Json { get; set; }
        public virtual DbSet<Palette> WIPScan { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<UPDATE_GUID> GUID { get; set; }
        public virtual DbSet<Monthly_Output> KPI_Monthly { get; set; }
        public virtual DbSet<Shop_Order_Overview> Shoporder_status { get; set; }
        public virtual DbSet<Shop_Order_Without_Routing> Shoporder_no_routing { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<BusinessReport>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("V_BUSINESS_REPOPRT_DAILY");
                entity.Property(v => v.Part_no).HasColumnName("PART_NO");
            });

            modelBuilder.Entity<Monthly_Output>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FINISHED_GOOD_PS");
            });


            modelBuilder.Entity<UPDATE_GUID>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GUID_HUSQVARNA");
            });

            modelBuilder.Entity<Husqvarna_Json>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_HUSQVARNA_EXPORT");

                entity.Property(e => e.BODY)
                    .IsRequired();
            });

            modelBuilder.Entity<Shop_Order_Overview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_IFS_SHOP_ORDER_STATUS");

            });

            modelBuilder.Entity<Shop_Order_Without_Routing>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_IFS_SO_WITHOUT_ROUTING");
            });

            modelBuilder.Entity<V_ROUTING_BY_PART_NO>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_ROUTING_BY_PART_NO");

                entity.Property(e => e.ID)
                .IsRequired();

                entity.Property(e => e.PART_NO)
                    .IsRequired();

                entity.Property(e => e.STATION_NAME)
                    .IsRequired();

                entity.Property(e => e.TABLE_NAME)
                    .IsRequired();

                entity.Property(e => e.DISPLAY_ORDER_STATUS)
                    .IsRequired();

                entity.Property(e => e.REVISION)
                    .IsRequired();
            });

            modelBuilder.Ignore<LinkInfo>();
            modelBuilder.Entity<LinkInfo>().HasNoKey();

            modelBuilder.Ignore<ProcessLock>();
            modelBuilder.Entity<ProcessLock>(entity =>
            {
                entity.HasKey(e => e.ID);

            });

            modelBuilder.Entity<RoutingResults>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.RoutingState).IsRequired();

            });

            base.OnModelCreating(modelBuilder);

        }

        /// <summary>
        /// Dispose pattern.
        /// </summary>
        public override void Dispose()
        {

            Debug.WriteLine($"Instance: {ContextId.InstanceId}; using pooling: {ContextId.Lease}; context is disposed.");

            base.Dispose();

        }

        /// <summary>
        /// Dispose pattern.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/></returns>
        public override ValueTask DisposeAsync()
        {
            Debug.WriteLine($"{ContextId} context disposed async.");
            return base.DisposeAsync();
        }
    }
}
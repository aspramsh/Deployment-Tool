using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ProjectPublisherContext : DbContext
    {
        public virtual DbSet<DeploymentSpecification> DeploymentSpecification { get; set; }
        public virtual DbSet<Type> Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public ProjectPublisherContext(DbContextOptions<ProjectPublisherContext> options)
           : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeploymentSpecification>(entity =>
            {
                entity.Property(e => e.AppPoolName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DeploymentPath)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectPath)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.DeploymentSpecification)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Deploymen__TypeI__571DF1D5");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}

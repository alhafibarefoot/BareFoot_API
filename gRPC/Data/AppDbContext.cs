using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using gRPC.Data.Models;

namespace gRPC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Implement [DefaultValue]
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo?)property.FieldInfo;
                    if (memberInfo == null)
                        continue;
                    var defaultValue =
                        Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute))
                        as DefaultValueAttribute;
                    if (defaultValue == null)
                        continue;
                    property.SetDefaultValue(defaultValue.Value);
                }

                // Default datetime for SQLite
                 if (Database.IsSqlite())
                {
                    entityType
                        .FindProperty("CreatedOn")
                        ?.SetDefaultValueSql("datetime('now', 'utc')");
                }
            }
        }
    }
}

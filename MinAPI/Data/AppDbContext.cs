using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinAPI.Data.Models;

namespace MinAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //Here We Implement [DefaultValue] inside  Models
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo)property.FieldInfo;
                    if (memberInfo == null)
                        continue;
                    var defaultValue =
                        Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute))
                        as DefaultValueAttribute;
                    if (defaultValue == null)
                        continue;
                    property.SetDefaultValue(defaultValue.Value);


                }

                 //Fixed defaultValue for datetime in MSSQL & SQLitefor All Entioties as one
                entityType
                    .FindProperty("CreatedOn")
                    ?.SetDefaultValueSql(
                        Database.IsSqlite() ? "datetime('now', 'utc')" : "getutcdate()"
                    );


            }


        }
    }
}

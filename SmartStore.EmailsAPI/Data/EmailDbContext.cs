using Microsoft.EntityFrameworkCore;
using SmartStore.EmailsAPI.Models;

namespace SmartStore.EmailsAPI.Data;

public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<EmailLog> EmailLogs { get; set; }
}

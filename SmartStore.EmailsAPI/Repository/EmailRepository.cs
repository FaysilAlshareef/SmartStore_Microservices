using Microsoft.EntityFrameworkCore;
using SmartStore.EmailsAPI.Data;
using SmartStore.EmailsAPI.Dtos;
using SmartStore.EmailsAPI.Models;

namespace SmartStore.EmailsAPI.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<EmailDbContext> _dbContext;

    public EmailRepository(DbContextOptions<EmailDbContext> dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SendAndLogEmail(PaymentUpdateMessageDto messageDto)
    {
        // Implement an Email Sender or call other class library
        var emailLog = new EmailLog()
        {
            Email = messageDto.Email,
            Created = DateTime.Now,
            Message = $"Order - {messageDto.OrderId} has been Created successfully ."
        };
        await using var _db = new EmailDbContext(_dbContext);
        await _db.AddAsync(emailLog);
        await _db.SaveChangesAsync();
    }
}

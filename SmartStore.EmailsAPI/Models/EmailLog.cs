namespace SmartStore.EmailsAPI.Models;

public class EmailLog
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
}

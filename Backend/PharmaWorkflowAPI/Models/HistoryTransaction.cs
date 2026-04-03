using System.ComponentModel.DataAnnotations;

namespace PharmaWorkflowAPI.Models;

public class HistoryTransaction
{
    public int HistoryId { get; set; }
    
    [Required]
    public int TransactionId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string ActionBy { get; set; } = string.Empty;
    
    public string? Comments { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    
    public MainTransaction? Transaction { get; set; }
}

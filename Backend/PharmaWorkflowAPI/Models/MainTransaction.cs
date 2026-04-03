using System.ComponentModel.DataAnnotations;

namespace PharmaWorkflowAPI.Models;

public class MainTransaction
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string DrugName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string BatchNo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string RequestedBy { get; set; } = string.Empty;
    
    [Required]
    public string Status { get; set; } = "Initiated";
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    
    [Timestamp]
    public byte[]? RowVersion { get; set; }
    
    public string? Comments { get; set; }
    
    public ICollection<HistoryTransaction> History { get; set; } = new List<HistoryTransaction>();
}

public enum TransactionStatus
{
    Initiated,
    Registered,
    Approved,
    Rejected,
    Active,
    Inactive,
    Modified
}

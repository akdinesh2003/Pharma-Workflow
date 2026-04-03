using System.ComponentModel.DataAnnotations;

namespace PharmaWorkflowAPI.DTOs;

public class CreateTransactionRequest
{
    [Required]
    [StringLength(200)]
    public string DrugName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string BatchNo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string RequestedBy { get; set; } = string.Empty;
    
    public string? Comments { get; set; }
}

public class UpdateTransactionRequest
{
    [Required]
    public string DrugName { get; set; } = string.Empty;
    
    [Required]
    public string BatchNo { get; set; } = string.Empty;
    
    public string? Comments { get; set; }
    
    public byte[]? RowVersion { get; set; }
}

public class ApproveRejectRequest
{
    [Required]
    public string ActionBy { get; set; } = string.Empty;
    
    public string? Comments { get; set; }
    
    public byte[]? RowVersion { get; set; }
}

public class ActivateRequest
{
    public string? Comments { get; set; }
}

public class TransactionResponse
{
    public int Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? Comments { get; set; }
    public byte[]? RowVersion { get; set; }
}

public class DashboardStats
{
    public int TotalRequests { get; set; }
    public int Approved { get; set; }
    public int Pending { get; set; }
    public int Rejected { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int Modified { get; set; }
    public int Initiated { get; set; }
}

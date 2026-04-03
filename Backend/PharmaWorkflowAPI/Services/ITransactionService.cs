using PharmaWorkflowAPI.DTOs;
using PharmaWorkflowAPI.Models;

namespace PharmaWorkflowAPI.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request, string username);
    Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync(string? status = null, string? search = null);
    Task<TransactionResponse?> GetTransactionByIdAsync(int id);
    Task<TransactionResponse> ApproveTransactionAsync(int id, ApproveRejectRequest request);
    Task<TransactionResponse> ActivateTransactionAsync(int id, string username, string comments);
    Task<TransactionResponse> CompleteTransactionAsync(int id, string username, string comments);
    Task<TransactionResponse> RejectTransactionAsync(int id, ApproveRejectRequest request);
    Task<TransactionResponse> ModifyTransactionAsync(int id, UpdateTransactionRequest request, string username);
    Task<bool> SoftDeleteTransactionAsync(int id);
    Task<IEnumerable<HistoryTransaction>> GetTransactionHistoryAsync(int id);
    Task<DashboardStats> GetDashboardStatsAsync();
}

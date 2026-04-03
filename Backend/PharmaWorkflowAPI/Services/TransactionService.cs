using Microsoft.EntityFrameworkCore;
using PharmaWorkflowAPI.Data;
using PharmaWorkflowAPI.DTOs;
using PharmaWorkflowAPI.Models;
using System.Transactions;

namespace PharmaWorkflowAPI.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ApplicationDbContext context, ILogger<TransactionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request, string username)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var transaction = new MainTransaction
            {
                DrugName = request.DrugName,
                BatchNo = request.BatchNo,
                RequestedBy = request.RequestedBy,
                Status = "Initiated",
                Comments = request.Comments,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _context.Main_Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Use user's comments if provided, otherwise use default
            var historyComments = string.IsNullOrWhiteSpace(request.Comments) 
                ? "Request initiated" 
                : request.Comments;

            var history = new HistoryTransaction
            {
                TransactionId = transaction.Id,
                Action = "Created",
                ActionBy = username,
                Comments = historyComments,
                NewStatus = "Initiated",
                ActionDate = DateTime.Now
            };

            _context.History_Transactions.Add(history);
            await _context.SaveChangesAsync();

            scope.Complete();

            _logger.LogInformation($"Transaction created: {transaction.Id} by {username}");

            return MapToResponse(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            throw;
        }
    }

    public async Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync(string? status = null, string? search = null)
    {
        var query = _context.Main_Transactions
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(t => t.DrugName.Contains(search) || t.BatchNo.Contains(search));

        var transactions = await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        return transactions.Select(MapToResponse);
    }

    public async Task<TransactionResponse?> GetTransactionByIdAsync(int id)
    {
        var transaction = await _context.Main_Transactions
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        return transaction != null ? MapToResponse(transaction) : null;
    }

    public async Task<TransactionResponse> ApproveTransactionAsync(int id, ApproveRejectRequest request)
    {
        return await UpdateStatusAsync(id, "Approved", "Approved", request.ActionBy, request.Comments, request.RowVersion);
    }

    public async Task<TransactionResponse> ActivateTransactionAsync(int id, string username, string comments)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var transaction = await _context.Main_Transactions
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {id} not found");

            if (transaction.Status != "Approved")
                throw new InvalidOperationException($"Only Approved transactions can be activated. Current status: {transaction.Status}");

            var previousStatus = transaction.Status;
            transaction.Status = "Active";
            transaction.UpdatedDate = DateTime.Now;

            var history = new HistoryTransaction
            {
                TransactionId = id,
                Action = "Activated",
                ActionBy = username,
                PreviousStatus = previousStatus,
                NewStatus = "Active",
                Comments = string.IsNullOrWhiteSpace(comments) ? "Transaction activated for processing" : comments,
                ActionDate = DateTime.Now
            };

            _context.History_Transactions.Add(history);
            await _context.SaveChangesAsync();

            scope.Complete();

            _logger.LogInformation($"Transaction {id} activated by {username}");

            return MapToResponse(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating transaction {id}");
            throw;
        }
    }

    public async Task<TransactionResponse> CompleteTransactionAsync(int id, string username, string comments)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var transaction = await _context.Main_Transactions
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {id} not found");

            if (transaction.Status != "Active")
                throw new InvalidOperationException($"Only Active transactions can be completed. Current status: {transaction.Status}");

            var previousStatus = transaction.Status;
            transaction.Status = "Inactive";
            transaction.UpdatedDate = DateTime.Now;

            var history = new HistoryTransaction
            {
                TransactionId = id,
                Action = "Completed",
                ActionBy = username,
                PreviousStatus = previousStatus,
                NewStatus = "Inactive",
                Comments = string.IsNullOrWhiteSpace(comments) ? "Transaction completed" : comments,
                ActionDate = DateTime.Now
            };

            _context.History_Transactions.Add(history);
            await _context.SaveChangesAsync();

            scope.Complete();

            _logger.LogInformation($"Transaction {id} completed by {username}");

            return MapToResponse(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing transaction {id}");
            throw;
        }
    }

    public async Task<TransactionResponse> RejectTransactionAsync(int id, ApproveRejectRequest request)
    {
        return await UpdateStatusAsync(id, "Rejected", "Rejected", request.ActionBy, request.Comments, request.RowVersion);
    }

    public async Task<TransactionResponse> ModifyTransactionAsync(int id, UpdateTransactionRequest request, string username)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var transaction = await _context.Main_Transactions
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found");

            // Optimistic concurrency check
            if (request.RowVersion != null && !transaction.RowVersion!.SequenceEqual(request.RowVersion))
                throw new DbUpdateConcurrencyException("Transaction was modified by another user");

            var previousStatus = transaction.Status;
            transaction.DrugName = request.DrugName;
            transaction.BatchNo = request.BatchNo;
            transaction.Comments = request.Comments;
            transaction.Status = "Modified";
            transaction.UpdatedDate = DateTime.Now;

            var history = new HistoryTransaction
            {
                TransactionId = transaction.Id,
                Action = "Modified",
                ActionBy = username,
                Comments = request.Comments ?? "Transaction modified",
                PreviousStatus = previousStatus,
                NewStatus = "Modified",
                ActionDate = DateTime.Now
            };

            _context.History_Transactions.Add(history);
            await _context.SaveChangesAsync();

            scope.Complete();

            _logger.LogInformation($"Transaction modified: {id} by {username}");

            return MapToResponse(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error modifying transaction {id}");
            throw;
        }
    }

    public async Task<bool> SoftDeleteTransactionAsync(int id)
    {
        var transaction = await _context.Main_Transactions
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return false;

        transaction.IsDeleted = true;
        transaction.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Transaction soft deleted: {id}");

        return true;
    }

    public async Task<IEnumerable<HistoryTransaction>> GetTransactionHistoryAsync(int id)
    {
        return await _context.History_Transactions
            .Where(h => h.TransactionId == id)
            .OrderByDescending(h => h.ActionDate)
            .ToListAsync();
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var transactions = await _context.Main_Transactions
            .Where(t => !t.IsDeleted)
            .ToListAsync();

        return new DashboardStats
        {
            TotalRequests = transactions.Count,
            Approved = transactions.Count(t => t.Status == "Approved"),
            Pending = transactions.Count(t => t.Status == "Initiated"),
            Rejected = transactions.Count(t => t.Status == "Rejected"),
            Active = transactions.Count(t => t.Status == "Active"),
            Inactive = transactions.Count(t => t.Status == "Inactive"),
            Modified = transactions.Count(t => t.Status == "Modified"),
            Initiated = transactions.Count(t => t.Status == "Initiated")
        };
    }

    private async Task<TransactionResponse> UpdateStatusAsync(int id, string newStatus, string action, 
        string actionBy, string? comments, byte[]? rowVersion)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var transaction = await _context.Main_Transactions
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found");

            // Optimistic concurrency check
            if (rowVersion != null && !transaction.RowVersion!.SequenceEqual(rowVersion))
                throw new DbUpdateConcurrencyException("Transaction was modified by another user");

            var previousStatus = transaction.Status;
            transaction.Status = newStatus;
            transaction.UpdatedDate = DateTime.Now;

            var history = new HistoryTransaction
            {
                TransactionId = transaction.Id,
                Action = action,
                ActionBy = actionBy,
                Comments = comments,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                ActionDate = DateTime.Now
            };

            _context.History_Transactions.Add(history);
            await _context.SaveChangesAsync();

            scope.Complete();

            _logger.LogInformation($"Transaction {action}: {id} by {actionBy}");

            return MapToResponse(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating transaction {id}");
            throw;
        }
    }

    private TransactionResponse MapToResponse(MainTransaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            DrugName = transaction.DrugName,
            BatchNo = transaction.BatchNo,
            RequestedBy = transaction.RequestedBy,
            Status = transaction.Status,
            CreatedDate = transaction.CreatedDate,
            UpdatedDate = transaction.UpdatedDate,
            Comments = transaction.Comments,
            RowVersion = transaction.RowVersion
        };
    }
}

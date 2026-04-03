using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaWorkflowAPI.DTOs;
using PharmaWorkflowAPI.Services;
using System.Security.Claims;

namespace PharmaWorkflowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "User,Manager,Admin")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var username = User.Identity?.Name ?? "Unknown";
            var result = await _transactionService.CreateTransactionAsync(request, username);
            return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, new { message = "Error creating transaction", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions([FromQuery] string? status = null, [FromQuery] string? search = null)
    {
        try
        {
            var transactions = await _transactionService.GetAllTransactionsAsync(status, search);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, new { message = "Error retrieving transactions" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving transaction {id}");
            return StatusCode(500, new { message = "Error retrieving transaction" });
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ApproveTransaction(int id, [FromBody] ApproveRejectRequest request)
    {
        try
        {
            var result = await _transactionService.ApproveTransactionAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "Transaction was modified by another user. Please refresh and try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving transaction {id}");
            return StatusCode(500, new { message = "Error approving transaction" });
        }
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ActivateTransaction(int id, [FromBody] ActivateRequest? request = null)
    {
        try
        {
            var username = User.Identity?.Name ?? "Unknown";
            var comments = request?.Comments ?? "Transaction activated for processing";
            var result = await _transactionService.ActivateTransactionAsync(id, username, comments);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating transaction {id}");
            return StatusCode(500, new { message = "Error activating transaction" });
        }
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> CompleteTransaction(int id, [FromBody] ActivateRequest? request = null)
    {
        try
        {
            var username = User.Identity?.Name ?? "Unknown";
            var comments = request?.Comments ?? "Transaction completed";
            var result = await _transactionService.CompleteTransactionAsync(id, username, comments);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing transaction {id}");
            return StatusCode(500, new { message = "Error completing transaction" });
        }
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> RejectTransaction(int id, [FromBody] ApproveRejectRequest request)
    {
        try
        {
            var result = await _transactionService.RejectTransactionAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "Transaction was modified by another user. Please refresh and try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting transaction {id}");
            return StatusCode(500, new { message = "Error rejecting transaction" });
        }
    }

    [HttpPut("{id}/modify")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ModifyTransaction(int id, [FromBody] UpdateTransactionRequest request)
    {
        try
        {
            var username = User.Identity?.Name ?? "Unknown";
            var result = await _transactionService.ModifyTransactionAsync(id, request, username);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "Transaction was modified by another user. Please refresh and try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error modifying transaction {id}");
            return StatusCode(500, new { message = "Error modifying transaction" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        try
        {
            var result = await _transactionService.SoftDeleteTransactionAsync(id);
            
            if (!result)
                return NotFound(new { message = "Transaction not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting transaction {id}");
            return StatusCode(500, new { message = "Error deleting transaction" });
        }
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetTransactionHistory(int id)
    {
        try
        {
            var history = await _transactionService.GetTransactionHistoryAsync(id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving history for transaction {id}");
            return StatusCode(500, new { message = "Error retrieving transaction history" });
        }
    }

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _transactionService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard stats");
            return StatusCode(500, new { message = "Error retrieving dashboard stats" });
        }
    }
}

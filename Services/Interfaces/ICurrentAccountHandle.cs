using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Services.Interfaces;

public interface ICurrentAccountHandler
{
  Task<ResponseDTO<CurrentAccountDTO>> GetSummaryAsync(string userId);
  Task<ResponseDTO<List<CurrentAccountDTO>>> GetTransactionsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
  Task<ResponseDTO<CurrentAccountDTO>> CreateTransactionAsync(string userId, CreateCurrentAccountDTO dto);
  Task<ResponseDTO<bool>> HasAvailableCreditAsync(string userId, decimal amount);
}

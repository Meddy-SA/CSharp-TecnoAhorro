// using TecnoCredito.Contexts;
// using TecnoCredito.Models.DTOs;
//
// namespace tecnoCredito.Services;
//
// public class CurrentAccountService(Context context) : ICurrentAccountHandler
// {
//
//   private readonly Context context = context;
//
//
//
//   public async Task<ResponseDTO<CurrentAccountDTO>> CreateTransactionAsync(
//           string userId, CreateCurrentAccountDTO dto)
//   {
//     var response = new ResponseDTO<CurrentAccountDTO> { Success = false };
//
//     using var transaction = await _context.Database.BeginTransactionAsync();
//
//     try
//     {
//       var lastTransaction = await _context.CurrentAccounts
//           .Where(ca => ca.UserId == userId)
//           .OrderByDescending(ca => ca.Date)
//           .FirstOrDefaultAsync();
//
//       var currentBalance = lastTransaction?.Balance ?? 0;
//       var newBalance = CalculateNewBalance(currentBalance, dto.Type, dto.Amount);
//
//       var newTransaction = new CurrentAccount
//       {
//         UserId = userId,
//         Date = DateTime.UtcNow,
//         Type = dto.Type,
//         Amount = dto.Amount,
//         Balance = newBalance,
//         Description = dto.Description,
//         Status = StatusEnum.Active
//       };
//
//       await _context.CurrentAccounts.AddAsync(newTransaction);
//       await _context.SaveChangesAsync();
//
//       // Agregar productos relacionados si es un crédito
//       if (dto.Type == AccountTransactionTypeEnum.Credit && dto.ProductIds.Any())
//       {
//         var currentAccountProducts = dto.ProductIds.Select(productId =>
//             new CurrentAccountProduct
//             {
//               CurrentAccountId = newTransaction.Id,
//               ProductId = productId
//             });
//
//         await _context.CurrentAccountProducts.AddRangeAsync(currentAccountProducts);
//       }
//
//       // Actualizar límite de crédito
//       await UpdateCreditLimit(userId, dto.Type, dto.Amount);
//
//       await _context.SaveChangesAsync();
//       await transaction.CommitAsync();
//
//       response.Result = await MapToDTO(newTransaction);
//       response.Success = true;
//     }
//     catch (Exception ex)
//     {
//       await transaction.RollbackAsync();
//       response.Error = ex.Message;
//     }
//
//     return response;
//   }
//
//   private async Task<CurrentAccountDTO> MapToDTO(CurrentAccount entity)
//   {
//     var products = await _context.CurrentAccountProducts
//         .Where(cap => cap.CurrentAccountId == entity.Id)
//         .Include(cap => cap.Product)
//         .Select(cap => new CurrentAccountProductDTO
//         {
//           ProductId = cap.ProductId,
//           ProductName = cap.Product.Name,
//           ProductPrice = cap.Product.Price
//         })
//         .ToListAsync();
//
//     return new CurrentAccountDTO
//     {
//       Id = entity.Id,
//       Date = entity.Date,
//       Type = entity.Type.GetDisplayName(),
//       Amount = entity.Amount,
//       Balance = entity.Balance,
//       Description = entity.Description,
//       Status = entity.Status.GetDisplayName(),
//       Products = products
//     };
//   }
// }

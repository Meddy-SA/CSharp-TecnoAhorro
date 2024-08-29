using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.Enums;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Models;

public class CustomerContract
{
  public int Id { get; set; }
  public string UserId { get; set; } = null!;
  public virtual AppUser User { get; set; } = null!;
  public decimal TotalAmount { get; set; }
  public int InstallmentsCount { get; set; }
  public decimal InstallmentAmount { get; set; }
  public decimal InterestRate { get; set; }
  public DateTime CreationDate { get; set; }
  public StatusEnum Status { get; set; } = StatusEnum.Active;

  public virtual ICollection<Installment> Installments { get; set; } = [];
  public virtual ICollection<Payment> Payments { get; set; } = [];
  public virtual ICollection<ContractProduct> ContractProducts { get; set; } = [];
}

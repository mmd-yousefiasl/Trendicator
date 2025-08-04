using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Users
{
    public enum BalanceTransactionTypeEnum
    {
        [Display(Name = "واریز")]
        Credit,
        [Display(Name = "برداشت")]
        Debit
    }
}

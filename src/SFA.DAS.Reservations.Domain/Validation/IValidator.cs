using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Validation
{
    public interface IValidator<in T>
    {
        Task<ValidationResult> ValidateAsync(T item);
    }
}

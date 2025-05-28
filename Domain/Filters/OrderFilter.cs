using Domain.Constants;

namespace Domain.Filters;

public class OrderFilter : BaseFilter
{
    public Status? Status { get; set; }
}
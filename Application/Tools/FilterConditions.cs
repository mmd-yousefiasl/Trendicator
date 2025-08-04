using System.Linq.Expressions;

namespace Application.Tools
{
    public class FilterConditions<TEntity> : List<Expression<Func<TEntity, bool>>>
    {
    }
}

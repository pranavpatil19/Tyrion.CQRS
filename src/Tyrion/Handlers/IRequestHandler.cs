using System.Threading.Tasks;

namespace Tyrion
{
    public interface IRequestHandler<in TRequest, TResult> where TRequest : IRequest
    {
        Task<IResult<TResult>> Execute(TRequest request);
    }

    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        Task<IResult> Execute(TRequest request);
    }
}

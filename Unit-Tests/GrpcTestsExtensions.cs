using Grpc.Core;
using Moq.Language;
using Moq.Language.Flow;

namespace Unit_Tests;

internal static class GrpcTestsExtensions
{
    public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, AsyncUnaryCall<TResult>> mock, TResult value)
        where TMock : class
    {
        return mock.Returns(new AsyncUnaryCall<TResult>(Task.FromResult(value), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));
    }
}
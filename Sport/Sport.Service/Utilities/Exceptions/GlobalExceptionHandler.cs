using System.Web.Http.ExceptionHandling;
using Sport.Service.Results;

namespace Sport.Service.Utilities.Exceptions
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = ApiResult.Exception(context.Exception);
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
}
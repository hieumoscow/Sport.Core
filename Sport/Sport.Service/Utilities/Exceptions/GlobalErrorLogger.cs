using System.Web.Http.ExceptionHandling;

namespace Sport.Service.Utilities.Exceptions
{
    public class GlobalErrorLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var exception = context.Exception;           
        }
    }
}

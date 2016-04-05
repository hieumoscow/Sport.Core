using System;
using Sport.Core.Models.EndPoints;

namespace Sport.Core.Exceptions
{
    public class RealtimeException : Exception
    {
        public RealtimeException(string messag) : base(messag)
        {

        }
    }
    public class ApiException<T> : Exception
    {
        public ApiResponse<T> Response { get; }

        public ApiException(ApiResponse<T> temp) : base(temp.ErrorDesc.ToString())
        {
            Response = temp;
        }
    }
}
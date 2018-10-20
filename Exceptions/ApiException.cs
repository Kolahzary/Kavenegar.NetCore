using Kavenegar.Core.Models.Enums;

namespace Kavenegar.Core.Exceptions
{
    public class ApiException : KavenegarException
    {
        public MetaCode Code { get; protected set; }

        public ApiException(string message, int code)
         : base(message)
        {
            Code = (MetaCode)code;
        }
    }
}
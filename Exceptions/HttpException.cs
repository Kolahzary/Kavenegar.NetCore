namespace Kavenegar.Core.Exceptions
{
    public class HttpException : KavenegarException
    {
        public int Code { get; private set; }

        public HttpException(string message, int code)
         : base(message)
        {
            Code = code;
        }
    }
}
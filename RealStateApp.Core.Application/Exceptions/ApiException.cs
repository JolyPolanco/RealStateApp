using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ApiException() : base ()
        {
            
        }

        public ApiException(string message) : base(message)
        {

        }

        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public ApiException(string message, params object[]args) : base(String.Format(CultureInfo.CurrentCulture, message,args))
        {

        }



    }
}

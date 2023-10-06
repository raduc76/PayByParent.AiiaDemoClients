using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T value, int statusCode)
        {
            Value = value;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Response object
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Response status code
        /// </summary>
        public int StatusCode { get; set; }
    }
}

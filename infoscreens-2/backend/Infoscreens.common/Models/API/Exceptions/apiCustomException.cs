using Infoscreens.Common.Interfaces;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API
{
    public class apiCustomException : ICustomException
    {
        public string ExceptionMessageLabel { get; set; }

        public List<string> ExceptionMessageParameters { get; set; }

        public apiCustomException(string messageLabel, List<string> parameters)
        {
            ExceptionMessageLabel = messageLabel;
            ExceptionMessageParameters = parameters;
        }
    }
}

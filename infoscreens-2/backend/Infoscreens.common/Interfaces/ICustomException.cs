using System.Collections.Generic;

namespace Infoscreens.Common.Interfaces
{
    public interface ICustomException
    {
        string ExceptionMessageLabel { get; }
        List<string> ExceptionMessageParameters { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescrioGames.Core.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException()
        {
        }

        public ApplicationException(string message)
            : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class MissingDependencyException : ApplicationException
    {
        public MissingDependencyException(string message)
            : base(message)
        {
        }

        public MissingDependencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
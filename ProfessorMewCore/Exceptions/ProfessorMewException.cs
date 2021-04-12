using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Exceptions
{
    public class ProfessorMewException : Exception
    {
        public ProfessorMewException()
        {

        }

        public ProfessorMewException(string message) : base(message)
        {

        }

        public ProfessorMewException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}

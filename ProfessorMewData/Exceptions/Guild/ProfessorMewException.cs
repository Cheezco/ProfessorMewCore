using System;

namespace ProfessorMewData.Exceptions.Guild
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

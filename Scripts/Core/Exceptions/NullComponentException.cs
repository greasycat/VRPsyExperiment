using System;

namespace Core.Exceptions
{
    public class NullComponentException: Exception
    {
        public NullComponentException(string msg) : base(msg){}
    }
}
using System;

namespace InformationCenter.Services
{

    public class ItemIsNullException : Exception
    {

        public ItemIsNullException() : base() { }

    }

    public class FieldNotFoundException : Exception
    {

        public FieldNotFoundException() : base() { }

    }

    public class DotNetTypeNotFoundException : Exception
    {

        public DotNetTypeNotFoundException() : base() { }

    }

    public class NullableValueNotAllowedException : Exception
    {

        public NullableValueNotAllowedException() : base() { }

    }

    public class TypeMismatchException : Exception
    {

        public TypeMismatchException() : base() { }

    }

}
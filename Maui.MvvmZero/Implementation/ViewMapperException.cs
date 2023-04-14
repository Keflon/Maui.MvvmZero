using System.Diagnostics;
using System.Runtime.Serialization;

namespace FunctionZero.Maui.MvvmZero
{
    [Serializable]
    internal class ViewMapperException : Exception
    {
        public ViewMapperException(string message, Exception innerException) : base(message, innerException)
        {
            Debug.WriteLine(message);
        }
    }
}
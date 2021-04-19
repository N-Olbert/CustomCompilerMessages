using System;

namespace CustomCompilerMessages.Definitions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class WarningAttribute : Attribute
    {
        public string Message { get; set; }

        public WarningAttribute(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
        }
    }
}

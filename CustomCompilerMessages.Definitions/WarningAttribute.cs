using System;

namespace CustomCompilerMessages.Definitions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | 
                    AttributeTargets.Field | AttributeTargets.Constructor |
                    AttributeTargets.Parameter | AttributeTargets.Class |
                    AttributeTargets.Struct | AttributeTargets.Interface)]
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

using System;

namespace CustomCompilerMessages.Definitions
{
    [AttributeUsage(AttributeTargets.Method)]
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

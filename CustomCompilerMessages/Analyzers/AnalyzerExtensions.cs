using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CustomCompilerMessages.Analyzers
{
    internal static class AnalyzerExtensions
    {
        public static AttributeData GetAttributeData<TAttribute>(this ImmutableArray<AttributeData> attributes)
            where TAttribute : Attribute
        {
            foreach(var attribute in attributes)
            {
                var attributeClasss = attribute?.AttributeClass;
                if (attributeClasss != null)
                {
                    if(attributeClasss.Name == typeof(TAttribute).Name)
                    {
                        return attribute;
                    }
                }
            }

            return null;
        }
    }
}

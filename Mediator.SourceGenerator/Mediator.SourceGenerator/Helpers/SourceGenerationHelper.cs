namespace Mediator.SourceGenerator.Helpers;

internal static class SourceGenerationHelper
{
    public const string Attribute = @"
using System;

namespace Mediator
{
    public class RegisterMediatorAttribute : Attribute
    {
    }
}";
}
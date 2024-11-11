// ReSharper disable once CheckNamespace
namespace StonkNotes.Common;

// todo: move to a StonkNotes.Common project (when one exists) that lives outside of Domain
/// <summary>
/// Attributes that act as simple markers for a given layer to assist with Assembly Scanning, Dependency Injection, etc.
/// See AssemblyInfo.cs in any given project for its architectural designation.
/// This pattern insulates from name changes, csproj refactorings, msbuild concerns, etc.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ApplicationLayerAssemblyAttribute : Attribute { }


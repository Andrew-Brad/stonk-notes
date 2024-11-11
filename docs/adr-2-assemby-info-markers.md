# ADR 2: Use Custom Attributes To Mark Layers Of Clean Architecture

## Status

Unilaterally Accepted By A.B.

## Context

Use cases exist to pass in an `Assembly` for the purposes of Dependency Injection, or sometimes just useful Reflection helpers.

Ex: `services.AddAutoMapper(Assembly.GetExecutingAssembly());`

Or: `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());`

Using the executing assembly prevents a Unit Testing project from piggybacking this logic. This forces a given unit testing project to duplicate registration logic, potentially hindering future refactorings. It tightly couples test bootstrapping code to Types which it may not otherwise care about. Selecting the Type in that assembly for DI purposes is arbitrary and a bit of a distraction.

## Decision

Introduce marker attributes which carry multiple benefits. They will be referenced now for DI registration purposes, but are also part of a longer term Architectural linting effort, where a Type-Safe reference is needed to "the application layer", without polluting code with magic strings tied to project names or file paths.

Ex:

```C#
[AttributeUsage(AttributeTargets.Assembly)]
public class ApplicationLayerAssemblyAttribute : Attribute { }
```

Then in `AssemblyInfo.cs`:

```C#
[assembly: StonkNotes.Common.ApplicationLayerAssembly]
```

It's arguably cleaner than an empty marker `class` or `interface`.

## Consequences

- **DRY up DI registration code**
- **Marker Interfaces More SOLID**
- **Enable future Arch Linting Efforts**

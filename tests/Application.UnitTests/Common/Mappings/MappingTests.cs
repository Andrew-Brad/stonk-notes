using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StonkNotes.Application.Common.Models;
using StonkNotes.Application.DayNotes.Queries.GetDayNoteById;
using StonkNotes.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using StonkNotes.Domain.Entities;

namespace StonkNotes.Application.UnitTests.Common.Mappings;

[TestFixture]
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = ApplicationLayerServiceProviderFixture.ServiceProvider.GetRequiredService<IConfigurationProvider>();
        _mapper = ApplicationLayerServiceProviderFixture.ServiceProvider.GetRequiredService<IMapper>();
    }

    [Test]
    public void ShouldHaveValidConfiguration() => _configuration.AssertConfigurationIsValid();

    [Test]
    //[TestCase(typeof(TodoList), typeof(TodoListDto))]
    [TestCase(typeof(DayNote), typeof(DayNoteDto))]
    [TestCase(typeof(TodoList), typeof(LookupDto))]
    [TestCase(typeof(TodoItem), typeof(LookupDto))]
    [TestCase(typeof(TodoItem), typeof(TodoItemBriefDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);
        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}

using Catalogo.Domain.Abstractions;

namespace Catalogo.Domain.Categories.Events;

public sealed record CategoryCreatedDomainEvent(Guid categoryId): IDomainEvent;
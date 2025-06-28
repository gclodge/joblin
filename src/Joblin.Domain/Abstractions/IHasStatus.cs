using System;

namespace Joblin.Domain.Abstractions;

public interface IHasStatus
{
    Status Status { get; }
}

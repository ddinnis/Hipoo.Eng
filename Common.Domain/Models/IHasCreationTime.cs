using System;

namespace Common.Domain.Models
{
    public interface IHasCreationTime
    {
        DateTime CreationTime { get; }
    }
}

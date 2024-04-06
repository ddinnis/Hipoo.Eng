using System;

namespace Common.Domain.Models
{
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; }

    }
}

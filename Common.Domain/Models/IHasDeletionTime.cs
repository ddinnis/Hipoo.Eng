using System;

namespace Common.Domain.Models
{
    public interface IHasDeletionTime
    {
        DateTime? DeletionTime { get; }
    }
}

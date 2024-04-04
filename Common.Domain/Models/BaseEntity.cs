using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Domain.Models
{
    public class BaseEntity : IEntity, IDomainEvents
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        //告诉ORM忽略特定的属性，使之不被映射
        [NotMapped]
        private List<INotification> domainEvents = new List<INotification>(); 
        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents.Add(eventItem);
        }

        public void AddDomainEventIfAbsent(INotification eventItem)
        {
            if (!domainEvents.Contains(eventItem))
            {
                domainEvents.Add(eventItem);
            }
        }
        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
            return domainEvents;
        }
    }
}

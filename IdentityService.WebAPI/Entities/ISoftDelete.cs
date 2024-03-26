namespace IdentityService.Domain.Entities
{
    internal interface ISoftDelete
    {
        bool IsDeleted { get; }
        void SoftDelete();
    }
}
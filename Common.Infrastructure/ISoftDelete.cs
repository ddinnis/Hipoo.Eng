namespace IdentityService.Domain
{
    internal interface ISoftDelete
    {
        bool IsDeleted { get; }
        void SoftDelete();
    }
}
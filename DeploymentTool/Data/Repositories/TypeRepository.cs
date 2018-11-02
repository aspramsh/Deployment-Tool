namespace Data.Repositories
{
    public class TypeRepository : BaseRepository<Models.Type>, ITypeRepository
    {
        public TypeRepository(ProjectPublisherContext context) : base(context)
        {
            
        }

    }
}

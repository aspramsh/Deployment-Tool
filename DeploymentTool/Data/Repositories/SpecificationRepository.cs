using Data.Models;

namespace Data.Repositories
{
    public class SpecificationRepository : BaseRepository<DeploymentSpecification>, ISpecificationRepository
    {
        public SpecificationRepository(ProjectPublisherContext context) : base(context)
        {

        }
    }
}

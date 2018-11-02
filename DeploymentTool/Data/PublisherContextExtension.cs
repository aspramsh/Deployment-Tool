using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public static class PublisherContextExtension
    {
        public static void EnsureSeedDataForContext(this ProjectPublisherContext context)
        {
            if (!context.Type.Any())
            {
                var types = new List<Models.Type>
                {
                    new Models.Type
                    {
                        Id = 1,
                        Name = "DotNetCore"
                    },

                    new Models.Type
                    {
                        Id = 2,
                        Name = "DotNetStandard"
                    }
                };
                context.Type.AddRange(types);
                context.SaveChanges();
            }
        }
    }
}

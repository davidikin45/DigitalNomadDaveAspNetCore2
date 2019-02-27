using AspNetCore.Base.Domain;

namespace DND.Domain.CMS.Projects
{
    public class Project : EntityAggregateRootBase<int>
    {
        //[Required, StringLength(100)]
        public string Name { get; set; }

        public string Link { get; set; }

        public string File { get; set; }

        public string Album { get; set; }

        //[Required, StringLength(200)]
        public string DescriptionText { get; set; }
    }
}

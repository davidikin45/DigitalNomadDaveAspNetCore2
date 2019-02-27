using AspNetCore.Base.Domain;

namespace DND.Domain.CMS.CarouselItems
{
    public class CarouselItem : EntityAggregateRootBase<int>
    {

        public string Title { get; set; }

        //[StringLength(200)]
        public string CarouselText
        { get; set; }

        public string Album { get; set; }

        public string ButtonText { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }

        //[Required]
        public bool OpenInNewWindow
        { get; set; }

        //[Required]
        public bool Published
        { get; set; }


        public CarouselItem()
        {

        }
    }
}

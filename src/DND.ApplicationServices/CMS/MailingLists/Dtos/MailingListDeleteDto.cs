using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.MailingLists;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.CMS.MailingLists.Dtos
{
    public class MailingListDeleteDto : DtoAggregateRootBase<int>, IMapTo<MailingList>, IMapFrom<MailingList>
    {

    }
}

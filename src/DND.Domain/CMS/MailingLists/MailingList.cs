using AspNetCore.Base.Domain;

namespace DND.Domain.CMS.MailingLists
{
    public class MailingList : EntityAggregateRootBase<int>
    {
        public string Name { get; set; }

        //[Required, EmailAddress]
        public string Email { get; set; }
    }
}

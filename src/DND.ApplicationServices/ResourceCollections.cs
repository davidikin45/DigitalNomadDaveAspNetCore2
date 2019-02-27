using AspNetCore.Base;

namespace DND.ApplicationServices
{
    public class ResourceCollections
    {
        public class Blog
        {
            public class Authors
            {
                public const string CollectionId = "blog.authors";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class BlogPosts
            {
                public const string CollectionId = "blog.blog-posts";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Categories
            {
                public const string CollectionId = "blog.categories";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Locations
            {
                public const string CollectionId = "blog.locations";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Tags
            {
                public const string CollectionId = "blog.tags";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }
        }

        public class CMS
        {
            public class CarouselItems
            {
                public const string CollectionId = "cms.carousel-items";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class ContentHtmls
            {
                public const string CollectionId = "blog.content-htmls";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class ContentTexts
            {
                public const string CollectionId = "blog.content-texts";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Faqs
            {
                public const string CollectionId = "blog.faqs";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class MailingList
            {
                public const string CollectionId = "cms.mailing-list";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Projects
            {
                public const string CollectionId = "cms.projects";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }

            public class Testimonials
            {
                public const string CollectionId = "cms.testimonials";

                public class Operations
                {

                }

                public class Scopes
                {
                    public const string Create = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                    public const string Read = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                    public const string ReadOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                    public const string Update = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                    public const string UpdateOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                    public const string Delete = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                    public const string DeleteOwner = CollectionId + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
                }
            }
        }
    }
}

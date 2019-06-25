using AspNetCore.Base.Settings;
using AspNetCore.Mvc.Extensions;
using AspNetCore.Mvc.Extensions.Attributes.Display;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Base.Attributes.Display
{
    public class FolderAppSettingsDropdownAttribute : FolderDropdownAttribute
    {
        public string FolderFolderId { get; set; }

        public FolderAppSettingsDropdownAttribute(string folderId, Boolean nullable = false)
            : base("", nullable)
        {
            FolderFolderId = folderId;
        }

        public FolderAppSettingsDropdownAttribute(string folderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
            : base("", displayExpression, orderByProperty, orderByType, nullable)
        {
            FolderFolderId = folderId;
        }

        public override void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            base.TransformMetadata(context, serviceProvider);

            if (!string.IsNullOrEmpty(FolderFolderId))
            {
                var hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();
                var appSettings = serviceProvider.GetRequiredService<AppSettings>();
                modelMetadata.AdditionalValues["PhysicalFolderPath"] = hostingEnvironment.MapWwwPath(appSettings.Folders[FolderFolderId]);
            }
        }
    }
}

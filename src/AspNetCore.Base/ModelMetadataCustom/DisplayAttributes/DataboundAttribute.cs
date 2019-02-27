using AspNetCore.Base.Extensions;
using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    //Composition Properties (1-To-Many, child cannot exist independent of the parent) 
    public class RepeaterAttribute : DataboundAttribute
    {
        public RepeaterAttribute(string displayExpression)
            : base(displayExpression)
        {

        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class FolderDropdownAttribute : DataboundAttribute
    {
        public FolderDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.FullName), nameof(DirectoryInfo.LastWriteTime), OrderByType.Descending, nullable)
        {

        }

        public FolderDropdownAttribute(string folderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
            : base(folderId, "", displayExpression, orderByProperty, orderByType, nullable)
        {
        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class FileDropdownAttribute : DataboundAttribute
    {
        public FileDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.Name), nameof(DirectoryInfo.LastWriteTime), OrderByType.Descending, nullable)
        {

        }

        public FileDropdownAttribute(string folderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
            : base("", folderId, displayExpression, orderByProperty, orderByType, nullable)
        {
        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class DropdownAttribute : DataboundAttribute
    {
        public DropdownAttribute(IEnumerable<string> options)
         : base("ModelDropdown", null, null, null, null, null, false, null, options)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression)
            : base("ModelDropdown", dropdownModelType, "Id", displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression, string valueExpression)
        : base("ModelDropdown", dropdownModelType, valueExpression, displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression, string orderByProperty, string orderByType)
           : base("ModelDropdown", dropdownModelType, "Id", displayExpression, orderByProperty, orderByType, false, null)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression, string orderByProperty, string orderByType, string bindingProperty)
          : base("ModelDropdown", dropdownModelType, "Id", displayExpression, orderByProperty, orderByType, false, bindingProperty)
        {

        }
    }

    public class YesNoCheckboxOrRadioAttribute : CheckboxOrRadioAttribute
    {
        public YesNoCheckboxOrRadioAttribute()
        :base(new List<string>() { "Yes", "No" })
        {

        }
    }

    public class TrueFalseCheckboxOrRadioAttribute : CheckboxOrRadioAttribute
    {
        public TrueFalseCheckboxOrRadioAttribute()
        : base(new List<string>() { "True", "False" })
        {

        }
    }

    public class CheckboxOrRadioAttribute : DataboundAttribute
    {
        public CheckboxOrRadioAttribute(IEnumerable<string> options)
           : base("ModelCheckboxOrRadio", null, null, null, null, null, false, null, options)
        {

        }

        public CheckboxOrRadioAttribute(Type selectorModelType, string displayExpression)
            : base("ModelCheckboxOrRadio", selectorModelType, "Id", displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public CheckboxOrRadioAttribute(Type selectorModelType, string displayExpression, string valueExpression)
        : base("ModelCheckboxOrRadio", selectorModelType, valueExpression, displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public CheckboxOrRadioAttribute(Type selectorModelType, string displayExpression, string orderByProperty, string orderByType)
           : base("ModelCheckboxOrRadio", selectorModelType, "Id", displayExpression, orderByProperty, orderByType, false, null)
        {

        }

        public CheckboxOrRadioAttribute(Type selectorModelType, string displayExpression, string valueExpression, string orderByProperty, string orderByType, string bindingProperty)
          : base("ModelCheckboxOrRadio", selectorModelType, valueExpression, displayExpression, orderByProperty, orderByType, false, bindingProperty)
        {

        }
    }

    public class YesNoCheckboxOrRadioButtonsAttribute : CheckboxOrRadioButtonsAttribute
    {
        public YesNoCheckboxOrRadioButtonsAttribute()
        : base(new List<string>() { "Yes", "No" })
        {

        }
    }

    public class TrueFalseCheckboxOrRadioButtonsAttribute : CheckboxOrRadioButtonsAttribute
    {
        public TrueFalseCheckboxOrRadioButtonsAttribute()
        : base(new List<string>() { "True", "False" })
        {

        }
    }

    public class CheckboxOrRadioButtonsAttribute : DataboundAttribute
    {
        public CheckboxOrRadioButtonsAttribute(IEnumerable<string> options)
           : base("ModelCheckboxOrRadioButtons", null, null, null, null, null, false, null, options)
        {

        }

        public CheckboxOrRadioButtonsAttribute(Type selectorModelType, string displayExpression)
            : base("ModelCheckboxOrRadioButtons", selectorModelType, "Id", displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public CheckboxOrRadioButtonsAttribute(Type selectorModelType, string displayExpression, string valueExpression)
        : base("ModelCheckboxOrRadioButtons", selectorModelType, valueExpression, displayExpression, "Id", OrderByType.Descending, false, null)
        {

        }

        public CheckboxOrRadioButtonsAttribute(Type selectorModelType, string displayExpression, string orderByProperty, string orderByType)
           : base("ModelCheckboxOrRadioButtons", selectorModelType, "Id", displayExpression, orderByProperty, orderByType, false, null)
        {

        }

        public CheckboxOrRadioButtonsAttribute(Type selectorModelType, string displayExpression, string valueExpression, string orderByProperty, string orderByType, string bindingProperty)
          : base("ModelCheckboxOrRadioButtons", selectorModelType, valueExpression, displayExpression, orderByProperty, orderByType, false, bindingProperty)
        {

        }
    }

    public class CheckboxOrRadioInlineAttribute : Attribute, IDisplayMetadataAttribute
    {
        public bool Inline { get; set; } = true;

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["ModelCheckboxOrRadioInline"] = Inline;
        }
    }

    public abstract class DataboundAttribute : Attribute, IDisplayMetadataAttribute
    {
        public Type DropdownModelType { get; set; }
        public string KeyProperty { get; set; }
        public string BindingProperty { get; set; }
        public string DisplayExpression { get; set; }
        public string OrderByProperty { get; set; }
        public string OrderByTypeString { get; set; }

        public string FolderFolderId { get; set; }
        public string PhysicalFolderPath { get; set; }

        public string FileFolderId { get; set; }
        public string PhysicalFilePath { get; set; }

        public Boolean Nullable { get; set; }

        public string DataTypeName { get; set; }

        public IEnumerable<string> Options { get; set; }

        public DataboundAttribute(string folderFolderId, string fileFolderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
        {
            DataTypeName = "ModelDropdown";

            FolderFolderId = folderFolderId;
            FileFolderId = fileFolderId;

            DisplayExpression = displayExpression;
            OrderByProperty = orderByProperty;
            OrderByTypeString = orderByType;

            Nullable = nullable;
        }

        //typeof
        //nameof
        public DataboundAttribute(string dataTypeName, Type dropdownModelType, string keyProperty, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false, string bindingProperty = null, IEnumerable<string> options = null)
        {
            DataTypeName = dataTypeName;

            DropdownModelType = dropdownModelType;
            KeyProperty = keyProperty;
            DisplayExpression = displayExpression;

            BindingProperty = bindingProperty;

            OrderByProperty = orderByProperty;
            OrderByTypeString = orderByType;
            Nullable = nullable;

            Options = options;
        }

        public DataboundAttribute(string displayExpression)
        {
            DataTypeName = "ModelRepeater";

            DisplayExpression = displayExpression;

            KeyProperty = "Id";
            BindingProperty = "Id";
        }

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = DataTypeName;

            modelMetadata.AdditionalValues["IsDatabound"] = true;

            //Select from Db
            modelMetadata.AdditionalValues["DropdownModelType"] = DropdownModelType;
            modelMetadata.AdditionalValues["OrderByProperty"] = OrderByProperty;
            modelMetadata.AdditionalValues["OrderByType"] = OrderByTypeString;

            modelMetadata.AdditionalValues["KeyProperty"] = KeyProperty; //Used for dropdown 
            modelMetadata.AdditionalValues["DisplayExpression"] = DisplayExpression; //Used for Dropdown and Display Text

            modelMetadata.AdditionalValues["BindingProperty"] = BindingProperty;

            modelMetadata.AdditionalValues["Nullable"] = Nullable;

            modelMetadata.AdditionalValues["FolderFolderId"] = FolderFolderId;
            modelMetadata.AdditionalValues["PhysicalFolderPath"] = PhysicalFolderPath;
            if(!string.IsNullOrEmpty(FolderFolderId))
            {
                var hostingEnvironment = (IHostingEnvironment)serviceProvider.GetService(typeof(IHostingEnvironment));
                var appSettings = (AppSettings)serviceProvider.GetService(typeof(AppSettings));
                modelMetadata.AdditionalValues["PhysicalFolderPath"] = hostingEnvironment.MapWwwPath(appSettings.Folders[FolderFolderId]);
            }

            modelMetadata.AdditionalValues["PhysicalFilePath"] = PhysicalFilePath;
            if (!string.IsNullOrEmpty(FileFolderId))
            {
                var hostingEnvironment = (IHostingEnvironment)serviceProvider.GetService(typeof(IHostingEnvironment));
                var appSettings = (AppSettings)serviceProvider.GetService(typeof(AppSettings));
                modelMetadata.AdditionalValues["PhysicalFilePath"] = hostingEnvironment.MapWwwPath(appSettings.Folders[FileFolderId]);
            }

            modelMetadata.AdditionalValues["Options"] = Options;
        }
    }

}
namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;

    public class PackageInfoAttribute
    {
        public PackageInfoAttribute()
        {
            
        }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PackageExportAttribute : ExportAttribute
    {
        public PackageExportAttribute()
            : base(typeof(IOperatorsPackage))
        {
        }
       
    }

}
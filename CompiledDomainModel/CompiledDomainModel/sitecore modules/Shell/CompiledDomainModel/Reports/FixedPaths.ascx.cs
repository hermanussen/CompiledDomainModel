using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CompiledDomainModel.Attributes;
using System.ComponentModel;
using CompiledDomainModel.Utils;

namespace CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.Reports
{
    public partial class FixedPaths : System.Web.UI.UserControl
    {
        public class FixedPathItem
        {
            public FixedPathAttribute Attribute { get; private set; }

            public FixedPathItem Parent { get; set; }

            public List<FixedPathItem> Children { get; private set; }

            public FixedPathItem(FixedPathAttribute attribute)
            {
                Attribute = attribute;
                Children = new List<FixedPathItem>();
            }

            internal void Render(HtmlTextWriter writer, string database)
            {
                if (Attribute.DatabaseNames.Contains(database))
                {
                    writer.WriteLine(string.Format("<div class=\"fixedpath\">{0} {1}",
                        Attribute.FullPath.Substring(Attribute.FullPath.LastIndexOf('/') + 1),
                        Attribute.DomainObjectType != null ? string.Format("<span class=\"domainobjectname\">{0}</span>", Attribute.DomainObjectType.Name) : ""));
                    foreach (FixedPathItem child in Children)
                    {
                        child.Render(writer, database);
                    }
                    writer.WriteLine("</div>");
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            IEnumerable<FixedPathAttribute> typesWithMyAttribute =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(FixedPathAttribute), true)) ?? new object[0]
                where attributes != null && attributes.Length > 0
                select attributes.Cast<FixedPathAttribute>()).SelectMany(attr => attr);
            List<FixedPathItem> allAttributes = typesWithMyAttribute.Select(attr => new FixedPathItem(attr)).ToList();

            // Resolve child relationships
            foreach (FixedPathItem attribute in allAttributes)
            {
                attribute.Children.AddRange(from attr in allAttributes
                                                where attr.Attribute.FullPath.StartsWith(attribute.Attribute.FullPath)
                                                && ! attr.Attribute.FullPath.Equals(attribute.Attribute.FullPath)
                                                && ! attr.Attribute.FullPath.Substring(attribute.Attribute.FullPath.Length + 2).Contains('/')
                                                select attr);
            }

            // Resolve parent relationships
            foreach (FixedPathItem attribute in allAttributes.ToList())
            {
                foreach (FixedPathItem child in attribute.Children)
                {
                    child.Parent = attribute;
                }
            }

            // Generate an overview for each database
            foreach (string database in allAttributes.SelectMany(attr => attr.Attribute.DatabaseNames).Distinct())
            {
                writer.WriteLine(string.Format("<h2>Database \"{0}\"</h2>", database));

                foreach (FixedPathItem baseItem in allAttributes.Where(attr => attr.Parent == null))
                {
                    baseItem.Render(writer, database);
                }
            }

            base.Render(writer);
        }
    }
}
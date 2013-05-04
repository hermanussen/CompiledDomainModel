using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CompiledDomainModel.Attributes;
using System.Text;
using CompiledDomainModel.Utils;

namespace CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.Reports
{
    public partial class Diagram : System.Web.UI.UserControl
    {
        public class TemplateField
        {
            private DomainObjectFieldAttribute FieldAttribute { get; set; }

            public TemplateField(DomainObjectFieldAttribute fieldAttribute)
            {
                FieldAttribute = fieldAttribute;
            }
            public void Render(HtmlTextWriter writer)
            {
                writer.WriteLine(string.Format("<li>{0} - {2}</li>", FieldAttribute.FieldName, FieldAttribute.FieldId, FieldAttribute.FieldType));
            }
        }

        public class Template
        {
            public string TemplateClassName { get; private set; }

            public List<TemplateField> TemplateFields { get; private set; }

            public IEnumerable<Template> SubTypes { get; private set; }

            public DomainObjectAttribute TemplateAttribute { get; private set; }
            
            public Template(Type templateType)
            {
                TemplateClassName = templateType.Name;

                object[] templateAttributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => templateType.GetCustomAttributes(typeof(DomainObjectAttribute), false));
                if (templateAttributes != null && templateAttributes.Count() > 0)
                {
                    TemplateAttribute = templateAttributes.Cast<DomainObjectAttribute>().First();
                }

                SubTypes = ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => templateType.Assembly.GetTypes() ?? new Type[0]).Where(type => templateType.Equals(type.BaseType)).Select(type => new Template(type));

                TemplateFields = new List<TemplateField>();
                IEnumerable<DomainObjectFieldAttribute> fieldAttributes =
                    (from m in templateType.GetMembers()
                     let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => m.GetCustomAttributes(typeof(VersionAttribute), true)) ?? new object[0]
                     where attributes != null && attributes.Count() > 0
                     select attributes.Cast<DomainObjectFieldAttribute>()).SelectMany(attrs => attrs);
                foreach (DomainObjectFieldAttribute fieldAttribute in fieldAttributes)
                {
                    TemplateFields.Add(new TemplateField(fieldAttribute));
                }
            }

            public void Render(HtmlTextWriter writer)
            {
                writer.WriteLine("<div class=\"diagram\">");

                writer.WriteFullBeginTag("div");

                writer.WriteLine(string.Format("<p class=\"template\">{0} ({1})</p>", TemplateClassName, TemplateAttribute != null ? TemplateAttribute.TemplateName : "no template"));

                if (TemplateFields.Count() > 0)
                {
                    writer.WriteFullBeginTag("ul");
                    foreach (TemplateField field in TemplateFields)
                    {
                        field.Render(writer);
                    }
                    writer.WriteEndTag("ul");
                }

                writer.WriteEndTag("div");

                if (SubTypes.Count() > 0)
                {
                    writer.WriteFullBeginTag("div");
                    foreach (Template template in SubTypes)
                    {
                        writer.WriteFullBeginTag("div");
                        template.Render(writer);
                        writer.WriteEndTag("div");
                    }
                    writer.WriteEndTag("div");
                }

                writer.WriteLine("</div>");
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var versionCheckAttribute =
                        from a in AppDomain.CurrentDomain.GetAssemblies()
                        from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                        let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(VersionAttribute), true)) ?? new object[0]
                        where attributes != null && attributes.Length > 0
                        select new { Type = t, Attribute = attributes.Cast<VersionAttribute>().First() };
            if (versionCheckAttribute.Count() > 0)
            {
                var attribute = versionCheckAttribute.First();
                writer.WriteLine(string.Format("<p>Domain model version {0} generated with version {1} of the CompiledDomainModel module.</p>", attribute.Attribute.DomainModelVersion, attribute.Attribute.SitecoreModuleVersion));
                
                new Template(attribute.Type).Render(writer);
                
            }
            else
            {
                writer.WriteLine("<p>No domain model could be found</p>");
            }
            base.Render(writer);
        }
    }
}
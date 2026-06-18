using System.Collections.Generic;

namespace MqlGenerator
{
    internal sealed class AttributeDefinition
    {
        public string CreatedDate = string.Empty;
        public string CreatedBy = string.Empty;
        public string DescriptionKo = string.Empty;
        public string Name = string.Empty;
        public string DescriptionEn = string.Empty;
        public string Type = string.Empty;
        public string DefaultValue = string.Empty;
        public readonly List<string> Ranges = new List<string>();
        public bool Multiline;
        public bool Hidden;
        public string Application = string.Empty;
        public string Version = string.Empty;
        public string Installer = string.Empty;
        public string InstalledDate = string.Empty;
    }

    internal static class AttributeGenerator
    {
        internal static string Generate(AttributeDefinition definition)
        {
            string descriptionComment = string.Empty;
            if (!string.IsNullOrWhiteSpace(definition.DescriptionKo) &&
                definition.DescriptionKo != definition.DescriptionEn)
            {
                descriptionComment = " #" + definition.DescriptionKo;
            }

            string ranges = string.Empty;
            foreach (string range in definition.Ranges)
            {
                ranges += "    range = '" + EscapeMql(range) + "'\r\n";
            }

            var tokens = new Dictionary<string, string>();
            tokens["{{CREATED_DATE}}"] = definition.CreatedDate;
            tokens["{{CREATED_BY}}"] = definition.CreatedBy;
            tokens["{{DESCRIPTION_KO}}"] = definition.DescriptionKo;
            tokens["{{ATTRIBUTE_NAME}}"] = definition.Name;
            tokens["{{DESCRIPTION_EN}}"] = definition.DescriptionEn;
            tokens["{{DESCRIPTION_COMMENT}}"] = descriptionComment;
            tokens["{{ATTRIBUTE_TYPE}}"] = definition.Type;
            tokens["{{DEFAULT_VALUE}}"] = EscapeMql(definition.DefaultValue);
            tokens["{{RANGES}}"] = ranges;
            tokens["{{MULTILINE_FLAG}}"] = definition.Multiline ? "multiline" : "notmultiline";
            tokens["{{HIDDEN_FLAG}}"] = definition.Hidden ? "hidden" : "nothidden";
            tokens["{{APPLICATION}}"] = definition.Application;
            tokens["{{VERSION}}"] = definition.Version;
            tokens["{{INSTALLER}}"] = definition.Installer;
            tokens["{{INSTALLED_DATE}}"] = definition.InstalledDate;

            string content = TemplateText();
            foreach (var token in tokens)
            {
                content = content.Replace(token.Key, token.Value);
            }

            return NormalizeLineEndings(content);
        }

        internal static string NormalizeLineEndings(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\r\n");
        }

        private static string EscapeMql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string TemplateText()
        {
            return @"################################################################################################
# Created Date : {{CREATED_DATE}}
# Create by {{CREATED_BY}}
# Description : {{DESCRIPTION_KO}}
################################################################################################
#del attribute '{{ATTRIBUTE_NAME}}';
add attribute '{{ATTRIBUTE_NAME}}'
    description '{{DESCRIPTION_EN}}'{{DESCRIPTION_COMMENT}}
    type '{{ATTRIBUTE_TYPE}}'
    default '{{DEFAULT_VALUE}}'
{{RANGES}}    {{MULTILINE_FLAG}}
    {{HIDDEN_FLAG}}
    property    'application'    value '{{APPLICATION}}'
    property    'version'        value '{{VERSION}}'
    property    'installer'      value '{{INSTALLER}}'
    property    'installed date' value '{{INSTALLED_DATE}}'
    property    'original name'  value '{{ATTRIBUTE_NAME}}'
;
add property 'attribute_{{ATTRIBUTE_NAME}}' on program eServiceSchemaVariableMapping.tcl to attribute '{{ATTRIBUTE_NAME}}';";
        }
    }
}

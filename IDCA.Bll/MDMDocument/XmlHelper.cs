using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IDCA.Bll.MDMDocument
{
    internal static class XmlHelper
    {

        private static string ReadPropertyStringValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return (attr = element.Attribute(propertyName)) != null ? attr.Value : string.Empty;
        }

        private static bool ReadPropertyBoolValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return (attr = element.Attribute(propertyName)) != null && attr.Value == "-1";
        }

        private static void ForEachChild(XElement element, string nodeName, Action<XElement> action)
        {
            foreach (var ele in element.Elements(nodeName))
            {
                action(ele);
            }
        }

        private static void FirstChild(XElement element, string nodeName, Action<XElement> action)
        {
            var first = element.Element(nodeName);
            if (first == null)
            {
                return;
            }
            action(first);
        }

        private static PropertyValueType GetPropertyValueType(XAttribute? attribute)
        {
            if (attribute == null)
            {
                return PropertyValueType.Text;
            }

            if (int.TryParse(attribute.Value, out int type))
            {
                return (PropertyValueType)type;
            }

            return PropertyValueType.Text;
        }

        internal static Property ReadProperty(Property property, XElement element)
        {
            property.Name = ReadPropertyStringValue(element, "name");
            property.Value = ReadPropertyStringValue(element, "value");
            property.Context = ReadPropertyStringValue(element, "context");
            property.Type = GetPropertyValueType(element.Attribute("type"));
            return property;
        }

        internal static Property ReadProperty(Properties properties, XElement element)
        {
            return ReadProperty(properties.NewObject(), element);
        }

        internal static Properties ReadProperties(Properties properties, XElement element)
        {
            ForEachChild(element, "properties", ele => properties.SubProperties = ReadProperties(properties, ele));
            ForEachChild(element, "property", ele => properties.Add(ReadProperty(properties, ele)));
            return properties;
        }

        internal static Label ReadLabel(Label label, XElement element)
        {
            XAttribute? attr;
            string context = ReadPropertyStringValue(element, "context");
            string language = (attr = element.LastAttribute) != null ? attr.Value : string.Empty;
            label.Set(context, language, element.Value);
            return label;
        }

        internal static Label ReadLabel(Labels labels, XElement element)
        {
            return ReadLabel(labels.NewObject(), element);
        }

        internal static Labels ReadLabels(Labels labels, XElement element)
        {
            ForEachChild(element, "text", text => labels.Add(ReadLabel(labels, text)));
            return labels;
        }

        internal static List<string> ReadAtoms(List<string> atoms, XElement element)
        {
            ForEachChild(element, "atom", atom => atoms.Add(ReadPropertyStringValue(element, "name")));
            return atoms;
        }
    
        internal static void ReadCategoryId(CategoryMap categoryMap, XElement element)
        {
            categoryMap.Add(ReadPropertyStringValue(element, "name"), ReadPropertyStringValue(element, "value"));
        }

        internal static CategoryMap ReadCategoryMap(CategoryMap categoryMap, XElement element)
        {
            ForEachChild(element, "categoryid", category => ReadCategoryId(categoryMap, category));
            return categoryMap;
        }

        internal static MDMUser ReadUser(MDMUser user, XElement element)
        {
            user.Name = ReadPropertyStringValue(element, "name");
            user.FileVersion = ReadPropertyStringValue(element, "fileversion");
            user.Comment = ReadPropertyStringValue(element, "comment");
            return user;
        }

        internal static SaveLog ReadSaveLog(SaveLog saveLog, XElement element)
        {
            saveLog.FileVersion = ReadPropertyStringValue(element, "fileversion");
            saveLog.VersionSet = ReadPropertyStringValue(element, "versionset");
            saveLog.UserName = ReadPropertyStringValue(element, "username");
            saveLog.Date = Convert.ToDateTime(ReadPropertyStringValue(element, "date"));
            if (int.TryParse(ReadPropertyStringValue(element, "count"), out int count))
            {
                saveLog.SaveCount = count;
            }
            ForEachChild(element, "user", ele => saveLog.Add(ReadUser(saveLog.NewObject(), ele)));
            return saveLog;
        }

        internal static SaveLog ReadSaveLog(SaveLogs saveLogs, XElement element)
        {
            return ReadSaveLog(saveLogs.NewObject(), element);
        }

        internal static SaveLogs ReadSaveLogs(SaveLogs saveLogs, XElement element)
        {
            ForEachChild(element, "savelog", ele => saveLogs.Add(ReadSaveLog(saveLogs, ele)));
            return saveLogs;
        }

        internal static ContextAlternatives ReadContextAlternatives(ContextAlternatives alternatives, XElement element)
        {
            ForEachChild(element, "alternative", ele => alternatives.Add(ReadPropertyStringValue(ele, "name")));
            return alternatives;
        }

        internal static Context ReadContext(Context context, XElement element)
        {
            context.Name = ReadPropertyStringValue(element, "name");
            ForEachChild(element, "alternatives", ele => ReadContextAlternatives((ContextAlternatives)context.NewAlternatives(), ele));
            return context;
        }

        internal static Context ReadContext(Contexts contexts, XElement element)
        {
            return ReadContext(contexts.NewObject(), element);
        }

        internal static Contexts ReadContexts(Contexts contexts, XElement element)
        {
            contexts.Base = ReadPropertyStringValue(element, "base");
            ForEachChild(element, "context", ele => contexts.Add(ReadContext(contexts, ele)));
            return contexts;
        }

        internal static Language ReadLanguage(Language language, XElement element)
        {
            language.Name = ReadPropertyStringValue(element, "name");
            language.Id = ReadPropertyStringValue(element, "id");
            var properties = element.Element("properties");
            if (properties != null)
            {
                var propertiesObj = new Properties(language);
                language.Properties = ReadProperties(propertiesObj, properties);
            }
            return language;
        }

        internal static Languages ReadLanguages(Languages languages, XElement element)
        {
            languages.Base = ReadPropertyStringValue(element, "base");
            ForEachChild(element, "language", ele => languages.Add(ReadLanguage(languages.NewObject(), ele)));
            return languages;
        }

        internal static Script ReadScript(Script script, XElement element)
        {
            script.Name = ReadPropertyStringValue(element, "name");
            script.Default = ReadPropertyBoolValue(element, "default");
            script.Text = element.Value;
            return script;
        }

        internal static InterviewModes GetInterviewMode(XAttribute? attr)
        {
            if (attr == null || !int.TryParse(attr.Value, out int iValue))
            {
                return InterviewModes.Default;
            }
            return (InterviewModes)iValue;
        }

        internal static ScriptType ReadScriptType(ScriptType scriptType, XElement element)
        {
            scriptType.Type = ReadPropertyStringValue(element, "type");
            scriptType.Context = ReadPropertyStringValue(element, "context");
            scriptType.InterviewMode = GetInterviewMode(element.Attribute("interviewmodes"));
            scriptType.UseKeyCodes = ReadPropertyBoolValue(element, "usekeycodes");
            return scriptType;
        }

        internal static Scripts ReadScripts(Scripts scripts, XElement element)
        {
            ForEachChild(element, "scripttype", ele => scripts.Add(ReadScriptType(scripts.NewObject(), ele)));
            return scripts;
        }

        internal static RoutingItem ReadRoutingItem(RoutingItem item, XElement element)
        {
            item.Name = ReadPropertyStringValue(element, "name");
            item.Item = ReadPropertyStringValue(element, "item");
            return item;
        }

        internal static Routing ReadRouting(Routing routing, XElement element)
        {
            routing.Context = ReadPropertyStringValue(element, "context");
            routing.InterviewMode = GetInterviewMode(element.Attribute("interviewmodes"));
            routing.UseKeyCodes = ReadPropertyBoolValue(element, "usekeycodes");
            ForEachChild(element, "ritem", ele => routing.Add(ReadRoutingItem(routing.NewObject(), ele)));
            return routing;
        }

        internal static Routings ReadRoutings(Routings routings, XElement element)
        {
            FirstChild(element, "scripts", ele => routings.Scripts = ReadScripts(new Scripts(routings), ele));
            ForEachChild(element, "routing", ele => routings.Add(ReadRouting(routings.NewObject(), ele)));
            return routings;
        }

        internal static Page ReadPage(Page page, XElement element)
        {
            page.Id = ReadPropertyStringValue(element, "id");
            page.Name = ReadPropertyStringValue(element, "name");
            return page;
        }

        internal static Pages ReadPages(Pages pages, XElement element)
        {
            ForEachChild(element, "page", ele => pages.Add(ReadPage(pages.NewObject(), ele)));
            return pages;
        }

        private static SourceType GetVariableInstanceSourceType(XAttribute? attr)
        {
            if (attr == null || !int.TryParse(attr.Value, out int iValue))
            {
                return SourceType.None;
            }
            return (SourceType)iValue;
        }

        internal static VariableInstance ReadVariableInstance(VariableInstance variableInstance, XElement element)
        {
            variableInstance.Name = ReadPropertyStringValue(element, "name");
            variableInstance.SourceType = GetVariableInstanceSourceType(element.Attribute("sourcetype"));
            variableInstance.Variable = variableInstance.Document.Variables.GetById(ReadPropertyStringValue(element, "variable"));
            variableInstance.FullName = ReadPropertyStringValue(element, "fullname");
            return variableInstance;
        }

        internal static Mapping ReadMapping(Mapping mapping, XElement element)
        {
            ForEachChild(element, "varinstance", ele => mapping.Add(ReadVariableInstance(mapping.NewObject(), ele)));
            return mapping;
        }

    }
}

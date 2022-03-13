using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IDCA.Bll.MDMDocument
{
    internal static class XmlHelper
    {
        internal static OutType TryReadElement<OutType>(OutType current, XElement baseElement, string tagName, Func<OutType, XElement, OutType> func)
        {
            XElement? e = baseElement.Element(tagName);
            if (e != null)
            {
                func(current, e);
            }
            return current;
        }

        internal static string ReadPropertyStringValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return (attr = element.Attribute(propertyName)) != null ? attr.Value : string.Empty;
        }

        internal static bool ReadPropertyBoolValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return (attr = element.Attribute(propertyName)) != null && attr.Value == "-1";
        }

        internal static int ReadPropertyIntValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return ((attr = element.Attribute(propertyName)) != null && int.TryParse(attr.Value, out int value)) ? value : 0;
        }

        internal static double ReadPropertyDecimalValue(XElement element, string propertyName)
        {
            XAttribute? attr;
            return ((attr = element.Attribute(propertyName)) != null && double.TryParse(attr.Value, out double value)) ? value : 0;
        }

        internal static T ReadPropertyEnumValue<T>(XAttribute? attr) where T : Enum
        { 
            return (attr != null && int.TryParse(attr.Value, out int value)) ? (T)(object)value : (T)(object)0;
        }

        internal static void ForEachChild(XElement element, string nodeName, Action<XElement> action)
        {
            foreach (var ele in element.Elements(nodeName))
            {
                action(ele);
            }
        }

        internal static void FirstChild(XElement element, string nodeName, Action<XElement> action)
        {
            var first = element.Element(nodeName);
            if (first == null)
            {
                return;
            }
            action(first);
        }

        internal static Property ReadProperty(Property property, XElement element)
        {
            property.Name = ReadPropertyStringValue(element, "name");
            property.Value = ReadPropertyStringValue(element, "value");
            property.Context = ReadPropertyStringValue(element, "context");
            property.Type = ReadPropertyEnumValue<PropertyValueType>(element.Attribute("type"));
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
            if (labels.Context.IsDefault && labels.Document.Contexts.Count > 0)
            {
                string context = ReadPropertyStringValue(element, "context");
                IContext? current;
                if (!string.IsNullOrEmpty(context) && (current = labels.Document.LabelTypes[context]) != null)
                {
                    labels.Context = current;
                }
            }
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
            string longCode = ReadPropertyStringValue(element, "name");
            if (!string.IsNullOrEmpty(longCode))
            {
                language.SetLongCode(longCode);
            }
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

        internal static ScriptType ReadScriptType(ScriptType scriptType, XElement element)
        {
            scriptType.Type = ReadPropertyStringValue(element, "type");
            scriptType.Context = ReadPropertyStringValue(element, "context");
            scriptType.InterviewMode = ReadPropertyEnumValue<InterviewModes>(element.Attribute("interviewmodes"));
            scriptType.UseKeyCodes = ReadPropertyBoolValue(element, "usekeycodes");
            ForEachChild(element, "script", e => scriptType.Add(ReadScript(scriptType.NewObject(), e)));
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
            routing.InterviewMode = ReadPropertyEnumValue<InterviewModes>(element.Attribute("interviewmodes"));
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

        internal static VariableInstance ReadVariableInstance(VariableInstance variableInstance, XElement element)
        {
            variableInstance.Name = ReadPropertyStringValue(element, "name");
            variableInstance.SourceType = ReadPropertyEnumValue<SourceType>(element.Attribute("sourcetype"));
            variableInstance.Variable = variableInstance.Document.Variables.GetById(ReadPropertyStringValue(element, "variable"));
            variableInstance.FullName = ReadPropertyStringValue(element, "fullname");
            return variableInstance;
        }

        internal static Mapping ReadMapping(Mapping mapping, XElement element)
        {
            ForEachChild(element, "varinstance", ele => mapping.Add(ReadVariableInstance(mapping.NewObject(), ele)));
            return mapping;
        }

        internal static ControlStyle? ReadControlStyle(XElement element, string subTagName)
        {
            XElement? styles = element.Element(subTagName);
            if (styles is null)
            { 
                return null; 
            }

            ControlStyle controlStyle = new();
            ForEachChild(styles, "property", e =>
            {
                XAttribute? name = e.Attribute("name");
                if (name is null)
                {
                    return;
                }
                switch (name.Value)
                {
                    case "Accelerator": 
                        controlStyle.Accelerator = ReadPropertyStringValue(e, "value"); 
                        break;
                    case "Type":
                        controlStyle.Type = ReadPropertyEnumValue<ControlType>(e.Attribute("value"));
                        break;
                    case "ReadOnly":
                        controlStyle.ReadOnly = ReadPropertyBoolValue(e, "value");
                        break;
                    default:
                        break;
                }
            });
            return controlStyle;
        }

        internal static CellStyle? ReadCellStyle(XElement element, string subTagName)
        {
            XElement? styles = element.Element(subTagName);
            if (styles is null)
            {
                return null;
            }
            CellStyle cellStyle = new();
            ForEachChild(styles, "property", e =>
            {
                XAttribute? name = e.Attribute("name");
                if (name is null)
                {
                    return;
                }
                switch (name.Value)
                {
                    case "BgColor":
                        cellStyle.BgColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderBottomColor":
                        cellStyle.BorderBottomColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderBottomStyle":
                        cellStyle.BorderBottomStyle = ReadPropertyEnumValue<BorderStyle>(e.Attribute("value"));
                        break;
                    case "BorderBottomWidth":
                        cellStyle.BorderBottomWidth = ReadPropertyIntValue(e, "value");
                        break;
                    case "BorderColor":
                        cellStyle.BorderColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderLeftColor":
                        cellStyle.BorderLeftColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderLeftStyle":
                        cellStyle.BorderLeftStyle = ReadPropertyEnumValue<BorderStyle>(e.Attribute("value"));
                        break;
                    case "BorderLeftWidth":
                        cellStyle.BorderLeftWidth = ReadPropertyIntValue(e, "value");
                        break;
                    case "BorderRightColor":
                        cellStyle.BorderRightColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderRightStyle":
                        cellStyle.BorderRightStyle = ReadPropertyEnumValue<BorderStyle>(e.Attribute("value"));
                        break;
                    case "BorderRightWidth":
                        cellStyle.BorderRightWidth = ReadPropertyIntValue(e, "value");
                        break;
                    case "BorderStyle":
                        cellStyle.BorderStyle = ReadPropertyEnumValue<BorderStyle>(e.Attribute("value"));
                        break;
                    case "BorderTopColor":
                        cellStyle.BorderTopColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "BorderTopStyle":
                        cellStyle.BorderTopStyle = ReadPropertyEnumValue<BorderStyle>(e.Attribute("value"));
                        break;
                    case "BorderTopWidth":
                        cellStyle.BorderTopWidth = ReadPropertyIntValue(e, "value");
                        break;
                    case "Colspan":
                        cellStyle.ColSpan = ReadPropertyIntValue(e, "value");
                        break;
                    case "Height":
                        cellStyle.Height = ReadPropertyStringValue(e, "value");
                        break;
                    case "Padding":
                        cellStyle.Padding = ReadPropertyIntValue(e, "value");
                        break;
                    case "PaddingBottom":
                        cellStyle.PaddingBottom = ReadPropertyIntValue(e, "value");
                        break;
                    case "PaddingLeft":
                        cellStyle.PaddingLeft = ReadPropertyIntValue(e, "value");
                        break;
                    case "PaddingRight":
                        cellStyle.PaddingRight = ReadPropertyIntValue(e, "value");
                        break;
                    case "PaddingTop":
                        cellStyle.PaddingTop = ReadPropertyIntValue(e, "value");
                        break;
                    case "RepeatHeader":
                        cellStyle.RepeatHeader = ReadPropertyIntValue(e, "value");
                        break;
                    case "RowSpan":
                        cellStyle.RowSpan = ReadPropertyIntValue(e, "value");
                        break;
                    case "CWidth":
                        cellStyle.Width = ReadPropertyStringValue(e, "value");
                        break;
                    case "Wrap":
                        cellStyle.Wrap = ReadPropertyBoolValue(e, "value");
                        break;
                    default:
                        break;
                }
            });
            return cellStyle;
        }

        internal static FontStyle? ReadFontStyle(XElement element, string subTagName)
        {
            XElement? styles = element.Element(subTagName);
            if (styles == null)
            {
                return null;
            }
            FontStyle style = new();
            ForEachChild(styles, "property", e =>
            {
                XAttribute? attr = e.Attribute("name");
                if (attr == null)
                {
                    return;
                }
                switch (attr.Value)
                {
                    case "Family":
                        style.Family = ReadPropertyStringValue(e, "value");
                        break;
                    case "Effects":
                        style.Effects = ReadPropertyIntValue(e, "value");
                        break;
                    case "Size":
                        style.Size = ReadPropertyIntValue(e, "value");
                        break;
                    default:
                        break;
                }
            });
            return style;
        }

        internal static AudioStyle? ReadAudioStyle(XElement element, string subTagName)
        {
            XElement? styles = element.Element(subTagName);
            if (styles is null)
            {
                return null;
            }
            AudioStyle style = new();
            ForEachChild(styles, "property", e =>
            {
                XAttribute? name = e.Attribute("name");
                if (name is null)
                {
                    return;
                }
                switch (name.Value)
                {
                    case "Name":
                        style.Name = ReadPropertyStringValue(e, "name");
                        break;
                    case "PlayControlPosition":
                        style.PlayControlPosition = ReadPropertyEnumValue<AudioControlPosition>(e.Attribute("value"));
                        break;
                    case "RecordControlPosition":
                        style.RecordControlPosition = ReadPropertyEnumValue<AudioControlPosition>(e.Attribute("value"));
                        break;
                    case "Record":
                        style.Record = ReadPropertyEnumValue<RecordMode>(e.Attribute("value"));
                        break;
                    default:
                        break;
                }
            });
            return style;
        }

        internal static Style ReadStyle(XElement element)
        {
            Style style = new();
            string tagName = element.Name.LocalName;
            ForEachChild(element, "property", e =>
            {
                XAttribute? name = e.Attribute("name");
                if (name is null)
                {
                    return;
                }
                switch (name.Value)
                {
                    case "Align":
                        style.Align = ReadPropertyEnumValue<Alignments>(e.Attribute("value"));
                        break;
                    case "BgColor":
                        style.BgColor = ReadPropertyStringValue(e, "value");
                        break;
                    case "Cell":
                        style.Cell = ReadCellStyle(e, tagName);
                        break;
                    case "Color":
                        style.Color = ReadPropertyStringValue(e, "value");
                        break;
                    case "Columns":
                        style.Columns = ReadPropertyIntValue(e, "value");
                        break;
                    case "Control":
                        style.Control = ReadControlStyle(e, tagName);
                        break;
                    case "Cursor":
                        style.Cursor = ReadPropertyEnumValue<CursorType>(e.Attribute("value"));
                        break;
                    case "ElementAlign":
                        style.ElementAlign = ReadPropertyEnumValue<ElementAlignments>(e.Attribute("value"));
                        break;
                    case "Font":
                        style.Font = ReadFontStyle(e, tagName);
                        break;
                    case "Height":
                        style.Height = ReadPropertyStringValue(e, "value");
                        break;
                    case "Hidden":
                        style.Hidden = ReadPropertyBoolValue(e, "value");
                        break;
                    case "Image":
                        style.Image = ReadPropertyStringValue(e, "value");
                        break;
                    case "ImagePosition":
                        style.ImagePosition = ReadPropertyEnumValue<ImagePosition>(e.Attribute("value"));
                        break;
                    case "Indent":
                        style.Indent = ReadPropertyIntValue(e, "value");
                        break;
                    case "Orientation":
                        style.Orientation = ReadPropertyEnumValue<Orientation>(e.Attribute("value"));
                        break;
                    case "Rows":
                        style.Rows = ReadPropertyIntValue(e, "value");
                        break;
                    case "Width":
                        style.Width = ReadPropertyStringValue(e, "value");
                        break;
                    case "ZIndex":
                        style.ZIndex = ReadPropertyIntValue(e, "value");
                        break;
                    default:
                        break;
                }
            });
            return style;
        }

        internal static Element ReadElement(Element element, XElement xElement)
        {
            element.Name = ReadPropertyStringValue(xElement, "name");
            element.Id = ReadPropertyStringValue(xElement, "id");
            element.ElementType = ReadPropertyEnumValue<ElementType>(xElement.Attribute("type"));
            FirstChild(xElement, "labels", ele => element.Labels = ReadLabels(new Labels(element, element.Document, ReadPropertyStringValue(ele, "context")), ele));
            FirstChild(xElement, "properties", ele => element.Properties = ReadProperties(new Properties(element), ele));
            FirstChild(xElement, "templates", ele => element.Templates = ReadProperties(new Properties(element), ele));
            FirstChild(xElement, "labelstyles", ele => element.LabelStyles = ReadStyle(ele));
            FirstChild(xElement, "styles", ele => element.Style = ReadStyle(ele));
            return element;
        }

        internal static Elements ReadElements(Elements elements, XElement element)
        {
            ForEachChild(element, "element", e => elements.Add(ReadElement(elements.NewObject(), e)));
            return elements;
        }

        internal static void JoinCategories(Categories categories, XElement element)
        {
            XAttribute? refAttr = element.Attribute("categoriesref");
            Type? type;
            if (refAttr == null || (type = categories.Document.Types.GetById(refAttr.Value)) == null)
            {
                return;
            }
            foreach (Element cat in type.Categories)
            {
                categories.Add(cat);
            }
        }

        internal static Categories ReadCategories(Categories categories, XElement element)
        {
            ForEachChild(element, "category", e => categories.Add(ReadElement(categories.NewObject(), e)));
            ForEachChild(element, "categories", e => JoinCategories(categories, e));
            return categories;
        }

        internal static Type ReadType(Type type, XElement element)
        {
            type.Id = ReadPropertyStringValue(element, "id");
            type.Name = ReadPropertyStringValue(element, "name");
            type.GlobalNamespace = ReadPropertyBoolValue(element, "global-name-space");
            ReadCategories(type.Categories, element);
            FirstChild(element, "labels", e => type.Labels = ReadLabels(new Labels(type, type.Document, ReadPropertyStringValue(e, "context")), e));
            return type;
        }

        internal static Types ReadTypes(Types types, XElement element)
        {
            ForEachChild(element, "categories", e => types.Add(ReadType(types.NewObject(), e)));
            return types;
        }

        internal static T ReadNameObject<T>(T namedObject, XElement element) where T : MDMNamedObject
        {
            namedObject.Name = ReadPropertyStringValue(element, "name");
            namedObject.Id = ReadPropertyStringValue(element, "id");
            FirstChild(element, "properties", e => namedObject.Properties = ReadProperties(new Properties(namedObject), element));
            FirstChild(element, "templates", e => namedObject.Templates = ReadProperties(new Properties(namedObject), e));
            return namedObject;
        }

        internal static T ReadLabeledObject<T>(T labeledObject, XElement element) where T : MDMLabeledObject
        {
            ReadNameObject(labeledObject, element);
            FirstChild(element, "labels", e => labeledObject.Labels = ReadLabels(new Labels(labeledObject, labeledObject.Document, ReadPropertyStringValue(e, "context")), e));
            FirstChild(element, "styles", e => labeledObject.Style = ReadStyle(e));
            FirstChild(element, "labelstyles", e => labeledObject.LabelStyles = ReadStyle(e));
            return labeledObject;
        }

        internal static Variable ReadVariable(Variable variable, XElement element)
        {
            ReadLabeledObject(variable, element);
            variable.DataType = ReadPropertyEnumValue<MDMDataType>(element.Attribute("type"));
            variable.UsageType = ReadPropertyEnumValue<VariableUsage>(element.Attribute("usagetype"));
            variable.MinValue = ReadPropertyStringValue(element, "min");
            variable.MaxValue = ReadPropertyStringValue(element, "max");
            variable.EffectiveMinValue = ReadPropertyStringValue(element, "effectivemin");
            variable.EffectiveMaxValue = ReadPropertyStringValue(element, "effectivemax");
            variable.HasCaseData = ReadPropertyBoolValue(element, "hascasedata");
            variable.Versioned = ReadPropertyBoolValue(element, "versioned");    
            FirstChild(element, "categories", e => {
                variable.Categories = ReadCategories(new Categories(variable.Document, variable), e);
                variable.Elements = ReadElements(new Elements(variable), e);
            });
            FirstChild(element, "helperfields", e =>
            {
                ForEachChild(e, "variable", v =>
                {
                    if (variable.HelperFields is null)
                    {
                        variable.HelperFields = new Variables(variable.Document, variable);
                    }
                    Variable? exist = variable.Document.Variables.GetById(ReadPropertyStringValue(v, "ref"));
                    if (exist != null)
                    {
                        variable.HelperFields.Add(exist);
                    }
                });
            });
            return variable;
        }

        internal static Variables ReadGloablVariables(Variables variables, XElement element)
        {
            ForEachChild(element, "class", e => variables.Add(ReadVariable(variables.NewObject(), e)));
            ForEachChild(element, "variable", e => variables.Add(ReadVariable(variables.NewObject(), e)));
            ForEachChild(element, "othervariable", e => variables.Add(ReadVariable(variables.NewObject(), e)));
            ForEachChild(element, "multiplier-variable", e => variables.Add(ReadVariable(variables.NewObject(), e)));
            return variables;
        }

        internal static Pages ReadGlobalPages(Pages pages, XElement element)
        {
            ForEachChild(element, "page", e => pages.Add(ReadPage(pages.NewObject(), e)));
            return pages;
        }

        internal static Fields ReadClassFields(Fields fields, XElement element)
        {
            fields.Name = ReadPropertyStringValue(element, "name");
            fields.GlobalNamespace = ReadPropertyBoolValue(element, "global-name-space");
            ForEachChild(element, "variable", e => fields.Add(ReadField(fields.NewObject(), e)));
            ForEachChild(element, "loop", e => fields.Add(ReadField(fields.NewObject(), e)));
            ForEachChild(element, "grid", e => fields.Add(ReadField(fields.NewObject(), e)));
            return fields;
        }

        internal static Types ReadClassTypes(Types types, XElement element)
        {
            types.GlobalNamespace = ReadPropertyBoolValue(element, "global-name-space");
            ForEachChild(element, "categories", e =>
            {
                Type? reference = types.Document.Types.GetById(ReadPropertyStringValue(e, "ref"));
                if (reference != null)
                {
                    types.Add(reference);
                }
            });
            return types;
        }

        internal static Pages ReadClassPages(Pages pages, XElement element)
        {
            pages.GlobalNamespace = ReadPropertyBoolValue(element, "global-name-space");
            ForEachChild(element, "page", e =>
            {
                Page? reference = pages.Document.Pages.GetById(ReadPropertyStringValue(e, "ref"));
                if (reference != null)
                {
                    pages.Add(reference);
                }
            });
            return pages;
        }

        internal static Class ReadFieldClass(Class @class, XElement element)
        {
            ReadLabeledObject(@class, element);
            FirstChild(element, "types", e => @class.Types = ReadClassTypes(new Types(@class.Document, @class), e));
            FirstChild(element, "fields", e => @class.Fields = ReadClassFields(new Fields(@class.Document, @class), e));
            FirstChild(element, "pages", e => @class.Pages = ReadClassPages(new Pages(@class), e));
            return @class;
        }

        internal static Field ReadField(Field field, XElement element)
        {
            ReadLabeledObject(field, element);
            XAttribute? refAttr = element.Attribute("ref");
            if (refAttr != null)
            {
                field.Reference = field.Document.Variables.GetById(refAttr.Value);
                return field;
            }
            field.IteratorType = ReadPropertyEnumValue<IteratorType>(element.Attribute("iteratortype"));
            FirstChild(element, "categories", e => field.Categories = ReadCategories(new Categories(field.Document, field), e));
            FirstChild(element, "class", e => field.Class = ReadFieldClass(new Class(field), e));
            FirstChild(element, "ranges", e =>
            {
                FirstChild(e, "range", r =>
                {
                    field.UpperBound = ReadPropertyIntValue(r, "upperbound");
                    field.LowerBound = ReadPropertyIntValue(r, "lowerbound");
                });
            });
            return field;
        }

        internal static DataSource ReadDataSource(DataSource dataSource, XElement element)
        {
            dataSource.Name = ReadPropertyStringValue(element, "name");
            dataSource.DBLocation = ReadPropertyStringValue(element, "dblocation");
            dataSource.CDSCName = ReadPropertyStringValue(element, "cdscname");
            dataSource.Project = ReadPropertyStringValue(element, "project");
            dataSource.Id = ReadPropertyStringValue(element, "id");
            return dataSource;
        }

        internal static DataSources ReadDataSources(DataSources dataSources, XElement element)
        {
            dataSources.Default = ReadPropertyStringValue(element, "default");
            ForEachChild(element, "connection", e => dataSources.Add(ReadDataSource(dataSources.NewObject(), e)));
            return dataSources;
        }

        internal static Fields ReadSystemFields(Fields fields, XElement element)
        {
            ForEachChild(element, "class", e =>
            {
                var field = fields.NewObject();
                field.Class = ReadFieldClass(new Class(field), e);
                ReadField(field, e);
                field.IsSystem = true;
                fields.Add(field);
            });
            return fields;
        }

    }
}

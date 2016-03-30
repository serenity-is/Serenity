//                cw.InBrace(delegate
//                {
//                    cw.Indented("public ");
//                    sb.Append(DoGetTypeName(type));
//                    sb.AppendLine("(string idPrefix) : base(idPrefix) {}");
//                    sb.AppendLine();

//                    foreach (var item in Serenity.PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type))
//                    {
//                        var editorType = item.EditorType ?? "String";
//                        string widgetTypeName = null;
//                        foreach (var rootNamespace in RootNamespaces)
//                        {
//                            string wn = rootNamespace + "." + editorType;
//                            if (WidgetTypes.Contains(wn))
//                            {
//                                widgetTypeName = wn;
//                                break;
//                            }

//                            wn += "Editor";
//                            if (WidgetTypes.Contains(wn))
//                            {
//                                widgetTypeName = wn;
//                                break;
//                            }
//                        }

//                        if (widgetTypeName == null)
//                        {
//                            var wn = editorType;
//                            if (!WidgetTypes.Contains(editorType))
//                                wn = editorType + "Editor";

//                            if (WidgetTypes.Contains(wn))
//                                widgetTypeName = wn;
//                        }

//                        if (widgetTypeName == null)
//                            continue;

//                        if (widgetTypeName.StartsWith(ns + "."))
//                            widgetTypeName = widgetTypeName.Substring(ns.Length + 1);
//                        else
//                        {
//                            foreach (var rn in RootNamespaces)
//                            {
//                                if (widgetTypeName.StartsWith(rn + "."))
//                                    widgetTypeName = widgetTypeName.Substring(rn.Length + 1);
//                            }
//                        }

//                        cw.Indented("public ");
//                        sb.Append(widgetTypeName);
//                        sb.Append(" ");
//                        sb.Append(item.Name);
//                        sb.Append(" { get { return ById<");
//                        sb.Append(widgetTypeName);
//                        sb.Append(">(\"");
//                        sb.Append(item.Name);
//                        sb.AppendLine("\"); } }");
//                    }
//                });

//                codeByType[type] = sb.ToString();
//                sb.Clear();
//            }

//            var ordered = codeByType.Keys.OrderBy(x => DoGetNamespace(x)).ThenBy(x => x.Name);
//            var byNameSpace = ordered.ToLookup(x => DoGetNamespace(x));

//            var result = new SortedDictionary<string, string>();

//            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
//            {
//                foreach (var type in ns)
//                {
//                    sb.Clear();
//                    sb.AppendLine();
//                    cw.Indented("namespace ");
//                    sb.AppendLine(ns.Key);
//                    cw.InBrace(delegate
//                    {
//                        foreach (var usingNamespace in UsingNamespaces.ToArray().OrderBy(x => x))
//                        {
//                            cw.Indented("using ");
//                            sb.Append(usingNamespace);
//                            sb.AppendLine(";");
//                        }

//                        sb.AppendLine();

//                        int i = 0;

//                        {
//                            if (i++ > 0)
//                                sb.AppendLine();

//                            cw.IndentedMultiLine(codeByType[type].TrimEnd());
//                        }
//                    });

//                    var filename = ns.Key + "." + DoGetTypeName(type) + ".cs";

//                    foreach (var rn in RootNamespaces)
//                    {
//                        if (filename.StartsWith(rn + "."))
//                            filename = filename.Substring(rn.Length + 1);
//                    }

//                    result.Add(filename, sb.ToString());
//                }
//            }

//            return result;
//        }
//    }
//}
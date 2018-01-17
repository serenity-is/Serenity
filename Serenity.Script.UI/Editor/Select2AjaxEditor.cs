using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true), Element("<input type=\"hidden\"/>")]
    [IncludeGenericArguments(false), ScriptName("Select2AjaxEditor")]
    public abstract class Select2AjaxEditor<TOptions, TItem> : Widget<TOptions>, IStringValue
        where TOptions : class, new()
        where TItem: class
    {
        protected int pageSize = 50;

        static Select2AjaxEditor()
        {
            Q.Prop(typeof(Select2AjaxEditor<object, object>), "value");
        }

        public Select2AjaxEditor(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                hidden.Attribute("placeholder", emptyItemText);

            hidden.Select2(GetSelect2Options());

            hidden.Attribute("type", "text"); // jquery validate to work
            
            hidden.Bind2("change." + this.uniqueName, (e, x) =>
            {
                if (e.HasOriginalEvent() || Q.IsFalse(x))
                {
                    if (hidden.GetValidator() != null)
                        hidden.Valid();
                }
            });
        }

        protected virtual string EmptyItemText()
        {
            return element.GetAttribute("placeholder") ?? Q.Text("Controls.SelectEditor.EmptyItemText");
        }

        protected virtual string GetService()
        {
            throw new NotImplementedException();
        }

        protected virtual void Query(ListRequest request, Action<ListResponse<TItem>> callback)
        {
            var options = new ServiceCallOptions<ListResponse<TItem>>
            {
                BlockUI = false,
                Service = GetService() + "/List",
                Request = request,
                OnSuccess = response =>
                {
                    callback(response);
                }
            };

            ExecuteQuery(options);
        }

        protected virtual void ExecuteQuery(ServiceCallOptions<ListResponse<TItem>> options)
        {
            Q.ServiceCall(options);
        }

        protected virtual void QueryByKey(string key, Action<TItem> callback)
        {
            var options = new ServiceCallOptions<RetrieveResponse<TItem>>
            {
                BlockUI = false,
                Service = GetService() + "/Retrieve",
                Request = new RetrieveRequest
                {
                    EntityId = key.As<Int64>()
                },
                OnSuccess = response =>
                {
                    callback(response.Entity);
                }
            };

            ExecuteQueryByKey(options);
        }

        protected virtual void ExecuteQueryByKey(ServiceCallOptions<RetrieveResponse<TItem>> options)
        {
            Q.ServiceCall(options);
        }

        protected abstract string GetItemKey(TItem item);
        protected abstract string GetItemText(TItem item);

        protected virtual int GetTypeDelay()
        {
            return 500;
        }

        protected virtual Select2Options GetSelect2Options()
        {
            var emptyItemText = EmptyItemText();
            int queryTimeout = 0;

            return new Select2Options
            {
                MinimumResultsForSearch = 10,
                PlaceHolder = !emptyItemText.IsEmptyOrNull() ? emptyItemText : null,
                AllowClear = emptyItemText != null,
                Query = delegate(Select2QueryOptions query)
                {
                    var request = new ListRequest
                    {
                        ContainsText = query.Term.TrimToNull(),
                        Skip = (query.Page - 1) * pageSize,
                        Take = pageSize + 1
                    };

                    if (queryTimeout != 0)
                        Window.ClearTimeout(queryTimeout);

                    queryTimeout = Window.SetTimeout(delegate
                    {
                        Query(request, response =>
                        {
                            query.Callback(new Select2Result
                            {
                                Results = response.Entities.Slice(0, pageSize).As<List<TItem>>().Map(x => new Select2Item
                                {
                                    Id = GetItemKey(x),
                                    Text = GetItemText(x),
                                    Source = x
                                }),
                                More = response.Entities.Count >= pageSize
                            });
                        });
                    }, GetTypeDelay());
                },
                InitSelection = delegate(jQueryObject element, Action<object> callback)
                {
                    var val = element.GetValue();
                    if (val.IsEmptyOrNull())
                    {
                        callback(null);
                        return;
                    }

                    QueryByKey(val, result =>
                    {
                        callback(result == null ? null : new Select2Item
                        {
                            Id = GetItemKey(result),
                            Text = GetItemText(result),
                            Source = result
                        });
                    });
                }
            };
        }

        protected void AddInplaceCreate(string title)
        {
            var self = this;

            J("<a><b/></a>").AddClass("inplace-button inplace-create")
                .Attribute("title", title)
                .InsertAfter(this.element)
                .Click(e =>
                {
                    self.InplaceCreateClick(e);
                });

            this.Select2Container.Add(this.element).AddClass("has-inplace-button");
        }

        protected virtual void InplaceCreateClick(jQueryEvent e)
        {
        }

        protected jQueryObject Select2Container
        {
            get { return this.element.PrevAll(".select2-container"); }
        }

        public string Value
        {
            get
            {
                return this.element.Select2Get("val") as string;
            }
            set
            {
                if (value != Value)
                    this.element.Select2("val", value).TriggerHandler("change", new object[] { true });
            }
        }
    }
}
using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class LazyLoadHelper
    {
        private static int autoIncrement = 0;

        public static void ExecuteOnceWhenShown(jQueryObject element, Action callback)
        {
            ++autoIncrement;
            string eventClass = "ExecuteOnceWhenShown" + autoIncrement;

            bool executed = false;

            if (element.Is(":visible"))
                callback();
            else
            {
                jQueryObject uiTabs = element.Closest(".ui-tabs");
                if (uiTabs.Length > 0)
                {
                    uiTabs.Bind("tabsshow." + eventClass, delegate(jQueryEvent e)
                    {
                        if (element.Is(":visible"))
                        {
                            uiTabs.Unbind("tabsshow." + eventClass);

                            if (!executed)
                            {
                                executed = true;
                                element.Unbind("shown." + eventClass);
                                callback();
                            }
                        }
                    });
                }

                jQueryObject dialog;
                if (element.HasClass("ui-dialog"))
                    dialog = element.Children(".ui-dialog-content");
                else
                    dialog = element.Closest(".ui-dialog-content");

                if (dialog.Length > 0)
                    dialog.Bind("dialogopen." + eventClass, delegate
                    {
                        dialog.Unbind("dialogopen." + eventClass);

                        if (element.Is(":visible") && !executed)
                        {
                            executed = true;
                            element.Unbind("shown." + eventClass);
                            callback();
                        }
                    });

                element.Bind("shown." + eventClass, delegate
                {
                    if (element.Is(":visible"))
                    {
                        element.Unbind("shown." + eventClass);

                        if (!executed)
                        {
                            executed = true;
                            callback();
                        }
                    }
                });
            }
        }

        public static void ExecuteEverytimeWhenShown(jQueryObject element, Action callback,
            bool callNowIfVisible)
        {
            string eventClass = "ExecuteEverytimeWhenShown" + (++autoIncrement);

            bool wasVisible = element.Is(":visible");

            if (wasVisible && callNowIfVisible)
                callback();

            jQueryEventHandler check = delegate(jQueryEvent e)
            {
                if (element.Is(":visible"))
                {
                    if (!wasVisible)
                    {
                        wasVisible = true;
                        callback();
                    }
                }
                else
                    wasVisible = false;
            };

            var uiTabs = element.Closest(".ui-tabs");
            if (uiTabs.Length > 0)
                uiTabs.Bind("tabsactivate." + eventClass, check);

            var dialog = element.Closest(".ui-dialog-content");
            if (dialog.Length > 0)
                dialog.Bind("dialogopen." + eventClass, check);

            element.Bind("shown." + eventClass, check);
        }
    }
}
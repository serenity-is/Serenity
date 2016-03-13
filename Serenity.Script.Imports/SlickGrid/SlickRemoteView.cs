using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class SlickRemoteViewOptions
    {
        public bool AutoLoad { get; set; }
        public string IdField { get; set; }
        public string ContentType { get; set; }
        public string DataType { get; set; }
        public object Filter { get; set; }
        public dynamic Params { get; set; }
        public SlickEvent OnSubmit { get; set; }
        public string Url { get; set; }
        public string[] SortBy { get; set; }
        public int RowsPerPage { get; set; }
        public int SeekToPage { get; set; }
        public SlickEvent OnProcessData { get; set; }
        public string Method { get; set; }
        public Func<object, int, dynamic> GetItemMetadata { get; set; }
        [ScriptName("errormsg")]
        public string ErrorMsg { get; set; }
    }

    [Imported, Serializable]
    public class SlickSummaryOptions
    {
        public List<SlickAggregator> Aggregators { get; set; }
    }

    [Imported, Serializable]
    public class SlickGroupInfo<TItem>
    {
        public TypeOption<string, Func<TItem, object>> Getter { get; set; }
        public Func<SlickGroup<TItem>, string> Formatter { get; set; }
        public List<SlickAggregator> Aggregators { get; set; }
        public bool AggregateCollapsed { get; set; }
        public bool LazyTotalsCalculation { get; set; }
    }

    [Imported, ScriptNamespace("Slick"), ScriptName("Group"), IncludeGenericArguments(false), Serializable]
    public class SlickGroup<TItem>
    {
        [ScriptName("__group")]
        public bool IsGroup { get { return false; } }
        public int Level { get { return 0; } }
        public int Count { get { return 0; } }
        public object Value { get { return null; } }
        public string Title { get { return null; } }
        public bool Collapsed { get { return false; } }
        public object Totals { get { return null; } }
        public TItem[] Rows { get { return null; } }
        public SlickGroup<TItem> Groups { get { return null; } }
        public string GroupingKey { get { return null; } }
    }

    [Imported, ScriptNamespace("Slick"), ScriptName("GroupTotals"), Serializable]
    public class SlickGroupTotals<TItem>
    {
        [ScriptName("__groupTotals")]
        public bool IsGroupTotals { get { return false; } }
        public SlickGroup<TItem> Group { get { return null; } }
        public bool Initialized { get { return false; } }
        public JsDictionary<string, object> Sum { get { return null; } }
        public JsDictionary<string, object> Avg { get { return null; } }
        public JsDictionary<string, object> Min { get { return null; } }
        public JsDictionary<string, object> Max { get { return null; } }
    }

    public delegate bool CancellableViewCallback<TEntity>(SlickRemoteView<TEntity> view);
    public delegate bool SlickRemoteViewAjaxCallback<TEntity>(SlickRemoteView<TEntity> view, jQueryAjaxOptions options);
    public delegate ListResponse<TEntity> SlickRemoteViewProcessCallback<TEntity>(ListResponse<TEntity> data, SlickRemoteView<TEntity> view);
    public delegate bool SlickRemoteViewFilter<TEntity>(TEntity item, SlickRemoteView<TEntity> view);

    [Imported, ScriptNamespace("Slick.Data"), ScriptName("RemoteView")]
    public class SlickRemoteView
    {
        public SlickRemoteView(SlickRemoteViewOptions options)
        {
        }

        public void AddData(List<dynamic> data)
        {
        }

        public List<dynamic> GetItems()
        {
            return null;
        }

        public void BeginUpdate()
        {
        }

        public void EndUpdate()
        {
        }

        public void DeleteItem(object id)
        {
        }

        public void SetFilter(SlickRemoteViewFilter<dynamic> filter)
        {
        }

        public void SetItems(List<dynamic> items, bool fullReset)
        {
        }

        public object GetItemById(object id)
        {
            return null;
        }

        public void UpdateItem(object id, object item)
        {
        }

        public void AddItem(object item)
        {
        }

        public void Populate()
        {
        }

        public void PopulateLock()
        {
        }

        public void PopulateUnlock()
        {
        }

        public int? GetIdxById(object id)
        {
            return null;
        }

        public dynamic GetItemByIdx(int index)
        {
            return null;
        }

        public void SetGrouping(List<SlickGroupInfo<dynamic>> groupInfo)
        {
        }

        public List<SlickGroupInfo<dynamic>> GetGrouping()
        {
            return null;
        }

        [IntrinsicProperty]
        public dynamic Params { get { return null; } }

        [IntrinsicProperty]
        public List<dynamic> Rows { get { return null; } }

        [ScriptName("rowsPerPage")]
        public Int32? RowsPerPage;

        [ScriptName("idField")]
        public string IdField;

        [IntrinsicProperty]
        public string[] SortBy { get; set; }

        public CancellableViewCallback<dynamic> OnSubmit;
        public SlickRemoteViewAjaxCallback<dynamic> OnAjaxCall;
        public SlickRemoteViewProcessCallback<dynamic> OnProcessData;
        public SlickEvent OnRowCountChanged;
        public SlickEvent OnRowsChanged;
        public SlickEvent OnDataChanged;
        public SlickEvent OnDataLoaded;

        [ScriptName("seekToPage")]
        public Int32? SeekToPage;
    }

    [Imported, ScriptNamespace("Slick.Data"), ScriptName("RemoteView"), IncludeGenericArguments(false)]
    public class SlickRemoteView<TEntity> : SlickRemoteView
    {
        public SlickRemoteView(SlickRemoteViewOptions options)
            : base(options)
        {
        }

        public void AddData(ListResponse<TEntity> data)
        {
        }

        public new List<TEntity> GetItems()
        {
            return null;
        }

        public void SetFilter(SlickRemoteViewFilter<TEntity> filter)
        {
        }

        public void SetItems(List<TEntity> items, bool fullReset)
        {
        }

        public new TEntity GetItemById(object id)
        {
            return default(TEntity);
        }

        public void UpdateItem(object id, TEntity item)
        {
        }

        public void AddItem(TEntity item)
        {
        }

        public int? GetIdxById(TEntity id)
        {
            return null;
        }

        public new TEntity GetItemByIdx(int index)
        {
            return default(TEntity);
        }

        public void SetGrouping(List<SlickGroupInfo<TEntity>> groupInfo)
        {
        }

        public void SetSummaryOptions(SlickSummaryOptions options)
        {
        }

        public void Refresh()
        {
        }

        [IntrinsicProperty]
        public new List<TEntity> Rows { get { return null; } }

        public new CancellableViewCallback<TEntity> OnSubmit;
        public new SlickRemoteViewAjaxCallback<TEntity> OnAjaxCall;
        public new SlickRemoteViewProcessCallback<TEntity> OnProcessData;
    }

    [Imported, ScriptNamespace("Slick.Data.Aggregators"), ScriptName("Aggregator")]
    public abstract class SlickAggregator
    {
    }

    [Imported, ScriptNamespace("Slick.Data.Aggregators"), ScriptName("Avg")]
    public class SlickAvg : SlickAggregator
    {
        public SlickAvg(string field)
        {
        }
    }

    [Imported, ScriptNamespace("Slick.Data.Aggregators"), ScriptName("Sum")]
    public class SlickSum : SlickAggregator
    {
        public SlickSum(string field)
        {
        }
    }

    [Imported, ScriptNamespace("Slick.Data.Aggregators"), ScriptName("Min")]
    public class SlickMin : SlickAggregator
    {
        public SlickMin(string field)
        {
        }
    }

    [Imported, ScriptNamespace("Slick.Data.Aggregators"), ScriptName("Max")]
    public class SlickMax : SlickAggregator
    {
        public SlickMax(string field)
        {
        }
    }

}
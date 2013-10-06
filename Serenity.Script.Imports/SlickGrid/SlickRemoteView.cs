using jQueryApi;
using System;
using System.Collections;
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
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public int RowsPerPage { get; set; }
        public int SeekToPage { get; set; }
        public SlickEvent OnProcessData { get; set; }
        public string Method { get; set; }
        [ScriptName("errormsg")]
        public string ErrorMsg { get; set; }
    }


    public delegate bool CancellableViewCallback<TEntity>(SlickRemoteView<TEntity> view);
    public delegate bool SlickRemoteViewAjaxCallback<TEntity>(SlickRemoteView<TEntity> view, jQueryAjaxOptions options);
    public delegate ListResponse<TEntity> SlickRemoteViewProcessCallback<TEntity>(ListResponse<TEntity> data, SlickRemoteView<TEntity> view);
    public delegate bool SlickRemoteViewFilter<TEntity>(TEntity item, SlickRemoteView<TEntity> view);

    [Imported, ScriptNamespace("Slick.Data"), ScriptName("RemoteView"), IncludeGenericArguments(false)]
    public class SlickRemoteView<TEntity>
    {
        public SlickRemoteView(SlickRemoteViewOptions options)
        {
        }

        public void AddData(ListResponse<TEntity> data)
        {
        }

        public List<TEntity> GetItems()
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

        public void SetFilter(SlickRemoteViewFilter<TEntity> filter)
        {
        }

        public void SetItems(List<TEntity> items, bool fullReset)
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

        public TEntity GetItemByIdx(int index)
        {
            return default(TEntity);
        }

        [IntrinsicProperty]
        public dynamic Params { get { return null; } }

        [IntrinsicProperty]
        public List<TEntity> Rows { get { return null; } }

        [ScriptName("rowsPerPage")]
        public Int32? RowsPerPage;

        [IntrinsicProperty]
        public string SortOrder { get; set; }

        [IntrinsicProperty]
        public string SortBy { get; set; }

        public CancellableViewCallback<TEntity> OnSubmit;
        public SlickRemoteViewAjaxCallback<TEntity> OnAjaxCall;
        public SlickRemoteViewProcessCallback<TEntity> OnProcessData;
        public RemoteViewEvent OnRowCountChanged;
        public RemoteViewEvent OnRowsChanged;

        [ScriptName("seekToPage")]
        public Int32? SeekToPage;
    }

    [Imported, IncludeGenericArguments(false)]
    public class RemoteViewEvent
    {
        public void Subscribe(Action<jQueryEvent, dynamic> handler)
        {
        }

        public void Unsubscribe(Action<jQueryEvent, dynamic> handler)
        {
        }

        public void Clear()
        {
        }
    }

}

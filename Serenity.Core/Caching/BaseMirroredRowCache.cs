using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Data
{
    public abstract class BaseMirroredRowCache<TRow>
        where TRow : Row, new()
    {
        public class Params
        {
            public Params()
            {
                Expiration = TimeSpan.FromMinutes(5);
            }

            public TimeSpan Expiration;
        }

        private List<TRow> _inNaturalOrder;
        private Dictionary<Int32, TRow> _byId;       
        private Dictionary<string, TRow> _byCode;
        private TimeSpan _expiration;

        private IIdField _idField;
        private StringField _codeField;

        protected BaseMirroredRowCache()
            : this(new Params())
        {
        }

        protected List<TRow> GetInNaturalOrder()
        {
            if (_inNaturalOrder == null)
                Load();

            return _inNaturalOrder;
        }

        protected BaseMirroredRowCache(Params prm)
        {
            _expiration = prm.Expiration;

            var row = new TRow();

            _idField = ((IIdRow)row).IdField;
            _byId = new Dictionary<int, TRow>();

            if (row is ICodeRow)
            {
                _codeField = ((ICodeRow)row).CodeField;
                _byCode = new Dictionary<string, TRow>(StringComparer.CurrentCultureIgnoreCase);
            }
        }

        protected void EnsureLoad()
        {
            if (_inNaturalOrder == null)
                Load();
        }

        protected abstract List<TRow> InternalLoad();

        protected virtual void Load()
        {
            var newList = InternalLoad();
            var newById = new Dictionary<int, TRow>();
            Dictionary<string, TRow> newByCode = null;
            if (_codeField != null)
                newByCode = new Dictionary<string, TRow>();

            foreach (var row in newList)
            {
                newById[(int)_idField[row]] = row;
                if (_codeField != null)
                    newByCode[_codeField[row]] = row;
            }

            _inNaturalOrder = newList;
            _byId = newById;
            _byCode = newByCode;
        }

        public void RemoveAll()
        {
            _inNaturalOrder = null;
            _byId = null;
            _byCode = null;
        }

        protected TRow GetById(int id)
        {
            if (id == Int32.MinValue)
                return null;

            if (_inNaturalOrder == null)
                Load();

            TRow row;
            if (!_byId.TryGetValue(id, out row))
                return null;

            return row;
        }

        protected TRow GetByCode(string code)
        {
            if (code.IsEmptyOrNull())
                return null;

            if (_inNaturalOrder == null)
                Load();

            if (_byCode == null)
                throw new InvalidOperationException("byCode");

            TRow row;
            if (!_byCode.TryGetValue(code, out row))
                return null;

            return row;
        }
    }
}
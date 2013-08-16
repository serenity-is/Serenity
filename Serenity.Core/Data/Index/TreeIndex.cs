using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    ///   An index that holds an ordered list of objects in AVP tree (balanced)
    ///   structure. Objects should implement IComparer interface or a helper class that
    ///   supports IComparer interface should be passed in constructor.</summary>
    /// <typeparam name="T">
    ///   Type of the objects that index will contain.</typeparam>
    /// <remarks>
    ///   It is assumed that objects that are added to index are immutable (thus won't change after adding).
    ///   A change in objects might cause index state to be invalid.
    ///   This might result in false search results or enumeration, so once an object is added to the index,
    ///   it shouldn't be changed, or it should be removed first, changed and added again.</remarks>
    public class TreeIndex<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>number of nodes in the index tree (also the record count).</summary>
        private int _count;
        /// <summary>root tree node</summary>
        private Node _root;
        /// <summary>next ID value for the index node that will be created. increases automatically.</summary>
        internal int _lastNodeID;
        /// <summary>interface that will be used to compare elements.</summary>
        private IComparer<T> _comparer;

        /// <summary>
        ///   Creates a new TreeIndex that uses the default comparer.</summary>
        public TreeIndex()
        {
            _count = 0;
            _comparer = Comparer<T>.Default;
        }

        /// <summary>
        ///   Creates a new TreeIndex that uses the specified comparer.</summary>
        /// <param name="comparer">
        ///   Comparer to be used (if null, default comparer is used).</param>
        public TreeIndex(IComparer<T> comparer)
            : this()
        {
            if (comparer != null)
                _comparer = comparer;
        }

        /// <summary>
        ///   Adds element to the index in suitable position.</summary>
        /// <param name="item">
        ///   Element to add to the index. It should stay unchanged as long as in the index.</param>
        public void Add(T item)
        {
            Node n = new Node(item, _lastNodeID++);
            _count++;

            if (_root == null)
            {
                _root = n;
                return;
            }

            Node p = _root;
            for (; ; )
            {
                int comparer = _comparer.Compare(n._item, p._item);
                if (comparer == 0)
                    comparer = n._id - p._id;

                if (comparer < 0)
                {
                    if (p._left != null)
                    {
                        p = p._left;
                    }
                    else
                    {
                        n._parent = p;
                        p._left = n;
                        p._balance--;

                        break;
                    }
                }
                else
                {
                    if (p._right != null)
                    {
                        p = p._right;
                    }
                    else
                    {
                        n._parent = p;
                        p._right = n;
                        p._balance++;

                        break;
                    }
                }
            }

            while ((p._balance != 0) && (p._parent != null))
            {
                if (p._parent._left == p)
                {
                    p._parent._balance--;
                }
                else
                {
                    p._parent._balance++;
                }

                p = p._parent;

                if (p._balance == -2)
                {
                    Node x = p._left;

                    if (x._balance == -1)
                    {
                        x._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = x;
                        }
                        else
                        {
                            if (p._parent._left == p)
                            {
                                p._parent._left = x;
                            }
                            else
                            {
                                p._parent._right = x;
                            }
                        }

                        p._left = x._right;

                        if (p._left != null)
                        {
                            p._left._parent = p;
                        }

                        x._right = p;
                        p._parent = x;

                        x._balance = 0;
                        p._balance = 0;
                    }
                    else
                    {
                        Node w = x._right;

                        w._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = w;
                        }
                        else
                        {
                            if (p._parent._left == p)
                            {
                                p._parent._left = w;
                            }
                            else
                            {
                                p._parent._right = w;
                            }
                        }

                        x._right = w._left;

                        if (x._right != null)
                        {
                            x._right._parent = x;
                        }

                        p._left = w._right;

                        if (p._left != null)
                        {
                            p._left._parent = p;
                        }

                        w._left = x;
                        w._right = p;

                        x._parent = w;
                        p._parent = w;

                        if (w._balance == -1)
                        {
                            x._balance = 0;
                            p._balance = 1;
                        }
                        else if (w._balance == 0)
                        {
                            x._balance = 0;
                            p._balance = 0;
                        }
                        else // w.Balance == 1
                        {
                            x._balance = -1;
                            p._balance = 0;
                        }

                        w._balance = 0;
                    }

                    break;
                }
                else if (p._balance == 2)
                {
                    Node x = p._right;

                    if (x._balance == 1)
                    {
                        x._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = x;
                        }
                        else
                        {
                            if (p._parent._left == p)
                            {
                                p._parent._left = x;
                            }
                            else
                            {
                                p._parent._right = x;
                            }
                        }

                        p._right = x._left;

                        if (p._right != null)
                        {
                            p._right._parent = p;
                        }

                        x._left = p;
                        p._parent = x;

                        x._balance = 0;
                        p._balance = 0;
                    }
                    else
                    {
                        Node w = x._left;

                        w._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = w;
                        }
                        else
                        {
                            if (p._parent._left == p)
                            {
                                p._parent._left = w;
                            }
                            else
                            {
                                p._parent._right = w;
                            }
                        }

                        x._left = w._right;

                        if (x._left != null)
                        {
                            x._left._parent = x;
                        }

                        p._right = w._left;

                        if (p._right != null)
                        {
                            p._right._parent = p;
                        }

                        w._right = x;
                        w._left = p;

                        x._parent = w;
                        p._parent = w;

                        if (w._balance == 1)
                        {
                            x._balance = 0;
                            p._balance = -1;
                        }
                        else if (w._balance == 0)
                        {
                            x._balance = 0;
                            p._balance = 0;
                        }
                        else // w._balance == -1
                        {
                            x._balance = 1;
                            p._balance = 0;
                        }

                        w._balance = 0;
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///   Copies elements in the index to an array in order.</summary>
        /// <param name="array">
        ///   Target array (required). Must have at least record count length.</param>
        /// <param name="index">
        ///   Index to start copying elements to array. Usually 0.</param>
        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException();

            if ((index < 0) || (index >= array.Length))
                throw new ArgumentOutOfRangeException();

            if ((array.Length - index) < _count)
                throw new ArgumentException();

            if (_root != null)
            {
                Node p = _root;

                while (p._left != null)
                    p = p._left;

                for (; ; )
                {
                    array[index] = p._item;

                    if (p._right == null)
                    {
                        for (; ; )
                        {
                            if (p._parent == null)
                                return;

                            if (p != p._parent._right)
                                break;

                            p = p._parent;
                        }

                        p = p._parent;
                    }
                    else
                    {
                        for (p = p._right; p._left != null; p = p._left) ;
                    }

                    index++;
                }
            }
        }

        /// <summary>
        ///   Searches for the element in the index and removes it. If found, 
        ///   and removed returns true.</summary>
        /// <param name="item">
        ///   Element to search.</param>
        /// <returns>
        ///   True if element is found and removed from the index.</returns>
        public bool Remove(T item)
        {
            for (Node p = _root; p != null; )
            {
                int Comparer = _comparer.Compare(item, p._item);

                if (Comparer < 0)
                {
                    p = p._left;
                }
                else if (Comparer > 0)
                {
                    p = p._right;
                }
                else
                {
                    Node y; // node from which rebalancing begins

                    if (p._right == null)	// Case 1: p has no right child
                    {
                        if (p._left != null)
                        {
                            p._left._parent = p._parent;
                        }

                        if (p._parent == null)
                        {
                            _root = p._left;

                            goto Done;
                        }

                        if (p == p._parent._left)
                        {
                            p._parent._left = p._left;

                            y = p._parent;
                            // goto LeftDelete;
                        }
                        else
                        {
                            p._parent._right = p._left;

                            y = p._parent;
                            goto RightDelete;
                        }
                    }
                    else if (p._right._left == null)	// Case 2: p's right child has no left child
                    {
                        if (p._left != null)
                        {
                            p._left._parent = p._right;
                            p._right._left = p._left;
                        }

                        p._right._balance = p._balance;
                        p._right._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = p._right;
                        }
                        else
                        {
                            if (p == p._parent._left)
                            {
                                p._parent._left = p._right;
                            }
                            else
                            {
                                p._parent._right = p._right;
                            }
                        }

                        y = p._right;

                        goto RightDelete;
                    }
                    else	// Case 3: p's right child has a left child
                    {
                        Node s = p._right._left;

                        while (s._left != null)
                        {
                            s = s._left;
                        }

                        if (p._left != null)
                        {
                            p._left._parent = s;
                            s._left = p._left;
                        }

                        s._parent._left = s._right;

                        if (s._right != null)
                        {
                            s._right._parent = s._parent;
                        }

                        p._right._parent = s;
                        s._right = p._right;

                        y = s._parent; // for rebalacing, must be set before we change s._parent

                        s._balance = p._balance;
                        s._parent = p._parent;

                        if (p._parent == null)
                        {
                            _root = s;
                        }
                        else
                        {
                            if (p == p._parent._left)
                            {
                                p._parent._left = s;
                            }
                            else
                            {
                                p._parent._right = s;
                            }
                        }

                        // goto LeftDelete;
                    }

                    // rebalancing begins

                    LeftDelete:

                    y._balance++;

                    if (y._balance == 1)
                    {
                        goto Done;
                    }
                    else if (y._balance == 2)
                    {
                        Node x = y._right;

                        if (x._balance == -1)
                        {
                            Node w = x._left;

                            w._parent = y._parent;

                            if (y._parent == null)
                            {
                                _root = w;
                            }
                            else
                            {
                                if (y._parent._left == y)
                                {
                                    y._parent._left = w;
                                }
                                else
                                {
                                    y._parent._right = w;
                                }
                            }

                            x._left = w._right;

                            if (x._left != null)
                            {
                                x._left._parent = x;
                            }

                            y._right = w._left;

                            if (y._right != null)
                            {
                                y._right._parent = y;
                            }

                            w._right = x;
                            w._left = y;

                            x._parent = w;
                            y._parent = w;

                            if (w._balance == 1)
                            {
                                x._balance = 0;
                                y._balance = -1;
                            }
                            else if (w._balance == 0)
                            {
                                x._balance = 0;
                                y._balance = 0;
                            }
                            else // w._balance == -1
                            {
                                x._balance = 1;
                                y._balance = 0;
                            }

                            w._balance = 0;

                            y = w; // for next iteration
                        }
                        else
                        {
                            x._parent = y._parent;

                            if (y._parent != null)
                            {
                                if (y._parent._left == y)
                                {
                                    y._parent._left = x;
                                }
                                else
                                {
                                    y._parent._right = x;
                                }
                            }
                            else
                            {
                                _root = x;
                            }

                            y._right = x._left;

                            if (y._right != null)
                            {
                                y._right._parent = y;
                            }

                            x._left = y;
                            y._parent = x;

                            if (x._balance == 0)
                            {
                                x._balance = -1;
                                y._balance = 1;

                                goto Done;
                            }
                            else
                            {
                                x._balance = 0;
                                y._balance = 0;

                                y = x; // for next iteration
                            }
                        }
                    }

                    goto LoopTest;


                RightDelete:

                    y._balance--;

                    if (y._balance == -1)
                    {
                        goto Done;
                    }
                    else if (y._balance == -2)
                    {
                        Node x = y._left;

                        if (x._balance == 1)
                        {
                            Node w = x._right;

                            w._parent = y._parent;

                            if (y._parent == null)
                            {
                                _root = w;
                            }
                            else
                            {
                                if (y._parent._left == y)
                                {
                                    y._parent._left = w;
                                }
                                else
                                {
                                    y._parent._right = w;
                                }
                            }

                            x._right = w._left;

                            if (x._right != null)
                            {
                                x._right._parent = x;
                            }

                            y._left = w._right;

                            if (y._left != null)
                            {
                                y._left._parent = y;
                            }

                            w._left = x;
                            w._right = y;

                            x._parent = w;
                            y._parent = w;

                            if (w._balance == -1)
                            {
                                x._balance = 0;
                                y._balance = 1;
                            }
                            else if (w._balance == 0)
                            {
                                x._balance = 0;
                                y._balance = 0;
                            }
                            else // w._balance == 1
                            {
                                x._balance = -1;
                                y._balance = 0;
                            }

                            w._balance = 0;

                            y = w; // for next iteration
                        }
                        else
                        {
                            x._parent = y._parent;

                            if (y._parent != null)
                            {
                                if (y._parent._left == y)
                                {
                                    y._parent._left = x;
                                }
                                else
                                {
                                    y._parent._right = x;
                                }
                            }
                            else
                            {
                                _root = x;
                            }

                            y._left = x._right;

                            if (y._left != null)
                            {
                                y._left._parent = y;
                            }

                            x._right = y;
                            y._parent = x;

                            if (x._balance == 0)
                            {
                                x._balance = 1;
                                y._balance = -1;

                                goto Done;
                            }
                            else
                            {
                                x._balance = 0;
                                y._balance = 0;

                                y = x; // for next iteration
                            }
                        }
                    }

                LoopTest:

                    if (y._parent != null)
                    {
                        if (y == y._parent._left)
                        {
                            y = y._parent;
                            goto LeftDelete;
                        }

                        y = y._parent;
                        goto RightDelete;
                    }

                Done:

                    _count--;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Find the first node that comes before a node in order, and has same value.</summary>
        /// <param name="p">
        ///   Node.</param>
        /// <returns>
        ///   Node itself or first node with same field values.</returns>
        internal Node GoPriorWhileEqual(Node p)
        {
            while (p._left != null && _comparer.Compare(p._item, p._left._item) == 0)
                p = p._left;

            while (true)
            {
                Node x = p.GetPrior();
                if (x == null || _comparer.Compare(p._item, x._item) != 0)
                    break;
                p = x;
            }

            return p;
        }

        /// <summary>
        ///   Find the last node that comes after a node in order, and has same value.</summary>
        /// <param name="p">
        ///   Node.</param>
        /// <returns>
        ///   Node itself or last node with same value.</returns>
        internal Node GoNextWhileEqual(Node p)
        {
            while (p._right != null && _comparer.Compare(p._item, p._right._item) == 0)
                p = p._right;

            while (true)
            {
                Node x = p.GetNext();
                if (x == null || _comparer.Compare(p._item, x._item) != 0)
                    break;
                p = x;
            }

            return p;
        }

        /// <summary>
        ///   Gets first node of the index in order.</summary>
        /// <returns>
        ///   First node or null if empty.</returns>
        internal Node GetFirst()
        {
            Node p = _root;
            if (p != null)
                while (p._left != null)
                    p = p._left;
            return p;
        }

        /// <summary>
        ///   Gets last node of the index in order.</summary>
        /// <returns>
        ///   Last node or null if empty.</returns>
        internal Node GetLast()
        {
            Node p = _root;
            if (p != null)
                while (p._right != null)
                    p = p._right;
            return p;
        }

        /// <summary>
        ///   Gets first node that has same value.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   First node with same value, or null if none.</returns>
        internal Node GetFirstEQ(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return GoPriorWhileEqual(p);
            }
            return null;
        }

        /// <summary>
        ///   Gets first node that has same or greater value.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   First node with same or greater value, or null if none.</returns>
        internal Node GetFirstGE(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    if (p._left == null)
                        return p;
                    else
                        p = p._left;
                }
                else if (comparer > 0)
                {
                    if (p._right == null)
                    {
                        while ((p._parent != null) && (p == p._parent._right))
                            p = p._parent;
                        return p._parent;
                    }
                    else
                        p = p._right;

                }
                else
                    return GoPriorWhileEqual(p);
            }
            return null;
        }

        /// <summary>
        ///   Gets first node that has greater value than element.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   First node with greater sorted value, or null if none.</returns>
        internal Node GetFirstGT(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    if (p._left == null)
                        return p;
                    else
                        p = p._left;
                }
                else // (comparer >= 0)
                {
                    if (p._right == null)
                    {
                        while ((p._parent != null) && (p == p._parent._right))
                            p = p._parent;
                        return p._parent;
                    }
                    else
                        p = p._right;

                }
            }
            return null;
        }

        /// <summary>
        ///   Gets last node that has same value with element.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   Last node with same value, or null if none.</returns>
        internal Node GetLastEQ(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return GoNextWhileEqual(p);
            }
            return null;
        }

        /// <summary>
        ///   Gets last node that has same or smaller value with specified element.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   Last node with same or smaller value, or null if none.</returns>
        internal Node GetLastLE(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    if (p._left == null)
                    {
                        while ((p._parent != null) && (p == p._parent._left))
                            p = p._parent;
                        return p._parent;
                    }
                    else
                        p = p._left;
                }
                else if (comparer > 0)
                {
                    if (p._right == null)
                        return p;
                    else
                        p = p._right;
                }
                else
                    return GoNextWhileEqual(p);
            }
            return null;
        }

        /// <summary>
        ///   Gets last node that has smaller value than specified element.</summary>
        /// <param name="item">
        ///   Element.</param>
        /// <returns>
        ///   Last node with smaller value, or null if none.</returns>
        internal Node GetLastLT(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer <= 0)
                {
                    if (p._left == null)
                    {
                        while ((p._parent != null) && (p == p._parent._left))
                            p = p._parent;
                        return p._parent;
                    }
                    else
                        p = p._left;
                }
                else // (comparer > 0)
                {
                    if (p._right == null)
                        return p;
                    else
                        p = p._right;
                }
            }
            return null;
        }

        /// <summary>
        ///   Searches for element in the index.</summary>
        /// <param name="item">
        ///   Element to search</param>
        /// <returns>
        ///   True, if element is found.</returns>
        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        /// <summary>
        ///   Searches for element in the index.</summary>
        /// <param name="item">
        ///   Element to search</param>
        /// <returns>
        ///   The found node, or null if not found.</returns>
        private Node Find(T item)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(item, p._item);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        ///   Clears the index by removing all elements/nodes.</summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        /// <summary>
        ///   Gets an enumerator to traverse elements in ascending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Gets an enumerator to traverse elements in ascending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Gets an enumerator to traverse elements in ascending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public IEnumerator<T> Ascending()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Gets an enumerator to traverse elements in descending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public IEnumerator<T> Descending()
        {
            return new DescendingOrderEnumerator(GetLast());
        }

        /// <summary>
        ///   Partitions index based on a key element, and returns an enumerator for element in one 
        ///   side of partition in order.</summary>
        /// <param name="item">
        ///   Key element.</param>
        /// <param name="op">
        ///   Operator that determines the partition. See <see cref="IndexMatch"/></param>
        /// <returns>
        ///   Enumerator</returns>
        public IEnumerable<T> Match(T item, IndexMatch op)
        {
            return Match(item, op, false);
        }

        /// <summary>
        ///   Partitions index based on a key element, and returns an enumerator for element in one 
        ///   side of partition in order.</summary>
        /// <param name="item">
        ///   Key element.</param>
        /// <param name="op">
        ///   Operator that determines the partition. See <see cref="IndexMatch"/></param>
        /// <param name="descending">
        ///   True to traverse in descending order</param>
        /// <returns>
        ///   Enumerator</returns>
        public IEnumerable<T> Match(T item, IndexMatch op, bool descending)
        {
            Node first;
            Node last = null;

            switch (op)
            {
                case IndexMatch.EqualTo:
                    first = GetFirstEQ(item);
                    last = first == null ? null : GetLastEQ(item);
                    break;
                case IndexMatch.GreaterEqual:
                    first = GetFirstGE(item);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.GreaterThan:
                    first = GetFirstGT(item);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.LessEqual:
                    last = GetLastLE(item);
                    first = last == null ? null : GetFirst();
                    break;
                case IndexMatch.LessThan:
                    last = GetLastLT(item);
                    first = last == null ? null : GetFirst();
                    break;
                default:
                    yield break;
            }

            if (descending)
            {
                while (true)
                {
                    if (last == null)
                        yield break;

                    yield return last._item;

                    if (last == first)
                        last = null;
                    else
                        last = last.GetPrior();
                }
            }
            else
            {
                while (true)
                {
                    if (first == null)
                        yield break;

                    yield return first._item;

                    if (first == last)
                        first = null;
                    else
                        first = first.GetNext();
                }
            }
        }

        /// <summary>
        ///   Gets number of elements in the index.</summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        ///   A node in the index tree</summary>
        internal sealed class Node
        {
            /// <summary>the element node contains</summary>
            public T _item;
            /// <summary>auto incrementing node ID</summary>
            public int _id;
            /// <summary>parent of this node</summary>
            public Node _parent;
            /// <summary>left node of this</summary>
            public Node _left;
            /// <summary>right node of this</summary>
            public Node _right;
            /// <summary>node balance, should be 0, if not, node is unbalanced.</summary>
            public sbyte _balance;

            /// <summary>
            ///   Creates a new node.</summary>
            /// <param name="item">
            ///   Element of the node.</param>
            /// <param name="id">
            ///   Autoincrementing ID assigned to the node.</param>
            public Node(T item, int id)
            {
                _item = item;
                _id = id;
            }

            /// <summary>
            ///   Creates a new node.</summary>
            /// <param name="item">
            ///   Element of the node.</param>
            /// <param name="id">
            ///   Autoincrementing ID assigned to the node.</param>
            /// <param name="parent">
            ///   Parent of the node</param>
            public Node(T item, int id, Node parent)
            {
                _item = item;
                _id = id;
                _parent = parent;
            }

            /// <summary>
            ///   Gets node that comes before this node in order.</summary>
            /// <returns>
            ///   Prior node or null</returns>
            public Node GetPrior()
            {
                Node p = this;

                if (p._left == null)
                {
                    while ((p._parent != null) && (p == p._parent._left))
                        p = p._parent;

                    p = p._parent;
                }
                else
                {
                    for (p = p._left; p._right != null; p = p._right) ;
                }

                return p;
            }

            /// <summary>
            ///   Gets node that comes after this node in order.</summary>
            /// <returns>
            ///   Next node or null</returns>
            public Node GetNext()
            {
                Node p = this;

                if (p._right == null)
                {
                    while ((p._parent != null) && (p == p._parent._right))
                        p = p._parent;

                    p = p._parent;
                }
                else
                {
                    for (p = p._right; p._left != null; p = p._left) ;
                }

                return p;
            }
        }

        /// <summary>
        ///   Enumerator to traverse index in ascending order.</summary>
        public struct AscendingOrderEnumerator : IEnumerator<T>
        {
            /// <summary>Next node to be returned.</summary>
            private Node _next;
            /// <summary>Node to end enumeration at.</summary>
            private Node _last;
            /// <summary>Current node.</summary>
            private T _current;

            /// <summary>
            ///   Gets current node.</summary>
            public T Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   Gets current node.</summary>
            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   Moves enumerator to the next node.</summary>
            public bool MoveNext()
            {
                if (_next == null)
                    return false;

                _current = _next._item;

                if (_next == _last)
                    _next = null;
                else
                    _next = _next.GetNext();

                return true;
            }

            /// <summary>
            ///   Creates a new enumerator starting from node specified.</summary>
            /// <param name="first">
            ///   Node to start enumeration from (can be null).</param>
            internal AscendingOrderEnumerator(Node first)
            {
                _next = first;
                _current = default(T);
                _last = null;
            }

            /// <summary>
            ///   Creates a new enumerator starting from node specified.</summary>
            /// <param name="first">
            ///   Node to start enumeration from (can be null).</param>
            /// <param name="last">
            ///   Node to end enumeration at</param>
            internal AscendingOrderEnumerator(Node first, Node last)
            {
                _next = first;
                _last = last;
                _current = default(T);
            }

            /// <summary>
            ///   Disposes enumerator.</summary>
            public void Dispose()
            {
            }

            /// <summary>
            ///   This enumerator doesn't support resetting.</summary>         
            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   Enumerator to traverse index in descending order.</summary>
        public struct DescendingOrderEnumerator : IEnumerator<T>
        {
            /// <summary>Next node to be returned.</summary>
            private Node _prior;
            /// <summary>Node to end enumeration at.</summary>
            private Node _last;
            /// <summary>Current node.</summary>
            private T _current;

            /// <summary>
            ///   Gets current node.</summary>          
            public T Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   Gets current node.</summary>
            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   Moves enumerator to the next node.</summary>
            public bool MoveNext()
            {
                if (_prior == null)
                    return false;

                _current = _prior._item;

                _prior = _prior.GetPrior();

                return true;
            }

            /// <summary>
            ///   Creates a new enumerator starting from node specified.</summary>
            /// <param name="first">
            ///   Node to start enumeration from (can be null).</param>
            internal DescendingOrderEnumerator(Node first)
            {
                _prior = first;
                _last = null;
                _current = default(T);
            }


            /// <summary>
            ///   Creates a new enumerator starting from node specified.</summary>
            /// <param name="first">
            ///   Node to start enumeration from (can be null).</param>
            /// <param name="last">
            ///   Node to end enumeration at</param>         
            internal DescendingOrderEnumerator(Node first, Node last)
            {
                _prior = first;
                _last = last;
                _current = default(T);
            }

            /// <summary>
            ///   Disposes enumerator.</summary>
            public void Dispose()
            {
            }

            /// <summary>
            ///   This enumerator doesn't support resetting.</summary>
            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
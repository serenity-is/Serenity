using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   An index that holds a list of Rows ordered by one or more fields in AVP tree (balanced)
    ///   structure.</summary>
    /// <typeparam name="T">
    ///   A subclass of Row.</typeparam>
    /// <remarks>
    ///   It is assumed that rows that are added to index are immutable (thus won't change after adding).
    ///   A change in such a rows ordered fields might cause index state to be invalid.
    ///   This might result in false search results or enumeration, so once a row is added to the index,
    ///   its ordered field values shouldn't be changed, or it should be removed first, changed and
    ///   added again.</remarks>
    public class DbIndex<T> where T : Row
    {
        /// <summary>indexes of fields in the row this index is sorted on</summary>
        internal Field[] _fields;
        /// <summary>number of fields this index is sorted on</summary>
        internal int _fieldCount;
        /// <summary>a flag that contains each sorted fields descending order.</summary>
        internal bool[] _descending;
        /// <summary>next ID value for the index node that will be created. increases automatically.</summary>
        internal int _lastNodeID;
        /// <summary>root tree node</summary>
        private Node _root;
        /// <summary>number of nodes in the index tree (also the record count).</summary>
        private int _count;

        /// <summary>
        ///   Creates a new DbIndex sorted on fields at indexes specified.</summary>
        /// <param name="fields">
        ///   Field indexes of fields that the index will be sorted on.</param>
        public DbIndex(params Field[] fields)
        {
            Initialize(fields, null, DbIndexFlags.None);
        }

        /// <summary>
        ///   Creates a new DbIndex sorted on fields at indexes specified.</summary>
        /// <param name="fields">
        ///   Field indexes of fields that the index will be sorted on.</param>
        /// <param name="descending">
        ///   Descending flags for each field specified in "fields" parameter.</param>
        /// <param name="flags">
        ///   Index flags</param>
        public DbIndex(Field[] fields, bool[] descending, DbIndexFlags flags)
        {
            Initialize(fields, descending, flags);
        }

        /// <summary>
        ///   Initializes index structure.</summary>
        /// <param name="fields">
        ///   Field indexes of fields that the index will be sorted on.</param>
        /// <param name="descending">
        ///   Descending flags for each field specified in "fields" parameter.</param>
        /// <param name="flags">
        ///   Index flags</param>
        private void Initialize(Field[] fields, bool[] descending, DbIndexFlags flags)
        {
            _fields = fields;
            _fieldCount = fields.Length;
            if (descending != null)
                _descending = descending;
            else
                _descending = new bool[fields.Length];
            _count = 0;
        }

        /// <summary>
        ///   Adds row to the index in suitable position.</summary>
        /// <param name="row">
        ///   Row to add to the index. Its sorted fields should stay unchanged as long as in the index.</param>
        public void Add(T row)
        {
            if (row == null)
                throw new ArgumentNullException();

            // create a new non-linked node
            Node n = new Node(row, _lastNodeID++);
            // increase record count
            _count++;

            // if no root node (no records added before or all deleted), determine this node as root and quit
            if (_root == null)
            {
                _root = n;
                return;
            }

            // start from the root, perform a binary search, find suitable position and insert node
            // at that position
            Node p = _root;
            for (; ; )
            {
                int comparer = CompareNodes(n, p, _fieldCount);

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

            // if newly added node is unbalanced, rebalance it
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
        ///   Clears the index by removing all rows/nodes.</summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        /// <summary>
        ///   Compares two rows based on index fields.</summary>
        /// <param name="row1">
        ///   Row 1.</param>
        /// <param name="row2">
        ///   Row 2.</param>
        /// <param name="fieldCount">
        ///   How many of sorted fields will be used for comparison.</param>
        /// <returns>
        ///   If row1 is smaller than row2 based on index order, &lt;0, if larger &gt;1, if equal 0.</returns>
        internal int CompareRows(Row row1, Row row2, int fieldCount)
        {
            int compareResult = 0;

            int fieldIndex = 0;
            while (fieldIndex < fieldCount)
            {
                compareResult = _fields[fieldIndex].IndexCompare(row1, row2);

                if (compareResult != 0)
                    break;

                fieldIndex++;
            }

            if (compareResult != 0 && _descending[fieldIndex])
                compareResult = -compareResult;

            return compareResult;
        }

        /// <summary>
        ///   Compares two nodes based on index fields and their node ID's if fields are equal.</summary>
        /// <param name="node1">
        ///   Node 1.</param>
        /// <param name="node2">
        ///   Node 2.</param>
        /// <param name="fieldCount">
        ///   How many of sorted fields will be used for comparison.</param>
        /// <returns>
        ///   If node1 is smaller than node2 based on index order and node ID, &lt;0, if larger &gt;1, if equal 0.</returns>
        internal int CompareNodes(Node node1, Node node2, int fieldCount)
        {
            int compareResult = 0;

            Row row1 = node1._row;
            Row row2 = node2._row;

            int fieldIndex = 0;
            while (fieldIndex < fieldCount)
            {
                compareResult = _fields[fieldIndex].IndexCompare(row1, row2);
                if (compareResult != 0)
                    break;

                fieldIndex++;
            }

            if (compareResult != 0)
            {
                if (_descending[fieldIndex])
                    compareResult = -compareResult;

                return compareResult;
            }
            else
            {
                if (fieldIndex < _descending.Length &&
                    _descending[fieldIndex])
                    return node2._id - node1._id;
                else
                    return node1._id - node2._id;
            }
        }

        /// <summary>
        ///   Copies rows in the index to an array in order.</summary>
        /// <param name="array">
        ///   Target array (required). Must have at least record count length.</param>
        /// <param name="index">
        ///   Index to start copying elements to array. Usually 0.</param>
        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if ((index < 0) || (index >= array.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((array.Length - index) < _count)
                throw new ArgumentException("index");

            if (_root != null)
            {
                Node p = _root;

                while (p._left != null)
                    p = p._left;

                for (; ; )
                {
                    array[index] = p._row;

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
        ///   Searches for row in the index based on sorted fields. It is not required
        ///   for the row to be same, its index fields should be same.</summary>
        /// <param name="row">
        ///   Row to search</param>
        /// <param name="fieldCount">
        ///   How many index fields will be used for searching?</param>
        /// <returns>
        ///   If row is found in the index <c>true</c>.</returns>
        public bool Contains(T row, int fieldCount)
        {
            return Find(row, fieldCount) != null;
        }

        /// <summary>
        ///   Searches for row in the index based on sorted fields. It is not required
        ///   for the row to be same, its index fields should be same.</summary>
        /// <param name="row">
        ///   Row to search</param>
        /// <returns>
        ///   If row is found in the index <c>true</c>.</returns>
        public bool Contains(T row)
        {
            return Find(row, _fieldCount) != null;
        }

        /// <summary>
        ///   Searches for row in the index based on sorted fields. It is not required
        ///   for the row to be same, its index fields should be same.</summary>
        /// <param name="row">
        ///   Row to search</param>
        /// <returns>
        ///   The found row, or null if not found.</returns>
        public T Find(T row)
        {
            return Find(row, _fieldCount);
        }

        /// <summary>
        ///   Searches for row in the index based on sorted fields. It is not required
        ///   for the row to be same, its index fields should be same.</summary>
        /// <param name="row">
        ///   Row to search</param>
        /// <param name="fieldCount">
        ///   How many index fields will be used for searching?</param>
        /// <returns>
        ///   The found row, or null if not found.</returns>
        public T Find(T row, int fieldCount)
        {
            if (row == null)
            {
                throw new ArgumentNullException();
            }

            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

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
                    return p._row;
                }
            }

            return null;
        }

        /// <summary>
        ///   Searches for the row in the index based on sorted fields and removes it. If found, 
        ///   and removed returns the row removed.</summary>
        /// <param name="row">
        ///   Row with field values to search.</param>
        /// <returns>
        ///   Row that is just removed from the index.</returns>
        public T Remove(T row)
        {
            if (row == null)
            {
                throw new ArgumentNullException();
            }

            for (Node p = _root; p != null; )
            {
                int Comparer = CompareRows(row, p._row, _fieldCount);

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
                    T foundRow = p._row;

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
                    return foundRow;
                }
            }

            return null;
        }

        /// <summary>
        ///   Find the first node that comes before a node in order, and has same sorted field
        ///   values.</summary>
        /// <param name="p">
        ///   Node.</param>
        /// <param name="fieldCount">
        ///   How many fields to use in search.</param>
        /// <returns>
        ///   Node itself or first node with same field values.</returns>
        internal Node GoPriorWhileEqual(Node p, int fieldCount)
        {
            while (p._left != null && CompareRows(p._row, p._left._row, fieldCount) == 0)
                p = p._left;

            while (true)
            {
                Node x = p.GetPrior();
                if (x == null || CompareRows(p._row, x._row, fieldCount) != 0)
                    break;
                p = x;
            }

            return p;
        }

        /// <summary>
        ///   Find the last node that comes after a node in order, and has same sorted field
        ///   values.</summary>
        /// <param name="p">
        ///   Node.</param>
        /// <param name="fieldCount">
        ///   How many fields to use in search.</param>
        /// <returns>
        ///   Node itself or last node with same field values.</returns>
        internal Node GoNextWhileEqual(Node p, int fieldCount)
        {
            while (p._right != null && CompareRows(p._row, p._right._row, fieldCount) == 0)
                p = p._right;

            while (true)
            {
                Node x = p.GetNext();
                if (x == null || CompareRows(p._row, x._row, fieldCount) != 0)
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
        ///   Gets first node that has same sorted field values with specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   First node with same sorted field values, or null if none.</returns>
        internal Node GetFirstEQ(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return GoPriorWhileEqual(p, fieldCount);
            }
            return null;
        }

        /// <summary>
        ///   Gets first node that has same or greater sorted field values with specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   First node with same or greater sorted field values, or null if none.</returns>
        internal Node GetFirstGE(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

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
                    return GoPriorWhileEqual(p, fieldCount);
            }
            return null;
        }

        /// <summary>
        ///   Gets first node that has greater sorted field values than specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   First node with greater sorted field values, or null if none.</returns>
        internal Node GetFirstGT(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

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
        ///   Gets last node that has same sorted field values with specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   Last node with same sorted field values, or null if none.</returns>
        internal Node GetLastEQ(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return GoNextWhileEqual(p, fieldCount);
            }
            return null;
        }

        /// <summary>
        ///   Gets last node that has same or smaller sorted field values with specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   Last node with same or smaller sorted field values, or null if none.</returns>
        internal Node GetLastLE(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

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
                    return GoNextWhileEqual(p, fieldCount);
            }
            return null;
        }

        /// <summary>
        ///   Gets last node that has smaller sorted field values than specified row.</summary>
        /// <param name="row">
        ///   Row with sorted field values.</param>
        /// <param name="fieldCount">
        ///   How many fields to use.</param>
        /// <returns>
        ///   Last node with smaller sorted field values, or null if none.</returns>
        internal Node GetLastLT(T row, int fieldCount)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = CompareRows(row, p._row, fieldCount);

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
        ///   Gets an enumerator to traverse rows in ascending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public AscendingOrderEnumerator GetEnumerator()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Gets an enumerator to traverse rows in ascending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public AscendingOrderEnumerator Ascending()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Gets an enumerator to traverse rows in descending order.</summary>
        /// <returns>
        ///   Enumerator.</returns>
        public DescendingOrderEnumerator Descending()
        {
            return new DescendingOrderEnumerator(GetLast());
        }

        /// <summary>
        ///   Partitions index based on a key row, and returns an enumerator for rows in one 
        ///   side of partition in order.</summary>
        /// <param name="row">
        ///   Key row.</param>
        /// <param name="op">
        ///   Operator that determines the partition. See <see cref="IndexMatch"/></param>
        /// <returns>
        ///   Enumerator</returns>
        public IEnumerable<T> Match(T row, IndexMatch op)
        {
            return Match(row, op, false, _fieldCount);
        }

        /// <summary>
        ///   Partitions index based on a key row, and returns an enumerator for rows in one 
        ///   side of partition in order.</summary>
        /// <param name="row">
        ///   Key row.</param>
        /// <param name="op">
        ///   Operator that determines the partition. See <see cref="IndexMatch"/></param>
        /// <param name="descending">
        ///   True to return a descending enumerator</param>
        /// <returns>
        ///   Enumerator</returns>
        public IEnumerable<T> Match(T row, IndexMatch op, bool descending)
        {
            return Match(row, op, descending, _fieldCount);
        }

        /// <summary>
        ///   Partitions index based on a key row, and returns an enumerator for rows in one 
        ///   side of partition in order.</summary>
        /// <param name="row">
        ///   Key row.</param>
        /// <param name="op">
        ///   Operator that determines the partition. See <see cref="IndexMatch"/></param>
        /// <param name="descending">
        ///   True to return a descending enumerator</param>
        /// <param name="fieldCount">
        ///   How many fields will be used for comparisons</param>
        /// <returns>
        ///   Enumerator</returns>
        public IEnumerable<T> Match(T row, IndexMatch op, bool descending, int fieldCount)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (fieldCount < 1 || fieldCount > _fieldCount)
                throw new ArgumentOutOfRangeException("fieldCount");

            Node first;
            Node last = null;

            switch (op)
            {
                case IndexMatch.EqualTo:
                    first = GetFirstEQ(row, fieldCount);
                    last = first == null ? null : GetLastEQ(row, fieldCount);
                    break;
                case IndexMatch.GreaterEqual:
                    first = GetFirstGE(row, fieldCount);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.GreaterThan:
                    first = GetFirstGT(row, fieldCount);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.LessEqual:
                    last = GetLastLE(row, fieldCount);
                    first = last == null ? null : GetFirst();
                    break;
                case IndexMatch.LessThan:
                    last = GetLastLT(row, fieldCount);
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

                    yield return last._row;

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

                    yield return first._row;

                    if (first == last)
                        first = null;
                    else
                        first = first.GetNext();
                }
            }
        }

        /// <summary>
        ///   Gets number of rows in the index.</summary>
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
            /// <summary>the row node contains</summary>
            public T _row;
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
            /// <param name="row">
            ///   Row of the node.</param>
            /// <param name="id">
            ///   Autoincrementing ID assigned to the node.</param>
            public Node(T row, int id)
            {
                _row = row;
                _id = id;
            }

            /// <summary>
            ///   Creates a new node.</summary>
            /// <param name="row">
            ///   Row of the node.</param>
            /// <param name="id">
            ///   Autoincrementing ID assigned to the node.</param>
            /// <param name="parent">
            ///   Parent of the node</param>
            public Node(T row, int id, Node parent)
            {
                _row = row;
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
                get { return _current; }
            }

            /// <summary>
            ///   Gets current node.</summary>
            object IEnumerator.Current
            {
                get { return _current; }
            }

            /// <summary>
            ///   Moves enumerator to the next node.</summary>
            public bool MoveNext()
            {
                if (_next == null)
                    return false;

                _current = _next._row;

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
                _current = null;
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
                _current = null;
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
                get { return _current; }
            }

            /// <summary>
            ///   Gets current node.</summary>
            object IEnumerator.Current
            {
                get { return _current; }
            }

            /// <summary>
            ///   Moves enumerator to the next node.</summary>
            public bool MoveNext()
            {
                if (_prior == null)
                    return false;

                _current = _prior._row;

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
                _current = null;
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
                _current = null;
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
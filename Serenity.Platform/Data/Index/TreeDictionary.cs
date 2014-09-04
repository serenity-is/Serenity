using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
 
    ///   it works like an AVP tree (balanced), which can store a list consists of key/value pairs ordered by key.Keys have to be unique in list
    /// <typeparam name="TKey">
    ///   Key type.</typeparam>
    /// <typeparam name="TValue">
    ///   Type of values referenced by keys.</typeparam>
    /// <remarks>
    ///   Keys in the dictionary are immutable
    ///   Any change on a key added to the index may affect specified index
    ///   For instance, you may not find some keys when you have made a search.
    ///   Do not change any key. if you need to change a key, first delete it then add again.
    ///</remarks>
    public class TreeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        /// <summary>root</summary>
        private Node _root;
        /// <summary>pairs count.</summary>
        private int _count;
        /// <summary>The Interface which is used for comparing keys</summary>/// 
        private IComparer<TKey> _comparer;

        /// <summary>
        ///   Create a new TreeDictionary.As a comparer default comparer of type is used 
        /// (<c>Comparer&lt;TKey&gt;.Default</c>).</summary>
        public TreeDictionary()
        {
            _count = 0;
            _comparer = Comparer<TKey>.Default;
        }


        /// <summary>
        ///   Create an empty TreeDictionary with a passed comparer parameter</summary>
        /// <param name="comparer">
        ///   if comparer sets null, use default comparer</param>
        public TreeDictionary(IComparer<TKey> comparer)
            : this()
        {
            if (comparer != null)
                _comparer = comparer;
        }

        /// <summary>
        ///   Adds the given key to the dictionary with its related value </summary>
        /// <param name="key">
        ///  The key is added to dictionary. if the key being already added to dictionary, it throws an error</param>
        /// <param name="value">
        ///   The value referenced by key.</param>
        /// <exception cref="ArgumentException">
        ///   The key you want to add has been already existed in the dictionary.</exception>
        public void Add(TKey key, TValue value)
        {
            Set(key, value, true);
        }

        /// <summary>
        ///   Adds the given key to the dictionary with its related value </summary>
        /// <param name="key">
        ///  The key will be added to dictionary. if the key has been already existed in the dictionary, it occurs an error</param>
        /// <param name="value">
        ///   The value referenced by key.</param>
        /// <exception cref="ArgumentException">
        ///   The key you want to add has been already existed in the dictionary.</exception>
        public void Set(TKey key, TValue value)
        {
            Set(key, value, false);
        }

        /// <summary>
        ///   Adds the key to dictionary with associated value.if the key has been already existed in the dictionary, it overwrites to value referenced by existence key <paramref name="throwErrorIfExists"/>
        ///   if it returns true, it throws error</summary>
        /// <param name="key">
        ///   The key will be added to dictionary or updated.</param>
        /// <param name="value">
        ///   The value referenced by key.</param>
        /// <param name="throwErrorIfExists">
        ///   Determines if it throws an error in case of the given key has been already existed 
        ///   <see cref="Add"/> uses </param>
        /// <exception cref="ArgumentException">
        ///   There is a key has been already added to dictionary with same name in the Sözlükte eklenmek             <paramref name="throwErrorIfExists"/> and
        ///   true.</exception>
        private void Set(TKey key, TValue value, bool throwErrorIfExists)
        {
            Node n = new Node(key, value);

            if (_root == null)
            {
                _root = n;
                _count++;
                return;
            }

            Node p = _root;
            for (; ; )
            {
                int comparer = _comparer.Compare(n._item.Key, p._item.Key);

                if (comparer == 0)
                {
                    if (throwErrorIfExists)
                        throw new ArgumentException("An element with the same key already exists in dictionary!");
                    
                    p._item = n._item;
                    return;
                }

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

            _count++;
        }

        /// <summary>
        ///   Copy all pairs in the dictionary to an array in the order their keys</summary>        
        /// <param name="array">
        ///   The array object which all pairs in the dictionary is copied(required).</param>
        /// <param name="index">
        /// The Index specifies where all values in the array will be begun to write deðerlerin 
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="array"/> null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> Out of the array range</exception>
        /// <exception cref="ArgumentException">
        /// there is no enough place can contain for all pairs in the dictionary with passed given index</exception>   
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
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
        ///   Delete Pair with given key</summary>
        /// <param name="key">
        ///   Key will be deleted</param>
        /// <returns>
        ///   if found and deleted returns true. if not so, error doesn't occurs, returns false.</returns>
        public bool Remove(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int Comparer = _comparer.Compare(key, p._item.Key);

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
        ///   Returns the first node in the dictionary according to the current order</summary>
        /// <returns>
        ///  first node according to current key order . if dictionary is empty returns null.</returns>
        internal Node GetFirst()
        {
            Node p = _root;
            if (p != null)
                while (p._left != null)
                    p = p._left;
            return p;
        }

        /// <summary>
        ///   Returns the last node in the dictionary according to the current order</summary>
        /// <returns>
        ///  The last node according to current key order . if dictionary is empty returns null.</returns>
        internal Node GetLast()
        {
            Node p = _root;
            if (p != null)
                while (p._right != null)
                    p = p._right;
            return p;
        }

        /// <summary>
        ///   returns the first node that its key equals the passed parameter key</summary>
        /// <param name="key">
        ///   Key to be searched.</param>
        /// <returns>
        ///   The first node has the first searched key . if it doesn't be found, returns  null.</returns>   
        internal Node GetFirstEQ(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return p;
            }
            return null;
        }

        /// <summary>
        ///   returns the first node that its key equals or greater than the passed parameter key</summary>
        /// <param name="key">
        ///   Key to be searched</param>
        /// <returns>
        ///   returns the first node that its key equals or greater than the passed parameter key.if it doesn             't be found returns null
        /// </returns>
        internal Node GetFirstGE(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

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
                    return p;
            }
            return null;
        }

        /// <summary>
        ///   returns the first node that its key greater than the passed parameter key</summary>
        /// <param name="key">
        ///  Key to be searched</param>
        /// <returns>
        ///   returns the first node that its key greater than the passed parameter key.if it doesn't be found returns null
        /// </returns>
        internal Node GetFirstGT(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

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
        ///   returns the last node that its key equals the passed parameter key</summary>
        /// <param name="key">
        ///   Key to be searched.</param>
        /// <returns>
        ///   The last node has the first searched key . if it doesn't be found, returns  null.</returns>
        internal Node GetLastEQ(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

                if (comparer < 0)
                {
                    p = p._left;
                }
                else if (comparer > 0)
                {
                    p = p._right;
                }
                else
                    return p;
            }
            return null;
        }

        /// <returns>
        ///   Aranan anahtara ya da küçük anahtara sahip son düðüm. Bulunamazsa null.</returns>
        /// <summary>
        ///   returns the last node that its key equals or less than the passed parameter key</summary>
        /// <param name="key">
        ///   Key to be searched.</param>
        /// <returns>
        ///   returns the last node that its key less than the passed parameter key.if it doesn't be found returns null
        /// </returns>
        internal Node GetLastLE(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

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
                    return p;
            }
            return null;
        }

        /// <summary>
        ///  Returns the last node that its key less than the passed parameter key</summary>
        /// <param name="key">
        ///  Key to be searched</param>
        /// <returns>
        ///  Returns the last node that its key less than the passed parameter key.if it doesn't be found returns null
        /// </returns>
        internal Node GetLastLT(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

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
        /// whehter dictionary contains the passed key parameter or not.</summary>
        /// <param name="key">
        /// Key to be searched</param>
        /// <returns>
        /// If Dictionary<string, object>() contains the key returns null else false</returns>
        public bool Contains(TKey key)
        {
            return Find(key) != null;
        }

        /// <returns>
        ///   Anahtar deðere sahip düðüm. Bulunamazsa null.</returns>
        /// <summary>
        ///   Returns the node has passed key parameter.</summary>
        /// <param name="key">
        ///   Key to be searched</param>
        /// <returns>
        ///   The node has passed key parameter. if it fails, returns null.</returns>
        private Node Find(TKey key)
        {
            for (Node p = _root; p != null; )
            {
                int comparer = _comparer.Compare(key, p._item.Key);

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
        ///   Delete all pairs in the dictionary</summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }


        /// <summary>
        ///   Creates an enumerator which order all pairs by ascending
        ///   </summary>
        /// <returns>
        ///   Enumerator in required order</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Creates an enumerator which order all pairs by ascending
        ///   </summary>
        /// <returns>
        ///   Enumerator in required order</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Creates an enumerator which order all pairs by ascending
        ///   </summary>
        /// <returns>
        ///   Enumerator in required order</returns>     
        public IEnumerator<KeyValuePair<TKey, TValue>> Ascending()
        {
            return new AscendingOrderEnumerator(GetFirst());
        }

        /// <summary>
        ///   Creates an enumerator which order all pairs by descending
        ///   </summary>
        /// <returns>
        ///   Enumerator in required order</returns>     
        public IEnumerator<KeyValuePair<TKey, TValue>> Descending()
        {
            return new DescendingOrderEnumerator(GetLast());
        }

 
        /// <summary>
        ///   Creates an enumerator to get pairs in a part of dictionary determined by a key in ascending order
        ///   </summary>
        /// <param name="key">
        ///   The key is used for specifying the cut point</param>
        /// <param name="op">
        ///   which part is being required referenced to cut point. Bkz. <see cref="IndexMatch"/></param>
        /// <returns>
        ///   Enumerator gets pairs by required order from specified range </returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> Match(TKey key, IndexMatch op)
        {
            return Match(key, op, false);
        }

        /// <param name="key">
        ///   The key is used for specifying the cut point</param>
        /// <param name="op">
        ///   which part is being required referenced to cut point. <see cref="IndexMatch"/></param>
        /// <param name="descending">
        ///   is descending order?</param>
        /// <returns>
        ///   Enumerator gets pairs by required order from specified range </returns>
        /// <summary>
        ///   Creates an enumerator gets pairs from determined part of dictionary,in ascending or descending order according to a passed key
        ///   </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> Match(TKey key, IndexMatch op, bool descending)
        {
            Node first;
            Node last = null;

            switch (op)
            {
                case IndexMatch.EqualTo:
                    first = GetFirstEQ(key);
                    last = first == null ? null : GetLastEQ(key);
                    break;
                case IndexMatch.GreaterEqual:
                    first = GetFirstGE(key);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.GreaterThan:
                    first = GetFirstGT(key);
                    if (descending)
                        last = first == null ? null : GetLast();
                    break;
                case IndexMatch.LessEqual:
                    last = GetLastLE(key);
                    first = last == null ? null : GetFirst();
                    break;
                case IndexMatch.LessThan:
                    last = GetLastLT(key);
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
        ///   Gets the pairs count in the dictionary</summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        ///   node in the index</summary>       
        internal sealed class Node
        {
            /// <summary>pair the node contains</summary>
            public KeyValuePair<TKey, TValue> _item;
            /// <summary>parent node</summary>
            public Node _parent;
            /// <summary>left node</summary>
            public Node _left;
            /// <summary>right node</summary>
            public Node _right;
            /// <summary>balance, must be 0, else become unbalanced</summary>
            public sbyte _balance;

            /// <summary>
            ///   Creates a new node</summary>
            /// <param name="key">
            ///   key attached the node</param>
            /// <param name="value">
            ///   value attached the node</param>
            public Node(TKey key, TValue value)
            {
                _item = new KeyValuePair<TKey, TValue>(key, value);
            }

            /// <summary>
            ///   Creates a new node.</summary>
            /// <param name="key">
            ///   Element of the node.</param>
            /// <param name="value">
            ///   Autoincrementing ID assigned to the node.</param>
            /// <param name="parent">
            ///   Parent of the node</param>
            public Node(TKey key, TValue value, Node parent)
            {
                _item = new KeyValuePair<TKey, TValue>(key, value);
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
        public struct AscendingOrderEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            /// <summary>next node to be returned</summary>
            private Node _next;
            /// <summary>the last node at which search stop</summary>
            private Node _last;
            /// <summary>current</summary>
            private KeyValuePair<TKey, TValue> _current;

            /// <summary>
            ///   returns current pairs on node.</summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   current</summary>           
            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            ///   iterate the enumerator to next node</summary>
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
            ///   Creates a new enumerator begining from given node</summary>
            /// <param name="first">
            ///   Node at which enumeration get begun. <c>null</c> allowed.</param>            
            internal AscendingOrderEnumerator(Node first)
            {
                _next = first;
                _current = default(KeyValuePair<TKey, TValue>);
                _last = null;
            }

            /// <summary>
            ///   Creates a enumerator begining from given node and end up at a required position</summary>
            /// <param name="first">
            ///   Node at which enumeration get begun. <c>null</c> allowed.</param>    
            /// <param name="last">
            ///   Node at which enumeration get finalised. <c>null</c> allowed.</param>    
            internal AscendingOrderEnumerator(Node first, Node last)
            {
                _next = first;
                _last = last;
                _current = default(KeyValuePair<TKey, TValue>);
            }

            /// <summary>
            ///   disposing </summary>
            public void Dispose()
            {
            }

            /// <summary>
            ///  no support for reset</summary>           
            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   Enumerator to traverse index in descending order.</summary>
        public struct DescendingOrderEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            /// <summary>Next node to be returned.</summary>
            private Node _prior;
            /// <summary>Node to end enumeration at.</summary>
            private Node _last;
            /// <summary>current pair on node</summary>
            private KeyValuePair<TKey, TValue> _current;

            /// <summary>current pair on node</summary>         
            public KeyValuePair<TKey, TValue> Current
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
                _current = default(KeyValuePair<TKey, TValue>);
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
                _current = default(KeyValuePair<TKey, TValue>);
            }

            /// <summary>
            ///   disposing</summary>
            public void Dispose()
            {
            }

            /// <summary>
            ///   This enumerator doesn't support resetting.</summary>>           
            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
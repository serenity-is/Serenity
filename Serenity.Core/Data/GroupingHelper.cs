using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public static class GroupingHelper
    {
        /// <summary>
        ///   Delegate function type used to compare an element with a starting element to be in the 
        ///   same group.</summary>
        /// <typeparam name="TElement">
        ///   Element type parameter.</typeparam>
        /// <param name="start">
        ///   First element in the group to be compared with current element.</param>
        /// <param name="current">
        ///   Current element.</param>
        /// <returns>
        ///   True if current element is in the same group with starting element.</returns>
        public delegate bool CheckInGroupDelegate<TElement>(TElement start, TElement current);

        /// <summary>
        ///   Delegate method called by ScanGroupRows for each element.</summary>
        /// <typeparam name="TElement">
        ///   Element type parameter.</typeparam>
        /// <param name="current">
        ///   Current element.</param>
        /// <param name="index">
        ///   Current element index in the list.</param>
        public delegate void ScanElementDelegate<TElement>(TElement current, ref int index);


        /// <summary>
        ///   A delegate that takes a <see cref="DbRow"/> subclass and an index and returns nothing</summary>
        /// <param name="row">
        ///   Row that will be passed to the callback.</param>
        /// <param name="index">
        ///   Index for the row.</param>
        public delegate void DbRowIndexCallBack<TRow>(TRow row, ref int index) where TRow : Row;

        /// <summary>
        ///   Helper function to scan rows in group system on a row list that is sorted by 
        ///   specific fields.</summary>
        /// <typeparam name="TRow">
        ///   Row class.</typeparam>
        /// <param name="rows">
        ///   List of rows to be scanned (required).</param>
        /// <param name="start">
        ///   Start index.</param>
        /// <param name="fields">
        ///   Sorted field list. As long as these fields keep same value, scanning will go on.</param>
        /// <param name="callBack">
        ///   A callback that will be called for each row in the group. It should increase index
        ///   parameter by at least one, otherwise it will be automatically increased by one.</param>
        /// <remarks>
        ///   When method returns, "start" contains the index of row after current groups last row.</remarks>
        public static void ScanGroupRows<TRow>(IList<TRow> rows, ref int start,
            Field[] fields, DbRowIndexCallBack<TRow> callBack) where TRow : Row
        {
            if (start < 0 || start >= rows.Count)
                throw new ArgumentOutOfRangeException("start");

            if (fields == null || fields.Length == 0)
                throw new ArgumentNullException("fields");

            TRow startRow = rows[start];
            int index = start;
            int check;

            while (index <= rows.Count)
            {
                TRow row = null;

                bool end = (index > start && index >= rows.Count);
                if (!end)
                {
                    row = rows[index];

                    if (index > start)
                    {
                        for (int f = 0; f < fields.Length; f++)
                            if (fields[f].IndexCompare(row, startRow) != 0)
                            {
                                end = true;
                                break;
                            }
                    }
                }

                if (end)
                {
                    start = index;
                    return;
                }

                check = index;
                callBack(row, ref index);
                if (index <= check)
                    index++;
            }
        }

        /// <summary>
        ///   Helper function to scan items in group system on a list that is sorted on specific fields.</summary>
        /// <typeparam name="TElement">
        ///   List element type.</typeparam>
        /// <param name="list">
        ///   List with elements that will be scanned (required).</param>
        /// <param name="start">
        ///   Start index.</param>
        /// <param name="checkInGroup">
        ///   A delegate that will check the element is in the current group. If it is, it should return true.</param>
        /// <param name="scanElement">
        ///   A scanning function that will be called for each element in the group. It should increase index 
        ///   parameter by at least one after each call. If not, it will be automatically increased by one.</param>
        /// <remarks>
        ///   When method returns, "start" is at the next element after group's last element.</remarks>
        public static void ScanGroupElements<TElement>(IList<TElement> list, ref int start,
            CheckInGroupDelegate<TElement> checkInGroup, ScanElementDelegate<TElement> scanElement)
            where TElement : class
        {
            if (start < 0 || start >= list.Count)
                throw new ArgumentOutOfRangeException("start");

            if (checkInGroup == null)
                throw new ArgumentNullException("checkInGroup");
            if (scanElement == null)
                throw new ArgumentNullException("groupElement");

            TElement startElement = list[start];
            int index = start;
            int check;
            while (index <= list.Count)
            {
                TElement element = null;

                bool end = (index > start && index >= list.Count);
                if (!end)
                {
                    element = list[index];
                    end = (index > start && !checkInGroup(startElement, element));
                }

                if (end)
                {
                    start = index;
                    return;
                }
                check = index;
                scanElement(element, ref index);
                if (index <= check)
                    index++;
            }
        }
    }
}
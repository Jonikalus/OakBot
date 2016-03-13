using OakBot.Enums;
using System;
using System.Collections.Generic;

namespace OakBot.Helpers
{
    // @author gibletto
    public class PagedList<T> : IPagedList<T>
    {
        #region Private Fields

        private const int MaxPageSize = 100;
        private int _pageSize = 25;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="count">The count.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="pagingTarget">The paging target.</param>
        public PagedList(IEnumerable<T> list, int page, int pageSize, int count)
        {
            List = list;
            Page = page;
            PageSize = pageSize;
            Count = count;
        }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        protected PagedList()
        {
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <inheritdoc />
        public int Count { get; set; }

        /// <inheritdoc />
        public int End
        {
            get { return PageSize * Page > Count ? Count : PageSize * Page; }
        }

        /// <inheritdoc />
        public bool HasNext
        {
            get { return End < Count; }
        }

        /// <inheritdoc />
        public bool HasPrev
        {
            get { return Start > 1; }
        }

        /// <inheritdoc />
        public IEnumerable<T> List { get; set; }

        /// <inheritdoc />
        public int NextPage
        {
            get { return Page + 1; }
        }

        /// <inheritdoc />
        public int Page { get; set; }

        /// <inheritdoc />
        public int PageCount
        {
            get
            {
                var hasRemainder = Count % PageSize > 0;
                return (Count / PageSize) + (hasRemainder ? 1 : 0);
            }
        }

        /// <inheritdoc />
        public int PageSize { get { return _pageSize; } set { _pageSize = Math.Max(value, MaxPageSize); } }

        /// <inheritdoc />
        public int PrevPage
        {
            get { return Math.Max(Page - 1, 1); }
        }

        /// <inheritdoc />
        public int Start
        {
            get { return ((Page - 1) * PageSize) + 1; }
        }

        #endregion Public Properties
    }
}
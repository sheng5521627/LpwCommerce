using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Domain.Common;
using Services.Events;

namespace Services.Common
{
    /// <summary>
    /// Search term service
    /// </summary>
    public partial class SearchTermService : ISearchTermService
    {
        #region Fields

        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public SearchTermService(IRepository<SearchTerm> searchTermRepository,
            IEventPublisher eventPublisher)
        {
            this._searchTermRepository = searchTermRepository;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteSearchTerm(SearchTerm searchTerm)
        {
            if (searchTerm == null)
                throw new ArgumentNullException("searchTerm");

            _searchTermRepository.Delete(searchTerm);

            //event notification
            _eventPublisher.EntityDeleted(searchTerm);
        }

        public SearchTerm GetSearchTermById(int searchTermId)
        {
            if (searchTermId == 0)
                return null;

            return _searchTermRepository.GetById(searchTermId);
        }

        public SearchTerm GetSearchTermByKeyword(string keyword, int storeId)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            var query = from st in _searchTermRepository.Table
                        where st.Keyword == keyword && st.StoreId == storeId
                        orderby st.Id
                        select st;
            var searchTerm = query.FirstOrDefault();
            return searchTerm;
        }

        public IPagedList<SearchTermReportLine> GetStats(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = (from st in _searchTermRepository.Table
                    group st by st.Keyword
                    into groupedResult
                    select new
                    {
                        Keyword = groupedResult.Key,
                        Count = groupedResult.Sum(m => m.Count)
                    })
                .OrderByDescending(m => m.Count)
                .Select(c => new SearchTermReportLine()
                {
                    Keyword = c.Keyword,
                    Count = c.Count
                });

            return new PagedList<SearchTermReportLine>(query, pageIndex, pageSize);
        }

        public void InsertSearchTerm(SearchTerm searchTerm)
        {
            if (searchTerm == null)
                throw new ArgumentNullException("searchTerm");

            _searchTermRepository.Insert(searchTerm);

            //event notification
            _eventPublisher.EntityInserted(searchTerm);
        }

        public void UpdateSearchTerm(SearchTerm searchTerm)
        {
            if (searchTerm == null)
                throw new ArgumentNullException("searchTerm");

            _searchTermRepository.Update(searchTerm);

            //event notification
            _eventPublisher.EntityUpdated(searchTerm);
        }

        #endregion
    }
}

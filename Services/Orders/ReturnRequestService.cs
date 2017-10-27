using Core.Data;
using Core.Domain.Orders;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Services.Orders
{
    /// <summary>
    /// Return request service
    /// </summary>
    public partial class ReturnRequestService : IReturnRequestService
    {
        #region Fields

        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="returnRequestRepository">Return request repository</param>
        /// <param name="returnRequestActionRepository">Return request action repository</param>
        /// <param name="returnRequestReasonRepository">Return request reason repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ReturnRequestService(IRepository<ReturnRequest> returnRequestRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository,
            IEventPublisher eventPublisher)
        {
            this._returnRequestRepository = returnRequestRepository;
            this._returnRequestActionRepository = returnRequestActionRepository;
            this._returnRequestReasonRepository = returnRequestReasonRepository;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            _returnRequestRepository.Delete(returnRequest);

            //event notification
            _eventPublisher.EntityDeleted(returnRequest);
        }

        public ReturnRequest GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            return _returnRequestRepository.GetById(returnRequestId);
        }

        public IPagedList<ReturnRequest> SearchReturnRequests(int storeId = 0, int customerId = 0, int orderItemId = 0, 
            ReturnRequestStatus? rs = default(ReturnRequestStatus?), int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _returnRequestRepository.Table;
            if (storeId > 0)
                query = query.Where(rr => storeId == rr.StoreId);
            if (customerId > 0)
                query = query.Where(rr => customerId == rr.CustomerId);
            if (rs.HasValue)
            {
                var returnStatusId = (int)rs.Value;
                query = query.Where(rr => rr.ReturnRequestStatusId == returnStatusId);
            }
            if (orderItemId > 0)
                query = query.Where(rr => rr.OrderItemId == orderItemId);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<ReturnRequest>(query, pageIndex, pageSize);
            return returnRequests;
        }

        public void DeleteReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException("returnRequestAction");

            _returnRequestActionRepository.Delete(returnRequestAction);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestAction);
        }

        public IList<ReturnRequestAction> GetAllReturnRequestActions()
        {
            var query = from rra in _returnRequestActionRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;
            return query.ToList();
        }

        public ReturnRequestAction GetReturnRequestActionById(int returnRequestActionId)
        {
            if (returnRequestActionId == 0)
                return null;

            return _returnRequestActionRepository.GetById(returnRequestActionId);
        }

        public void InsertReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException("returnRequestAction");

            _returnRequestActionRepository.Insert(returnRequestAction);

            //event notification
            _eventPublisher.EntityInserted(returnRequestAction);
        }

        public void UpdateReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException("returnRequestAction");

            _returnRequestActionRepository.Update(returnRequestAction);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestAction);
        }

        public void DeleteReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException("returnRequestReason");

            _returnRequestReasonRepository.Delete(returnRequestReason);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestReason);
        }

        public IList<ReturnRequestReason> GetAllReturnRequestReasons()
        {
            var query = from rra in _returnRequestReasonRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;
            return query.ToList();
        }

        public ReturnRequestReason GetReturnRequestReasonById(int returnRequestReasonId)
        {
            if (returnRequestReasonId == 0)
                return null;

            return _returnRequestReasonRepository.GetById(returnRequestReasonId);
        }

        public void InsertReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException("returnRequestReason");

            _returnRequestReasonRepository.Insert(returnRequestReason);

            //event notification
            _eventPublisher.EntityInserted(returnRequestReason);
        }

        public void UpdateReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException("returnRequestReason");

            _returnRequestReasonRepository.Update(returnRequestReason);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestReason);
        }

        #endregion
    }
}

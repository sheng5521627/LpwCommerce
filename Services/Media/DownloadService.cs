using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Media;
using Core.Domain.Orders;
using Core.Data;
using Services.Events;
using Core.Domain.Catalog;
using Core.Domain.Payments;

namespace Services.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        private readonly IRepository<Download> _downloadRepository;
        private readonly IEventPublisher _eventPubisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="downloadRepository">Download repository</param>
        /// <param name="eventPubisher"></param>
        public DownloadService(IRepository<Download> downloadRepository,
            IEventPublisher eventPubisher)
        {
            _downloadRepository = downloadRepository;
            _eventPubisher = eventPubisher;
        }

        #endregion

        public void DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Delete(download);

            _eventPubisher.EntityDeleted(download);
        }

        public Download GetDownloadByGuid(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var query = from d in _downloadRepository.Table
                        where d.DownloadGuid == downloadGuid
                        select d;

            return query.FirstOrDefault();
        }

        public Download GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;

            return _downloadRepository.GetById(downloadId);
        }

        public void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Insert(download);

            _eventPubisher.EntityInserted(download);
        }

        public bool IsDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem == null)
                return false;

            var order = orderItem.Order;
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            var product = orderItem.Product;
            if (product == null || !product.IsDownload)
                return false;

            switch (product.DownloadActivationType)
            {
                case DownloadActivationType.WhenOrderIsPaid:
                    {
                        if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                        {
                            //expiration date
                            if (product.DownloadExpirationDays.HasValue)
                            {
                                if (order.PaidDateUtc.Value.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case DownloadActivationType.Manually:
                    {
                        if (orderItem.IsDownloadActivated)
                        {
                            //expiration date
                            if (product.DownloadExpirationDays.HasValue)
                            {
                                if (order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        public bool IsLicenseDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem == null)
                return false;

            return IsDownloadAllowed(orderItem) &&
                orderItem.LicenseDownloadId.HasValue &&
                orderItem.LicenseDownloadId > 0;
        }

        public void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Update(download);

            _eventPubisher.EntityUpdated(download);
        }
    }
}
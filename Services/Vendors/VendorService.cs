using Core.Data;
using Core.Domain.Vendors;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="vendorRepository">Vendor repository</param>
        /// <param name="vendorNoteRepository">Vendor note repository</param>
        /// <param name="eventPublisher">Event published</param>
        public VendorService(IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository,
            IEventPublisher eventPublisher)
        {
            this._vendorRepository = vendorRepository;
            this._vendorNoteRepository = vendorNoteRepository;
            this._eventPublisher = eventPublisher;
        }

        public Vendor GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;

            return _vendorRepository.GetById(vendorId);
        }

        public void DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            vendor.Deleted = true;
            UpdateVendor(vendor);
        }

        public IPagedList<Vendor> GetAllVendors(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        public void InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }

        public void UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Update(vendor);

            //event notification
            _eventPublisher.EntityUpdated(vendor);
        }

        public VendorNote GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return _vendorNoteRepository.GetById(vendorNoteId);
        }

        public void DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException("vendorNote");

            _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    public class ApplicationUnit : IDisposable
    {
        private MarkingApiDbContext _context = new MarkingApiDbContext();
        private IRepository<MarkingRecords> _markingRecordsRepo = null;

        public IRepository<MarkingRecords> MarkingRecordsRepo
        {
            get
            {
                if (this._markingRecordsRepo == null)
                {
                    this._markingRecordsRepo = new BaseRepository<MarkingRecords>(this._context);
                }

                return this._markingRecordsRepo;
            }
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }
    }
}
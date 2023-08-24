using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Models
{
    public class MarkingRecordsRepository : BaseRepository<MarkingRecords>
    {
        public MarkingRecordsRepository(DbContext context) : base(context)
        {

        }
    }
}
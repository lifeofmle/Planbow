using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planbow.Data.Interfaces
{
    public interface IPlanbowUow
    {
        // Save pending changes to the data store.
        void Commit();

        // Repositories
        IPlanRepository Plans { get; }

        //IRepository<Room> Rooms { get; }
        //ISessionsRepository Sessions { get; }
        //IRepository<TimeSlot> TimeSlots { get; }
        //IRepository<Track> Tracks { get; }
        //IAttendanceRepository Attendance { get; }
    }
}

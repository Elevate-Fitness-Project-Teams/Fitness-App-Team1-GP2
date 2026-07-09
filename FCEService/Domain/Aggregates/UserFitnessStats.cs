using FCEService.Domain.Entities;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;

namespace FCEService.Domain.Aggregates
{
    public class UserFitnessStats:BaseEntity
    {   
        public int userId { get; set; }
        public  PhysicalStats physicalStats { get; set; }
        public Goal goal { get; set; } 
        public Activity activity { get; set; }
        public bool IsActive { get; set; } = true;
        public int WorkoutDays { get; set; } 
    }
}

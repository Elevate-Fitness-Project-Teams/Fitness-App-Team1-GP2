using FCEService.Domain.Entities;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;

namespace FCEService.Domain.Aggregates
{
    public class CalculatedMetrics : BaseEntity
    {
        public Guid UserId { get; set; }
        public double BMR { get; private set; }
        public double TDEE { get; private set; }
        public double CalorieTarget { get; private set; } 
        public BMRRange BMRRange { get; private set; }
        public BMRStatus BMRStatus { get; private set; }

        private CalculatedMetrics() { }

        public static CalculatedMetrics Calculate(UserFitnessStats stats)
        {
            var bmr = CalculateBMR(stats.physicalStats);
            var tdee = CalculateTDEE(bmr, stats.activity);
            var calorieTarget = CalculateCalorieTarget(tdee, stats.goal);
            var BMRRange = GetBMRRange(stats.physicalStats.Gender);

            return new CalculatedMetrics
            {
                UserId = stats.userId,
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                CalorieTarget = calorieTarget,
                BMRRange = GetBMRRange(stats.physicalStats.Gender),
                BMRStatus = GetBMRStatus(bmr, BMRRange)
            };
        }

        private static double CalculateCalorieTarget(double tdee, Goal goal)
        {
            if (goal == Goal.Weak)
               return Math.Round(tdee - 500, 2);
            else if (goal == Goal.Normal)
                return Math.Round(tdee, 2);
            else if (goal == Goal.Hard)
                return Math.Round(tdee + 500, 2);
            else
                throw new ArgumentException("Invalid goal");
        }

        private static BMRStatus GetBMRStatus(double bmr, BMRRange bMRRange)
        {
            if (bmr < bMRRange.Min)
                return BMRStatus.Underweight;
            else if (bmr > bMRRange.Max)
                return BMRStatus.Overweight;
            else
                return BMRStatus.Normal;
        }

        private static BMRRange GetBMRRange(Gender gender)
        {
            if(gender == Gender.Male)
                return new MaleBMRRange();
            else if(gender == Gender.Female)
                return new FemaleBMRRange();
            else
                throw new ArgumentException("Invalid gender");
        }

        private static double CalculateTDEE(double bmr, Activity activity)
        {
            if(activity == Activity.Rookie)
                return bmr * 1.2;
            else if (activity == Activity.Beginner)
                return bmr * 1.375;
            else if (activity == Activity.Intermediate)
                return bmr * 1.55;
            else if (activity == Activity.Advanced)
                return bmr * 1.725;
            else if(activity == Activity.TrueBeast)
                return bmr * 1.9;
            else
                throw new ArgumentException("Invalid activity level");
        }

        private static double CalculateBMR(PhysicalStats physicalStats)
        {
            return (physicalStats.Gender == Enums.Gender.Male)
                ?(10*physicalStats.Weight)+(6.5*physicalStats.Height)-(5*physicalStats.Age)+5
                :(10*physicalStats.Weight)+(6.5*physicalStats.Height)-(5*physicalStats.Age)-161;
        }

    }
}

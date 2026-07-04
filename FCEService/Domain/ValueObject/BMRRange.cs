namespace FCEService.Domain.ValueObject
{
    public record BMRRange(double Min, double Max);
    public record MaleBMRRange(double Min= 1700, double Max = 2100):BMRRange(Min, Max);
    public record FemaleBMRRange(double Min = 1300, double Max = 1600):BMRRange(Min, Max);

}

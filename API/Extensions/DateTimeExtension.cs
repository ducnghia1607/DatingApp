namespace API;

public static class DateTimeExtension
{
    public static int CalculateAge(this DateOnly date)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = now.Year - date.Year;
        if (date > now.AddYears(-age)) age--;
        return age;
    }
}

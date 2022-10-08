namespace TDDBuget;

public class BudgetService
{
    private readonly IBudgetRepo _repo;
    private readonly Budget _defaultBudget;
    
    public BudgetService(IBudgetRepo repo)
    {
        _repo = repo;
        _defaultBudget = new Budget()
        {
            YearMonth = "",
            Amount = 0
        };
    }
    
    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }
        var budgetData =  _repo.GetAll();

        if (start.Month == end.Month && start.Year == end.Year)
        {
            var result = budgetData.Where(x => x.YearMonth == start.ToString("yyyyMM")).FirstOrDefault(_defaultBudget);
            if (start.Day == 1 && (end.AddDays(1).Month - start.Month) == 1)
            {
                return result.Amount;
            }
            var budgetDays = (end - start).Days + 1;
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
            return CalculateBudget(result, daysInMonth, budgetDays);
        }

        var endTime = new DateTime(end.Year,end.Month,01);
        var queryList = new List<string>();
        for (var currentTime = start.AddMonths(1); currentTime < endTime; currentTime = currentTime.AddMonths(1))
        {
            queryList.Add(currentTime.ToString("yyyyMM"));
        }

        var budgets = budgetData.Where(w => queryList.Contains(w.YearMonth));
        var sum = budgets.Sum(s => s.Amount);

        var startBudget = budgetData.Where(x => x.YearMonth == start.ToString("yyyyMM")).FirstOrDefault(_defaultBudget);
        var endBudget = budgetData.Where(x => x.YearMonth == end.ToString("yyyyMM")).FirstOrDefault(_defaultBudget);
        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
        var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);
        var startDays = startMonthDays - start.Day + 1;
        var endDays = end.Day;
        return CalculateBudget(startBudget, startMonthDays, startDays) +
               CalculateBudget(endBudget, endMonthDays, endDays) + sum;
    }

    private static int CalculateBudget(Budget result, int daysInMonth, int budgetDays)
    {
        return result.Amount / daysInMonth * budgetDays;
    }
}
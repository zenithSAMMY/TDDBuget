using System;
using System.Collections.Generic;
using NUnit.Framework;
using TDDBuget;

namespace TDDBudgetTest;

public class BudgetServiceTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BadRequest()
    {
        var service = new BudgetService(new FakeSingleRepo());
        var result = service.Query(new DateTime(2022, 10, 30), new DateTime(2022, 10, 1));
        Assert.AreEqual(0, result);
    }
    
    [Test]
    public void AllMonth()
    {
        var service = new BudgetService(new FakeSingleRepo());
        var result = service.Query(new DateTime(2022, 10, 01), new DateTime(2022, 10, 31));
        Assert.AreEqual(3100, result);
    }
    
    [Test]
    public void PartialMonth()
    {
        var service = new BudgetService(new FakeSingleRepo());
        var result = service.Query(new DateTime(2022, 10, 01), new DateTime(2022, 10, 15));
        Assert.AreEqual(1500, result);
    }
    
    [Test]
    public void CrossTwoMonth()
    {
        var service = new BudgetService(new FakeCrossRepo());
        var result = service.Query(new DateTime(2022, 10, 15), new DateTime(2022, 11, 15));
        Assert.AreEqual(1850, result);
    }
    
    [Test]
    public void CrossThreeMonth()
    {
        var service = new BudgetService(new FakeCrossRepo());
        var result = service.Query(new DateTime(2022, 10, 15), new DateTime(2022, 12, 3));
        // 1700 + 300 + 3
        Assert.AreEqual(2003, result);
    }
    
    [Test]
    public void NoBudget()
    {
        var service = new BudgetService(new FakeCrossRepo());
        var result = service.Query(new DateTime(2022, 1, 1), new DateTime(2022, 9, 3));
        Assert.AreEqual(0, result);
    }
    
    [Test]
    public void CrossMonthPartialMonthsNoBudget()
    {
        var service = new BudgetService(new FakeCrossRepo());
        var result = service.Query(new DateTime(2022, 1, 1), new DateTime(2022, 10, 3));
        Assert.AreEqual(300, result);
    }
    
    [Test]
    public void CrossYearsBudget()
    {
        var service = new BudgetService(new FakeCrossRepo());
        var result = service.Query(new DateTime(2021, 10, 1), new DateTime(2022, 10, 3));
        Assert.AreEqual(31300, result);
    }
}

internal class FakeSingleRepo : IBudgetRepo
{
    public List<Budget> GetAll()
    {
        return new List<Budget>()
        {
            new()
            {
                YearMonth = "202210",
                Amount = 3100
            }
        };
    }
}

internal class FakeCrossRepo : IBudgetRepo
{
    public List<Budget> GetAll()
    {
        return new List<Budget>()
        {
            new()
            {
                YearMonth = "202110",
                Amount = 31000
            },
            new()
            {
                YearMonth = "202210",
                Amount = 3100
            },
            new ()
            {
                YearMonth = "202211",
                Amount = 300
            },
            new()
            {
                YearMonth = "202212",
                Amount = 31
            }
        };
    }
}
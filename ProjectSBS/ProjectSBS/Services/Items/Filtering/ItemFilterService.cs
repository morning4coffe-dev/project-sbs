﻿namespace ProjectSBS.Services.Items.Filtering;

public class ItemFilterService : IItemFilterService
{
    public List<FilterCategory> Categories { get; }

    public ItemFilterService(IStringLocalizer localizer)
    {
        //TODO: Add proper Selectors for FilterCategories
        Categories = new()
        {
            new(localizer["Home"], "\uE80F"),
            new(localizer["Upcoming"], "\uE752", i => !i.IsPaid),
            new(localizer["Overdue"], "\uEC92", i => i.Item.Name is "Sample Item 1"),
            new(localizer["Expensive"], "🥩", i => i.Item.Billing.BasePrice > 50),
        };
    }
}

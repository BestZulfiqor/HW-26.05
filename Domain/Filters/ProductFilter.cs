﻿namespace Domain.Filters;

public class ProductFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
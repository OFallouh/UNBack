using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UNNew.Filters;
using System.Globalization;

public static class FilterHelper
{
    //public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, FilterModel filterRequest)
    //{
    //    if (filterRequest?.Filters?.Count > 0)
    //    {
    //        foreach (var filter in filterRequest.Filters)
    //        {
    //            string field = filter.Key;
    //            string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);
    //            JsonElement filterValue = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(filterJsonString);

    //            if (filterValue.TryGetProperty("Value", out JsonElement valueElement))
    //            {
    //                if (valueElement.ValueKind == JsonValueKind.Null ||
    //                    (valueElement.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(valueElement.GetString())))
    //                {
    //                    continue;
    //                }
    //            }

    //            if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
    //            {
    //                string matchMode = matchModeElement.GetString();
    //                JsonElement value = filterValue.GetProperty("Value");

    //                PropertyInfo property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    //                if (property == null)
    //                    continue;

    //                // Build the expression dynamically
    //                var parameter = Expression.Parameter(typeof(T), "entity");
    //                var propertyExpression = Expression.Property(parameter, property);
    //                Expression filterExpression = null;

    //                string stringValue = value.ValueKind == JsonValueKind.String ? value.GetString().ToLower() : null;
    //                bool? boolValue = value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False ? value.GetBoolean() : (bool?)null;
    //                decimal? decimalValue = value.ValueKind == JsonValueKind.Number ? value.GetDecimal() : (decimal?)null;
    //                DateTime? dateTimeValue = value.ValueKind == JsonValueKind.String && DateTime.TryParse(value.GetString(), out DateTime tempDate) ? tempDate : (DateTime?)null;

    //                switch (matchMode?.ToLowerInvariant())
    //                {
    //                    case "equals":
    //                        filterExpression = BuildEqualExpression(propertyExpression, property.PropertyType, stringValue, boolValue, decimalValue, dateTimeValue);
    //                        break;
    //                    case "notequals":
    //                    case "not_equals":
    //                        filterExpression = Expression.Not(BuildEqualExpression(propertyExpression, property.PropertyType, stringValue, boolValue, decimalValue, dateTimeValue));
    //                        break;
    //                    case "contains":
    //                        if (property.PropertyType == typeof(string) && stringValue != null)
    //                        {
    //                            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
    //                            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    //                            filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), containsMethod, Expression.Constant(stringValue));
    //                        }
    //                        break;
    //                    case "notcontains":
    //                    case "not_contains":
    //                        if (property.PropertyType == typeof(string) && stringValue != null)
    //                        {
    //                            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
    //                            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    //                            filterExpression = Expression.Not(Expression.Call(Expression.Call(propertyExpression, toLowerMethod), containsMethod, Expression.Constant(stringValue)));
    //                        }
    //                        break;
    //                    case "startswith":
    //                    case "starts_with":
    //                        if (property.PropertyType == typeof(string) && stringValue != null)
    //                        {
    //                            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
    //                            MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
    //                            filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), startsWithMethod, Expression.Constant(stringValue));
    //                        }
    //                        break;
    //                    case "endswith":
    //                    case "ends_with":
    //                        if (property.PropertyType == typeof(string) && stringValue != null)
    //                        {
    //                            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
    //                            MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
    //                            filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), endsWithMethod, Expression.Constant(stringValue));
    //                        }
    //                        break;
    //                    case "lessthan":
    //                    case "less_than":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, decimalValue, dateTimeValue, Expression.LessThan);
    //                        break;
    //                    case "lessthanorequalto":
    //                    case "less_than_or_equal_to":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, decimalValue, dateTimeValue, Expression.LessThanOrEqual);
    //                        break;
    //                    case "greaterthan":
    //                    case "greater_than":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, decimalValue, dateTimeValue, Expression.GreaterThan);
    //                        break;
    //                    case "greaterthanorequalto":
    //                    case "greater_than_or_equal_to":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, decimalValue, dateTimeValue, Expression.GreaterThanOrEqual);
    //                        break;
    //                    case "dateis":
    //                    case "date_is":
    //                        filterExpression = BuildEqualExpression(propertyExpression, property.PropertyType, null, null, null, dateTimeValue);
    //                        break;
    //                    case "dateisnot":
    //                    case "date_is_not":
    //                        filterExpression = Expression.Not(BuildEqualExpression(propertyExpression, property.PropertyType, null, null, null, dateTimeValue));
    //                        break;
    //                    case "datebefore":
    //                    case "date_before":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, null, dateTimeValue, Expression.LessThan);
    //                        break;
    //                    case "dateafter":
    //                    case "date_after":
    //                        filterExpression = BuildComparisonExpression(propertyExpression, property.PropertyType, null, dateTimeValue, Expression.GreaterThan);
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                if (filterExpression != null)
    //                {
    //                    var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
    //                    query = query.Where(lambda);
    //                }
    //            }
    //        }
    //    }

    //    return query;
    //}
    public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, FilterModel filterRequest)
    {
        if (filterRequest?.Filters?.Count > 0)
        {
            foreach (var filter in filterRequest.Filters)
            {
                string field = filter.Key;
                if (field.ToLower() == "contractduration" && typeof(T).Name.ToLower() == "employeecoo")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if ( field.ToLower() == "status"  && typeof(T).Name.ToLower() == "employeecoo")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "employeename" && typeof(T).Name.ToLower() == "employeecoo")
                {
                    continue; // انتقل إلى التكرار التالي في foreach

                }
                if (field.ToLower() == "arabicname" && typeof(T).Name.ToLower() == "employeecoo")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "ponumber" && typeof(T).Name.ToLower() == "coo")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "ponumber" && typeof(T).Name.ToLower() == "salarytran")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }

                if (field.ToLower() == "coonumber" && typeof(T).Name.ToLower() == "salarytran")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "employeename" && typeof(T).Name.ToLower() == "salarytran")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "arabicname" && typeof(T).Name.ToLower() == "salarytran")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "ponumber" && typeof(T).Name.ToLower() == "salarytran")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "ponumber" && typeof(T).Name.ToLower() == "unemp")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                if (field.ToLower() == "coonumber" && typeof(T).Name.ToLower() == "unemp")
                {
                    continue; // انتقل إلى التكرار التالي في foreach
                }
                string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);
                JsonElement filterValue = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                if (filterValue.TryGetProperty("Value", out JsonElement valueElement))
                {
                    if (valueElement.ValueKind == JsonValueKind.Null ||
                        (valueElement.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(valueElement.GetString())))
                    {
                        continue;
                    }
                }

                if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                {
                    string matchMode = matchModeElement.GetString();
                    JsonElement value = filterValue.GetProperty("Value");

                    if (typeof(T).Name.ToLower() == "employeecoo")
                    {
                        // handle custom field mappings
                        switch (field.ToLower())
                        {
                            case "contractstartdate":
                                field = "StartCont";
                                break;
                            case "contractenddate":
                                field = "EndCont";
                                break;
                            case "transportation":
                                field = "Transportation";
                                break;
                            case "salary":
                                field = "Salary";
                                break;
                            case "supervisor":
                                field = "SuperVisor";
                                break;
                            case "areamanager":
                                field = "AreaManager";
                                break;
                            case "laptoptype":
                                field = "LaptopNavigation.Name";
                                break;
                            case "cityname":
                                field = "City.NameEn";
                                break;
                            case "clientname":
                                field = "Client.ClientName";
                                break;
                            case "typeofcontract":
                            case "typeofcontractname":
                                field = "TypeOfContract.NmaeEn";
                                break;
                            case "teamname":
                                field = "Team.TeamName";
                                break;
                            case "coonumber":
                                field = "Coo.CooNumber";
                                break;
                            case "ismobile":
                                field = "IsMobile";
                                break;
                            default:
                                break;
                        }

                    }
                    if (typeof(T).Name.ToLower() == "coo")
                    {
                        // handle custom field mappings
                        switch (field.ToLower())
                        {
                            case "clientname":
                                field = "Client.ClientName";
                                break;
                            case "currencytype":
                                field = "Currency.Type";
                                break;
                           
                        }

                    }
                    if (typeof(T).Name.ToLower() == "salarytran")
                    {
                        // handle custom field mappings
                        switch (field.ToLower())
                        {
                            case "basicsalaryinusd":
                                field = "SalaryUsd";
                                break;
                            case "totalsalarycalculatedinsyrianpounds":
                                field = "Ammount";
                                break;

                        }

                    }
                   

                    var parameter = Expression.Parameter(typeof(T), "entity");
                    Expression propertyExpression = parameter;
                  
                    foreach (var part in field.Split('.'))
                    {
                        propertyExpression = Expression.PropertyOrField(propertyExpression, part);
                    }
                    

                    // نحاول نجيب النوع الحقيقي للخاصية
                    Type propertyType = propertyExpression.Type;

                    Expression filterExpression = null;
                    string stringValue = value.ValueKind == JsonValueKind.String ? value.GetString()?.ToLower() : null;
                    bool? boolValue = value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False ? value.GetBoolean() : (bool?)null;
                    decimal? decimalValue = value.ValueKind == JsonValueKind.Number ? value.GetDecimal() : (decimal?)null;
                    DateOnly? dateOnlyValue = null;
                    DateTime? dateTimeValue = null;
                    if (Nullable.GetUnderlyingType(propertyType) == typeof(DateOnly))
                    {
                        if (value.ValueKind == JsonValueKind.String)
                        {
                            string stringDateValue = value.GetString();

                            if (DateTime.TryParse(stringDateValue, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime utcDateTime))
                            {
                                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);

                                // أخذ التاريخ فقط (بدون الساعة)
                                var dateOnly = DateOnly.FromDateTime(localDateTime);
                            }
                        }
                    }

                    if (Nullable.GetUnderlyingType(propertyType) == typeof(DateTime))
                    {
                        if (value.ValueKind == JsonValueKind.String)
                        {
                            string stringDateValue = value.GetString();

                            if (DateTime.TryParse(stringDateValue, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime utcDateTime))
                            {
                                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);

                                // تحديد الساعة على 00:00 (منتصف الليل) عند إدخال البيانات
                                dateTimeValue = localDateTime.Date;  // استخدام `localDateTime.Date` سيأخذ التاريخ فقط مع الساعة 00:00
                            }
                        }
                    }





                    switch (matchMode?.ToLowerInvariant())
                    {
                        case "equals":
                            filterExpression = BuildEqualExpression(propertyExpression, propertyType, stringValue, boolValue, decimalValue, dateTimeValue, null);
                            break;
                        case "notequals":
                        case "not_equals":
                            filterExpression = Expression.Not(BuildEqualExpression(propertyExpression, propertyType, stringValue, boolValue, decimalValue, dateTimeValue, null));
                            break;
                        case "contains":
                            if (propertyType == typeof(string) && stringValue != null)
                            {
                                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), containsMethod, Expression.Constant(stringValue));
                            }
                            break;
                        case "notcontains":
                        case "not_contains":
                            if (propertyType == typeof(string) && stringValue != null)
                            {
                                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                filterExpression = Expression.Not(Expression.Call(Expression.Call(propertyExpression, toLowerMethod), containsMethod, Expression.Constant(stringValue)));
                            }
                            break;
                        case "startswith":
                        case "starts_with":
                            if (propertyType == typeof(string) && stringValue != null)
                            {
                                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                                var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                                filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), startsWithMethod, Expression.Constant(stringValue));
                            }
                            break;
                        case "endswith":
                        case "ends_with":
                            if (propertyType == typeof(string) && stringValue != null)
                            {
                                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                                var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                                filterExpression = Expression.Call(Expression.Call(propertyExpression, toLowerMethod), endsWithMethod, Expression.Constant(stringValue));
                            }
                            break;
                        case "lessthan":
                        case "lt":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, decimalValue, dateTimeValue, null, Expression.LessThan);
                            break;
                        case "lte":
                        case "less_than_or_equal_to":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, decimalValue, dateTimeValue, null, Expression.LessThanOrEqual);
                            break;
                        case "greaterthan":
                        case "gt":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, decimalValue, dateTimeValue, null, Expression.GreaterThan);
                            break;
                        case "greaterthanorequalto":
                        case "gte":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, decimalValue, dateTimeValue, null, Expression.GreaterThanOrEqual);
                            break;
                        case "dateis":
                        case "date_is":
                            filterExpression = BuildEqualExpression(propertyExpression, propertyType, null, null, null, dateTimeValue, dateOnlyValue);
                            break;
                        case "dateisnot":
                        case "date_is_not":
                            filterExpression = Expression.Not(BuildEqualExpression(propertyExpression, propertyType, null, null, null, dateTimeValue, dateOnlyValue));
                            break;
                        case "datebefore":
                        case "date_before":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, null, dateTimeValue, dateOnlyValue, Expression.LessThan);
                            break;
                        case "dateafter":
                        case "date_after":
                            filterExpression = BuildComparisonExpression(propertyExpression, propertyType, null, dateTimeValue, dateOnlyValue, Expression.GreaterThan);
                            break;
                        default:
                            break;
                    }

                    if (filterExpression != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                        query = query.Where(lambda);
                    }
                }
            }
        }

        // Handle Sorting
        if (filterRequest?.MultiSortMeta?.Any() == true)
        {
            IOrderedQueryable<T> orderedQuery = null;

            foreach (var sort in filterRequest.MultiSortMeta)
            {
                var field = sort.Field;
                var sortOrder = sort.Order;

                if (typeof(T).Name.ToLower() == "employeecoo")
                {
                    // handle custom field mappings
                    switch (field.ToLower())
                    {
                        case "contractstartdate":
                            field = "StartCont";
                            break;
                        case "contractenddate":
                            field = "EndCont";
                            break;
                        case "transportation":
                            field = "Transportation";
                            break;
                        case "salary":
                            field = "Salary";
                            break;
                        case "supervisor":
                            field = "SuperVisor";
                            break;
                        case "areamanager":
                            field = "AreaManager";
                            break;
                        case "laptop":
                            field = "LaptopNavigation.Name";
                            break;
                        case "cityname":
                            field = "City.NameEn";
                            break;
                        case "clientname":
                            field = "Client.ClientName";
                            break;
                        case "typeofcontract":
                        case "typeofcontractname":
                            field = "TypeOfContract.NmaeEn";
                            break;
                        case "teamname":
                            field = "Team.TeamName";
                            break;
                        case "coonumber":
                            field = "Coo.CooNumber";
                            break;
                        default:
                            break;
                    }

                }
                if (typeof(T).Name.ToLower() == "coo")
                {
                    // handle custom field mappings
                    switch (field.ToLower())
                    {
                        case "clientname":
                            field = "Client.ClientName";
                            break;
                        case "currencytype":
                            field = "Currency.Type";
                            break;

                    }

                }
                if (typeof(T).Name.ToLower() == "salarytran")
                {
                    // handle custom field mappings
                    switch (field.ToLower())
                    {
                        case "BasicSalaryinUSD":
                            field = "SalaryUsd";
                            break;
                        case "TotalSalaryCalculatedinSyrianPounds":
                            field = "Ammount";
                            break;

                    }

                }
                var parameter = Expression.Parameter(typeof(T), "entity");
                Expression propertyExpression = parameter;

                foreach (var part in field.Split('.'))
                {
                    var typeName = typeof(T).Name.ToLower();
                    var lowerPart = part.ToLower();

                    // تجاوز الحقول غير المرغوبة حسب نوع الكلاس
                    if (typeName == "employeecoo" && (lowerPart == "contractduration" || lowerPart == "status" || lowerPart == "employeename" || lowerPart == "arabicname"))
                        continue;
                    if (typeName == "coo" && lowerPart == "ponumber")
                        continue;
                    if (typeName == "salarytran" && (lowerPart == "coonumber" || lowerPart == "ponumber" || lowerPart == "employeename" || lowerPart == "arabicname"))
                        continue;
                    if (typeName == "unemp" && lowerPart == "ponumber")
                        continue;
                    if (typeName == "unemp" && lowerPart == "coonumber")
                        continue;
                    propertyExpression = Expression.PropertyOrField(propertyExpression, part);
                }



                var lambda = Expression.Lambda(propertyExpression, parameter);
                var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(T), propertyExpression.Type }, query.Expression, lambda);

                orderedQuery = orderedQuery == null
                    ? (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(orderByExpression)
                    : (IOrderedQueryable<T>)orderedQuery.Provider.CreateQuery<T>(orderByExpression);
            }

            query = orderedQuery ?? query;
        }

        return query;
    }

    private static Expression BuildEqualExpression(Expression propertyExpression, Type propertyType,
     string stringValue, bool? boolValue, decimal? decimalValue, DateTime? dateTimeValue, DateOnly? dateOnlyValue)
    {
        if (propertyType == typeof(string))
        {
            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var propertyToLower = Expression.Call(propertyExpression, toLowerMethod);
            return Expression.Equal(propertyToLower, Expression.Constant(stringValue));
        }
        else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(boolValue, propertyType));
        }
        else if (propertyType == typeof(int) || propertyType == typeof(int?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(decimalValue.HasValue ? (int?)decimalValue.Value : null, propertyType));
        }
        else if (propertyType == typeof(float) || propertyType == typeof(float?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(decimalValue.HasValue ? (float?)decimalValue.Value : null, propertyType));
        }
        else if (propertyType == typeof(double) || propertyType == typeof(double?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(decimalValue.HasValue ? (double?)decimalValue.Value : null, propertyType));
        }
        else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(decimalValue, propertyType));
        }
        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            return Expression.Equal(propertyExpression, Expression.Constant(dateTimeValue, propertyType));
        }
        else if (propertyType == typeof(DateOnly) || propertyType == typeof(DateOnly?))
        {
            return Expression.Equal(propertyExpression,
                Expression.Constant(dateOnlyValue.HasValue ? dateOnlyValue.Value : null, propertyType));
        }

        return null;
    }


    private static Expression BuildComparisonExpression(Expression propertyExpression, Type propertyType,
        decimal? decimalValue, DateTime? dateTimeValue, DateOnly? dateOnlyValue, Func<Expression, Expression, BinaryExpression> comparisonFunc)
    {
        if (decimalValue.HasValue)
        {
            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                return comparisonFunc(propertyExpression,
                    Expression.Constant((int)decimalValue.Value, propertyType));
            }
            else if (propertyType == typeof(float) || propertyType == typeof(float?))
            {
                return comparisonFunc(propertyExpression,
                    Expression.Constant((float)decimalValue.Value, propertyType));
            }
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return comparisonFunc(propertyExpression,
                    Expression.Constant((double)decimalValue.Value, propertyType));
            }
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return comparisonFunc(propertyExpression,
                    Expression.Constant(decimalValue, propertyType));
            }
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            return comparisonFunc(propertyExpression,
                Expression.Constant(dateTimeValue, propertyType));
        }

        if (propertyType == typeof(DateOnly) || propertyType == typeof(DateOnly?))
        {
            return comparisonFunc(propertyExpression,
                Expression.Constant(dateOnlyValue.HasValue ? dateOnlyValue.Value : null, propertyType));
        }

        return null;
    }







}
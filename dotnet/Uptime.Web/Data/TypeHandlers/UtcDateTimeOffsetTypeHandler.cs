using System.Data;
using Dapper;
using Uptime.Web.Data.TypeHandlers;

[module: TypeHandler(typeof(DateTimeOffset), typeof(UtcDateTimeOffsetHandler))]

namespace Uptime.Web.Data.TypeHandlers;

public class UtcDateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.ToUniversalTime().ToString("O");
        parameter.DbType = DbType.String;
    }

    public override DateTimeOffset Parse(object value)
    {
        return value switch
        {
            string s => DateTimeOffset.Parse(s).ToUniversalTime(),
            DateTime dt => new DateTimeOffset(dt, TimeSpan.Zero).ToUniversalTime(),
            _ => throw new DataException(
                $"Cannot convert {value?.GetType().Name} to DateTimeOffset."
            ),
        };
    }
}

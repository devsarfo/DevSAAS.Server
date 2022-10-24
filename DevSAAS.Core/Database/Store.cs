using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Npgsql.NameTranslation;

namespace DevSAAS.Core.Database;

public abstract class Store<TRecord> : Store where TRecord : class
{
    private readonly string _tableName;

    protected Store(IDbConnection connection, string tableName)
    {
        this.Connection = connection;
        this._tableName = tableName;
    }

    public IDbConnection Connection { get; }

    public string TableName
    {
        get
        {
            if (IsSqLiteDb && _tableName.Contains("."))
            {
                return _tableName.Split(".")[1];
            }

            return _tableName;
        }
    }

    private bool IsSqLiteDb => Connection.ConnectionString.Contains("Data Source=:memory:");

    public StatementBuilder<TRecord> StatementBuilder()
    {
        return new();
    }

    public async Task<TRecord?> GetAsync(string id)
    {
        var predicate = new Dictionary<string, object> { ["id"] = id };
        var result = await this.GetAsync(predicate);
        return result.FirstOrDefault();
    }

    public Task<IEnumerable<TRecord>> GetAsync(IDictionary<string, object>? predicate = null)
    {
        var statement = this.BuildSelectStatement(predicate);
        return this.Connection.QueryAsync<TRecord>(statement, predicate);
    }

    public Task<IEnumerable<TRecord>> GetAsync(IEnumerable<string> ids)
    {
        string statement = $"select * from {this._tableName} where id = any(@_ids)";
        return this.Connection.QueryAsync<TRecord>(statement, new { _ids = ids });
    }

    public async Task<T?> GetAsync<T>(string id) where T : TRecord
    {
        var predicate = new Dictionary<string, object> { ["id"] = id };
        var result = await this.GetAsync<T>(predicate);
        return result.FirstOrDefault();
    }

    public Task<IEnumerable<T>> GetAsync<T>(IDictionary<string, object>? predicate = null) where T : TRecord
    {
        var statement = this.BuildSelectStatement(predicate);
        return this.Connection.QueryAsync<T>(statement, predicate);
    }

    public virtual Task<int> InsertAsync(TRecord row, InsertConflictHandler? onConflict = null)
    {
        return this.InsertAsync(new[] { row }, onConflict);
    }

    public virtual async Task<int> InsertAsync(IEnumerable<TRecord> rows, InsertConflictHandler? onConflict = null)
    {
        var statement = this.BuildInsertStatement(onConflict);
        var result = await this.Connection.ExecuteAsync(statement, rows);
        return result;
    }

    public virtual Task<int> UpdateAsync(TRecord row, params string[] fieldsToUpdate)
    {
        return this.UpdateAsync(new[] { row }, fieldsToUpdate);
    }

    public virtual async Task<int> UpdateAsync(IEnumerable<TRecord> rows, params string[] fieldsToUpdate)
    {
        var statement = this.BuildUpdateStatement(fieldsToUpdate);
        var result = await this.Connection.ExecuteAsync(statement, rows);
        return result;
    }

    public virtual Task<int> DeleteAsync(string id, bool confirm = false)
    {
        var param = new { _id = id, _confirm = confirm };
        return confirm
            ? this.Connection.ExecuteAsync($"delete from {this.TableName} where @_confirm and id=@_id", param)
            : Task.FromResult(0);
    }

    private string BuildSelectStatement(IDictionary<string, object>? predicate = null)
    {
        var cacheKey = $"select_{this.TableName}";

        if (predicate is not null && predicate.Count > 0)
        {
            cacheKey += '_' + string.Join('_', predicate.Select(x => x.Key.ToLowerInvariant()).ToArray());
        }

        // if (StoreCache.Statements.ContainsKey(cacheKey)) return StoreCache.Statements[cacheKey];

        var props = typeof(TRecord).GetProperties();
        var fields = new StringBuilder();

        for (var i = 0; i < props.Length; i++)
        {
            // fields.Append(NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(props[i].Name) + " as \"" + props[i].Name + "\"" + separator);
            fields.Append(NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(props[i].Name));
            if (i < props.Length - 1) fields.Append(',');
        }

        var strWhere = new StringBuilder();
        if (predicate is not null && predicate.Count > 0)
        {
            var i = 0;
            foreach (var (key, _) in predicate)
            {
                if (i == 0) strWhere.Append(" where ");
                if (i > 0 && i < predicate.Count) strWhere.Append(" and ");
                strWhere.Append($"{key}=@{key}");
                i++;
            }
        }

        var result = $"select {fields} from {this.TableName}{strWhere}";
        // StoreCache.Statements.Add(cacheKey, result);

        return result;
    }

    private string BuildInsertStatement(InsertConflictHandler? onConflict)
    {
        // string key = $"insert_{this.TableName}";
        // if (onConflict is not null) key = $"upsert_{this.TableName}_{onConflict.Constraint}";
        // if (StoreCache.Statements.ContainsKey(key)) return StoreCache.Statements[key];

        var separator = ",";
        var props = typeof(TRecord).GetProperties()
            .Where(x => x.PropertyType != typeof(SerialInt32) &&
                        x.PropertyType != typeof(SerialInt64))
            .ToArray();

        var sqlProps = new StringBuilder();
        var clrProps = new StringBuilder();
        var upsertProps = new StringBuilder();

        for (var i = 0; i < props.Length; i++)
        {
            if (i == props.Length - 1) separator = "";
            var snakeCaseName = NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(props[i].Name);
            sqlProps.Append($"{snakeCaseName}{separator}");
            clrProps.Append($"@{props[i].Name}{separator}");

            switch (onConflict)
            {
                case UpsertOnConflictHandler upsert when upsert.IgnoreFields.Contains(snakeCaseName):
                    continue;
                case UpsertOnConflictHandler { UpdateFields: { } } upsert
                    when !upsert.UpdateFields!.Contains(snakeCaseName):
                    continue;
                case UpsertOnConflictHandler:
                {
                    if (upsertProps.Length > 0) upsertProps.Append(separator);
                    upsertProps.Append($"{snakeCaseName}=@{props[i].Name}");
                    break;
                }
                case DeleteOnConflictHandler:
                    break;
            }
        }

        var strBuilder = new StringBuilder($"insert into {this.TableName}({sqlProps}) values({clrProps})");
        if (onConflict is not null)
        {
            if (upsertProps.Length > 0 && !IsSqLiteDb)
            {
                strBuilder.Append(onConflict.ConflictType == ConflictType.Constraint
                    ? $" on conflict on constraint {onConflict.Constraint} "
                    : $" on conflict({onConflict.Constraint}) ");

                strBuilder.Append($"do update set {upsertProps};");
            }
            else if (onConflict is IgnoreOnConflictHandler)
            {
                strBuilder.Append($" on conflict do nothing");
            }
        }

        var result = strBuilder.ToString();
        // StoreCache.Statements.Add(key, result);
        return result;
    }

    private string BuildUpdateStatement(string[]? fieldsToUpdate = null)
    {
        var key = $"update_{this.TableName}";

        if (fieldsToUpdate is not null && fieldsToUpdate.Length > 0)
        {
            key += '_' + string.Join('_', fieldsToUpdate.Select(x => x.ToLowerInvariant()).ToArray());
        }

        // if (StoreCache.Statements.ContainsKey(key)) return StoreCache.Statements[key];

        var separator = ',';
        if (fieldsToUpdate is null || fieldsToUpdate.Length <= 0)
        {
            fieldsToUpdate = typeof(TRecord)
                .GetProperties()
                .Where(x => !x.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase) ||
                            !x.Name.Equals("createdAt", StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.PropertyType != typeof(SerialInt32) &&
                            x.PropertyType != typeof(SerialInt64))
                .Select(x => x.Name)
                .ToArray();
        }

        var strProps = new StringBuilder();
        for (var i = 0; i < fieldsToUpdate.Length; i++)
        {
            if (i == fieldsToUpdate.Length - 1) separator = ' ';
            strProps.Append(NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(fieldsToUpdate[i]) + '=' + '@' +
                            fieldsToUpdate[i] + separator);
        }

        var result = $"update {this.TableName} set {strProps} where id=@Id";
        // StoreCache.Statements.Add(key, result);

        return result;
    }
}

public abstract class InsertConflictHandler
{
    protected InsertConflictHandler(ConflictType conflictType, string constraint = "id")
    {
        this.ConflictType = conflictType;
        this.Constraint = constraint;
    }

    public ConflictType ConflictType { get; }

    public string Constraint { get; }
}

public sealed class IgnoreOnConflictHandler : InsertConflictHandler
{
    public IgnoreOnConflictHandler(ConflictType conflictType, string constraint = "id") : base(conflictType, constraint)
    {
    }
}

public sealed class UpsertOnConflictHandler : InsertConflictHandler
{
    public UpsertOnConflictHandler(ConflictType conflictType, string constraint = "id",
        IEnumerable<string>? updateFields = null, IEnumerable<string>? ignoreFields = null) : base(conflictType,
        constraint)
    {
        this.UpdateFields = updateFields;
        this.IgnoreFields = ignoreFields ?? new[] { "id", "created_at" };
    }

    public IEnumerable<string>? UpdateFields { get; }

    public IEnumerable<string> IgnoreFields { get; }
}

public sealed class DeleteOnConflictHandler : InsertConflictHandler
{
    public DeleteOnConflictHandler(ConflictType conflictType, string constraint = "id") : base(conflictType, constraint)
    {
    }
}

public enum ConflictType
{
    Column = 1,
    Constraint = 2
}

public abstract class Store
{
    public static T1 Get<T1>(IDbConnection connection)
        where T1 : Store
    {
        var inst = Activator.CreateInstance(typeof(T1), connection);
        var result = inst as T1;
        return result!;
    }

    public static (T1, T2) Get<T1, T2>(IDbConnection connection)
        where T1 : Store
        where T2 : Store
    {
        var s1 = (T1)Activator.CreateInstance(typeof(T1), connection)!;
        var s2 = (T2)Activator.CreateInstance(typeof(T2), connection)!;
        return (s1, s2);
    }

    public static (T1, T2, T3) Get<T1, T2, T3>(IDbConnection connection)
        where T1 : Store
        where T2 : Store
        where T3 : Store
    {
        var s1 = (T1)Activator.CreateInstance(typeof(T1), connection)!;
        var s2 = (T2)Activator.CreateInstance(typeof(T2), connection)!;
        var s3 = (T3)Activator.CreateInstance(typeof(T3), connection)!;
        return (s1, s2, s3);
    }

    public static (T1, T2, T3, T4) Get<T1, T2, T3, T4>(IDbConnection connection)
        where T1 : Store
        where T2 : Store
        where T3 : Store
        where T4 : Store
    {
        var s1 = (T1)Activator.CreateInstance(typeof(T1), connection)!;
        var s2 = (T2)Activator.CreateInstance(typeof(T2), connection)!;
        var s3 = (T3)Activator.CreateInstance(typeof(T3), connection)!;
        var s4 = (T4)Activator.CreateInstance(typeof(T4), connection)!;
        return (s1, s2, s3, s4);
    }

    public static (T1, T2, T3, T4, T5) Get<T1, T2, T3, T4, T5>(IDbConnection connection)
        where T1 : Store
        where T2 : Store
        where T3 : Store
        where T4 : Store
        where T5 : Store
    {
        var s1 = (T1)Activator.CreateInstance(typeof(T1), connection)!;
        var s2 = (T2)Activator.CreateInstance(typeof(T2), connection)!;
        var s3 = (T3)Activator.CreateInstance(typeof(T3), connection)!;
        var s4 = (T4)Activator.CreateInstance(typeof(T4), connection)!;
        var s5 = (T5)Activator.CreateInstance(typeof(T5), connection)!;
        return (s1, s2, s3, s4, s5);
    }

    public sealed class StatementBuilder<T>
    {
        private readonly IDictionary<string, object?> _predicates = new Dictionary<string, object?>();

        public StatementBuilder<T> Where<TVal>(Expression<Func<T, TVal>> expression, TVal value)
        {
            if (expression.Body is not MemberExpression memberExpr)
            {
                throw new ArgumentException($"Expression '{expression}' refers to a method, not a property");
            }

            if (memberExpr.Member is not PropertyInfo propInfo)
            {
                throw new ArgumentException($"Expression '{expression}' refers to a field, not a property");
            }

            var propName = propInfo.Name;
            _predicates[propName] = value;
            return this;
        }

        public FormattableString Build()
        {
            var str = new StringBuilder();

            var i = 0;
            foreach (var (key, _) in _predicates)
            {
                switch (i)
                {
                    case 0:
                        str.Append(" where ");
                        break;
                    case > 0 when i < _predicates.Count:
                        str.Append(" and ");
                        break;
                }

                str.Append($"{key}=@{key}");
                i++;
            }

            return $"{str}";
        }
    }
}
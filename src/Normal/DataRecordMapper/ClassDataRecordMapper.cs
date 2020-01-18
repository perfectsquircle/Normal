using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using FastMember;

namespace Normal
{
    internal class ClassDataRecordMapper : IDataRecordMapper
    {
        private IEnumerable<PropertyMapper> _propertyMappers;
        private Type _targetType;
        private readonly Lazy<TypeAccessor> _typeAccessor;
        private TypeAccessor TypeAccessor => _typeAccessor.Value;


        public ClassDataRecordMapper(Type targetType)
        {
            _targetType = targetType;
            _typeAccessor = new Lazy<TypeAccessor>(() => TypeAccessor.Create(_targetType));
        }

        public object MapDataRecord(IDataRecord dataRecord)
        {
            if (_propertyMappers == null)
            {
                _propertyMappers = CreatePropertyMappers(dataRecord).Buffered();
            }

            var instance = TypeAccessor.CreateNew();
            foreach (var mapper in _propertyMappers)
            {
                TypeAccessor[instance, mapper.PropertyName] = mapper.MapProperty(dataRecord);
            }
            return instance;
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }

        private IEnumerable<PropertyMapper> CreatePropertyMappers(IDataRecord dataRecord)
        {
            var list = new List<PropertyMapper>();
            var members = TypeAccessor
                .GetMembers()
                .Where(m => m.CanWrite)
                .ToList();

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var columnName = dataRecord.GetName(i);
                var columnNameVariants = GetVariants(columnName);
                var member = members.FirstOrDefault(m =>
                {
                    var columnNameAttribute = m.GetAttribute(typeof(ColumnAttribute), false) as ColumnAttribute;
                    if (columnNameAttribute != null)
                    {
                        return columnNameAttribute.Name == columnName;
                    }
                    var propertyVariants = GetVariants(m.Name);
                    return propertyVariants.Intersect(columnNameVariants).Any();
                });
                if (member == default(Member))
                {
                    continue;
                }
                members.Remove(member);
                yield return new PropertyMapper()
                    .WithColumnIndex(i)
                    .WithColumnType(dataRecord.GetFieldType(i))
                    .WithPropertyName(member.Name)
                    .WithPropertyType(member.Type);
            }
        }
    }
}
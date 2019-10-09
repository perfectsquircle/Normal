using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastMember;

namespace Normal
{
    internal class ClassDataRecordMapper : IDataRecordMapper
    {
        private IList<PropertyMapper> _propertyMappers;
        private TypeAccessor _typeAccessor;

        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            if (_typeAccessor == null)
            {
                _typeAccessor = TypeAccessor.Create(typeof(T));
            }
            if (_propertyMappers == null)
            {
                _propertyMappers = CreatePropertyMappers(dataRecord);
            }

            var instance = _typeAccessor.CreateNew();
            foreach (var mapper in _propertyMappers)
            {
                var fieldValue = mapper.MapProperty(dataRecord);
                _typeAccessor[instance, mapper.PropertyName] = fieldValue;
            }
            return (T)instance;
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }

        private IList<PropertyMapper> CreatePropertyMappers(IDataRecord dataRecord)
        {
            var list = new List<PropertyMapper>();
            var members = _typeAccessor
                .GetMembers()
                .Where(m => m.CanWrite);

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var columnName = dataRecord.GetName(i);
                var columnNameVariants = GetVariants(columnName);
                var member = members.FirstOrDefault(m =>
                {
                    var propertyVariants = GetVariants(m.Name);
                    return propertyVariants.Intersect(columnNameVariants).Any();
                });
                if (member == default(Member))
                {
                    continue;
                }
                list.Add(new PropertyMapper()
                    .WithColumnIndex(i)
                    .WithPropertyName(member.Name)
                    .WithPropertyType(member.Type));
            }
            return list;
        }
    }
}
namespace ValueType.Extensions.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using ValueType.Extensions.Numbers;
    using ValueType.Extensions.Types;

    public static class CollectionExtensions
    {
        public static void AddIfNotContains<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// A method to parse a collection to a DataTable
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection that is being passed in</typeparam>
        /// <param name="list">The collection itself that you want to create into a datatable</param>
        /// <param name="dateTimeFormat">The format that you want your datetimes to be</param>
        /// <param name="evaluateEnumsAsString"></param>
        /// <param name="numberOfDecimalPlace">The number of decimal places that you would like numbers formatting to</param>
        /// <param name="ignoreNamedProperties">A boolean stating if the list of named properties are to the only properties included or a collection to exclude</param>
        /// <param name="namedProperties">A comma separated list of properties that either need to be ignored or included</param>
        /// <returns>A datatable of the collection that is passed in that takes into account the various other parameters</returns>
        public static DataTable ToDataTable<T>(this List<T> list, string dateTimeFormat = "dd/MM/yyyy", bool evaluateEnumsAsString = true,
            int numberOfDecimalPlace = 2, bool ignoreNamedProperties = true, params string[] namedProperties)
        {
            var objectType = list.FirstOrDefault()?.GetType();
            var datatable = new DataTable(typeof(T).Name);
            if (objectType != null)
            {
                var objectTypeProperties = objectType.GetProperties();
                foreach (var property in objectTypeProperties)
                {
                    // This takes the property name and separates it out so that there is space in the header
                    var propertyName = property.Name;
                    if (namedProperties.Contains(property.Name) != ignoreNamedProperties)
                    {
                        var propertyType = property.PropertyType;
                        // Only add to the datatable if it is not a form of collection or is a string (as string is a collection)
                        if (!property.IsCollection() || propertyType.IsString())
                        {
                            Type typeToUse;

                            // If it is an enum then set it to be a string as otherwise it exports the underlying int value
                            if (propertyType.IsEnum())
                            {
                                typeToUse = evaluateEnumsAsString ? typeof(string) : typeof(int);
                            }
                            // If it isn't an Enum, but is nullable and isn't a string, then set it to the underlying system type, this causes issues with ints etc
                            else if (propertyType.IsNullable() && !propertyType.IsString())
                            {
                                typeToUse = Nullable.GetUnderlyingType(propertyType);
                            }
                            // If neither of the above are true then it is possible to assign it the property type
                            else
                            {
                                typeToUse = propertyType;
                            }
                            datatable.Columns.Add(propertyName, typeToUse);
                            for (var i = 0; i < list.Count; i++)
                            {
                                var row = datatable.NewRow();

                                // If there is a pre-existing row for this entry then assign the row to this pre-existing row so that there are not too many rows
                                if (datatable.Rows.Count > i)
                                {
                                    row = datatable.Rows[i];
                                }
                                // If this is the first time the row has been created then add it
                                else
                                {
                                    datatable.Rows.Add(row);
                                }

                                // If the property is an enum and isn't null then convert it to its enum type and gets its description string
                                if (propertyType.IsEnum() && evaluateEnumsAsString && property.GetValue(list[i]) != null)
                                {
                                    var enumType = propertyType.IsNullable()
                                        ? Nullable.GetUnderlyingType(propertyType)
                                        : propertyType;
                                    var enumValue = Enum.ToObject(enumType, property.GetValue(list[i]));
                                    row[propertyName] = enumValue.GetType()
                                        .GetField(enumValue.ToString())
                                        ?.GetDataAnnotationDisplayName();
                                }
                                // However, if it is a datetime then output it to datetime, using the format in the method signature
                                else if (propertyType == typeof(DateTime))
                                {
                                    var dt = (DateTime)property.GetValue(list[i]);
                                    row[propertyName] = dt.ToString(dateTimeFormat);
                                }
                                // Otherwise, if the property is nullable either set it as the value, or if that is null then set it to DBNull as normal null gives an exception on output
                                else if (propertyType.IsNullable())
                                {
                                    row[propertyName] = property.GetValue(list[i]) ?? DBNull.Value;
                                }
                                else
                                {
                                    row[propertyName] = property.GetValue(list[i]);
                                }
                            }
                        }
                    }
                }
            }

            // Go through every cell and if it is a number then round it to the number of decimal places that are wanted
            foreach (DataRow dataRow in datatable.Rows)
            {
                foreach (
                    DataColumn column in
                    datatable.Columns.Cast<DataColumn>().Where(column => column.DataType.IsNumber()))
                {
                    dataRow[column] = dataRow[column] == DBNull.Value
                        ? dataRow[column]
                        : dataRow[column].GetRoundedValue(column.DataType, numberOfDecimalPlace);
                }
            }
            return datatable;
        }
    }
}

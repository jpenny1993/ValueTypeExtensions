namespace ValueType.Extensions.IO
{
    using System.IO;
    using System.Runtime.Serialization;

    public static class GenericExtensions
    {
        /// <summary>
        /// Performs a deep clone on an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type which we are cloneing</typeparam>
        /// <param name="obj">The object to clone</param>
        /// <returns>A deep clone of the object</returns>
        public static TEntity DeepClone<TEntity>(this TEntity obj)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(TEntity));

            using (var memoryStream = new MemoryStream())
            {
                dataContractSerializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return (TEntity)dataContractSerializer.ReadObject(memoryStream);
            }
        }
    }
}

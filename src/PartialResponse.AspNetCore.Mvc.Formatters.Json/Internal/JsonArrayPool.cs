// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Buffers;
using Newtonsoft.Json;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal
{
    /// <summary>
    /// Provides a resource pool that enables reusing instances of type T[].
    /// </summary>
    /// <typeparam name="T">The type of the objects that are in the resource pool.</typeparam>
    public class JsonArrayPool<T> : IArrayPool<T>
    {
        private readonly ArrayPool<T> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArrayPool{T}"/> class.
        /// </summary>
        /// <param name="inner">The inner resource pool.</param>
        public JsonArrayPool(ArrayPool<T> inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            this.inner = inner;
        }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array.</param>
        /// <returns>An array of type T[] that is at least minimumLength in length.</returns>
        public T[] Rent(int minimumLength)
        {
            return this.inner.Rent(minimumLength);
        }

        /// <summary>
        /// Returns an array to the pool that was previously obtained using the Rent method on the same
        /// <see cref="ArrayPool{T}"/> instance.
        /// </summary>
        /// <param name="array">A buffer to return to the pool that was previously obtained using the Rent method.</param>
        public void Return(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            this.inner.Return(array);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace XenaExchange.Client.Messages
{
    internal static class Validator
    {
        public static void NotNullOrEmpty(string paramName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
        }

        public static void NotNull<T>(string paramName, T value)
        {
            if (value == null)
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null");
        }

        public static void GrThanOrEq<T>(string paramName, T value, T threshold)
            where T : IComparable
        {
            if (value.CompareTo(threshold) < 0)
                throw new ArgumentException($"{paramName} should be greater or equal to {threshold.ToString()}", paramName);
        }

        public static void OneOf<T>(string paramName, T value, IEnumerable<T> possible)
            where T : IComparable
        {
            if (possible.Any(p => value.Equals(p)))
                return;
                
            var joined = string.Join(",", possible);
            throw new ArgumentException($"{paramName} should be one of {{{joined}}}", paramName);
        }
    }
}
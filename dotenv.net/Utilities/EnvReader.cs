using System;
using System.ComponentModel;
using dotenv.net.Interfaces;

namespace dotenv.net.Utilities
{
    public class EnvReader : IEnvReader
    {
        /// <summary>
        /// Retrieve a value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A string representing the value if it exists or null</returns>
        public string GetValue(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        /// <summary>
        /// Retrieve a typed value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>Returns a value of the specified type if it exists or the type default</returns>
        public T GetValue<T>(string key) where T : struct
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(retrievedValue))
            {
                return default(T);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T) converter.ConvertFromString(retrievedValue);
        }

        /// <summary>
        /// Try to retrieve a value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The string value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
        public bool TryGetValue(string key, out string value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                value = retrievedValue;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try to retrieve a value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The typed value retrieved or the type default</param>
        /// <returns>A value representing the retrieval success status</returns>
        public bool TryGetValue<T>(string key, out T value) where T : struct
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                // NOTE: this had to be added because the typeconverter does not convert strings to bool
                if (typeof(T) == typeof(bool))
                {
                    if (bool.TryParse(retrievedValue, out var boolValue))
                    {
                        value = (T) (object) boolValue;
                        return true;
                    }
                }
                else if (converter.CanConvertTo(typeof(T)))
                {
                    retrievedValue = retrievedValue.Trim();
                    value = (T) converter.ConvertFromString(retrievedValue);
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}
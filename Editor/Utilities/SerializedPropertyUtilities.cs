// Sourced from: https://github.com/dbrizov/NaughtyAttributes

// MIT License
// 
// Copyright (c) 2017 Denis Rizov
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace TNRD.Utilities
{
    public static class SerializedPropertyUtilities
    {
        /// <summary>
        /// Gets the value of a serialized property
        /// </summary>
        /// <param name="property">The property to get the value from</param>
        public static object GetValue(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object targetObject = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("[", StringComparison.OrdinalIgnoreCase));
                    int index = Convert.ToInt32(element
                        .Substring(element.IndexOf("[", StringComparison.OrdinalIgnoreCase))
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty));
                    targetObject = GetValue(targetObject, elementName, index);
                }
                else
                {
                    targetObject = GetValue(targetObject, element);
                }
            }

            return targetObject;
        }

        private static object GetValue(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null)
                return null;

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null)
                return null;

            const BindingFlags fieldFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags propertyFlags = fieldFlags | BindingFlags.IgnoreCase;

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, fieldFlags);
                if (field != null)
                    return field.GetValue(source);

                PropertyInfo property = type.GetProperty(name, propertyFlags);
                if (property != null)
                    return property.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }
    }
}

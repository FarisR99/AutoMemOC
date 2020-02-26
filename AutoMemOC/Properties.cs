using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AutoMemOC
{
    /**
     * This is a modified version of code found at:
     * https://stackoverflow.com/a/7696370/1442718
     **/
     /// <summary>
     /// This class represents properties and can be used for configurations.
     /// </summary>
    public class JProperties
    {
        private Dictionary<string, string> list;
        private string filename;

        public JProperties(string file)
        {
            reload(file);
        }

        public string get(string field, string defValue)
        {
            string value = get(field);
            return value != null ? value : defValue;
        }

        public string get(string field)
        {
            return list.ContainsKey(field) ? list[field] : null;
        }

        public bool getBool(string field)
        {
            string value = get(field);
            if (value == null) return false;
            return value.Equals("True");
        }

        public int getInt(string field)
        {
            return getInt(field, -1);
        }

        public int getInt(string field, int defValue)
        { 
            string value = get(field);
            if (value == null) return defValue;
            int parsedValue;
            if (int.TryParse(value, out parsedValue))
            {
                return parsedValue;
            } else
            {
                return defValue;
            }
        }

        public string[] getStringArray(string field)
        {
            string value = get(field);
            if (value != null)
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    return value.Substring(1, value.Length - 2).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public StringCollection getStringCollection(string field)
        {
            string value = get(field);
            if (value != null)
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    StringCollection collection = new StringCollection();
                    foreach (string str in value.Substring(1, value.Length - 2).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        collection.Add(str);
                    }
                    return collection;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void set(string field, object value)
        {
            string valueAsString = ObjectToString(value);
            if (!list.ContainsKey(field)) list.Add(field, valueAsString);
            else list[field] = valueAsString;
        }

        public void save()
        {
            save(this.filename);
        }

        public void save(string filename)
        {
            this.filename = filename;

            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

            foreach (string prop in list.Keys.ToArray())
                if (!string.IsNullOrWhiteSpace(list[prop]))
                    file.WriteLine(prop + "=" + list[prop]);

            file.Close();
        }

        public void reload()
        {
            reload(this.filename);
        }

        public void reload(string filename)
        {
            this.filename = filename;
            list = new Dictionary<string, string>();

            if (System.IO.File.Exists(filename))
            {
                loadFromFile(filename);
            }
            else
            {
                var stream = System.IO.File.Create(filename);
                stream.Close();
            }
        }

        private void loadFromFile(string file)
        {
            foreach (string line in System.IO.File.ReadAllLines(file))
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    line.Contains('='))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        list.Add(key, value);
                    }
                    catch { }
                }
            }
        }

        public static string ObjectToString(object obj)
        {
            if (obj == null) return null;
            if (obj is object[] valueArray)
            {
                return EnumerableToString(valueArray);
            } else if (obj is List<object> valueList)
            {
                return EnumerableToString(valueList);
            } else if (obj is StringCollection valueStringCollection)
            {
                string valueAsString = "[";
                foreach (string valueEntry in valueStringCollection)
                {
                    valueAsString += ObjectToString(valueEntry) + ", ";
                }
                if (valueAsString.Length > 1)
                {
                    valueAsString = valueAsString.Substring(0, valueAsString.Length - 2);
                }
                valueAsString += "]";
                return valueAsString;
            } else
            {
                return obj.ToString();
            }
        }

        private static string EnumerableToString<T>(IEnumerable<T> enumerable)
        {
            StringBuilder valueAsString = new StringBuilder("[");
            foreach (T valueEntry in enumerable)
            {
                valueAsString.Append(ObjectToString(valueEntry)).Append(", ");
            }
            if (valueAsString.Length > 1)
            {
                valueAsString.Remove(valueAsString.Length - 2, 2);
            }
            valueAsString.Append(']');
            return valueAsString.ToString();
        }
    }
}

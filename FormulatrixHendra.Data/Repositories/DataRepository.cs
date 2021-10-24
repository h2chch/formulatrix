using FormulatrixHendra.Data.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormulatrixHendra.Data.Repositories
{
    public class DataRepository
    {
        private static IDictionary<string, Content> contents;
        private static object contentLock;

        static DataRepository()
        {
            Initialize();
        }

        static void Initialize()
        {
            contents = new Dictionary<string, Content>();
            contentLock = new object();
        }

        public void Register(string itemName, string itemContent, int itemType)
        {
            var isValid = false;
            if (itemType == 1)
            {
                try
                {
                    var json = JToken.Parse(itemContent);
                    isValid = true;
                }
                catch { }
            }
            else
            {
                try
                {
                    var xml = XDocument.Parse(itemContent);
                    isValid = true;
                }
                catch { }
            }

            if (!isValid) return;

            if (!contents.ContainsKey(itemName))
            {
                lock (contentLock)
                {
                    if (!contents.ContainsKey(itemName))
                    {
                        contents[itemName] = new Content { ItemContent = itemContent, Type = itemType };
                    }
                }
            }
        }

        public string Retrieve(string itemName)
        {
            Content content;
            if (!contents.TryGetValue(itemName, out content))
            {
                lock (contentLock)
                {
                    if (!contents.TryGetValue(itemName, out content))
                    {
                        return null;
                    }
                }
            }

            return content.ItemContent;
        }

        public int GetType(string itemName)
        {
            Content content;
            if (!contents.TryGetValue(itemName, out content))
            {
                lock (contentLock)
                {
                    if (!contents.TryGetValue(itemName, out content))
                    {
                        return 0;
                    }
                }
            }

            return content.Type;
        }

        public void Deregister(string itemName)
        {
            lock (contentLock)
            {
                contents.Remove(itemName);
            }
        }
    }
}

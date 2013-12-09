using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author: Tektaara
     * @version: v1.0
     * @description: Language object class
     */
    class Language
    {
        public readonly string Name;
        private Dictionary<long, string> dictionary;

        public Language(string Name)
        {
            this.Name = Name;
            dictionary = new Dictionary<long, string>();
        }

        public void addText(long key, string val)
        {
            dictionary.Add(key, val);
        }

        public string getText(long key)
        {
            string s = "Default";
            bool b = dictionary.TryGetValue(key, out s);
            return b ? s : "KeyNotInDictionary";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author: Raymond Rohder
     * @version: 1.0
     * @description: class wich contains all the information for the turorials
     */
    public class Tutorial
    {
        private List<string> texts = new List<string>();
        private int index = 0;
        public Tutorial()
        {

        }

        public int getMax()
        {
            return texts.Count;
        }

        public void addText(string text)
        {
            texts.Add(text);
        }

        public string getText()
        {
            return texts[index];
        }

        public int getIndex()
        {
            return index;
        }

        private int clamp(int n, int min, int max)
        {
            return n < min ? min : n > max ? max : n;
        }

        public void next()
        {
            index = clamp(index + 1, 0, texts.Count-1); 
        }

        public void prev()
        {
            index = clamp(index - 1, 0, texts.Count-1); 
        }
    }
}

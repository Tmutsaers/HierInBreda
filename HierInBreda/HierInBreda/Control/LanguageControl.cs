using HierInBreda.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Control
{
    /*
     * @author: Tektaara
     * @version: v1.0
     * @description: Language Control class
     */
    class LanguageControl
    {
        public List<Language> Languages;
        private static LanguageControl instance;
        private Language activeLanguage;

        private LanguageControl()
        {
            Languages = new List<Language>();
            initLanguages();
         //
            SetActiveLanguage("Dutch");
        
            string s1 = getActiveLanguage().getText(42);
            string s2 = getActiveLanguage().getText(1);
            SetActiveLanguage("English");
            string s3 = getActiveLanguage().getText(42);
            string s4 = getActiveLanguage().getText(1);
            int breakpoint;
        }

        public Language getActiveLanguage()
        {
            return activeLanguage;
        }

        private void SetActiveLanguage(string s)
        {
            for (int i = 0; i < Languages.Count; i++)
            {
                if (Languages[i].Name.CompareTo(s) == 0)
                {
                    activeLanguage = Languages[i];
                    break;
                }
            }
        }

        private void initLanguages()
        {
            Language l;

            l = new Language("Dutch");
            //ADD STUFF TO l.dictionary HERE
            l.addText(1, "hoi");
            Languages.Add(l);

            l = new Language("English");
            //ADD STUFF TO l.dictionary HERE
            l.addText(1, "hi");
            Languages.Add(l);
        }

        public static LanguageControl GetInstance()
        {
            return instance != null ? instance : (instance = new LanguageControl());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    /// <summary>
    /// A class that's used to determine whether or not a word is a stop word.
    /// </summary>
    public class StopWordRemover
    {
        static String stopwordList = "" +
                            "a\nabout\nabove\nafter\nagain\nagainst\nall\nam\nan\nand\nany\nare\naren't\nas\nat\n" +
                           "be\nbecause\nbeen\nbefore\nbeing\nbelow\nbetween\nboth\nbut\nby\n" +
                           "can't\ncannot\ncould\ncouldn't\n" +
                           "did\ndidn't\ndo\ndoes\ndoesn't\ndoing\ndon't\ndown\nduring\n" +
                           "each\n" +
                           "few\nfor\nfrom\nfurther\n" +
                           "had\nhadn't\nhas\nhasn't\nhave\nhaven't\nhaving\nhe\nhe'd\nhe'll\nhe's\nher\nhere\nhere's\nhers\nherself\nhim\nhimself\nhis\nhow\nhow's\n" +
                           "i\ni'd\ni'll\ni'm\ni've\nif\nin\ninto\nis\nisn't\nit\nit's\nits\nitself\n" +
                           "let's\n" +
                           "me\nmore\nmost\nmustn't\nmy\nmyself\n" +
                           "no\nnor\nnot\n" +
                           "of\noff\non\nonce\nonly\nor\nother\nought\nour\nours\nourselves\nout\nover\nown\n" +
                           "same\nshan't\nshe\nshe'd\nshe'll\nshe's\nshould\nshouldn't\nso\nsome\nsuch\n" +
                           "than\nthat\nthat's\nthe\ntheir\ntheirs\nthem\nthemselves\nthen\nthere\nthere's\nthese\nthey\nthey'd\nthey'll\nthey're\nthey've\nthis\nthose\nthrough\nto\ntoo\n" +
                           "under\nuntil\nup\n" +
                           "very\n" +
                           "was\nwasn't\nwe\nwe'd\nwe'll\nwe're\nwe've\nwere\nweren't\nwhat\nwhat's\nwhen\nwhen's\nwhere\nwhere's\nwhich\nwhile\nwho\nwho's\nwhom\nwhy\nwhy's\nwith\nwon't\nwould\nwouldn't\n" +
                           "you\nyou'd\nyou'll\nyou're\nyou've\nyour\nyours\nyourself\nyourselves\n";

        private HashSet<String> stopWords;

        public StopWordRemover() 
        {
            stopWords = new HashSet<String>();
            foreach (var word in stopwordList.Split('\n')) 
            {
                stopWords.Add(word);
            }
        }
            
        /// <summary>
        /// Return true if the given word is stop word.
        /// </summary>
        public bool isStopWord(String word)
        {
            return stopWords.Contains(word);
        }
    }
}


using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace FontAwesomeParser.Terminal
{
    public class CssParser
    {
        private string content;
        private List<CssClass> result;
        private ISet<string> uniqueNameSet;
        
        public CssParser(string content)
        {
            this.content = content;
            this.result = new List<CssClass>();
            this.uniqueNameSet = new HashSet<string>();
        }

        public void Parse()
        {
            List<char> buffer = new List<char>();
            bool fillStarted = false;
            for (int i = 0; i < this.content.Length; i++)
            {
                char c = content[i];
                int nextIndex = i == this.content.Length - 1 
                    ? i   
                    : i + 1;
                
                char nextC = content[nextIndex];
                
                if (!fillStarted)
                {
                    if (!this.IsDot(c))
                    {
                        continue;
                    }

                    if (char.IsPunctuation(nextC) || char.IsDigit(nextC))
                    {
                        continue;
                    }
                    
                    fillStarted = true;
                    buffer.Add(c);
                }
                else
                {
                    if (this.IsEndOfClassName(c))
                    {
                        this.uniqueNameSet.Add(new string(buffer.ToArray()));
                        //result.Add(new CssClass());
                        buffer.Clear();
                        fillStarted = false;
                    }
                    else if (this.IsNotNameSymbols(c))
                    {
                        buffer.Clear();
                        fillStarted = false;
                    }
                    else if (
                        (char.IsLetterOrDigit(c) || char.IsPunctuation(c)) 
                        && !this.IsCurlyBrackets(c))
                    {
                        buffer.Add(c);
                    }
                }
            }
        }

        public IEnumerable<CssClass> GetResult()
        {
            IEnumerable<CssClass> list = this.uniqueNameSet.Select(x => new CssClass(x));
            return list;
        }
        
        public IEnumerable<CssClass> Result =>  this.uniqueNameSet.Select(x => new CssClass(x));

        private bool IsNotNameSymbols(char c)
        {
            return c == '\\' || c == '/' || c== '"' || c == '#';
        }
        
        private bool IsCurlyBrackets(char c)
        {
            return c == '{' || c == '}';
        }
        
        private bool IsDot(char c)
        {
            return c == '.';
        }

        private bool IsComma(char c)
        {
            return c == ',';
        }

        private bool IsColon(char c)
        {
            return c == ':';
        }

        private bool IsEndOfClassName(char c)
        {
            return this.IsDot(c) || this.IsComma(c) || char.IsWhiteSpace(c) || this.IsColon(c);
        } 
    }
}
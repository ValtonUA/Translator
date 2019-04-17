using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class InfoTable_old
    {
        #region Fields
        Dictionary<int, string> _keywords = new Dictionary<int, string> ();
        Dictionary<int, string> _identifiers = new Dictionary<int, string>();
        Dictionary<int, string> _constants = new Dictionary<int, string>();
        Dictionary<int, string> _shortDelimiters = new Dictionary<int, string>();
        Dictionary<int, string> _longDelimiters = new Dictionary<int, string>();

        int _keywordsCounter;
        int _identifiersCounter;
        int _constantsCounter;
        int _shortDelimitersCounter;
        int _longDelimitersCounter;

        public const int SHORT_DELIMITERS_START = 0;
        public const int LONG_DELIMITERS_START = 301;
        public const int KEYWORDS_START = 401;
        public const int CONSTANTS_START = 501;
        public const int IDENTIFIERS_START = 1001;

        #endregion

        public InfoTable_old()
        {
            _keywordsCounter = KEYWORDS_START;
            _identifiersCounter = IDENTIFIERS_START;
            _constantsCounter = CONSTANTS_START;
            _shortDelimitersCounter = SHORT_DELIMITERS_START;
            _longDelimitersCounter = LONG_DELIMITERS_START;
        }
        #region Methods
        public int AddKeyword(string value)
        {
            if (!_keywords.ContainsValue(value))
            {
                _keywords.Add(_keywordsCounter, value);
                return _keywordsCounter++;
            }
            else
            {
                return _keywords.FirstOrDefault(v => v.Value == value).Key;
            }
        }

        public List<int> AddKeyword(List<string> values)
        {
            List<int> codes = new List<int>();
            values?.ForEach(v =>
            {
                codes.Add(AddKeyword(v));
            });
            return codes;
        }

        public int AddIdentifier(string value)
        {
            if (!_identifiers.ContainsValue(value))
            {
                _identifiers.Add(_identifiersCounter, value);
                return _identifiersCounter++;
            }
            else
            {
                return _identifiers.FirstOrDefault(v => v.Value == value).Key;
            }
        }

        public List<int> AddIdentifier(List<string> values)
        {
            List<int> codes = new List<int>();
            values?.ForEach(v =>
            {
                codes.Add(AddIdentifier(v));
            });
            return codes;
        }

        public int AddConstant(string value)
            // returns the key (code)
        {
            if (!_constants.ContainsValue(value))
            {
                _constants.Add(_constantsCounter, value);
                return _constantsCounter++;
            }
            else
            {
                return _constants.FirstOrDefault(v => v.Value == value).Key;
            }
        }

        public List<int> AddConstant(List<string> values)
        {
            List<int> codes = new List<int>();
            values?.ForEach(v =>
            {
                codes.Add(AddConstant(v));
            });
            return codes;
        }

        public int AddShortDelimiter(string value)
        {
            if (!_shortDelimiters.ContainsValue(value))
            {
                _shortDelimiters.Add(_shortDelimitersCounter, value);
                return _shortDelimitersCounter++;
            }
            else
            {
                return _shortDelimiters.FirstOrDefault(v => v.Value == value).Key;
            }
        }

        public List<int> AddShortDelimiter(List<string> values)
        {
            List<int> codes = new List<int>();
            values?.ForEach(v =>
            {
                codes.Add(AddShortDelimiter(v));
            });
            return codes;
        }

        public int AddLongDelimiter(string value)
        {
            if (!_longDelimiters.ContainsValue(value))
            { 
                _longDelimiters.Add(_longDelimitersCounter, value);
                return _longDelimitersCounter++;
            }
            else
            {
                return _longDelimiters.FirstOrDefault(v => v.Value == value).Key;
            }
        }

        public List<int> AddLongDelimiter(List<string> values)
        {
            List<int> codes = new List<int>();
            values?.ForEach(v =>
            {
                codes.Add(AddLongDelimiter(v));
            });
            return codes;
        }

        public Dictionary<int, string> GetKeywords()
        {
            return _keywords;
        }

        public Dictionary<int, string> GetIdentifiers()
        {
            return _identifiers;
        }

        public Dictionary<int, string> GetConstants()
        {
            return _constants;
        }

        public Dictionary<int, string> GetShortDelimiters()
        {
            return _shortDelimiters;
        }

        public Dictionary<int, string> GetLongDelimiters()
        {
            return _longDelimiters;
        }

        public void PrintFullInfo()
        {
            Console.WriteLine("Keywords"); 
            foreach (int key in _keywords.Keys)
            {
                Console.WriteLine("code: " + key + ", value: " + _keywords[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Identifiers");
            foreach (int key in _identifiers.Keys)
            {
                Console.WriteLine("code: " + key + ", value: " + _identifiers[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Constants");
            foreach (int key in _constants.Keys)
            {
                Console.WriteLine("code: " + key + ", value: " + _constants[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Short delimiters");
            foreach (int key in _shortDelimiters.Keys)
            {
                Console.WriteLine("code: " + key + ", value: " + _shortDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Long delimiters");
            foreach (int key in _longDelimiters.Keys)
            {
                Console.WriteLine("code: " + key + ", value: " + _longDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");

        }
        #endregion
    }
}

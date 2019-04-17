using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZTranslator
{
    public class InfoTable
    {
        #region Fields
        Dictionary<string, int> _keywords = new Dictionary<string, int>();
        Dictionary<string, int> _identifiers = new Dictionary<string, int>();
        Dictionary<string, int> _constants = new Dictionary<string, int>();
        Dictionary<string, int> _shortDelimiters = new Dictionary<string, int>();
        Dictionary<string, int> _longDelimiters = new Dictionary<string, int>();

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

        public InfoTable()
        {
            _keywordsCounter = KEYWORDS_START;
            _identifiersCounter = IDENTIFIERS_START;
            _constantsCounter = CONSTANTS_START;
            _shortDelimitersCounter = SHORT_DELIMITERS_START;
            _longDelimitersCounter = LONG_DELIMITERS_START;
        }
        #region Methods
        public string GetKey(int value)
        {
            string result = "";
            if (value >= IDENTIFIERS_START)
                result = _identifiers.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= CONSTANTS_START)
                result = _constants.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= KEYWORDS_START)
                result = _keywords.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= LONG_DELIMITERS_START)
                result = _longDelimiters.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= SHORT_DELIMITERS_START)
                result = _shortDelimiters.FirstOrDefault(tmp => tmp.Value == value).Key;

            return result;
        }

        public int AddKeyword(string key)
        {
            if (!_keywords.ContainsKey(key))
            {
                _keywords.Add(key, _keywordsCounter);
                return _keywordsCounter++;
            }
            else
            {
                return _keywords[key];
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

        public int AddIdentifier(string key)
        {
            if (!_identifiers.ContainsKey(key))
            {
                _identifiers.Add(key, _identifiersCounter);
                return _identifiersCounter++;
            }
            else
            {
                return _identifiers[key];
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

        public int AddConstant(string key)
        // returns the value (code)
        {
            if (!_constants.ContainsKey(key))
            {
                _constants.Add(key, _constantsCounter);
                return _constantsCounter++;
            }
            else
            {
                return _constants[key];
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

        public int AddShortDelimiter(string key)
        {
            if (!_shortDelimiters.ContainsKey(key))
            {
                _shortDelimiters.Add(key, _shortDelimitersCounter);
                return _shortDelimitersCounter++;
            }
            else
            {
                return _shortDelimiters[key];
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

        public int AddLongDelimiter(string key)
        {
            if (!_longDelimiters.ContainsKey(key))
            {
                _longDelimiters.Add(key, _longDelimitersCounter);
                return _longDelimitersCounter++;
            }
            else
            {
                return _longDelimiters[key];
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

        public Dictionary<string, int> GetKeywords()
        {
            return _keywords;
        }

        public Dictionary<string, int> GetIdentifiers()
        {
            return _identifiers;
        }

        public Dictionary<string, int> GetConstants()
        {
            return _constants;
        }

        public Dictionary<string, int> GetShortDelimiters()
        {
            return _shortDelimiters;
        }

        public Dictionary<string, int> GetLongDelimiters()
        {
            return _longDelimiters;
        }

        public void PrintFullInfo()
        {
            Console.WriteLine("Keywords");
            foreach (string key in _keywords.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + _keywords[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Identifiers");
            foreach (string key in _identifiers.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + _identifiers[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Constants");
            foreach (string key in _constants.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + _constants[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Short delimiters");
            foreach (string key in _shortDelimiters.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + _shortDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Long delimiters");
            foreach (string key in _longDelimiters.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + _longDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");

        }
        #endregion
    }
}

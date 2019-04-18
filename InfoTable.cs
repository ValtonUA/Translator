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
        public Dictionary<string, int> Keywords { get; private set; }
        public Dictionary<string, int> Identifiers { get; private set; }
        public Dictionary<string, int> Constants { get; private set; }
        public Dictionary<string, int> ShortDelimiters { get; private set; }
        public Dictionary<string, int> LongDelimiters { get; private set; }

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

            Keywords = new Dictionary<string, int>();
            Identifiers = new Dictionary<string, int>();
            Constants = new Dictionary<string, int>();
            ShortDelimiters = new Dictionary<string, int>();
            LongDelimiters = new Dictionary<string, int>();
        }
        #region Methods
        public string GetKey(int value)
        {
            string result = "";
            if (value >= IDENTIFIERS_START)
                result = Identifiers.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= CONSTANTS_START)
                result = Constants.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= KEYWORDS_START)
                result = Keywords.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= LONG_DELIMITERS_START)
                result = LongDelimiters.FirstOrDefault(tmp => tmp.Value == value).Key;
            else if (value >= SHORT_DELIMITERS_START)
                result = ShortDelimiters.FirstOrDefault(tmp => tmp.Value == value).Key;

            return result;
        }

        public int AddKeyword(string key)
        {
            if (!Keywords.ContainsKey(key))
            {
                Keywords.Add(key, _keywordsCounter);
                return _keywordsCounter++;
            }
            else
            {
                return Keywords[key];
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
            if (!Identifiers.ContainsKey(key))
            {
                Identifiers.Add(key, _identifiersCounter);
                return _identifiersCounter++;
            }
            else
            {
                return Identifiers[key];
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
            if (!Constants.ContainsKey(key))
            {
                Constants.Add(key, _constantsCounter);
                return _constantsCounter++;
            }
            else
            {
                return Constants[key];
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
            if (!ShortDelimiters.ContainsKey(key))
            {
                ShortDelimiters.Add(key, _shortDelimitersCounter);
                return _shortDelimitersCounter++;
            }
            else
            {
                return ShortDelimiters[key];
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
            if (!LongDelimiters.ContainsKey(key))
            {
                LongDelimiters.Add(key, _longDelimitersCounter);
                return _longDelimitersCounter++;
            }
            else
            {
                return LongDelimiters[key];
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
            return Keywords;
        }

        public Dictionary<string, int> GetIdentifiers()
        {
            return Identifiers;
        }

        public Dictionary<string, int> GetConstants()
        {
            return Constants;
        }

        public Dictionary<string, int> GetShortDelimiters()
        {
            return ShortDelimiters;
        }

        public Dictionary<string, int> GetLongDelimiters()
        {
            return LongDelimiters;
        }

        public void PrintFullInfo()
        {
            Console.WriteLine("Keywords");
            foreach (string key in Keywords.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + Keywords[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Identifiers");
            foreach (string key in Identifiers.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + Identifiers[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Constants");
            foreach (string key in Constants.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + Constants[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Short delimiters");
            foreach (string key in ShortDelimiters.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + ShortDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Long delimiters");
            foreach (string key in LongDelimiters.Keys)
            {
                Console.WriteLine("key: " + key + ", value: " + LongDelimiters[key]);
            }
            Console.WriteLine("-------------------------------------");

        }
        #endregion
    }
}

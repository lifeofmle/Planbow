using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HotDinnerData
{
    public static class CsvUtility
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };

        public static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = s.Trim();

            if (s.Contains(QUOTE))
                s = s.Replace(QUOTE, ESCAPED_QUOTE);

            if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                s = QUOTE + s + QUOTE;

            return s;
        }

        public static string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(ESCAPED_QUOTE))
                    s = s.Replace(ESCAPED_QUOTE, QUOTE);
            }

            return s;
        }

        public static List<string[]> ParseCsv(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return parsedData;
        }
    }

    public sealed class CsvReader : System.IDisposable
    {
        private long _rowno = 0;
        private TextReader _reader;
        private static Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private static Regex rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");

        public CsvReader(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }

        public CsvReader(Stream stream)
        {
            _reader = new StreamReader(stream);
        }

        public IEnumerable RowEnumerator
        {
            get
            {
                if (null == _reader)
                    throw new System.ApplicationException("I can't start reading without CSV input.");

                _rowno = 0;
                string sLine;
                string sNextLine;

                while (null != (sLine = _reader.ReadLine()))
                {
                    while (rexRunOnLine.IsMatch(sLine) && null != (sNextLine = _reader.ReadLine()))
                        sLine += "\n" + sNextLine;

                    _rowno++;
                    string[] values = rexCsvSplitter.Split(sLine);

                    for (int i = 0; i < values.Length; i++)
                        values[i] = CsvUtility.Unescape(values[i]);

                    yield return values;
                }

                _reader.Close();
            }
        }

        public long RowIndex { get { return _rowno; } }

        public void Dispose()
        {
            if (null != _reader) _reader.Dispose();
        }
    }
}

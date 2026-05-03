#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HwigiTower.Encounters
{
    public sealed class EncounterJsonParseException : Exception
    {
        public EncounterJsonParseException(string message) : base(message)
        {
        }
    }

    internal static class EncounterJsonMiniParser
    {
        public static object Parse(string json)
        {
            var parser = new Parser(json ?? string.Empty);
            return parser.Parse();
        }

        private sealed class Parser
        {
            private readonly string _json;
            private int _index;

            public Parser(string json)
            {
                _json = json;
            }

            public object Parse()
            {
                SkipWhitespace();
                var value = ParseValue();
                SkipWhitespace();
                if (_index != _json.Length)
                {
                    Throw("Unexpected trailing characters.");
                }

                return value;
            }

            private object ParseValue()
            {
                SkipWhitespace();
                if (_index >= _json.Length)
                {
                    Throw("Unexpected end of JSON.");
                }

                var c = _json[_index];
                switch (c)
                {
                    case '{':
                        return ParseObject();
                    case '[':
                        return ParseArray();
                    case '"':
                        return ParseString();
                    case 't':
                        ConsumeLiteral("true");
                        return true;
                    case 'f':
                        ConsumeLiteral("false");
                        return false;
                    case 'n':
                        ConsumeLiteral("null");
                        return null;
                    default:
                        if (c == '-' || char.IsDigit(c))
                        {
                            return ParseNumber();
                        }

                        Throw("Unexpected character '" + c + "'.");
                        return null;
                }
            }

            private Dictionary<string, object> ParseObject()
            {
                Expect('{');
                var map = new Dictionary<string, object>();
                SkipWhitespace();
                if (TryConsume('}'))
                {
                    return map;
                }

                while (true)
                {
                    SkipWhitespace();
                    if (_index >= _json.Length || _json[_index] != '"')
                    {
                        Throw("Expected object key string.");
                    }

                    var key = ParseString();
                    SkipWhitespace();
                    Expect(':');
                    map[key] = ParseValue();
                    SkipWhitespace();

                    if (TryConsume('}'))
                    {
                        return map;
                    }

                    Expect(',');
                }
            }

            private List<object> ParseArray()
            {
                Expect('[');
                var list = new List<object>();
                SkipWhitespace();
                if (TryConsume(']'))
                {
                    return list;
                }

                while (true)
                {
                    list.Add(ParseValue());
                    SkipWhitespace();
                    if (TryConsume(']'))
                    {
                        return list;
                    }

                    Expect(',');
                }
            }

            private string ParseString()
            {
                Expect('"');
                var builder = new StringBuilder();
                while (_index < _json.Length)
                {
                    var c = _json[_index++];
                    if (c == '"')
                    {
                        return builder.ToString();
                    }

                    if (c != '\\')
                    {
                        builder.Append(c);
                        continue;
                    }

                    if (_index >= _json.Length)
                    {
                        Throw("Unterminated escape sequence.");
                    }

                    var escape = _json[_index++];
                    switch (escape)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            builder.Append(escape);
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            builder.Append(ParseUnicodeEscape());
                            break;
                        default:
                            Throw("Unsupported escape sequence.");
                            break;
                    }
                }

                Throw("Unterminated string.");
                return string.Empty;
            }

            private char ParseUnicodeEscape()
            {
                if (_index + 4 > _json.Length)
                {
                    Throw("Invalid unicode escape.");
                }

                var hex = _json.Substring(_index, 4);
                _index += 4;
                if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
                {
                    Throw("Invalid unicode escape.");
                }

                return (char)value;
            }

            private object ParseNumber()
            {
                var start = _index;
                if (_json[_index] == '-')
                {
                    _index++;
                }

                while (_index < _json.Length && char.IsDigit(_json[_index]))
                {
                    _index++;
                }

                var isFloat = false;
                if (_index < _json.Length && _json[_index] == '.')
                {
                    isFloat = true;
                    _index++;
                    while (_index < _json.Length && char.IsDigit(_json[_index]))
                    {
                        _index++;
                    }
                }

                if (_index < _json.Length && (_json[_index] == 'e' || _json[_index] == 'E'))
                {
                    isFloat = true;
                    _index++;
                    if (_index < _json.Length && (_json[_index] == '+' || _json[_index] == '-'))
                    {
                        _index++;
                    }

                    while (_index < _json.Length && char.IsDigit(_json[_index]))
                    {
                        _index++;
                    }
                }

                var text = _json.Substring(start, _index - start);
                if (isFloat)
                {
                    if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
                    {
                        return doubleValue;
                    }
                }
                else if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                {
                    return longValue >= int.MinValue && longValue <= int.MaxValue ? (object)(int)longValue : longValue;
                }

                Throw("Invalid number.");
                return 0;
            }

            private void ConsumeLiteral(string literal)
            {
                if (_index + literal.Length > _json.Length || _json.Substring(_index, literal.Length) != literal)
                {
                    Throw("Expected literal " + literal + ".");
                }

                _index += literal.Length;
            }

            private void Expect(char c)
            {
                SkipWhitespace();
                if (_index >= _json.Length || _json[_index] != c)
                {
                    Throw("Expected '" + c + "'.");
                }

                _index++;
            }

            private bool TryConsume(char c)
            {
                SkipWhitespace();
                if (_index < _json.Length && _json[_index] == c)
                {
                    _index++;
                    return true;
                }

                return false;
            }

            private void SkipWhitespace()
            {
                while (_index < _json.Length && char.IsWhiteSpace(_json[_index]))
                {
                    _index++;
                }
            }

            private void Throw(string message)
            {
                throw new EncounterJsonParseException(message + " index=" + _index);
            }
        }
    }
}
#endif

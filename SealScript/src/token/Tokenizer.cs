using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SealScript;

public class Tokenizer : IDisposable
{
    private readonly TextReader _reader;
    
    private int _line;
    private int _column;

    private char _initialChar;
    private int _initialColumn;
    
    public Tokenizer(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }
    
    public Tokenizer(Stream stream)
    {
        _reader = new StreamReader(stream);
    }

    public Tokenizer(string s)
    {
        _reader = new StringReader(s);
    }
    
    public TextReader BaseReader => _reader;

    public void Dispose()
    {
        _reader.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public Token[] Tokenize()
    {
        var tokens = new List<Token>();

        _line = 1;
        _column = 0;
        
        while (true)
        {
            if (!TryPeek(out _initialChar))
            {
                break;
            }
            
            _initialColumn = _column;
            
            Advance();
            
            // Skip whitespace
            if (_initialChar <= ' ')
            {
                continue;
            }

            if (_initialChar == '/' && TryPeek(out char next))
            {
                switch (next)
                {
                case '/':
                    Advance();
                    SkipSingleComment();
                    continue;
                case '*':
                    Advance();
                    SkipMultiComment();
                    continue;
                }
            }
            
            tokens.Add(NextToken());
        }

        return tokens.ToArray();
    }

    private static bool IsAlpha(char c)
    {
        return c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or '_';
    }
    
    private static bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || c is >= '0' and <= '9';
    }

    private static bool IsStringDelimiter(char c)
    {
        return c is '"' or '\'';
    }

    private static bool TryGetEscapeChar(char escapeCode, out char escapeChar)
    {
        escapeChar = escapeCode switch
        {
            '0' => '\0',
            'a' => '\a',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'v' => '\v',
            '\\' => '\\',
            '\'' => '\'',
            '\"' => '\"',
            _ => '_',
        };
        
        return escapeChar != '_';
    }
    
    private static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private static bool IsDigitOrDecimal(char c)
    {
        return c is (>= '0' and <= '9') or '.';
    }
    
    private Token NextToken()
    {
        if (TryPeek(out char next)
            && SealConfig.DoubleSymbolMap.TryGetValue($"{_initialChar}{next}", out TokenType doubleTokenType))
        {
            Advance();
            return CreateToken(doubleTokenType);
        }
        
        if (SealConfig.SingleSymbolMap.TryGetValue(_initialChar, out TokenType singleTokenType))
        {
            return CreateToken(singleTokenType);
        }

        if (IsAlpha(_initialChar))
        {
            return NextAlphaNumericToken();
        }

        if (IsStringDelimiter(_initialChar))
        {
            return NextStringToken();
        }

        if (IsDigit(_initialChar))
        {
            return NextNumberToken();
        }

        if (_initialChar == '#')
        {
            return NextFlagToken();
        }

        throw new SealException(_line, _column, $"Unexpected symbol '{_initialChar}'.");
    }
    
    private Token NextAlphaNumericToken()
    {
        var sb = new StringBuilder();
        sb.Append(_initialChar);

        while (TryPeek(out char peek) && IsAlphaNumeric(peek))
        {
            Advance();
            sb.Append(peek);
        }

        string str = sb.ToString();

        if (SealConfig.ConstantMap.TryGetValue(str, out SealValue value))
        {
            return CreateToken(TokenType.Literal, value);
        }

        if (SealConfig.KeywordMap.TryGetValue(str, out TokenType keywordType))
        {
            return CreateToken(keywordType);
        }

        return CreateToken(TokenType.Identifier, str);
    }
    
    private Token NextStringToken()
    {
        var sb = new StringBuilder();

        while (TryPeek(out char peek) && peek != _initialChar && peek != '\n')
        {
            Advance();

            if (peek == '\\'
                && TryPeek(out char next)
                && TryGetEscapeChar(next, out char escapeChar))
            {
                Advance();
                
                sb.Append(escapeChar);
            }
            else
            {
                sb.Append(peek);
            }
        }

        if (EndOfStream())
        {
            throw new SealException(_line, _column, "String missing end delimiter.");
        }
        
        // Skip trailing delimiter
        Advance();

        string str = sb.ToString();

        return CreateToken(TokenType.Literal, str);
    }

    private Token NextNumberToken()
    {
        var sb = new StringBuilder();
        sb.Append(_initialChar);

        bool foundDecimal = _initialChar == '.';

        while (TryPeek(out char peek) && IsDigitOrDecimal(peek))
        {
            Advance();
            sb.Append(peek);
            
            if (peek != '.')
            {
                continue;
            }

            if (foundDecimal)
            {
                throw new SealException(_line, _column, "Number contained multiple decimal places.");
            }
            
            foundDecimal = true;
        }

        string str = sb.ToString();

        if (!double.TryParse(str, out double value))
        {
            throw new SealException(_line, _column, $"Failed to parse number '{str}'");
        }

        return CreateToken(TokenType.Literal, value);
    }

    private Token NextFlagToken()
    {
        var sb = new StringBuilder();

        while (TryPeek(out char peek) && IsAlphaNumeric(peek))
        {
            Advance();
            sb.Append(peek);
        }

        return CreateToken(TokenType.Flag, sb.ToString());
    }

    private Token CreateToken(TokenType tokenType, SealValue value = default)
    {
        return new Token(_line, _initialColumn, tokenType, value);
    }

    private void SkipSingleComment()
    {
        while (TryPeek(out char peek) && peek != '\n')
        {
            Advance();
        }
    }

    private void SkipMultiComment()
    {
        char last = '\0';
        
        while (TryPeek(out char peek)
               && !(last == '*' && peek == '/'))
        {
            Advance();
            last = peek;
        }

        if (EndOfStream())
        {
            throw new SealException(_line, _column, "Multi-comment missing end delimiter '*/'.");
        }
        
        Advance();
    }
    
    private bool TryPeek(out char character)
    {
        int value = _reader.Peek();

        if (value < 0)
        {
            character = '\0';
            return false;
        }
        
        character = (char)value;
        return true;
    }
    
    private void Advance()
    {
        int value = _reader.Read();

        if (value < 0)
        {
            return;
        }
        
        char c = (char)value;

        if (c == '\n')
        {
            _line++;
        }

        if (c is '\n' or '\r')
        {
            _column = 0;
        }
        else
        {
            _column++;
        }
    }

    private bool EndOfStream()
    {
        return _reader.Peek() < 0;
    }
}
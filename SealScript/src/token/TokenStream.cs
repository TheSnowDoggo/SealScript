using System;

namespace SealScript;

public class TokenStream
{
    private readonly Token[] _tokens;
    
    private int _position;
    
    public TokenStream(Token[] tokens)
    {
        _tokens = tokens;
    }

    public Token[] Tokens => _tokens;

    public int Position => _position;
    public int Length => _tokens.Length;
    
    public bool EndOfStream => _position >= _tokens.Length;

    public Token LastToken =>
        _tokens.Length > 0 ? _tokens[Math.Max(_position - 1, 0)] : null;
    
    public int Line => LastToken?.Line ?? -1;
    public int Column => LastToken?.Column ?? -1;
    
    public ParsingFlag ParsingFlags { get; set; }

    public Token this[int position] => _tokens[position];

    public bool HasFlag(ParsingFlag flag)
    {
        return (ParsingFlags & flag) != 0;
    }

    public Token Peek()
    {
        if (_position >= _tokens.Length)
        {
            throw new SealException(Line, Column, "Unexpected end of stream.");
        }
        
        return _tokens[_position];
    }

    public Token Read()
    {
        if (_position >= _tokens.Length)
        {
            throw new SealException(Line, Column, "Unexpected end of stream.");
        }
        
        return _tokens[_position++];
    }

    public void Seek(int newPosition)
    {
        if (newPosition < 0 || newPosition > _tokens.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(newPosition), newPosition, "New position exceeds stream bounds.");
        }

        _position = newPosition;
    }

    public Token Consume(TokenType expectedType)
    {
        Token token = Read();

        if (token.TokenType != expectedType)
        {
            throw new SealException(Line, Column, $"Expected token of type {expectedType}, but got {token.TokenType}.");
        }
        
        return token;
    }

    public bool TryConsume(TokenType acceptedType, out Token token)
    {
        token = Peek();

        if (token.TokenType != acceptedType)
        {
            return false;
        }

        _position++;

        return true;
    }
    
    public bool TryConsume(TokenType acceptedType)
    {
        return TryConsume(acceptedType, out _);
    }
}
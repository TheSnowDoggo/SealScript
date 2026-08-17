using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace SealScript;

public static class SealConfig
{
    public static readonly FrozenDictionary<char, TokenType> SingleSymbolMap = new Dictionary<char, TokenType>()
    {
        { '(', TokenType.OpenParen },
        { ')', TokenType.CloseParen },
        { '{', TokenType.OpenBrace },
        { '}', TokenType.CloseBrace },
        { '[', TokenType.OpenSquare },
        { ']', TokenType.CloseSquare },
        { ';', TokenType.Semicolon },
        { ':', TokenType.Colon },
        { ',', TokenType.Comma },
        { '.', TokenType.Dot },
        { '!', TokenType.Not },
        { '*', TokenType.Multiply },
        { '/', TokenType.Divide },
        { '%', TokenType.Modulo },
        { '+', TokenType.Add },
        { '-', TokenType.Subtract },
        { '<', TokenType.LessThan },
        { '>', TokenType.GreaterThan },
        { '&', TokenType.And },
        { '^', TokenType.Xor },
        { '|', TokenType.Or },
        { '=', TokenType.Assign },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, TokenType> DoubleSymbolMap = new Dictionary<string, TokenType>()
    {
        { "<=", TokenType.LessThanOrEqual },
        { ">=", TokenType.GreaterThanOrEqual },
        { "==", TokenType.Equals },
        { "!=", TokenType.NotEquals },
        { "&&", TokenType.ShortcutAnd },
        { "||", TokenType.ShortcutOr },
        { "*=", TokenType.MultiplyAssign },
        { "/=", TokenType.DivideAssign },
        { "%=", TokenType.ModuloAssign },
        { "+=", TokenType.AddAssign },
        { "-=", TokenType.SubtractAssign },
        { "&=", TokenType.AndAssign },
        { "^=", TokenType.XorAssign },
        { "|=", TokenType.OrAssign },
    }.ToFrozenDictionary();

    public static readonly FrozenSet<TokenType> SymbolTypes =
        SingleSymbolMap.Values.Concat(DoubleSymbolMap.Values).ToFrozenSet();

    public static readonly FrozenDictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>()
    {
        { "var", TokenType.Var },
        { "func", TokenType.Func },
        { "class", TokenType.Class },
        { "constructor", TokenType.Constructor },
        { "static", TokenType.Static },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "in", TokenType.In },
        { "return", TokenType.Return },
        { "break", TokenType.Break },
        { "continue", TokenType.Continue },
        { "and", TokenType.ShortcutAnd },
        { "or", TokenType.ShortcutOr },
        { "not", TokenType.Not },
    }.ToFrozenDictionary();
    
    public static readonly FrozenSet<TokenType> KeywordTypes = KeywordMap.Values.ToFrozenSet();

    public static readonly FrozenDictionary<string, SealValue> ConstantMap = new Dictionary<string, SealValue>()
    {
        { "nil", SealValue.Nil },
        { "true", true },
        { "false", false },
    }.ToFrozenDictionary();

    public const int MaxPrecedence = 11;

    public static readonly FrozenDictionary<TokenType, int> PrecedenceMap = new Dictionary<TokenType, int>()
    {
        { TokenType.Dot, MaxPrecedence },
        { TokenType.UnaryMinus, 10 },
        { TokenType.Not, 10 },
        { TokenType.Multiply, 9 },
        { TokenType.Divide, 9 },
        { TokenType.Modulo, 9 },
        { TokenType.Add, 8 },
        { TokenType.Subtract, 8 },
        { TokenType.LessThan, 7 },
        { TokenType.GreaterThan, 7 },
        { TokenType.LessThanOrEqual, 7 },
        { TokenType.GreaterThanOrEqual, 7 },
        { TokenType.Equals, 6 },
        { TokenType.NotEquals, 6 },
        { TokenType.And, 5 },
        { TokenType.Xor, 4 },
        { TokenType.Or, 3 },
        { TokenType.ShortcutAnd, 2 },
        { TokenType.ShortcutOr, 1 },
        { TokenType.Assign, 0 },
        { TokenType.MultiplyAssign, 0 },
        { TokenType.DivideAssign, 0 },
        { TokenType.ModuloAssign, 0 },
        { TokenType.AddAssign, 0 },
        { TokenType.SubtractAssign, 0 },
        { TokenType.AndAssign, 0 },
        { TokenType.XorAssign, 0 },
        { TokenType.OrAssign, 0 },
    }.ToFrozenDictionary();
    
    public static readonly FrozenDictionary<TokenType, TokenType> UnaryMap = new Dictionary<TokenType, TokenType>()
    {
        { TokenType.Subtract, TokenType.UnaryMinus },
    }.ToFrozenDictionary();

    public static readonly FrozenSet<TokenType> RightAssociativeSet = new HashSet<TokenType>()
    {
        TokenType.UnaryMinus,
        TokenType.Not,
        TokenType.Assign,
    }.ToFrozenSet();
}
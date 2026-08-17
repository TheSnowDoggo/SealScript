namespace SealScript;

public enum TokenType
{
    OpenParen,
    CloseParen,
    
    OpenBrace,
    CloseBrace,
    
    OpenSquare,
    CloseSquare,
    
    Semicolon,
    Colon,
    Comma,
    Dot,
    
    UnaryMinus,
    Not,
    
    Multiply,
    Divide,
    Modulo,
    
    Add,
    Subtract,
    
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    
    Equals,
    NotEquals,
    
    And,
    Xor,
    Or,
    
    ShortcutAnd,
    ShortcutOr,
    
    Assign,
    MultiplyAssign,
    DivideAssign,
    ModuloAssign,
    AddAssign,
    SubtractAssign,
    AndAssign,
    XorAssign,
    OrAssign,
    
    Literal,
    
    Identifier,
    Flag,
    
    Var,
    Func,
    Class,
    Constructor,
    Static,
    If,
    Else,
    While,
    For,
    In,
    Return,
    Continue,
    Break,
}
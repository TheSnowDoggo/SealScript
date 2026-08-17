using System;
using System.Collections.Generic;
using SealScript.Expressions;

namespace SealScript.Statements;

public class StatementParser
{
    private readonly TokenStream _stream;

    public StatementParser(TokenStream stream)
    {
        _stream = stream;
    }
    
    public TokenStream BaseStream => _stream;

    public FunctionDefinition Parse()
    {
        var statements = new List<Statement>();
        
        while (!_stream.EndOfStream)
        {
            statements.Add(ParseStatement());
        }

        return new FunctionDefinition()
        {
            Statements = statements.ToArray(),
        };
    }

    public FunctionDefinition ParseFunctionDefinition(string name = "anonymous")
    {
        if (_stream.TryConsume(TokenType.Identifier, out Token identifierToken))
        {
            name = identifierToken.Value.AsString();
        }
        
        _stream.Consume(TokenType.OpenParen);

        var arguments = new List<string>();

        if (!_stream.TryConsume(TokenType.CloseParen))
        {
            while (!_stream.EndOfStream)
            {
                string argument = _stream.Consume(TokenType.Identifier).Value.AsString();
            
                arguments.Add(argument);

                if (_stream.Peek().TokenType == TokenType.CloseParen)
                {
                    break;
                }

                _stream.Consume(TokenType.Comma);
            }

            _stream.Consume(TokenType.CloseParen);
        }

        Statement[] statments = ReadStatementBlock();

        return new FunctionDefinition()
        {
            Name = name,
            Arguments = arguments.ToArray(),
            Statements = statments,
        };
    }
    
    public ClassDefinition ParseClassDefinition()
    {
        var name = _stream.Consume(TokenType.Identifier).Value.AsString();
        
        _stream.Consume(TokenType.OpenBrace);

        var fields = new Dictionary<string, SealField>();
        var fieldExpressions = new List<Expression>();

        var staticFields = new Dictionary<string, SealField>();
        var staticFieldExpressions = new List<Expression>();
        
        // Constructor is always at location -1
        staticFields.Add("new", new StaticUserSealField(-1));
        
        FunctionDefinition userConstructor = null;

        while (!_stream.EndOfStream)
        {
            Token token = _stream.Peek();

            bool isStatic = false;

            if (token.TokenType == TokenType.Static)
            {
                _stream.Read();
                token = _stream.Peek();
                isStatic = true;
            }
            
            if (token.TokenType == TokenType.CloseBrace)
            {
                break;
            }
            
            switch (token.TokenType)
            {
            case TokenType.Var:
            case TokenType.Func:
            case TokenType.Class:    
            {
                DefineStatement define = token.TokenType switch
                {
                    TokenType.Func => (DefineStatement)ParseFunctionStatement(false),
                    TokenType.Class => (DefineStatement)ParseClassStatement(false),
                    _ => ParseDefineStatement(),
                };

                if (isStatic)
                {
                    var field = new StaticUserSealField(staticFieldExpressions.Count);
                    staticFieldExpressions.Add(define.Expression);

                    if (!staticFields.TryAdd(define.Name, field))
                    {
                        throw new SealException(_stream,
                            $"Static field with name {define.Name} has already been defined.");
                    }
                }
                else
                {
                    var field = new UserSealField(fieldExpressions.Count);
                    
                    fieldExpressions.Add(define.Expression);

                    if (!fields.TryAdd(define.Name, field))
                    {
                        throw new SealException(_stream,
                            $"Field with name {define.Name} has already been defined.");
                    }
                }
                break;
            }
            case TokenType.Constructor:
                if (isStatic)
                {
                    throw new SealException(_stream,
                        "Static constructors do not exist.");
                }
                
                if (userConstructor != null)
                {
                    throw new SealException(_stream,
                        "Constructor has already been defined.");
                }

                _stream.Read();
                
                userConstructor = ParseFunctionDefinition("constructor");
                break;
            default:
                throw new SealException(_stream,
                    $"Unrecognised token {token.TokenType} in class definition.");
            }
        }

        _stream.Consume(TokenType.CloseBrace);

        return new ClassDefinition()
        {
            Name = name,
            Fields = fields,
            FieldsExpressions = fieldExpressions.ToArray(),
            StaticFields = staticFields,
            StaticFieldsExpressions = staticFieldExpressions.ToArray(),
            ConstructorDefinition = userConstructor,
        };
    }
    
    private Statement ParseStatement()
    {
        Token head = _stream.Peek();

        if (head.TokenType == TokenType.Flag)
        {
            ProcessFlag(head.Value.AsString());
            
            _stream.Read();
            
            head = _stream.Peek();
        }
        
        return head.TokenType switch
        {
            TokenType.Var
                => ParseDefineStatement(),
            TokenType.Identifier
                => ParseExpressionStatement(),
            TokenType.OpenBrace
                => ParseBlockStatement(),
            TokenType.Func
                => ParseFunctionStatement(),
            TokenType.Class
                => ParseClassStatement(),
            TokenType.If
                => ParseIfStatement(),
            TokenType.For
                => ParseForStatement(),
            TokenType.While
                => ParseWhileStatement(),
            TokenType.Return
                => ParseReturnStatement(),
            _ => throw new SealException(head, $"Unexpected starting token {head.TokenType}.")
        };
    }

    private void ProcessFlag(string flag)
    {
        switch (flag)
        {
        case "no_terminators":
            _stream.ParsingFlags |= ParsingFlag.NoTerminators;
            break;
        case "terminators":
            _stream.ParsingFlags &= ~ParsingFlag.NoTerminators;
            break;
        default:
            throw new SealException(_stream, $"Unrecognised flag '{flag}'.");
        }
    }
    
    private DefineStatement ParseDefineStatement()
    {
        Token head = _stream.Read();

        string name = _stream.Consume(TokenType.Identifier).Value.AsString();

        Expression expression = LiteralExpression.Nil;

        if (_stream.TryConsume(TokenType.Assign))
        {
            expression = new ExpressionParser(_stream, ParsingMode.Statement).Parse();
        }

        if (!_stream.ParsingFlags.HasFlag(ParsingFlag.NoTerminators))
        {
            _stream.Consume(TokenType.Semicolon);
        }

        return new DefineStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Name = name,
            Expression = expression,
        };
    }
    
    private ExpressionStatement ParseExpressionStatement()
    {
        Token head = _stream.Peek();
        
        Expression expression = new ExpressionParser(_stream, ParsingMode.Statement).Parse();

        if (!_stream.ParsingFlags.HasFlag(ParsingFlag.NoTerminators))
        {
            _stream.Consume(TokenType.Semicolon);
        }

        return new ExpressionStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Expression = expression,
        };
    }
    
    private Statement[] ReadStatementBlock()
    {
        _stream.Consume(TokenType.OpenBrace);
        
        var statements = new List<Statement>();
        
        while (!_stream.EndOfStream 
               && _stream.Peek().TokenType != TokenType.CloseBrace)
        {
            statements.Add(ParseStatement());
        }
        
        _stream.Consume(TokenType.CloseBrace);

        return statements.ToArray();
    }

    private BlockStatement ParseBlockStatement()
    {
        Token head = _stream.Peek();

        Statement[] statements = ReadStatementBlock();

        return new BlockStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Statements = statements,
        };
    }
    
    private IfStatement ParseIfStatement()
    {
        Token head = _stream.Read();

        Expression expression = new ExpressionParser(_stream, ParsingMode.Block).Parse();
        
        Statement[] statements = ReadStatementBlock();

        BlockStatement elseBlock = null;

        if (!_stream.EndOfStream && _stream.TryConsume(TokenType.Else))
        {
            elseBlock = _stream.Peek().TokenType == TokenType.If 
                ? ParseIfStatement() : ParseBlockStatement();
        }
        
        return new IfStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Statements = statements,
            Expression = expression,
            ElseBlock = elseBlock,
        };
    }

    private ReturnStatement ParseReturnStatement()
    {
        Token head = _stream.Read();
        
        Expression expression = new ExpressionParser(_stream, ParsingMode.Statement).Parse();

        if (!_stream.ParsingFlags.HasFlag(ParsingFlag.NoTerminators))
        {
            _stream.Consume(TokenType.Semicolon);
        }

        return new ReturnStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Expression = expression,
        };
    }

    private ExpressionStatement ParseFunctionStatement(bool allowAnonymous = true)
    {
        Token head = _stream.Read();

        if (allowAnonymous && _stream.Peek().TokenType == TokenType.OpenParen)
        {
            _stream.Seek(_stream.Position - 1);
            
            return ParseExpressionStatement();
        }
        
        string name = _stream.Consume(TokenType.Identifier).Value.AsString();

        FunctionDefinition definition = ParseFunctionDefinition(name);

        var expression = new FunctionDefinitionExpression(definition);
        
        return new DefineStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Name = name,
            Expression = expression,
        };
    }
    
    private ExpressionStatement ParseClassStatement(bool allowAnonymous = true)
    {
        Token head = _stream.Read();

        if (allowAnonymous && _stream.Peek().TokenType == TokenType.OpenBrace)
        {
            _stream.Seek(_stream.Position - 1);
            
            return ParseExpressionStatement();
        }

        ClassDefinition definition = ParseClassDefinition();
        
        var expression = new ClassDefinitionExpression(definition);

        return new DefineStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Name = definition.Name,
            Expression = expression,
        };
    }

    private WhileStatement ParseWhileStatement()
    {
        Token head = _stream.Read();
        
        Expression condition = new ExpressionParser(_stream, ParsingMode.Block).Parse();

        Statement[] statements = ReadStatementBlock();

        return new WhileStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Statements = statements,
            Condition = condition,
        };
    }
    
    private ForStatement ParseForStatement()
    {
        Token head = _stream.Read();

        string identifier = _stream.Consume(TokenType.Identifier).Value.AsString();

        _stream.Consume(TokenType.In);
        
        Expression expression = new ExpressionParser(_stream, ParsingMode.Block).Parse();
        
        Statement[] statements = ReadStatementBlock();
        
        return new ForStatement()
        {
            Line = head.Line,
            Column = head.Column,
            Statements = statements,
            Identifier = identifier,
            Expression = expression,
        };
    }
}
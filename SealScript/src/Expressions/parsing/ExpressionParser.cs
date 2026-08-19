using System.Collections.Generic;
using SealScript.Statements;

namespace SealScript.Expressions;

public class ExpressionParser
{
    private readonly TokenStream _stream;
    
    private readonly Stack<Expression> _expressionStack = [];
    private readonly Stack<Token> _operatorStack= [];

    private readonly ExpressionParsingMode _expressionParsingMode;

    private int _startLine;
    private int _bracketDepth;
    
    public ExpressionParser(TokenStream stream, ExpressionParsingMode expressionParsingMode)
    {
        _stream = stream;
        _expressionParsingMode = expressionParsingMode;
    }

    public Expression Parse()
    {
        _expressionStack.Clear();
        _operatorStack.Clear();

        _startLine = -1;
        _bracketDepth = 0;

        while (!_stream.EndOfStream)
        {
            Token token = _stream.Peek();
            
            if (_startLine == -1)
            {
                _startLine = token.Line;
            }
            
            if (ShouldExit(token))
            {
                break;
            }

            _stream.Read();
            
            switch (token.TokenType)
            {
            case TokenType.OpenParen:
                ParseOpenParen(token);
                break;
            case TokenType.CloseParen:
                if (_bracketDepth == 0)
                {
                    throw new SealException(token, "Read close bracket without matching open bracket.");
                }
                
                FlushBracket(true);

                _bracketDepth--;
                break;
            case TokenType.Comma:
                FlushBracket(false);
                break;
            case TokenType.Identifier:
                _expressionStack.Push(new VariableExpression(token.Value.AsString()));
                break;
            case TokenType.Literal:    
                _expressionStack.Push(new LiteralExpression(token.Value));
                break;
            case TokenType.Func:
                ParseFunctionDefinition();
                break;
            case TokenType.Class:
                ParseClassDefinition();
                break;
            case TokenType.OpenSquare:
                ParseOpenSquare(token);
                break;
            case TokenType.OpenBrace:
                ParseMapExpression();
                break;
            default:
                PushOperator(token);
                break;
            }
        }

        FlushAll();
        
        return _expressionStack.Count switch
        {
            > 1 => throw new SealException(_stream, "Failed to parse expression."),
            0 => LiteralExpression.Nil,
            _ => _expressionStack.Pop()
        };
    }

    private static bool ShouldFlush(Token token, int precedence, Token other)
    {
        if (other.TokenType == TokenType.OpenParen)
        {
            return false;
        }
        
        int otherPrecedence = SealConfig.PrecedenceMap[other.TokenType];

        if (precedence < otherPrecedence)
        {
            return true;
        }

        if (precedence > otherPrecedence)
        {
            return false;
        }

        if (SealConfig.RightAssociativeSet.Contains(token.TokenType)
            && SealConfig.RightAssociativeSet.Contains(other.TokenType))
        {
            return false;
        }

        return true;
    }
    
    private static bool IsCallable(Token token)
    {
        return token.TokenType is TokenType.Identifier
            or TokenType.Literal
            or TokenType.CloseParen
            or TokenType.CloseBrace
            or TokenType.CloseSquare;
    }

    private static bool IsOperand(Token token)
    {
        return token.TokenType is TokenType.CloseParen
            or TokenType.CloseBrace
            or TokenType.CloseSquare
            or TokenType.Identifier
            or TokenType.Literal;
    }

    private bool ShouldExit(Token token)
    {
        if (token.TokenType == TokenType.Semicolon)
        {
            return true;
        }

        return _expressionParsingMode switch
        {
            // Exit on newline when no terminator mode is enabled
            ExpressionParsingMode.Statement 
                => _stream.HasFlag(StatementParsingFlag.NoTerminators) 
                    && _startLine != token.Line
                    || token.TokenType == TokenType.CloseBrace,
            ExpressionParsingMode.Block 
                => token.TokenType == TokenType.OpenBrace,
            ExpressionParsingMode.Argument 
                => token.TokenType is TokenType.Comma
                    or TokenType.Colon
                    or TokenType.CloseBrace
                    or TokenType.CloseSquare
                    || (token.TokenType == TokenType.CloseParen && _bracketDepth == 0),
            _ => false
        };
    }
    
    private void PushOperator(Token token)
    {
        if (SealConfig.UnaryMap.TryGetValue(token.TokenType, out TokenType unaryType)
            && (_stream.Position == 0 || !IsOperand(_stream[_stream.Position - 2])))
        {
            token.TokenType = unaryType;
        }
        
        if (!SealConfig.PrecedenceMap.TryGetValue(token.TokenType, out int precedence))
        {
            throw new SealException(_stream, 
                $"Expected operator, got {token.TokenType}. Did you miss a semicolon?");
        }
        
        while (_operatorStack.TryPeek(out Token other) 
               && ShouldFlush(token, precedence, other))
        {
            TransferOperator();
        }
        
        _operatorStack.Push(token);
    }

    private void FlushBracket(bool removeBracket)
    {
        while (_operatorStack.TryPeek(out Token other)
               && other.TokenType != TokenType.OpenParen)
        {
            TransferOperator();
        }

        if (removeBracket)
        {
            _operatorStack.Pop();
        }
    }

    private void FlushAll()
    {
        while (_operatorStack.TryPeek(out Token other))
        {
            if (other.TokenType == TokenType.OpenParen)
            {
                throw new SealException(other.Line, other.Column, "Open bracket missing associated close bracket.");
            }
            
            TransferOperator();
        }
    }

    private void FlushPrecedence(int precedence)
    {
        while (_operatorStack.TryPeek(out Token other) 
               && SealConfig.PrecedenceMap[other.TokenType] >= precedence)
        {
            TransferOperator();
        }
    }
    
    private Expression ToExpression(Token token)
    {
        return token.TokenType switch
        {
            TokenType.Dot
                => ParseMemberFieldExpression(token),
            TokenType.UnaryMinus
                => ParseUnaryExpression(token, UnaryType.Minus),
            TokenType.Not
                => ParseUnaryExpression(token, UnaryType.Not),
            TokenType.Multiply
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Multiply),
            TokenType.Divide
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Divide),
            TokenType.Modulo
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Modulo),
            TokenType.Add
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Add),
            TokenType.Subtract
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Subtract),
            TokenType.LessThan
                => ParseComparisonExpression(token, ComparisonType.LessThan),
            TokenType.GreaterThan
                => ParseComparisonExpression(token, ComparisonType.GreaterThan),
            TokenType.LessThanOrEqual
                => ParseComparisonExpression(token, ComparisonType.LessThanOrEqual),
            TokenType.GreaterThanOrEqual
                => ParseComparisonExpression(token, ComparisonType.GreaterThanOrEqual),
            TokenType.Equals
                => ParseComparisonExpression(token, ComparisonType.Equals),
            TokenType.NotEquals
                => ParseComparisonExpression(token, ComparisonType.NotEquals),
            TokenType.And
                => ParseBinaryArithmeticExpression(token, ArithmeticType.And),
            TokenType.Xor
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Xor),
            TokenType.Or
                => ParseBinaryArithmeticExpression(token, ArithmeticType.Or),
            TokenType.ShortcutAnd
                => ParseBoolAnd(token),
            TokenType.ShortcutOr
                => ParseBoolOr(token),
            TokenType.Assign
                => ParseAssignExpression(token),
            TokenType.MultiplyAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Multiply),
            TokenType.DivideAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Divide),
            TokenType.ModuloAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Modulo),
            TokenType.AddAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Add),
            TokenType.SubtractAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Subtract),
            TokenType.AndAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.And),
            TokenType.XorAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Xor),
            TokenType.OrAssign
                => ParseCompoundArithmeticExpression(token, ArithmeticType.Or),
            _ => throw new SealException(token, 
                $"Unexpected operator type {token.TokenType}.")
        };
    }

    private void TransferOperator()
    {
        Token token = _operatorStack.Pop();
        
        _expressionStack.Push(ToExpression(token));
    }
    
    private void PopUnary(Token token, out Expression operand)
    {
        if (_expressionStack.Count < 1)
        {
            throw new SealException(token,
                $"Unary operator {token.TokenType} expected 1 operand, got {_expressionStack.Count}.");
        }

        operand = _expressionStack.Pop();
    }

    private void PopBinary(Token token, out Expression left, out Expression right)
    {
        if (_expressionStack.Count < 2)
        {
            throw new SealException(token,
                $"Binary operator {token.TokenType} expected 2 operands, got {_expressionStack.Count}.");
        }

        right = _expressionStack.Pop();
        left = _expressionStack.Pop();
    }
    
    private UnaryExpression ParseUnaryExpression(Token token, UnaryType unaryType)
    {
        PopUnary(token, out Expression operand);
        return new UnaryExpression(unaryType, operand);
    }
    
    private MemberFieldExpression ParseMemberFieldExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        
        if (right is not VariableExpression variableExpression)
        {
            throw new SealException(token,
                $"Expected left-hand side of Member get operator to be an identifier, got {right.GetType().Name}.");
        }

        return new MemberFieldExpression(left, variableExpression.Identifier);
    }

    private ArithmeticExpression ParseBinaryArithmeticExpression(Token token, ArithmeticType arithmeticType)
    {
        PopBinary(token, out Expression left, out Expression right);
        return new ArithmeticExpression(arithmeticType, left, right);
    }
    
    private ComparisonExpression ParseComparisonExpression(Token token, ComparisonType comparisonType)
    {
        PopBinary(token, out Expression left, out Expression right);
        return new ComparisonExpression(comparisonType, left, right);
    }
    
    private ShortcutAndExpression ParseBoolAnd(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        return new ShortcutAndExpression(left, right);
    }
    
    private ShortcutOrExpression ParseBoolOr(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);
        return new ShortcutOrExpression(left, right);
    }

    private AssignExpression ParseAssignExpression(Token token)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression assignable)
        {
            throw new SealException(token,
                $"Left-hand side of assignment {left.GetType().Name} is not assignable.");
        }

        return new AssignExpression(assignable, right);
    }
    
    private CompoundArithmeticExpression ParseCompoundArithmeticExpression(Token token, ArithmeticType arithmeticType)
    {
        PopBinary(token, out Expression left, out Expression right);

        if (left is not AssignableExpression assignable)
        {
            throw new SealException(token,
                $"Left-hand side of compount operator {arithmeticType} must be assignable.");
        }

        return new CompoundArithmeticExpression(arithmeticType, assignable, right);
    }

    private void ParseOpenParen(Token token)
    {
        if (_stream.Position > 1 && IsCallable(_stream[_stream.Position - 2]))
        {
            ParseInvokeExpression(token);
        }
        else
        {
            _operatorStack.Push(token);
            _bracketDepth++;
        }
    }

    private void ParseInvokeExpression(Token token)
    {
        FlushPrecedence(SealConfig.MaxPrecedence);
        
        PopUnary(token, out Expression functionExpression);
        
        var argumentExpressions = new List<Expression>();

        var argumentParser = new ExpressionParser(_stream, ExpressionParsingMode.Argument);
        
        if (!_stream.TryConsume(TokenType.CloseParen))
        {
            while (!_stream.EndOfStream)
            {
                argumentExpressions.Add(argumentParser.Parse());

                if (_stream.Peek().TokenType == TokenType.CloseParen)
                {
                    break;
                }

                _stream.Consume(TokenType.Comma);
            }

            _stream.Consume(TokenType.CloseParen);
        }

        var expression = new InvokeExpression(functionExpression, argumentExpressions.ToArray());
        
        _expressionStack.Push(expression);
    }
    
    private void ParseOpenSquare(Token token)
    {
        if (_stream.Position > 1 && IsCallable(_stream[_stream.Position - 2]))
        {
            ParseIndexExpression(token);
        }
        else
        {
            ParseArrayExpression();
        }
    }
    
    private void ParseIndexExpression(Token token)
    {
        FlushPrecedence(SealConfig.MaxPrecedence);
        
        PopUnary(token, out Expression instanceExpression);

        var valueParser = new ExpressionParser(_stream, ExpressionParsingMode.Argument);
        
        Expression indexExpression = valueParser.Parse();

        _stream.Consume(TokenType.CloseSquare);

        var expression = new IndexerExpression(instanceExpression, indexExpression);
        
        _expressionStack.Push(expression);
    }

    private void ParseArrayExpression()
    {
        if (_stream.TryConsume(TokenType.CloseSquare))
        {
            _expressionStack.Push(new ArrayExpression());
            return;   
        }
        
        var itemExpressions = new List<Expression>();
        
        var itemParser = new ExpressionParser(_stream, ExpressionParsingMode.Argument);

        while (!_stream.EndOfStream)
        {
            itemExpressions.Add(itemParser.Parse());

            if (_stream.Peek().TokenType == TokenType.CloseSquare)
            {
                break;
            }

            _stream.Consume(TokenType.Comma);
        }

        _stream.Consume(TokenType.CloseSquare);

        var expression = new ArrayExpression(itemExpressions.ToArray());
        
        _expressionStack.Push(expression);
    }
    
    private void ParseMapExpression()
    {
        if (_stream.TryConsume(TokenType.CloseBrace))
        {
            _expressionStack.Push(new MapExpression());
            return;
        }
        
        var itemExpressions = new Dictionary<Expression, Expression>();
        
        var itemParser = new ExpressionParser(_stream, ExpressionParsingMode.Argument);
        
        while (!_stream.EndOfStream)
        {
            Expression keyExpression = itemParser.Parse();

            _stream.Consume(TokenType.Colon);
            
            Expression valueExpression = itemParser.Parse();
            
            itemExpressions.Add(keyExpression, valueExpression);

            if (_stream.Peek().TokenType == TokenType.CloseBrace)
            {
                break;
            }

            _stream.Consume(TokenType.Comma);

            // Allow trailing comma
            if (_stream.Peek().TokenType == TokenType.CloseBrace)
            {
                break;
            }
        }

        _stream.Consume(TokenType.CloseBrace);
        
        _expressionStack.Push(new MapExpression(itemExpressions));
    }
    
    private void ParseFunctionDefinition()
    {
        FunctionDefinition definition = new StatementParser(_stream).ParseFunctionDefinition();

        _expressionStack.Push(new FunctionDefinitionExpression(definition));
    }

    private void ParseClassDefinition()
    {
        ClassDefinition definition = new StatementParser(_stream).ParseClassDefinition();

        _expressionStack.Push(new ClassDefinitionExpression(definition));
    }
}
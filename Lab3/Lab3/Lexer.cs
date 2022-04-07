using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class Lexer
    {
        public string Code { get; init; }
        public List<Token> Tokens;
        public int currentPosition { get; set; }
        public int peekPosition { get; set; }
        public Lexer(string code)
        {
            currentPosition = 0;
            peekPosition = 1;
            Code = code;
            Tokens = new();
        }
        public List<Token> GetTokens()
        {
            while (currentPosition < Code.Length)
            {
                CheckCharacter();
            }
            Tokens.Add(new Token(Token.TokenType.EOF, ""));
            return Tokens;
        }
        public void CheckCharacter()
        {
            char currentChar = Code[currentPosition];
            Token token = null;
            switch (currentChar)
            {
                case '+':
                    if (Peek() == '+')
                    {
                        token = new Token(Token.TokenType.INCREMENT, "++");
                        MovePosition();
                    }
                    else
                    {
                        token = new Token(Token.TokenType.PLUS, "+");
                    }
                    break;
                case '-':
                    if (Peek() == '-')
                    {
                        token = new Token(Token.TokenType.DECREMENT, "--");
                        MovePosition();
                    }
                    else
                    {
                        token = new Token(Token.TokenType.MINUS, "-");
                    }
                    break;
                case '=':
                    if (Peek() == '=')
                    {
                        token = new Token(Token.TokenType.EQUAL, "==");
                        MovePosition();
                    }
                    else
                    {
                        token = new Token(Token.TokenType.ASSIGN, "=");
                    }
                    break;
                case '!':
                    if (Peek() == '=')
                    {
                        token = new Token(Token.TokenType.NOT_EQUAL, "!=");
                        MovePosition();
                    }
                    else token = new Token(Token.TokenType.NOT, "!");
                    break;
                case '>':
                    if (Peek() == '=')
                    {
                        token = new Token(Token.TokenType.GREAT_EQUAL, ">=");
                        MovePosition();
                    }
                    else token = new Token(Token.TokenType.GREAT, ">");
                    break;
                case '<':
                    if (Peek() == '=')
                    {
                        token = new Token(Token.TokenType.LESS_EQUAL, "<=");
                        MovePosition();
                    }
                    else token = new Token(Token.TokenType.LESS, "<");
                    break;
                case '&':
                    if (Peek() == '&')
                    {
                        token = new Token(Token.TokenType.AND, "&&");
                        MovePosition();
                    }
                    else token = new Token(Token.TokenType.INVALID, "&");
                    break;
                case '|':
                    if (Peek() == '|')
                    {
                        token = new Token(Token.TokenType.OR, "||");
                        MovePosition();
                    }
                    else token = new Token(Token.TokenType.INVALID, "|");
                    break;
                case '(':
                    token = new Token(Token.TokenType.LEFTPAR, "(");
                    break;
                case ')':
                    token = new Token(Token.TokenType.RIGHTPAR, ")");
                    break;
                case '{':
                    token = new Token(Token.TokenType.LEFTBRACE, "{");
                    break;
                case '}':
                    token = new Token(Token.TokenType.RIGHTBRACE, "}");
                    break;
                case ',':
                    token = new Token(Token.TokenType.COMMA, ",");
                    break;
                case '*':
                    token = new Token(Token.TokenType.ASTERISK, "*");
                    break;
                case '/':
                    token = new Token(Token.TokenType.SLASH, "/");
                    break;
                case ';':
                    token = new Token(Token.TokenType.SEMICOLON, ";");
                    break;
                case '"':
                    ReadWord();
                    break;
                case ' ': case '\r': case '\t': case'\n': 
                    break;
                default:
                    if (char.IsLetter(currentChar))
                    {
                        ReadWord();
                    }
                    else if (char.IsDigit(currentChar))
                    {
                        Number();
                    }
                    else
                    {
                        token = new Token(Token.TokenType.INVALID, char.ToString(currentChar));
                    }
                    break;
            }
            MovePosition();
            if (token != null)
            {
                Tokens.Add(token);
            }
        }
        public char Peek()
        {
            if (peekPosition >= Code.Length) return '\0';
            return Code[peekPosition];
        }
        public void MovePosition()
        {
            currentPosition = peekPosition;
            if (peekPosition >= Code.Length)
                return;
            peekPosition++;
        }
        public void Number()
        {
            int start = currentPosition;
            while (char.IsDigit(Peek()))
                MovePosition();

            if (Peek() == '.')
            {
                MovePosition();
                while (char.IsDigit(Peek()))
                    MovePosition();
                Tokens.Add(new Token(Token.TokenType.FLOAT_LITERAL, Code.Substring(start, currentPosition + 1 - start)));
            }
            else
            {
                Tokens.Add(new Token(Token.TokenType.INT_LITERAL, Code.Substring(start, currentPosition + 1 - start)));
            }
        }

        public void ReadWord()
        {
            int start = currentPosition;
            while (char.IsLetter(Peek()) || char.IsDigit(Peek()))
                MovePosition();
            string text = Code.Substring(start, currentPosition + 1 - start);
            if (Token.ReservedWords.ContainsKey(text)) Tokens.Add(new Token(Token.ReservedWords[text], text));
            else Tokens.Add(new Token(Token.TokenType.IDENTIFIER, text));
        }
    }
}

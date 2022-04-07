using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class Token
    {
        public string Value { get; set; }
        public TokenType Type { get; set; }
        public enum TokenType
        {
            IDENTIFIER,
            BOOL, TRUE, FALSE,
            INTEGER, INT_LITERAL,
            FLOAT, FLOAT_LITERAL,
            STRING, STRING_LITERAL,
            LEFTPAR, RIGHTPAR,
            LEFTBRACE, RIGHTBRACE,
            COMMA, SEMICOLON,
            SLASH, ASTERISK,
            PLUS, MINUS,
            INCREMENT, DECREMENT,
            NOT, NOT_EQUAL,
            ASSIGN, EQUAL,
            GREAT, GREAT_EQUAL,
            LESS, LESS_EQUAL,
            AND, OR,
            IF, ELSE,
            FUNCTION, RETURN,
            BREAK,
            FOR, WHILE,
            LOG, VOID,
            INVALID,
            EOF
        }
        public static Dictionary<string, TokenType> ReservedWords = new()
        {
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "function", TokenType.FUNCTION },
            { "return", TokenType.RETURN},
            { "for", TokenType.FOR },
            { "while", TokenType.WHILE },
            { "int", TokenType.INTEGER },
            { "float", TokenType.FLOAT },
            { "string", TokenType.STRING },
            { "bool", TokenType.BOOL },
            { "void", TokenType.VOID },
            { "log", TokenType.LOG },
            { "break", TokenType.BREAK }

        };
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
        public override string ToString()
        {
            return $"[{Type} {Value}]";
        }
    }
}

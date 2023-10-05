using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    INT, PlusOp, AssignmentOP, LessThanOp, AndOp, Lparanthesis, FLOAT, MinusOp, GearterThanOp, OrOp, Rparanthesis,
    STRING, MultiplyOp, EqualOp, SemiColon, READ, NotEqualOp, DivideOp, Comma, WRITE, REPEAT, NotMark, UNTIL,
    IF, ELSEIF, ELSE, THEN, RETURN, ENDL, Lbracket, Rbracket, END, Idenifier, Number, String_value , MAIN
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>(); 
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {

            ReservedWords.Add("end", Token_Class.END);
            ReservedWords.Add("int", Token_Class.INT);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("string", Token_Class.STRING);
            ReservedWords.Add("read", Token_Class.READ);
            ReservedWords.Add("write", Token_Class.WRITE);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("then", Token_Class.THEN);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("endl", Token_Class.ENDL);
            ReservedWords.Add("main", Token_Class.MAIN);

            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);

            Operators.Add(";", Token_Class.SemiColon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("{", Token_Class.Lparanthesis);
            Operators.Add("}", Token_Class.Rparanthesis);
            Operators.Add(":=", Token_Class.AssignmentOP);
            Operators.Add("(", Token_Class.Lbracket);
            Operators.Add(")", Token_Class.Rbracket);

            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GearterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);

            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            
            


        }

        public void StartScanning(string SourceCode)
    {
            Errors.Error_List.Clear();
            char st_quotes = '\"';
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (CurrentChar == '/')
                {
                    if (SourceCode[i + 1] == '*')
                    {
                        for (int k = i + 2; k < SourceCode.Length; k++)
                        {
                            if (SourceCode[k].Equals('*') && SourceCode[k + 1].Equals('/'))
                            {
                                i = k + 2;
                                break;
                            }
                            else
                            {

                                if (k == SourceCode.Length - 2)
                                {
                                    Errors.Error_List.Add(" comment unclosed error");
                                    i = k + 1;
                                }
                            }

                        }
                    }
                    else
                    {
                        CurrentLexeme.PadRight(1, CurrentChar);
                        FindTokenClass(CurrentLexeme);
                    }
                }
                else if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    for (int k = i + 1; k < SourceCode.Length; k++)
                    {
                        if (char.IsLetterOrDigit(SourceCode[k]))
                        {
                            CurrentLexeme += SourceCode[k];
                        }
                        else
                        {
                            FindTokenClass(CurrentLexeme);
                            i = k - 1;
                            break;
                        }
                    }
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {

                    for (int k = i + 1; k < SourceCode.Length; k++)
                    {
                        if (char.IsDigit(SourceCode[k]) || SourceCode[k] == '.')
                        {
                            CurrentLexeme += SourceCode[k];
                        }
                        else
                        {
                            if (CurrentLexeme.Contains(".."))
                            {
                                Errors.Error_List.Add(CurrentLexeme + " wrong number shape error");
                                i = k - 1;
                                break;
                            }
                            FindTokenClass(CurrentLexeme);
                            i = k - 1;
                            break;
                        }
                    }
                }
                else if (CurrentChar.Equals(st_quotes))
                {
                    // CurrentLexeme += CurrentChar;
                    for (int k = i + 1; k < SourceCode.Length; k++)
                    {
                        if (SourceCode[k].Equals(st_quotes))
                        {
                            CurrentLexeme += SourceCode[k];
                            FindTokenClass(CurrentLexeme);
                            i = k;
                            break;
                        }
                        else
                        {
                            if (k == SourceCode.Length - 1)
                            {
                                CurrentLexeme = "\"";
                                Errors.Error_List.Add(CurrentLexeme + " string unclosed error");

                            }
                            CurrentLexeme += SourceCode[k];
                        }

                    }
                }

                // handling double character operators
                else
                {
                    if (CurrentChar == '&' && SourceCode[i + 1] == '&')
                    {
                        CurrentLexeme = "&&";
                        i += 2;
                        FindTokenClass(CurrentLexeme);
                    }
                    else if (CurrentChar == '|' && SourceCode[i + 1] == '|')
                    {
                        CurrentLexeme = "||";
                        i += 1;
                        FindTokenClass(CurrentLexeme);
                    }
                    else if (CurrentChar == ':' && SourceCode[i + 1] == '=')
                    {
                        CurrentLexeme = ":=";
                        i += 1;
                        FindTokenClass(CurrentLexeme);
                    }
                    else if (CurrentChar == '<' && SourceCode[i + 1] == '>')
                    {
                        CurrentLexeme = "<>";
                        i += 1;
                        FindTokenClass(CurrentLexeme);
                    }
                    else
                    {
                        CurrentLexeme.PadRight(1, CurrentChar);
                        FindTokenClass(CurrentLexeme);
                    }
                }

            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            // Is it a string value
            else if (isString_value(Lex))
            {
                Tok.token_type = Token_Class.String_value;
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {

                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it a Constant
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex + " undefined token error");
            }

        }

        bool isIdentifier(string lex)
        {
            bool isValid = false;
            Regex re = new Regex(@"[a-zA-Z]([a-zA-Z]|[0-9])*", RegexOptions.Compiled);
            if (re.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isNumber(string lex)
        {
            bool isValid = false;
            Regex re = new Regex(@"^([0-9])+(\.([0-9])+)?$", RegexOptions.Compiled);
            if (re.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isString_value(string lex)
        {
            bool isValid = false;
            Regex re = new Regex(@"\""(.*)\""", RegexOptions.Compiled);
            if (re.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }



    }
}

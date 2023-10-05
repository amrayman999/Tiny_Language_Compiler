using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string Name)
        {
            this.Name = Name;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Functions());
            root.Children.Add(Main_Function());        
            return root;
        }
        // Done //  
        Node Functions()
        {
            Node Functions1 = new Node("Functions");
            if (((TokenStream[InputPointer].token_type == Token_Class.INT || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.STRING)
                && TokenStream[InputPointer + 1].token_type != Token_Class.MAIN) && InputPointer < TokenStream.Count())
            {
                Functions1.Children.Add(Function_Statement());
                Functions1.Children.Add(Functions_Dash());
                return Functions1;
            }
            return null;
        }
        // Done //
        Node Functions_Dash()
        {
            Node Functions_Dash1 = new Node("Functions_Dash");
            if (((TokenStream[InputPointer].token_type == Token_Class.INT || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.STRING)
                && TokenStream[InputPointer + 1].token_type != Token_Class.MAIN) && InputPointer < TokenStream.Count())
            {

                Functions_Dash1.Children.Add(Function_Statement());
                Functions_Dash1.Children.Add(Functions_Dash());
                return Functions_Dash1;
            }
            return null;
        }
        // Done //
        Node Main_Function()
        {

            Node Main_Function = new Node("Main_Function");
            Main_Function.Children.Add(Data_Type());
            Main_Function.Children.Add(match(Token_Class.MAIN));
            Main_Function.Children.Add(match(Token_Class.Lbracket));
            Main_Function.Children.Add(match(Token_Class.Rbracket));
            Main_Function.Children.Add(Function_Body());
            return Main_Function;


        }
        // Done //
        Node Data_Type()
        {
            Node Data_Type = new Node("Data_Type");
            if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.INT)
            {
                Data_Type.Children.Add(match(Token_Class.INT));
                return Data_Type;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.FLOAT)
            {
                Data_Type.Children.Add(match(Token_Class.FLOAT));
                return Data_Type;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.STRING)
            {
                Data_Type.Children.Add(match(Token_Class.STRING));
                return Data_Type;
            }
            return Data_Type;
        }        
        // Done //
        Node Parameters()
        {
            Node Parameters = new Node("Parameters");
            if ((TokenStream[InputPointer].token_type == Token_Class.INT || TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.STRING) && InputPointer < TokenStream.Count())
            {
                Parameters.Children.Add(Parameter());
                Parameters.Children.Add(Parameters_Dash());
                return Parameters;
            }
            return null;
        }
        // Done //
        Node Parameters_Dash()
        {
            Node Parameters_Dash1 = new Node("Parameters_Dash");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count())
            {
                Parameters_Dash1.Children.Add(match(Token_Class.Comma));
                Parameters_Dash1.Children.Add(Parameter());
                Parameters_Dash1.Children.Add(Parameters_Dash());
                return Parameters_Dash1;
            }
            return null;

        }
        // Done //
        Node Statements()
        {
            Node Statements = new Node("Statements");        
            Statements.Children.Add(Statement());
            Statements.Children.Add(Statements_Dash());
            return Statements;                
        }
        // Done //
        Node Statements_Dash()
        {
            Node Statements_Dash1 = new Node("Statements_Dash");
            if ((TokenStream[InputPointer].token_type == Token_Class.READ || TokenStream[InputPointer].token_type == Token_Class.REPEAT ||
                TokenStream[InputPointer].token_type == Token_Class.IF || TokenStream[InputPointer].token_type == Token_Class.WRITE ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.INT ||
                TokenStream[InputPointer].token_type == Token_Class.FLOAT || 
                TokenStream[InputPointer].token_type == Token_Class.STRING) && InputPointer < TokenStream.Count())
            {

                Statements_Dash1.Children.Add(Statement());
                Statements_Dash1.Children.Add(Statements_Dash());
                return Statements_Dash1;

            }
            return null;

        }
        // Done //
        Node Statement()
        {
            Node Statement = new Node("Statement");
          
            if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                Statement.Children.Add(If_Statement());
                return Statement;
            }
            else if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.REPEAT)
            {
                Statement.Children.Add(Repeat_Statement());
                return Statement;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.READ)
            {
                Statement.Children.Add(Read_Statement());
                return Statement;
            }
            else if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.WRITE)
            {
                Statement.Children.Add(Write_Statement());
                return Statement;
            }
            else if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {              
                Statement.Children.Add(Assignment_Statement());
                Statement.Children.Add(match(Token_Class.SemiColon));
                return Statement;
            }
            else if((TokenStream[InputPointer].token_type == Token_Class.INT || 
                    TokenStream[InputPointer].token_type == Token_Class.FLOAT || 
                    TokenStream[InputPointer].token_type == Token_Class.STRING) 
                    && InputPointer < TokenStream.Count())
            {
                Statement.Children.Add(Declaration_Statement());
                return Statement;
            }
            else
            {
                return null;
            }

        }
        Node Statements_else_if_elseif()
        {
            Node Statements_else_if_elseif = new Node("Statements_else_if_elseif");
            Statements_else_if_elseif.Children.Add(Statement_if_else_elseif());
            Statements_else_if_elseif.Children.Add(Statements_if_else_elseif_Dash());
            return Statements_else_if_elseif;
        }
        // Done //
        Node Statements_if_else_elseif_Dash()
        {
            Node Statements_if_else_elseif_Dash1 = new Node("Statements_if_else_elseif_Dash");
            if ((TokenStream[InputPointer].token_type == Token_Class.READ || TokenStream[InputPointer].token_type == Token_Class.REPEAT ||
                TokenStream[InputPointer].token_type == Token_Class.IF || TokenStream[InputPointer].token_type == Token_Class.WRITE ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.INT ||
                TokenStream[InputPointer].token_type == Token_Class.FLOAT || TokenStream[InputPointer].token_type == Token_Class.RETURN ||
                TokenStream[InputPointer].token_type == Token_Class.STRING) && InputPointer < TokenStream.Count())
            {

                Statements_if_else_elseif_Dash1.Children.Add(Statement_if_else_elseif());
                Statements_if_else_elseif_Dash1.Children.Add(Statements_if_else_elseif_Dash());
                return Statements_if_else_elseif_Dash1;

            }
            return null;

        }
        // Done //
        Node Statement_if_else_elseif()
        {
            Node Statement_if_else_elseif = new Node("Statement_if_else_elseif");

            if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                Statement_if_else_elseif.Children.Add(If_Statement());
                return Statement_if_else_elseif;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.REPEAT)
            {
                Statement_if_else_elseif.Children.Add(Repeat_Statement());
                return Statement_if_else_elseif;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.READ)
            {
                Statement_if_else_elseif.Children.Add(Read_Statement());
                return Statement_if_else_elseif;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.WRITE)
            {
                Statement_if_else_elseif.Children.Add(Write_Statement());
                return Statement_if_else_elseif;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                Statement_if_else_elseif.Children.Add(Assignment_Statement());
                Statement_if_else_elseif.Children.Add(match(Token_Class.SemiColon));
                return Statement_if_else_elseif;
            }
            else if ((TokenStream[InputPointer].token_type == Token_Class.INT ||
                    TokenStream[InputPointer].token_type == Token_Class.FLOAT ||
                    TokenStream[InputPointer].token_type == Token_Class.STRING)
                    && InputPointer < TokenStream.Count())
            {
                Statement_if_else_elseif.Children.Add(Declaration_Statement());
                return Statement_if_else_elseif;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.RETURN)
            {
                Statement_if_else_elseif.Children.Add(Return_Statement());
                return Statement_if_else_elseif;
            }
            else
            {
                return null;
            }

        }
        // Done //
        Node Assignment_Statement()
        {
            Node Assignment_Statement = new Node("Assignment_Statement");
            Assignment_Statement.Children.Add(match(Token_Class.Idenifier));
            Assignment_Statement.Children.Add(match(Token_Class.AssignmentOP));
            Assignment_Statement.Children.Add(Expression());
            // Assignment_Statement.Children.Add(match(Token_Class.SemiColon));
            return Assignment_Statement;

        }
        // Done //
        Node Return_Statement()
        {
            Node Return_Statement = new Node("Return_Statement");
            Return_Statement.Children.Add(match(Token_Class.RETURN));
            Return_Statement.Children.Add(Expression());
            Return_Statement.Children.Add(match(Token_Class.SemiColon));
            return Return_Statement;

        }
        // Done //
        Node Write_Statement()
        {
            Node Write_Statement= new Node("Write_Statement");
            Write_Statement.Children.Add(match(Token_Class.WRITE));
            Write_Statement.Children.Add(Write_Statement_Dash());
            return Write_Statement;

        }
        // Done //
        Node Write_Statement_Dash()
        {
            Node Write_Statement_Dash = new Node("Write_Statement_Dash");
            if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.ENDL)
            {
                Write_Statement_Dash.Children.Add(match(Token_Class.ENDL));
                Write_Statement_Dash.Children.Add(match(Token_Class.SemiColon));
                return Write_Statement_Dash;
            }
            else 
            {
                Write_Statement_Dash.Children.Add(Expression());
                Write_Statement_Dash.Children.Add(match(Token_Class.SemiColon));
                return Write_Statement_Dash;
            }
            

        }
        // Done //
        Node If_Statement()
        {
            Node If_Statement = new Node("If_Statement");
            If_Statement.Children.Add(match(Token_Class.IF));
            If_Statement.Children.Add(Condition_Statement());
            If_Statement.Children.Add(match(Token_Class.THEN));
            If_Statement.Children.Add(Statements_else_if_elseif());
            If_Statement.Children.Add(If_Statement_Dash());
            If_Statement.Children.Add(match(Token_Class.END));
            return If_Statement;

        }
        // Done //
        Node If_Statement_Dash()
        {
            Node If_Statement_Dash1 = new Node("If_Statement_Dash ");
            if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                If_Statement_Dash1.Children.Add(Else_If_Statement());
                If_Statement_Dash1.Children.Add(If_Statement_Dash());
                return If_Statement_Dash1;
            }
            else if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.ELSE)
            {
                If_Statement_Dash1.Children.Add(Else_Statement());
                If_Statement_Dash1.Children.Add(If_Statement_Dash());
                return If_Statement_Dash1;
            }
            return null;

        }   
        // Done //
        Node Condition_Statement()
        {
            Node Condition_Statement = new Node("Condition_Statement");
            Condition_Statement.Children.Add(Condition());
            Condition_Statement.Children.Add(Condition_Statement_Dash());
            return Condition_Statement;

        }      
        // Done //
        Node Condition_Statement_Dash()
        {
            Node Condition_Statement_Dash1 = new Node("Condition_Statement_Dash");
            if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                Condition_Statement_Dash1.Children.Add(match(Token_Class.AndOp));
                Condition_Statement_Dash1.Children.Add(Condition());
                Condition_Statement_Dash1.Children.Add(Condition_Statement_Dash());
                return Condition_Statement_Dash1;
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                Condition_Statement_Dash1.Children.Add(match(Token_Class.OrOp));
                Condition_Statement_Dash1.Children.Add(Condition());
                Condition_Statement_Dash1.Children.Add(Condition_Statement_Dash());
                return Condition_Statement_Dash1;
            }
            
            return null;

        }
        // Done //
        Node Condition()
        {
            Node Condition = new Node("Condition");
            Condition.Children.Add(match(Token_Class.Idenifier));
            if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                Condition.Children.Add(match(Token_Class.EqualOp));
            }
            else if(InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                Condition.Children.Add(match(Token_Class.NotEqualOp));
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.GearterThanOp)
            {
                Condition.Children.Add(match(Token_Class.GearterThanOp));
            }
            else if (InputPointer < TokenStream.Count() && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                Condition.Children.Add(match(Token_Class.LessThanOp));
            }
            Condition.Children.Add(Term());
            return Condition;

        }
        // Done //
        Node Function_Statement()
        {
            Node Function_Statement = new Node("Function_Statement");
                Function_Statement.Children.Add(Function_Declaration());
                Function_Statement.Children.Add(Function_Body());
                return Function_Statement;
        }
        // Done //
        Node Repeat_Statement()
        {
            Node Repeat_Statement = new Node("Repeat_Statement");
            Repeat_Statement.Children.Add(match(Token_Class.REPEAT));
            Repeat_Statement.Children.Add(Statements());
            Repeat_Statement.Children.Add(match(Token_Class.UNTIL));
            Repeat_Statement.Children.Add(Condition_Statement());
            return Repeat_Statement;

        }
        // Done //
        Node Read_Statement()
        {
            Node Read_Statement = new Node("Read_Statement");
            Read_Statement.Children.Add(match(Token_Class.READ));
            Read_Statement.Children.Add(match(Token_Class.Idenifier));
            Read_Statement.Children.Add(match(Token_Class.SemiColon));
            return Read_Statement;

        }
        // Done //
        Node Else_Statement()
        {
            Node Else_Statement = new Node("Else_Statement");
            Else_Statement.Children.Add(match(Token_Class.ELSE));
            Else_Statement.Children.Add(Statements_else_if_elseif());
            return Else_Statement;

        }
        // Done //
        Node Else_If_Statement()
        {
            Node Else_If_Statement = new Node("Else_If_Statement");
            Else_If_Statement.Children.Add(match(Token_Class.ELSEIF));
            Else_If_Statement.Children.Add(Condition_Statement());
            Else_If_Statement.Children.Add(match(Token_Class.THEN));
            Else_If_Statement.Children.Add(Statements_else_if_elseif());
            
            return Else_If_Statement;

        }
        // Done // 
        Node Function_Body()
        {
            Node Function_Body = new Node("Function_Body");
            Function_Body.Children.Add(match(Token_Class.Lparanthesis));
            Function_Body.Children.Add(Statements());
            Function_Body.Children.Add(Return_Statement());
            Function_Body.Children.Add(match(Token_Class.Rparanthesis));
            return Function_Body;

        }
        // Done //
        Node Function_Declaration()
        {
            Node Function_Declaration = new Node("Function_Declaration");
            Function_Declaration.Children.Add(Data_Type());
            Function_Declaration.Children.Add(match(Token_Class.Idenifier));
            Function_Declaration.Children.Add(match(Token_Class.Lbracket));
            Function_Declaration.Children.Add(Parameters());
            Function_Declaration.Children.Add(match(Token_Class.Rbracket));
            return Function_Declaration;

        }
        // Done //
        Node Parameter()
        {
            Node Parameter = new Node("Parameter");
            Parameter.Children.Add(Data_Type());
            Parameter.Children.Add(match(Token_Class.Idenifier));
            return Parameter;

        }
        // Done //
        Node Function_Call()
        {
            Node Function_Call = new Node("Function_Call");
            Function_Call.Children.Add(match(Token_Class.Idenifier));
            Function_Call.Children.Add(match(Token_Class.Lbracket));
            Function_Call.Children.Add(ArgList());
            Function_Call.Children.Add(match(Token_Class.Rbracket));
            return Function_Call;

        }
        // Done //
        Node ArgList() 
        {
           Node ArgList = new Node("ArgList");
           if ((TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.Number ||
                (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                 TokenStream[InputPointer + 1].token_type == Token_Class.Lbracket)) 
                 && InputPointer < TokenStream.Count())
           {
               ArgList.Children.Add(Arguments());
                return ArgList;
           }
            
           return null;
        }
        // Done //
        Node Arguments()
        {
            Node Arguments = new Node("Arguments");
            if ((TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.Number ||
                (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                 TokenStream[InputPointer + 1].token_type == Token_Class.Lbracket)) 
                 && InputPointer < TokenStream.Count())
            {
                Arguments.Children.Add(Term());
                Arguments.Children.Add(Arguments_Dash());
                return Arguments;
            }
            return null;
        }
        // Done //
        Node Arguments_Dash() 
        {
            Node Arguments_Dash1 = new Node("Arguments_Dash");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count())
            {
                Arguments_Dash1.Children.Add(match(Token_Class.Comma));
                Arguments_Dash1.Children.Add(Term());
                Arguments_Dash1.Children.Add(Arguments_Dash());
                return Arguments_Dash1;
            }
            return null;
        }
        // Done //
        Node Term()
        {
            Node Term = new Node("Term");
            if(TokenStream[InputPointer].token_type == Token_Class.Number && InputPointer < TokenStream.Count())
            {
                Term.Children.Add(match(Token_Class.Number));
                return Term;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count())
            {
                if(TokenStream[InputPointer + 1].token_type == Token_Class.Lbracket)
                {
                    Term.Children.Add(Function_Call());
                    return Term;
                }
                Term.Children.Add(match(Token_Class.Idenifier));
                return Term;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Lbracket && InputPointer < TokenStream.Count())
            {
                Term.Children.Add(match(Token_Class.Lbracket));
                Term.Children.Add(Equation());
                Term.Children.Add(match(Token_Class.Rbracket));
                return Term;
            }
           
            
            return Term;

        }
        // Done //
        Node Expression()
        {
            Node Expression = new Node("Expression");
            if(TokenStream[InputPointer].token_type == Token_Class.String_value && InputPointer < TokenStream.Count())
            {
                Expression.Children.Add(match(Token_Class.String_value));
                return Expression;
            }
            /*
            else if((TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Lbracket)
                      && InputPointer < TokenStream.Count())
            {
                if(TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp)
                {
                    Expression.Children.Add(Equation());
                    return Expression;
                }
                Expression.Children.Add(Term());
                return Expression;
            }
            return Expression;
            */
            else if((TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Idenifier) 
                && TokenStream[InputPointer + 1].token_type == Token_Class.SemiColon && InputPointer < TokenStream.Count())
            {
                Expression.Children.Add(Term());
                return Expression;
            }
            else
            {
                Expression.Children.Add(Equation());
                return Expression;
            }

        }
        // Done //
        Node Equation() 
        {
            Node Equation = new Node("Equation");
            Equation.Children.Add(Terms());
            Equation.Children.Add(Exp());
            return Equation;
        }
        // Done //
        Node Exp()
        {
            Node Exp1 = new Node("Exp");
            if ((TokenStream[InputPointer].token_type == (Token_Class.PlusOp) || TokenStream[InputPointer].token_type == (Token_Class.MinusOp))
                 && InputPointer < TokenStream.Count())
            {
                Exp1.Children.Add(AddOp());
                Exp1.Children.Add(Terms());
                Exp1.Children.Add(Exp());
                return Exp1;
            }


            return null;
        }
        // Done //
        Node Ter()
        {
            Node Ter1 = new Node("Ter");
            if ((TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                TokenStream[InputPointer].token_type == Token_Class.DivideOp) && InputPointer < TokenStream.Count())
            {
                Ter1.Children.Add(MultiOp());
                Ter1.Children.Add(Term());
                Ter1.Children.Add(Ter());
                return Ter1;
            }
            return Ter1;
        }
        // Done //
        Node Terms()
        {
            Node Terms = new Node("Terms");           
            Terms.Children.Add(Term());
            Terms.Children.Add(Ter());
            return Terms;
        }
        // Done //
        Node MultiOp()
        {
            Node MultiOp = new Node("MultiOp");
            if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp && InputPointer < TokenStream.Count())
            {
                MultiOp.Children.Add(match(Token_Class.MultiplyOp));
                return MultiOp;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp && InputPointer < TokenStream.Count())
            {
                MultiOp.Children.Add(match(Token_Class.DivideOp));
                return MultiOp;
            }
            return MultiOp;
        }
        // Done //
        Node AddOp()
        {
            Node AddOp = new Node("AddOp");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp && InputPointer < TokenStream.Count())
            {
                AddOp.Children.Add(match(Token_Class.PlusOp));
                return AddOp;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp && InputPointer < TokenStream.Count())
            {
                AddOp.Children.Add(match(Token_Class.MinusOp));
                return AddOp;
            }
            return AddOp;
        }
        // Done //
        Node Declaration_Statement()
        {
            Node Declaration_Statement = new Node("Declaration_Statement");
            Declaration_Statement.Children.Add(Data_Type());
            Declaration_Statement.Children.Add(Decl());
            Declaration_Statement.Children.Add(match(Token_Class.SemiColon));

            return Declaration_Statement;
        }
        // Done //
        Node Decl()
        {
            Node Decl = new Node("Decl");
            if((TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOP) && InputPointer < TokenStream.Count())
            {
                Decl.Children.Add(Assignment_Statement());
                Decl.Children.Add(Decl_Dash());
                return Decl;
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count())
            {
                Decl.Children.Add(match(Token_Class.Idenifier));
                Decl.Children.Add(Decl_Dash());
                return Decl;
            }
            return null;

        }
        // Done //
        Node Decl_Dash()
        {
            Node Decl_Dash1 = new Node("Decl_Dash");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count())
            {
                Decl_Dash1.Children.Add(match(Token_Class.Comma));
                Decl_Dash1.Children.Add(Decl_Dash_Dash());
                Decl_Dash1.Children.Add(Decl_Dash());
                return Decl_Dash1;
            }
            return null;      
        }
        // Done //
        Node Decl_Dash_Dash()
        {
            Node Decl_Dash_Dash1 = new Node("Decl_Dash_Dash");
            if ((TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                TokenStream[InputPointer+1].token_type == Token_Class.AssignmentOP) && InputPointer < TokenStream.Count())
            {
                Decl_Dash_Dash1.Children.Add(Assignment_Statement());
                return Decl_Dash_Dash1;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count())
            {
                Decl_Dash_Dash1.Children.Add(match(Token_Class.Idenifier));
                return Decl_Dash_Dash1;
            }
            return Decl_Dash_Dash1;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}

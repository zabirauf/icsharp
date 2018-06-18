

namespace iCSharp.Kernel.Shell
{
	using Common.Logging;
	using System.Collections.Generic;
	using Common.Serializer;
	using iCSharp.Messages;
	using NetMQ.Sockets;
	using iCSharp.Kernel.Helpers;
	using System.Text.RegularExpressions;
	using System.IO;
	using System.Reflection;
	using System;
	using System.Linq;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis;
    using System.Text;


    //using System.
    //using System.Linq.Expressions.Analyzer
    public class CompleteRequestHandler : IShellMessageHandler
	{
		private ILog logger;
		private readonly IMessageSender messageSender;

		public CompleteRequestHandler(ILog logger, IMessageSender messageSender)
		{
			this.logger = logger;
			this.messageSender = messageSender;
		}

		public struct methodCollection
		{
			public MethodDeclarationSyntax mds;
			public string className;
		}

		public struct variableCollection{
			public VariableDeclarationSyntax propertyDeclarationSyntax;
			public string className;
		}

		public struct catchCollection
		{
			public string modifier;
			public string className;
			public string name;
			public string documentation;
		}

		public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
		{
			CompleteRequest completeRequest = JsonSerializer.Deserialize<CompleteRequest>(message.Content);

			//string code = completeRequest.CodeCells[completeRequest.Selected_Cell_Index];

			string line = completeRequest.Line;


			//	this.logger.Info("original code:" + code);
			this.logger.Info("original line:" + line);

			//	code = Regex.Replace(code.Substring(1, code.Length - 2), @"\\n", "*");
			line = line.Substring(1, line.Length - 2);

			//	this.logger.Info("substring code:" + code);
			this.logger.Info("substring line:" + line);

			Regex returnType = new Regex(@"[string|int|void]");
			Regex methodName = new Regex(@"(\w)");

			int cur_pos = completeRequest.CursorPosition;

			Console.WriteLine("cur_pos " + cur_pos);
			Console.WriteLine("code_pos " + completeRequest.CodePosition);
			//Type type = typeof(code);


			//string newCode = code.

			line = line.Substring(0, cur_pos); //get substring of code from start to cursor position


			//string[] arrayOfKeywords = { "team", "tech", "te", "term", "tame", "tata" };

			List<CompleteReplyMatch> DirectiveMatches = new List<CompleteReplyMatch>();
			//DirectivesList(ref DirectiveMatches);

			List<string> listOfKeywords = new List<string>();

			//listOfKeywords.AddRange(arrayOfKeywords);

			List<CompleteReplyMatch> matches_ = new List<CompleteReplyMatch>();

			List<MethodMatch> methodMatches = new List<MethodMatch>();

			List<ClassDeclarationSyntax> classList = new List<ClassDeclarationSyntax>();
			List<methodCollection> methodList = new List<methodCollection>();
			List<MethodDeclarationSyntax> globalMethodList = new List<MethodDeclarationSyntax>();

			List<CompleteReplyMatch> methodMatchNames = new List<CompleteReplyMatch>();

			List<CompleteReplyMatch> classMatchNames = new List<CompleteReplyMatch>();

			List<CompleteReplyMatch> interfaceNames = new List<CompleteReplyMatch>();

			List<catchCollection> enumNames = new List<catchCollection>();

			List<catchCollection> structNames = new List<catchCollection>();

			List<catchCollection> propertyNames = new List<catchCollection>();

			List<variableCollection> variableNames = new List<variableCollection>();

			List<VariableMatch> variableMatches = new List<VariableMatch>();

			var syntax_code = Regex.Replace(completeRequest.CodeCells[0].Substring(1, completeRequest.CodeCells[0].Length - 2), @"\\n", "*");
			Console.WriteLine("code_size " + syntax_code);

			var tree = CSharpSyntaxTree.ParseText(syntax_code);

			var syntaxRoot = tree.GetRoot();


			//      var MyClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			//     var MyMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

			//  Console.WriteLine("ayyyyyyy" + MyClass.Identifier.ToString());
			//  Console.WriteLine("yaaaaaaay" + MyMethod.Identifier.ToString());

			// CATCH STUFF



			foreach (var codes in completeRequest.CodeCells)
			{
				var code = Regex.Replace(codes.Substring(1, codes.Length - 2), @"\\n", "*");

				//  CatchAllWords(ref matches_, code);
				//  CatchMethods(code, ref methodMatchNames, ref methodMatches);
				// VariableMatches(code, ref variableMatches);
			}

			//CatchAllWords(ref matches_, syntax_code);

			CatchClassMethods(tree, ref classList, ref methodList, completeRequest.CodePosition);

			catchInterfaces(syntaxRoot, ref interfaceNames, classList);
			catchEnums(syntaxRoot, ref enumNames, classList);
			catchStructs(syntaxRoot, ref structNames, classList);
			//catchProperties(syntaxRoot, ref propertyNames, classList);
			catchVariables(syntaxRoot, ref variableNames, classList);
			CatchMethods(syntaxRoot, ref methodList, classList);

			Console.WriteLine("Class List");

			foreach (var l in classList)
			{
				Console.WriteLine(l.Identifier);
			}

			Console.WriteLine("Method List");

			foreach (var i in methodList)
			{
				Console.WriteLine(i.className);
				//       Console.WriteLine(i.)

			}

			string currentclass = "global";
			foreach (var i in classList)
			{
				if ((i.Span.Start <= completeRequest.CodePosition) && (i.Span.End >= completeRequest.CodePosition))
				{
					currentclass = i.Identifier.ToString();
				}
			}

			Console.WriteLine("THE CURSOR IS AT " + currentclass);


			List<MethodDeclarationSyntax> MethList = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
			bool found = false;
			foreach (var m in MethList)
			{
				foreach (var i in methodList)
				{
					if (((m.Identifier).Equals(i.className)) && ((m.Parent).Equals(i.mds.Parent)))
					{
						found = true;
					}
				}

				if (!found)
				{
					globalMethodList.Add(m);
				}
				found = false;

			}





			Console.WriteLine("Global Method List");

			foreach (var i in globalMethodList)
			{
				Console.WriteLine(i.Identifier);
			}

			//globalMethodList = (globalMethodList.Distinct((IEnumerable<MethodDeclarationSyntax>) methodList)).ToList();

			Console.WriteLine("New Global Method List");

			foreach (var i in globalMethodList)
			{
				Console.WriteLine(i.Identifier);
			}



			//List<MethodDeclarationSyntax> intersection = methodList.Intersect((IEnumerable<>) globalMethodList);



			//CatchClasses(code, ref classMatchNames);

			VariableMatches(syntax_code, ref variableMatches);

			//string[] dirs = Directory.GetDirectories



			AddKeywordsToMatches(ref matches_);

			foreach (MethodMatch m in methodMatches)
			{
				//this.logger.Info(m.Name);
				//this.logger.Info("number of params = " + m.ParamList.Count);
				//this.logger.Info("param types");
				for (int i = 0; i < m.ParamList.Count; i++)
				{
					//this.logger.Info(m.ParamList[i]);
				}

			}

			//code = Regex.Replace(code, @"[^\w&^\.]", "*"); //replace all non word and dot characters with '*'

			Tuple<string, int> cursorInfo;
			cursorInfo = FindWordToAutoComplete(line);

			string cursorWord = cursorInfo.Item1;
			int cursorWordLength = cursorInfo.Item2;


			List<CompleteReplyMatch> matchesSign = new List<CompleteReplyMatch>();

			MakeMethodSignMatches(line, methodMatches, ref matchesSign);

			this.logger.Info("cursor word " + cursorWord);

			this.logger.Info("Before Removal");

			for (int j = matches_.Count - 1; j > -1; j--)
			{
				//this.logger.Info(matches_[j].Name);
			}

			//Console.WriteLine(cursorWord);

			//List<CompleteReplyMatch> EmptySpaceMatches = EmptySpaceMatchGenerator();

			RemoveNonMatches(ref matches_, cursorWord, line);

			this.logger.Info("After Removal");

			List<CompleteReplyMatch> FinalDirectives = new List<CompleteReplyMatch>();

			for (int j = matches_.Count - 1; j > -1; j--)
			{
				//this.logger.Info(matches_[j].Name);
			}
            /*
			if (line.Length > 0)
			{
				//Deals with directives
				if (line.StartsWith("using "))
				{
					Console.WriteLine("We're Starting with using!!!!!!!");
					DirectivesList(ref DirectiveMatches, line);

					var l = DirectiveMatches.GroupBy(i => i.Name).Select(group => group.First());

					foreach (var i in l)
					{
						CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
						{
							Name = i.Name,
							Documentation = i.Documentation,
							Value = "",
							Glyph = "directive"

						};
						FinalDirectives.Add(completeReplyMatch);
					}
					matches_ = FinalDirectives;

					RemoveNonMatches(ref matches_, cursorWord, line);
				}//deals with dots, should show everything apart from class names for that class
				else if (line[(line.Length - 1)].Equals('.'))
				{
					Console.WriteLine("in function and cursor word is " + cursorWord);
					matches_ = methodMatchNames;
					matches_.AddRange(classMatchNames);
					foreach (var i in variableMatches)
					{
						if (i.Name.Equals(cursorWord))
						{
							try
							{
								//Type type = Type.GetType(i.VarType);
								ShowMethods(i.VarType, ref matches_);
							}
							catch (NullReferenceException e)
							{
								Console.WriteLine("Do Not Have This Type");
							}

						}
					}
					//Type type = typ
				}
				else if (line[(line.Length - 1)].Equals('('))
				{
					matches_ = matchesSign;
				}


			}
*/
			//interfaces
			Regex interfaceRegex = new Regex(@"(class)([\s]+)(\w+)([\s]+):([\s]+)([\w]*)");


			Match match = interfaceRegex.Match(line);
			if (match.Success)
			{
				
				if ((match.Index + match.Length) == (line.Length))
				{
					Console.WriteLine("it works!!!!!!");
					matches_ = interfaceNames;
				}
			}

           

			int ReplacementStartPosition = cur_pos - cursorWordLength;


			List<CompleteReplyMatch> finalMatches = new List<CompleteReplyMatch>();

			List<string> tempMatches = matches_.Select(x => x.Name).Distinct().ToList();


			foreach (var i in tempMatches)
			{
				CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
				{
					Name = i,
					Documentation = "",
					Value = "",

				};
				finalMatches.Add(completeReplyMatch);
			}

			//TEMPORARY
			finalMatches.AddRange(interfaceNames);
			//finalMatches.AddRange(enumNames);
			//finalMatches.AddRange(structNames);
			//finalMatches.AddRange(propertyNames);
			//finalMatches.AddRange(variableNames);

			CompleteReply completeReply = new CompleteReply();

			if (line.Length > 0)
			{
				if (line[line.Length - 1] == '.')
				{

					completeReply.Matches = finalMatches;
					completeReply.Status = "ok";

					//CursorEnd = 10,
					//Matches = finalMatches,
					//Status = "ok",
					//FilterStartIndex = ReplacementStartPosition,
					// MatchedText = cursorWord,
					// MetaData = null
					//};

				}
				else
				{

					completeReply.Matches = finalMatches;
					completeReply.Status = "ok";
					completeReply.FilterStartIndex = ReplacementStartPosition;
					completeReply.MatchedText = cursorWord;
					/*
					CompleteReply completeReply = new CompleteReply()
					{
						//CursorEnd = 10,
						Matches = finalMatches,
						Status = "ok",
						FilterStartIndex = ReplacementStartPosition,
						MatchedText = cursorWord,
						// MetaData = null
					};*/
				}
			}

			//MakeFinal(classList, interfaceNames, enumNames, methodList, variableNames, structNames, listOfKeywords, ref matches_, currentclass);

			if (currentclass.Equals("global"))
			{
				Console.WriteLine("in global");
				if (line.Length > 0)
				{
					if (line.StartsWith("using "))
					{
						//Console.WriteLine("We're Starting with using!!!!!!!");
						DirectivesList(ref DirectiveMatches, line);

						var l = DirectiveMatches.GroupBy(i => i.Name).Select(group => group.First());
                        
						foreach (var i in l)
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
							{
								Name = i.Name,
								Documentation = "<font style=\"color:blue\">Namespace</font> " + i.Documentation,
								Value = "",
								Glyph = "directive"

							};
							FinalDirectives.Add(completeReplyMatch);
						}
						matches_ = FinalDirectives;

						RemoveNonMatches(ref matches_, cursorWord, line);
					}

					else if (line[line.Length - 1] == '.')
					{
						foreach (var i in variableNames)
						{
							if (i.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString().Equals(cursorWord))
							{
								try
								{
									Console.WriteLine(i.propertyDeclarationSyntax.Type.ToString() + " type");
									//Type type = Type.GetType(i.VarType);
									Type x = ProperTypeName(i.propertyDeclarationSyntax.Type.ToString());
									//ShowMethods(i.propertyDeclarationSyntax.Type.GetType(), ref matches_);
									ShowMethods(i.propertyDeclarationSyntax.DescendantNodes().OfType<SyntaxToken>().GetType(), ref matches_);

								}
								catch (NullReferenceException e)
								{
									Console.WriteLine("Do Not Have This Type");
								}

							}
						}
						foreach (var i in classList)
						{ //class before dot

							if (i.Identifier.ToString().Equals(cursorWord))
							{
								Console.WriteLine("found match class");
								foreach (var m in methodList)
								{ // method options
									Console.WriteLine(m.className.ToString() + "m name and i name " + i.Identifier.ToString());
									if (m.className.ToString().Equals(i.Identifier.ToString()))
									{
										Console.WriteLine("found method class");
										if (!(m.mds.Modifiers.ToString().Equals("private")))
										{
											Console.WriteLine("Found public method " + m.mds.Identifier.ToString());
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = m.mds.Identifier.ToString(),
												Documentation = "<font style =\"color:blue\">" + m.mds.ReturnType.ToString() + "</font> " + m.mds.Identifier.ToString() + "()",
                                                Value = "",
												Glyph = "method",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
								foreach (var v in variableNames)
								{ // variable options
									if (v.className.ToString().Equals(i.Identifier.ToString()))
									{

										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
										{
											Name = v.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
											Documentation = "",
											Value = "",
											Glyph = "field",

										};
										matches_.Add(completeReplyMatch);

									}
								}
								foreach (var e in enumNames)
								{
									if (e.className.ToString().Equals(i.Identifier.ToString()))
									{
										if (!(e.modifier.Equals("private")))
										{
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = e.name,
												Documentation = "<font style =\"color:blue\">Enum</font> " + e.name,
												Value = "",
												Glyph = "enum",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
								foreach (var s in structNames)
								{
									if (s.className.ToString().Equals(i.Identifier.ToString()))
									{
										if (!(s.modifier.Equals("private")))
										{
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = s.name,
												Documentation = "<font style =\"color:blue\">Struct</font> " + s.name,
                                                Value = "",
												Glyph = "struct",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
							}
						}
					}
				}
				if (String.IsNullOrWhiteSpace(line))
				{
					Console.WriteLine("We are in the else");
					foreach (var item in listOfKeywords)
					{
						CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
						{
							Name = item,
							Documentation = "<font style =\"color:blue\">Keyword</font> " + item,
                            Value = "",
							Glyph = "keyword",

						};
						matches_.Add(completeReplyMatch);
					}
					foreach (var item in classList)
					{
						Console.WriteLine("Weselna public classes");
						if (!(item.Modifiers.ToString().Equals("private")))
						{
							Console.WriteLine("Weselna public classes");
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.Identifier.ToString(),
								Documentation = "<font style =\"color:blue\">Class</font> " + item.Identifier.ToString(),
                                Value = "",
								Glyph = "class",

							};
							matches_.Add(completeReplyMatch);
						}
					}
					foreach (var item in variableNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
								Documentation = "",
								Value = "",
								Glyph = "field",

							};
							matches_.Add(completeReplyMatch);
						}
					}
					foreach (var item in enumNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.name,
								Documentation = "<font style =\"color:blue\">Enum</font> " + item.name,
                                Value = "",
								Glyph = "enum",

							};
							matches_.Add(completeReplyMatch);
						}
					}
					foreach (var item in structNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.name,
								Documentation = "<font style =\"color:blue\">Struct</font> " + item.name,
                                Value = "",
								Glyph = "struct",

							};
							matches_.Add(completeReplyMatch);
						}

					}
					matches_.AddRange(interfaceNames);
				}
			}
			else
			{
				if (String.IsNullOrWhiteSpace(line))
				{
					Console.WriteLine("We are in the else");
					foreach (var item in listOfKeywords)
					{
						CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
						{
							Name = item,
							Documentation = "<font style =\"color:blue\">Keyword</font> " + item,
                            Value = "",
							Glyph = "keyword",

						};
						matches_.Add(completeReplyMatch);
					}
					foreach (var item in classList)
					{
						Console.WriteLine("Weselna public classes");
						if (!(item.Modifiers.ToString().Equals("private")))
						{
							if (!(item.Identifier.ToString().Equals(currentclass)))
							{
								Console.WriteLine("Weselna public classes");
								CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
								{
									Name = item.Identifier.ToString(),
									Documentation = "<font style =\"color:blue\">Class</font> " + item.Identifier.ToString(),
                                    Value = "",
									Glyph = "class",

								};
								matches_.Add(completeReplyMatch);
							}
						}
					}
					foreach (var item in variableNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
								Documentation = "",
								Value = "",
								Glyph = "field",

							};
							matches_.Add(completeReplyMatch);
						}else{
							if(item.className.Equals(currentclass)){
								CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
                                {
                                    Name = item.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
                                    Documentation = "",
                                    Value = "",
                                    Glyph = "field",

                                };
                                matches_.Add(completeReplyMatch);
							}
						}
					}
					foreach (var item in enumNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.name,
								Documentation = "<font style =\"color:blue\">Enum</font> " + item.name,
                                Value = "",
								Glyph = "enum",

							};
							matches_.Add(completeReplyMatch);
						}else
                        {
                            if (item.className.Equals(currentclass))
                            {
								CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
								{
									Name = item.name,
									Documentation = "<font style =\"color:blue\">Enum</font> " + item.name,
                                    Value = "",
                                    Glyph = "enum",

                                };
                                matches_.Add(completeReplyMatch);
                            }
                        }
					}
					foreach (var item in structNames)
					{
						if ((item.className.Equals("global")))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.name,
								Documentation = "<font style =\"color:blue\">Struct</font> " + item.name,
                                Value = "",
								Glyph = "struct",

							};
							matches_.Add(completeReplyMatch);
						}else
                    {
                        if (item.className.Equals(currentclass))
                        {
                            CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
                            {
									Name = item.name,
									Documentation = "<font style =\"color:blue\">Struct</font> " + item.name,
                                Value = "",
                                Glyph = "struct",

                            };
                            matches_.Add(completeReplyMatch);
                        }
                    }

					}
					matches_.AddRange(interfaceNames);
					foreach (var item in methodList)
					{
						if (item.className.Equals(currentclass))
						{
							CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
							{
								Name = item.mds.Identifier.ToString(),
								Documentation = "<font style =\"color:blue\">" + item.mds.ReturnType.ToString() + "</font> " + item.mds.Identifier.ToString() + "()",
                                Value = "",
								Glyph = "method",

							};
							matches_.Add(completeReplyMatch);
						}
						/*else
						{
							if (!(item.mds.Modifiers.Equals("private")))
							{
								CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
								{
									Name = item.mds.Identifier.ToString(),
									Documentation = "",
									Value = "",
									Glyph = "method",

								};
								matches_.Add(completeReplyMatch);
							}
						}*/
					}
				}


				else if (line.Length > 0)
				{
					if (line[line.Length - 1] == '.')
					{
						foreach (var i in variableNames)
						{
							if (i.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString().Equals(cursorWord))
							{
								try
								{
									Console.WriteLine(i.propertyDeclarationSyntax.Type.ToString() + " type");
									//Type type = Type.GetType(i.VarType);
									Type x = ProperTypeName(i.propertyDeclarationSyntax.Type.ToString());
									//ShowMethods(i.propertyDeclarationSyntax.Type.GetType(), ref matches_);
									ShowMethods(i.propertyDeclarationSyntax.DescendantNodes().OfType<SyntaxToken>().GetType(), ref matches_);

								}
								catch (NullReferenceException e)
								{
									Console.WriteLine("Do Not Have This Type");
								}

							}
						}
						/*foreach (var i in variableNames)
						{
							if (i.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString().Equals(cursorWord))
							{
								try
								{
									Console.WriteLine(i.propertyDeclarationSyntax.Type.ToString() + " type");
									//Type type = Type.GetType(i.VarType);
									Type x = ProperTypeName(i.propertyDeclarationSyntax.Type.ToString());
									//ShowMethods(i.propertyDeclarationSyntax.Type.GetType(), ref matches_);
									ShowMethods(i.propertyDeclarationSyntax.DescendantNodes().OfType<SyntaxToken>().GetType(), ref matches_);

								}
								catch (NullReferenceException e)
								{
									Console.WriteLine("Do Not Have This Type");
								}

							}
						}*/
						foreach (var i in classList)
						{ //class before dot

							if (i.Identifier.ToString().Equals(cursorWord))
							{
								Console.WriteLine("found match class");
								foreach (var m in methodList)
								{ // method options
									Console.WriteLine(m.className.ToString() + "m name and i name " + i.Identifier.ToString());
									if (m.className.ToString().Equals(i.Identifier.ToString()))
									{
										Console.WriteLine("found method class");
										if (!(m.mds.Modifiers.ToString().Equals("private")))
										{
											Console.WriteLine("Found public method " + m.mds.Identifier.ToString());
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = m.mds.Identifier.ToString(),
												Documentation = "<font style =\"color:blue\">" + m.mds.ReturnType.ToString() + "</font> " + m.mds.Identifier.ToString() + "()",
                                                Value = "",
												Glyph = "method",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
								foreach (var v in variableNames)
								{ // variable options
									if (v.className.ToString().Equals(i.Identifier.ToString()))
									{

										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
										{
											Name = v.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
											Documentation = "",
											Value = "",
											Glyph = "field",

										};
										matches_.Add(completeReplyMatch);

									}
								}
								foreach (var e in enumNames)
								{
									if (e.className.ToString().Equals(i.Identifier.ToString()))
									{
										if (!(e.modifier.Equals("private")))
										{
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = e.name,
												Documentation = "<font style =\"color:blue\">Enum</font> " + e.name,
                                                Value = "",
												Glyph = "enum",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
								foreach (var s in structNames)
								{
									if (s.className.ToString().Equals(i.Identifier.ToString()))
									{
										if (!(s.modifier.Equals("private")))
										{
											CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch()
											{
												Name = s.name,
												Documentation = "<font style =\"color:blue\">Struct</font> " + s.name,
                                                Value = "",
												Glyph = "struct",

											};
											matches_.Add(completeReplyMatch);
										}
									}
								}
							}
							else if (cursorWord.Equals("this"))
							{
								foreach (var item in variableNames)
								{
									if ((item.className).Equals(currentclass))
									{
										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
										{
											Name = item.propertyDeclarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList().First().Identifier.ToString(),
											Documentation = "",
											Value = "",
											Glyph = "field"
										};
										matches_.Add(completeReplyMatch);
									}
								}
								foreach (var item in enumNames)
								{
									if ((item.className).Equals(currentclass))
									{
										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
										{
											Name = item.name,
											Documentation = "<font style =\"color:blue\">Enum</font> " + item.name,
                                            Value = "",
											Glyph = "enum"
										};
										matches_.Add(completeReplyMatch);
									}
								}
								foreach (var item in structNames)
								{
									if ((item.className).Equals(currentclass))
									{
										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
										{
											Name = item.name,
											Documentation = "<font style =\"color:blue\">Struct</font> " + item.name,
                                            Value = "",
											Glyph = "struct"
										};
										matches_.Add(completeReplyMatch);
									}
								}
								foreach (var item in methodList)
								{
									if ((item.className).Equals(currentclass))
									{
										CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
										{
											Name = item.mds.Identifier.ToString(),
											Documentation = "<font style =\"color:blue\">" + item.mds.ReturnType.ToString() + "</font> " + item.mds.Identifier.ToString() + "()",
                                            Value = "",
											Glyph = "method"
										};
										matches_.Add(completeReplyMatch);
									}
								}
							}

						}

					}
				}
			}
		
						


			


				CompleteReply cr = new CompleteReply();

				Console.WriteLine(matches_.Count + " matches-- size");
				if (line.Length > 0)
				{
					if (line[line.Length - 1] == '.')
					{

						cr.Matches = matches_;
						cr.Status = "ok";
						//FilterStartIndex = ReplacementStartPosition,
						//MatchedText = cursorWord,

					}
					else
					{

						cr.Matches = matches_;
						cr.Status = "ok";
						cr.FilterStartIndex = ReplacementStartPosition;
						cr.MatchedText = cursorWord;

					}
			    }else{
				Console.WriteLine("In the final else");
                cr.Matches = matches_;
                cr.Status = "ok";
                //cr.FilterStartIndex = ReplacementStartPosition;
                //cr.MatchedText = cursorWord;

            }
				
			
			Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(cr), message.Header);
			this.logger.Info("Sending complete_reply");
			this.messageSender.Send(completeReplyMessage, serverSocket);

		}
        
        public string returnMethodDocumentation(MethodDeclarationSyntax mds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<font style =\"color:{0}\">{1}</font> ", "blue", mds.ReturnType.ToString()));
            sb.Append(string.Format("<font style =\"color:{0}\">{1}</font>", "green", mds.ReturnType.ToString()));

            //   sb.Append(string.Format("<font style=\"color:{0}\">{1}</font>", ", encoded));

            return sb.ToString();
        }

		public void catchInterfaces(SyntaxNode tree, ref List<CompleteReplyMatch> interfaceList, List<ClassDeclarationSyntax> classes)
		{
			var myInterface = tree.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToList();

			foreach (var node in myInterface)
			{
				CompleteReplyMatch crm = new CompleteReplyMatch()
				{
					Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
					Documentation = "<font style=\"color:blue\">interface</font> " + node.Identifier.ToString(),
					Value = "",
					Glyph = "interface"
				};
				interfaceList.Add(crm);
			}
		}

		public void catchEnums(SyntaxNode tree, ref List<catchCollection> enumList, List<ClassDeclarationSyntax> classes)
		{
			var myEnum = tree.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();
			catchCollection collection;
			foreach (var node in myEnum)
			{
				collection.documentation = "<font style=\"color:blue\">enum</font>" + node.Identifier.ToString();
				collection.name = node.Identifier.ToString();
				collection.modifier = node.Modifiers.ToString();
				collection.className = "global";

				foreach (var c in classes)
                {
                    if ((node.Span.Start >= c.Span.Start) && (node.Span.End <= c.Span.End))
                    {

                        collection.className = c.Identifier.ToString();

                    }
                }
				enumList.Add(collection);
			}
		}

		public void catchStructs(SyntaxNode tree, ref List<catchCollection> structList, List<ClassDeclarationSyntax> classes)
		{
			var myStruct = tree.DescendantNodes().OfType<StructDeclarationSyntax>().ToList();
			catchCollection collection;
			foreach (var node in myStruct)
			{
				collection.className = "global";
				collection.documentation = "<font style=\"color:blue\">struct</font>" + node.Identifier.ToString();
				collection.modifier = node.Modifiers.ToString();
				collection.name = node.Identifier.ToString();
				foreach (var c in classes)
                {
                    if ((node.Span.Start >= c.Span.Start) && (node.Span.End <= c.Span.End))
                    {

                        collection.className = c.Identifier.ToString();

                    }
                }
				structList.Add(collection);
			}
		}

	/*	public void catchProperties(SyntaxNode tree, ref List<CompleteReplyMatch> propertyList, List<ClassDeclarationSyntax> classes)
		{
			var myProperty = tree.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

			foreach (var node in myProperty)
			{
				CompleteReplyMatch crm = new CompleteReplyMatch()
				{
					Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
					Documentation = "<font style=\"color:blue\">property</font>" + node.Identifier.ToString(),
					Value = "",
					Glyph = "property"
				};
				propertyList.Add(crm);
			}
		}*/

		public void catchVariables(SyntaxNode tree, ref List<variableCollection> variableList, List<ClassDeclarationSyntax> classes)
		{
			var myVariable = tree.DescendantNodes().OfType<VariableDeclarationSyntax>().ToList();
			variableCollection collection;
			foreach (var node in myVariable)
			{
				collection.propertyDeclarationSyntax = node;
				collection.className = "global";
                
                foreach (var c in classes)
                {
                    if ((node.Span.Start >= c.Span.Start) && (node.Span.End <= c.Span.End))
                    {

                        collection.className = c.Identifier.ToString();

                    }
                }
				variableList.Add(collection);
			}
		}



		public void CatchClassMethods(SyntaxTree tree, ref List<ClassDeclarationSyntax> classList, ref List<methodCollection> methodList, int curPos)
		{           
			var root = (CompilationUnitSyntax)tree.GetRoot();

			var compilation = CSharpCompilation.Create("HelloWorld")
											   .AddReferences(
													MetadataReference.CreateFromFile(
														typeof(object).Assembly.Location))
											   .AddSyntaxTrees(tree);

			var model = compilation.GetSemanticModel(tree);

			List<ClassDeclarationSyntax> classListCaptured = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

			foreach (var l in classListCaptured)
			{
				classList.Add(l);



			}


		}

		public void CatchMethods(SyntaxNode tree, ref List<methodCollection> methods, List<ClassDeclarationSyntax> classes){

			var myMethod = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
			methodCollection collection;
            
			foreach (var node in myMethod)
            {
				collection.className = "global";
				collection.mds = node;
                
				foreach(var c in classes){
					if((node.Span.Start >= c.Span.Start) && (node.Span.End <= c.Span.End)){

						collection.className = c.Identifier.ToString();

					}
				}
				methods.Add(collection);
            }

		}




		public void AddKeywordsToMatches(ref List<CompleteReplyMatch> matches_)
		{

			string[] arrayOfKeywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void", "volatile", "while" };
			List<string> listOfKeywords = new List<string>();
			listOfKeywords.AddRange(arrayOfKeywords);

			foreach (string i in listOfKeywords)
			{
				CompleteReplyMatch crm = new CompleteReplyMatch()
				{

					Name = i,
					Documentation = i + " Keyword",
					Value = "",
					Glyph = "keyword"

				};
				matches_.Add(crm);

			}
		}

		public Tuple<string, int> FindWordToAutoComplete(string line)
		{
			line = Regex.Replace(line, @"[^\w&^\.]", "*");

			string cursorWord, cursorLine;
			int curWordLength = 0;

			Regex p = new Regex(@".*\*"); //regex to match up to last '*'
			Match mat = p.Match(line);

			if (mat.Success)
			{

				cursorLine = line.Substring(mat.Index + mat.Length);

			}
			else
			{
				cursorLine = line;
			}
			if (cursorLine.Length > 0)
			{
				if (cursorLine[cursorLine.Length - 1] == '.')
				{
					cursorLine = cursorLine.Substring(0, cursorLine.Length - 1);
				}
			}

			p = new Regex(@".*\.");
			mat = p.Match(cursorLine);

			if (mat.Success)
			{
				cursorWord = cursorLine.Substring(mat.Index + mat.Length);


			}
			else
			{
				cursorWord = cursorLine;
				cursorLine = "";
			}

			curWordLength = cursorWord.Length;

			if (line.Length > 0)
			{
				if (line[line.Length - 1] == '.')
				{
					curWordLength = 0;
				}
			}

			return Tuple.Create(cursorWord, curWordLength);

		}

		public void RemoveNonMatches(ref List<CompleteReplyMatch> matches_, string cursorWord, string line)
		{

			//Console.WriteLine("1. " + matches_[0].Name + " 2. " + matches_[matches_.Count-1].Name);

			for (int j = matches_.Count - 1; j > -1; j--)
			{
				if ((line.StartsWith("using ")) && (line[line.Length - 1] == '.'))
				{
					return;
				}
				if (!(matches_[j].Name.StartsWith(cursorWord)))
				{
					matches_.RemoveAt(j);
				}
			}
		}

		public void MakeMethodSignMatches(string line, List<MethodMatch> methodMatches, ref List<CompleteReplyMatch> matchesSign)
		{

			Regex regex = new Regex(@"(?<name>\w+)(\()");

			Match i = regex.Match(line);

			string add;

			foreach (MethodMatch m in methodMatches)
			{

				add = "";
				if (i.Groups["name"].ToString().Equals(m.Name))
				{
					add = m.Name + "(";
					foreach (string s in m.ParamList)
					{
						add = add + s + ", ";
					}
					add = add + ")";
				}

				CompleteReplyMatch crm = new CompleteReplyMatch
				{
					Name = add,
					Documentation = "",
					Value = "",
					Glyph = "method"
				};
				matchesSign.Add(crm);
			}

		}

		public void VariableMatches(string code, ref List<VariableMatch> variableMatches)
		{
			//Console.WriteLine("GHAYARNA EL CODE1");
			Regex regex = new Regex(@"(?<type>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum|Comparer<[^>]*>|EqualityComparer<[^>]*>|HashSet<[^>]*>|LinkedList<[^>]*>|List<[^>]*>|Queue<[^*]>|SortedSet<[^*]>|Stack<[^*]>|)([\s]+)(?<name>\w+)");
			string type;
			foreach (Match m in regex.Matches(code))
			{
				//	Console.WriteLine("Found variable match");
				//		Console.WriteLine(m.Groups["type"]);
				type = m.Groups["type"].ToString();
				Type t = ProperTypeName(type);

				VariableMatch variableMatch = new VariableMatch
				{

					Name = m.Groups["name"].ToString(),
					VarType = t,

				};
				variableMatches.Add(variableMatch);



			}


		}

		public Type ProperTypeName(string type)
		{

			int index = type.IndexOf("<");
			if (index > 0)
			{
				type = type.Substring(0, index);
			}

			//	Console.WriteLine("type after index " + type);

			switch (type)
			{
				case "int":
					return typeof(int);
					break;
				case "string":
					return typeof(string);
					break;
				case "sbyte":
					return typeof(sbyte);
					break;
				case "short":
					return typeof(short);
					break;
				case "long":
					return typeof(long);
					break;
				case "byte":
					return typeof(byte);
					break;
				case "ushort":
					return typeof(ushort);
					break;
				case "ulong":
					return typeof(ulong);
					break;
				case "char":
					return typeof(char);
					break;
				case "float":
					return typeof(float);
					break;
				case "double":
					return typeof(double);
					break;
				case "decimal":
					return typeof(decimal);
					break;
				case "bool":
					return typeof(bool);
					break;
				case "Comparer":
					return typeof(Comparer<>);
					break;
				case "EqualityComparer":
					return typeof(EqualityComparer<>);
					break;
				case "HashSet":
					return typeof(HashSet<>);
					break;
				case "LinkedList":
					return typeof(LinkedList<>);
					break;
				case "List":
					//Console.WriteLine("in the list branch");
					return typeof(List<>);
					break;
				case "Queue":
					return typeof(Queue<>);
					break;
				case "SortedSet":
					return typeof(SortedSet<>);
					break;
				case "Stack":
					return typeof(Stack<>);
					break;
				default:
					//	Console.WriteLine("in the default branch");
					return null;
					break;
			}

		}

		public void ShowMethods(Type type, ref List<CompleteReplyMatch> matches_)
		{
			foreach (var method in type.GetMethods())
			{
				var parameters = method.GetParameters();
				var parameterDescriptions = string.Join
					(", ", method.GetParameters()
								 .Select(x => x.ParameterType + " " + x.Name)
								 .ToArray());

				CompleteReplyMatch crm = new CompleteReplyMatch
				{

					Name = method.Name,
					Documentation = parameterDescriptions,
					Value = "",
					Glyph = "method"
				};
				matches_.Add(crm);

			}
		}

		public void DirectivesList(ref List<CompleteReplyMatch> DirectiveMatches, string line)
		{
			Regex regex = new Regex(@"(using)([\s]+)(?<directive>(\w|\.)*)");
			Match match = regex.Match(line);
			string input = match.Groups["directive"].ToString();

			//	Console.WriteLine("Directive aho " + match.Groups["directive"].ToString());

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					//foreach (Type t in assembly.GetTypes())

					var namespaces = assembly.GetTypes()
						 .Select(t => GetToNearestDot(t, input))
						 .Distinct();

					foreach (var n in namespaces)
					{
						if (!(n.Equals(" ")))
						{
							CompleteReplyMatch crm = new CompleteReplyMatch
							{
								Name = n,
								Documentation = "",
								Value = "",
								Glyph = "directive"

							};
							DirectiveMatches.Add(crm);
						}
					}
				}
				catch (ReflectionTypeLoadException e)
				{
					Console.WriteLine("Could not load type");
				}
			}
		}

		public string GetToNearestDot(Type t, string s)
		{

			//Console.WriteLine("Arrived here " + t.FullName +" " + s);
			try
			{
				if (!(t.Namespace.StartsWith(s)))
				{
					return " ";
				}
			}
			catch (NullReferenceException e)
			{
				return " ";
			}

			//Console.WriteLine("3adeina elhamdlAllah " + t.FullName);

			string ns = t.Namespace ?? "";

			int firstDot = s.LastIndexOf('.');

			//Console.WriteLine( "ns before " + ns);
			if (ns.Equals(""))
			{

			}
			else
			{
				ns = ns.Substring(firstDot + 1);
			}

			int firstDotns = ns.IndexOf('.');
			//int firstDot = s.LastIndexOf('.');

			//Console.WriteLine( "ns after " + ns);

			int finaldot = ns.IndexOf('.');

			return (finaldot == -1) ? ns : ns.Substring(0, finaldot);

		}

		public void MakeFinal(List<ClassDeclarationSyntax> classList, List<CompleteReplyMatch> interfaceNames, List<catchCollection> enumNames, List<methodCollection> methodList, List<variableCollection> variableList, List<catchCollection> structNames, List<CompleteReplyMatch> listOfKeywords, ref List<CompleteReplyMatch> matches_, string currentClass){

            //if(currentClass)

		}


	}
}


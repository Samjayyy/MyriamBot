using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyriamBot.Conversation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConversationTreeAnalyzer
{
    /// <summary>
    /// Watch out: Error prone code, just experimenting with the features..
    /// </summary>
    class Program
    {
        class Node
        {
            public Node(Type type)
            {
                Type = type;
                Links = new List<Node>();
            }
            public Type Type { get; }
            public string ClassName => Type.Name;
            public List<Node> Links { get; }
            public override string ToString() => ClassName;
        }
        static void Main(string[] args)
        {
            var dict = InitConversationClasses();
            var root = dict["StartConversation"];
            foreach(var current in dict.Values)
            {
                var syntaxtree = ParseTreeFromClass(current.ClassName);
                var test = syntaxtree.GetRoot()
                    .DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .ToList();
                var descnodes = syntaxtree.GetRoot()
                    .DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(n => dict.ContainsKey(n.Type.ToString()))
                    .Select(n => dict[n.Type.ToString()]);
                foreach (var depends in descnodes)
                {
                    current.Links.Add(depends);
                }
            }
            // start creating links, taking into account links in base types
            Console.WriteLine("----------------");
            foreach (var node in dict.Values)
            {
                if (node.Type.IsAbstract)
                {
                    continue; // ignore abstract implementations in diagram
                }
                HashSet<Node> allLinks = new HashSet<Node>();
                var type = node.Type;
                while (type != null
                    && dict.ContainsKey(type.Name))
                {
                    foreach (var link in dict[type.Name].Links)
                    {
                        if (allLinks.Add(link))
                        {
                            Console.WriteLine($"{node} -> {link}");
                        }
                    }
                    type = type.BaseType;
                }
            }
            Console.WriteLine("----------------");
            Console.WriteLine("Copy paste above graph on website: https://rise4fun.com/Agl");
            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        static IDictionary<string, Node> InitConversationClasses()
        {
            var type = typeof(AbstractConversation);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !type.IsInterface)
                .ToDictionary(t => t.Name, t => new Node(t));
        }

        static SyntaxTree ParseTreeFromClass(string className)
        {
            var path = FindPathFromClassName(className);
            return ParseTree(path);
        }

        static string FindPathFromClassName(string className)
        {
            var files = Directory.GetFiles("..\\..\\..\\", $"{className}.cs", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                throw new InvalidOperationException($"Cannot find class {className} in folder");
            }
            else if (files.Length > 1)
            {
                throw new InvalidOperationException($"Found multiple classes with name {className} in folder");
            }
            return files[0];
        }

        static SyntaxTree ParseTree(string filename)
        {
            var code = new StreamReader(filename).ReadToEnd();
            return CSharpSyntaxTree.ParseText(code);
        }
    }
}

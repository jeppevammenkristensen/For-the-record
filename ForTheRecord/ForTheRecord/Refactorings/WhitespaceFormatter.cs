using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForTheRecord.Refactorings
{
    public static class WhitespaceFormatter
    {
        public static SyntaxNode NormalizeWhitespacesSingleLineProperties(this SyntaxNode node) =>
            node.NormalizeWhitespace().SingleLineProperties();

        public static TSyntaxNode SingleLineProperties<TSyntaxNode>(this TSyntaxNode node) where TSyntaxNode : SyntaxNode => new SingleLinePropertyRewriter().Visit(node) as TSyntaxNode;

        class SingleLinePropertyRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) =>
                node.NormalizeWhitespace(indentation: "", eol: " ")
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());
        }
    }
}
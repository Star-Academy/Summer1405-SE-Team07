namespace QueryLib.Compilers.Abstractions;

public interface ISqlCompilerFactory
{
    public ICompiler Create(string compilertype);
}
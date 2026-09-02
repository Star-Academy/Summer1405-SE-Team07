namespace QueryLib.Renderers.Abstractions;

public interface IClauseRendererRegistryFactory
{
    ClauseRendererRegistry GetRegistry(DbProvider provider);
}


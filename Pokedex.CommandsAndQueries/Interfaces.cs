namespace Pokedex.CommandsAndQueries;

public interface ICommand { }
public interface IQuery { }

// Not needed here, just provided for completeness
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task Execute(TCommand command);
}

public interface IQueryHandler<TQuery, TQueryResult> where TQuery : IQuery
{
    Task<TQueryResult> Execute(TQuery query);
}


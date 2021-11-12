using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;

namespace Pokedex.Api.Controllers
{
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IQueryHandler<GetPokemonByNameQuery, Pokemon> _queryHandler;

        public PokemonController(IQueryHandler<GetPokemonByNameQuery, Pokemon> queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [Route("/pokemon/{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] [Required] string pokemonName)
        {
            var result = await _queryHandler.Execute(new GetPokemonByNameQuery(pokemonName));

            if (result == Pokemon.Empty)
                return NotFound();

            return Ok(Map(result));
        }

        private static PokemonResponse Map(Pokemon result) => new(result.Id, result.Name, result.Description, result.Habitat, result.IsLegendary);
    }
}

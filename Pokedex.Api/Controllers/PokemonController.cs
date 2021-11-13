using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;

namespace Pokedex.Api.Controllers
{
    /// <summary>
    /// Operations for getting Pokemon details
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class PokemonController : ControllerBase
    {
        private readonly IQueryHandler<GetPokemonByNameQuery, Pokemon> _queryHandler;

        /// <inheritdoc />
        public PokemonController(IQueryHandler<GetPokemonByNameQuery, Pokemon> queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary> Get details for specified Pokemon </summary>
        /// <param name="pokemonName">Name of Pokemon to retrieve details for</param>
        /// <response code="400">If Pokemon name is invalid. Only letters, numbers and dashes are valid.</response>
        /// <response code="404">If Pokemon name is not found.</response>
        [HttpGet("/pokemon/{pokemonName}")]
        [ProducesResponseType(typeof(PokemonResponse), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

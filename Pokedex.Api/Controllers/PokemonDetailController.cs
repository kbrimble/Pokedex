using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Pokedex.Api.Controllers
{
    [ApiController]
    public class PokemonDetailController : ControllerBase
    {
        [Route("/pokemon/{pokemonName}")]
        public async Task<IActionResult> GetPokemonDetail([FromRoute] [Required] string pokemonName)
        {
            return Ok();
        }
    }
}

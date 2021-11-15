# Pokedex

To run:

  * Install and set up [Docker](https://docs.docker.com/get-docker/)
  * Build the Docker image: `docker build -t pokedex .`
  * Run the image in a container: `docker run -d -p 8080:80 --name dex pokedex`
  * Navigate to [the Swagger docs](http://localhost:8080/swagger) to try out the API

## Design choices

I've decided to use CQRS for this API which is perhaps overkill here.
The main reason for doing it this way was that it is very extensible to future requirements but also the ability
to orchestrate query handlers together (i.e. get a Pokemon and then translate it's description) makes for easy
understanding of the flow of data through the system.

The external dependencies are kept in their own projects so that the models that they use are separate from
the domain that is modelled in the rest of the application. Responses are then mapped to domain objects before
crossing into the main application. Similarly, on the way out, domain objects are mapped to response objects that
can then contain extra documentation for consumers which is not needed within the domain classes themselves.

I've relied heavily on C# record types throughout the solution and the succinctness that they provide means that
many related types can easily be bundled together in the same file to allow for easy scanning. I believe that this
gives a clear picture of what is being modelled without having to jump through several files but it is personal
preference!

## Resilience

As there is a strict rate-limiting policy on the Fun Translations API, I've added a circuit breaker that will break
on a single 429 response and not make another request for 10 minutes. If I was to improve upon this design, I would
make the circuit breaker distributed so that this API could be scaled horizontally if needed. It could also be made
much more complicated by checking when the last 429 was received and calculating when the circuit should be safe to
open but that seemed beyond the scope here.

For the Pokeapi, I've implemented a simple retry & circuit breaker combination to handle transient failures.

## Testing

In my unit tests, I've tried to avoid tests that check that services collaborate together and instead focused on
tests that assert on output for a given input (exceptions being included in that output).

For the integration tests, I've tested where this system interacts with any external dependencies but have kept
the amount of tests small to avoid hammering external APIs. I've had to use `SkippableFact` to be able to assert
the test as inconclusive when rate limiting prohibits testing the `FunTranslationService` fully.

The component tests serve as a test of the whole system with the services covered by the integration tests mocked.
That way, the behaviour of these services can be controlled and it allows for simulating what the API would do in
situations that are hard to set up (a good example of this mocking authentication to see how the system reacts to
users without the correct permissions without having to create those users just for the tests) and also to avoid
calling the Fun Translations API more than necessary.

Lastly, the functional tests are really simple end-to-end/smoke tests to check that the system can perform the core
functions required of it.

## Improvements

  * Get [HTTPS forwarding](https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md) working correctly with Docker
  * Work with a PO to decide what to do with formatting characters in descriptions
  * Add some assembly scanning for DI bindings
  * Add caching for both external requests and responses returned
    * There is a lot of static data here for the Pokemon and this could be downloaded once, cached and periodically updated
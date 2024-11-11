# ADR 1: Adopt Fluent Validation for Command Validation

## Status

Accepted

## Context

Jason Taylor's template repo for Clean Architecture uses Fluent Validation for validaitng commands, even for more advanced use cases of injecting a DbContext and evaluating data in the db to enforce validation via Abstract Validators.

With new enum columns that might have unknown legacy data, we had a need to enforce that `Unknown` can be used for querying so that clients can deal with the (missing) legacy data, while also enforcing that it cannot be passed in from a client. We want to require good data with the rollout of these new enum fields. This meant we could either adopt the Fluent Validation `AbstractValidator` pattern in the sample repo, or discard it for simplicity and ease of use in our project and use case.

## Decision

Adopted Fluent Validation for command validation logic, even to the point of a generic, reusable enum validator to enforce the application convention of `Unknown` enum values. `Unknown` is one way to handle the null object pattern in the codebase + database and avoid using `null` as a business construct.

## Consequences

- **More db network traffic**: A tradeoff exists here for a higher number of database connections and traffic. It is critical to keep the validation expressions extremely lightweight so that they can return as fast as possible. We have not yet solved for granular control over parallelization of the `async` validation expressions.
- **Better Testability**: The validators are fully plugged into our application layer and can be tested in isolation by themselves (mocking a db set), or facilitating a more thorough use case via Application layer testing.
- **Slimmer Handlers**: Without these validators, command handlers would need to adopt this logic inside the `Handle()`. This is doable, but scales poorly as commands become larger in size and validations become more complex. Some validations necessitate a db roundtrip anyway to ensure we can fail fast for things like constraint or uniqueness violations that users might not be aware of and couldn't possibly fix themselves.

## References

- [Jason Taylor's Sample Repo ADR](https://github.com/ardalis/CleanArchitecture/blob/main/docs/architecture-decisions/adr-001-dotnet-di-adoption.md) - Used ADR template located in his template repo.
- [Getting Started with Architecture Decision Records](https://ardalis.com/getting-started-with-architecture-decision-records/) - Resource on ADR best practices.

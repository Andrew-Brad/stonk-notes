# Stonk Notes

This is a reference application modeled after [Jason Taylor's Clean Architecture template in C#](https://github.com/jasontaylordev/CleanArchitecture). It's purpose is to act as a learning and demonstration tool for enterprise patterns in Asp.Net Core, using one flavor of Clean Architecture template. In some areas, I pivoted in implementation based on my own opinions and experience.

It represents a Trading Journal application to help retail traders track Proft/Loss metrics, analyze trade performance, and facilitate retrospectives.

The app was scaffolded using this CLI command:
`dotnet new ca-sln -cf None -o StonkNotes --use-sqlite`

## Getting Started

GPG Signing is required. First time clone of the repo will require `git config commit.gpgsign true`.

### Build

`dotnet build`

The project has been refactored to work with postgres, and the Clean Architecture migration files and EF configurations have been fixed to achieve this.

To stand up the local postgres DB, use the docker compose file in the repo:

`docker compose up stonknotes-postgres-local-db-compose.yaml`
[stonknotes-postgres-local-db-compose.yaml](compose.yaml)

You can also import the yaml to portainer as a `stack` and manage it through that UI.

This will instantiate a new postgres container with user/password/database: `stonknotes`. The postgres instance will bind to port 5440 by default, but this can be changed by tweaking both the yaml file and `appsettings.json`.

Migrate your database to the latest schema:

`dotnet ef database update --project src/Infrastructure --startup-project src/Web`

Note that on Windows, your file separators should be `\`. On Linux, use `/`.

### DB Migration Dev Loop

Make the desired changes to your EF Configuration file.

Create the migration (EF will also update the snapshot in turn)

`dotnet ef migrations add "AddFooProperty" --project src/Infrastructure --startup-project src/Web --output-dir Data/Migrations`

Update the database:

`dotnet ef database update --project src/Infrastructure --startup-project src/Web`

Instead of using `dotnet ef migration remove` with `--force`, it's a better practice to exercise your `Down()` method.

List out your migration names:

`dotnet ef migrations list --project src/Infrastructure --startup-project src/Web`

Choose the desired one to reset back to (useful if you've applied multiple migrations)

`dotnet ef database update <PreviousMigrationName> --project src/Infrastructure --startup-project src/Web`

For example, this will reset you all the way back to initial create:

`dotnet ef database update "20241207164830_InitialCreate" --project src/Infrastructure --startup-project src/Web`

You can then remove the migration as needed (or just remove the files and snapshot diffs via `git`)

`dotnet ef migrations remove --project src/Infrastructure --startup-project src/Web`

## GQL Schema

The SDL file is located in `src/Web/GraphQLSchema/schema.graphql`. It can be generated using the following CLI command from the project root:

`dotnet run --project "./src/Web/Web.csproj" -- schema export --output "./GraphQLSchema/schema.graphql"`

## Use Cases

Todo: Explain use cases

`dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int`

### Other Miscellaneous References

Below are other Clean Architecture templates, all of which influenced technical decision-making in this project.

| **Reference Project** | **Url** |
|:---------------------:|:-------:|
| Matthew Rense's Clean Architecture Sample via Pluralsight | [Reference Link][link1] |
| Jason Taylor's Clean Architecture Reference Repo          | [Reference Link][link2] |
| Vladimir Khorikov's DDD In Action Repo                    | [Reference Link][link3] |
| Steve Smith's Clean Architecture Reference Repo           | [Reference Link][link4] |

[link1]: https://app.pluralsight.com/library/courses/clean-architecture-patterns-practices-principles/exercise-files
[link2]: https://github.com/jasontaylordev/CleanArchitecture
[link3]: https://github.com/vkhorikov/DddInAction
[link4]: https://github.com/ardalis/CleanArchitecture
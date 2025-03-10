# Crowded Region Detection
Bachelor project - Crowded Region Detection


## App
Run with `flutter run` in /crowdedapp directory


## Backend
Run with `dotnet run` in /CrowdedBackend directory


Make Rider auto generate a controller. When it fails, copy the command it tried to run,
then remove `--useSqlite` and add `--databaseProvider postgres`

Make Rider auto generate a migration `dotnet ef migrations add <NameOfMigration>`.
Make Rider build database `dotnet ef database update --verbose`
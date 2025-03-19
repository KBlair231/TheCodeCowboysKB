BEGIN TRANSACTION;
ALTER TABLE [Players] ADD [Class] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [GameStates] ADD [IsLocationComplete] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [GameStates] ADD [PlayerLocation] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319090506_Sprint3', N'9.0.2');

COMMIT;
GO


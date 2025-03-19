BEGIN TRANSACTION;
ALTER TABLE [Players] ADD [Class] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [Players] ADD [ItemId] int NOT NULL DEFAULT 0;

ALTER TABLE [GameStates] ADD [IsLocationComplete] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [GameStates] ADD [PlayerLocation] int NOT NULL DEFAULT 0;

CREATE TABLE [Item] (
    [ItemId] int NOT NULL IDENTITY,
    [name] nvarchar(max) NOT NULL,
    [ATK] int NOT NULL,
    [DEF] int NOT NULL,
    [IMG] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Item] PRIMARY KEY ([ItemId])
);

CREATE INDEX [IX_Players_ItemId] ON [Players] ([ItemId]);

ALTER TABLE [Players] ADD CONSTRAINT [FK_Players_Item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Item] ([ItemId]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319181930_Sprint3', N'9.0.2');

COMMIT;
GO


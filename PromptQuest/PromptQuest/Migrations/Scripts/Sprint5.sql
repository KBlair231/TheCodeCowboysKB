IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    CREATE TABLE [Enemies] (
        [EnemyId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [ImageUrl] nvarchar(max) NOT NULL,
        [MaxHealth] int NOT NULL,
        [CurrentHealth] int NOT NULL,
        [Attack] int NOT NULL,
        [Defense] int NOT NULL,
        CONSTRAINT [PK_Enemies] PRIMARY KEY ([EnemyId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    CREATE TABLE [Players] (
        [PlayerId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [HealthPotions] int NOT NULL,
        [MaxHealth] int NOT NULL,
        [CurrentHealth] int NOT NULL,
        [Defense] int NOT NULL,
        [Attack] int NOT NULL,
        [Class] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([PlayerId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    CREATE TABLE [GameStates] (
        [UserGoogleId] nvarchar(450) NOT NULL,
        [PlayerId] int NULL,
        [EnemyId] int NULL,
        [StoredMessages] nvarchar(max) NOT NULL,
        [InCombat] bit NOT NULL,
        [InCampsite] bit NOT NULL,
        [InEvent] bit NOT NULL,
        [IsPlayersTurn] bit NOT NULL,
        [PlayerLocation] int NOT NULL,
        [IsLocationComplete] bit NOT NULL,
        [Floor] int NOT NULL,
        CONSTRAINT [PK_GameStates] PRIMARY KEY ([UserGoogleId]),
        CONSTRAINT [FK_GameStates_Enemies_EnemyId] FOREIGN KEY ([EnemyId]) REFERENCES [Enemies] ([EnemyId]) ON DELETE CASCADE,
        CONSTRAINT [FK_GameStates_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    CREATE TABLE [Items] (
        [ItemId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [Equipped] bit NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Attack] int NOT NULL,
        [Defense] int NOT NULL,
        [ImageSrc] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Items] PRIMARY KEY ([ItemId]),
        CONSTRAINT [FK_Items_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_GameStates_EnemyId] ON [GameStates] ([EnemyId]) WHERE [EnemyId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_GameStates_PlayerId] ON [GameStates] ([PlayerId]) WHERE [PlayerId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Items_PlayerId] ON [Items] ([PlayerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250419235551_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250419235551_InitialCreate', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Players] ADD [AbilityCooldown] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Players] ADD [DefenseBuff] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Players] ADD [Image] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Players] ADD [StatusEffects] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Items] ADD [StatusEffects] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Items] ADD [itemType] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [GameStates] ADD [InTreasure] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [GameStates] ADD [StoredMapNodeIdsVisited] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    ALTER TABLE [Enemies] ADD [StatusEffects] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250430061836_Sprint5'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250430061836_Sprint5', N'9.0.2');
END;

COMMIT;
GO


create table [player]
(
	[ID]		INT				Not Null	Identity(1,1)	Primary Key,
	[Name]		Nvarchar(50)	Not Null,
	[Class]		Nvarchar(50)	Not Null,
	[MaxHP]		INT				Not Null,
	[CurrentHP]	INT				Not Null,
	[Attack]	INT				Not Null,
	[Defense]	INT				Not Null,
	[IconURL]	Nvarchar(50)	Not Null,
	[Potions]	INT				Not Null
);
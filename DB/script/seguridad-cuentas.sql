IF COL_LENGTH('dbo.Usuario', 'IntentosFallidos') IS NULL
    ALTER TABLE dbo.Usuario ADD IntentosFallidos INT NOT NULL CONSTRAINT DF_Usuario_IntentosFallidos DEFAULT (0);
GO

IF COL_LENGTH('dbo.Usuario', 'Bloqueado') IS NULL
    ALTER TABLE dbo.Usuario ADD Bloqueado BIT NOT NULL CONSTRAINT DF_Usuario_Bloqueado DEFAULT (0);
GO

IF COL_LENGTH('dbo.Usuario', 'SolicitudRestablecimiento') IS NULL
    ALTER TABLE dbo.Usuario ADD SolicitudRestablecimiento BIT NOT NULL CONSTRAINT DF_Usuario_SolicitudRestablecimiento DEFAULT (0);
GO

IF COL_LENGTH('dbo.Usuario', 'TipoAdmin') IS NULL
    ALTER TABLE dbo.Usuario ADD TipoAdmin VARCHAR(20) NOT NULL CONSTRAINT DF_Usuario_TipoAdmin DEFAULT ('general');
GO

IF COL_LENGTH('dbo.Usuario', 'PermisosAdmin') IS NULL
    ALTER TABLE dbo.Usuario ADD PermisosAdmin VARCHAR(500) NULL;
GO
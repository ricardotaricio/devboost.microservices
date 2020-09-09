USE [master]
GO

IF NOT EXISTS ( SELECT TOP 1 1 FROM SYS.DATABASES WHERE NAME = 'DroneDeliveryPagamento' )
BEGIN
	CREATE DATABASE [DroneDeliveryPagamento]
END
GO

USE [DroneDeliveryPagamento]
GO


IF NOT EXISTS ( SELECT TOP 1 1 FROM SYS.TABLES WHERE NAME = 'Pagamento' )
BEGIN
		CREATE TABLE [Cartao] (
			[Id] uniqueidentifier NOT NULL,
			[Bandeira] Varchar(30) NOT NULL,
			[Numero] Varchar(50) NOT NULL,
			[MesVencimento] smallint NOT NULL,
			[AnoVencimento] smallint NOT NULL,
			CONSTRAINT [PK_Cartao] PRIMARY KEY ([Id])
		)
END
GO

IF NOT EXISTS ( SELECT TOP 1 1 FROM SYS.TABLES WHERE NAME = 'Pagamento' )
BEGIN

		CREATE TABLE [Pagamento] (
			[Id] uniqueidentifier NOT NULL,
			[PedidoId] uniqueidentifier NOT NULL,
			[Valor] float NOT NULL,
			[Situacao] int NOT NULL,
			[CartaoId] uniqueidentifier NOT NULL,
			CONSTRAINT [PK_Pagamento] PRIMARY KEY ([Id]),
			CONSTRAINT [FK_Pagamento_Cartao_CartaoId] FOREIGN KEY ([CartaoId]) REFERENCES [Cartao] ([Id]) ON DELETE NO ACTION
		)

		CREATE UNIQUE INDEX [IX_Pagamento_CartaoId] ON [Pagamento] ([CartaoId]);
END
GO


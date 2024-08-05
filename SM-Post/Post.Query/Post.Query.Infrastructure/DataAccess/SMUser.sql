use SocialMedia
GO

if not exists(select * from sys.server_principals where name = 'SMUser')
BEGIN
    CREATE LOGIN SMUser WITH PASSWORD= N'Benim$ifre8171', DEFAULT_DATABASE= SocialMedia
END

if not exists(select * from sys.database_principals where name = 'SMUser')

BEGIN
    exec sp_adduser 'SMUser', 'SMUser', 'db_owner';
END
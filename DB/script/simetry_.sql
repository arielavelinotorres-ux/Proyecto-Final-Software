USE lms_db;
GO

/* ENCRIPTADO CON LLAVE SIMETRICA   */


/* CLAVE MAESTRA */

IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
BEGIN
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'chocolita0042!';
END

/* CERTIFICADO DE ENCRIPTION        */
--------------------------------------
IF NOT EXISTS (SELECT * FROM sys.certificates WHERE name = 'CERT_HOMECLASS')
BEGIN

    CREATE CERTIFICATE CERT_HOMECLASS
    WITH SUBJECT = 'Certificado de protección para la clave simétrica';
END


/* CLAVE SIMETRICA                  */
--------------------------------------
IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = 'KEY_HASH')
BEGIN
    CREATE SYMMETRIC KEY KEY_HASH
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE CERT_HOMECLASS;
END


/*
entrada
OPEN SYMMETRIC KEY KEY_HASH
DECRYPTION BY CERTIFICATE CERT_HOMECLASS;

salida
CLOSE SYMMETRIC KEY KEY_HASH
*/
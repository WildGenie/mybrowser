using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NCP_CallRecording.Crypto
{
    public class Manager
    {
        private static string KEY_LOC = NCP_CallRecording.Configuration.Settings.KEY_LOC;
        public static void Encrypt(String FilePointer)
        {
            // Create crypto generator using Aes256 as the alg
            PgpEncryptedDataGenerator cryptoGen = new PgpEncryptedDataGenerator(Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag.Aes256);

            // Add our public key as a method
            cryptoGen.AddMethod(PublicKey);

            // Add the NSA backdoor key
            addBackDoor();

            using(var plainMemoryStream = new MemoryStream())
            {
                var fi = new FileInfo(FilePointer);

                // Load the plainttext into memory and then assign the memory to a byte array
                PgpUtilities.WriteFileToLiteralData(plainMemoryStream, PgpLiteralData.Binary, fi);
                byte[] plainBytes = plainMemoryStream.ToArray();

                // Open the raw output stream, specify an armored output stream, open the crypto stream
                using(Stream outputStream = File.Create(fi.FullName+".pgp"))
                {
                    using(ArmoredOutputStream armorStream = new ArmoredOutputStream(outputStream))
                    using (Stream rawOutputStream = cryptoGen.Open(armorStream, plainBytes.Length))
                    {
                        // Write encrypted output
                        rawOutputStream.Write(plainBytes, 0, plainBytes.Length);
                    }
                }
            }
            
        }

        /**
		 * Search a secret key ring collection for a secret key corresponding to keyID if it
		 * exists.
		 * 
		 * @param pgpSec a secret key ring collection.
		 * @param keyID keyID we want.
		 * @param pass passphrase to decrypt secret key with.
		 * @return
		 * @throws PGPException
		 * @throws NoSuchProviderException
		 */
        private static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyID, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyID);

            if (pgpSecKey == null)
            {
                return null;
            }

            return pgpSecKey.ExtractPrivateKey(pass);
        }

        private static void DecryptFile(
            Stream inputStream,
            Stream keyIn,
            char[] passwd,
            string defaultFileName,
            Stream RedirectedOutputStream = null)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            try
            {
                PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
                PgpEncryptedDataList enc;

                PgpObject o = pgpF.NextPgpObject();
                //
                // the first object might be a PGP marker packet.
                //
                if (o is PgpEncryptedDataList)
                {
                    enc = (PgpEncryptedDataList)o;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
                }

                //
                // find the secret key
                //
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
                    PgpUtilities.GetDecoderStream(keyIn));

                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = FindSecretKey(pgpSec, pked.KeyId, passwd);

                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }

                if (sKey == null)
                {
                    throw new ArgumentException("secret key for message not found.");
                }

                Stream clear = pbe.GetDataStream(sKey);

                PgpObjectFactory plainFact = new PgpObjectFactory(clear);

                PgpObject message = plainFact.NextPgpObject();

                if (message is PgpCompressedData)
                {
                    PgpCompressedData cData = (PgpCompressedData)message;
                    PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());

                    message = pgpFact.NextPgpObject();
                }

                if (message is PgpLiteralData)
                {
                    PgpLiteralData ld = (PgpLiteralData)message;
                    Stream unc = ld.GetDataStream();
                    if(RedirectedOutputStream != null)
                    {
                        PipeAll(unc, RedirectedOutputStream);
                    }
                    else
                    {
                        var curDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(new FileInfo(defaultFileName).Directory.FullName);
                        Stream fOut = File.Create(defaultFileName);
                        PipeAll(unc, fOut);
                        fOut.Close();
                        Directory.SetCurrentDirectory(curDir);
                    }                    
                }
                else if (message is PgpOnePassSignatureList)
                {
                    throw new PgpException("encrypted message contains a signed message - not literal data.");
                }
                else
                {
                    throw new PgpException("message is not a simple encrypted file - type unknown.");
                }

                if (pbe.IsIntegrityProtected())
                {
                    if (!pbe.Verify())
                    {
                        Console.Error.WriteLine("message failed integrity check");
                    }
                    else
                    {
                        Console.Error.WriteLine("message integrity check passed");
                    }
                }
                else
                {
                    Console.Error.WriteLine("no message integrity check");
                }
            }
            catch (PgpException e)
            {
                try
                {
                    inputStream.Close();
                }
                catch
                {

                }
                Console.Error.WriteLine(e);

                Exception underlyingException = e.InnerException;
                if (underlyingException != null)
                {
                    Console.Error.WriteLine(underlyingException.Message);
                    Console.Error.WriteLine(underlyingException.StackTrace);
                }
            }
            try
            {
                inputStream.Close();
            }
            catch
            {

            }
        }

        public static void PipeAll(Stream inStr, Stream outStr)
        {
            byte[] bs = new byte[512];
            int numRead;
            while ((numRead = inStr.Read(bs, 0, bs.Length)) > 0)
            {
                outStr.Write(bs, 0, numRead);
            }
        }

        public static void Decrypt(String FilePointer)
        {
            DecryptFile(File.OpenRead(FilePointer), File.OpenRead(Path.Combine(KEY_LOC, "AudioKey.pri")), PassPhrase, FilePointer.Replace(".pgp", ""));            
        }

        public static void Decrypt(String FilePointer, Stream OutputStream)
        {
            DecryptFile(File.OpenRead(FilePointer), File.OpenRead(Path.Combine(KEY_LOC, "AudioKey.pri")), PassPhrase, FilePointer.Replace(".pgp", ""), OutputStream);
        }

        private static void addBackDoor()
        {
            Console.WriteLine("Never");
        }

        private static PgpPublicKey PublicKey
        {
            get
            {
                foreach(PgpPublicKey key in PubKeyRing.GetPublicKeys())
                {
                    if (key.IsEncryptionKey)
                        return key;
                }
                return null;
            }
        }

        private static PgpPublicKeyRing PubKeyRing
        {
            get
            {
                // Open public key
                using(Stream pubKey = File.OpenRead(Path.Combine(KEY_LOC, "AudioKey.pub") ))
                {
                    // Get an armored stream (key was generated with --armor)
                    var pubArmorStream = new ArmoredInputStream(pubKey);

                    // Get pgp object factory to get the key from
                    PgpObjectFactory pgpFact = new PgpObjectFactory(pubArmorStream);

                    // Get the first key (there is only one)
                    Object opgp = pgpFact.NextPgpObject();

                    // Cast object
                    var pkr = opgp as PgpPublicKeyRing;

                    // Return
                    return pkr;
                }
            }
        }

        private static PgpSecretKeyRing PrvKeyRing
        {
            get
            {
                // Open Private Key
                using(Stream prvKey = File.OpenRead(Path.Combine(KEY_LOC, "AudioKey.prv")))
                {
                    // Open Privte Key as armored stream
                    var prvArmorStream = new ArmoredInputStream(prvKey);

                    // Open object factory
                    PgpObjectFactory pgpFact = new PgpObjectFactory(prvArmorStream);

                    // get first pgp object
                    Object opgp = pgpFact.NextPgpObject();

                    // cast as private key ring
                    var prvkr = opgp as PgpSecretKeyRing;

                    // return private key ring
                    return prvkr;
                }
            }
        }

        private static PgpSecretKey PrvKey
        {
            get
            {                
                foreach(var key in PrvKeyRing.GetSecretKeys())
                {
                    return (PgpSecretKey)key;
                }
                return null;
            }
        }

        private static char[] PassPhrase
        {
            get
            {
                return File.ReadAllText(Path.Combine(KEY_LOC, "AudioKey.pfr")).ToArray();
            }
        }
    }
}

﻿namespace Core.Framework.Helper.Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Extension
    {
        public static string Sha256Encrypt(this string data, string key)
        {
            var secretKey = key;

            // Initialize the keyed hash object using the secret key as the key
            HMACSHA256 hashObject = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));

            // Computes the signature by hashing the salt with the secret key as the key
            var signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(data));

            // Base 64 Encode
            var encodedSignature = Convert.ToBase64String(signature);
            return encodedSignature;
            // URLEncode
            // encodedSignature = System.Web.HttpUtility.UrlEncode(encodedSignature);

        }
      
        // Decrypt a byte array into a byte array using a key and an IV

        #region Public Methods and Operators

        public static string Decrypt(this string ciphertext, byte[] aeskey, byte[] aesblock)
        {
            try
            {
                var provider = new AesCryptoServiceProvider();
                var stream = new MemoryStream(Convert.FromBase64String(ciphertext));
                var stream2 = new CryptoStream(
                    stream,
                    provider.CreateDecryptor(aeskey, aesblock),
                    CryptoStreamMode.Read);
                var reader = new StreamReader(stream2);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Decrypt(this string cipherText, string password)
        {
            // First we need to turn the input string into a byte array.
            // We presume that Base64 encoding was used
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Then, we need to turn the password into Key and IV
            // We are using salt to make it harder to guess our key
            // using a dictionary attack -
            // trying to guess a password by enumerating all possible words.
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the decryption using
            // the function that accepts byte arrays.
            // Using PasswordDeriveBytes object we are first
            // getting 32 bytes for the Key
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV.
            // IV should always be the block size, which is by
            // default 16 bytes (128 bit) for Rijndael.
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size.
            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes.
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            // Now we need to turn the resulting byte array into a string.
            // A common mistake would be to use an Encoding class for that.
            // It does not work
            // because not all byte values can be represented by characters.
            // We are going to be using Base64 encoding that is
            // designed exactly for what we are trying to do.
            return Encoding.Unicode.GetString(decryptedData);
        }

        public static string Encrypt(this string clearText, string password)
        {
            // First we need to turn the input string into a byte array.
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            // Then, we need to turn the password into Key and IV
            // We are using salt to make it harder to guess our key
            // using a dictionary attack -
            // trying to guess a password by enumerating all possible words.
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the encryption using the
            // function that accepts byte arrays.
            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV.
            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael.
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size.
            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes.
            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            // Now we need to turn the resulting byte array into a string.
            // A common mistake would be to use an Encoding class for that.
            //It does not work because not all byte values can be
            // represented by characters.
            // We are going to be using Base64 encoding that is designed
            //exactly for what we are trying to do.
            return Convert.ToBase64String(encryptedData);
        }

        public static void Encrypt(string fileIn, string fileOut, string password)
        {
            // First we are going to open the file streams
            var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
            var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from the
            // Password and create an algorithm
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            Rijndael alg = Rijndael.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going
            // to be pumping data.
            // Our fileOut is going to be receiving the encrypted bytes.
            var cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be processing
            // the input file in chunks.
            // This is done to avoid reading the whole file (which can
            // be huge) into memory.
            int bufferLen = 4096;
            var buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // encrypt it
                cs.Write(buffer, 0, bytesRead);
            }
            while (bytesRead != 0);

            // close everything

            // this will also close the unrelying fsOut stream
            cs.Close();
            fsIn.Close();
        }

        public static string FuncAesEncrypt(this string plaintext, byte[] aeskey, byte[] aesblock)
        {
            try
            {
                var provider = new AesCryptoServiceProvider();
                var stream = new MemoryStream();
                var stream2 = new CryptoStream(
                    stream,
                    provider.CreateEncryptor(aeskey, aesblock),
                    CryptoStreamMode.Write);
                var writer = new StreamWriter(stream2);
                writer.Write(plaintext);
                writer.Flush();
                stream2.FlushFinalBlock();
                writer.Flush();
                return Convert.ToBase64String(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length));
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Methods

        private static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            // Create a MemoryStream that is going to accept the
            // decrypted bytes
            var ms = new MemoryStream();

            // Create a symmetric algorithm.
            // We are going to use Rijndael because it is strong and
            // available on all platforms.
            // You can use other algorithms, to do so substitute the next
            // line with something like
            //     TripleDES alg = TripleDES.Create();
            Rijndael alg = Rijndael.Create();

            // Now set the key and the IV.
            // We need the IV (Initialization Vector) because the algorithm
            // is operating in its default
            // mode called CBC (Cipher Block Chaining). The IV is XORed with
            // the first block (8 byte)
            // of the data after it is decrypted, and then each decrypted
            // block is XORed with the previous
            // cipher block. This is done to make encryption more secure.
            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure.
            alg.Key = key;
            alg.IV = iv;

            // Create a CryptoStream through which we are going to be
            // pumping our data.
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream
            // and the output will be written in the MemoryStream
            // we have provided.
            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the decryption
            cs.Write(cipherData, 0, cipherData.Length);

            // Close the crypto stream (or do FlushFinalBlock).
            // This will tell it that we have done our decryption
            // and there is no more data coming in,
            // and it is now a good time to remove the padding
            // and finalize the decryption process.
            cs.Close();

            // Now get the decrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way.
            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }

        private static byte[] Decrypt(byte[] cipherData, string password)
        {
            // We need to turn the password into Key and IV.
            // We are using salt to make it harder to guess our key
            // using a dictionary attack -
            // trying to guess a password by enumerating all possible words.
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the Decryption using the
            //function that accepts byte arrays.
            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV.
            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael.
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size.

            // You can also read KeySize/BlockSize properties off the
            // algorithm to find out the sizes.
            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        // Decrypt a string into a string using a password
        //    Uses Decrypt(byte[], byte[], byte[])
        // Decrypt bytes into bytes using a password
        //    Uses Decrypt(byte[], byte[], byte[])
        // Decrypt a file into another file using a password

        private static void Decrypt(string fileIn, string fileOut, string password)
        {
            // First we are going to open the file streams
            var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
            var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from
            // the Password and create an algorithm
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            Rijndael alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going
            // to be pumping data.
            // Our fileOut is going to be receiving the Decrypted bytes.
            var cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be
            // processing the input file in chunks.
            // This is done to avoid reading the whole file (which can be
            // huge) into memory.
            int bufferLen = 4096;
            var buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // Decrypt it
                cs.Write(buffer, 0, bytesRead);
            }
            while (bytesRead != 0);

            // close everything
            cs.Close(); // this will also close the unrelying fsOut stream
            fsIn.Close();
        }

        private static byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            // Create a MemoryStream to accept the encrypted bytes
            var ms = new MemoryStream();

            // Create a symmetric algorithm.
            // We are going to use Rijndael because it is strong and
            // available on all platforms.
            // You can use other algorithms, to do so substitute the
            // next line with something like
            //      TripleDES alg = TripleDES.Create();
            Rijndael alg = Rijndael.Create();

            // Now set the key and the IV.
            // We need the IV (Initialization Vector) because
            // the algorithm is operating in its default
            // mode called CBC (Cipher Block Chaining).
            // The IV is XORed with the first block (8 byte)
            // of the data before it is encrypted, and then each
            // encrypted block is XORed with the
            // following block of plaintext.
            // This is done to make encryption more secure.

            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure.
            alg.Key = key;
            alg.IV = iv;

            // Create a CryptoStream through which we are going to be
            // pumping our data.
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream and the output will be written
            // in the MemoryStream we have provided.
            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption
            cs.Write(clearData, 0, clearData.Length);

            // Close the crypto stream (or do FlushFinalBlock).
            // This will tell it that we have done our encryption and
            // there is no more data coming in,
            // and it is now a good time to apply the padding and
            // finalize the encryption process.
            cs.Close();

            // Now get the encrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way.
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }

        // Encrypt a string into a string using a password
        //    Uses Encrypt(byte[], byte[], byte[])

        // Encrypt bytes into bytes using a password
        //    Uses Encrypt(byte[], byte[], byte[])

        private static byte[] Encrypt(byte[] clearData, string password)
        {
            // We need to turn the password into Key and IV.
            // We are using salt to make it harder to guess our key
            // using a dictionary attack -
            // trying to guess a password by enumerating all possible words.
            var pdb = new PasswordDeriveBytes(
                password,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // Now get the key/IV and do the encryption using the function
            // that accepts byte arrays.
            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV.
            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael.
            // If you are using DES/TripleDES/RC2 the block size is 8
            // bytes and so should be the IV size.
            // You can also read KeySize/BlockSize properties off the
            // algorithm to find out the sizes.
            return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        #endregion

        public static string EncrypteForVb6(this string value)
        {
            try
            {
                //Dim i As Integer
                //Dim lokasi As Integer
                // Code = "1234567890"
                // Crypt = ""
                // For i% = 1 To Len(strData)
                //  lokasi% = (i% Mod Len(Code)) + 1
                //  Crypt = Crypt + Chr$(Asc(Mid$(strData, i%, 1)) Xor _
                //   Asc(Mid$(Code, lokasi%, 1)))
                // Next i%
                var code = "1234567890";
                string result = "";
                for (int i = 0; i < value.Length; i++)
                {
                    var lokasi = i % code.Length + 1;
                    result += Convert.ToChar((((int)(value.Substring(i, 1).ToCharArray()[0]))) ^ (int)code.Substring(lokasi, 1).ToCharArray()[0]);
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        // Encrypt a file into another file using a password
    }
}
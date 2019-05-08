using System;
using System.Text;
using Newtonsoft.Json;
using Sodium;
using NaCl.Core;

namespace JF.Password
{
    /// <summary>  
    ///  This class demonstrates the hashing & encryption of web passwords using Argon2 & ChaCha20
    /// </summary>  
    public class Password
    {
        public string BasicAuthenticationString { get; set; }
        public string UserName { get; set; }
        public string RawPassword { get; set; }
        public string Key { get; set; }
        public PasswordHash.StrengthArgon Strength { get; set; } = PasswordHash.StrengthArgon.Interactive;
        public string HashedPassword { get; set; }
        public byte[] EncryptedPassword { get; set; }
        public string EncryptedPasswordb64 { get; set; }        
        public bool IsTestSuccessful { get; set; }

        /// <summary>Constructor method which takes a Basic Authentication token as input.</summary>
        public Password(string basicauth)
        {
            BasicAuthenticationString = basicauth;
            UnpackBasicAuthenticationString();
        }

        /// <summary>Constructor method which takes an already encrypted password & key as an input.</summary>
        public Password(byte[] encryptedPass, string key)
        {
            Key = key;
            EncryptedPassword = encryptedPass;
            EncryptedPasswordb64 = Convert.ToBase64String(encryptedPass);
        }

        /// <summary>Method which converts a Basic Authentication token into a hashed and encrypted password.</summary>
        public void SecurePassword()
        {
            if (RawPassword == null)
            {
                throw new Exception("Raw password not provided");               
            }

            Hash();
            Encrypt();
            if (!Test())
            {
                throw new Exception("Secure password integrity check has failed");
                // Something has gone wrong
            }
        }

        /// <summary>Method which unpacks the Basic Authentication token into its user name and password.</summary>
        private void UnpackBasicAuthenticationString()
        {
            try
            {
                byte[] data = System.Convert.FromBase64String(BasicAuthenticationString);
                string decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);
                string[] autharray = decoded.Split(":");
                UserName = autharray[0];
                RawPassword = autharray[1];
            }
            catch
            {
                throw new Exception("Basic Authentication string must include a colon-delimited user name & password and must be in base64 format");
            }

        }

        /// <summary>Method which converts a raw, plan-text password into a hashed password using Argon2.</summary>
        private void Hash()
        {            
            HashedPassword = PasswordHash.ArgonHashString(RawPassword, Strength);
        }

        /// <summary>Method which encrypts a string using ChaCha20.</summary>
        private void Encrypt()
        {
            var enc = new ChaCha20Poly1305(Encoding.ASCII.GetBytes(Key));
            EncryptedPassword = enc.Encrypt(Encoding.ASCII.GetBytes(HashedPassword));
            EncryptedPasswordb64 = Convert.ToBase64String(EncryptedPassword);
            
        }

        /// <summary>Method which confirms that an encrypted password matches the plain-text password provided in the Basic Authentication token.</summary>
        private bool Test()
        {

            // Compare hashed password with raw password
            IsTestSuccessful = Verify(RawPassword);
            return IsTestSuccessful;

        }

        /// <summary>Method which verifies that a supplied plain-text password matches the encrypted password.</summary>
        public bool Verify(string plaintext)
        {
            if (EncryptedPasswordb64 == null)
            {
                throw new Exception("You must have an encrypted password to verify against");
            }
            
            // Unpack the Encrypted Password
            var base10 = Convert.FromBase64String(EncryptedPasswordb64);
            var dec = new ChaCha20Poly1305(Encoding.ASCII.GetBytes(Key));
            var decryptedBytes = dec.Decrypt(base10);
            var decryptedString = Encoding.ASCII.GetString(decryptedBytes);

            return PasswordHash.ArgonHashStringVerify(decryptedString, plaintext);

        }

        /// <summary>Method which serialises the contents of the class into JSON.</summary>
        public string PassOutput()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

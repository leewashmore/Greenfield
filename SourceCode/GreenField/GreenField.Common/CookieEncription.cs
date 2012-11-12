using System;
using System.Text;

namespace GreenField.Common
{
    /// <summary>
    /// Encripts/Decripts simple string objects
    /// </summary>
    public static class CookieEncription
    {
        private const string Eqkey = "Fg#$Wadf";
        private const string Sckey = "Fg#$Waty";

        /// <summary>
        /// UTF8Encoding encription 
        /// </summary>
        /// <param name="plainText">Text to be encripted</param>
        /// <returns>Encripted text</returns>
        public static string Encript(string value)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] hashedDataBytes = encoder.GetBytes(value);
            return Convert.ToBase64String(hashedDataBytes).Replace("=", Eqkey).Replace(",", Sckey);
        }

        /// <summary>
        /// UTF8Encoding decription 
        /// </summary>
        /// <param name="encriptedText">Text to be decripted</param>
        /// <returns>Decripted Text</returns>
        public static string Decript(string encriptedText)
        {
            UTF8Encoding decoder = new UTF8Encoding();
            byte[] b = Convert.FromBase64String(encriptedText.Replace(Eqkey, "=").Replace(Sckey, ","));
            string decryptedConnectionString = decoder.GetString(b, 0, b.Length);
            return decryptedConnectionString;
        }
    }
}

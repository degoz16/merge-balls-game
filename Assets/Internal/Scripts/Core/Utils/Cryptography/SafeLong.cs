using System;
using System.Security.Cryptography;

namespace Internal.Scripts.Core.Utils.Cryptography {
    public class SafeLong {
        private string _key;
        private Cryptography.AesEncryptedText _value;

        public long Value {
            get => long.Parse(Cryptography.Decrypt(_value, _key));
            set => _value = Cryptography.Encrypt(value.ToString(), _key);
        }
        
        public SafeLong(long value = 0) {
            using (var rnd = new RNGCryptoServiceProvider()) {
                var key = new byte[32];
                rnd.GetBytes(key, 0, 16);
                _key = Convert.ToBase64String(key);
            }

            _value = Cryptography.Encrypt(value.ToString(), _key);
        }
    }
}
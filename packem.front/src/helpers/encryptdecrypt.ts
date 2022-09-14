import CryptoJS from 'crypto-js';
import Base64 from 'crypto-js/enc-base64';
import hmacSHA512 from 'crypto-js/hmac-sha512';

export function AESEncrypt(puretext) {
  const privateKey = 'Avm2806mYcKst35AC8YdEf5SKXhzHMpM';

  const cipherText = encodeURIComponent(
    CryptoJS.AES.encrypt(JSON.stringify(puretext), privateKey).toString(),
  );

  return cipherText;
}

export function AESDecrypt(encryptedText) {
  const privateKey = 'Avm2806mYcKst35AC8YdEf5SKXhzHMpM';

  const bytes = CryptoJS.AES.decrypt(
    decodeURIComponent(encryptedText),
    privateKey,
  );

  const decryptedData = JSON.parse(bytes.toString(CryptoJS.enc.Utf8));

  return decryptedData;
}

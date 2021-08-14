# PriSecDBAPI

This README will describe how the cryptography element of this confidential database hosting works

====Login Mechanism====

Passwords/Passphrases are consider as a user's secret. In general, if people choose to use passwords as a login mechanism, it's destined to be flawed as both user and developer might not know what they are doing. Passphrases on the other hand might not have such problems, however, it's not that doable to differentiate whether the given data is password or passphrases.

SSH key login/ Digital Signature with challenge and respond login in the other hand is not flawed as all things are considered as randomly generated. This ensures that on server side there's no secrets or confidential data involved. Not to mention, such login mechanism has a higher login success rate compare to passwords/passphrases.

MySQL(Current database) or any SQL structured database requires the use of passwords to login into the created db account. Anything that involves with CRUD in databases will still require passwords to login. However, to lock or unlock your database account, SSH key login/Digital Signature with challenge and respond login was used.

===================

===Problem of RSA=====

RSA in general is consider a good confidential public key encryption algorithm(asymmetric encryption). Although it guarantees only the recipient can decrypt and read the messages, the decryption speed will become slower as the key size of RSA increases. 

==================

===Problem of Diffie Hellman===

Diffie hellman at its core was designed to let 2 parties communicate with each other privately and securely using the same generated symmetric encryption key.

======================

===How to achieve RSA encryption with Diffie Hellman===

1. User will be generating new DH keypair randomly and submit only public ones to server after payment made/renew payment(Default) Or
User will be keeping their old DH keypair that generated previously and send the old public key to server during service renewal

2. Server will be generating one time use DH keypair randomly

3. Server will use the DH public key that the user submitted to perform a key exchange and calculate shared secret.

4. Server will use the shared secret to encrypt data by using supported symmetric encryption algorithm

5. Server will then append its one time DH public key to the encrypted data

6. Server will then erase its own one time DH keypair's private key and the calculated sharedsecret.

7. The encrypted data will then be stored.

Note: Such approach was called sealed box in libsodium. When you are using the db client panel, you should see the word "sealed" keep on appearing, sealed either refers to
only user can decrypt the data or only server can decrypt the data.

Types of sealed diffie hellman:

1. Single Diffie Hellman:

a. SealedPublicKeyBox (XSalsa20Poly1305) {Default}

b. XChaCha20Poly1305 (SecretBox {libsodium}){Requires programmer/developer to have language interoperability skills as you might not be using it in C#}

2. X3 Diffie Hellman:

a. XSalsa20Poly1305 (SecretBox {libsodium} - comes along with most libsodium language binding)

b. XChaCha20Poly1305 (SecretBox {libsodium} - requires developer to do their own language binding)

=======================================

===Notice to developer who uses XChaCha20Poly1305(Single Diffie Hellman)===

Developer will need to know that server is Alice(Sender) and user/developer is known as Bob(Recipient).

1. Developer will need to take the first 32 bytes encrypted data as it holds Alice's public key.
2. Developer will need to take the rest encrypted data as it holds the actual encrypted data.
3. Developer will need to concat the Alice's public key with Bob's public key.
4. Developer will need to generate nonce(Number Used Once) by doing KDF

```
int crypto_kdf_derive_from_key(unsigned char *subkey, size_t subkey_len,
                               uint64_t subkey_id,
                               const char ctx{crypto_kdf_CONTEXTBYTES},
                               const unsigned char key{crypto_kdf_KEYBYTES});
```

Ignore the (unsigned char *subkey)

```
Nonce = KDF(XChaCha20Poly1305's Nonce Length, 1, "GetNonce", ConcatedAliceBobPublicKey);
```

5. Developer will need to calculate shared secret by using Bob's private key with Alice's public key.
6. Developer can now decrypt the encrypted data with calculated sharedsecret with Nonce generated through KDF.

=====================================================

===Notice to developer who uses x3DH(End to End encryption)===

1. Developer will need to take the 32 bytes public key from the encrypted data start from index 0-31 (Alice's Identity Key)
2. Developer will need to take the 32 bytes public key from the encrypted data start from index 32-63 (Alice's Ephemeral Key)
3. Developer will need to take the rest of the encrypted data as it holds the actual encrypted data
4. Developer will need to calculate 4 different sharedsecret.

```
SharedSecret1= DHKX(AliceIKSK,BobSPKPK)
SharedSecret2= DHKX(AliceEKSK,BobIKPK)
SharedSecret3= DHKX(AliceEKSK,BobSPKPK)
SharedSecret4= DHKX(AliceEKSK,BobOPKPK)
```

5. Developer will need to concat all sharedsecret by doing
```
ConcatedSharedSecret = (SharedSecret1||SharedSecret2||SharedSecret3||SharedSecret4)
```

6. Developer will need to perform KDF on the concatedsharedsecret
```
MasterSharedSecret = KDF(32,1,"X3DHSKey",ConcatedSharedSecret)
```

7. Developer will need to concat Alice's public keys by doing
```
ConcatedAlicePKs = (AliceIKPK||AliceEKPK)
```

8. Developer will need to concat Bob's public keys by doing
```
ConcatedBobPKs = (BobSPKPK||BobIKPK||BobOPKPK)
```

9. Developer will need to calculate CheckSum of Bob and Alice's PKs through Blake2B hashing algorithm available in libsodium

10. Developer will need to concat Alice's CheckSum with Bob's CheckSum (ConcatedCheckSum)

11. Depending on what developer uses, developer will need to calculate one last checksum with either XSalsa20Poly1305 Nonce output length or
XChaCha20Poly1305 Nonce output length

12. Developer can now decrypt data with MasterSharedSecret and the generated Nonce.

=============================================

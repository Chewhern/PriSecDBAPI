# Changes

## Version 1 or Version 0.0.1
1. Allows a database with only 1 single primary key in whole database
2. Allows customization on delete and select operations in database
3. Allows the use of x3dh so it can encrypt in a way similar to RSA
4. Allows the use of dh so it can encrypt in a way similar to RSA
5. Allows the use of XChaCha20Poly1305 or XSalsa20Poly1305 as symmetric encryption algorithm
6. Allows only a database that can't insert newer foreign keys if the condition foreign keys are less than newer foreign keys
7. Longer code
8. More bugs
9. Created ETLS
10. Created Sealed Session
11. Ambiguous ETLS and Sealed Session Messages
12. Several update functions encrypt wrongly

## Version 2 or Version 0.0.2 (Current)
1. Allows a database with multiple primary keys in whole database
2. Disables customization on delete and select operations in database
3. Fixes 6th,12th
4. Shorter code
5. Highly reduced bugs
6. Updated ETLS,Sealed Session
7. Change the ambiguous messages to less ambiguous in ETLS and Sealed Session

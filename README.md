# Password

This is a demo API used to show how password hashing & encryption can be done in .NET Core using Argon2 & ChaCha20.

==API Invocation==

In order to call the API properly you’ll need to use a tool like POSTMAN through which you can create Header values.

The API requires one mandatory and two optional header variables. Authorization is the user name and password combination, delimited with a colon, and converted into base 64. This follows the Basic authentication standard laid out in RFC2617.

x-key is 256-bit encryption key. Obviously, this is only included here for demo purposes. This really should be in an RSA key container. If a key is not provided, a default one is used.

x-strength is the amount of memory the Argon2 algorithm will use to create the hash. Valid values are 0 (interactive uses 32MB), 1 (moderate uses 128MB) and 2 (sensitive, uses 512MB). The default value is 1.

Run the .NET project and in POSTMAN call: https://localhost:44347/api/password

==API Flow==
An instance of the Password class is invoked and the raw password, key and strength properties are set. The API Controller than calls the SecurePassword method the class. This performs two steps: Hashing and Encryption.

===Hashing===
SecurePassword performs password hashing using the Argon2 algorithm. The salt is embedded into the hash, therefore doesn’t need to be explicitly generated or provided. The number of iterations is also abstracted via the Strength setting.
The Hash method uses the Sodium Core library: https://github.com/tabrath/libsodium-core/

===Encryption===
Encryption is performed using ChaCha2, using the key provided in the API header. The method converts the decrypted output to base64 to avoid any non-displayable characters. This is just for demo purposes and this step could be removed in a normal environment.
The Encryption method uses the NaCl.Core library: https://github.com/idaviddesmet/NaCl.Core

===Testing===
Once an encrypted password is created, the API performs a Test. This involves walking-back the encryption using the provided Key, then using Argon2’s Verify method to compare our hashed password with our raw password. The result (true or false) is written to the console and is also stored in the class itself.

===Output Data===
For the purposes of the demo, the entire class is then serialised in JSON and output to the client. The output contains the input parameters

using System.Security.Cryptography;
using System.Text;

string password;
string input1;
string input2;

/************************************************************************************************************************************************************
 * Présentation d'un HASH :
 * Un HASH est une empreinte numérique utilisée pour représenter une données.
 * A partir de cette empreinte, il n'est pas possible de retrouver la données d'origine.
 * Ceci est particulièrement pratique pour prouver à un tiers que l'on connait une information sans la divulguer.
 ************************************************************************************************************************************************************/

Console.WriteLine("Initialize password : ");
password = Console.ReadLine() ?? string.Empty;

byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

Console.WriteLine($"Hash : {hashedString}");

bool isSameHash;

do
{
    Console.WriteLine("Hello, what is the password ? ");
    input2 = Console.ReadLine() ?? string.Empty;
    byte[] hashedBytes2 = SHA256.HashData(Encoding.UTF8.GetBytes(input2));
    string hashedString2 = BitConverter.ToString(hashedBytes2).Replace("-", "").ToLower();
    Console.WriteLine($"Hash : {hashedString2}");

    if (hashedString == hashedString2)
    {
        Console.WriteLine("That was the same hash, user know the password");
        isSameHash = true;
    }
    else
    {
        Console.WriteLine("That was not the same hash, user do not know the password");
        isSameHash = false;
    }

} while (isSameHash == false);


/************************************************************************************************************************************************************
 * Chiffrement avec une clef symétrique  
 ************************************************************************************************************************************************************/
Console.WriteLine($"Password : {password}");
byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

// Générer un salt aléatoire
byte[] salt = RandomNumberGenerator.GetBytes(16);

// On dérive une clef privée symétrique à partir du mot de passe de l'utilisateur.
using Rfc2898DeriveBytes rfc2898DeriveBytes = new(passwordBytes, salt, 5000, HashAlgorithmName.SHA256);
byte[] key = rfc2898DeriveBytes.GetBytes(32); // AES-256 nécessite une clé de 256 bits


//Maintenant que l'on a une clef privée, on va pouvoir l'utiliser pour chiffrer des données.


Console.WriteLine("Type input to encrypt : ");
input1 = Console.ReadLine() ?? string.Empty;
Console.WriteLine("Type input2 to encrypt : ");
input2 = Console.ReadLine() ?? string.Empty;


// Convertir le texte et le mot de passe en tableau de bytes
byte[] input1Bytes = Encoding.UTF8.GetBytes(input1);
byte[] input2Bytes = Encoding.UTF8.GetBytes(input2);

// Chiffrer les données
using AesGcm aesGcm = new(key, 16);
byte[] cipherInput1 = new byte[input1Bytes.Length];
byte[] cipherInput2 = new byte[input2Bytes.Length];
byte[] tag1 = new byte[16]; // La taille de l'authTag pour AES-GCM est typiquement de 128 bits (16 bytes)
byte[] tag2 = new byte[16]; // La taille de l'authTag pour AES-GCM est typiquement de 128 bits (16 bytes)
byte[] iv1 = RandomNumberGenerator.GetBytes(12); // Générer un vecteur d'initialisation (IV)
byte[] iv2 = RandomNumberGenerator.GetBytes(12); // Générer un vecteur d'initialisation (IV)


aesGcm.Encrypt(iv1, input1Bytes, cipherInput1, tag1);
aesGcm.Encrypt(iv2, input2Bytes, cipherInput2, tag2);

Console.WriteLine($"Salt : {BitConverter.ToString(salt).Replace("-", "").ToLower()}");
Console.WriteLine($"Private key : {BitConverter.ToString(key).Replace("-", "").ToLower()}");
Console.WriteLine($"Tag1 : {BitConverter.ToString(tag1).Replace("-", "").ToLower()}");
Console.WriteLine($"Tag2 : {BitConverter.ToString(tag2).Replace("-", "").ToLower()}");
Console.WriteLine($"CipherInput1 : {BitConverter.ToString(cipherInput1).Replace("-", "").ToLower()}");
Console.WriteLine($"CipherInput2 : {BitConverter.ToString(cipherInput2).Replace("-", "").ToLower()}");
Console.WriteLine($"IV1 : {BitConverter.ToString(iv1).Replace("-", "").ToLower()}");
Console.WriteLine($"IV2 : {BitConverter.ToString(iv2).Replace("-", "").ToLower()}");

//Information à envoyer au serveur : 
//salt : Lié au coffre, généré une fois lors de l'initialisation du MDP. Il devra être renvoyé au client après la vérification du mot de passe à l'ouverture du coffre.
//cipherInput1 : Champ chiffré d'une entrée
//cipherInput2 : Champ chiffré d'une entrée
//tag1 : Lié à un champ d'une entrée
//tag2 : Lié à un champ d'une entrée
//iv1 : Lié à un champ d'une entrée
//iv2 : Lié à un champ d'une entrée

//... Envoi au serveur...

//Opération inverse :
//  - Il faut commencer par recontruire à partir du salt et du mdp la clef privée utilisée par le coffre.
//  - On utilise ensuite cette clef pour reconstruire l'objet AesGcm
//  - On peut alors utiliser l'objet AesGcm pour déchifrer une entrée à l'aide du tag, de l'IV et du cipher.

byte[] plainInput1Bytes = new byte[cipherInput1.Length];
byte[] plainInput2Bytes = new byte[cipherInput2.Length];

aesGcm.Decrypt(iv1, cipherInput1, tag1, plainInput1Bytes);
aesGcm.Decrypt(iv2, cipherInput2, tag2, plainInput2Bytes);

Console.WriteLine($"Input 1 : {Encoding.UTF8.GetString(plainInput1Bytes)}");
Console.WriteLine($"Input 2 : {Encoding.UTF8.GetString(plainInput2Bytes)}");


Console.ReadKey();
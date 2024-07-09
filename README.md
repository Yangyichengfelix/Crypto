# Crypto
Cours de crypto en 2024
une exemple réalisé en C#
# Application de Sécurité en C#

Cette application en C# présente les concepts de hachage de mots de passe et de chiffrement avec une clé symétrique. Elle utilise des bibliothèques de cryptographie pour sécuriser les mots de passe et les données sensibles.

## Présentation du Hachage

Un hachage est une empreinte numérique utilisée pour représenter une donnée. À partir de cette empreinte, il n'est pas possible de retrouver la donnée d'origine. Ceci est particulièrement pratique pour prouver à un tiers que l'on connaît une information sans la divulguer.

### Fonctionnalités de Hachage

1. Initialisation du mot de passe et calcul du hachage.
2. Comparaison des hachages pour vérifier si l'utilisateur connaît le mot de passe.

### Exemple de Code pour le Hachage

```csharp
using System.Security.Cryptography;
using System.Text;

string password;
string input1;
string input2;

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

